//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;
//using System.Threading.Tasks;
using CsvHelper;
using CsvHelper.Configuration.Attributes;
using System.Globalization;


namespace Beadando1.Models
{
    public class User
    {
        public static string? VaultPath { get; set; }

        public User ShallowCopy() => (User)this.MemberwiseClone();

        [Name("username")]
        public string? Username { get; set; }

        [Name("password")]
        public string? Password { get; set; }

        [Name("email")]
        public string? Email { get; set; }

        [Name("firstname")]
        public string? FirstName { get; set; }

        [Name("lastname")]
        public string? LastName { get; set; }

        [Ignore]
        public List<VaultEntry>? VaultEntries
        {
            get
            {
                if (VaultPath == null) return null;
                using StreamReader reader = new(VaultPath);
                using CsvReader csv = new(
                    reader, CultureInfo.InvariantCulture);
                return csv.GetRecords<VaultEntry>()
                    .Where(us => us.UserId == Username).ToList();
            }
        }

    }
}
