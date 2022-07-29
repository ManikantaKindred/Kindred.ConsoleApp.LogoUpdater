using Kindred.ConsoleApp.LogoUpdater.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Text.Json;
using Dapper;
using System.Data;
using System.Data.SqlClient;
using System.Linq;

namespace Kindred.ConsoleApp.LogoUpdater
{
    internal class Program
    {
        private const string CONNECTION_STRING = "Add Connection string";
        static void Main(string[] args)
        {
            var deals = ReadNetworkData();

            var brands = GetBrands();

            Console.WriteLine($"No of logos missing in DB: {brands.Count}");

            var logoUpdates = new List<UpdateLogo>();

            foreach (var brand in brands)
            {
                var logo = deals.NetworkBrands.Where(x => x.Id == brand.MerchantId).FirstOrDefault()?.Logo;
                if (logo != null)
                {
                    logoUpdates.Add(new UpdateLogo
                    {
                        Id = brand.Id,
                        Logo = logo,
                    });
                }
                else
                {
                    //There may be some deals present in DB but not in imported data so I found below url pattern matching for all logos in Coupons.de.
                    //I could use this without looking at imported data but just want to minimize the errorness if there is a wrong patteren present if incase.
                    var missingLogoUrl = $"https://www.coupons.de/images/merchant_logos/mer_logo_{brand.MerchantId}.jpg";
                    logoUpdates.Add(new UpdateLogo
                    {
                        Id = brand.Id,
                        Logo = missingLogoUrl,
                    });
                }
            }

            if (logoUpdates.Any())
            {
                Console.WriteLine(JsonSerializer.Serialize(logoUpdates));
                Console.WriteLine($"No of logos updating: {logoUpdates.Count}");
                UpdateBrandLogos(logoUpdates);
            }

        }


        public static List<Brand> GetBrands()
        {
            var query = @$"SELECT b.id as ID, b.logo as Logo, c.merchant_id as MerchantId
                            FROM campaign c
                            INNER JOIN brand b
                            ON c.brand_id = b.id
                            WHERE c.source='coupons4U' and b.logo is NULL
                            order by merchant_id asc";

            var brands = new List<Brand>();

            using (SqlConnection connection = new SqlConnection(CONNECTION_STRING))
            {
                var brandsData = connection.Query<Brand>(query);
                brands.AddRange(brandsData);
            }

            return brands;
        }

        public static void UpdateBrandLogos(List<UpdateLogo> brands)
        {
            var query = "UPDATE Brand SET logo = @Logo WHERE Id = @Id;";

            using (SqlConnection connection = new SqlConnection(CONNECTION_STRING))
            {
                int RowsAffected = connection.Execute(query, brands);
            }

        }

        public static NetworkData ReadNetworkData()
        {
            string path = Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), @"Data\ImportData.json");
            return Read<NetworkData>(path);
        }


        private static T Read<T>(string filePath)
        {
            string text = File.ReadAllText(filePath);
            return JsonSerializer.Deserialize<T>(text);
        }

    }
}
