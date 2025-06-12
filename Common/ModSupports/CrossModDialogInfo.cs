using System;
using Terraria.Localization;

namespace TouhouPets
{
    public struct CrossModDialogInfo
    {
        public LocalizedText dialogText;
        public Func<bool> condition;
        public int weight;
    }
}
