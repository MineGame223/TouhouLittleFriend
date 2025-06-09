using System.ComponentModel;
using Terraria.ModLoader.Config;

namespace TouhouPets
{
    public class MiscConfig : ModConfig
    {
        public override ConfigScope Mode => ConfigScope.ServerSide;

        [DefaultValue(false)]
        public bool CompatibilityMode;

        [DefaultValue(true)]
        [ReloadRequired]
        public bool EnableCustomMusicMode;

        [DefaultValue(false)]
        public bool PetLightOnPlayer;

        [DefaultValue(true)]
        public bool PetInvisWhenMouseHover;
    }
}