using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ID;
using Terraria.Utilities;
using TouhouPets.Content.Buffs.PetBuffs;

namespace TouhouPets.Content.Projectiles.Pets
{
    public class Tenshi : BasicTouhouPet
    {
        private enum States
        {
            Idle,
            Blink,
            Eating,
            EatingToBreak,
            Break,
            AfterEating,
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
        private int EatingCount
        {
            get => (int)Projectile.ai[2];
            set => Projectile.ai[2] = value;
        }
        private bool IsIdleState => CurrentState <= States.Blink;

        private int stoneFrame, stoneFrameCounter;
        private int blinkFrame, blinkFrameCounter;
        private int clothFrame, clothFrameCounter;
        private int randomCount2, randomCount3;

        private DrawPetConfig drawConfig = new(2);
        private readonly Texture2D clothTex = AltVanillaFunction.GetExtraTexture("Tenshi_Cloth");
        public override void SetStaticDefaults()
        {
            Main.projFrames[Type] = 14;
            Main.projPet[Type] = true;
            ProjectileID.Sets.LightPet[Type] = false;
        }
        public override bool DrawPetSelf(ref Color lightColor)
        {
            DrawPetConfig config = drawConfig with
            {
                ShouldUseEntitySpriteDraw = true,
                AltTexture = clothTex,
            };

            Projectile.DrawPet(stoneFrame, lightColor, drawConfig, 1);
            Projectile.DrawPet(stoneFrame, lightColor, config, 1);
            Projectile.ResetDrawStateForPet();

            Projectile.DrawPet(clothFrame + 4, lightColor, drawConfig, 1);
            Projectile.DrawPet(clothFrame + 4, lightColor, config, 1);
            Projectile.ResetDrawStateForPet();

            Projectile.DrawPet(Projectile.frame, lightColor, drawConfig);

            if (CurrentState == States.Blink)
                Projectile.DrawPet(blinkFrame, lightColor, drawConfig);

            Projectile.DrawPet(Projectile.frame, lightColor, config);
            Projectile.DrawPet(clothFrame, lightColor,
                drawConfig with
                {
                    ShouldUseEntitySpriteDraw = true,
                }, 1);
            return false;
        }
        public override Color ChatTextColor => new Color(69, 170, 234);
        public override void RegisterChat(ref string name, ref Vector2 indexRange)
        {
            name = "Tenshin";
            indexRange = new Vector2(1, 9);
        }
        public override void SetRegularDialog(ref int timePerDialog, ref int chance, ref bool whenShouldStop)
        {
            timePerDialog = 740;
            chance = 7;
            whenShouldStop = !IsIdleState;
        }
        public override string GetRegularDialogText()
        {
            WeightedRandom<string> chat = new WeightedRandom<string>();
            {
                chat.Add(ChatDictionary[1]);
                chat.Add(ChatDictionary[2]);
                chat.Add(ChatDictionary[3]);
                chat.Add(ChatDictionary[4]);
                chat.Add(ChatDictionary[5]);
                chat.Add(ChatDictionary[6]);
                chat.Add(ChatDictionary[9]);
            }
            return chat;
        }
        public override void OnFindBoss(NPC boss)
        {
            Projectile.SetChat(ChatSettingConfig, 8);
        }
        public override void VisualEffectForPreview()
        {
            UpdateStoneFrame();
            UpdateClothAndHairFrame();
        }
        private void UpdateTalking()
        {
            if (FindChatIndex(2))
            {
                Chatting1(currentChatRoom ?? Projectile.CreateChatRoomDirect());
            }
        }
        private void Chatting1(PetChatRoom chatRoom)
        {
            int type = ProjectileType<Iku>();
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
            Projectile tenshin = chatRoom.initiator;
            Projectile iku = chatRoom.member[0];
            int turn = chatRoom.chatTurn;
            if (turn == -1)
            {
                //天子：今天也要大干一场！
                iku.CloseCurrentDialog();

                if (tenshin.CurrentDialogFinished())
                    chatRoom.chatTurn++;
            }
            else if (turn == 0)
            {
                //衣玖：天女大人您还是安分点吧...
                iku.SetChat(ChatSettingConfig, 16, 20);

                if (iku.CurrentDialogFinished())
                    chatRoom.chatTurn++;
            }
            else
            {
                chatRoom.CloseChatRoom();
            }
        }
        public override void AI()
        {
            Projectile.SetPetActive(Owner, BuffType<TenshiBuff>());

            UpdateTalking();

            ControlMovement();

            switch (CurrentState)
            {
                case States.Blink:
                    Blink();
                    break;

                case States.Eating:
                    shouldNotTalking = true;
                   Eating();
                    break;

                case States.EatingToBreak:
                    shouldNotTalking = true;
                    EatingToBreak();
                    break;

                case States.Break:
                    shouldNotTalking = true;
                    Break();
                    break;

                case States.AfterEating:
                    shouldNotTalking = true;
                    AfterEating();
                    break;

                default:
                    Idle();
                    break;
            }

            if (IsIdleState && ActionCD > 0)
            {
                ActionCD--;
            }
        }
        private void ControlMovement()
        {
            Projectile.tileCollide = false;
            Projectile.rotation = Projectile.velocity.X * 0.005f;

            ChangeDir();

            Vector2 point = new Vector2(-50 * Owner.direction, -30 + Owner.gfxOffY);
            if (FindPet(ProjectileType<Iku>(), false))
            {
                point = new Vector2(50 * Owner.direction, -30 + Owner.gfxOffY);
            }
            MoveToPoint(point, 15f);
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
                if (mainTimer > 0 && mainTimer % 860 == 0
                    && currentChatRoom == null && ActionCD <= 0 && Owner.velocity.Length() < 4f)
                {
                    if (Main.rand.NextBool(4))
                    {
                        RandomCount = Main.rand.Next(4, 6);
                        CurrentState = States.Eating;
                    }
                }
            }
        }
        private void Blink()
        {
            Projectile.frame = 0;
            if (blinkFrame < 11)
            {
                blinkFrame = 11;
            }
            if (++blinkFrameCounter > 3)
            {
                blinkFrameCounter = 0;
                blinkFrame++;
            }
            if (blinkFrame > 13)
            {
                blinkFrame = 11;
                CurrentState = States.Idle;
            }
        }
        private void Eating()
        {
            if (OwnerIsMyPlayer && Timer == 0)
            {
                randomCount2 = Main.rand.Next(3, 6);
                randomCount3 = Main.rand.Next(180, 360);
            }
            if (++Projectile.frameCounter > 7)
            {
                Projectile.frameCounter = 0;
                Projectile.frame++;
            }
            if (Projectile.frame > 8)
            {
                Projectile.frame = 7;
                Timer++;
            }
            if (OwnerIsMyPlayer && Timer > randomCount2)
            {
                Timer = 0;
                CurrentState = States.EatingToBreak;
            }
        }
        private void EatingToBreak()
        {
            if (++Projectile.frameCounter > 7)
            {
                Projectile.frameCounter = 0;
                Projectile.frame++;
            }
            if (Projectile.frame > 9)
            {
                Projectile.frame = 5;
                if (OwnerIsMyPlayer)
                {
                    CurrentState = States.Break;
                }
            }
        }
        private void Break()
        {
            Projectile.frame = 5;
            if (OwnerIsMyPlayer)
            {
                Timer++;
                if (Timer > randomCount3)
                {
                    Timer = 0;
                    EatingCount++;
                    if (EatingCount > RandomCount)
                    {
                        EatingCount = 0;
                        CurrentState = States.AfterEating;
                    }
                    else
                    {
                        CurrentState = States.Eating;
                    }
                }
            }
        }
        private void AfterEating()
        {
            if (++Projectile.frameCounter > 7)
            {
                Projectile.frameCounter = 0;
                Projectile.frame++;
            }
            if (Projectile.frame == 9)
            {
                Projectile.frame = 10;
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
        private void UpdateStoneFrame()
        {
            if (stoneFrame < 8)
            {
                stoneFrame = 8;
            }
            int count = 6;
            if (++stoneFrameCounter > count)
            {
                stoneFrameCounter = 0;
                stoneFrame++;
            }
            if (stoneFrame > 10)
            {
                stoneFrame = 8;
            }
        }
        private void UpdateClothAndHairFrame()
        {
            int count = 4;
            if (++clothFrameCounter > count)
            {
                clothFrameCounter = 0;
                clothFrame++;
            }
            if (clothFrame > 3)
            {
                clothFrame = 0;
            }
        }
    }
}


