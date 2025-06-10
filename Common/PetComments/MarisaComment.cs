using Terraria.ID;
using Terraria;
using TouhouPets.Content.Projectiles.Pets;
using static TouhouPets.ModUtils;
using Terraria.Localization;
using System.Collections.Generic;

namespace TouhouPets
{
    public static partial class MarisaComment
    {
        private enum DictionaryID : int
        {
            Vanilla,
            Coralite,
            Thorium,
            HomewardJourney,
            Gensokyo,
            Mods,
            Count,
        }

        private const string Path = $"Mods.{nameof(TouhouPets)}.Chat_Marisa";
        private const int MaxVanillaComment = 19;
        private const int MaxCoraliteComment = 8;
        private const int MaxThoriumComment = 12;
        private const int MaxHJComment = 15;
        private const int MaxGensokyoComment = 0;

        private static Dictionary<int, string> vanillaDictionary = [];
        private static Dictionary<int, string> coraliteDictionary = [];
        private static Dictionary<int, string> thoriumDictionary = [];
        private static Dictionary<int, string> hjDictionary = [];
        private static Dictionary<int, string> gensokyoDictionary = [];
        private static int[] startIndex = new int[(int)DictionaryID.Count];

        public static void RegisterComment_Vanilla(this Marisa marisa)
        {
            for (int i = 1; i <= MaxVanillaComment; i++)
            {
                vanillaDictionary[i] = Language.GetTextValue($"{Path}.Vanilla_{i}");
            }

            int index = marisa.ChatDictionary.Count;
            startIndex[(int)DictionaryID.Vanilla] = index + 1;

            foreach (var comment in vanillaDictionary)
            {
                marisa.ChatDictionary.TryAdd(index + comment.Key, comment.Value);
            }
        }
        public static void RegisterComment_Coralite(this Marisa marisa)
        {
            for (int i = 1; i <= MaxCoraliteComment; i++)
            {
                coraliteDictionary[i] = Language.GetTextValue($"{Path}.Coralite_{i}");
            }

            int index = marisa.ChatDictionary.Count;
            startIndex[(int)DictionaryID.Coralite] = index + 1;

            foreach (var comment in coraliteDictionary)
            {
                marisa.ChatDictionary.TryAdd(index + comment.Key, comment.Value);
            }
        }
        public static void RegisterComment_Thorium(this Marisa marisa)
        {
            for (int i = 1; i <= MaxThoriumComment; i++)
            {
                thoriumDictionary[i] = Language.GetTextValue($"{Path}.Thorium_{i}");
            }

            int index = marisa.ChatDictionary.Count;
            startIndex[(int)DictionaryID.Thorium] = index + 1;

            foreach (var comment in thoriumDictionary)
            {
                marisa.ChatDictionary.TryAdd(index + comment.Key, comment.Value);
            }
        }
        public static void RegisterComment_HJ(this Marisa marisa)
        {
            for (int i = 1; i <= MaxHJComment; i++)
            {
                hjDictionary[i] = Language.GetTextValue($"{Path}.HJ_{i}");
            }

            int index = marisa.ChatDictionary.Count;
            startIndex[(int)DictionaryID.HomewardJourney] = index + 1;

            foreach (var comment in hjDictionary)
            {
                marisa.ChatDictionary.TryAdd(index + comment.Key, comment.Value);
            }
        }
        public static void BossChat_Vanilla(this Projectile marisa, ChatSettingConfig config, NPC boss)
        {
            int index = startIndex[(int)DictionaryID.Vanilla];
            switch (boss.type)
            {
                case NPCID.KingSlime:
                    marisa.SetChat(config, index);
                    break;
                case NPCID.EyeofCthulhu:
                    marisa.SetChat(config, index + 1);
                    break;
                case NPCID.EaterofWorldsHead:
                    marisa.SetChat(config, index + 2);
                    break;
                case NPCID.BrainofCthulhu:
                    marisa.SetChat(config, index + 3);
                    break;
                case NPCID.QueenBee:
                    marisa.SetChat(config, index + 4);
                    break;
                case NPCID.SkeletronHead:
                    marisa.SetChat(config, index + 5);
                    break;
                case NPCID.Deerclops:
                    marisa.SetChat(config, index + 6);
                    break;
                case NPCID.WallofFlesh:
                    marisa.SetChat(config, index + 7);
                    break;
                case NPCID.QueenSlimeBoss:
                    marisa.SetChat(config, index + 8);
                    break;
                case NPCID.Retinazer:
                case NPCID.Spazmatism:
                    marisa.SetChat(config, index + 9);
                    break;
                case NPCID.TheDestroyer:
                    marisa.SetChat(config, index + 10);
                    break;
                case NPCID.SkeletronPrime:
                    marisa.SetChat(config, index + 11);
                    break;
                case NPCID.Plantera:
                    marisa.SetChat(config, index + 12);
                    break;
                case NPCID.Golem:
                    marisa.SetChat(config, index + 13);
                    break;
                case NPCID.DukeFishron:
                    marisa.SetChat(config, index + 14);
                    break;
                case NPCID.HallowBoss:
                    marisa.SetChat(config, index + 15);
                    break;
                case NPCID.CultistBoss:
                    marisa.SetChat(config, index + 16);
                    break;
                case NPCID.MoonLordCore:
                    marisa.SetChat(config, index + 17);
                    break;
                case NPCID.DD2Betsy:
                    marisa.SetChat(config, index + 18);
                    break;
                default:
                    break;
            }
        }
        public static void BossChat_Coralite(this Projectile marisa, ChatSettingConfig config, NPC boss)
        {
            string modName = "Coralite";
            int index = startIndex[(int)DictionaryID.Coralite];
            if (HasModAndFindNPC(modName, boss, "Rediancie"))
            {
                marisa.SetChat(config, index);
            }
            if (HasModAndFindNPC(modName, boss, "BabyIceDragon"))
            {
                marisa.SetChat(config, index + 1);
            }
            if (HasModAndFindNPC(modName, boss, "SlimeEmperor"))
            {
                marisa.SetChat(config, index + 2);
            }
            if (HasModAndFindNPC(modName, boss, "ShadowBall"))
            {
                marisa.SetChat(config, index + 3);
            }
            if (HasModAndFindNPC(modName, boss, "Bloodiancie"))
            {
                marisa.SetChat(config, index + 4);
            }
            if (HasModAndFindNPC(modName, boss, "ThunderveinDragon"))
            {
                marisa.SetChat(config, index + 5);
            }
            if (HasModAndFindNPC(modName, boss, "ZacurrentDragon"))
            {
                marisa.SetChat(config, index + 6);
            }
            if (HasModAndFindNPC(modName, boss, "NightmarePlantera"))
            {
                marisa.SetChat(config, index + 7);
            }
        }
        public static void BossChat_Thorium(this Projectile marisa, ChatSettingConfig config, NPC boss)
        {
            string modName = "ThoriumMod";
            int index = startIndex[(int)DictionaryID.Thorium];
            if (HasModAndFindNPC(modName, boss, "TheGrandThunderBird"))
            {
                marisa.SetChat(config, index);
            }
            if (HasModAndFindNPC(modName, boss, "QueenJellyfish"))
            {
                marisa.SetChat(config, index + 1);
            }
            if (HasModAndFindNPC(modName, boss, "Viscount"))
            {
                marisa.SetChat(config, index + 2);
            }
            if (HasModAndFindNPC(modName, boss, "GraniteEnergyStorm"))
            {
                marisa.SetChat(config, index + 3);
            }
            if (HasModAndFindNPC(modName, boss, "BuriedChampion"))
            {
                marisa.SetChat(config, index + 4);
            }
            if (HasModAndFindNPC(modName, boss, "StarScouter"))
            {
                marisa.SetChat(config, index + 5);
            }
            if (HasModAndFindNPC(modName, boss, "BoreanStrider"))
            {
                marisa.SetChat(config, index + 6);
            }
            if (HasModAndFindNPC(modName, boss, "FallenBeholder"))
            {
                marisa.SetChat(config, index + 7);
            }
            if (HasModAndFindNPC(modName, boss, "Lich"))
            {
                marisa.SetChat(config, index + 8);
            }
            if (HasModAndFindNPC(modName, boss, "ForgottenOne"))
            {
                marisa.SetChat(config, index + 9);
            }
            if (HasModAndFindNPC(modName, boss, "SlagFury")
                || HasModAndFindNPC(modName, boss, "Aquaius")
                || HasModAndFindNPC(modName, boss, "Omnicide"))
            {
                marisa.SetChat(config, index + 10);
            }
            if (HasModAndFindNPC(modName, boss, "DreamEater"))
            {
                marisa.SetChat(config, index + 11);
            }
        }
        public static void BossChat_HomewardHourney(this Projectile marisa, ChatSettingConfig config, NPC boss)
        {
            string modName = "ContinentOfJourney";
            int index = startIndex[(int)DictionaryID.HomewardJourney];
            if (HasModAndFindNPC(modName, boss, "MarquisMoonsquid"))
            {
                marisa.SetChat(config, index);
            }
            if (HasModAndFindNPC(modName, boss, "PriestessRod"))
            {
                marisa.SetChat(config, index + 1);
            }
            if (HasModAndFindNPC(modName, boss, "TheMotherbrain"))
            {
                marisa.SetChat(config, index + 2);
            }
            if (HasModAndFindNPC(modName, boss, "Diver"))
            {
                marisa.SetChat(config, index + 3);
            }
            if (HasModAndFindNPC(modName, boss, "WallofShadow"))
            {
                marisa.SetChat(config, index + 4);
            }
            if (HasModAndFindNPC(modName, boss, "SlimeGod"))
            {
                marisa.SetChat(config, index + 5);
            }
            if (HasModAndFindNPC(modName, boss, "TheOverwatcher"))
            {
                marisa.SetChat(config, index + 6);
            }
            if (HasModAndFindNPC(modName, boss, "TheLifebringerHead"))
            {
                marisa.SetChat(config, index + 7);
            }
            if (HasModAndFindNPC(modName, boss, "TheMaterealizer"))
            {
                marisa.SetChat(config, index + 8);
            }
            if (HasModAndFindNPC(modName, boss, "ScarabBelief"))
            {
                marisa.SetChat(config, index + 9);
            }
            if (HasModAndFindNPC(modName, boss, "WorldsEndEverlastingFallingWhale"))
            {
                marisa.SetChat(config, index + 10);
            }
            if (HasModAndFindNPC(modName, boss, "TheSon"))
            {
                marisa.SetChat(config, index + 11);
            }
            if (HasModAndFindNPC(modName, boss, "UNK"))
            {
                marisa.SetChat(config, index + 12);
            }
            if (HasModAndFindNPC(modName, boss, "UNK"))
            {
                marisa.SetChat(config, index + 13);
            }
            if (HasModAndFindNPC(modName, boss, "UNK"))
            {
                marisa.SetChat(config, index + 14);
            }
        }
        public static void BossChat_Gensokyo(this Projectile marisa, ChatSettingConfig config, NPC boss)
        {
            //string modName = "Gensokyo";
        }
    }
}
