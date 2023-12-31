using System.ComponentModel;
using Terraria.ModLoader.Config;

namespace TouhouPets
{
    public class PetObtainConfig : ModConfig
    {
        public override ConfigScope Mode => ConfigScope.ServerSide;
        [DefaultValue(true)]
        public bool PetCanDropFromBoss;
    }
}