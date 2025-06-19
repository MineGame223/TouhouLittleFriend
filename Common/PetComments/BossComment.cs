using Terraria.ID;
using Terraria;
using static TouhouPets.ModUtils;
using Terraria.Localization;
using System.Collections.Generic;

namespace TouhouPets
{
    public static partial class BossComment
    {
        public const string Name_Vanilla = "Vanilla";
        public const string Name_Coralite = "Coralite";
        public const string Name_Thorium = "ThoriumMod";
        public const string Name_HJ = "ContinentOfJourney";
        public const string Name_Gensokyo = "Gensokyo";

        private const string Path_Marisa = $"Mods.{nameof(TouhouPets)}.Chat_Marisa";
        private const string Path_Youmu = $"Mods.{nameof(TouhouPets)}.Chat_Youmu";
        private enum StartIndexID : int
        {
            Vanilla,
            Coralite,
            Thorium,
            HomewardJourney,
            Gensokyo,
            Mods,
            Count
        }
        private static Dictionary<(TouhouPetID, StartIndexID), int> startIndex = [];
        private static Dictionary<(TouhouPetID uniqueID, StartIndexID indexID), int> StartIndex { get => startIndex; set => startIndex = value; }

        private static readonly List<int> bossIDList_Vanilla = [
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
        private static readonly List<string> bossIDList_Coralite = [
            "Rediancie",
            "BabyIceDragon",
            "SlimeEmperor",
            "ShadowBall",
            "Bloodiancie",
            "ThunderveinDragon",
            "ZacurrentDragon",
            "NightmarePlantera"
            ];
        private static readonly List<string> bossIDList_Thorium = [
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
        private static readonly List<string> bossIDList_HJ = [
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
        private static readonly List<string> bossIDList_Gensokyo = [
            "TenshiHinanawi",
            ];

        /// <summary>
        /// 注册评价的方法
        /// </summary>
        /// <param name="pet"></param>
        /// <param name="modName">模组名（其实是Hjson文件中的末位路径名）</param>
        public static void RegisterComment(this Projectile pet, string modName)
        {
            if (!pet.IsATouhouPet())
                return;

            TouhouPetID uniqueID = pet.AsTouhouPet().UniqueID;
            string chatPath = uniqueID switch
            {
                TouhouPetID.Marisa => Path_Marisa,
                TouhouPetID.Youmu => Path_Youmu,
                _ => string.Empty,
            };

            StartIndexID startIndexID = modName switch
            {
                Name_Vanilla => StartIndexID.Vanilla,
                Name_Coralite => StartIndexID.Coralite,
                Name_Thorium => StartIndexID.Thorium,
                Name_HJ => StartIndexID.HomewardJourney,
                Name_Gensokyo => StartIndexID.Gensokyo,
                _ => StartIndexID.Count,
            };

            object list = modName switch
            {
                Name_Vanilla => bossIDList_Vanilla,
                Name_Coralite => bossIDList_Coralite,
                Name_Thorium => bossIDList_Thorium,
                Name_HJ => bossIDList_HJ,
                Name_Gensokyo => bossIDList_Gensokyo,
                _ => null,
            };

            if (startIndexID >= StartIndexID.Count || list == null)
                return;

            //若路径不存在，则不执行后续
            if (string.IsNullOrEmpty(chatPath))
                return;

            //记录对应字典的起始索引值
            int lastIndex = pet.AsTouhouPet().ChatDictionary.Count;
            StartIndex[(uniqueID, startIndexID)] = lastIndex + 1;

            //根据对应列表长度赋予其索引值
            if (list is List<int> vanillaList)
            {
                if (vanillaList.Count > 0)
                {
                    for (int i = 0; i < vanillaList.Count; i++)
                    {
                        string path = $"{chatPath}.{modName}_{i + 1}";
                        string text = Language.GetTextValue(path);

                        //由于列表的长度是固定的，为了防止出现获取到不该获取的索引的情况，
                        //这里无论是否实际存在文本，都应注册进字典
                        //不存在文本的将注册空字符串
                        if (path.Equals(text))
                            text = string.Empty;

                        pet.AsTouhouPet().ChatDictionary.TryAdd(StartIndex[(uniqueID, startIndexID)] + i, text);
                    }
                }
            }

            if (list is List<string> modList)
            {
                if (modList.Count > 0)
                {
                    for (int i = 0; i < modList.Count; i++)
                    {
                        string path = $"{chatPath}.{modName}_{i + 1}";
                        string text = Language.GetTextValue(path);

                        if (path.Equals(text))
                            text = string.Empty;

                        pet.AsTouhouPet().ChatDictionary.TryAdd(StartIndex[(uniqueID, startIndexID)] + i, text);
                    }
                }
            }
        }

        /// <summary>
        /// 处理主动添加的跨模组Boss评价的方法
        /// </summary>
        /// <param name="pet"></param>
        /// <param name="modName">模组名</param>
        /// <param name="boss">Boss的实例</param>
        /// <param name="bossType">Boss名称</param>
        /// <param name="list">对应列表</param>
        /// <param name="startIndexID">对应起始索引枚举</param>
        private static bool HandleModBossChatFromList(this Projectile pet, string modName, NPC boss)
        {
            if (!pet.IsATouhouPet())
                return false;

            StartIndexID startIndexID = modName switch
            {
                Name_Coralite => StartIndexID.Coralite,
                Name_Thorium => StartIndexID.Thorium,
                Name_HJ => StartIndexID.HomewardJourney,
                Name_Gensokyo => StartIndexID.Gensokyo,
                _ => StartIndexID.Count,
            };

            List<string> list = modName switch
            {
                Name_Coralite => bossIDList_Coralite,
                Name_Thorium => bossIDList_Thorium,
                Name_HJ => bossIDList_HJ,
                Name_Gensokyo => bossIDList_Gensokyo,
                _ => null,
            };

            if (startIndexID >= StartIndexID.Count || list == null)
                return false;

            if (!StartIndex.TryGetValue((pet.AsTouhouPet().UniqueID, startIndexID), out int index))
                return false;

            foreach (string type in list)
            {
                //若被检测的种类不包含在该列表中，则不执行后续
                if (!list.Contains(type))
                    return false;

                //发现存在对应模组与生物时，进行评价
                if (HasModAndFindNPC(modName, boss, type))
                {
                    string actualType = type;

                    //三灾的评价是一样的
                    if (startIndexID == StartIndexID.Thorium)
                    {
                        if (type == "Aquaius" || type == "Omnicide")
                            actualType = "SlagFury";
                    }

                    int finalIndex = index + list.IndexOf(actualType);
                    //由于无论是否实际添加了文本、字典中都会被注册，所以这里需要直接获取实际值
                    if (pet.AsTouhouPet().ChatDictionary.TryGetValue(finalIndex, out string value))
                    {
                        //排除先前注册中的空字符串
                        if (!string.IsNullOrEmpty(value))
                        {
                            //使用索引值减少一次遍历
                            pet.SetChat(finalIndex);
                            return true;
                        }
                    }
                }
            }
            return false;
        }

        /// <summary>
        /// 原版Boss评价
        /// </summary>
        /// <param name="pet"></param>
        /// <param name="bossType"></param>
        public static bool BossChat_Vanilla(this Projectile pet, int bossType)
        {
            if (!pet.IsATouhouPet())
                return false;

            //以防万一（？）
            if (bossIDList_Vanilla.Count <= 0)
                return false;

            if (!StartIndex.TryGetValue((pet.AsTouhouPet().UniqueID, StartIndexID.Vanilla), out int index))
                return false;

            //若被检测的种类不包含在该列表中，则不执行后续
            if (!bossIDList_Vanilla.Contains(bossType))
                return false;

            //魔焰眼和激光眼的评价是一样的
            if (bossType == NPCID.Spazmatism)
                bossType = NPCID.Retinazer;

            int finalIndex = index + bossIDList_Vanilla.IndexOf(bossType);
            //由于无论是否实际添加了文本、字典中都会被注册，所以这里需要直接获取实际值
            if (pet.AsTouhouPet().ChatDictionary.TryGetValue(finalIndex, out string value))
            {
                //排除先前注册中的空字符串
                if (!string.IsNullOrEmpty(value))
                {
                    //使用索引值减少一次遍历
                    pet.SetChat(finalIndex);
                    return true;
                }
            }
            return false;
        }

        /// <summary>
        /// 珊瑚石Boss评价
        /// </summary>
        /// <param name="pet"></param>
        /// <param name="boss"></param>
        public static bool BossChat_Coralite(this Projectile pet, NPC boss)
        {
            return pet.HandleModBossChatFromList(Name_Coralite, boss);
        }

        /// <summary>
        /// 瑟银Boss评价
        /// </summary>
        /// <param name="pet"></param>
        /// <param name="boss"></param>
        public static bool BossChat_Thorium(this Projectile pet, NPC boss)
        {
            return pet.HandleModBossChatFromList(Name_Thorium, boss);
        }

        /// <summary>
        /// 旅人归途Boss评价
        /// </summary>
        /// <param name="pet"></param>
        /// <param name="boss"></param>
        public static bool BossChat_HomewardHourney(this Projectile pet, NPC boss)
        {
            return pet.HandleModBossChatFromList(Name_HJ, boss);
        }

        /// <summary>
        /// 幻想乡Boss评价
        /// </summary>
        /// <param name="marisa"></param>
        /// <param name="boss"></param>
        public static bool BossChat_Gensokyo(this Projectile pet, NPC boss)
        {
            return pet.HandleModBossChatFromList(Name_Gensokyo, boss);
        }
    }
}
