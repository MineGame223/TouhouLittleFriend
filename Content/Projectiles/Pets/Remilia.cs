using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ID;
using Terraria.Utilities;
using TouhouPets.Content.Buffs.PetBuffs;

namespace TouhouPets.Content.Projectiles.Pets
{
    public class Remilia : BasicTouhouPetNeo
    {
        private enum States
        {
            Idle,
            Blink,
            Defense,
            Drinking,
            DrinkingBreak,
            AfterDrinking,
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
        private bool IsIdleState => CurrentState <= States.Blink;
        private bool IsDrinkingState => CurrentState >= States.Drinking && CurrentState <= States.DrinkingBreak;

        private int drinkTimer, drinkRandomCount;
        private int wingFrame, wingFrameCounter;
        private int blinkFrame, blinkFrameCounter;
        private int clothFrame, clothFrameCounter;
        private int extraAdjX, extraAdjY;

        private DrawPetConfig drawConfig = new(1);
        private readonly Texture2D clothTex = AltVanillaFunction.GetExtraTexture("Remilia_Cloth");
        public override void SetStaticDefaults()
        {
            Main.projFrames[Type] = 25;
            Main.projPet[Type] = true;
            ProjectileID.Sets.LightPet[Type] = false;
        }
        public override bool PreDraw(ref Color lightColor)
        {
            Vector2 extraOffset = new Vector2(extraAdjX, extraAdjY);
            Vector2 shake = new Vector2(Main.rand.Next(-1, 1), Main.rand.Next(-1, 1));
            DrawPetConfig config = drawConfig with
            {
                ShouldUseEntitySpriteDraw = true,
            };

            Projectile.DrawPet(wingFrame, lightColor,
                drawConfig with
                {
                    PositionOffset = extraOffset,
                });

            Projectile.DrawPet(Projectile.frame, lightColor, drawConfig);

            if (CurrentState == States.Blink)
                Projectile.DrawPet(blinkFrame, lightColor, drawConfig);

            Projectile.DrawPet(Projectile.frame, lightColor,
                config with
                {
                    AltTexture = clothTex,
                    PositionOffset = ShouldDefense(Projectile) ? shake : Vector2.Zero,
                });

            if (CurrentState != States.Defense)
                Projectile.DrawPet(clothFrame, lightColor,
                config with
                {
                    PositionOffset = extraOffset,
                });
            return false;
        }
        public static bool ShouldDefense(Projectile projectile)
        {
            Player player = Main.player[projectile.owner];
            bool sunlight = Main.dayTime && (player.ZoneOverworldHeight || player.ZoneSkyHeight) && !player.behindBackWall;
            bool rain = Main.raining && (player.ZoneOverworldHeight || player.ZoneSkyHeight);
            if (sunlight || rain)
            {
                bool isRemilia = projectile.type == ProjectileType<Remilia>()
                    && projectile.ToPetClass().FindPet(ProjectileType<Sakuya>(), false);
                bool isFlandre = projectile.type == ProjectileType<Flandre>()
                    && projectile.ToPetClass().FindPet(ProjectileType<Meirin>(), false);
                if (isRemilia || isFlandre)
                    return false;
                else
                    return true;
            }
            return false;
        }
        public override Color ChatTextColor => new Color(255, 10, 10);
        public override void RegisterChat(ref string name, ref Vector2 indexRange)
        {
            name = "Remilia";
            indexRange = new Vector2(1, 14);
        }
        public override void SetRegularDialog(ref int timePerDialog, ref int chance, ref bool whenShouldStop)
        {
            timePerDialog = 720;
            chance = 9;
            whenShouldStop = !IsIdleState;
        }
        public override string GetRegularDialogText()
        {
            WeightedRandom<string> chat = new WeightedRandom<string>();
            {
                chat.Add(ChatDictionary[1]);
                chat.Add(ChatDictionary[2]);
                if (Main.bloodMoon)
                {
                    chat.Add(ChatDictionary[5]);
                }
                if (FindPet(ProjectileType<Flandre>()))
                {
                    chat.Add(ChatDictionary[6]);
                }
            }
            return chat;
        }
        public override void VisualEffectForPreview()
        {
            UpdateWingFrame();
            UpdateClothFrame();
        }
        private void UpdateTalking()
        {
            if (FindChatIndex(6, 8))
            {
                Chatting1(currentChatRoom ?? Projectile.CreateChatRoomDirect());
            }
        }
        private void Chatting1(PetChatRoom chatRoom)
        {
            int type = ProjectileType<Flandre>();
            if (FindPet(out Projectile member, type))
            {
                chatRoom.member[0] = member;
                member.ToPetClass().currentChatRoom = chatRoom;
            }
            else
            {
                chatRoom.CloseChatRoom();
                return;
            }
            Projectile remilia = chatRoom.initiator;
            Projectile flandre = chatRoom.member[0];
            int turn = chatRoom.chatTurn;
            if (turn == -1)
            {
                //蕾米：我亲爱的芙兰哟...
                flandre.CloseCurrentDialog();

                if (remilia.CurrentDialogFinished())
                    chatRoom.chatTurn++;
            }
            else if (turn == 0)
            {
                //芙兰：姐姐？叫芙兰有什么事嘛？
                flandre.SetChat(ChatSettingConfig, 6, 20);

                if (flandre.CurrentDialogFinished())
                    chatRoom.chatTurn++;
            }
            else if (turn == 1)
            {
                //蕾米：没什么...只是想叫你一下。
                remilia.SetChat(ChatSettingConfig, 7, 20);

                if (remilia.CurrentDialogFinished())
                    chatRoom.chatTurn++;
            }
            else if (turn == 2)
            {
                //芙兰：...姐姐什么时候能和芙兰一起玩...
                flandre.SetChat(ChatSettingConfig, 7, 20);

                if (flandre.CurrentDialogFinished())
                    chatRoom.chatTurn++;
            }
            else if (turn == 3)
            {
                //蕾米：有空会陪你的啦~
                remilia.SetChat(ChatSettingConfig, 8, 20);

                if (remilia.CurrentDialogFinished())
                    chatRoom.chatTurn++;
            }
            else if (turn == 4)
            {
                //芙兰：姐姐老是这么说...
                flandre.SetChat(ChatSettingConfig, 8, 20);

                if (flandre.CurrentDialogFinished())
                    chatRoom.chatTurn++;
            }
            else
            {
                chatRoom.CloseChatRoom();
            }
        }
        public override void AI()
        {
            Projectile.SetPetActive(Owner, BuffType<RemiliaBuff>());
            Projectile.SetPetActive(Owner, BuffType<ScarletBuff>());

            UpdateTalking();

            ControlMovement(Owner);

            if (ShouldDefense(Projectile) && CurrentState != States.Defense)
            {
                if (OwnerIsMyPlayer)
                {
                    Timer = 0;
                    CurrentState = States.Defense;
                    return;
                }
            }

            switch (CurrentState)
            {
                case States.Blink:
                    Blink();
                    break;

                case States.Defense:
                    Defense();
                    break;

                case States.Drinking:
                    shouldNotTalking = true;
                    Drinking();
                    break;

                case States.DrinkingBreak:
                    shouldNotTalking = true;
                    DrinkingBreak();
                    break;

                case States.AfterDrinking:
                    shouldNotTalking = true;
                    AfterDrinking();
                    break;

                default:
                    Idle();
                    break;
            }

            if (IsIdleState && ActionCD > 0)
            {
                ActionCD--;
            }

            UpdateMiscData();
        }
        private void UpdateMiscData()
        {
            extraAdjX = 0;
            extraAdjY = 0;
            if (Projectile.frame >= 1 && Projectile.frame <= 10)
            {
                extraAdjY = -2;
                if (Projectile.frame != 10)
                {
                    extraAdjX = -2 * Projectile.spriteDirection;
                }
            }
        }
        private void ControlMovement(Player player)
        {
            Projectile.tileCollide = false;
            if (IsDrinkingState)
                Projectile.rotation = Projectile.velocity.X * 0.03f;
            else
                Projectile.rotation = Projectile.velocity.X * 0.005f;

            Vector2 point = new Vector2(-50 * player.direction, -50 + player.gfxOffY);
            if (FindPet(ProjectileType<Flandre>(), false))
            {
                point = new Vector2(50 * player.direction, -50 + player.gfxOffY);
            }
            if (player.HasBuff<ScarletBuff>())
            {
                point = new Vector2(60 * player.direction, -20 + player.gfxOffY);
            }

            ChangeDir();
            MoveToPoint(point, 19f);
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
                if (mainTimer > 0 && mainTimer % 900 == 0 && currentChatRoom == null && ActionCD <= 0
                     && Owner.velocity.Length() < 4f)
                {
                    if (Main.rand.NextBool(3))
                    {
                        RandomCount = Main.rand.Next(120, 360);
                        drinkRandomCount = Main.rand.Next(3, 9);
                        CurrentState = States.Drinking;
                    }
                }
            }
        }
        private void Blink()
        {
            if (blinkFrame < 22)
            {
                blinkFrame = 22;
            }
            if (++blinkFrameCounter > 3)
            {
                blinkFrameCounter = 0;
                blinkFrame++;
            }
            if (blinkFrame > 24)
            {
                blinkFrame = 22;
                CurrentState = States.Idle;
            }
        }
        private void Drinking()
        {
            if (++Projectile.frameCounter > 9)
            {
                Projectile.frameCounter = 0;
                Projectile.frame++;
            }
            if (Projectile.frame >= 5)
            {
                Projectile.frame = 5;
                Timer++;
            }
            if (OwnerIsMyPlayer && Timer > RandomCount)
            {
                Timer = 0;
                RandomCount = Main.rand.Next(120, 360);
                CurrentState = States.DrinkingBreak;
            }
        }
        private void DrinkingBreak()
        {
            if (++Projectile.frameCounter > 9)
            {
                Projectile.frameCounter = 0;
                Projectile.frame++;
            }
            if (Projectile.frame >= 6)
            {
                Projectile.frame = 6;
                Timer++;
            }
            if (OwnerIsMyPlayer && Timer > RandomCount)
            {
                Timer = 0;
                drinkTimer++;
                if (drinkTimer > drinkRandomCount)
                {
                    drinkTimer = 0;
                    CurrentState = States.AfterDrinking;
                }
                else
                {
                    if (Main.rand.NextBool(3) && currentChatRoom == null && Projectile.CurrentDialogFinished())
                    {
                        int chance = Main.rand.Next(2);
                        switch (chance)
                        {
                            case 1:
                                Projectile.SetChat(ChatSettingConfig, 4, 20);
                                break;
                            default:
                                Projectile.SetChat(ChatSettingConfig, 3, 20);
                                break;
                        }
                    }
                    CurrentState = States.Drinking;
                }
            }
        }
        private void AfterDrinking()
        {
            if (++Projectile.frameCounter > 9)
            {
                Projectile.frameCounter = 0;
                Projectile.frame++;
            }
            if (Projectile.frame > 10)
            {
                Projectile.frame = 0;
                if (OwnerIsMyPlayer)
                {
                    ActionCD = 1800;
                    CurrentState = States.Idle;
                }
            }
        }
        private void Defense()
        {
            Projectile.rotation = 0f;
            Projectile.frame = 11;
            if (!ShouldDefense(Projectile) && OwnerIsMyPlayer)
            {
                CurrentState = States.Idle;
            }
        }
        private void UpdateWingFrame()
        {
            if (wingFrame < 16)
            {
                wingFrame = 16;
            }
            int count = 6;
            if (++wingFrameCounter > count)
            {
                wingFrameCounter = 0;
                wingFrame++;
            }
            if (wingFrame > 21)
            {
                wingFrame = 16;
            }
        }
        private void UpdateClothFrame()
        {
            if (clothFrame < 12)
            {
                clothFrame = 12;
            }
            int count = 4;
            if (++clothFrameCounter > count)
            {
                clothFrameCounter = 0;
                clothFrame++;
            }
            if (clothFrame > 15)
            {
                clothFrame = 12;
            }
        }
    }
}


