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

        private static int[] startIndex_Vanilla = new int[(int)TouhouPetID.Count];

        /// <summary>
        /// 注册原版Boss评价
        /// </summary>
        /// <param name="pet"></param>
        /// <param name="list"></param>
        /// <param name="chatPath"></param>
        public static void RegisterVanillaComment(this Projectile pet, List<int> list, string chatPath)
        {
            if (!pet.IsATouhouPet())
                return;

            //若列表为空、或路径不存在，则不执行后续
            if (list.Count <= 0 || string.IsNullOrEmpty(chatPath))
                return;

            int id = (int)pet.AsTouhouPet().UniqueID;

            //记录对应字典的起始索引值
            int lastIndex = pet.AsTouhouPet().ChatDictionary.Count;
            startIndex_Vanilla[id] = lastIndex + 1;

            //根据对应列表长度赋予其索引值
            if (list is List<int> vanillaList)
            {
                if (vanillaList.Count > 0)
                {
                    for (int i = 0; i < vanillaList.Count; i++)
                    {
                        LocalizedText text = Language.GetText($"{chatPath}.Vanilla_{i + 1}");
                        pet.AsTouhouPet().ChatDictionary.TryAdd(startIndex_Vanilla[id] + i, text);
                    }
                }
            }
        }

        /// <summary>
        /// 主动添加的跨模组Boss评价
        /// </summary>
        /// <param name="pet"></param>
        /// <param name="boss"></param>
        /// <param name="chatPath"></param>
        /// <param name="modName"></param>
        /// <param name="list"></param>
        /// <returns></returns>
        private static bool BossComment_Mod(this Projectile pet, NPC boss, string chatPath, string modName, List<string> list)
        {
            if (!pet.IsATouhouPet())
                return false;

            if (list.Count <= 0 || string.IsNullOrEmpty(modName))
                return false;

            foreach (string type in list)
            {
                //发现存在对应模组与生物时，进行评价
                if (HasModAndFindNPC(modName, boss, type))
                {
                    string actualType = type;

                    //三灾的评价是一样的
                    if (modName == Name_Thorium)
                    {
                        if (type == "Aquaius" || type == "Omnicide")
                            actualType = "SlagFury";
                    }

                    string path = $"{chatPath}.{modName}_{list.IndexOf(actualType) + 1}";
                    LocalizedText text = Language.GetText(path);

                    //排除不存在的文本
                    if (!text.Equals(path))
                    {
                        pet.SetChat(text);
                        return true;
                    }
                }
            }
            return false;
        }

        /// <summary>
        /// 原版Boss评价
        /// </summary>
        /// <param name="pet"></param>
        /// <param name="list"></param>
        /// <param name="bossType"></param>
        public static bool BossComment_Vanilla(this Projectile pet, List<int> list, int bossType)
        {
            if (!pet.IsATouhouPet())
                return false;

            //若列表为空、或被检测的种类不包含在该列表中，则不执行后续
            if (list.Count <= 0 || !list.Contains(bossType))
                return false;

            int id = (int)pet.AsTouhouPet().UniqueID;

            //魔焰眼和激光眼的评价是一样的
            if (bossType == NPCID.Spazmatism)
                bossType = NPCID.Retinazer;

            int finalIndex = startIndex_Vanilla[id] + MarisaList_Vanilla.IndexOf(bossType);
            //由于无论是否实际添加了文本、字典中都会被注册，所以这里需要直接获取实际值
            if (pet.AsTouhouPet().ChatDictionary.TryGetValue(finalIndex, out LocalizedText key))
            {
                //排除可能不存在的文本
                if(!key.IsLocalizedTextEmpty())
                {
                    pet.SetChat(key);
                    return true;
                }
            }
            return false;
        }
    }
}
