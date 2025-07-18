﻿using TouhouPets.Content.Projectiles.Pets;
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

        private static readonly List<int> ReimuList_Touhou = [
            ProjectileType<Cirno>(),
            ProjectileType<Doremy>(),
            ProjectileType<Eirin>(),
            ProjectileType<Flandre>(),
            ProjectileType<Iku>(),
            ProjectileType<Junko>(),
            ProjectileType<Kokoro>(),
            -1,//跳过7和25，因为这两条是根据Buff判定的
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
            -1,//25
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
            for (int i = 0; i < ReimuList_Touhou.Count; i++)
            {
                reimu.ChatDictionary.TryAdd(startIndex + i, Language.GetText($"{Path}.LightPet_{i + 1}"));
            }
        }

        /// <summary>
        /// 将灵梦的评价加入随机选择器
        /// </summary>
        /// <param name="reimu"></param>
        /// <param name="chat">被添加的随机选择器</param>
        public static void Comment_TouhouLightPet(this Reimu reimu, ref WeightedRandom<LocalizedText> chat)
        {
            //以防万一（？）
            if (ReimuList_Touhou.Count <= 0)
                return;

            int index = startIndex;
            foreach (Projectile pet in Main.ActiveProjectiles)
            {
                if (pet.owner != reimu.Owner.whoAmI)
                    continue;

                if (ReimuList_Touhou.Contains(pet.type))
                {
                    chat.Add(reimu.ChatDictionary[index + ReimuList_Touhou.IndexOf(pet.type)]);
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
