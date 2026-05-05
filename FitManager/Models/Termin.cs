using System.ComponentModel.DataAnnotations.Schema;

namespace FitManager.Models
{

    [NotMapped]
    public class Termin
    {
        public DateTime DatumVrijeme { get; set; }
        public int Trajanje { get; set; }
    }
}
