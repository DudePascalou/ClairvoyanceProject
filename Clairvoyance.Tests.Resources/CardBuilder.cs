using Clairvoyance.Domain;
using Clairvoyance.Services;

namespace Clairvoyance.Tests.Resources
{
    public static class CardBuilder
    {
        private static MtgAbilityService _ABILITYSERVICE;
        private static MtgCardService _CARDSERVICE;

        static CardBuilder()
        {
            _ABILITYSERVICE = new MtgAbilityService();
            _CARDSERVICE = new MtgCardService();
        }

        public static Card GetFromJson(string cardJson)
        {
            var card = _CARDSERVICE.ParseCardJson(cardJson);
            _ABILITYSERVICE.AffectAbilities(card);
            return card;
        }

    }
}
