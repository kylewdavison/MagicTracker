using MagicTracker.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using mtgCard = MtgApiManager.Lib.Model;
using mtgService = MtgApiManager.Lib.Service;

namespace MagicTracker.Services
{
    public class CardApiService
    {
        public void FindCardWithApi(Card card)
        {
            List<mtgCard.Card> listOfResults = new List<mtgCard.Card>();
            mtgService.CardService apiService = new mtgService.CardService();

            CardApi newCard = new CardApi();

            var searchResults = apiService.Where(x => x.Name, card.Name)
                .All();
            if (searchResults.Value.Count != 0)
            {
                newCard.Name = searchResults.Value[1].Name;
                newCard.ManaCost = searchResults.Value[1].ManaCost;
                newCard.Colors = searchResults.Value[1].Colors;
                newCard.Type = searchResults.Value[1].Type;
                newCard.Subtypes = searchResults.Value[1].SubTypes;
                newCard.Name = searchResults.Value[1].Text;
                newCard.Printings = searchResults.Value[1].Printings;
                newCard.Text = searchResults.Value[1].Text;

                foreach (var result in searchResults.Value)
                {
                    newCard.MultiSetDict.Add(result.MultiverseId.Value, result.Set);
                    newCard.SetNameDict.Add(result.Set, result.SetName);
                }
            }
        }
    }
}
