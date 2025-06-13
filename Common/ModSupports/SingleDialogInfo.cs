using System;
using Terraria.Localization;

namespace TouhouPets
{
    public struct SingleDialogInfo
    {
        public LocalizedText DialogText;
        public Func<bool> Condition;
        public int Weight;
    }
}
