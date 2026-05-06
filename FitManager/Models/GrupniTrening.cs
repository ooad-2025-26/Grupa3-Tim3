namespace FitManager.Models
{
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
        public Korisnik Trener { get; set; } = null!;

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
}
