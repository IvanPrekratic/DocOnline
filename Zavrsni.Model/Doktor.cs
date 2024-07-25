using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Zavrsni.Model
{
    public class Doktor
    {
        [Key]
        public int DoktorID { get; set; }
        [Required(ErrorMessage = "Polje ne smije biti prazno")]
        public string Ime { get; set; }
        [Required(ErrorMessage = "Polje ne smije biti prazno")]
        public string Prezime { get; set; }
        [Required(ErrorMessage = "Polje ne smije biti prazno")]
        [ForeignKey(nameof(Specijalizacija))]
        public int SpecijalizacijaID { get; set; }
        public Specijalizacija Specijalizacija { get; set; }
        [EmailAddress(ErrorMessage = "Nevažeća email adresa")]
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
        public string Titula { get; set; }
        [Required(ErrorMessage = "Polje ne smije biti prazno")]
        public string PocetakRadnogVremena { get; set; }
        [Required(ErrorMessage = "Polje ne smije biti prazno")]
        public string KrajRadnogVremena { get; set; }
        public virtual ICollection<Pacijent>? Pacijenti { get; set; }
        public string ImePrezime => $"{Ime} {Prezime}";
    }
}
