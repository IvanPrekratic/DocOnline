﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Zavrsni.Model
{
    public class Doktor
    {
        public int DoktorID { get; set; }
        public string Ime { get; set; }
        public string Prezime { get; set; }
        public Specijalizacija Specijalizacija { get; set; }
        public string Email { get; set; }
        public string Telefon { get; set; }
        public string Adresa { get; set; }
        public string Grad { get; set; }
        public string Drzava { get; set; }
        public string JMBG { get; set; }
        public string KorisnickoIme { get; set; }
        public string Titula { get; set; }
        public string PocetakRadnogVremena { get; set; }
        public string KrajRadnogVremena { get; set; }
    }
}
