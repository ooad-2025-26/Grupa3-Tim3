namespace FitManager.Models
{
    public class Clanarina
    {
        public int Id { get; set; }
        public DateTime DatumPocetka { get; set; }
        public DateTime DatumIsteka { get; set; }
        public decimal Cijena { get; set; }
        public StatusClanarine Status { get; set; } = StatusClanarine.AKTIVNA;
        public bool ObavjestenjePoslano { get; set; }
        public int ClanId { get; set; }
        public Korisnik Clan { get; set; } = null!;
        public int TipClanarineId { get; set; }
        public TipClanarine TipClanarine { get; set; } = null!;

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
}
