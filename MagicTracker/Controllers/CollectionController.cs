using MagicTracker.Models;
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
            var model = new CollectionItem[0];
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
            if (ModelState.IsValid)
            {

            }
            return View(model);
        }
    }
}