namespace FitManager.Models
{
    public class TipClanarine
    {
        public int Id { get; set; }
        public TipClanarineNaziv Naziv { get; set; }
        public int TrajanjeDana { get; set; }
        public decimal Cijena { get; set; }

        public void PromijeniCijenu(decimal cijena)
        {
            Cijena = cijena;
        }
    }
}
