using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Azure.WebJobs.Host;

namespace AlaskaAir.Alexa
{
    public static class AskJennSkill
    {
        [FunctionName("AskJennSkill")]
        public static async Task<HttpResponseMessage> Run([HttpTrigger(AuthorizationLevel.Function, "get", "post", Route = null)]HttpRequestMessage req, TraceWriter log)
        {
            log.Info("C# HTTP trigger function processed a request.");

            log.Info($"Request={req}");
            var speechlet = new AskJennSpeechlet(log);
            var getResponse = await speechlet.GetResponseAsync(req);

            return getResponse;
        }
    }
}
