using CsvHelper;
using CsvHelper.Configuration.Attributes;
using System.Globalization;
//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;
//using System.Threading.Tasks;

namespace Beadando1.Models
{
    public class VaultEntry
    {
        public static string? UsersPath { get; set; }


        [Name("user_id")]
        public string? UserId { get; set; }

        [Name("username")]
        public string? Name { get; set; }

        [Name("password")]
        public string? Password { get; set; }

        [Name("website")]
        public string? Website { get; set; }

        [Ignore]
        public User? User
        {
            get
            {
                if (UsersPath == null) return null;
                using StreamReader reader = new(UsersPath);
                using CsvReader csv = new(
                    reader, CultureInfo.InvariantCulture);
                return csv.GetRecords<User>()
                    .Where(us => us.Username == UserId)
                    .FirstOrDefault();
            }
        }
    }
}
