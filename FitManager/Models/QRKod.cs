namespace FitManager.Models
{
    public class QRKod
    {
        public int Id { get; set; }
        public string Kod { get; set; } = string.Empty;
        public DateTime DatumGenerisanja { get; set; } = DateTime.UtcNow;
        public bool Aktivan { get; set; } = true;
        public int ClanId { get; set; }
        public Korisnik Clan { get; set; } = null!;

        public void Deaktiviraj()
        {
            Aktivan = false;
        }
    }
}
