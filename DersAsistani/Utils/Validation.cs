namespace DersAsistani.Utils
{
    public static class Validation
    {
        public static bool IsNonEmpty(string s)
        {
            return s != null && s.Trim().Length > 0;
        }

        public static bool IsValidUsername(string s)
        {
            if (!IsNonEmpty(s)) return false;
            // Basit kural: en az 3 karakter
            return s.Trim().Length >= 3;
        }

        public static bool IsValidPassword(string s)
        {
            if (!IsNonEmpty(s)) return false;
            // Basit kural: en az 6 karakter
            return s.Trim().Length >= 6;
        }

        public static bool IsValidFullName(string s)
        {
            if (!IsNonEmpty(s)) return false;
            // Basit kural: en az 3 karakter, boşluk içermesi tercih edilir
            return s.Trim().Length >= 3 && s.Contains(" ");
        }
    }
}