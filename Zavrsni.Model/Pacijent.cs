using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Zavrsni.Model
{
    public class Pacijent
    {
        [Key]
        public int PacijentID { get; set; }
        [Required]
        public string Ime { get; set; }
        [Required]
        public string Prezime { get; set; }
        [Required]
        public string Email { get; set; }
        [Required]
        public string Telefon { get; set; }
        [Required]
        public string Adresa { get; set; }
        [Required]
        public string Grad { get; set; }
        [Required]
        public string Drzava { get; set; }
        [Required]
        public string JMBG { get; set; }
        [Required]
        public string KorisnickoIme { get; set; }
        [Required]
        public DateTime DatumRodjenja { get; set; }
        public virtual ICollection<Doktor>? Doktori { get; set; }
        public string ImePrezime => $"{Ime} {Prezime}";
    }
}
