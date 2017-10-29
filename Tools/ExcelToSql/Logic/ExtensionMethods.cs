using System.Collections.Generic;
using System.Globalization;
using System.Text;

namespace ExcelToSql.Logic
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

        public static string Clean(this string text)
        {
            string cleanText = text.Trim().ToLower().RemoveDiacritics();

            byte[] asciiBytes = Encoding.ASCII.GetBytes(cleanText);
            byte[] cleanAsciiBytes = new byte[asciiBytes.Length];

            int position = 0;
            foreach (byte currentByte in asciiBytes)
            {
                if((currentByte >= 97 && currentByte <= 122)
                 || (currentByte >= 48 && currentByte <= 57))
                 {
                    cleanAsciiBytes[position++] = currentByte;
                }
                else
                {
                    cleanAsciiBytes[position++] = 95;
                }
            }

            return Encoding.ASCII.GetString(cleanAsciiBytes);
        }

        // https://stackoverflow.com/questions/249087/how-do-i-remove-diacritics-accents-from-a-string-in-net
        static string RemoveDiacritics(this string text)
        {
            var normalizedString = text.Normalize(NormalizationForm.FormD);
            var stringBuilder = new StringBuilder();

            foreach (var c in normalizedString)
            {
                var unicodeCategory = CharUnicodeInfo.GetUnicodeCategory(c);
                if (unicodeCategory != UnicodeCategory.NonSpacingMark)
                {
                    stringBuilder.Append(c);
                }
            }

            return stringBuilder.ToString().Normalize(NormalizationForm.FormC);
        }
    }
}
