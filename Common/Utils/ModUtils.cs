using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Graphics;
using Terraria;
using Terraria.ID;
using Terraria.Localization;

namespace TouhouPets
{
    /// <summary>
    /// 各种静态拓展方法
    /// </summary>
    internal static class ModUtils
    {
        /// <summary>
        /// 将宠物的绘制状态重置，防止被染料的Shader影响
        /// <br>仅需要插在不需要着色的语句之后和执行着色的语句之前</br>
        /// </summary>
        public static void DrawStateNormalizeForPet(this Projectile projectile)
        {
            Main.spriteBatch.End();
            Main.spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend, Main.DefaultSamplerState, DepthStencilState.None, Main.Rasterizer, null, projectile.isAPreviewDummy ? Main.UIScaleMatrix : Main.Transform);
        }
        /// <summary>
        /// 获取对话文本
        /// </summary>
        /// <param name="tag">角色对应标签</param>
        /// <param name="index">文本对应编号</param>
        /// <returns></returns>
        public static string GetChatText(string tag,string index)
        {
            return Language.GetTextValue($"Mods.{nameof(TouhouPets)}.Chat_{tag}.Chat{index}");
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
        /// <param name="value">物品出售价值，默认为2金</param>
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
        /// 检测玩家是否被Boss锁定为目标或附近是否存在Boss
        /// </summary>
        /// <param name="player"></param>
        /// <returns></returns>
        public static bool AnyBosses(this Player player)
        {
            if (player.active && !player.dead)
            {
                foreach (NPC n in Main.npc)
                {
                    if (n.active && !n.friendly && (n.target == player.whoAmI || Vector2.Distance(n.Center, player.Center) <= 1280))
                    {
                        if (n.boss)
                        {
                            return true;
                        }
                    }
                }
            }
            return false;
        }
        /// <summary>
        /// 快速设置Additive模式绘制
        /// </summary>
        /// <param name="spriteBatch"></param>
        /// <param name="start">开启or关闭。设置非默认混合模式时，开启后必须设置关闭</param>
        /// <param name="state">混合模式，默认为Additive</param>
        public static void QuickToggleAdditiveMode(this SpriteBatch spriteBatch, bool start, BlendState state = default)
        {
            if (state == default)
            {
                state = BlendState.Additive;
            }
            if (start)
            {
                spriteBatch.End();
                spriteBatch.Begin(SpriteSortMode.Immediate, state, Main.DefaultSamplerState, DepthStencilState.None, Main.Rasterizer, null, Main.Transform);
            }
            else
            {
                spriteBatch.End();
                spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, Main.DefaultSamplerState, DepthStencilState.None, Main.Rasterizer, null, Main.Transform);
            }
        }
        /// <summary>
        /// 绘制带有边框的文字，允许设置旋转
        /// </summary>
        /// <param name="sb"></param>
        /// <param name="font">字体</param>
        /// <param name="text">文本</param>
        /// <param name="x">x位置</param>
        /// <param name="y">y位置</param>
        /// <param name="textColor">文本颜色</param>
        /// <param name="borderColor">边框颜色</param>
        /// <param name="origin">绘制中心</param>
        /// <param name="scale">大小</param>
        /// <param name="rotation">旋转</param>
        public static void MyDrawBorderStringFourWay(SpriteBatch sb, DynamicSpriteFont font, string text, float x, float y, Color textColor, Color borderColor, Vector2 origin, float scale = 1f, float rotation = 0f)
        {
            Color color = borderColor;
            Vector2 zero = Vector2.Zero;
            for (int i = 0; i < 5; i++)
            {
                switch (i)
                {
                    case 0:
                        zero.X = x - 2f;
                        zero.Y = y;
                        break;
                    case 1:
                        zero.X = x + 2f;
                        zero.Y = y;
                        break;
                    case 2:
                        zero.X = x;
                        zero.Y = y - 2f;
                        break;
                    case 3:
                        zero.X = x;
                        zero.Y = y + 2f;
                        break;
                    default:
                        zero.X = x;
                        zero.Y = y;
                        color = textColor;
                        break;
                }

                sb.DrawString(font, text, zero, color, rotation, origin, scale, SpriteEffects.None, 0f);
            }
        }
    }
}
