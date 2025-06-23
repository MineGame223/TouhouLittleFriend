using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;
using Terraria;
using Terraria.ID;
using Terraria.Localization;
using Terraria.Utilities;
using TouhouPets.Content.Buffs.PetBuffs;

namespace TouhouPets.Content.Projectiles.Pets
{
    /// <summary>
    /// 示范好孩子琪露诺
    /// </summary>
    public class Cirno : BasicTouhouPet
    {
        private enum States
        {
            Idle,
            Blink,
            Laughing,
            AfterLaughing,
            Hot,
            HotBlink,
        }
        private States CurrentState
        {
            get => (States)PetState;
            set => PetState = (int)value;
        }
        private int ActionCD
        {
            get => (int)Projectile.localAI[0];
            set => Projectile.localAI[0] = value;
        }
        private int Timer
        {
            get => (int)Projectile.localAI[1];
            set => Projectile.localAI[1] = value;
        }
        private int RandomCount
        {
            get => (int)Projectile.localAI[2];
            set => Projectile.localAI[2] = value;
        }
        private bool IsIdleState => PetState <= 1;
        private bool IsHotState => CurrentState == States.Hot || CurrentState == States.HotBlink;
        private bool InHotZone => (Owner.ZoneDesert && Main.dayTime)
            || Owner.ZoneUnderworldHeight || Owner.ZoneJungle;
        private bool CanSeeFrogs => Owner.ZoneJungle && Owner.ZoneOverworldHeight;

        private int wingFrame, wingFrameCounter;
        private bool useSummerSkin;

        private DrawPetConfig drawConfig = new(2);
        private readonly Texture2D clothTex = AltVanillaFunction.GetExtraTexture("Cirno_Cloth");
        public override void SetStaticDefaults()
        {
            Main.projFrames[Type] = 11;
            Main.projPet[Type] = true;
            ProjectileID.Sets.LightPet[Type] = true;
        }
        public override TouhouPetID UniqueID => TouhouPetID.Cirno;
        public override bool DrawPetSelf(ref Color lightColor)
        {
            DrawPetConfig config = drawConfig with
            {
                AltTexture = clothTex,
                ShouldUseEntitySpriteDraw = true,
            };
            Projectile.DrawPet(wingFrame, lightColor, drawConfig);
            Projectile.DrawPet(wingFrame, Color.White * 0.5f, drawConfig);

            Projectile.DrawPet(Projectile.frame, lightColor, drawConfig);

            if (useSummerSkin)
            {
                Projectile.DrawPet(Projectile.frame, lightColor,
                    drawConfig with
                    {
                        AltTexture = clothTex,
                    });
                Projectile.DrawPet(Projectile.frame, lightColor, drawConfig, 1);
            }
            else
            {
                Projectile.DrawPet(Projectile.frame, lightColor, config);
            }
            return false;
        }
        public override ChatSettingConfig ChatSettingConfig => new ChatSettingConfig() with
        {
            TextColor = new Color(76, 207, 239),
        };
        public override void RegisterChat(ref string name, ref Vector2 indexRange)
        {
            name = "Cirno";
            indexRange = new Vector2(1, 12);
        }
        public override void SetRegularDialog(ref int timePerDialog, ref int chance, ref bool whenShouldStop)
        {
            timePerDialog = 480;//480
            chance = 9;//9
            whenShouldStop = CurrentState == States.Laughing || CurrentState == States.AfterLaughing;
        }
        public override WeightedRandom<LocalizedText> RegularDialogText()
        {
            WeightedRandom<LocalizedText> chat = new ();
            {
                if (IsHotState)
                {
                    chat.Add(ChatDictionary[7]);
                }
                else
                {
                    chat.Add(ChatDictionary[1]);
                    chat.Add(ChatDictionary[2]);
                    chat.Add(ChatDictionary[3]);

                    if (FindPet(ProjectileType<Daiyousei>()))//查找玩家是否同时携带了大妖精
                    {
                        chat.Add(ChatDictionary[4]);
                    }

                    chat.Add(ChatDictionary[6]);
                    if (CanSeeFrogs)
                    {
                        chat.Add(ChatDictionary[11], 3);//该文本的权重为3，即更大概率出现
                    }
                }
            }
            return chat;
        }
        public override List<List<ChatRoomInfo>> RegisterChatRoom()
        {
            return new()
            {
                Chatting1(),
                Chatting2()
            };
        }

