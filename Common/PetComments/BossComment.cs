using Terraria.ID;
using Terraria;
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
        public static bool BossComment_Vanilla(this Projectile pet, string chatPath, List<int> list, int bossType)
        {
            if (!pet.IsATouhouPet())
                return false;

            //若列表为空、或被检测的种类不包含在该列表中，则不执行后续
            if (list.Count <= 0 || !list.Contains(bossType))
                return false;

            //魔焰眼和激光眼的评价是一样的
            if (bossType == NPCID.Spazmatism)
                bossType = NPCID.Retinazer;

            LocalizedText text = Language.GetText($"{chatPath}.Vanilla_{MarisaList_Vanilla.IndexOf(bossType) + 1}");
            pet.SetChat(text);
            return true;
        }
    }
}
