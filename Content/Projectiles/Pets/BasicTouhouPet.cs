using Microsoft.Xna.Framework;
using ReLogic.Graphics;
using System;
using System.Collections.Generic;
using Terraria;
using Terraria.DataStructures;
using Terraria.GameContent;
using Terraria.GameInput;
using Terraria.ID;

namespace TouhouPets.Content.Projectiles.Pets
{
    /// <summary>
    /// 东方可交互宠物基类，基本示范见 <see cref="Cirno"/>
    /// </summary>
    public abstract class BasicTouhouPet : ModProjectile
    {
        #region 字段与属性
        /// <summary>
        /// 是否不应该说话
        /// <br/>该属性并不会影响宠物更新常规对话，仅作为是否应当参与聊天的判断条件
        /// <br/>!--该属性会反复重置
        /// </summary>
        internal bool shouldNotTalking;
        /// <summary>
        /// 对话文本不透明度
        /// </summary>
        internal float chatOpacity;

        /// <summary>
        /// 对话文本所用颜色
        /// </summary>
        internal Color chatColor;

        /// <summary>
        /// 对话字体大小
        /// </summary>
        internal float chatScale;

        /// <summary>
        /// 整段对话文本的底部坐标
        /// </summary>
        internal float chatBaseY = 0;

        /// <summary>
        /// 对话的持续剩余时间
        /// </summary>
        internal int chatTimeLeft;

        /// <summary>
        /// 完成一次对话后的间隔，在大于0且 <see cref="chatTimeLeft"/> 小于等于0时会一直减少至0
        /// <br/>用途：说完一句话以后一段时间内不会再进行其他对话
        /// <br/>仅由赤蛮奇使用
        /// </summary>
        internal int chatCD;

        /// <summary>
        /// 对话文本对应的索引值
        /// </summary>
        internal int chatIndex;

        /// <summary>
        /// 对话文本
        /// </summary>
        internal string chatText;

        /// <summary>
        /// 说话前的延时
        /// </summary>
        internal int chatLag;

        /// <summary>
        /// 打字机式文本显示状态下，打印文本总共需要的时间
        /// </summary>
        internal float totalTimeToType;

        /// <summary>
        /// 打字机式文本显示状态下，打印单个字符所需的时间
        /// </summary>
        internal float timeToType;

        /// <summary>
        /// 主要计时器，从0增加至4800再重置并循环
        /// <br/>由于该计时器并不接受同步，故最好只在本地客户端执行与其相关的操作
        /// </summary>
        internal int mainTimer;

        /// <summary>
        /// 是否开启对话文字震动
        /// <br/>恋恋专用
        /// <br/>!--该属性会反复重置
        /// </summary>
        internal bool textShaking;

        /// <summary>
        /// 是否发现Boss
        /// <br/>该变量会自动更新，无需手动更改
        /// </summary>
        internal bool findBoss;

        /// <summary>
        /// 对话字典
        /// </summary>
        internal Dictionary<int, string> ChatDictionary = new Dictionary<int, string>();

        /// <summary>
        /// 当前聊天室
        /// </summary>
        internal PetChatRoom currentChatRoom;

        /// <summary>
        /// 对话属性配置
        /// </summary>
        public static ChatSettingConfig ChatSettingConfig
        {
            get => new();
        }
        /// <summary>
        /// 宠物的状态值（Projectile.ai[1]），设置该值时会进行一次netUpdate
        /// </summary>
        public int PetState
        {
            get
            {
                return (int)Projectile.ai[1];
            }
            set
            {
                Projectile.ai[1] = value;
                Projectile.netUpdate = true;
            }
        }
        /// <summary>
        /// 宠物的所属玩家
        /// </summary>
        public Player Owner => Main.player[Projectile.owner];
        /// <summary>
        /// 宠物的 owner 是否为 Main.myPlayer
        /// </summary>
        public bool OwnerIsMyPlayer => Projectile.owner == Main.myPlayer;
        #endregion

