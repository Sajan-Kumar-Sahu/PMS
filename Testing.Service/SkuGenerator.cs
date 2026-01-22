using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pms.Service
{
    public static class SkuGenerator
    {
     
       
            private const string Prefix = "PROD-";
            private const int PadLength = 3;

            public static string GenerateNextSku(string? lastSku)
            {
                if (string.IsNullOrWhiteSpace(lastSku))
                {
                    return $"{Prefix}{1.ToString().PadLeft(PadLength, '0')}";
                }

                var numericPart = lastSku.Replace(Prefix, "");

                if (!int.TryParse(numericPart, out int lastNumber))
                {
                    return "Invalid SKU format found in database.";
                }

                var nextNumber = lastNumber + 1;

                return $"{Prefix}{nextNumber.ToString().PadLeft(PadLength, '0')}";
            }
        }
}
