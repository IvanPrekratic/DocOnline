using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Zavrsni.Model
{
    public class Pregled
    {
        [Key]
        public int PregledID { get; set; }
        [Required]
        public DateTime DatumIVrijemePregleda { get; set; }
        public string Napomena { get; set; }
        [Required]
        [ForeignKey(nameof(Pacijent))]
        public int PacijentID { get; set; }
        public Pacijent Pacijent { get; set; }
        [Required]
        [ForeignKey(nameof(Doktor))]
        public int DoktorID { get; set; }
        public Doktor Doktor { get; set; }
        public bool Potvrdeno { get; set; }
    }
}
