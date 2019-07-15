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

        public ActionResult IndexEdit()
        {
            var userId = Guid.Parse(User.Identity.GetUserId());
            var service = new CardService(userId);
            var model = service.GetCollection();
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult IndexEdit(CollectionItem[] collectionItems)
        {
            var userId = Guid.Parse(User.Identity.GetUserId());
            var service = new CardService(userId);
            foreach(CollectionItem collectionItem in collectionItems)
            {
                var model = new CardEdit
                {
                    CardId = collectionItem.CardId,
                    Name = collectionItem.Name,
                    Printing = collectionItem.Printing,
                    CardCondition = (Models.Condition)(int)collectionItem.CardCondition,
                    IsFoil = collectionItem.IsFoil,
                    InUse = collectionItem.InUse,
                    ForTrade = collectionItem.ForTrade
                };
                service.UpdateCard(model);
            }
            return RedirectToAction("IndexEdit");
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

        public ActionResult DetailsMultiple()
        {
            var service = CreateCardService();
            List<CardEdit> listOfCards = new List<CardEdit>();
            var collection = service.GetCollection();
            foreach (var entry in collection)
            {
                var card = service.GetCardById(entry.CardId);
                var cardEdit = new CardEdit()
                {
                    CardId = card.CardId,
                    Name = card.Name,
                    Printing = card.Printing,
                    CardCondition = (Models.Condition)(int)card.CardCondition,
                    IsFoil = card.IsFoil,
                    InUse = card.InUse,
                    ForTrade = card.ForTrade,
                    MultiverseId = card.MultiverseId,
                    Holder = card.Holder,
                };

                listOfCards.Add(cardEdit);
            }
            var cardDetailsMultiple = new CardDetailMultiple()
            {
                CardList = listOfCards
            };
            return View(listOfCards);

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
                TempData["SaveResult"] = "Your card was updated.";
                return RedirectToAction("Index");
            }

            ModelState.AddModelError("", "Your card could not be updated.");
            return View(model);
        }

        public ActionResult EditMultiple()
        {
            var service = CreateCardService();
            List<CardEdit> listOfCards = new List<CardEdit>();
            var collection = service.GetCollection();
            foreach (var entry in collection)
            {
                var card = service.GetCardById(entry.CardId);
                var cardEdit = new CardEdit()
                {
                    CardId = card.CardId,
                    Name = card.Name,
                    Printing = card.Printing,
                    CardCondition = (Models.Condition)(int)card.CardCondition,
                    IsFoil = card.IsFoil,
                    InUse = card.InUse,
                    ForTrade = card.ForTrade,
                    MultiverseId = card.MultiverseId,
                    Holder = card.Holder,
                };

                listOfCards.Add(cardEdit);
            }
            var cardDetailsMultiple = new CardDetailMultiple()
            {
                CardList = listOfCards
            };
            return View(cardDetailsMultiple);

        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult EditMultiple(CardDetailMultiple model)
        {
            if (!ModelState.IsValid) return View(model);

/*            if (model.CardId != id)
            {
                ModelState.AddModelError("", "Id Mismatch");
                return View(model);
            }*/

            var service = CreateCardService();

            if (service.UpdateCards(model))
            {
                TempData["SaveResult"] = "Your card was updated.";
                return RedirectToAction("Index");
            }

            ModelState.AddModelError("", "Your card could not be updated.");
            return View(model);
        }

        [ActionName("Delete")]
        public ActionResult Delete(int id)
        {
            var svc = CreateCardService();
            var model = svc.GetCardById(id);

            return View(model);
        }

        [HttpPost]
        [ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeletePost(int id)
        {
            var service = CreateCardService();

            service.DeleteCard(id);

            TempData["SaveResult"] = "Your card was deleted";

            return RedirectToAction("Index");
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