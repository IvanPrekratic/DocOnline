using System.ComponentModel.DataAnnotations;

namespace Zavrsni.Model
{
    public class Specijalizacija
    {
        [Key]
        public int SpecijalizacijaID { get; set; }
        [Required]
        public string Naziv { get; set; }
    }
}