        /// <summary>
        /// 一份对话信息列表
        /// <br>对话系统原理如下：</br>
        /// <br>将包含对话信息的列表作为元素加入二维列表 <see cref="RegisterChatRoom"/> 中，每份列表中的对话信
        /// 息包括宠物独特ID、本回合对话文本与对话应当出现的回合数。</br>
        /// <br>宠物的AI中会持续遍历二维列表，若发现宠物当前对话的键等于对话信息列表中首条
        /// 对话信息（即聊天发起者的信息）的键，则会创建一个聊天室并进行编辑。</br>
        /// <br>开始编辑时，会根据输入的信息列表自动生成成员独特ID列表并统计该对话的总回合数。
        /// 宠物会根据ID列表遍历玩家持有的所有东方宠物，若发现其独特ID被包含在列表中，则该宠物的实例将被加入聊天室。</br>
        /// <br>期间，如果在列表中的宠物被发现不存在，聊天室会立刻关闭。</br>
        /// <br>将实例加入聊天室后，会再生成一份包括[成员实例、对话文本、所属回合数]的元组列表并进行遍历，
        /// 若实例对应的所属回合数与当前回合数相匹配，则进行对话。聊天的发起者在第一回合时将不会说话。</br>
        /// <br>当前回合内的宠物若已说完话，当前回合数将会自动 +1 以进入下一回合。
        /// 当本回合内存在多个说话者时，以排在第一位的说话者为准。</br>
        /// <br>若当前回合数已超过对话最终回合，则聊天室将被关闭。</br>
        /// <br>无论如何，聊天室关闭的同时，参与聊天的所有宠物的 currentChatRoom 将设为空。</br>
        /// </summary>
        private List<ChatRoomInfo> Chatting1()
        {
            TouhouPetID cirno = TouhouPetID.Cirno;
            TouhouPetID daiyousei = TouhouPetID.Daiyousei;

            //设置对话相关信息的结构体列表，结构参数依次为：参与聊天的宠物独特ID，对话文本，对话所在的回合数
            List<ChatRoomInfo> list =
            [
                new ChatRoomInfo(cirno, ChatDictionary[4], -1),//琪露诺：最喜欢大酱了！
                new ChatRoomInfo(daiyousei, GetChatText("Daiyousei",7), 0),//大妖精：我也最喜欢琪露诺酱！
            ];

            return list;
        }
        private List<ChatRoomInfo> Chatting2()
        {
            TouhouPetID cirno = TouhouPetID.Cirno;
            TouhouPetID daiyousei = TouhouPetID.Daiyousei;

            List<ChatRoomInfo> list =
            [
                new ChatRoomInfo(cirno, ChatDictionary[7], -1),//琪露诺：热死了...要化了...
                new ChatRoomInfo(daiyousei, GetChatText("Daiyousei",8), 0),//大妖精：琪露诺酱你没事吧...
                new ChatRoomInfo(cirno, ChatDictionary[8], 1),//琪露诺：我没事...大概...
            ];

            return list;
        }
        public override void VisualEffectForPreview()
        {
            UpdateWingFrame();
        }
        public override void SetPetLight(ref Vector2 position, ref Vector3 rgb, ref bool inactive)
        {
            rgb = new Vector3(0.57f, 1.61f, 1.84f);
            inactive = IsHotState;
        }
        public override void AI()
        {
            Projectile.SetPetActive(Owner, BuffType<CirnoBuff>());

            ControlMovement();

            if (Owner.ZoneUnderworldHeight && !IsHotState && !useSummerSkin)
            {
                CurrentState = States.Hot;
            }

            switch (CurrentState)
            {
                case States.Blink:
                    Blink();
                    break;

                case States.Laughing:
                    shouldNotTalking = true;
                    Laughing();
                    break;

                case States.AfterLaughing:
                    shouldNotTalking = true;
                    AfterLaughing();
                    break;

                case States.Hot:
                    shouldNotTalking = true;
                    Hot();
                    break;

                case States.HotBlink:
                    shouldNotTalking = true;
                    HotBlink();
                    break;

                default:
                    Idle();
                    break;
            }

            if (IsIdleState && ActionCD > 0)
            {
                ActionCD--;
            }
            useSummerSkin = Owner.miscDyes[1].type == ItemID.BrownDye;
        }
        private void ControlMovement()
        {
            Projectile.tileCollide = false;
            Projectile.rotation = Projectile.velocity.X * 0.032f;

            ChangeDir();

            Vector2 point = new Vector2(40 * Owner.direction, -30 + Owner.gfxOffY);
            MoveToPoint(point, 9f);
        }
        private void MeltingDust()
        {
            if (Main.rand.NextBool(12))
            {
                for (int i = 0; i < Main.rand.Next(1, 4); i++)
                {
                    Dust.NewDustPerfect(Projectile.position + new Vector2(Main.rand.Next(10, Projectile.width - 10), Main.rand.Next(10, Projectile.height - 10)),
                        MyDustId.Water, null, 100, Color.White).scale = Main.rand.NextFloat(0.5f, 1.2f);
                }
            }
        }
        private void Idle()
        {
            Projectile.frame = 0;
            if (OwnerIsMyPlayer)
            {
                if (mainTimer % 270 == 0)
                {
                    CurrentState = States.Blink;
                }
                bool canLaugh = !InHotZone || (useSummerSkin && !Owner.ZoneUnderworldHeight);

                if (mainTimer > 0 && mainTimer % 600 == 0 && canLaugh
                    && currentChatRoom == null && ActionCD <= 0)
                {
                    if (Main.rand.NextBool(3))
                    {
                        RandomCount = Main.rand.Next(10, 20);
                        CurrentState = States.Laughing;
                    }
                }
            }
        }
        private void Blink()
        {
            if (++Projectile.frameCounter > 3)
            {
                Projectile.frameCounter = 0;
                Projectile.frame++;
            }
            if (Projectile.frame > 2)
            {
                Projectile.frame = 0;
                CurrentState = States.Idle;
            }
        }
        private void Laughing()
        {
            int count = 12;
            if (Projectile.frame == 3)
            {
                count = 7;
            }
            if (Projectile.frame < 3)
            {
                Projectile.frame = 3;
            }
            if (++Projectile.frameCounter > count)
            {
                Projectile.frameCounter = 0;
                Projectile.frame++;
            }
            if (Projectile.frame > 5)
            {
                Projectile.frame = 4;
                Timer++;
            }
            if (OwnerIsMyPlayer && Timer > RandomCount)//拥有随机数的操作一般需要在本端选择完成后同步到其他端
            {
                Timer = 0;
                CurrentState = States.AfterLaughing;
            }
        }
        private void AfterLaughing()
        {
            if (++Projectile.frameCounter > 7)
            {
                Projectile.frameCounter = 0;
                Projectile.frame++;
            }
            if (Projectile.frame > 6)
            {
                Projectile.frame = 0;
                if (OwnerIsMyPlayer)
                {
                    ActionCD = 600;
                    CurrentState = States.Idle;
                }
            }
        }
        private void Hot()
        {
            if (ShouldExtraVFXActive)
                MeltingDust();

            Projectile.frame = 1;
            if (OwnerIsMyPlayer && mainTimer % 270 == 0)
            {
                CurrentState = States.HotBlink;
            }
            if (!Owner.ZoneUnderworldHeight || useSummerSkin)
            {
                CurrentState = States.Idle;
            }
        }
        private void HotBlink()
        {
            if (ShouldExtraVFXActive)
                MeltingDust();

            if (++Projectile.frameCounter > 3)
            {
                Projectile.frameCounter = 0;
                Projectile.frame++;
            }
            if (Projectile.frame > 2)
            {
                Projectile.frame = 1;
                CurrentState = States.Hot;
            }
        }
        private void UpdateWingFrame()
        {
            if (wingFrame < 7)
            {
                wingFrame = 7;
            }
            int count = IsHotState ? 8 : 4;
            if (++wingFrameCounter > count)
            {
                wingFrameCounter = 0;
                wingFrame++;
            }
            if (wingFrame > 10)
            {
                wingFrame = 7;
            }
        }
    }
}


