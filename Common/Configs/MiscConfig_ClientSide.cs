using System.Collections.Generic;
using System.ComponentModel;
using Terraria.ModLoader.Config;

namespace TouhouPets
{
    public class MiscConfig_ClientSide : ModConfig
    {
        public override ConfigScope Mode => ConfigScope.ClientSide;

        [DefaultValue(true)]
        [ReloadRequired]
        public bool EnableCustomMusicMode;

        [DefaultValue(false)]
        public bool PetLightOnPlayer;

        [DefaultValue(true)]
        public bool PetInvisWhenMouseHover;

        public List<ItemDefinition> YuyukoBanList;
    }
}
