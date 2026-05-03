using System.ComponentModel.DataAnnotations.Schema;

namespace FitManager.Models
{
    public enum UlogaKorisnika
    {
        CLAN,
        TRENER,
        ADMINISTRATOR
    }

    public enum TipClanarineNaziv
    {
        MJESECNA,
        KVARTALNA,
        GODISNJA
    }

    public enum StatusClanarine
    {
        AKTIVNA,
        ISTEKLA,
        OTKAZANA
    }

    public enum StatusRezervacije
    {
        AKTIVNA,
        OTKAZANA
    }

    public enum StatusObavjestenja
    {
        POSLANO,
        GRESKA,
        NA_CEKANJU
    }

    public enum TipTreninga
    {
        KARDIO,
        SNAGA,
        JOGA,
        PILATES
    }

    public enum TipIzvjestaja
    {
        PRIHODI,
        POSJETE,
        TRENINZI
    }

    public enum FitnessCilj
    {
        MRSAVLJENJE,
        JACANJE,
        IZDRZLJIVOST
    }

    public enum BmiKategorija
    {
        POTHRANJEN,
        NORMALAN,
        PREKOMJERAN,
        GOJAZAN
    }

    public enum Intenzitet
    {
        POCETNIK,
        SREDNJI,
        NAPREDNI
    }

    public abstract class Korisnik
    {
        public int Id { get; set; }
        public int? IdClanarine { get; set; }
        public string Email { get; set; } = string.Empty;
        public string KorisnickoIme { get; set; } = string.Empty;
        public UlogaKorisnika Uloga { get; set; }

        public void AzurirajEmail(string email)
        {
            Email = email;
        }
    }

    public class Clan : Korisnik
    {
        public Clan()
        {
            Uloga = UlogaKorisnika.CLAN;
            DatumRegistracije = DateTime.UtcNow;
        }

        public string Ime { get; set; } = string.Empty;
        public string Prezime { get; set; } = string.Empty;
        public string Telefon { get; set; } = string.Empty;
        public DateTime DatumRodjenja { get; set; }
        public DateTime DatumRegistracije { get; set; }

        public QRKod? QRKod { get; set; }
        public ICollection<Clanarina> Clanarine { get; set; } = new List<Clanarina>();
        public ICollection<Rezervacija> Rezervacije { get; set; } = new List<Rezervacija>();
        public ICollection<Dolazak> Dolasci { get; set; } = new List<Dolazak>();
        public ICollection<PlanTreninga> PlanoviTreninga { get; set; } = new List<PlanTreninga>();

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
    }

    public class Trener : Korisnik
    {
        public Trener()
        {
            Uloga = UlogaKorisnika.TRENER;
        }

        public string Ime { get; set; } = string.Empty;
        public string Prezime { get; set; } = string.Empty;
        public ICollection<GrupniTrening> GrupniTreninzi { get; set; } = new List<GrupniTrening>();

        public bool ImaPreklapanje(Termin termin)
        {
            return GrupniTreninzi.Any(trening => trening.PreklapaSeSa(termin));
        }
    }

    public class Administrator : Korisnik
    {
        public Administrator()
        {
            Uloga = UlogaKorisnika.ADMINISTRATOR;
        }

        public ICollection<Izvjestaj> Izvjestaji { get; set; } = new List<Izvjestaj>();

        public void KonfigurisiCijenu(TipClanarine tipClanarine, decimal cijena)
        {
            tipClanarine.PromijeniCijenu(cijena);
        }
    }

    public class QRKod
    {
        public int Id { get; set; }
        public string Kod { get; set; } = string.Empty;
        public DateTime DatumGenerisanja { get; set; } = DateTime.UtcNow;
        public bool Aktivan { get; set; } = true;
        public int ClanId { get; set; }
        public Clan Clan { get; set; } = null!;

        public void Deaktiviraj()
        {
            Aktivan = false;
        }
    }

    public class TipClanarine
    {
        public int Id { get; set; }
        public TipClanarineNaziv Naziv { get; set; }
        public int TrajanjeDana { get; set; }
        public decimal Cijena { get; set; }
        public ICollection<Clanarina> Clanarine { get; set; } = new List<Clanarina>();

        public void PromijeniCijenu(decimal cijena)
        {
            Cijena = cijena;
        }
    }

    public class Clanarina
    {
        public int Id { get; set; }
        public DateTime DatumPocetka { get; set; }
        public DateTime DatumIsteka { get; set; }
        public decimal Cijena { get; set; }
        public StatusClanarine Status { get; set; } = StatusClanarine.AKTIVNA;
        public bool ObavjestenjePoslano { get; set; }
        public int ClanId { get; set; }
        public Clan Clan { get; set; } = null!;
        public int TipClanarineId { get; set; }
        public TipClanarine TipClanarine { get; set; } = null!;
        public ICollection<EmailObavjestenje> EmailObavjestenja { get; set; } = new List<EmailObavjestenje>();

        public bool AktivnaNa(DateTime datum)
        {
            return Status == StatusClanarine.AKTIVNA && DatumPocetka <= datum && DatumIsteka >= datum;
        }

