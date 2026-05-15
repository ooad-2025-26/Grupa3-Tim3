using Microsoft.AspNetCore.Identity;

namespace FitManager.Models
{
    public class Korisnik : IdentityUser
    {
        public UlogaKorisnika Uloga { get; set; }

        public string Ime { get; set; } = string.Empty;

        public string Prezime { get; set; } = string.Empty;

        public DateTime DatumRodjenja { get; set; }

        public DateTime DatumRegistracije { get; set; } = DateTime.UtcNow;

        public QRKod? QRKod { get; set; }

        public string KorisnickoIme
        {
            get => UserName!;
            set => UserName = value;
        }

        public string Telefon
        {
            get => PhoneNumber!;
            set => PhoneNumber = value;
        }

        public void AzurirajEmail(string email)
        {
            Email = email;
        }

        public void AzurirajProfil(ProfilClana podaci)
        {
            Ime = podaci.Ime;
            Prezime = podaci.Prezime;
            Telefon = podaci.Telefon;
            DatumRodjenja = podaci.DatumRodjenja;
        }

        public void KonfigurisiCijenu(TipClanarine tipClanarine, decimal cijena)
        {
            tipClanarine.PromijeniCijenu(cijena);
        }
    }
}