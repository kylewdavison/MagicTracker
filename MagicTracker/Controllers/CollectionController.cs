using MagicTracker.Models;
using MagicTracker.Services;
using Microsoft.AspNet.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace MagicTracker.Controllers
{
    [Authorize]
    public class CollectionController : Controller
    {
        // GET: Collection
        public ActionResult Index()
        {
            var userId = Guid.Parse(User.Identity.GetUserId());
            var service = new CardService(userId);
            var model = service.GetCollection();
            return View(model);
        }

        //GET
        public ActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(CardCreate model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var service = CreateCardService();
            
            if (service.CreateCard(model))
            {
                TempData["SaveResult"] = "Your card was created.";
                return RedirectToAction("Index");
            };

            ModelState.AddModelError("", "Card could not be found.");
            return View(model);
        }

        private CardService CreateCardService()
        {
            var userId = Guid.Parse(User.Identity.GetUserId());
            var service = new CardService(userId);
            return service;
        }
    }
}