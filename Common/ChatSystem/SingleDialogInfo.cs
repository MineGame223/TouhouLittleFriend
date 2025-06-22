using System;
using Terraria.Localization;

namespace TouhouPets
{
    public struct SingleDialogInfo(LocalizedText dialogText, Func<bool> condition, int weight)
    {
        public LocalizedText DialogText = dialogText;
        public Func<bool> Condition = condition;
        public int Weight = weight;
    }
}
