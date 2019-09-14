using LineDC.CEK;
using Microsoft.Azure.Functions.Extensions.DependencyInjection;

[assembly: FunctionsStartup(typeof(DiceRoller.Startup))]
namespace DiceRoller
{
    public class Startup : FunctionsStartup
    {
        public override void Configure(IFunctionsHostBuilder builder)
        {
            builder.Services.AddClova<ILoggableClova, DiceClova>();
        }
    }
}