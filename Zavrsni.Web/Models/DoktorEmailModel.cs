namespace Zavrsni.Web.Models
{
    public class DoktorEmailModel
    {
        public int DoktorID { get; set; }
        public int PacijentID { get; set; }
        public int PregledID { get; set; }
        public string EmailPoruka { get; set; }

    }
}
