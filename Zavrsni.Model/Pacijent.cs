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
        [Required(ErrorMessage = "Polje ne smije biti prazno")]
        public string Ime { get; set; }
        [Required(ErrorMessage = "Polje ne smije biti prazno")]
        public string Prezime { get; set; }
        [EmailAddress(ErrorMessage = "Invalid Email Address")]
        [Required(ErrorMessage = "Polje ne smije biti prazno")]
        public string Email { get; set; }
        [Required(ErrorMessage = "Polje ne smije biti prazno")]
        public string Telefon { get; set; }
        [Required(ErrorMessage = "Polje ne smije biti prazno")]
        public string Adresa { get; set; }
        [Required(ErrorMessage = "Polje ne smije biti prazno")]
        public string Grad { get; set; }
        [Required(ErrorMessage = "Polje ne smije biti prazno")]
        public string Drzava { get; set; }
        [Required(ErrorMessage = "Polje ne smije biti prazno")]
        public string JMBG { get; set; }
        [Required(ErrorMessage = "Polje ne smije biti prazno")]
        public string KorisnickoIme { get; set; }
        [Required(ErrorMessage = "Polje ne smije biti prazno")]
        public DateTime DatumRodjenja { get; set; }
        public virtual ICollection<Doktor>? Doktori { get; set; }
        public string ImePrezime => $"{Ime} {Prezime}";
    }
}