        #region 绘制方法
        private void DrawTestInfo()
        {
            bool drawingForTest = false;
            string chatTurn = "#";
            if (currentChatRoom != null)
                chatTurn = currentChatRoom.chatTurn.ToString();

            DrawStatePanelForTesting(drawingForTest, chatCD + "," + chatIndex + "," + chatLag + "," + chatTimeLeft + "," + chatTurn, new Vector2(0, 0));
            DrawStatePanelForTesting(drawingForTest, Projectile.localAI[0] + "," + Projectile.localAI[1] + "," + Projectile.localAI[2] + "," + PetState + "," + mainTimer, new Vector2(0, 30));
            DrawStatePanelForTesting(drawingForTest, timeToType + "," + totalTimeToType, new Vector2(0, 60));
            DrawStatePanelForTesting(drawingForTest, Projectile.ai[0] + "," + Projectile.ai[2], new Vector2(0, 90));
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
                        DrawUtils.MyDrawBorderStringFourWay(Main.spriteBatch, FontAssets.MouseText.Value
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
        #endregion

        #region 对话更新方法
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
            if (chatTimeLeft > 0)
            {
                if (chatLag <= 0)
                {
                    chatBaseY += 1.2f;
                    chatScale += 0.05f;
                    chatOpacity += 0.1f;
                    if (timeToType >= totalTimeToType)
                        chatTimeLeft--;
                }
            }
            else
            {
                chatBaseY = 0;
                chatScale = 1;
                chatTimeLeft = 0;
                chatOpacity -= 0.05f;

                if (chatCD > 0)
                {
                    chatCD--;
                }
                else
                {
                    chatCD = 0;
                }
            }

            if (chatOpacity > 0)
                timeToType++;

            chatBaseY = Math.Clamp(chatBaseY, -24, 0);
            chatScale = Math.Clamp(chatScale, 0, 1);
            chatOpacity = Math.Clamp(chatOpacity, 0, 1);
            timeToType = Math.Clamp(timeToType, 0, totalTimeToType);

            textShaking = false;
            shouldNotTalking = false;
        }
        private void UpdateRegularDialog()
        {
            if (currentChatRoom != null || mainTimer <= 0)
                return;

            int time = 0;
            int chance = 0;
            bool stop = true;
            SetRegularDialog(ref time, ref chance, ref stop);
            if (mainTimer % time == 0 && Main.rand.NextBool(chance) && !stop)
            {
                for (int i = 1; i < ChatDictionary.Count; i++)
                {
                    if (string.IsNullOrEmpty(GetRegularDialogText()))
                        return;

                    if (GetRegularDialogText().Equals(ChatDictionary[i]))
                    {
                        Projectile.SetChat(ChatSettingConfig, i);
                        break;
                    }
                }
            }
        }
        #endregion

        #region 查找方法
        /// <summary>
        /// 查找自身的<see cref="chatIndex"/>
        /// </summary>
        /// <param name="minIndex">最小索引值</param>
        /// <param name="maxIndex">最大索引值，默认等于最小索引值</param>
        /// <returns></returns>
        internal bool FindChatIndex(int minIndex, int maxIndex = 0)
        {
            if (maxIndex <= minIndex)
            {
                maxIndex = minIndex;
            }
            if (chatIndex >= minIndex && chatIndex <= maxIndex)
            {
                return true;
            }
            return false;
        }
        /// <summary>
        /// 查找对应宠物
        /// </summary>
        /// <param name="target">被查找的对象</param>
        /// <param name="type">宠物ID</param>
        /// <param name="minState">最小状态值（ai[1]），为-1时则将无视状态检测</param>
        /// <param name="maxState">最大状态值，默认等于最小状态值</param>
        /// <param name="checkTalkable">是否检测对应宠物应当说话</param>
        /// <returns></returns>
        internal bool FindPet(out Projectile target, int type, int minState = -1, int maxState = 0, bool checkTalkable = true)
        {
            target = null;
            if (maxState <= minState && minState > 0
                || maxState >= minState && minState < 0)
            {
                maxState = minState;
            }
            foreach (Projectile p in Main.ActiveProjectiles)
            {
                if (p.owner == Projectile.owner)
                {
                    if (p.type == type
                       && (p.ai[1] >= minState && p.ai[1] <= maxState || minState < 0)
                       && (!checkTalkable || p.ShouldPetTalking()))
                    {
                        target = p;
                    }
                }
            }
            return target != null;
        }
        /// <summary>
        /// 查找对应宠物
        /// </summary>
        /// <param name="type">宠物ID</param>
        /// <param name="minState">最小状态值（ai[1]），为-1时则将无视状态检测</param>
        /// <param name="maxState">最大状态值，默认等于最小状态值</param>
        /// <param name="checkTalkable">是否检测对应宠物应当说话</param>
        /// <returns></returns>
        internal bool FindPet(int type, bool checkTalkable = true, int minState = -1, int maxState = 0)
        {
            if (maxState <= minState && minState > 0
                || maxState >= minState && minState < 0)
            {
                maxState = minState;
            }
            foreach (Projectile p in Main.ActiveProjectiles)
            {
                if (p.owner == Projectile.owner)
                {
                    if (p.type == type
                        && (p.ai[1] >= minState && p.ai[1] <= maxState || minState < 0)
                        && (!checkTalkable || p.ShouldPetTalking()))
                    {
                        return true;
                    }
                }
            }
            return false;
        }
        #endregion

        #region 移动方法
        /// <summary>
        /// 常规移动AI
        /// </summary>
        /// <param name="point">移动到的位置</param>
        /// <param name="speed">移动速度</param>
        internal void MoveToPoint(Vector2 point, float speed, Vector2 center = default)
        {
            if (center == default)
            {
                center = Owner.MountedCenter;
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
            float velMultiplier = 1f;
            Vector2 dist = Owner.Center + point - Projectile.Center;
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
        /// <param name="dist">设置与玩家同向的最小距离</param>
        internal void ChangeDir(float dist = 100)
        {
            if (Projectile.Distance(Owner.Center) <= dist)
            {
                Projectile.spriteDirection = Owner.direction;
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
        #endregion

        #region 其他更新方法
        private bool UpdateFindBoss()
        {
            bool ok = false;
            foreach (NPC b in Main.ActiveNPCs)
            {
                if ((b.boss || b.type == NPCID.EaterofWorldsHead) && (b.target == Owner.whoAmI || Vector2.Distance(b.Center, Owner.Center) <= 1280))
                {
                    if (!findBoss)
                    {
                        OnFindBoss(b);
                        findBoss = true;
                    }
                    ok = true;
                }
            }
            if (!ok && findBoss)
            {
                findBoss = false;
            }
            return ok;
        }
        private void UpdatePetLight()
        {
            Vector2 position = Projectile.Center;
            Vector3 rgb = new Vector3(0, 0, 0);
            bool inactive = false;
            SetPetLight(ref position, ref rgb, ref inactive);
            if (!inactive)
            {
                Lighting.AddLight(position, rgb);
            }
        }
        private void UpdateMouseEntered()
        {
            if (!PlayerInput.IgnoreMouseInterface && OwnerIsMyPlayer)
            {
                Vector2 projPos = Projectile.position - Main.screenPosition;
                Rectangle projRect = new((int)projPos.X, (int)projPos.Y, Projectile.width, Projectile.height);
                if (projRect.Contains(new Point(Main.mouseX, Main.mouseY)))
                {
                    if (!OnMouseHover())
                        return;

                    if (Main.mouseRight && Main.mouseRightRelease)
                    {
                        OnMouseClick(false, true);
                    }
                    if (Main.mouseLeft && Main.mouseLeftRelease)
                    {
                        OnMouseClick(true, false);
                    }
                }
            }
        }
        #endregion

        #region 自身重写函数
        /// <summary>
        /// 注册对话文本及其索引值
        /// <br/>仅在本地端更新
        /// </summary>
        /// <param name="name">对话所属宠物的名字</param>
        /// <param name="indexRange">对话索引的范围</param>
        public virtual void RegisterChat(ref string name, ref Vector2 indexRange)
        {

        }
        /// <summary>
        /// 设置常规对话文本
        /// <br/>仅在本地端更新
        /// </summary>
        /// <param name="timePerDialog">每次说话机会的间隔</param>
        /// <param name="chance">说话的几率（1 / chance）</param>
        /// <param name="whenShouldStop">何时应当停止说话</param>
        public virtual void SetRegularDialog(ref int timePerDialog, ref int chance, ref bool whenShouldStop)
        {

        }
        /// <summary>
        /// 设置宠物照明（于PostDraw中更新）
        /// </summary>
        /// <param name="position">发光位置，默认为宠物中心</param>
        /// <param name="rgb">颜色，单值最高为2.55、最低为0</param>
        /// <param name="inactive">何时不会发光，默认为false</param>
        public virtual void SetPetLight(ref Vector2 position, ref Vector3 rgb, ref bool inactive)
        {

        }
        /// <summary>
        /// 鼠标置于宠物之上时执行的方法，可控制是否能点击宠物（于PostDraw中更新）
        /// <br/>仅在本地端更新
        /// </summary>
        /// <returns></returns>
        public virtual bool OnMouseHover()
        {
            return true;
        }
        /// <summary>
        /// 点击宠物时执行的方法，需要OnMouseHover返回true才能执行
        /// <br/>仅在本地端更新
        /// </summary>
        /// <param name="leftMouse">左键点击</param>
        /// <param name="rightMouse">右键点击</param>
        public virtual void OnMouseClick(bool leftMouse, bool rightMouse)
        {

        }
        /// <summary>
        /// 当Boss出场的一刻间执行的方法
        /// </summary>
        /// <param name="boss"></param>
        public virtual void OnFindBoss(NPC boss)
        {

        }
        /// <summary>
        /// 对话文本边框颜色，默认为黑色
        /// </summary>
        public virtual Color ChatTextBoardColor
        {
            get => Color.Black;
        }
        /// <summary>
        /// 对话文本颜色，默认为白色
        /// </summary>
        public virtual Color ChatTextColor
        {
            get => Color.White;
        }
        /// <summary>
        /// 常规对话
        /// <br/>仅在本地端更新
        /// </summary>
        /// <returns></returns>
        public virtual string GetRegularDialogText()
        {
            return null;
        }
        /// <summary>
        /// 视觉效果，用于常驻动画表现（包含玩家选择界面）
        /// <br/>若寻常动作下本体包含动画，则该动画也应当在此运行
        /// </summary>
        public virtual void VisualEffectForPreview()
        {
        }
        /// <summary>
        /// 绘制宠物，替代PreDraw
        /// </summary>
        /// <param name="lightColor"></param>
        /// <returns></returns>
        public virtual bool DrawPetSelf(ref Color lightColor)
        {
            return true;
        }
        #endregion

        #region 原有重写函数
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
            string name = string.Empty;
            Vector2 indexRange = Vector2.Zero;
            RegisterChat(ref name, ref indexRange);

            if (string.IsNullOrWhiteSpace(name))
            {
                return;
            }
            for (int i = (int)indexRange.X; i <= (int)indexRange.Y; i++)
            {
                ChatDictionary[i] = ModUtils.GetChatText(name, i.ToString());
            }
        }
        public override bool PreAI()
        {
            if (++mainTimer > 4800)
            {
                mainTimer = 0;
            }

            if (OwnerIsMyPlayer && GetInstance<PetDialogConfig>().CanPetChat)
            {
                UpdateChat();

                if (UpdateFindBoss())
                {
                    shouldNotTalking = true;
                    return base.PreAI();
                }

                UpdateRegularDialog();
            }
            return base.PreAI();
        }
        public override void PostAI()
        {
            UpdatePetLight();
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
        public override bool PreDraw(ref Color lightColor)
        {
            Projectile.ResetDrawStateForPet();//用于让染料正常工作
            return DrawPetSelf(ref lightColor);
        }
        public override void PostDraw(Color lightColor)
        {
            if (!GetInstance<MiscConfig>().CompatibilityMode)
                Main.spriteBatch.QuickEndAndBegin(false, Projectile.isAPreviewDummy);

            if (chatOpacity > 0 && OwnerIsMyPlayer && GetInstance<PetDialogConfig>().CanPetChat)
            {
                Vector2 drawPos = Projectile.position - Main.screenPosition + new Vector2(Projectile.width / 2, -20) + new Vector2(0, 7f * Main.essScale);
                float alpha = chatOpacity * Projectile.Opacity;
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
            DrawTestInfo();

            UpdateMouseEntered();
        }
        #endregion
    }
}
