using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace DDStudy2022.Common
{
    public static class HashHelper
    {
        public static string GetHash(string input)
        {
            using var hashingAlgorithm = SHA256.Create();
            
            var data = hashingAlgorithm.ComputeHash(Encoding.UTF8.GetBytes(input));
            var sb = new StringBuilder();

            foreach (var item in data)
                sb.Append(item.ToString("x2"));
            return sb.ToString();
        }

        public static bool Verify(string input, string hash)
        {
            var hashInput = GetHash(input);
            var comaprer = StringComparer.OrdinalIgnoreCase;
            return comaprer.Compare(hashInput, hash) == 0;
        }
    }
}
