using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Graphics;
using System.Collections.Generic;
using System.Drawing.Drawing2D;
using Terraria;
using tModPorter;
using TouhouPets.Content.Projectiles.Pets;

namespace TouhouPets
{
    public static class DrawUtils
    {
        /// <summary>
        /// 输出单独控制Alpha值的颜色
        /// </summary>
        /// <param name="color"></param>
        /// <param name="alpha">Alpha值</param>
        /// <returns></returns>
        public static Color ModifiedAlphaColor(this Color color, byte alpha = 0)
        {
            Color result = color;
            result.A = alpha;
            return result;
        }
        /// <summary>
        /// 宠物们默认的绘制位置
        /// </summary>
        /// <param name="projectile"></param>
        /// <returns></returns>
        public static Vector2 DefaultDrawPetPosition(this Projectile projectile)
        {
            return projectile.Center - Main.screenPosition + new Vector2(0, 7f * Main.essScale);
        }
        /// <summary>
        /// 绘制宠物的基本方法
        /// </summary>
        /// <param name="projectile"></param>
        /// <param name="frame">当前帧</param>
        /// <param name="lightColor">颜色</param>
        /// <param name="config">DrawPetConfig</param>
        /// <param name="currentRow">当前贴图应当采用哪一列</param>
        public static void DrawPet(this Projectile projectile, int frame, Color lightColor, DrawPetConfig config, int currentRow = 0)
        {
            Texture2D t = config.AltTexture ?? AltVanillaFunction.ProjectileTexture(projectile.type);

            Vector2 pos = projectile.DefaultDrawPetPosition() + config.PositionOffset;
            Color clr = projectile.GetAlpha(lightColor) * projectile.ToPetClass().mouseOpacity;

            int height = t.Height / Main.projFrames[projectile.type];
            Rectangle rect = new(t.Width / config.TextureRow * currentRow, frame * height, t.Width / config.TextureRow, height);

            Vector2 orig = rect.Size() / 2;
            float scale = config.Scale;

            SpriteEffects effect = projectile.spriteDirection == -1 ? SpriteEffects.None : SpriteEffects.FlipHorizontally;

            if (config.ShouldUseEntitySpriteDraw && !GetInstance<MiscConfig>().CompatibilityMode)
                Main.EntitySpriteDraw(t, pos, rect, clr, projectile.rotation, orig, projectile.scale * scale, effect, 0f);
            else
                Main.spriteBatch.MyDraw(t, pos, rect, clr, projectile.rotation, orig, projectile.scale * scale, effect, 0f);
        }
        /// <summary>
        /// 将宠物的绘制状态重置，防止被染料的Shader影响
        /// <br>仅需要插在不需要着色的语句之前和执行着色的语句之后</br>
        /// </summary>
        public static void ResetDrawStateForPet(this Projectile projectile)
        {
            if (GetInstance<MiscConfig>().CompatibilityMode)
                return;

            Main.spriteBatch.QuickEndAndBegin(true, projectile.isAPreviewDummy);
        }
        /// <summary>
        /// 快速设置End Begin
        /// </summary>
        /// <param name="spriteBatch"></param>
        /// <param name="immedaite">是否立刻渲染，必须与默认渲染模式形成闭合</param>
        /// <param name="state">混合模式，默认为AlphaBlend</param>
        /// <param name="useUIMatrix">是否采用UI矩阵转换</param>
        public static void QuickEndAndBegin(this SpriteBatch spriteBatch, bool immedaite, bool useUIMatrix = false, BlendState state = null)
        {
            spriteBatch.End();
            spriteBatch.Begin(immedaite ? SpriteSortMode.Immediate : SpriteSortMode.Deferred, state ?? BlendState.AlphaBlend
                , Main.DefaultSamplerState, DepthStencilState.None, Main.Rasterizer, null
                , useUIMatrix ? Main.UIScaleMatrix : Main.Transform);
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
        /// <summary>
        /// 自用WordwrapString方法，针对中文取消连字符
        /// </summary>
        /// <param name="text">文本内容</param>
        /// <param name="font">字体</param>
        /// <param name="maxWidth">文本最大宽度</param>
        /// <param name="maxLines">文本最大行数</param>
        /// <param name="lineAmount">当前行数</param>
        /// <returns></returns>
        public static string[] MyWordwrapString(string text, DynamicSpriteFont font, int maxWidth, int maxLines, out int lineAmount)
        {
            string[] array = new string[maxLines];
            int num = 0;
            List<string> list = new List<string>(text.Split('\n'));
            List<string> list2 = new List<string>(list[0].Split(' '));
            for (int i = 1; i < list.Count && i < maxLines; i++)
            {
                list2.Add("\n");
                list2.AddRange(list[i].Split(' '));
            }

            bool flag = true;
            while (list2.Count > 0)
            {
                string text2 = list2[0];
                string text3 = " ";
                if (list2.Count == 1)
                    text3 = "";

                if (text2 == "\n")
                {
                    array[num++] += text2;
                    flag = true;
                    if (num >= maxLines)
                        break;

                    list2.RemoveAt(0);
                }
                else if (flag)
                {
                    if (font.MeasureString(text2).X > (float)maxWidth)
                    {
                        string text4 = text2[0].ToString() ?? "";
                        int num2 = 1;
                        while (font.MeasureString(text4 + text2[num2] + "-").X <= (float)maxWidth)
                        {
                            text4 += text2[num2++];
                        }

                        if (!ModUtils.IsSpecificLanguage(Terraria.Localization.GameCulture.CultureName.Chinese))
                        {
                            text4 += "-";
                        }
                        array[num++] = text4 + " ";
                        if (num >= maxLines)
                            break;

                        list2.RemoveAt(0);
                        list2.Insert(0, text2.Substring(num2));
                    }
                    else
                    {
                        ref string reference = ref array[num];
                        reference = reference + text2 + text3;
                        flag = false;
                        list2.RemoveAt(0);
                    }
                }
                else if (font.MeasureString(array[num] + text2).X > (float)maxWidth)
                {
                    num++;
                    if (num >= maxLines)
                        break;

                    flag = true;
                }
                else
                {
                    ref string reference2 = ref array[num];
                    reference2 = reference2 + text2 + text3;
                    flag = false;
                    list2.RemoveAt(0);
                }
            }

            lineAmount = num;
            if (lineAmount == maxLines)
                lineAmount--;

            return array;
        }
    }
}
