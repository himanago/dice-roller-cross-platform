using System;
using System.Text;

namespace DiceRoller
{
    public static class Dice
    {
        public static string ThrowDice(int count)
        {
            var random = new Random();
            var builder = new StringBuilder("結果は");
            var sum = 0;
            for (int i = 0; i < count; i++)
            {
                var val = random.Next(1, 6);
                sum += val;
                builder.AppendFormat("{0}、", val);
            }
            return builder.AppendFormat("で、合計は{0}でした。", sum).ToString();
        }
    }
}
