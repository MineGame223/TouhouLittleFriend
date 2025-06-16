using TouhouPets.Content.Projectiles.Pets;
using Terraria.Localization;
using Terraria.Utilities;
using TouhouPets.Content.Buffs.PetBuffs;
using Terraria;
using System.Collections.Generic;

namespace TouhouPets
{
    public static class ReimuComment
    {
        private const string Path = $"Mods.{nameof(TouhouPets)}.Chat_Reimu";

        private readonly static List<int> petIDList_Touhou = [
            ProjectileType<Cirno>(),
            ProjectileType<Doremy>(),
            ProjectileType<Eirin>(),
            ProjectileType<Flandre>(),
            ProjectileType<Iku>(),
            ProjectileType<Junko>(),
            ProjectileType<Kokoro>(),
            -1,
            ProjectileType<Meirin>(),
            ProjectileType<Moku>(),
            ProjectileType<Nitori>(),
            ProjectileType<Patchouli>(),
            ProjectileType<Sanae>(),
            ProjectileType<Satori>(),
            ProjectileType<Shinki>(),
            ProjectileType<Sizuha>(),
            ProjectileType<StarPet>(),
            ProjectileType<Sunny>(),
            ProjectileType<Luna>(),
            ProjectileType<Utsuho>(),
            ProjectileType<Wakasagihime>(),
            ProjectileType<Wriggle>(),
            ProjectileType<Youmu>(),
            ProjectileType<Lily>(),
            ProjectileType<AliceOld>(),
            -1,
            ProjectileType<Lunasa>(),
            ProjectileType<Merlin>(),
            ProjectileType<Lyrica>(),
            ];

        private static int startIndex = 0;
        /// <summary>
        /// 注册灵梦对宠物的评价
        /// </summary>
        /// <param name="reimu"></param>
        public static void RegisterComments(this Reimu reimu)
        {
            //记录起始索引值
            int index = reimu.ChatDictionary.Count;
            startIndex = index + 1;

            //以ID列表的长度为索引，注册相应对话
            for (int i = 0; i < petIDList_Touhou.Count; i++)
            {
                reimu.ChatDictionary.TryAdd(startIndex + i, Language.GetTextValue($"{Path}.LightPet_{i + 1}"));
            }
        }

        /// <summary>
        /// 将灵梦的评价加入随机选择器
        /// </summary>
        /// <param name="reimu"></param>
        /// <param name="chat">被添加的随机选择器</param>
        public static void Comment_TouhouLightPet(this Reimu reimu, ref WeightedRandom<string> chat)
        {
            //以防万一（？）
            if (petIDList_Touhou.Count <= 0)
                return;

            int index = startIndex;
            foreach (Projectile pet in Main.ActiveProjectiles)
            {
                if (pet.owner != reimu.Owner.whoAmI)
                    continue;

                //跳过7和25，因为这两条是根据Buff判定的
                if (petIDList_Touhou.Contains(pet.type))
                {
                    chat.Add(reimu.ChatDictionary[index + petIDList_Touhou.IndexOf(pet.type)]);
                }
            }

            if (reimu.Owner.HasBuff<TheThreeFairiesBuff>())
            {
                chat.Add(reimu.ChatDictionary[index + 7]);
            }
            if (reimu.Owner.HasBuff<PoltergeistBuff>())
            {
                chat.Add(reimu.ChatDictionary[index + 25]);
            }
        }
    }
}
