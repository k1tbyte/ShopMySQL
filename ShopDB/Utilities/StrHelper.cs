using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Linq;
using System.Windows.Forms;

namespace ShopDB.Utilities
{
    internal enum PassType : byte
    {
        Empty,
        SmallLen,
        EmptyOrOnlySpaces,
        Control,
        OnlyOneCase,
        NoDigit,
        OK,
    }

    internal static class StrHelper
    {
        internal static bool IsNullOrWhiteSpaceCollection(List<string> collection)
        {
            foreach (var item in collection)
            {
                if (String.IsNullOrEmpty(item) || item.Contains(' ')) return true;
            }
            return false;
        }

        internal static bool IsSpecialContain(string Str, char[] chars)
        {
            foreach (var item in Str)
            {
                for (int i = 0; i < chars.Length; i++)
                {
                    if (item == chars[i]) return true;
                }
            }
            return false;
        }

        internal static PassType IsNormalizePassword(string password)
        {
            bool IsUpperContain = false,IsLowerContain = false, IsDigitContain = false;

            if (String.IsNullOrWhiteSpace(password))
                return PassType.EmptyOrOnlySpaces;
            else if (password.Length < 8)
                return PassType.SmallLen;

            foreach (var item in password)
            {
                if (char.IsControl(item))
                    return PassType.Control;
                else if (char.IsDigit(item) && !IsDigitContain)
                    IsDigitContain = true;
                else if (char.IsLower(item) && !IsLowerContain)
                    IsLowerContain = true;
                else if (char.IsUpper(item) && !IsUpperContain)
                    IsUpperContain = true;
            }

            if (!IsLowerContain || !IsUpperContain)
                return PassType.OnlyOneCase;
            if (!IsDigitContain)
                return PassType.NoDigit;


            return PassType.OK;
        }
    }
}
