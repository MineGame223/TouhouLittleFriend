using Microsoft.Xna.Framework;
using ReLogic.Graphics;
using System;
using System.Collections.Generic;
using System.IO;
using Terraria;
using Terraria.DataStructures;
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
        /// <br/>现在只有赤蛮奇在使用
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
        /// <summary>
        /// 参数初始化
        /// ！仅在本地端执行
        /// </summary>
        private void Initialize()
        {
            chatFuncIsOccupied = false;
            ChatIndex = 0;
            talkInterval = 0;
            ChatCD = 0;
        }
        /// <summary>
        /// 更新对话系统
        /// <br>对话系统原理如下：</br>
        /// <br>由一个宠物发起首句对话，同时另一个宠物在自身的AI中反复调用 FindChatIndex 检测是否出现相应对话，
        /// 若出现，则将自身的 ChatCD 始终设为1，以避免自身出现对话导致接话失败。</br>
        /// <br>检测对方的对话主要使用 FindChatIndex，在响应对方第一句的反复检测中使用的 FindChatIndex 
        /// 需要将 timeLeft 参数设置为0以达到只要出现对话就视为检测成功的效果。</br>
        /// <br>设置交互主要使用 SetChatWithOtherOne，由于 FindChatIndex 往往只在对方的 ChatTimeLeft 归零前的
        /// 最后一刻生效，该方法会同时为自身和对方设置 ChatCD。一来让自己在说完话后不会立刻出现额外对话，
        /// 二来让对方在“聆听”时同样不会在中途出现额外对话。</br>
        /// <br>若出现了第三句对话，则 FindChatIndex 的 ignoreCD 参数应当为true，因为此前由自己发起的
        /// SetChatWithOtherOne 方法给对方设置了超量的CD，只有无视其CD才能进行检测。</br>
        /// <br>后续的对话如此反复使用 FindChatIndex 与 SetChatWithOtherOne 即可。</br>
        /// <br>当一句对话来到结尾时，结尾对话的ChatIndex需要设置为0，同时还需要将对方的ChatIndex设置为0以彻底关闭对话。</br>
        /// </summary>
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
                    if (!string.IsNullOrWhiteSpace(text[i]) && text[i].Equals(dialog))
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
        /// <param name="typerTime">打字机模式打印文本所需总时长，默认为字符数 * 5且默认上限为150</param>
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

            if (string.IsNullOrWhiteSpace(chat))
                return;

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
        /// <summary>
        /// 设置与其他宠物的对话
        /// </summary>
        /// <param name="otherP">另一个宠物</param>
        /// <param name="text">文本</param>
        /// <param name="color">文本颜色</param>
        /// <param name="index">文本索引</param>
        /// <param name="cd">说完这句话以后的CD，默认10s</param>
        /// <param name="cd2">另一个宠物的对应CD，防止出现意外接话。默认与cd一致</param>
        /// <param name="timeleft">每个字符的剩余时间值；文本持续时间为该变量 * 字符数</param>
        /// <param name="breakLimit">打破字符剩余时间的限制，默认情况下，当字符长度超过10个时，timeLeftPreWord上限为10</param>
        /// <param name="lag">说话前的延时</param>
        /// <param name="typerTime">打字机模式打印文本所需总时长</param>
        internal void SetChatWithOtherOne(Projectile otherP, string text, Color color, int index, int cd = 600, int cd2 = -1, int timeleft = 20, bool breakLimit = false, int lag = 20, int typerTime = -1)
        {
            if (cd == default)
                cd = 600;

            ChatCD = 0;
            SetChat(color, text, index, lag, timeleft, breakLimit, typerTime);
            ChatCD = cd;
            if (otherP != null)
                otherP.localAI[1] = (cd2 == -1 || cd2 == default) ? cd : cd2;
        }
        /// <summary>
        /// 查找其他宠物的对话索引值
        /// </summary>
        /// <param name="target">被查找的对象</param>
        /// <param name="type">被查找宠物的Type</param>
        /// <param name="minIndex">最小索引值</param>
        /// <param name="maxIndex">最大索引值，默认等于最小索引值</param>
        /// <param name="timeLeft">指定小于的剩余时间，设置为0时直接返回 true（往往用于检测开头的对话）</param>
        /// <param name="ignoreCD">是否忽略对象的 <see cref="ChatCD"/>，用于第三句对话及之后的检测</param>
        /// <returns>当target的 <see cref="ChatCD"/> 大于0 且ignoreCD为 false 时总是返回 false</returns>
        internal bool FindChatIndex(out Projectile target, int type, int minIndex, int maxIndex = 0, int timeLeft = 1, bool ignoreCD = false)
        {
            target = null;
            if (maxIndex <= minIndex)
            {
                maxIndex = minIndex;
            }
            if (timeLeft == default)
            {
                timeLeft = 1;
            }
            foreach (Projectile p in Main.projectile)
            {
                if (p != null && p.active && p.owner == Projectile.owner)
                {
                    if (p.localAI[1] <= 0 || ignoreCD)
                    {
                        if (p.type == type && p.localAI[2] >= minIndex && p.localAI[2] <= maxIndex)
                        {
                            if (timeLeft <= 0 || p.localAI[0] <= timeLeft)
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
        /// <returns>当 <see cref="ChatCD"/> 大于0时总是返回 false</returns>
        internal bool FindChainedChat(int minIndex, int maxIndex = 0)
        {
            if (maxIndex <= minIndex)
            {
                maxIndex = minIndex;
            }
            if (ChatIndex >= minIndex && ChatIndex <= maxIndex)
            {
                if (ChatTimeLeft <= 1)
                {
                    if (chatAlpha <= 0)
                    {
                        return true;
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
        /// 常规移动AI
        /// </summary>
        /// <param name="point">移动到的位置</param>
        /// <param name="speed">移动速度</param>
        internal void MoveToPoint(Vector2 point, float speed, Vector2 center = default)
        {
            Player player = Main.player[Projectile.owner];
            if (center == default)
            {
                center = player.MountedCenter;
            }
            Vector2 pos = center + point;
            float dist = Vector2.Distance(Projectile.Center, pos);
            if (dist > 1200f)
                Projectile.Center = center + point;
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
        /// 常规移动AI：2
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
        public override void OnSpawn(IEntitySource source)
        {
            Initialize();
        }
        public override bool PreAI()
        {
            if (++mainTimer > 4800)
            {
                mainTimer = 0;
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
            bool drawingForTest = false;
            DrawStatePanelForTesting(drawingForTest, ChatCD + "," + ChatIndex + "," + chatLag + "," + ChatTimeLeft + "," + talkInterval, new Vector2(0, 0));
            DrawStatePanelForTesting(drawingForTest, extraAI[0] + "," + extraAI[1] + "," + extraAI[2] + "," + PetState + "," + mainTimer, new Vector2(0, 30));
            DrawStatePanelForTesting(drawingForTest, timeToType + "," + totalTimeToType, new Vector2(0, 60));
            DrawStatePanelForTesting(drawingForTest, Projectile.ai[0] + "," + Projectile.ai[2], new Vector2(0, 90));
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
