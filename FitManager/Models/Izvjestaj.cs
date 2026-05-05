namespace FitManager.Models
{
    public class Izvjestaj
    {
        public int Id { get; set; }
        public TipIzvjestaja TipIzvjestaja { get; set; }
        public DateTime DatumOd { get; set; }
        public DateTime DatumDo { get; set; }
        public DateTime DatumGenerisan { get; set; } = DateTime.UtcNow;
        public string Sadrzaj { get; set; } = string.Empty;
        public int AdministratorId { get; set; }
        public Korisnik Administrator { get; set; } = null!;

        public bool PokrivaPeriod(DateTime datum)
        {
            return DatumOd <= datum && DatumDo >= datum;
        }
    }
}
