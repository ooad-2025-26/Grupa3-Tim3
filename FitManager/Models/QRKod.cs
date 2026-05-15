namespace FitManager.Models
{
    public class QRKod
    {
        public int Id { get; set; }
        public string Kod { get; set; } = string.Empty;
        public DateTime DatumGenerisanja { get; set; } = DateTime.UtcNow;
        public bool Aktivan { get; set; } = true;
        public string ClanId { get; set; }= string.Empty;
        public Korisnik Clan { get; set; } = null!;

        public void Deaktiviraj()
        {
            Aktivan = false;
        }
    }
}
