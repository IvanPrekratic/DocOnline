using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Zavrsni.Model
{
    public class AppUser : IdentityUser
    {
        public UserType UserType { get; set; }

        [ForeignKey(nameof(Doktor))]
        public int? DoktorID { get; set; }
        public Doktor? Doktor { get; set; }

        [ForeignKey(nameof(Pacijent))]
        public int? PacijentID { get; set; }
        public Pacijent? Pacijent { get; set; }
    }
    public enum UserType
    {
        Doktor,
        Pacijent
    }
}
