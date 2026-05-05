using System.ComponentModel.DataAnnotations.Schema;

namespace FitManager.Models
{
    [NotMapped]
    public class ProfilClana
    {
        public string Ime { get; set; } = string.Empty;
        public string Prezime { get; set; } = string.Empty;
        public string Telefon { get; set; } = string.Empty;
        public DateTime DatumRodjenja { get; set; }
    }
}
