namespace FitManager.Models
{
    public class Dolazak
    {
        public int Id { get; set; }
        public DateTime VrijemeDolaska { get; set; } = DateTime.UtcNow;
        public string ClanId { get; set; } = string.Empty;
        public Korisnik Clan { get; set; } = null!;

        public bool RegistrovanNaDan(DateTime datum)
        {
            return VrijemeDolaska.Date == datum.Date;
        }
    }
}
