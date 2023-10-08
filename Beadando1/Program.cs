using System.Globalization;
using System.IO;
using Beadando1.Models;
using CsvHelper;
using CsvHelper.Configuration;
//using System;
//using ConsoleTables;
//using System.Diagnostics;
//using System.IO;
//using System.Runtime.CompilerServices;

namespace Beadando {
    class Program
    {
        public static User argUser = new User(); // argumentumként megadott adatok alapján
        public static bool loginSuccess = false; // sikeres volt-e a belépés
        public static bool hasError = false; // csv beolvasásnál van-e hiba
        public static List<string> method = new List<string>(); // mit akarunk kiíratni (meg lehet adni több arg-ot pl --register --list ennék még sok értelme nincs, de lehet késöbb jó lesz valamire) 

        public static string? WorkDirUser { get; set; } // user.csv location
        public static string? WorkDirEntry { get; set; } // entry.csv location ugyan az a kettő csak a filenév változik

        // csv fileok tartalmai
        public static List<VaultEntry> vaultEntries = new List<VaultEntry>();
        public static List<User> users = new List<User>();

        public static User? loggedUser = null; // bejelentkezett felhasználó adatai, lehetne az argUser is, de így átláthatóbb sztem
        public static VaultEntry newVault = new VaultEntry(); // ha új vaultot akarunk hozzá adni

        static void Main(string[] args)
        {
            //Console.WriteLine(Directory.GetCurrentDirectory());
            WorkDirUser = Path.Combine("..", "..", "..", "..", "resources", "user.csv"); // default csv location 
            WorkDirEntry = Path.Combine("..", "..", "..", "..", "resources", "vault.csv");
            //Console.WriteLine(workDirUser);
            foreach (var arg in args) // argumentumok kezelése
            {
                switch (arg.Split("=")[0]) // --password=asd   [0] = "--password" | [1] = "asd"
                {
                    case "--workdir": // workdir path felülírása
                        WorkDirUser = Path.Combine(arg.Split("=")[1], "user.csv");
                        WorkDirEntry = Path.Combine(arg.Split("=")[1], "vault.csv");
                        break;
                    case "--register": // register parancs
                        method.Add("register");
                        break;
                    case "--list": // list parancs
                        method.Add("list");
                        break;
                    case "--username": // username mentése
                        argUser.Username = arg.Split("=")[1];
                        break;
                    case "--password": // jelszó mentése
                        argUser.Password = arg.Split("=")[1];
                        break;
                    case "--email": // Email mentése
                        // nem kéri MÉG a feladat, de ha vaultokat is akarunk savelni oda kell már:)
                        argUser.Email = arg.Split("=")[1];
                        break;
                    case "--firstname": // First name mentése
                        argUser.FirstName = arg.Split("=")[1];
                        break;
                    case "--lastname": // First name mentése
                        argUser.FirstName = arg.Split("=")[1];
                        break;
                    case "--add": // add to vault
                        method.Add("add");
                        break;
                    case "--website": // vault mentéshez
                        newVault.Website = arg.Split("=")[1];
                        break;
                    case "--webpwd":
                        newVault.Password = arg.Split("=")[1];
                        break;
                    case "--webuser":
                        newVault.Name = arg.Split("=")[1];
                        break;
                    case "--remove":
                        method.Add("remove");
                        break;


                }
            }

            // adatok beolvasása + path mentése
            VaultEntry.UsersPath = WorkDirUser;
            User.VaultPath = WorkDirEntry;
            ReadCsvUser(WorkDirUser);
            ReadCsvVault(WorkDirEntry);

            if (hasError)
            {
                // valami baj volt a beolvasás közben!
                return;
            }

            loginSuccess = DoLogin();

            // azért itt kerülnek megvalósításra a list / register mert így argként sorrendfüggetlen módon tudják megadni
            // nem kell először az username passwd és utána a method hanem kezdheti azzal is 
            foreach (var meth in method)
            {
                if (loginSuccess) // ha sikeres volt a login akkor tudja elérni a funkciókat amúgy csak reg!
                {
                    if (meth == "add")
                    {
                        // VaultEntryhez addja hozzá
                        AddToVault();
                    }
                    if(meth == "remove")
                    {
                        RemoveFromVault();
                    }
                    if (meth == "list")
                    {
                        DoList();
                    }
                }
                else
                {
                    if(meth == "register") {
                        DoRegister();
                    }
                }
            }

            Console.Write("\n");
            Console.ReadKey();
        }

        static bool DoLogin()
        {
            //EncryptedType currentPasswd = new EncryptedType("earth@world.com", "ezajelszo");
            //Console.WriteLine(currentPasswd.mainDecrypt());
            if (argUser.Username == null || argUser.Password == null)
            {
                Console.WriteLine("Felhasználónév / jelszó megadása kötelező!");
                return false;
            }
            var currentUser = users.Find(x => x.Username == argUser.Username); 
            if (currentUser == null)
            {
                return false;
            }
            EncryptedType argPasswd = new EncryptedType(currentUser.Email, argUser.Password);

            if (currentUser.Password == argPasswd.MainPasswordHash())
            {
                Console.WriteLine("Sikeres bejelentkezés.\n");
                loggedUser = currentUser;
                return true;
            }
            Console.WriteLine("Ezzel a felhasználó nincs még regisztrálva / Hibás jelszó!");
            return false;
        }

