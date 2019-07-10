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

        public ActionResult Details(int id)
        {
            var svc = CreateCardService();
            var model = svc.GetCardById(id);

            return View(model);
        }

        public ActionResult Edit(int id)
        {
            var service = CreateCardService();
            var detail = service.GetCardById(id);
            var model = new CardEdit
            {
                CardId = detail.CardId,
                Name = detail.Name,
                Printing = detail.Printing,
                CardCondition = (Models.Condition)(int)detail.CardCondition,
                IsFoil = detail.IsFoil,
                InUse = detail.InUse,
                ForTrade = detail.ForTrade
            };
            return View(model);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(int id, CardEdit model)
        {
            if (!ModelState.IsValid) return View(model);

            if (model.CardId != id)
            {
                ModelState.AddModelError("", "Id Mismatch");
                return View(model);
            }

            var service = CreateCardService();

            if (service.UpdateCard(model))
            {
                TempData["SaveResult"] = "Your note was updated.";
                return RedirectToAction("Index");
            }

            ModelState.AddModelError("", "Your note could not be updated.");
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        private CardService CreateCardService()
        {
            var userId = Guid.Parse(User.Identity.GetUserId());
            var service = new CardService(userId);
            return service;
        }
    }
}