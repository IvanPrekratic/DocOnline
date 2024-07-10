using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics;
using Zavrsni.DAL;
using Zavrsni.Model;
using Zavrsni.Web.Models;

namespace Zavrsni.Web.Controllers
{
    public class HomeController(DataManagerDbContext _dbContext, UserManager<AppUser> _userManager) : Controller
    {

        public IActionResult Index()
        {
            /*
            Doktor noviDoktor = new Doktor
            {
                Ime = "Ivan",
                Prezime = "Ivanovic",
                SpecijalizacijaID = 1,
                Email = "ivanovici@gmail.com",
                Telefon = "061/123-456",
                Adresa = "Ulica 1",
                Grad = "Sarajevo",
                Drzava = "Bosna i Hercegovina",
                JMBG = "1234567890123",
                KorisnickoIme = "ivanovici",
                Titula = "Doktor",
                PocetakRadnogVremena = "08:00",
                KrajRadnogVremena = "16:00"
            };
            _dbContext.Doktori.Add(noviDoktor);
            _dbContext.SaveChanges();
            */


            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }

        public IActionResult About()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
        public IActionResult KreirajPregled()
        {
            ViewBag.Doktori = VratiDoktore();
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> KreirajPregled(KreirajPregledModel model)
        {
            Pregled pregled = new Pregled
            {
                DatumIVrijemePregleda = model.DatumPregleda.Add(model.VrijemePregleda),
                Napomena = model.Pregled.Napomena,
                PacijentID = model.Pregled.PacijentID,
                DoktorID = model.Pregled.DoktorID,
                Potvrdeno = false
            };
            ViewBag.Doktori = VratiDoktore();
            var user = await _userManager.GetUserAsync(User);
            pregled.Doktor = _dbContext.Doktori.AsQueryable().Where(c => c.DoktorID == pregled.DoktorID).FirstOrDefault();
            pregled.Pacijent = _dbContext.Pacijenti.AsQueryable().Where(c => c.PacijentID== user.PacijentID).FirstOrDefault();
            ModelState.Remove("Pregled.Doktor");
            ModelState.Remove("Pregled.Pacijent");
            if (ModelState.IsValid)
            {
                _dbContext.Pregledi.Add(pregled);
                _dbContext.SaveChanges();
                return RedirectToAction(nameof(Index));
            }

            return View(model);
        }



        public List<SelectListItem> VratiDoktore()
        {
            var doktori = _dbContext.Doktori.ToList();
            var selectList = new List<SelectListItem>
            {
                new SelectListItem("", "")
            };
            foreach (var dok in doktori)
            {
                selectList.Add(new SelectListItem(dok.ImePrezime, dok.DoktorID.ToString()));
            }
            return selectList;
        }
    }
}
