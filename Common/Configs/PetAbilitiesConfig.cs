using System.ComponentModel;
using Terraria.ModLoader.Config;

namespace TouhouPets
{
    public class PetAbilitiesConfig : ModConfig
    {
        public override ConfigScope Mode => ConfigScope.ServerSide;

        [DefaultValue(true)]
        public bool SpecialAbility_Hina;

        [DefaultValue(true)]
        public bool SpecialAbility_Minoriko;

        [DefaultValue(true)]
        public bool SpecialAbility_Murasa;

        [DefaultValue(true)]
        public bool SpecialAbility_Satori;

        [DefaultValue(true)]
        public bool SpecialAbility_Star;

        [DefaultValue(true)]
        public bool SpecialAbility_Tenshi;

        [DefaultValue(true)]
        public bool SpecialAbility_Wriggle;

        [DefaultValue(true)]
        public bool SpecialAbility_Yuka;

        [DefaultValue(true)]
        public bool SpecialAbility_Yuyuko;

        [DefaultValue(true)]
        public bool SpecialAbility_MokuAndKaguya;

        [DefaultValue(true)]
        public bool SpecialAbility_Tewi;
    }
}