using Microsoft.Xna.Framework;
using Stubble.Core.Classes;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;
using Terraria;
using Terraria.ID;
using Terraria.Localization;
using TouhouPets.Content.Projectiles.Pets;

namespace TouhouPets
{
    /// <summary>
    /// 各种静态拓展方法
    /// </summary>
    public static class ModUtils
    {
        /// <summary>
        /// 判断是否为特定语言
        /// </summary>
        /// <param name="lang"></param>
        /// <returns></returns>
        public static bool IsSpecificLanguage(GameCulture.CultureName lang)
        {
            return Language.ActiveCulture.LegacyId == (int)lang;
        }
        /// <summary>
        /// 将输入价格转换为货币单位价格的文本
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static string CoinValue(int value)
        {
            int p = value % (int)Math.Pow(100, 4) / (int)Math.Pow(100, 3);
            int g = value % (int)Math.Pow(100, 3) / (int)Math.Pow(100, 2);
            int s = value % (int)Math.Pow(100, 2) / 100;
            int c = value % 100;
            string textP = p <= 0 ? "" : $"[i/s{p}:{ItemID.PlatinumCoin}]";
            string textG = g <= 0 ? "" : $"[i/s{g}:{ItemID.GoldCoin}]";
            string textS = s <= 0 ? "" : $"[i/s{s}:{ItemID.SilverCoin}]";
            string textC = c <= 0 ? "" : $"[i/s{c}:{ItemID.CopperCoin}]";
            return textP + textG + textS + textC;
        }
        /// <summary>
        /// 在Buff内生成宠物并设置Buff时间
        /// </summary>
        /// <param name="player"></param>
        /// <param name="buffIndex"></param>
        /// <param name="petType">宠物ID</param>
        /// <param name="buffTime">Buff持续时间，默认18000</param>
        public static void SpawnPetAndSetBuffTime(this Player player, int buffIndex, int petType, int buffTime = 18000)
        {
            player.buffTime[buffIndex] = buffTime;
            bool flag = petType != -1 && player.ownedProjectileCounts[petType] <= 0;
            if (flag && player.whoAmI == Main.myPlayer)
            {
                if (petType == ProjectileType<Koakuma>())
                {
                    player.GetModPlayer<TouhouPetPlayer>().koakumaNumber = Main.rand.Next(1, 301);
                }
                Projectile.NewProjectile(player.GetSource_Buff(buffIndex), player.position.X + player.width / 2, player.position.Y + player.height / 2, 0f, 0f, petType, 0, 0f, player.whoAmI);
            }
        }
        /// <summary>
        /// 在已有物品描述后插入新描述(允许添加变量)
        /// </summary>
        public static void InsertTooltipLine(this List<TooltipLine> tooltips, string text)
        {
            int index = tooltips.FindLastIndex((TooltipLine x) =>
            x.Name.StartsWith("Tooltip") && x.Mod == "Terraria");

            if (index == -1)
            {
                index = tooltips.FindIndex((TooltipLine x) => x.Name == "Tooltip0" && x.Mod == "Terraria");
            }
            if (index == -1)
                return;

            tooltips.Insert(index + 1, new TooltipLine(TouhouPets.Instance, "EachLine" + index.ToString(), text));
        }
        /// <summary>
        /// 获取对话文本
        /// </summary>
        /// <param name="tag">角色对应标签</param>
        /// <param name="index">文本对应编号</param>
        /// <returns></returns>
        public static string GetChatText(string tag, string index, params object[] args)
        {
            return Language.GetTextValue($"Mods.{nameof(TouhouPets)}.Chat_{tag}.Chat{index}", args);
        }
        /// <summary>
        /// 快速添加商店物品
        /// </summary>
        /// <param name="shop"></param>
        /// <param name="ItemType">物品ID</param>
        /// <param name="value">价值，默认为物品的原价值</param>
        /// <param name="condition">添加入商店的条件</param>
        public static void AddShopItemSimply(this NPCShop shop, int ItemType, int value = -1, params Condition[] condition)
        {
            shop.Add(ItemType, condition);
            if (shop.TryGetEntry(ItemType, out NPCShop.Entry item) && value > -1)
                item.Item.value = value;
        }
        /// <summary>
        /// 额外设置宠物物品基础属性
        /// </summary>
        /// <param name="item"></param>
        /// <param name="width">物品宽</param>
        /// <param name="height">物品高</param>
        /// <param name="rare">物品稀有度，默认为橙色</param>
        /// <param name="value">物品购买价值，默认为2金</param>
        public static void DefaultToVanitypetExtra(this Item item, int width, int height, int rare = ItemRarityID.Orange, int value = 20000)
        {
            item.width = width;
            item.height = height;
            item.rare = rare;
            item.value = value;
        }
        /// <summary>
        /// 维持宠物弹幕的存在时间
        /// </summary>
        /// <param name="projectile"></param>
        /// <param name="owner"></param>
        /// <param name="buffType">宠物对应buff种类</param>
        public static void SetPetActive(this Projectile projectile, Player owner, int buffType)
        {
            if (owner.active && !owner.dead && owner.HasBuff(buffType))
            {
                projectile.timeLeft = 2;
            }
        }
        /// <summary>
        /// 打印带有物品贴图的文本
        /// </summary>
        /// <param name="id">物品ID</param>
        /// <returns></returns>
        public static string ItemText(int id)
        {
            StringBuilder result = new("[i");
            result.Append(':');
            result.Append(id);
            result.Append(']');
            return result.ToString();
        }
    }
}
