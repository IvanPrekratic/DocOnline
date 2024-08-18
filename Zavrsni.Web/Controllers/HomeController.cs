using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics;
using Zavrsni.DAL;
using Zavrsni.Model;
using Zavrsni.Web.Models;
using Zavrsni.Web.Util;

namespace Zavrsni.Web.Controllers
{
    
    public class HomeController(DataManagerDbContext _dbContext, UserManager<AppUser> _userManager) : Controller
    {

        public IActionResult Index()
        {
            return View();
        }
        [Route("privacy")]
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
        [Authorize(Policy = "RequireUserTypePacijent")]
        [Route("kreirajPregled")]
        public IActionResult KreirajPregled()
        {
            var user = _userManager.GetUserAsync(User).Result;
            ViewBag.Doktori = VratiDoktore((int)user.PacijentID);
            return View();
        }
        [Authorize(Policy = "RequireUserTypePacijent")]
        [Route("kreirajPregled")]
        [HttpPost]
        public async Task<IActionResult> KreirajPregled(KreirajPregledModel model)
        {
            Pregled pregled = new Pregled
            {
                DatumIVrijemePregleda = model.DatumPregleda.Add(model.VrijemePregleda),
                Napomena = model.Pregled.Napomena,
                PacijentID = model.Pregled.PacijentID,
                DoktorID = model.Pregled.DoktorID,
                Potvrdeno = false,
                UrlVideopoziva = "",
                BiljeskeDoktora = ""
            };
            var user = await _userManager.GetUserAsync(User);
            ViewBag.Doktori = VratiDoktore((int)user.PacijentID);
            pregled.Doktor = _dbContext.Doktori.AsQueryable().Where(c => c.DoktorID == pregled.DoktorID).FirstOrDefault();
            pregled.Pacijent = _dbContext.Pacijenti.AsQueryable().Where(c => c.PacijentID== user.PacijentID).FirstOrDefault();
            ModelState.Remove("Pregled.Doktor");
            ModelState.Remove("Pregled.Pacijent");
            ModelState.Remove("Pregled.UrlVideopoziva");
            ModelState.Remove("Pregled.BiljeskeDoktora");
            if (ModelState.IsValid)
            {
                _dbContext.Pregledi.Add(pregled);
                _dbContext.SaveChanges();
                return Redirect("/mojiPregledi");
            }

            return View(model);
        }
        [Authorize(Policy = "RequireUserTypePacijent")]
        [Route("mojiPregledi")]
        [ActionName("MojiPregledi")]
        public async Task<IActionResult> MojiPreglediAsync()
        {
            var pacijent = await _userManager.GetUserAsync(User);
            var pregledi = _dbContext.Pregledi.Include(p => p.Pacijent).Include(p => p.Doktor).Where(p => p.PacijentID == pacijent.PacijentID).ToList();
            return View(pregledi);
        }

        [Authorize(Policy = "RequireUserTypePacijent")]
        [Route("pregledDetalji/{id}")]
        public IActionResult DetaljiOPregledu(int id)
        {
            var pregled = _dbContext.Pregledi.Include(p => p.Pacijent).Include(p => p.Doktor).FirstOrDefault(p => p.PregledID == id);
            return View(pregled);
        }
        [Authorize(Policy = "RequireUserTypePacijent")]
        [Route("listaDoktora")]
        public IActionResult OdabirDoktora()
        {
            var doktori = _dbContext.Doktori.Include(p => p.Specijalizacija).ToList();
            var pacijent = _dbContext.Pacijenti.Include(p => p.Doktori).FirstOrDefault(p => p.PacijentID == _userManager.GetUserAsync(User).Result.PacijentID);
            ViewBag.Pacijent = pacijent;
            return View(doktori);
        }
        [Authorize(Policy = "RequireUserTypePacijent")]
        [Route("doktorDetalji/{id}")]
        public IActionResult AboutDoktor(int id)
        {
            var doktor = _dbContext.Doktori.Include(p => p.Specijalizacija).Include(p => p.Pacijenti).FirstOrDefault(p => p.DoktorID == id);
            Pacijent pacijent = _dbContext.Pacijenti.Include(p => p.Doktori).FirstOrDefault(p => p.PacijentID == _userManager.GetUserAsync(User).Result.PacijentID);
            if (pacijent.Doktori.Contains(doktor))
            {
                ViewBag.UpisanKodDoktora = true;
            }
            else
            {
                ViewBag.UpisanKodDoktora = false;
            }
            return View(doktor);
        }

