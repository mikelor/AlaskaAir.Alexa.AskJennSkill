using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using AlexaSkillsKit.Speechlet;
using AlexaSkillsKit.UI;
using AlexaSkillsKit.Slu;
using Microsoft.Azure.WebJobs.Host;

using AlmeApiSdk;
using AlmeApiSdk.Models.Conversation.Converse;

namespace AlaskaAir.Alexa
{
    class AskJennSpeechlet : SpeechletAsync
    {
        #region Properties
        public TraceWriter Logger { get; set; }
        #endregion

        #region Constructor
        public AskJennSpeechlet(TraceWriter log)
        {
            Logger = log;
        }
        #endregion

        #region Public Overrides
        public override async Task OnSessionStartedAsync(SessionStartedRequest request, Session session)
        {
            Logger.Info($"OnSessionStarted requestId={request.RequestId}, sessionId={session.SessionId}");
        }

        public override async Task OnSessionEndedAsync(SessionEndedRequest request, Session session)
        {
            Logger.Info($"OnSessionStarted requestId={request.RequestId}, sessionId={session.SessionId}");
        }

        public override async Task<SpeechletResponse> OnLaunchAsync(LaunchRequest request, Session session)
        {
            Logger.Info($"OnSessionStarted requestId={request.RequestId}, sessionId={session.SessionId}");
            return await GetWelcomeResponseAsync();
        }

        public override async Task<SpeechletResponse> OnIntentAsync(IntentRequest request, Session session)
        {
            // Get intent from the request object.
            Intent intent = request.Intent;
            string intentName = (intent != null) ? intent.Name : null;

            Logger.Info($"OnIntent intentName={intentName} requestId={request.RequestId}, sessionId={session.SessionId}");


            // Note: If the session is started with an intent, no welcome message will be rendered;
            // rather, the intent specific response will be returned.
            switch (intentName)
            {
                case "AskJennIntent":
                    return await BuildAskJennResponseAsync(intent, session);

                case "FlightStatusIntent":
                    return BuildFlightStatusAsync(intent, session);
                default:
                    throw new SpeechletException("Invalid Intent");
            }
        }
        #endregion

        #region Private Methods
        private async Task<SpeechletResponse> GetWelcomeResponseAsync()
        {
            // Create the welcome message.
            string speechOutput = "Welcome to the Alaska Airlines Ask Jenn Skill. Say something like AskJenn What about bag fees?";

            // Here we are setting shouldEndSession to false to not end the session and
            // prompt the user for input
            return BuildSpeechletResponse("Welcome", speechOutput, false);
        }
        private SpeechletResponse BuildSpeechletResponse(string title, string output, bool shouldEndSession)
        {
            // Create the Simple card content.
            SimpleCard card = new SimpleCard();
            card.Title = title;
            card.Content = output;

            // Create the plain text output.
            PlainTextOutputSpeech speech = new PlainTextOutputSpeech();
            speech.Text = output;

            // Create the speechlet response.
            SpeechletResponse response = new SpeechletResponse();
            response.ShouldEndSession = shouldEndSession;
            response.OutputSpeech = speech;
            response.Card = card;

            return response;
        }

        private async Task<SpeechletResponse> BuildAskJennResponseAsync(Intent intent, Session session)
        {
            var almeClient = new AlmeClient(new Uri("https://askjenn.alaskaair.com/"));

            // Setup the request
            ConverseRequest req = new ConverseRequest();
            req.channel = "Console";
            req.origin = "Typed";
            req.parameters = new ConverseRequestParameters();

            Slot question = intent.Slots["Question"];
            Logger.Info($"Question={question.Value.ToString()}");
            req.question = question.Value.ToString();

            // Call the Converse endpoint
            var res = await almeClient.ConverseAsync(req);

            return BuildSpeechletResponse(intent.Name, res.text, false);
        }

        private SpeechletResponse BuildFlightStatusAsync(Intent intent, Session session)
        {
            return BuildSpeechletResponse(intent.Name, @"test", false);
        }
        #endregion
    }
}
