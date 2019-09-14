using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using LineDC.CEK;
using LineDC.CEK.Models;

namespace DiceRoller
{
    public class DiceClova : ClovaBase, ILoggableClova
    {
        public ILogger Logger { get; set; }

        protected override Task OnLaunchRequestAsync(Session session, CancellationToken cancellationToken)
        {
            // Azure Functions標準のロガーが使用できる
            Logger.LogInformation("LaunchRequest");

            Response
                .AddText("サイコロをいくつ振りますか？")
                .AddRepromptText("サイコロを振る個数を言ってください。")
                .KeepListening();

            return Task.CompletedTask;
        }

        protected override Task OnIntentRequestAsync(Intent intent, Session session, CancellationToken cancellationToken)
        {
            // Azure Functions標準のロガーが使用できる
            Logger.LogInformation("IntentRequest");

            switch (intent.Name)
            {
                case "ThrowDiceIntent":
                    if (intent.Slots.TryGetValue("diceCount", out var slot) &&
                        int.TryParse(slot.Value, out var count))
                    {
                        Response.AddText(Dice.ThrowDice(count));
                    }
                    else
                    {
                        Response
                            .AddText("個数がわかりませんでした。もう一度言ってください。")
                            .AddRepromptText("サイコロを振る個数を言ってください。")
                            .KeepListening();
                    }
                    break;

                case "Clova.CancelIntent":
                    Response.AddText("終了します。");
                    break;

                case "Clova.GuideIntent":
                default:
                    Response
                        .AddText("好きな数のサイコロを振って、合計をしゃべります。いくつ振りますか？")
                        .AddRepromptText("サイコロを振る個数を言ってください。")
                        .KeepListening();
                    break;
            }
            return Task.CompletedTask;
        }
    }
}