        [Authorize(Policy = "RequireUserTypePacijent")]
        public IActionResult DodajDoktora(int id)
        {
            var user = _userManager.GetUserAsync(User).Result;
            Pacijent pacijent = _dbContext.Pacijenti.Include(p => p.Doktori).FirstOrDefault(p => p.PacijentID == user.PacijentID);
            List<Doktor> doktoriPacijenta = pacijent.Doktori.ToList();
            Doktor doktor = _dbContext.Doktori.Include(p => p.Pacijenti).Include(p => p.Specijalizacija).FirstOrDefault(p => p.DoktorID == id);
            if(pacijent.Doktori.Contains(doktor))
            {
                return Redirect("/listaDoktora");
            }
            doktoriPacijenta.Add(_dbContext.Doktori.FirstOrDefault(p => p.DoktorID == id));
            pacijent.Doktori = doktoriPacijenta;
            _dbContext.Update(pacijent);
            
            doktor.Pacijenti.Add(pacijent);
            _dbContext.Update(doktor);
            _dbContext.SaveChanges();
            return Redirect("/listaDoktora");

        }

        [Authorize(Policy = "RequireUserTypePacijent")]
        public IActionResult MakniDoktora(int id)
        {
            var user = _userManager.GetUserAsync(User).Result;
            Pacijent pacijent = _dbContext.Pacijenti.Include(p => p.Doktori).FirstOrDefault(p => p.PacijentID == user.PacijentID);
            List<Doktor> doktoriPacijenta = pacijent.Doktori.ToList();
            Doktor doktor = _dbContext.Doktori.Include(p => p.Pacijenti).Include(p => p.Specijalizacija).FirstOrDefault(p => p.DoktorID == id);
            if (!pacijent.Doktori.Contains(doktor))
            {
                return Redirect("/listaDoktora");
            }
            doktoriPacijenta.Remove(_dbContext.Doktori.FirstOrDefault(p => p.DoktorID == id));
            pacijent.Doktori = doktoriPacijenta;
            _dbContext.Update(pacijent);

            doktor.Pacijenti.Remove(pacijent);
            _dbContext.Update(doktor);
            _dbContext.SaveChanges();
            return Redirect("/listaDoktora");

        }

        [Authorize(Policy = "RequireUserTypePacijent")]
        public async Task<IActionResult> OtkaziPregledAsync(OtkaziPregledModel model)
        {
            var pregled = _dbContext.Pregledi.Include(p => p.Pacijent).Include(p => p.Doktor).FirstOrDefault(p => p.PregledID == model.PregledID);

            string body = string.Empty;
            using (StreamReader reader = new StreamReader("MailTemplate/EmailZaDoktoraOtkazivanje.html"))
            {
                body = reader.ReadToEnd();
            }
            body = body.Replace("{UserName}", pregled.Doktor.ImePrezime);
            body = body.Replace("{PacijentName}", pregled.Pacijent.ImePrezime);
            body = body.Replace("{DatumPregleda}", pregled.DatumIVrijemePregleda.ToString("d.M.yyyy"));
            body = body.Replace("{VrijemePregleda}", pregled.DatumIVrijemePregleda.ToString("HH:mm"));
            EmailConfirmation emailConfirmation = new EmailConfirmation();
            await emailConfirmation.SendEmail(pregled.Doktor.Email, "Obavijest o otkazivanju pregleda", body);


            _dbContext.Pregledi.Remove(pregled);
            _dbContext.SaveChanges();
            return Redirect("/mojiPregledi");
        }

        [Route("confirmEmail")]
        public IActionResult ConfirmEmail()
        {
           return View("ConfirmEmail");
        }

        [Route("mojiDoktori")]
        public IActionResult MojiDoktori()
        {
            var user = _userManager.GetUserAsync(User).Result;
            var pacijent = _dbContext.Pacijenti.Include(p => p.Doktori).FirstOrDefault(p => p.PacijentID== user.PacijentID);
            return View(pacijent.Doktori.ToList());
        }



        public List<SelectListItem> VratiDoktore(int pacijentID)
        {
            var user = _userManager.GetUserAsync(User).Result;
            var pacijent = _dbContext.Pacijenti.Include(p => p.Doktori).ThenInclude(p => p.Specijalizacija).FirstOrDefault(p => p.PacijentID == user.PacijentID);
            var doktori = pacijent.Doktori.ToList();
            var selectList = new List<SelectListItem>
            {
                new SelectListItem("-- odaberite doktora --", "")
            };
            foreach (var dok in doktori)
            {
                selectList.Add(new SelectListItem(dok.ImePrezime, dok.DoktorID.ToString()));
            }
            return selectList;
        }
    }
}
