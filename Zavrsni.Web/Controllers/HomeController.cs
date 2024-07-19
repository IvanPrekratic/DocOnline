using Microsoft.AspNetCore.Authorization;
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
    [Authorize(Policy = "RequireUserTypePacijent")]
    public class HomeController(DataManagerDbContext _dbContext, UserManager<AppUser> _userManager) : Controller
    {

        public IActionResult Index()
        {
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
                Potvrdeno = false,
                UrlVideopoziva = ""
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

        [Route("mojiPregledi")]
        [ActionName("MojiPregledi")]
        public async Task<IActionResult> MojiPreglediAsync()
        {
            var pacijent = await _userManager.GetUserAsync(User);
            var pregledi = _dbContext.Pregledi.Include(p => p.Pacijent).Include(p => p.Doktor).Where(p => p.PacijentID == pacijent.PacijentID).ToList();
            return View(pregledi);
        }


        [Route("pregledDetalji/{id}")]
        public IActionResult DetaljiOPregledu(int id)
        {
            var pregled = _dbContext.Pregledi.Include(p => p.Pacijent).Include(p => p.Doktor).FirstOrDefault(p => p.PregledID == id);
            return View(pregled);
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
