using Ozeki.Media;

using System;
using System.Threading.Tasks;
using System.Threading;
using System.Linq;
using Google.Cloud.Speech.V1;

namespace BaseService
{
    public partial class GoogleSTT : AudioReceiver
    {
        SpeechClient speech;
        SpeechClient.StreamingRecognizeStream streamingCall;

        Task printResponses;

        AudioFormat _format;

        private string _languageCode;

        private GoogleLanguage _language;
        public GoogleLanguage Language
        {
            get { return _language; }
            set
            {
                _language = value;
                _languageCode = _language.GetCode();
            }
        }

        //public GoogleSTT(string languageCode)
        //    : this(GoogleLanguageExt.GetGoogleLanguageFromCode(languageCode),
        //            new AudioFormat())
        //{ }

        public GoogleSTT(GoogleLanguage languageCode, AudioFormat format, Action<string> callback)
        {
            Language = languageCode;

            SetReceiveFormats(format);

            _format = format;

            Init(callback);
        }

        private void Init(Action<string> callback)
        {
            System.Environment.SetEnvironmentVariable("GOOGLE_APPLICATION_CREDENTIALS", @".\nckh-speechtotext-344006-314d7682f67d.json");

            speech = SpeechClient.Create();

            streamingCall = speech.StreamingRecognize();

            streamingCall.WriteAsync(
               new StreamingRecognizeRequest()
               {
                   StreamingConfig = new StreamingRecognitionConfig()
                   {
                       Config = new RecognitionConfig()
                       {
                           Encoding =
                           RecognitionConfig.Types.AudioEncoding.Linear16,
                           SampleRateHertz = 16000,
                           LanguageCode = LanguageCodes.Vietnamese.Vietnam,
                       },
                       InterimResults = true,
                   }
               });

             printResponses = Task.Run(async () =>
            {
                while (await streamingCall.GrpcCall.ResponseStream.MoveNext(
                    default(CancellationToken)))
                {
                    foreach (var result in streamingCall.GrpcCall.ResponseStream
                        .Current.Results)
                    {
                        Console.WriteLine(result);
                        if (result.IsFinal)
                        {
                            var top =
                            result.Alternatives.OrderBy(x => x.Confidence).First();

                            Console.WriteLine(top.Transcript);
                            callback(top.Transcript);
                        }
                            
                        }
                }
            });

        }

        object writeLock = new object();

        public bool IsRunning { get; private set; }

        public void Stop()
        {
            IsRunning = false;
        }

        public void Start()
        {
            IsRunning = true;
        }

        protected override void OnDataReceived(object sender, AudioData data)
        {
            if (!IsRunning) return;

            lock (writeLock)
            {
                var request = new StreamingRecognizeRequest();
                request.AudioContent = Google.Protobuf.ByteString
                            .CopyFrom(data.Data, 0, data.Data.Length);

                try
                {
                    streamingCall.WriteAsync(request).Wait();
                }
                catch (Exception e)
                {
                    streamingCall.WriteCompleteAsync();
                    //Init();
                }
            }
        }

        protected override void Dispose(bool disposing)
        {
            Stop();

            if (printResponses != null)
            {
                printResponses = null;
            }

            if (streamingCall != null)
            {
                streamingCall = null;
            }

            if (speech != null)
            {
                speech = null;
            }

            base.Dispose(disposing);
        }
    }
}