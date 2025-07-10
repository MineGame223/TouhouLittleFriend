using Microsoft.Xna.Framework;
using System;
using Terraria;
using Terraria.GameContent;
using Terraria.GameInput;
using Terraria.ID;
using TouhouPets.Common.ModSupports.ModPetRegisterSystem;
using TouhouPets.Content.Projectiles.Pets;

namespace TouhouPets
{
    /// <summary>
    /// 可交互宠物基类，基本示范见 <see cref="Cirno"/>
    /// </summary>
    public abstract partial class BasicTouhouPet : ModProjectile
    {
        #region 字段与属性

        protected internal bool useDye = false;

        /// <summary>
        /// 是否发现Boss
        /// <br/>该变量会自动更新，无需手动更改
        /// </summary>
        private bool findBoss;

        /// <summary>
        /// 当鼠标悬停在宠物身上时应当变化的透明度，同时影响宠物本身和对话
        /// </summary>
        protected internal float mouseOpacity = 1f;

        /// <summary>
        /// 是否应当启用额外视觉效果，当 <see cref="mouseOpacity"/> >= 1 时为 true
        /// </summary>
        public bool ShouldExtraVFXActive => mouseOpacity >= 1f;

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
        /// 宠物所属玩家是否被Boss锁定为目标或其附近是否存在Boss
        /// </summary>
        public bool FindBoss => findBoss;

        /// <summary>
        /// 宠物的所属玩家
        /// </summary>
        public Player Owner => Main.player[Projectile.owner];

        /// <summary>
        /// 宠物的 owner 是否为 Main.myPlayer
        /// </summary>
        public bool OwnerIsMyPlayer => Projectile.owner == Main.myPlayer;
        #endregion

