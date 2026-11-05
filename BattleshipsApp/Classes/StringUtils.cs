using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Text;

namespace BattleshipsApp.Classes
{
    public static class StringUtils
    {
        public static bool IsEmpty(this string text)
        {
            return string.IsNullOrEmpty(text);
        }
    }
}
