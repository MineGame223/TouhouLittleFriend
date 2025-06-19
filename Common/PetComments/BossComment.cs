using Terraria.ID;
using Terraria;
using static TouhouPets.ModUtils;
using static TouhouPets.TouhouPets;
using Terraria.Localization;
using System.Collections.Generic;

namespace TouhouPets
{
    public static partial class BossComment
    {
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
        /// 给予特定Boss评价
        /// </summary>
        /// <param name="pet"></param>
        /// <param name="id">给予评价的宠物的独特ID</param>
        /// <param name="boss"></param>
        /// <returns></returns>
        public static bool GiveCertainBossComment(this Projectile pet, NPC boss)
        {
            if (!pet.IsATouhouPet())
                return false;

            TouhouPetID id = pet.AsTouhouPet().UniqueID;

            if (!pet.BossChat_CrossMod(boss.type, id)
                    && !pet.BossChat_Vanilla(boss.type)
                    && !pet.BossChat_Coralite(boss)
                    && !pet.BossChat_Thorium(boss)
                    && !pet.BossChat_HomewardHourney(boss)
                    && !pet.BossChat_Gensokyo(boss))
            {
                return false;
            }
            return true;
        }
        /// <summary>
        /// 处理注册评价的方法
        /// </summary>
        /// <param name="pet"></param>
        /// <param name="modName">模组名</param>
        /// <param name="list">注册对象列表</param>
        /// <param name="startIndexID">起始索引枚举</param>
        private static void HandleRegisterComment(this Projectile pet, string modName, StartIndexID startIndexID, object list)
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

                        //若发现并不存在实际文本，则将该注册值设为空字符串
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

                        //若发现并不存在实际文本，则将该注册值设为空字符串
                        if (path.Equals(text))
                            text = string.Empty;

                        pet.AsTouhouPet().ChatDictionary.TryAdd(StartIndex[(uniqueID, startIndexID)] + i, text);
                    }
                }
            }
        }

        /// <summary>
        /// 注册评价
        /// </summary>
        /// <param name="pet"></param>
        public static void RegisterComment(this Projectile pet)
        {
            pet.HandleRegisterComment("Vanilla", StartIndexID.Vanilla, bossIDList_Vanilla);
            pet.HandleRegisterComment("Coralite", StartIndexID.Coralite, bossIDList_Coralite);
            pet.HandleRegisterComment("Coralite", StartIndexID.Coralite, bossIDList_Coralite);
            pet.HandleRegisterComment("Thorium", StartIndexID.Thorium, bossIDList_Thorium);
            pet.HandleRegisterComment("HJ", StartIndexID.HomewardJourney, bossIDList_HJ);
            pet.HandleRegisterComment("Gensokyo", StartIndexID.Gensokyo, bossIDList_Gensokyo);
            //由于被动添加的跨模组评价不采用索引值，因此无需注册流程
        }

        /// <summary>
        /// 跨模组Boss评价
        /// </summary>
        /// <param name="pet"></param>
        /// <param name="bossType"></param>
        public static bool BossChat_CrossMod(this Projectile pet, int bossType, TouhouPetID uniqueID)
        {
            int id = (int)uniqueID;
            List<CommentInfo> comments = CrossModBossComment[id];
            //若列表不存在内容，则不执行后续
            if (comments == null || comments.Count <= 0)
            {
                return false;
            }
            foreach (var i in comments)
            {
                if (bossType == i.ObjectType)
                {
                    pet.SetChat(i.CommentText.Get().Value);
                    return true;
                }
            }
            return false;
        }

        /// <summary>
        /// 原版Boss评价
        /// </summary>
        /// <param name="pet"></param>
        /// <param name="bossType"></param>
        private static bool BossChat_Vanilla(this Projectile pet, int bossType)
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
        /// 处理主动添加的跨模组Boss评价的方法
        /// </summary>
        /// <param name="pet"></param>
        /// <param name="modName">模组名</param>
        /// <param name="boss">Boss的实例</param>
        /// <param name="bossType">Boss名称</param>
        /// <param name="list">对应列表</param>
        /// <param name="startIndexID">对应起始索引枚举</param>
        private static bool HandleModBossChatFromList(this Projectile pet, string modName,
            NPC boss, List<string> list, StartIndexID startIndexID)
        {
            if (!pet.IsATouhouPet())
                return false;

            //以防万一（？）
            if (list.Count <= 0)
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
                    //由于无论是否实际添加了文本、都会被注册到字典中，所以这里需要直接获取实际值
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
        /// 珊瑚石Boss评价
        /// </summary>
        /// <param name="pet"></param>
        /// <param name="boss"></param>
        private static bool BossChat_Coralite(this Projectile pet, NPC boss)
        {
            string modName = "Coralite";
            List<string> list = bossIDList_Coralite;
            StartIndexID dictionary = StartIndexID.Coralite;

            return pet.HandleModBossChatFromList(modName, boss, list, dictionary);
        }

        /// <summary>
        /// 瑟银Boss评价
        /// </summary>
        /// <param name="pet"></param>
        /// <param name="boss"></param>
        private static bool BossChat_Thorium(this Projectile pet, NPC boss)
        {
            string modName = "ThoriumMod";
            List<string> list = bossIDList_Thorium;
            StartIndexID dictionary = StartIndexID.Thorium;

            return pet.HandleModBossChatFromList(modName, boss, list, dictionary);
        }

        /// <summary>
        /// 旅人归途Boss评价
        /// </summary>
        /// <param name="pet"></param>
        /// <param name="boss"></param>
        private static bool BossChat_HomewardHourney(this Projectile pet, NPC boss)
        {
            string modName = "ContinentOfJourney";
            List<string> list = bossIDList_HJ;
            StartIndexID dictionary = StartIndexID.HomewardJourney;

            return pet.HandleModBossChatFromList(modName, boss, list, dictionary);
        }

        /// <summary>
        /// 幻想乡Boss评价
        /// </summary>
        /// <param name="marisa"></param>
        /// <param name="boss"></param>
        private static bool BossChat_Gensokyo(this Projectile pet, NPC boss)
        {
            string modName = "Gensokyo";
            List<string> list = bossIDList_Gensokyo;
            StartIndexID dictionary = StartIndexID.Gensokyo;

            return pet.HandleModBossChatFromList(modName, boss, list, dictionary);
        }
    }
}
