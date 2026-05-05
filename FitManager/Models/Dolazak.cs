namespace FitManager.Models
{
    public class Dolazak
    {
        public int Id { get; set; }
        public DateTime VrijemeDolaska { get; set; } = DateTime.UtcNow;
        public int ClanId { get; set; }
        public Korisnik Clan { get; set; } = null!;

        public bool RegistrovanNaDan(DateTime datum)
        {
            return VrijemeDolaska.Date == datum.Date;
        }
    }
}
