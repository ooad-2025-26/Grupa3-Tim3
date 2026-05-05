namespace FitManager.Models
{
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
}
