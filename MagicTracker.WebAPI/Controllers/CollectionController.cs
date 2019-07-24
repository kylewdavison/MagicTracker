using MagicTracker.Models;
using MagicTracker.Services;
using Microsoft.AspNet.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace MagicTracker.WebAPI.Controllers
{
    [Authorize]
    public class CollectionController : ApiController
    {
        public IHttpActionResult GetAll()
        {
            CardService cardService = CreateCardService();
            var notes = cardService.GetCollection();
            return Ok(notes);
        }

        public IHttpActionResult Get(int id)
        {
            CardService cardService = CreateCardService();
            var card = cardService.GetCardById(id);
            return Ok(card);
        }

        public IHttpActionResult Post(CardCreate card)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var service = CreateCardService();

            if (service.CreateCard(card) != -1)
                return InternalServerError();

            return Ok();
        }

        public IHttpActionResult Put(CardEdit card)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var service = CreateCardService();

            if (!service.UpdateCard(card))
                return InternalServerError();

            return Ok();
        }

        public IHttpActionResult Delete(int id)
        {
            var service = CreateCardService();

            if (!service.DeleteCard(id))
                return InternalServerError();

            return Ok();
        }

        private CardService CreateCardService()
        {
            var userId = Guid.Parse(User.Identity.GetUserId());
            var cardService = new CardService(userId);
            return cardService;
        }
    }
}
