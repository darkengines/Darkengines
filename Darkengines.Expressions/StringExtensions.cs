using Microsoft.EntityFrameworkCore;
using System.Security.Cryptography;
using System.Text;

namespace Darkengines.Expressions {
    public static class StringExtensions {
        public static string ToPascalCase(this string @string) {
            return $"{char.ToUpper(@string[0])}{@string.Substring(1)}";
        }
        public static string ToCamelCase(this string @string) {
            return $"{char.ToLower(@string[0])}{@string.Substring(1)}";
        }
        public static byte[] ToLowerInvariantSHA256(this string @string) {
            return ToSHA256(@string.ToLowerInvariant());
        }
        public static byte[] ToSHA256(this string @string) {
            byte[] hashedValue = null;
            using (var sha256 = SHA256.Create()) {
                hashedValue = sha256.ComputeHash(Encoding.UTF8.GetBytes(@string));
            }
            return hashedValue;
        }
        public static bool Like(this string @string, string pattern) {
            return EF.Functions.Like(@string, pattern);
        }
    }
}
