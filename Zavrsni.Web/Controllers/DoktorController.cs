using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.Routing;
using Microsoft.EntityFrameworkCore;
using System.Text;
using Zavrsni.DAL;
using Zavrsni.Model;
using Zavrsni.Web.Areas.Identity.Pages.Account;
using Zavrsni.Web.Models;
using Zavrsni.Web.Util;
using System.Security.Cryptography;
using Sqids;
using Microsoft.AspNetCore.Authorization;

namespace Zavrsni.Web.Controllers
{
    [Authorize(Policy = "RequireUserTypeDoktor")]
    public class DoktorController : Controller
    {
        private readonly DataManagerDbContext _dbContext;
        private readonly UserManager<AppUser> _userManager;
        private readonly IUserStore<AppUser> _userStore;
        private readonly SignInManager<AppUser> _signInManager;
        private readonly ILogger<RegisterModel> _logger;
        private readonly IEmailSender _emailSender;
        private readonly IUrlHelperFactory _urlHelperFactory;
        private readonly DailyCoService _dailyCoService;

        public DoktorController(DataManagerDbContext dbContext,
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

        [Route("pregledi")]
        [ActionName("NadolazeciPregledi")]
        public async Task<IActionResult> NadolazeciPreglediAsync()
        {
            var doktor = await _userManager.GetUserAsync(User);
            var pregledi = _dbContext.Pregledi.Include(p => p.Pacijent).Include(p => p.Doktor).Where(p => p.DoktorID == doktor.DoktorID).ToList();
            return View(pregledi);
        }
        [Route("pregledByID/{id}")]
        public IActionResult DetaljiOPregleduDoktor(int id)
        {
            var pregled = _dbContext.Pregledi.Include(p => p.Pacijent).Include(p => p.Doktor).FirstOrDefault(p => p.PregledID == id);

            return View(pregled);
        }
        [HttpPost]
        [Route("potvrdaPregleda")]
        public async Task<IActionResult> PotvrdiPregled(Pregled model)
        {
            Pregled pregled = _dbContext.Pregledi.Include(p => p.Pacijent).Include(p => p.Doktor).FirstOrDefault(p => p.PregledID == model.PregledID);
            pregled.Potvrdeno = true;
            var sqids = new SqidsEncoder<int>(new()
            {
                MinLength = 5,
            });
            var roomName = sqids.Encode(pregled.PregledID);
            var dailycoUrl = await _dailyCoService.CreateRoomAsync(roomName, pregled.DatumIVrijemePregleda);
            pregled.UrlVideopoziva = roomName;

            string body = string.Empty;
            using (StreamReader reader = new StreamReader("MailTemplate/EmailZaPacijentaPotvrda.html"))
            {
                body = reader.ReadToEnd();
            }
            body = body.Replace("{UserName}", pregled.Pacijent.ImePrezime);
            body = body.Replace("{DoktorName}", pregled.Doktor.ImePrezime);
            body = body.Replace("{DatumPregleda}", pregled.DatumIVrijemePregleda.ToString("d.M.yyyy"));
            body = body.Replace("{VrijemePregleda}", pregled.DatumIVrijemePregleda.ToString("HH:mm"));
            EmailConfirmation emailConfirmation = new EmailConfirmation();
            await emailConfirmation.SendEmail(pregled.Pacijent.Email, "Obavijest o potvrđenom pregledu", body);


            _dbContext.Pregledi.Update(pregled);
            _dbContext.SaveChanges();
            return Redirect("/pregledByID/" + model.PregledID);
        }

        [Route("videopoziv/{id}")]
        public IActionResult Videopoziv(string id)
        {
            var redir = "https://doc-online.daily.co/" + id;
            ViewBag.Url = "https://doc-online.daily.co/" + id;
            //redirect to link in new tab

            return View("VideopozivView", id);
        }
        [HttpPost]
        public IActionResult DodajBiljesku(Pregled model)
        {
            Pregled pregled = _dbContext.Pregledi.Include(p => p.Pacijent).Include(p => p.Doktor).FirstOrDefault(p => p.PregledID == model.PregledID);
            pregled.BiljeskeDoktora = model.BiljeskeDoktora;
            _dbContext.Pregledi.Update(pregled);
            _dbContext.SaveChanges();
            return Redirect("/pregledByID/" + model.PregledID);
        }

        [HttpPost]
        public IActionResult UrediBiljesku(Pregled model)
        {
            Pregled pregled = _dbContext.Pregledi.Include(p => p.Pacijent).Include(p => p.Doktor).FirstOrDefault(p => p.PregledID == model.PregledID);
            pregled.BiljeskeDoktora = model.BiljeskeDoktora;
            _dbContext.Pregledi.Update(pregled);
            _dbContext.SaveChanges();
            return Redirect("/pregledByID/" + model.PregledID);
        }
        [HttpPost]
        public async Task<IActionResult> PosaljiEmailAsync(DoktorEmailModel model)
        {
            var pacijent = _dbContext.Pacijenti.FirstOrDefault(p => p.PacijentID == model.PacijentID);
            var doktor = _dbContext.Doktori.FirstOrDefault(p => p.DoktorID == model.DoktorID);
            var pregled = _dbContext.Pregledi.Include(p => p.Pacijent).Include(p => p.Doktor).FirstOrDefault(p => p.PregledID == model.PregledID);

            string body = string.Empty;
            using (StreamReader reader = new StreamReader("MailTemplate/EmailMessageTemplate.html"))
            {
                body = reader.ReadToEnd();
            }
            body = body.Replace("{PregledDatum}", pregled.DatumIVrijemePregleda.ToString());
            body = body.Replace("{UserName}", pacijent.ImePrezime);
            body = body.Replace("{Poruka}", model.EmailPoruka);
            EmailConfirmation emailConfirmation = new EmailConfirmation();
            await emailConfirmation.SendEmail(pacijent.Email, "Nova poruka od doktora", body);
            return Redirect("/pregledByID/" + model.PregledID);
        }

        [Route("popisPacijenata")]
        public IActionResult PopisPacijenata()
        {
            var user = _userManager.GetUserAsync(User).Result;
            var doktor = _dbContext.Doktori.Include(p => p.Pacijenti).FirstOrDefault(p => p.DoktorID == user.DoktorID);
            return View(doktor.Pacijenti.ToList());
        }

        [Route("pacijent/{id}")]
        public IActionResult AboutPacijent(int id)
        {
            ViewBag.PacijentPregledi = _dbContext.Pregledi.Include(p => p.Doktor).Include(p => p.Pacijent).Where(p => p.PacijentID == id).ToList();
            var pacijent = _dbContext.Pacijenti.FirstOrDefault(p => p.PacijentID == id);
            return View(pacijent);
        }
        [ActionName("OtkaziPregled")]
        public async Task<IActionResult> OtkaziPregledAsync(OtkaziPregledModel model)
        {
            var pregled = _dbContext.Pregledi.Include(p => p.Pacijent).Include(p => p.Doktor).FirstOrDefault(p => p.PregledID == model.PregledID);

            string body = string.Empty;
            using (StreamReader reader = new StreamReader("MailTemplate/EmailZaPacijentaOtkazivanje.html"))
            {
                body = reader.ReadToEnd();
            }
            body = body.Replace("{UserName}", pregled.Pacijent.ImePrezime);
            body = body.Replace("{DoktorName}", pregled.Doktor.ImePrezime);
            body = body.Replace("{DatumPregleda}", pregled.DatumIVrijemePregleda.ToString("d.M.yyyy"));
            body = body.Replace("{VrijemePregleda}", pregled.DatumIVrijemePregleda.ToString("HH:mm"));
            EmailConfirmation emailConfirmation = new EmailConfirmation();
            await emailConfirmation.SendEmail(pregled.Pacijent.Email, "Obavijest o otkazivanju pregleda", body);


            _dbContext.Pregledi.Remove(pregled);
            _dbContext.SaveChanges();
            return Redirect("/pregledi");
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
        public static byte[] GetHash(string inputString)
        {
            using (HashAlgorithm algorithm = SHA256.Create())
                return algorithm.ComputeHash(Encoding.UTF8.GetBytes(inputString));
        }

        public static string GetHashString(string inputString)
        {
            StringBuilder sb = new StringBuilder();
            foreach (byte b in GetHash(inputString))
                sb.Append(b.ToString("X2"));

            return sb.ToString();
        }
    }
}
