namespace FitManager.Models
{
    public class Korisnik
    {
        public int Id { get; set; }
        public UlogaKorisnika Uloga { get; set; }
        public string Email { get; set; } = string.Empty;
        public string KorisnickoIme { get; set; } = string.Empty;
        public string Ime { get; set; } = string.Empty;
        public string Prezime { get; set; } = string.Empty;
        public string Telefon { get; set; } = string.Empty;
        public DateTime DatumRodjenja { get; set; }
        public DateTime DatumRegistracije { get; set; } = DateTime.UtcNow;

        public QRKod? QRKod { get; set; }

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
