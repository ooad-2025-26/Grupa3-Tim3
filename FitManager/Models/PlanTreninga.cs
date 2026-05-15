namespace FitManager.Models
{
    public class PlanTreninga
    {
        public int Id { get; set; }
        public FitnessCilj FitnessCilj { get; set; }
        public decimal Bmi { get; set; }
        public BmiKategorija BmiKategorija { get; set; }
        public Intenzitet Intenzitet { get; set; }
        public string SedmicniPlan { get; set; } = string.Empty;
        public DateTime DatumKreiranja { get; set; } = DateTime.UtcNow;
        public string ClanId { get; set; } = string.Empty;
        public Korisnik Clan { get; set; } = null!;

        public void AzurirajIntenzitet(Intenzitet intenzitet)
        {
            Intenzitet = intenzitet;
        }
    }
}
