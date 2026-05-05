namespace FitManager.Models
{
    public class Korisnik
    {
        public int Id { get; set; }
        public int? IdClanarine { get; set; }
        public UlogaKorisnika Uloga { get; set; }
        public string Email { get; set; } = string.Empty;
        public string KorisnickoIme { get; set; } = string.Empty;
        public string Ime { get; set; } = string.Empty;
        public string Prezime { get; set; } = string.Empty;
        public string Telefon { get; set; } = string.Empty;
        public DateTime DatumRodjenja { get; set; }
        public DateTime DatumRegistracije { get; set; } = DateTime.UtcNow;

        public QRKod? QRKod { get; set; }
        public ICollection<Clanarina> Clanarine { get; set; } = new List<Clanarina>();
        public ICollection<Rezervacija> Rezervacije { get; set; } = new List<Rezervacija>();
        public ICollection<Dolazak> Dolasci { get; set; } = new List<Dolazak>();
        public ICollection<PlanTreninga> PlanoviTreninga { get; set; } = new List<PlanTreninga>();
        public ICollection<GrupniTrening> GrupniTreninzi { get; set; } = new List<GrupniTrening>();
        public ICollection<Izvjestaj> Izvjestaji { get; set; } = new List<Izvjestaj>();

        public void AzurirajEmail(string email)
        {
            Email = email;
        }

        public bool ImaAktivnuClanarinu()
        {
            return Clanarine.Any(clanarina => clanarina.AktivnaNa(DateTime.UtcNow));
        }

        public void AzurirajProfil(ProfilClana podaci)
        {
            Ime = podaci.Ime;
            Prezime = podaci.Prezime;
            Telefon = podaci.Telefon;
            DatumRodjenja = podaci.DatumRodjenja;
        }

        public bool ImaPreklapanje(Termin termin)
        {
            return GrupniTreninzi.Any(trening => trening.PreklapaSeSa(termin));
        }

        public void KonfigurisiCijenu(TipClanarine tipClanarine, decimal cijena)
        {
            tipClanarine.PromijeniCijenu(cijena);
        }
    }
}