        #region 调试信息绘制方法
        private void DrawTestInfo()
        {
            bool drawingForTest = false;
            string chatTurn = "#";
            if (currentChatRoom != null)
                chatTurn = currentChatRoom.chatTurn.ToString();

            int chatIndex = -1;
            foreach (var t in ChatDictionary)
            {
                if (chatText != null && chatText.Equals(t.Value))
                {
                    chatIndex = t.Key;
                }
            }

            string testMsg1 = $"{chatCD}, {chatIndex}, {chatLag}, {chatTimeLeft}, {chatTurn}";
            string testMsg2 = $"{timeToType}, {totalTimeToType}, {Math.Round(chatOpacity, 1)}, {mainTimer}";
            string testMsg3 = $"{Projectile.localAI[0]}, {Projectile.localAI[1]}, {Projectile.localAI[2]}, {PetState}";
            string testMsg4 = $"{Projectile.ai[0]}, {Projectile.ai[1]}, {Projectile.ai[2]}, {Projectile.ShouldPetTalking()}";

            DrawStatePanelForTesting(drawingForTest, testMsg1, new Vector2(0, 0));
            DrawStatePanelForTesting(drawingForTest, testMsg2, new Vector2(0, 30));
            DrawStatePanelForTesting(drawingForTest, testMsg3, new Vector2(0, 60));
            DrawStatePanelForTesting(drawingForTest, testMsg4, new Vector2(0, 90));
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

        #region 查找方法
        private bool FindPet_Inner(out Projectile target, int minState, int maxState, bool checkTalkable, int type = -1, int id = 0)
        {
            target = null;
            if (maxState <= minState && minState > 0
                || maxState >= minState && minState < 0)
            {
                maxState = minState;
            }
            foreach (Projectile p in Main.ActiveProjectiles)
            {
                if (!p.IsATouhouPet())
                    continue;

                if (p.owner == Projectile.owner)
                {
                    bool findType = false;
                    if (type > 0)
                    {
                        findType = p.type == type;
                    }
                    else if (id > 0 && id < ModTouhouPetLoader.TotalCount)
                    {
                        findType = p.AsTouhouPet().TouhouPetType == id;
                    }
                    if (findType
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
        /// <param name="target">被查找的对象</param>
        /// <param name="type">宠物ID</param>
        /// <param name="minState">最小状态值（ai[1]），为-1时则将无视状态检测</param>
        /// <param name="maxState">最大状态值，默认等于最小状态值</param>
        /// <param name="checkTalkable">是否检测对应宠物应当说话</param>
        /// <returns></returns>
        internal bool FindPet(out Projectile target, int type, int minState = -1, int maxState = 0, bool checkTalkable = true)
        {
            FindPet_Inner(out target, minState, maxState, checkTalkable, type);
            return target != null;
        }
        /// <summary>
        /// 通过独特标识ID查找对应宠物
        /// </summary>
        /// <param name="target">被查找的对象</param>
        /// <param name="id">宠物独特标识ID</param>
        /// <param name="minState">最小状态值（ai[1]），为-1时则将无视状态检测</param>
        /// <param name="maxState">最大状态值，默认等于最小状态值</param>
        /// <param name="checkTalkable">是否检测对应宠物应当说话</param>
        /// <returns></returns>
        internal bool FindPetByUniqueID(out Projectile target, TouhouPetID id, int minState = -1, int maxState = 0, bool checkTalkable = true)
        {
            FindPet_Inner(out target, minState, maxState, checkTalkable, -1, (int)id);
            return target != null;
        }
        /// <summary>
        /// 通过拓展独特标识ID查找对应宠物
        /// </summary>
        /// <param name="target">被查找的对象</param>
        /// <param name="id">宠物独特标识ID</param>
        /// <param name="minState">最小状态值（ai[1]），为-1时则将无视状态检测</param>
        /// <param name="maxState">最大状态值，默认等于最小状态值</param>
        /// <param name="checkTalkable">是否检测对应宠物应当说话</param>
        /// <returns></returns>
        internal bool FindPetByPetType(out Projectile target, int id, int minState = -1, int maxState = 0, bool checkTalkable = true)
        {
            FindPet_Inner(out target, minState, maxState, checkTalkable, -1, id);
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
            return FindPet_Inner(out _, minState, maxState, checkTalkable, type);
        }
        /// <summary>
        /// 通过独特标识ID查找对应宠物
        /// </summary>
        /// <param name="type">宠物独特标识ID</param>
        /// <param name="minState">最小状态值（ai[1]），为-1时则将无视状态检测</param>
        /// <param name="maxState">最大状态值，默认等于最小状态值</param>
        /// <param name="checkTalkable">是否检测对应宠物应当说话</param>
        /// <returns></returns>
        internal bool FindPet(TouhouPetID id, bool checkTalkable = true, int minState = -1, int maxState = 0)
        {
            return FindPet_Inner(out _, minState, maxState, checkTalkable, -1, (int)id);
        }
        #endregion

        #region 移动方法
        /// <summary>
        /// 常规移动AI
        /// </summary>
        /// <param name="point">移动到的位置</param>
        /// <param name="speed">移动速度</param>
        protected internal void MoveToPoint(Vector2 point, float speed, Vector2 center = default)
        {
            if (center == default)
            {
                center = Owner.MountedCenter;
            }

            Vector2 pos = center + point;
            float dist = Vector2.Distance(Projectile.Center, pos);
            if (dist > 1200f)
                Projectile.Center = pos;

            Vector2 vel = pos - Projectile.Center;
            float closeValue = 1f;

            if (dist < closeValue)
                Projectile.velocity *= 0.25f;

            if (vel != Vector2.Zero)
            {
                if (vel.Length() < closeValue)
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
        protected internal void MoveToPoint2(Vector2 point, float speed)
        {
            Vector2 targetPos = Owner.Center + point;
            Vector2 targetVel = targetPos - Projectile.Center;

            float length = (targetPos == Vector2.Zero) ? 0f : Projectile.Distance(targetPos);
            float distanceLimit = MathHelper.Lerp(0f, speed, length / 200f);
            float scaledSpeed = MathHelper.SmoothStep(0f, speed, distanceLimit);

            if (scaledSpeed <= 0.1f)
                scaledSpeed = 0f;

            Projectile.velocity = targetVel.SafeNormalize(Vector2.One) * scaledSpeed;
        }
        /// <summary>
        /// 设置转向
        /// </summary>
        /// <param name="dist">设置与玩家同向的最小距离</param>
        protected internal void ChangeDir(float dist = 100)
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
                        if (PreFindBoss(b))
                        {
                            OnFindBoss(b, !BossChat_CrossMod(b.type, UniqueID));
                        }
                        findBoss = true;
                    }
                    ok = true;
                    break;
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
            Vector2 position = PetLightOnPlayer ? Owner.Center : Projectile.Center;
            Vector3 rgb = new(0, 0, 0);
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
                Rectangle projRect = new((int)projPos.X, (int)projPos.Y
                    , Projectile.width, Projectile.height);
                if (projRect.Contains(new Point(Main.mouseX, Main.mouseY)))
                {
                    bool dontInvis = false;
                    if (OnMouseHover(ref dontInvis))
                    {
                        if (Main.mouseRight && Main.mouseRightRelease)
                        {
                            OnMouseClick(false, true);
                        }
                        if (Main.mouseLeft && Main.mouseLeftRelease)
                        {
                            OnMouseClick(true, false);
                        }
                    }
                    if (!dontInvis && PetInvisWhenMouseHover)
                    {
                        mouseOpacity = MathHelper.Clamp(mouseOpacity - 0.1f, 0.05f, 1);
                    }
                }
                else
                {
                    mouseOpacity = MathHelper.Clamp(mouseOpacity + 0.1f, 0.05f, 1);
                }
            }
        }
        #endregion

        #region 自身重写函数
        /// <summary>
        /// 宠物的独特标识值
        /// </summary>
        public virtual TouhouPetID UniqueID => TouhouPetID.None;
        /// <summary>
        /// 设置宠物照明（于PostDraw中更新）
        /// </summary>
        /// <param name="position">发光位置，默认为宠物中心</param>
        /// <param name="rgb">颜色，单值最高为2.55、最低为0</param>
        /// <param name="inactive">何时不会发光，默认为false</param>
        public virtual void SetPetLight(ref Vector2 position, ref Vector3 rgb, ref bool inactive) { }
        /// <summary>
        /// 鼠标置于宠物之上时执行的方法，可控制是否能点击宠物（于PostDraw中更新）
        /// <br/>仅在本地端更新
        /// <br>默认返回 false</br>
        /// </summary>
        /// <param name="dontInvis">何时应当无视鼠标悬停时隐形的规则</param>
        /// <returns>返回 false 时，阻断<see cref="OnMouseClick(bool, bool)"/>的执行</returns>
        public virtual bool OnMouseHover(ref bool dontInvis) => false;
        /// <summary>
        /// 点击宠物时执行的方法，需要OnMouseHover返回true才能执行
        /// <br/>仅在本地端更新
        /// </summary>
        /// <param name="leftMouse">左键点击</param>
        /// <param name="rightMouse">右键点击</param>
        public virtual void OnMouseClick(bool leftMouse, bool rightMouse) { }
        /// <summary>
        /// <see cref="OnFindBoss(NPC, bool)"/>前执行的内容，返回 false 时将阻断其执行
        /// <br>默认返回 true</br>
        /// </summary>
        /// <param name="boss"></param>
        /// <returns></returns>
        public virtual bool PreFindBoss(NPC boss) => true;
        /// <summary>
        /// 当Boss出场的一刻间执行的方法
        /// </summary>
        /// <param name="boss"></param>
        /// <param name="noReaction">是否没有出现跨模组添加的反应</param>
        public virtual void OnFindBoss(NPC boss, bool noReaction) { }
        /// <summary>
        /// 对话设置结构体
        /// </summary>
        public virtual ChatSettingConfig ChatSettingConfig => new();
        /// <summary>
        /// 视觉效果，用于常驻动画表现（包含玩家选择界面）
        /// <br/>对于设置了 <see cref="ProjectileID.Sets.CharacterPreviewAnimations"/> 的宠物，待机状态的动画不应写在这里
        /// </summary>
        public virtual void VisualEffectForPreview() { }
        /// <summary>
        /// 绘制宠物，替代PreDraw
        /// <br>默认返回 true</br>
        /// </summary>
        /// <param name="lightColor"></param>
        /// <returns></returns>
        public virtual bool DrawPetSelf(ref Color lightColor) => true;

        /// <summary>
        /// 设置种类属性，替代SetStaticDefaults
        /// </summary>
        public virtual void PetStaticDefaults() { }

        /// <summary>
        /// 设置实例属性，替代SetDefaults
        /// </summary>
        public virtual void PetDefaults() { }
        #endregion

        #region 原有重写函数
        public override void Load()
        {
            RegisterToModPetLoader();
        }

        public override void SetStaticDefaults()
        {
            PetStaticDefaults();
            RegisterChat_Full();
        }
        public override void SetDefaults()
        {
            Projectile.netImportant = true;
            Projectile.width = 50;
            Projectile.height = 50;
            Projectile.friendly = true;
            Projectile.penetrate = -1;
            Projectile.ignoreWater = true;
            Projectile.timeLeft *= 5;
            SetTouhouPetType();
            PetDefaults();
            DynamicRegisterForDebug();
        }
        public override bool PreAI()
        {
            if (++mainTimer > 4800)
            {
                mainTimer = 0;
            }
            if (!Owner.active)
            {
                Projectile.active = false;
                return false;
            }
            UpdateChat_Full();
            return true;
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
            //Projectile.ResetDrawStateForPet();//用于让染料正常工作
            return DrawPetSelf(ref lightColor);
        }
        public override void PostDraw(Color lightColor)
        {
            Projectile.ResetDrawStateForPet();

            if (chatOpacity > 0 && PetChatFrequency > 0f)
            {
                Vector2 drawPos = Projectile.position - Main.screenPosition + new Vector2(Projectile.width / 2, -20) + new Vector2(0, 7f * Main.essScale);
                float alpha = MathHelper.Clamp(chatOpacity * Projectile.Opacity * mouseOpacity, 0, 1);
                if (textShaking)
                {
                    DrawChatText_Koishi(drawPos, alpha);
                }
                else
                {
                    DrawChatText(drawPos, alpha);
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
