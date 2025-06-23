using System.Collections.Generic;
using Terraria;
using Terraria.ID;

namespace TouhouPets
{
    partial class BossComment
    {
        private const string Path_Marisa = $"Mods.{nameof(TouhouPets)}.Chat_Marisa";

        private static readonly List<int> MarisaList_Vanilla = [
            NPCID.KingSlime,
            NPCID.EyeofCthulhu,
            NPCID.EaterofWorldsHead,
            NPCID.BrainofCthulhu,
            NPCID.QueenBee,
            NPCID.SkeletronHead,
            NPCID.Deerclops,
            NPCID.WallofFlesh,
            NPCID.QueenSlimeBoss,
            NPCID.Retinazer,
            NPCID.Spazmatism,
            NPCID.TheDestroyer,
            NPCID.SkeletronPrime,
            NPCID.Plantera,
            NPCID.Golem,
            NPCID.DukeFishron,
            NPCID.HallowBoss,
            NPCID.CultistBoss,
            NPCID.MoonLordCore,
            NPCID.DD2Betsy,
            ];
        private static readonly List<string> MarisaList_Coralite = [
            "Rediancie",
            "BabyIceDragon",
            "SlimeEmperor",
            "ShadowBall",
            "Bloodiancie",
            "ThunderveinDragon",
            "ZacurrentDragon",
            "NightmarePlantera"
            ];
        private static readonly List<string> MarisaList_Thorium = [
            "TheGrandThunderBird",
            "QueenJellyfish",
            "Viscount",
            "GraniteEnergyStorm",
            "BuriedChampion",
            "StarScouter",
            "BoreanStrider",
            "FallenBeholder",
            "Lich",
            "ForgottenOne",
            "SlagFury",
            "Aquaius",
            "Omnicide",
            "DreamEater"
            ];
        private static readonly List<string> MarisaList_HJ = [
            "MarquisMoonsquid",
            "PriestessRod",
            "TheMotherbrain",
            "Diver",
            "WallofShadow",
            "SlimeGod",
            "TheOverwatcher",
            "TheLifebringerHead",
            "TheMaterealizer",
            "ScarabBelief",
            "WorldsEndEverlastingFallingWhale",
            "TheSon"
            ];
        public static void GiveComment_Marisa(this Projectile marisa, NPC boss)
        {
            marisa.BossComment_Vanilla(Path_Marisa, MarisaList_Vanilla, boss.type);
            marisa.BossComment_Mod(boss, Path_Marisa, Name_Coralite, MarisaList_Coralite);
            marisa.BossComment_Mod(boss, Path_Marisa, Name_Thorium, MarisaList_Thorium);
            marisa.BossComment_Mod(boss, Path_Marisa, Name_HJ, MarisaList_HJ);
        }
    }
}
