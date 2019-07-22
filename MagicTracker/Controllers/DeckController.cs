using MagicTracker.Models;
using MagicTracker.Models.CardApi;
using MagicTracker.Models.Deck;
using MagicTracker.Services;
using Microsoft.AspNet.Identity;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace MagicTracker.Controllers
{
    public class DeckController : Controller
    {
        // GET: Deck
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult DeckCreate()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult DeckCreate(DeckCreate model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var service = CreateCardService();
            var resultId = service.CreateDeck(model);
            if (resultId != -1)
            {
                TempData["SaveResult"] = "Your Deck was created.";
                return RedirectToAction("DeckImportEdit", new { id = resultId });
            };
            ModelState.AddModelError("", "Card could not be found.");

            return View(model);
        }

        public ActionResult DeckDetails(int id)
        {
            var userId = Guid.Parse(User.Identity.GetUserId());
            var service = new CardService(userId);
            var model = service.GetDeck(id);
            return View(model);
        }

        public ActionResult DeckEdit(int id)
        {
            var userId = Guid.Parse(User.Identity.GetUserId());
            var service = new CardService(userId);
            var model = service.GetDeck(id);
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult DeckEdit(CollectionItem[] collectionItems)
        {
            var userId = Guid.Parse(User.Identity.GetUserId());
            var service = new CardService(userId);
            foreach (CollectionItem collectionItem in collectionItems)
            {
                var model = new CardEdit
                {
                    CardId = collectionItem.CardId,
                    Name = collectionItem.Name,
                    Printing = collectionItem.Printing,
                    CardCondition = collectionItem.CardCondition,
                    IsFoil = collectionItem.IsFoil,
                    InUse = collectionItem.InUse,
                    ForTrade = collectionItem.ForTrade,
                    DeckId = collectionItem.DeckId,
                    MultiverseId = collectionItem.MultiverseId,
                    CardApiId = collectionItem.CardApiId
                };
                service.UpdateCard(model);
            }
            return RedirectToAction("DeckIndex");
        }

        public ActionResult DeckImportEdit(int id)
        {
            var userId = Guid.Parse(User.Identity.GetUserId());
            var service = new CardService(userId);
            var deck = service.GetDeck(id);
            var apiDict = service.GetDeckApiDictionary(deck);
            ApiDeckView model = new ApiDeckView();
            model.Deck = deck.ToArray();
            model.ApiDict = apiDict;
            model.DeckDict = service.GetAllDecksDictionary();
            return View(model);
        }

        [HttpPost]
        [ActionName("DeckImportEdit")]
        [ValidateAntiForgeryToken]
        public ActionResult DeckImportEdit(ApiDeckView deckView)
        {
            var userId = Guid.Parse(User.Identity.GetUserId());
            var service = new CardService(userId);
            deckView.ApiDict = service.GetDeckApiDictionary(deckView.Deck);
            foreach (CollectionItem collectionItem in deckView.Deck)
            {
                var setDict = JsonConvert.DeserializeObject<Dictionary<string, int>>(deckView.ApiDict[collectionItem.CardApiId.GetValueOrDefault()].MultiSetDict);
                var model = new CardEdit
                {
                    CardId = collectionItem.CardId,
                    Name = collectionItem.Name,
                    Printing = collectionItem.Printing,
                    CardCondition = collectionItem.CardCondition,
                    IsFoil = collectionItem.IsFoil,
                    InUse = collectionItem.InUse,
                    ForTrade = collectionItem.ForTrade,
                    DeckId = collectionItem.DeckId,
                    MultiverseId = setDict[collectionItem.Printing],
                    CardApiId = collectionItem.CardApiId
                };
                if (model.DeckId == -1) { model.DeckId = null; }
                service.UpdateCard(model);
            }
            return RedirectToAction("DeckIndex");
        }

        [ActionName("DeckDelete")]
        public ActionResult DeckDelete(int id)
        {
            var svc = CreateCardService();
            var model = svc.GetDeckItem(id);

            return View(model);
        }

        [HttpPost]
        [ActionName("DeckDelete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeckDeletePost(int id)
        {
            var service = CreateCardService();

            service.DeleteDeck(id);

            TempData["SaveResult"] = "Your deck was deleted";

            return RedirectToAction("Index");
        }

        [ActionName("DeckDeleteComplete")]
        public ActionResult DeckDeleteComplete(int id)
        {
            var svc = CreateCardService();
            var model = svc.GetDeckItem(id);

            return View(model);
        }

        [HttpPost]
        [ActionName("DeckDeleteComplete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeckDeleteCompletePost(int id)
        {
            var service = CreateCardService();

            service.DeleteDeckComplete(id);

            TempData["SaveResult"] = "Your deck was deleted";

            return RedirectToAction("Index");
        }
    }
}