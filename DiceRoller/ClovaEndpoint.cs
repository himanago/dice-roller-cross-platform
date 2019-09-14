using LineDC.CEK;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Extensions.Logging;
using System.Threading.Tasks;

namespace DiceRoller
{
    public class ClovaEndpoint
    {
        private ILoggableClova Clova { get; }

        public ClovaEndpoint(ILoggableClova clova)
        {
            // DIで具体的な実装が外部から挿入される
            Clova = clova;
        }

        [FunctionName(nameof(ClovaEndpoint))]
        public async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Function, "get", "post",
                Route = null)] HttpRequest req,
            ILogger log)
        {
            // ロガーをセット
            Clova.Logger = log;

            var response = await Clova.RespondAsync(
                req.Headers["SignatureCEK"], req.Body);
            return new OkObjectResult(response);
        }
    }
}
