namespace FitManager.Models
{
    public class Rezervacija
    {
        public int Id { get; set; }
        public int ClanId { get; set; }
        public Korisnik Clan { get; set; } = null!;
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
}
