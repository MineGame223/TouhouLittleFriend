using System.ComponentModel;
using Terraria.ModLoader.Config;

namespace TouhouPets
{
    public class PetDialogConfig : ModConfig
    {
        public override ConfigScope Mode => ConfigScope.ClientSide;

        [DefaultValue(1f)]
        [Slider]
        [Range(0f, 2f)]
        [Increment(.2f)]
        public float ChatFrequency;

        [DefaultValue(true)]
        public bool TyperStyleChat;
    }
}