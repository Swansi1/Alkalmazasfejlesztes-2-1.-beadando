CSV path
Ha console-ból dotnet run-nal indítjuk az alkalmazást akkor a futtatási Dir 
**'C:\Users\Patrik\Desktop\beadando1\Beadando1\Beadando1'**

viszont ha Visual Studioban futtatjuk akkor
**'C:\Users\Patrik\Desktop\beadando1\Beadando1\Beadando1\bin\Debug\net6.0'**

Default workDir (ha nincs megadva argumentumként) a Visual Studios futtatáshoz lett beállítva.

**További funkciók**
- User class-ba is került egy *List<VaultEntry> vaultEntries*, így egyszerűbben lehet lekérni a jelszavakat.
- Meg lehet adni, emailt, firstname, lastname-t argumentumként is. (Késöbb az email úgy is fontos lesz majd gondolom)

**Használata**
*Argumentumok sorrendjei felcserélhetőek*
- username, password megadása kötelező!
- Regisztráció:
```dotnet run --workdir=<workdir> --register --username=<username> --password=<passwd>```
- Listázás:
```dotnet run --workdir=<workdir> --list --username=<username> --password=<passwd>```


felhasználók: 
earth  | passwd: ezaleszo
fire   | passwd: ezajelszo1
sky    | passwd: ezajelszo123
