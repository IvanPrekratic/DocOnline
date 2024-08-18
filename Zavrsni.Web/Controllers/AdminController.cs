using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.Routing;
using Microsoft.EntityFrameworkCore;
using Zavrsni.DAL;
using Zavrsni.Model;
using Zavrsni.Web.Areas.Identity.Pages.Account;
using Zavrsni.Web.Models;
using Zavrsni.Web.Util;

namespace Zavrsni.Web.Controllers
{
    [Authorize(Policy = "RequireAdministrator")]
    public class AdminController : Controller
    {
        private readonly DataManagerDbContext _dbContext;
        private readonly UserManager<AppUser> _userManager;
        private readonly IUserStore<AppUser> _userStore;
        private readonly SignInManager<AppUser> _signInManager;
        private readonly ILogger<RegisterModel> _logger;
        private readonly IEmailSender _emailSender;
        private readonly IUrlHelperFactory _urlHelperFactory;
        private readonly DailyCoService _dailyCoService;
        public AdminController(DataManagerDbContext dbContext,
           UserManager<AppUser> userManager,
           IUserStore<AppUser> userStore,
           SignInManager<AppUser> signInManager,
           ILogger<RegisterModel> logger,
           IEmailSender emailSender,
           IUrlHelperFactory urlHelperFactory,
           DailyCoService dailyCoService)
        {
            _dbContext = dbContext;
            _userManager = userManager;
            _userStore = userStore;
            _signInManager = signInManager;
            _logger = logger;
            _emailSender = emailSender;
            _urlHelperFactory = urlHelperFactory;
            _dailyCoService = dailyCoService;
        }

        [Route("kreirajDoktora")]
        public IActionResult KreirajDoktora()
        {
            this.FillDropdownSpecijalizacije();
            return View();
        }
        [HttpPost]
        [Route("kreirajDoktora")]
        public async Task<IActionResult> KreirajDoktoraAsync(DoktorKreiranje model)
        {
            Doktor doktor = new Doktor
            {
                Ime = model.Ime,
                Prezime = model.Prezime,
                SpecijalizacijaID = model.SpecijalizacijaID,
                Email = model.Email,
                Telefon = model.Telefon,
                Adresa = model.Adresa,
                Grad = model.Grad,
                Drzava = model.Drzava,
                JMBG = model.JMBG,
                KorisnickoIme = model.KorisnickoIme,
                Titula = model.Titula,
                PocetakRadnogVremena = model.PocetakRadnogVremena,
                KrajRadnogVremena = model.KrajRadnogVremena
            };

            var pacijentList = new List<Pacijent>();
            doktor.Pacijenti = pacijentList;
            ModelState.Remove("Specijalizacija");
            doktor.Specijalizacija = _dbContext.Specijalizacije.Find(model.SpecijalizacijaID);
            var returnUrl = Url.Content("~/");

            if (ModelState.IsValid)
            {
                var urlHelper = _urlHelperFactory.GetUrlHelper(ControllerContext);
                var rc = new DoktorRegister(_userManager, _userStore, _signInManager, _logger, _emailSender, _dbContext, urlHelper);
                var success = await rc.KreirajNovogDoktora(doktor, model, returnUrl, Request.Scheme);

                TempData["AlertMessage"] = "Doktor uspjesno kreiran";
                this.FillDropdownSpecijalizacije();

                //_dbContext.Doktori.Add(model);
                //_dbContext.SaveChanges();
                return Redirect("/administracijaDoktora");
            }
            else
            {
                this.FillDropdownSpecijalizacije();
                return View();
            }
        }
        [Route("/administriranjeDoktora")]
        public IActionResult AdministriranjeDoktora()
        {

           var doktori = _dbContext.Doktori.Include(p => p.Specijalizacija).ToList();
            return View(doktori);
        }
        public IActionResult AboutDoktor(int id)
        {
            var doktor = _dbContext.Doktori.Include(p => p.Specijalizacija).Include(p => p.Pacijenti).FirstOrDefault(p => p.DoktorID == id);
            return View(doktor);
        }
        public IActionResult ObrisiDoktora(int id)
        {
            var user = _dbContext.Users.FirstOrDefault(p => p.DoktorID == id);
            var doktor = _dbContext.Doktori.Include(p => p.Pacijenti).FirstOrDefault(p => p.DoktorID == id);
            _dbContext.Users.Remove(user);
            _dbContext.Doktori.Remove(doktor);
            _dbContext.SaveChanges();
            return Redirect("/administriranjeDoktora");
        }
        [Route("/administriranjePacijenta")]
        public IActionResult AdministriranjePacijenta()
        {
            var pacijenti = _dbContext.Pacijenti.Include(p => p.Doktori).ToList();
            return View(pacijenti);
        }
        public IActionResult AboutPacijent(int id)
        {
            var pacijent = _dbContext.Pacijenti.Include(p => p.Doktori).FirstOrDefault(p => p.PacijentID == id);
            return View(pacijent);
        }
        public IActionResult ObrisiPacijenta(int id)
        {
            var user = _dbContext.Users.FirstOrDefault(p => p.PacijentID == id);
            var pacijent = _dbContext.Pacijenti.Include(p => p.Doktori).FirstOrDefault(p => p.PacijentID == id);
            _dbContext.Users.Remove(user);
            _dbContext.Pacijenti.Remove(pacijent);
            _dbContext.SaveChanges();
            return Redirect("/administriranjePacijenta");
        }




        private void FillDropdownSpecijalizacije()
        {
            var selectItems = new List<SelectListItem>();

            //Polje je opcionalno
            var listItem = new SelectListItem();
            listItem.Text = "- odaberite spec. -";
            listItem.Value = "";
            selectItems.Add(listItem);

            foreach (var category in _dbContext.Specijalizacije)
            {
                listItem = new SelectListItem(category.Naziv, category.SpecijalizacijaID.ToString());
                selectItems.Add(listItem);
            }

            ViewBag.Specijalizacije = selectItems;
        }
    }
}
