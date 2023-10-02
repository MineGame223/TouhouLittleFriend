

using Terraria;
using Terraria.Localization;

namespace TouhouPets
{
    public static class NumberToCNCharacter
    {
        public static string SetCNNumber_SingleDigits(int value)
        {
            return value switch
            {
                1 => "一",
                2 => "二",
                3 => "三",
                4 => "四",
                5 => "五",
                6 => "六",
                7 => "七",
                8 => "八",
                9 => "九",
                _ => "零",
            };
        }
        public static string SetCNNumber_TensDigits(int value)
        {
            return value switch
            {
                1 => "十",
                2 => "二十",
                3 => "三十",
                4 => "四十",
                5 => "五十",
                6 => "六十",
                7 => "七十",
                8 => "八十",
                9 => "九十",
                _ => "零",
            };
        }
        public static string SetCNNumber_HundredDigits(int value)
        {
            return value switch
            {
                1 => "一百",
                2 => "二百",
                3 => "三百",
                4 => "四百",
                5 => "五百",
                6 => "六百",
                7 => "七百",
                8 => "八百",
                9 => "九百",
                _ => "零",
            };
        }
        public static string GetCNNumber(int value)
        {
            int _single = value % 10;
            int _tens = value % 100 / 10;
            int _hundred = value % 1000 / 100;
            string singleD = SetCNNumber_SingleDigits(_single);
            string tensD = SetCNNumber_TensDigits(_tens);
            string hundredD = SetCNNumber_HundredDigits(_hundred);
            if (_single <= 0)
            {
                if (_hundred > 0 || _tens > 0)
                    singleD = string.Empty;
            }
            if (_tens <= 0)
            {
                if (_hundred > 0 && _single <= 0)
                    tensD = string.Empty;
            }
            if (_tens == 1 && _hundred > 0)
            {
                tensD = "一十";
            }
            if (_hundred <= 0)
            {
                hundredD = string.Empty;
            }

            string result = hundredD + tensD + singleD;

            if (_single + _tens + _hundred <= 0)
                result = "零";
            return result;
        }
        public static string GetNumberText(int value)
        {
            string result = value.ToString();
            if(Language.ActiveCulture.LegacyId== (int)GameCulture.CultureName.Chinese)
            {
                result = GetCNNumber(value);
            }
            return result;
        }
    }
}