        public void OznaciObavijestena()
        {
            ObavjestenjePoslano = true;
        }

        public void Otkazi()
        {
            Status = StatusClanarine.OTKAZANA;
        }
    }

    public class GrupniTrening
    {
        public int Id { get; set; }
        public string Naziv { get; set; } = string.Empty;
        public string Opis { get; set; } = string.Empty;
        public int MaksKapacitet { get; set; }
        public int SlobodnaMjesta { get; set; }
        public DateTime DatumVrijeme { get; set; }
        public int Trajanje { get; set; }
        public TipTreninga TipTreninga { get; set; }
        public int TrenerId { get; set; }
        public Trener Trener { get; set; } = null!;
        public ICollection<Rezervacija> Rezervacije { get; set; } = new List<Rezervacija>();

        public bool ImaSlobodnihMjesta()
        {
            return SlobodnaMjesta > 0;
        }

        public void RezervisiMjesto()
        {
            if (!ImaSlobodnihMjesta())
            {
                return;
            }

            SlobodnaMjesta--;
        }

        public void OslobodiMjesto()
        {
            if (SlobodnaMjesta < MaksKapacitet)
            {
                SlobodnaMjesta++;
            }
        }

        public bool PreklapaSeSa(Termin termin)
        {
            var pocetak = DatumVrijeme;
            var kraj = DatumVrijeme.AddMinutes(Trajanje);
            var drugiPocetak = termin.DatumVrijeme;
            var drugiKraj = termin.DatumVrijeme.AddMinutes(termin.Trajanje);

            return pocetak < drugiKraj && drugiPocetak < kraj;
        }
    }

    public class Rezervacija
    {
        public int ClanId { get; set; }
        public Clan Clan { get; set; } = null!;
        public int GrupniTreningId { get; set; }
        public GrupniTrening GrupniTrening { get; set; } = null!;
        public DateTime DatumKreiranja { get; set; } = DateTime.UtcNow;
        public StatusRezervacije Status { get; set; } = StatusRezervacije.AKTIVNA;

        public bool Aktivna()
        {
            return Status == StatusRezervacije.AKTIVNA;
        }

        public void Otkazi(DateTime vrijeme)
        {
            Status = StatusRezervacije.OTKAZANA;
        }
    }

    public class Dolazak
    {
        public int Id { get; set; }
        public DateTime VrijemeDolaska { get; set; } = DateTime.UtcNow;
        public int ClanId { get; set; }
        public Clan Clan { get; set; } = null!;

        public bool RegistrovanNaDan(DateTime datum)
        {
            return VrijemeDolaska.Date == datum.Date;
        }
    }

    public class PlanTreninga
    {
        public int Id { get; set; }
        public FitnessCilj FitnessCilj { get; set; }
        public decimal Bmi { get; set; }
        public BmiKategorija BmiKategorija { get; set; }
        public Intenzitet Intenzitet { get; set; }
        public string SedmicniPlan { get; set; } = string.Empty;
        public DateTime DatumKreiranja { get; set; } = DateTime.UtcNow;
        public int ClanId { get; set; }
        public Clan Clan { get; set; } = null!;

        public void AzurirajIntenzitet(Intenzitet intenzitet)
        {
            Intenzitet = intenzitet;
        }
    }

    public class Izvjestaj
    {
        public int Id { get; set; }
        public TipIzvjestaja TipIzvjestaja { get; set; }
        public DateTime DatumOd { get; set; }
        public DateTime DatumDo { get; set; }
        public DateTime DatumGenerisan { get; set; } = DateTime.UtcNow;
        public string Sadrzaj { get; set; } = string.Empty;
        public int AdministratorId { get; set; }
        public Administrator Administrator { get; set; } = null!;

        public bool PokrivaPeriod(DateTime datum)
        {
            return DatumOd <= datum && DatumDo >= datum;
        }
    }

    public class EmailObavjestenje
    {
        public int Id { get; set; }
        public DateTime DatumSlanja { get; set; } = DateTime.UtcNow;
        public StatusObavjestenja Status { get; set; } = StatusObavjestenja.NA_CEKANJU;
        public string Sadrzaj { get; set; } = string.Empty;
        public int PokusajSlanja { get; set; }
        public int ClanarinaId { get; set; }
        public Clanarina Clanarina { get; set; } = null!;

        public void OznaciPoslano()
        {
            Status = StatusObavjestenja.POSLANO;
        }

        public void OznaciGresku()
        {
            Status = StatusObavjestenja.GRESKA;
            PokusajSlanja++;
        }
    }

    [NotMapped]
    public class ProfilClana
    {
        public string Ime { get; set; } = string.Empty;
        public string Prezime { get; set; } = string.Empty;
        public string Telefon { get; set; } = string.Empty;
        public DateTime DatumRodjenja { get; set; }
    }

    [NotMapped]
    public class Termin
    {
        public DateTime DatumVrijeme { get; set; }
        public int Trajanje { get; set; }
    }
}
