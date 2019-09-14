using Alexa.NET;
using Alexa.NET.Request;
using Alexa.NET.Request.Type;
using Alexa.NET.Response;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System.Threading.Tasks;

namespace DiceRoller
{
    public static class AlexaEndpoint
    {
        [FunctionName(nameof(AlexaEndpoint))]
        public static async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Function, "get", "post", Route = null)] HttpRequest req,
            ILogger log)
        {
            var request = JsonConvert.DeserializeObject<SkillRequest>(await req.ReadAsStringAsync());

            // レスポンス用の変数を宣言
            SkillResponse response = null;

            // 共通のRepromptを用意
            var reprompt = new Reprompt("サイコロを振る個数を言ってください。");

            switch (request.Request)
            {
                case LaunchRequest lr:
                    response = ResponseBuilder.Ask(
                        "サイコロをいくつ振りますか？", reprompt);
                    break;

                case IntentRequest ir:
                    switch (ir.Intent.Name)
                    {
                        case "ThrowDiceIntent":
                            // 回数指定のインテントならスロットから回数を取得＆数値変換
                            // C# 7.0以降は out var でこういった記述が楽にできる
                            if (ir.Intent.Slots.TryGetValue("diceCount", out var slot) &&
                                int.TryParse(slot.Value, out var count))
                            {
                                response = ResponseBuilder.Tell(Dice.ThrowDice(count));
                            }
                            else
                            {
                                response = ResponseBuilder.Ask(
                                    "個数がわかりませんでした。もう一度言ってください。", reprompt);
                            }
                            break;

                        case BuiltInIntent.Cancel:
                            response = ResponseBuilder.Tell("終了します。");
                            break;

                        case BuiltInIntent.Help:
                        default:
                            response = ResponseBuilder.Ask(
                                "好きな数のサイコロを振って、合計をしゃべります。いくつ振りますか？", reprompt);
                            break;
                    }
                    break;

                default:
                    response = ResponseBuilder.Tell(
                        "すみません。わかりませんでした。最初からやり直してください。");
                    break;
            }

            return new OkObjectResult(response);
        }
    }
}
