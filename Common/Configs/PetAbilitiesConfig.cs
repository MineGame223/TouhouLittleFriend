using System.ComponentModel;
using Terraria.ModLoader.Config;

namespace TouhouPets
{
    public class PetAbilitiesConfig : ModConfig
    {
        public override ConfigScope Mode => ConfigScope.ServerSide;

        [DefaultValue(true)]
        [ReloadRequired]
        public bool SpecialAbility;
    }
}