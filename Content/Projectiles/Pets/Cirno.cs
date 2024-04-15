using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ID;
using Terraria.Utilities;
using TouhouPets.Content.Buffs.PetBuffs;

namespace TouhouPets.Content.Projectiles.Pets
{
    /// <summary>
    /// 示范好孩子琪露诺
    /// </summary>
    public class Cirno : BasicTouhouPetNeo
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

        private DrawPetConfig drawConfig = new(1);
        private readonly Texture2D clothTex = AltVanillaFunction.GetExtraTexture("Cirno_Cloth");
        public override void SetStaticDefaults()
        {
            Main.projFrames[Type] = 11;
            Main.projPet[Type] = true;
            ProjectileID.Sets.LightPet[Type] = true;
        }
        public override bool PreDraw(ref Color lightColor)
        {
            DrawPetConfig config = drawConfig with
            {
                AltTexture = clothTex,
                ShouldUseEntitySpriteDraw = true,
            };
            Projectile.DrawPet(wingFrame, Projectile.GetAlpha(Color.White * 0.7f), drawConfig);
            Projectile.DrawPet(Projectile.frame, lightColor, drawConfig);
            Projectile.DrawPet(Projectile.frame, lightColor, config);
            return false;
        }
        public override Color ChatTextColor => new Color(76, 207, 239);
        public override void RegisterChat(ref string name, ref Vector2 indexRange)
        {
            name = "Cirno";
            indexRange = new Vector2(1, 12);
        }
        public override void SetRegularDialog(ref int timePerDialog, ref int chance, ref bool whenShouldStop)
        {
            timePerDialog = 480;
            chance = 9;
            whenShouldStop = CurrentState == States.Laughing || CurrentState == States.AfterLaughing;
        }
        public override string GetRegularDialogText()
        {
            WeightedRandom<string> chat = new WeightedRandom<string>();
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
        public override void VisualEffectForPreview()
        {
            UpdateWingFrame();
        }
        /// <summary>
        /// 执行对话过程
        /// <br>对话系统原理如下：</br>
        /// <br>随时检测自身可以引起对话回合的对话索引，若成功检测到则立刻创建一个聊天室，并将其赋值给 currentChatRoom。</br>
        /// <br>创建成功后，若检测到玩家携带有相关宠物，则该宠物会被拉入聊天室作为其中一员，其自身的 currentChatRoom 也会被赋值；
        /// 否则如果没有检测到（比如宠物中途消失），则聊天室将被立刻关闭。</br>
        /// <br>同时，currentChatRoom 非空的情况下将不再更新常规对话，以确保对话回合不会被打扰。</br>
        /// <br>聊天室依靠 chatTurn 变量进行对话回合的切换，每当当前回合内宠物的话说完后，chatTurn 应当+1以进入下一回合；
        /// chatTurn的初始值为-1，该回合用于等待宠物说完第一句话。</br>
        /// <br>若chatTrun已超过对话最终回合，则聊天室将被关闭。</br>
        /// <br>无论如何，聊天室关闭的同时，参与聊天的所有宠物的chatIndex将归零、其 currentChatRoom 也将设为空。</br>
        /// </summary>
        private void UpdateTalking()
        {
            if (FindChatIndex(4) || FindChatIndex(7, 8))//请确保这里包含了所有该宠物在对话期间使用到的对话索引
            {
                Chatting1(currentChatRoom ?? Projectile.CreateChatRoomDirect(), chatIndex);
            }
        }
        //由自己发起的对话过程（与大妖精）
        private void Chatting1(PetChatRoom chatRoom, int index)
        {
            int type = ProjectileType<Daiyousei>();
            if (FindPet(out Projectile member, type))//查找玩家是否携带大妖精
            {
                //将大妖精拉入聊天室
                chatRoom.member[0] = member;
                member.ToPetClass().currentChatRoom = chatRoom;
            }
            else//否则立刻关闭聊天室
            {
                chatRoom.CloseChatRoom();
                return;
            }
            Projectile cirno = chatRoom.initiator;
            Projectile daiyousei = chatRoom.member[0];
            int turn = chatRoom.chatTurn;
            if (index == 4)//对话1，关联索引为4
            {
                if (turn == -1)//起始回合
                {
                    //琪露诺：最喜欢大酱了！
                    daiyousei.CloseCurrentDialog();//将成员现有的对话关闭，以最大限度确保对话流畅性

                    if (cirno.CurrentDialogFinished())//当琪露诺的话说完时进入下一回合
                        chatRoom.chatTurn++;
                }
                else if (turn == 0)
                {
                    //大妖精：我也最喜欢琪露诺酱！
                    daiyousei.SetChat(ChatSettingConfig, 7, 20);//令大妖精说话

                    if (daiyousei.CurrentDialogFinished())//当大妖精的话说完时进入下一回合
                        chatRoom.chatTurn++;
                }
                else//对话回合完成后退出聊天室
                {
                    chatRoom.CloseChatRoom();
                }
            }
            else if (index == 7 || index == 8)//对话2，关联索引为7、8
            {
                if (turn == -1)//起始回合
                {
                    //琪露诺：热死了...要化了...
                    daiyousei.CloseCurrentDialog();

                    if (cirno.CurrentDialogFinished())
                        chatRoom.chatTurn++;
                }
                else if (turn == 0)
                {
                    //大妖精：琪露诺酱你没事吧...
                    daiyousei.SetChat(ChatSettingConfig, 8, 20);

                    if (daiyousei.CurrentDialogFinished())
                        chatRoom.chatTurn++;
                }
                else if (turn == 1)
                {
                    //琪露诺：我没事...大概...
                    cirno.SetChat(ChatSettingConfig, 8, 20);//令琪露诺说话

                    if (cirno.CurrentDialogFinished())
                        chatRoom.chatTurn++;
                }
                else
                {
                    chatRoom.CloseChatRoom();
                }
            }
        }
        public override void AI()
        {
            Projectile.SetPetActive(Owner, BuffType<CirnoBuff>());

            UpdateTalking();

            ControlMovement();

            if (Owner.ZoneUnderworldHeight && !IsHotState)
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

            Lighting.AddLight(Projectile.Center, 0.57f, 1.61f, 1.84f);
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
                if (mainTimer > 0 && mainTimer % 600 == 0 && !InHotZone
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
            MeltingDust();
            Projectile.frame = 1;
            if (OwnerIsMyPlayer && mainTimer % 270 == 0)
            {
                CurrentState = States.HotBlink;
            }
            if (!Owner.ZoneUnderworldHeight)
            {
                CurrentState = States.Idle;
            }
        }
        private void HotBlink()
        {
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