        static void DoList()
        { // jelszavak listázása
            Console.WriteLine("Mentett jelszavaid:\n");
            if(loggedUser == null || loggedUser.VaultEntries == null)
            {
                Console.WriteLine("Nincs még elmentett jelszavad!");
                return;
            }
            foreach (var vault in loggedUser.VaultEntries) // vault.name, vault.password, vault.website
            {
                Console.Write("Weboldal: ");
                Console.WriteLine(vault.Website);

                Console.Write("Felhasználónév: ");
                Console.WriteLine(vault.Name);

                Console.Write("Jelszó: ");
                EncryptedType encript = new EncryptedType(loggedUser.Email, vault.Password);
                Console.WriteLine(encript.Decrypt().Secret);
                Console.WriteLine("==============\n");
            }
        }

        static void AddToVault()
        {
            if (newVault.Name == null || newVault.Password == null || newVault.Website == null)
            {
                Console.WriteLine("Nem adtál meg minden argumentumot!");
                return;
            }
            using (StreamWriter writer = new(WorkDirEntry, append: true))
            {
                CsvConfiguration config = new(CultureInfo.InvariantCulture)
                {
                    HasHeaderRecord = false
                };
                using CsvWriter csv = new(writer, config);
                Console.WriteLine(loggedUser.Email);
                Console.WriteLine(newVault.Password);
                var encType = new EncryptedType(loggedUser.Email, newVault.Password);
                var encPasswd = encType.Encrypt().Secret; // kódolt jelszó
                newVault.Password = encPasswd;
                newVault.UserId = loggedUser.Username;
                csv.WriteRecords(new VaultEntry[] { newVault }); // hogy tegyen a végére sortörést is azért recordS
                Console.WriteLine("Sikeres regisztráció!");
                //Console.WriteLine(argUser.password);
            }
        }

        static void RemoveFromVault()
        {
            if (newVault.Name == null || newVault.Password == null || newVault.Website == null)
            {
                Console.WriteLine("Nem adtál meg minden argumentumot!");
                return;
            }
            vaultEntries.RemoveAll(entry => entry.UserId == loggedUser.Username && entry.Website == newVault.Website && entry.Name == newVault.Name);
            var nW = new VaultEntry();
            nW.UserId = "user_id";
            nW.Name = "username";
            nW.Password = "password";
            nW.Website = "website";
            vaultEntries.Insert(0, nW);

            using (StreamWriter writer = new(WorkDirEntry, append: false))
            {
                CsvConfiguration config = new(CultureInfo.InvariantCulture)
                {
                    HasHeaderRecord = false
                };
                using CsvWriter csv = new(writer, config);

                csv.WriteRecords(vaultEntries); // hogy tegyen a végére sortörést is azért recordS
                Console.WriteLine("Sikeres törlés!");

                //Console.WriteLine(argUser.password);
            }
        }

        static void DoRegister()
        {
            var currentUser = users.Find(x => x.Username == argUser.Username); 
            if(currentUser == null)
            {
                // ha null akkor kell csak reggelni
                using (StreamWriter writer = new(WorkDirUser, append: true))
                {
                    CsvConfiguration config = new(CultureInfo.InvariantCulture)
                    {
                        HasHeaderRecord = false
                    };
                    using CsvWriter csv = new(writer, config);
                    //TODO késöbb email kötelező megadni! anélkül nem tudjuk kódolni a jelszavakat!
                    var saveUser = argUser.ShallowCopy(); // nem akarom, hogy a beírt jelszó elvesszem, mert lehet ki kell íratni azt is majd a késöbbiekben
                    var encType = new EncryptedType("nem lényeg", saveUser.Password);
                    saveUser.Password = encType.MainPasswordHash(); // titkosítva mentse el
                    csv.WriteRecords(new User[] { saveUser }); // hogy tegyen a végére sortörést is azért recordS
                    Console.WriteLine("Sikeres regisztráció!");
                    //Console.WriteLine(argUser.password);
                }
                return;
            }
            Console.WriteLine("Van már ilyen felhasználóval regisztrálva!");
        }

        static void ReadCsvVault(string path) // vault.csv olvasás
        {
            try
            {
                using (StreamReader reader = new(path))
                {
                    using CsvReader csv = new(reader, CultureInfo.InvariantCulture);
                    vaultEntries = csv.GetRecords<VaultEntry>().ToList();
                }
            }
            catch (IOException)
            {
                hasError = true; // ha hiba van ne menjen tovább
                Console.WriteLine("Nem található a vault.csv file!");
            }
            
        }
        static void ReadCsvUser(string path) // user.csv olvasás
        {
            try
            {
                using (StreamReader reader = new(path))
                {
                    using CsvReader csv = new(reader, CultureInfo.InvariantCulture);
                    users = csv.GetRecords<User>().ToList();
                }
            }
            catch (IOException)
            {
                hasError = true; // ha hiba van ne menjen tovább
                Console.WriteLine("Nem található a user.csv file!");
            }
        }
    }
}