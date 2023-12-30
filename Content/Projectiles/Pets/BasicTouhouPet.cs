using Microsoft.Xna.Framework;
using ReLogic.Graphics;
using System;
using System.Collections.Generic;
using System.IO;
using Terraria;
using Terraria.GameContent;

namespace TouhouPets.Content.Projectiles.Pets
{
    /// <summary>
    /// 东方可交互宠物基类，基本示范见 <see cref="Cirno"/>
    /// </summary>
    public abstract class BasicTouhouPet : ModProjectile
    {
        private float chatAlpha;
        private Color chatColor;
        private float chatScale;
        private float chatBaseY = 0;
        /// <summary>
        /// 对话文本
        /// </summary>
        private string chatText;
        /// <summary>
        /// 说话前的延时
        /// </summary>
        private int chatLag;
        /// <summary>
        /// 打字机式文本显示状态下，打印文本总共需要的时间
        /// </summary>
        private float totalTimeToType;
        /// <summary>
        /// 打字机式文本显示状态下，打印单个字符所需的时间
        /// </summary>
        private float timeToType;
        /// <summary>
        /// 主要计时器，从0增加至4800再重置并循环
        /// <br/>由于该计时器并不接受同步，故最好只在本地客户端执行与其相关的操作
        /// </summary>
        internal int mainTimer;
        /// <summary>
        /// 额外的本地AI（等同于localAI），长度为3
        /// </summary>
        internal int[] extraAI = new int[3];
        /// <summary>
        /// 完成一次交互后的间隔，在大于0且 <see cref="ChatTimeLeft"/> 小于等于0时会一直减少至0       
        /// <br/>用途：主要为交互发起者设置。一般而言两个宠物完成一次长交互后一段时间内不会再进行交互。交互发起者在发起长交互前
        /// 先检查一次talkInterval是否为0，随后发起交互。在属于交互发起者的最后一句结束时设置talkInterval，以防止宠物在短时间内
        /// 重复该交互
        /// <br/>!--该属性不会自动设置与检测，需要手动执行
        /// </summary>
        internal int talkInterval;
        /// <summary>
        /// 对话功能是否已被占用
        /// <br/>当返回true时，大部分对话属性都将被重置为初始值
        /// <br/>!--该属性会反复重置
        /// </summary>
        internal bool chatFuncIsOccupied;
        /// <summary>
        /// 是否开启对话文字震动
        /// <br/>恋恋专用
        /// <br/>!--该属性会反复重置
        /// </summary>
        internal bool textShaking;
        /// <summary>
        /// 宠物的状态值（Projectile.ai[1]）
        /// </summary>
        public int PetState
        {
            get => (int)Projectile.ai[1];
            set => Projectile.ai[1] = value;
        }
        /// <summary>
        /// 对话的持续剩余时间（Projectile.localAI[0]）
        /// </summary>
        public int ChatTimeLeft
        {
            get => (int)Projectile.localAI[0];
            set => Projectile.localAI[0] = value;
        }
        /// <summary>
        /// 完成一次对话后的间隔（Projectile.localAI[1]），在大于0且 <see cref="ChatTimeLeft"/> 小于等于0时会一直减少至0
        /// <br/>用途：说完一句话以后一段时间内不会再进行其他对话
        /// </summary>
        public int ChatCD
        {
            get => (int)Projectile.localAI[1];
            set => Projectile.localAI[1] = value;
        }
        /// <summary>
        /// 对话文本对应的索引值（Projectile.localAI[2]）
        /// </summary>
        public int ChatIndex
        {
            get => (int)Projectile.localAI[2];
            set => Projectile.localAI[2] = value;
        }
        private void DrawChatPanel(Vector2 pos, string text, Color color, float alpha, Color boardColor = default, bool typerStyle = false)
        {
            if (string.IsNullOrEmpty(text) || string.IsNullOrWhiteSpace(text))
            {
                return;
            }
            if (boardColor == default)
            {
                boardColor = Color.Black;
            }
            DynamicSpriteFont font = FontAssets.MouseText.Value;
            float totalScale = 0.9f;
            string _text = text;
            if (typerStyle)
            {
                int textLength = (int)Math.Clamp(timeToType / (totalTimeToType / text.Length), 0, text.Length);
                _text = text.Remove(textLength);
            }
            string[] array = Utils.WordwrapString(_text, font, 210, 10, out int chatLine);
            chatLine++;
            for (int i = 0; i < chatLine; i++)
            {
                if (array[i] != null)
                {
                    if (typerStyle)
                    {
                        chatBaseY = 0;
                        chatScale = 1;
                    }
                    Vector2 offset = font.MeasureString(array[i]);
                    Vector2 orig = offset / 2;
                    Utils.DrawBorderStringFourWay(Main.spriteBatch, font
                            , array[i], pos.X
                            , pos.Y + orig.Y / 2 + i * 30 * totalScale - (chatLine - 1) * 30 * totalScale - chatBaseY
                            , color * alpha, boardColor * alpha
                            , orig, chatScale * totalScale);
                }
            }
        }
        private void DrawChatPanel_Koishi(Vector2 pos, string text, Color color, float alpha, Color boardColor = default)
        {
            if (string.IsNullOrEmpty(text) || string.IsNullOrWhiteSpace(text))
            {
                return;
            }
            if (boardColor == default)
            {
                boardColor = Color.Black;
            }
            DynamicSpriteFont font = FontAssets.MouseText.Value;
            float totalScale = 0.9f;
            string[] array = Utils.WordwrapString(text, font, 240, 10, out int chatLine);
            chatLine++;
            for (int i = 0; i < chatLine; i++)
            {
                if (array[i] != null)
                {
                    Vector2 offset = font.MeasureString(array[i]);
                    Vector2 orig = offset / 2;
                    char[] array2 = text.ToCharArray();
                    int textLength = (int)Math.Clamp(timeToType / (totalTimeToType / text.Length), 0, text.Length);
                    for (int l = 0; l < array.Length - text.Length + textLength; l++)
                    {
                        string chara = array2[l].ToString();
                        float xOffset = FontAssets.MouseText.Value.MeasureString(text.Remove(l)).X;
                        ModUtils.MyDrawBorderStringFourWay(Main.spriteBatch, FontAssets.MouseText.Value
                            , chara, pos.X + xOffset + Main.rand.Next(-2, 2)
                            , pos.Y + orig.Y / 2 + i * 30 * totalScale - (chatLine - 1) * 30 * totalScale + Main.rand.Next(-2, 2)
                            , color * alpha, boardColor * alpha, orig, chatScale * totalScale);
                    }
                }
            }
        }
        /// <summary>
        /// 用于测试的文本绘制，在宠物正下方
        /// </summary>
        /// <param name="visible">可见性</param>
        /// <param name="text">文本</param>
        internal void DrawStatePanelForTesting(bool visible, string text, Vector2 posAdj)
        {
            Vector2 pos = Projectile.position - Main.screenPosition + new Vector2(Projectile.width / 2, 70) + posAdj;
            float totalScale = 0.8f;
            string[] array = Utils.WordwrapString(text, FontAssets.MouseText.Value, 240, 10, out int chatLine);
            chatLine++;
            if (visible)
            {
                for (int i = 0; i < chatLine; i++)
                {
                    if (array[i] != null)
                    {
                        Vector2 vector2 = FontAssets.MouseText.Value.MeasureString(array[i]);
                        Vector2 orig = new Vector2(vector2.X * 0.5f, vector2.Y * 0.5f);
                        Utils.DrawBorderStringFourWay(Main.spriteBatch, FontAssets.MouseText.Value
                            , array[i], pos.X + orig.X / 8
                            , pos.Y + orig.Y + i * 30 * totalScale - (chatLine - 1) * 30 * totalScale
                            , Color.White, Color.Black
                            , orig, totalScale);
                    }
                }
            }
        }
        private void Initialize()
        {
            chatFuncIsOccupied = false;
            ChatIndex = 0;
            talkInterval = 0;
            ChatCD = 0;
            extraAI = new int[3];
        }
        private void UpdateChat()
        {
            if (chatLag > 0)
            {
                chatLag--;
            }
            else
            {
                chatLag = 0;
            }
            if (ChatTimeLeft > 0)
            {
                if (chatLag <= 0)
                {
                    chatBaseY += 1.2f;
                    chatScale += 0.05f;
                    chatAlpha += 0.1f;
                    if (timeToType >= totalTimeToType)
                        ChatTimeLeft--;
                }
            }
            else
            {
                chatBaseY = 0;
                chatScale = 1;
                ChatTimeLeft = 0;
                chatAlpha -= 0.05f;

                if (talkInterval > 0)
                {
                    talkInterval--;
                }
                else
                {
                    talkInterval = 0;
                }
                if (ChatCD > 0)
                {
                    ChatCD--;
                }
                else
                {
                    ChatCD = 0;
                }
            }
            if (chatBaseY >= 0)
            {
                chatBaseY = 0;
            }
            if (chatScale >= 1)
            {
                chatScale = 1;
            }
            if (chatAlpha < 0)
            {
                chatAlpha = 0;
            }
            if (chatAlpha > 1)
            {
                chatAlpha = 1;
            }
            if (chatAlpha > 0)
                timeToType++;
            timeToType = Math.Clamp(timeToType, 0, totalTimeToType);
            if (chatFuncIsOccupied)
            {
                ChatIndex = 0;
                ChatTimeLeft = 0;
                chatText = null;
                talkInterval = 0;
                ChatCD = 0;
            }
            chatFuncIsOccupied = false;

            textShaking = false;
        }
        private string GetChatAndSetIndex(out int index)
        {
            index = 0;
            string dialog = GetChatText(out string[] text);
            if (text != null)
            {
                for (int i = 1; i < text.Length; i++)
                {
                    if (GetChatText(out text) != null && text[i] != null && text[i].Equals(dialog))
                    {
                        index = i;
                    }
                }
            }
            return dialog;
        }
        /// <summary>
        /// 设置要说的话
        /// <br/>当 <see cref="ChatTimeLeft"/> 或 <see cref="ChatCD"/> 大于0时不输出结果
        /// </summary>
        /// <param name="color">文本颜色</param>
        /// <param name="altText">指定文本</param>
        /// <param name="altIndex">指定文本索引值</param>
        /// <param name="lag">说话前的延时</param>
        /// <param name="timeLeftPreWord">每个字符的剩余时间值；文本持续时间为该变量 * 字符数</param>
        /// <param name="breakTimeLimit">打破字符剩余时间的限制，默认情况下，当字符长度超过10个时，timeLeftPreWord上限为10</param>
        /// /// <param name="typerTime">打字机模式打印文本所需总时长，默认为字符数 * 5且默认上限为150</param>
        internal void SetChat(Color color = default, string altText = default, int altIndex = 0, int lag = 0, int timeLeftPreWord = 20, bool breakTimeLimit = false, float typerTime = -1)
        {
            if (Projectile.owner != Main.myPlayer || ChatTimeLeft > 0 || ChatCD > 0)
            {
                return;
            }
            if (color == default)
            {
                color = Lighting.GetColor((int)((Projectile.position.X + Projectile.width / 2f) / 16f), (int)((Projectile.position.Y + Projectile.height / 2f) / 16f));
            }
            string chat = GetChatAndSetIndex(out int index);
            int _index;
            if (altText != null)
            {
                chat = altText;
                _index = altIndex;
            }
            else
            {
                _index = index;
            }
            if (chat.Length > 10 && breakTimeLimit)
            {
                if (timeLeftPreWord > 10)
                {
                    timeLeftPreWord = 10;
                }
            }
            if (typerTime == -1)
            {
                typerTime = Math.Clamp(chat.Length * 5, 0, 150);
            }
            if (chat != null && chat != string.Empty)
            {
                ChatIndex = _index;
                chatBaseY = -24;
                chatScale = 0f;
                chatText = chat;
                ChatTimeLeft = Math.Clamp(chat.Length * timeLeftPreWord, 0, 420);
                timeToType = 0;
                totalTimeToType = typerTime;
                chatColor = color;
                chatLag = lag;
            }
        }
        /// <summary>
        /// 设置与其他宠物的对话
        /// </summary>
        /// <param name="otherP">另一个宠物</param>
        /// <param name="text">文本</param>
        /// <param name="color">文本颜色</param>
        /// <param name="index">文本索引</param>
        /// <param name="cd">说完这句话以后的CD</param>
        /// <param name="cd2">另一个宠物的对应CD，防止出现意外接话。默认与cd一致</param>
        /// <param name="timeleft">每个字符的剩余时间值；文本持续时间为该变量 * 字符数</param>
        /// <param name="breakLimit">打破字符剩余时间的限制，默认情况下，当字符长度超过10个时，timeLeftPreWord上限为10</param>
        /// <param name="lag">说话前的延时</param>
        /// <param name="typerTime">打字机模式打印文本所需总时长</param>
        internal void SetChatWithOtherOne(Projectile otherP, string text, Color color, int index, int cd, int cd2 = -1, int timeleft = 20, bool breakLimit = false, int lag = 20, int typerTime = -1)
        {
            if (ChatCD > 0)
            {
                ChatCD = 0;
            }
            SetChat(color, text, index, lag, timeleft, breakLimit, typerTime);
            ChatCD = cd;
            if (otherP != null)
                otherP.localAI[1] = cd2 == -1 ? cd : cd2;
        }
        /// <summary>
        /// 查找对应宠物的对话索引值
        /// </summary>
        /// <param name="target">被查找的对象</param>
        /// <param name="type">宠物ID</param>
        /// <param name="minIndex">最小索引值</param>
        /// <param name="maxIndex">最大索引值，默认等于最小索引值</param>
        /// <param name="timeLeft">指定小于的剩余时间，设置为0时直接返回 true</param>
        /// <param name="ignoreCD">是否忽略对象的 <see cref="ChatCD"/></param>
        /// <returns>当target的 <see cref="ChatCD"/> 大于0 且ignoreCD为 false 时总是返回 false</returns>
        internal bool FindChatIndex(out Projectile target, int type, int minIndex, int maxIndex = 0, int timeLeft = 1, bool ignoreCD = false)
        {
            target = null;
            if (maxIndex <= minIndex)
            {
                maxIndex = minIndex;
            }
            foreach (Projectile p in Main.projectile)
            {
                if (p != null && p.active && p.owner == Projectile.owner)
                {
                    if (p.localAI[1] <= 0 && !ignoreCD || ignoreCD)
                    {
                        if (p.type == type && p.localAI[2] >= minIndex && p.localAI[2] <= maxIndex)
                        {
                            if (timeLeft <= 0 || timeLeft > 0 && p.localAI[0] <= timeLeft)
                            {
                                target = p;
                            }
                        }
                    }
                }
            }
            return target != null;
        }
        /// <summary>
        /// 查找自身的连锁对话
        /// </summary>
        /// <param name="minIndex">最小索引值</param>
        /// <param name="maxIndex">最大索引值，默认等于最小索引值</param>
        /// <param name="timeLeft">指定小于的剩余时间，设置为0时直接返回 true</param>
        /// <param name="alphaLeft">指定小于的剩余对话剩余透明度，最大为1</param>
        /// <param name="ignoreCD">是否忽略 ChatCD</param>
        /// <returns>当 <see cref="ChatCD"/> 大于0且 ignoreCD 为 false 时总是返回 false</returns>
        internal bool FindChainedChat(int minIndex, int maxIndex = 0, int timeLeft = 1, float alphaLeft = 0, bool ignoreCD = true)
        {
            if (maxIndex <= minIndex)
            {
                maxIndex = minIndex;
            }
            if (ChatCD <= 0 && !ignoreCD || ignoreCD)
            {
                if (ChatIndex >= minIndex && ChatIndex <= maxIndex)
                {
                    if (timeLeft <= 0 || timeLeft > 0 && ChatTimeLeft <= timeLeft)
                    {
                        if (chatAlpha <= alphaLeft)
                        {
                            return true;
                        }
                    }
                }
            }
            return false;
        }
        /// <summary>
        /// 查找对应宠物
        /// </summary>
        /// <param name="target">被查找的对象</param>
        /// <param name="type">宠物ID</param>
        /// <returns></returns>
        internal bool FindPet(out Projectile target, int type)
        {
            target = null;
            foreach (Projectile p in Main.projectile)
            {
                if (p != null && p.active && p.owner == Projectile.owner)
                {
                    if (p.type == type)
                    {
                        target = p;
                    }
                }
            }
            return target != null;
        }
        /// <summary>
        /// 查找对应宠物的状态（ai[1]）
        /// </summary>
        /// <param name="target">被查找的对象</param>
        /// <param name="type">宠物ID</param>
        /// <param name="minState">最小状态值</param>
        /// <param name="maxState">最大状态值，默认等于最小状态值</param>
        /// <returns></returns>
        internal bool FindPetState(out Projectile target, int type, int minState = 0, int maxState = 0)
        {
            target = null;
            if (maxState <= minState && minState > 0
                || maxState >= minState && minState < 0)
            {
                maxState = minState;
            }
            foreach (Projectile p in Main.projectile)
            {
                if (p != null && p.active && p.owner == Projectile.owner)
                {
                    if (p.type == type && p.ai[1] >= minState && p.ai[1] <= maxState)
                    {
                        target = p;
                    }
                }
            }
            return target != null;
        }
        /// <summary>
        /// 常规移动AI，以玩家为中心
        /// </summary>
        /// <param name="point">移动到的位置</param>
        /// <param name="speed">移动速度</param>
        internal void MoveToPoint(Vector2 point, float speed)
        {
            Player player = Main.player[Projectile.owner];
            Vector2 pos = player.MountedCenter + point;
            float dist = Vector2.Distance(Projectile.Center, pos);
            if (dist > 1200f)
                Projectile.Center = player.Center + point;
            Vector2 vel = pos - Projectile.Center;

            float actualSpeed = 1;

            if (dist < actualSpeed)
                Projectile.velocity *= 0.25f;

            if (vel != Vector2.Zero)
            {
                if (vel.Length() < actualSpeed)
                    Projectile.velocity = vel;
                else
                    Projectile.velocity = vel * 0.01f * speed;
            }
        }
        /// <summary>
        /// 常规移动AI，自定义中心
        /// </summary>
        /// <param name="point">移动到的位置</param>
        /// <param name="speed">移动速度</param>
        internal void MoveToPointFreely(Vector2 point, Vector2 center, float speed)
        {
            Vector2 pos = center + point;
            float dist = Vector2.Distance(Projectile.Center, pos);
            if (dist > 1200f)
                Projectile.Center = pos;
            Vector2 vel = pos - Projectile.Center;

            float actualSpeed = 1;

            if (dist < actualSpeed)
                Projectile.velocity *= 0.25f;

            if (vel != Vector2.Zero)
            {
                if (vel.Length() < actualSpeed)
                    Projectile.velocity = vel;
                else
                    Projectile.velocity = vel * 0.01f * speed;
            }
        }
        /// <summary>
        /// 常规移动AI：2，以玩家为中心
        /// </summary>
        /// <param name="point">移动到的位置</param>
        /// <param name="speed">移动速度</param>
        internal void MoveToPoint2(Vector2 point, float speed)
        {
            Player player = Main.player[Projectile.owner];
            float velMultiplier = 1f;
            Vector2 dist = player.Center + point - Projectile.Center;
            float length = (dist == Vector2.Zero) ? 0f : dist.Length();
            if (length < speed)
            {
                velMultiplier = MathHelper.Lerp(0f, 1f, length / 16f);
            }
            Projectile.velocity = (length == 0f) ? Vector2.Zero : Vector2.Normalize(dist);
            Projectile.velocity *= speed;
            Projectile.velocity *= velMultiplier;
        }
        /// <summary>
        /// 设置转向
        /// </summary>
        /// <param name="player"></param>
        /// <param name="style2">用于处在玩家后方的宠物</param>
        /// <param name="dist">设置与玩家同向的最小距离</param>
        internal void ChangeDir(Player player, bool style2 = false, float dist = 100)
        {
            if (style2)
            {
                if (Projectile.Distance(player.Center) <= dist)
                {
                    Projectile.spriteDirection = player.direction;
                }
                else
                {
                    if (Projectile.velocity.X > 0.25f)
                    {
                        Projectile.spriteDirection = 1;
                    }
                    else if (Projectile.velocity.X < -0.25f)
                    {
                        Projectile.spriteDirection = -1;
                    }
                }
            }
            else
            {
                if (Projectile.velocity.X > 0.25f)
                {
                    Projectile.spriteDirection = 1;
                }
                else if (Projectile.velocity.X < -0.25f)
                {
                    Projectile.spriteDirection = -1;
                }
                else
                    Projectile.spriteDirection = player.direction;
            }
        }
        public virtual Color ChatTextBoardColor
        {
            get
            {
                return Color.Black;
            }
        }
        /// <summary>
        /// 常规对话
        /// </summary>
        /// <param name="text">文本，应自己规定数组长度</param>
        /// <returns></returns>
        public virtual string GetChatText(out string[] text)
        {
            text = default;
            return null;
        }
        /// <summary>
        /// 视觉效果，用于动画表现（包含玩家选择界面）
        /// </summary>
        public virtual void VisualEffectForPreview()
        {
        }
        public override void SetDefaults()
        {
            Projectile.netImportant = true;
            Projectile.width = 50;
            Projectile.height = 50;
            Projectile.friendly = true;
            Projectile.penetrate = -1;
            Projectile.ignoreWater = true;
            Projectile.timeLeft = 5;
        }
        public override bool PreAI()
        {
            if (mainTimer == 0)
            {
                Initialize();
                mainTimer = 1;
            }
            if (++mainTimer >= 4800)
            {
                mainTimer = 1;
            }
            UpdateChat();
            return base.PreAI();
        }
        public override void PostAI()
        {
            VisualEffectForPreview();
        }
        public override bool? CanCutTiles()
        {
            return false;
        }
        public override bool? CanDamage()
        {
            return false;
        }
        public override void PostDraw(Color lightColor)
        {
            Projectile.DrawStateNormalizeForPet();
            if (chatAlpha > 0 && Projectile.owner == Main.myPlayer && GetInstance<PetDialogConfig>().CanPetChat)
            {
                Vector2 drawPos = Projectile.position - Main.screenPosition + new Vector2(Projectile.width / 2, -20) + new Vector2(0, 7f * Main.essScale);
                float alpha = chatAlpha * Projectile.Opacity;
                if (textShaking)
                {
                    DrawChatPanel_Koishi(drawPos, chatText, chatColor, alpha, ChatTextBoardColor);
                }
                else
                {
                    DrawChatPanel(drawPos, chatText, chatColor, alpha, ChatTextBoardColor, GetInstance<PetDialogConfig>().TyperStyleChat);
                }
            }
            if (Projectile.isAPreviewDummy)
            {
                VisualEffectForPreview();
            }
            else
            {
                bool drawingForTest = false;
                DrawStatePanelForTesting(drawingForTest, ChatCD + "," + ChatIndex + "," + chatLag + "," + ChatTimeLeft + "," + talkInterval, new Vector2(0, 0));
                DrawStatePanelForTesting(drawingForTest, extraAI[0] + "," + extraAI[1] + "," + extraAI[2] + "," + PetState + "," + mainTimer, new Vector2(0, 30));
                DrawStatePanelForTesting(drawingForTest, timeToType + "," + totalTimeToType, new Vector2(0, 60));
                DrawStatePanelForTesting(drawingForTest, Projectile.ai[0] + "," + Projectile.ai[2], new Vector2(0, 90));
            }
        }
        public override void DrawBehind(int index, List<int> behindNPCsAndTiles, List<int> behindNPCs, List<int> behindProjectiles, List<int> overPlayers, List<int> overWiresUI)
        {
            behindProjectiles.Add(index);
        }
        public override void ReceiveExtraAI(BinaryReader reader)
        {
            for (int i = 0; i < extraAI.Length; i++)
            {
                extraAI[i] = reader.ReadInt32();
            }
        }
        public override void SendExtraAI(BinaryWriter writer)
        {
            for (int i = 0; i < extraAI.Length; i++)
            {
                writer.Write(extraAI[i]);
            }
        }
    }
}
