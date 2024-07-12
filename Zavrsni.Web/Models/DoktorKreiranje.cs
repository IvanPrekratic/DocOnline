using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using Zavrsni.Model;

namespace Zavrsni.Web.Models
{
	public class DoktorKreiranje
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
        [Required]
        [DataType(DataType.Password)]
        public string Password { get; set; }
        [Required]
        [DataType(DataType.Password)]
        public string PasswordConfirm { get; set; }
    }
}
