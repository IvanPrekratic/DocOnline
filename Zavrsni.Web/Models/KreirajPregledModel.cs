using Zavrsni.Model;

namespace Zavrsni.Web.Models
{
    public class KreirajPregledModel
    {
        public Pregled Pregled { get; set; }
        public DateTime DatumPregleda { get; set; }
        public TimeSpan VrijemePregleda { get; set; }

    }
}
