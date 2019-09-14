using LineDC.CEK;
using Microsoft.Extensions.Logging;

namespace DiceRoller
{
    // IClovaを拡張
    public interface ILoggableClova : IClova
    {
        // ロガーを受け取るプロパティ
        ILogger Logger { get; set; }
    }
}