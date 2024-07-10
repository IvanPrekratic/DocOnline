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
        [Required]
        public string Ime { get; set; }
        [Required]
        public string Prezime { get; set; }
        [Required]
        [ForeignKey(nameof(Specijalizacija))]
        public int SpecijalizacijaID { get; set; }
        public Specijalizacija Specijalizacija { get; set; }
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
        public string Titula { get; set; }
        [Required]
        public string PocetakRadnogVremena { get; set; }
        [Required]
        public string KrajRadnogVremena { get; set; }
        public virtual ICollection<Pacijent>? Pacijenti { get; set; }
        public string ImePrezime => $"{Ime} {Prezime}";
    }
}
