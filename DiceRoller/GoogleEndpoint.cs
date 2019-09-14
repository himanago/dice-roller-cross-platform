using Google.Cloud.Dialogflow.V2;
using Google.Protobuf;
using Google.Protobuf.WellKnownTypes;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Extensions.Logging;
using System.Threading.Tasks;

namespace DiceRoller
{
    public static class GoogleEndpoint
    {
        [FunctionName(nameof(GoogleEndpoint))]
        public static async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Function, "get", "post", Route = null)] HttpRequest req,
            ILogger log)
        {
            var parser = new JsonParser(JsonParser.Settings.Default.WithIgnoreUnknownFields(true));
            var webhookRequest = parser.Parse<WebhookRequest>(await req.ReadAsStringAsync());
            var webhookResponse = new WebhookResponse();

            switch (webhookRequest.QueryResult.Intent.DisplayName)
            {
                case "Default Welcome Intent":
                    webhookResponse.FulfillmentText = "サイコロをいくつ振りますか？";
                    break;

                case "ThrowDiceIntent":
                    if (webhookRequest.QueryResult.Parameters.Fields.TryGetValue("diceCount", out var entity))
                    {
                        // 結果メッセージ
                        webhookResponse.FulfillmentText = Dice.ThrowDice((int)entity.NumberValue);

                        // 対話を終了させる
                        webhookResponse.Payload = new Struct
                        {
                            Fields =
                            {
                                {
                                    "google", Value.ForStruct(new Struct
                                    {
                                        Fields = { { "expectUserResponse", Value.ForBool(false) } }
                                    })
                                }
                            }
                        };
                    }
                    break;

                case "Default Fallback Intent":
                default:
                    webhookResponse.FulfillmentText =
                        "好きな数のサイコロを振って、合計をしゃべります。いくつ振りますか？";
                    break;
            }

            return new OkObjectResult(webhookResponse.ToString());
        }
    }
}
