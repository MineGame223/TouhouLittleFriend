using System.ComponentModel;
using Terraria.ModLoader.Config;

namespace TouhouPets
{
    public class PetDialogConfig : ModConfig
    {
        public override ConfigScope Mode => ConfigScope.ClientSide;
        [DefaultValue(true)]
        public bool CanPetChat;

        [DefaultValue(true)]
        public bool TyperStyleChat;
    }
}