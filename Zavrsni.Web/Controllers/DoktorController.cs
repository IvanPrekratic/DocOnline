﻿using Microsoft.AspNetCore.Identity;
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


        [ActionName("NadolazeciPregledi")]
        public async Task<IActionResult> NadolazeciPreglediAsync()
        {
            var doktor = await _userManager.GetUserAsync(User);
            var pregledi = _dbContext.Pregledi.Include(p => p.Pacijent).Include(p => p.Doktor).Where(p => p.DoktorID == doktor.DoktorID).ToList();
            return View(pregledi);
        }
        
        public IActionResult KreirajDoktora()
        {
            this.FillDropdownSpecijalizacije();
            return View();
        }
        [HttpPost]
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
                await rc.KreirajNovogDoktora(doktor, model, returnUrl, Request.Scheme);

                //_dbContext.Doktori.Add(model);
                //_dbContext.SaveChanges();
                return View();
            }
            else
            {
                this.FillDropdownSpecijalizacije();
                return View();
            }
        }
        [Route("pregledByID/{id}")]
        public IActionResult DetaljiOPregleduDoktor(int id)
        {
            var pregled = _dbContext.Pregledi.Include(p => p.Pacijent).Include(p => p.Doktor).FirstOrDefault(p => p.PregledID == id);
            return View(pregled);
        }
        [HttpPost]
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
            _dbContext.Pregledi.Update(pregled);
            _dbContext.SaveChanges();
            return Redirect("/pregledByID/" + model.PregledID);
        }
        
        public IActionResult Videopoziv(string id)
        {
            var redir= "https://doc-online.daily.co/" + id;
            ViewBag.Url = "https://doc-online.daily.co/" + id;
            //redirect to link in new tab
            
            return View("VideopozivView", id);
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
