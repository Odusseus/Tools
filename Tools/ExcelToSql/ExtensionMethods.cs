using System;

namespace ExcelToSql
{
    public static class ExtensionMethods
    {
        public static int RoundUp(this int toRound)
        {
            int roundOff = (10 - toRound % 10) + toRound;
            if( roundOff < 10)
            {
                roundOff = 10;
            }

            return roundOff;
        }
    }
}
