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
        public bool SpecialAbility_Tenshin;

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

        [DefaultValue(true)]
        public bool SpecialAbility_Letty;

        [DefaultValue(true)]
        public bool SpecialAbility_Prismriver;

        [DefaultValue(true)]
        public bool SpecialAbility_Shion;

        [DefaultValue(true)]
        public bool SpecialAbility_Jyoon;
    }
}