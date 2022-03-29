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

        public GoogleSTT(GoogleLanguage languageCode, AudioFormat format, Action<string> callback,bool isMic,String filePath=null)
        {
            Language = languageCode;

            SetReceiveFormats(format);

            _format = format;

            IsMic = isMic;

            Init(callback,filePath);
        }

        private void Init(Action<string> callback,String filePath)
        {
            System.Environment.SetEnvironmentVariable("GOOGLE_APPLICATION_CREDENTIALS", @".\nckh-speechtotext-344006-314d7682f67d.json");

            speech = SpeechClient.Create();
            if (IsMic)
            {
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
                            if (result.IsFinal)
                            {
                                var top =
                                result.Alternatives.OrderBy(x => x.Confidence).First();
                                callback(top.Transcript);
                            }

                        }
                    }
                });
            }
            else
            {
                var config = new RecognitionConfig
                {
                    Encoding = RecognitionConfig.Types.AudioEncoding.Flac,
                    SampleRateHertz = 24000,
                    LanguageCode = LanguageCodes.Vietnamese.Vietnam,
                };
                var audio = RecognitionAudio.FromFile(filePath);
                var response = speech.Recognize(config, audio);
                foreach (var result in response.Results)
                {
                    foreach (var alternative in result.Alternatives)
                    {
                        callback( alternative.Transcript);
                    }
                }
            }

        }

        object writeLock = new object();

        public bool IsRunning { get; private set; }
        public bool IsMic { get; private set; }

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
            if (!IsRunning||!IsMic) return;

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