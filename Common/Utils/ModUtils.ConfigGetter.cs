using System.Collections.Generic;
using Terraria.ModLoader.Config;

namespace TouhouPets
{
    partial class ModUtils
    {
        public static bool CompatibilityMode => GetInstance<MiscConfig_ServerSide>().CompatibilityMode || TouhouPets.ForceCompatibilityMode;
        public static bool AllowCall_MarisasReaction => GetInstance<MiscConfig_ServerSide>().AllowModCall_MarisasReaction;
        public static bool AllowCall_YuyukosReaction => GetInstance<MiscConfig_ServerSide>().AllowModCall_YuyukosReaction;
        public static bool AllowCall_PetDialog => GetInstance<MiscConfig_ServerSide>().AllowModCall_PetDialog;
        public static bool AllowCall_PetChatRoom => GetInstance<MiscConfig_ServerSide>().AllowModCall_PetChatRoom;

        public static bool EnableCustomMusicMode => GetInstance<MiscConfig_ClientSide>().EnableCustomMusicMode;
        public static bool PetInvisWhenMouseHover => GetInstance<MiscConfig_ClientSide>().PetInvisWhenMouseHover;
        public static bool PetLightOnPlayer => GetInstance<MiscConfig_ClientSide>().PetLightOnPlayer;
        public static List<ItemDefinition> YuyukoBanList => GetInstance<MiscConfig_ClientSide>().YuyukoBanList;

        public static bool SpecialAbility_Hina => GetInstance<PetAbilitiesConfig>().SpecialAbility_Hina;
        public static bool SpecialAbility_Letty => GetInstance<PetAbilitiesConfig>().SpecialAbility_Letty;
        public static bool SpecialAbility_Minoriko => GetInstance<PetAbilitiesConfig>().SpecialAbility_Minoriko;
        public static bool SpecialAbility_MokuAndKaguya => GetInstance<PetAbilitiesConfig>().SpecialAbility_MokuAndKaguya;
        public static bool SpecialAbility_Murasa => GetInstance<PetAbilitiesConfig>().SpecialAbility_Murasa;
        public static bool SpecialAbility_Prismriver => GetInstance<PetAbilitiesConfig>().SpecialAbility_Prismriver;
        public static bool SpecialAbility_Satori => GetInstance<PetAbilitiesConfig>().SpecialAbility_Satori;
        public static bool SpecialAbility_Star => GetInstance<PetAbilitiesConfig>().SpecialAbility_Star;
        public static bool SpecialAbility_Tenshin => GetInstance<PetAbilitiesConfig>().SpecialAbility_Tenshin;
        public static bool SpecialAbility_Tewi => GetInstance<PetAbilitiesConfig>().SpecialAbility_Tewi;
        public static bool SpecialAbility_Wriggle => GetInstance<PetAbilitiesConfig>().SpecialAbility_Wriggle;
        public static bool SpecialAbility_Yuka => GetInstance<PetAbilitiesConfig>().SpecialAbility_Yuka;
        public static bool SpecialAbility_Yuyuko => GetInstance<PetAbilitiesConfig>().SpecialAbility_Yuyuko;
        public static bool SpecialAbility_Jyoon => GetInstance<PetAbilitiesConfig>().SpecialAbility_Jyoon;
        public static bool SpecialAbility_Shion => GetInstance<PetAbilitiesConfig>().SpecialAbility_Shion;

        public static float PetChatFrequency => GetInstance<PetDialogConfig>().ChatFrequency;
        public static bool TyperStylePetDialog => GetInstance<PetDialogConfig>().TyperStyleChat;

        public static bool PetCanDropFromBoss => GetInstance<PetObtainConfig>().PetCanDropFromBoss;
        public static bool ObtainPetByFishing => GetInstance<PetObtainConfig>().ObtainPetByFishing;
        public static bool AllowGapToSpawn => GetInstance<PetObtainConfig>().AllowGapToSpawn;
    }
}
