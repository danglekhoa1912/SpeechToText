using System;
using System.Globalization;
using System.Threading.Tasks;
using Microsoft.CognitiveServices.Speech;
using Microsoft.CognitiveServices.Speech.Audio;

namespace BaseService
{
    public class SpeechService
    {
        private SpeechConfig _config;
        private string wavFileName;

        public string WavFileName;
        public void setWavFileName(string name)
        {
            this.wavFileName = name;
        }

        public void SetConfig(string subscriptionKey, string region, string language)
        {
            _config = SpeechConfig.FromSubscription(subscriptionKey, region);
            _config.SpeechRecognitionLanguage = language;
        }

        public async Task<string> Start(TaskCompletionSource<int> source, Action<string> callback , bool isUseMic)
        {
            if(isUseMic)
            {
                using (var audioConfig = AudioConfig.FromDefaultMicrophoneInput())
                {
                    using (var recognizer = new SpeechRecognizer(_config, audioConfig))
                    {
                        await this.RunRecognizer(recognizer, source, callback).ConfigureAwait(false);
                    }
                }

                return "";
            }
            else
            {
                using (var audioInput = AudioConfig.FromWavFileInput(wavFileName))
                {
                    using (var recognizer = new SpeechRecognizer(_config, audioInput))
                    {
                        await this.RunRecognizer(recognizer, source, callback).ConfigureAwait(false);
                    }
                }
                return "";
            }

        }

        private async Task RunRecognizer(SpeechRecognizer recognizer, TaskCompletionSource<int> source, Action<string> callback)
        {
            //subscribe to events

            EventHandler<SpeechRecognitionEventArgs> recognizingHandler = (sender, e) => RecognizingEventHandler(e);
            EventHandler<SpeechRecognitionEventArgs> recognizedHandler = (sender, e) => RecognizedEventHandler(e, callback);
            EventHandler<SpeechRecognitionCanceledEventArgs> canceledHandler = (sender, e) => CanceledEventHandler(e, source);
            EventHandler<SessionEventArgs> sessionStartedHandler = (sender, e) => SessionStartedEventHandler(e);
            EventHandler<SessionEventArgs> sessionStoppedHandler = (sender, e) => SessionStoppedEventHandler(e, source);
            EventHandler<RecognitionEventArgs> speechStartDetectedHandler = (sender, e) => SpeechDetectedEventHandler(e, "start");
            EventHandler<RecognitionEventArgs> speechEndDetectedHandler = (sender, e) => SpeechDetectedEventHandler(e, "end");

            recognizer.Recognizing += recognizingHandler;
            recognizer.Recognized += recognizedHandler;
            recognizer.Canceled += canceledHandler;
            recognizer.SessionStarted += sessionStartedHandler;
            recognizer.SessionStopped += sessionStoppedHandler;
            recognizer.SpeechStartDetected -= speechStartDetectedHandler;
            recognizer.SpeechEndDetected -= speechEndDetectedHandler;

            //start,wait,stop recognition
            await recognizer.StartContinuousRecognitionAsync().ConfigureAwait(false);
            await source.Task.ConfigureAwait(false);
            await recognizer.StopContinuousRecognitionAsync().ConfigureAwait(false);

            // unsubscribe from events
            recognizer.Recognizing -= recognizingHandler;
            recognizer.Recognized -= recognizedHandler;
            recognizer.Canceled -= canceledHandler;
            recognizer.SessionStarted -= sessionStartedHandler;
            recognizer.SessionStopped -= sessionStoppedHandler;
            recognizer.SpeechStartDetected -= speechStartDetectedHandler;
            recognizer.SpeechEndDetected -= speechEndDetectedHandler;
        }

        #region Recognition Event Handlers

        /// <summary>
        /// Logs intermediate recognition results
        /// </summary>
        private void RecognizingEventHandler(SpeechRecognitionEventArgs e)
        {
            var result = e.Result.Text;
        }

        /// <summary>
        /// Logs the final recognition result
        /// </summary>
        private void RecognizedEventHandler(SpeechRecognitionEventArgs e, Action<string> callback)
        {
            callback(e.Result.Text.ProcessingContent());

            // if access to the JSON is needed it can be obtained from Properties
            string json = e.Result.Properties.GetProperty(PropertyId.SpeechServiceResponse_JsonResult);
        }

        /// <summary>
        /// Logs Canceled events
        /// And sets the TaskCompletionSource to 0, in order to trigger Recognition Stop
        /// </summary>
        private void CanceledEventHandler(SpeechRecognitionCanceledEventArgs e, TaskCompletionSource<int> source)
        {
            source.TrySetResult(0);
        }

        /// <summary>
        /// Session started event handler.
        /// </summary>
        private void SessionStartedEventHandler(SessionEventArgs e)
        {
        }

        /// <summary>
        /// Session stopped event handler. Set the TaskCompletionSource to 0, in order to trigger Recognition Stop
        /// </summary>
        private void SessionStoppedEventHandler(SessionEventArgs e, TaskCompletionSource<int> source)
        {
            source.TrySetResult(0);
        }

        private void SpeechDetectedEventHandler(RecognitionEventArgs e, string eventType)
        {

        }

        #endregion
    }
}
