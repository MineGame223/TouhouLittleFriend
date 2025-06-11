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
            ShowingOff,
            AfterShowing,
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

        private int stoneFrame, stoneFrameCounter;
        private int blinkFrame, blinkFrameCounter;
        private int clothFrame, clothFrameCounter;

        private DrawPetConfig drawConfig = new(2);
        private readonly Texture2D clothTex = AltVanillaFunction.GetExtraTexture("Tenshi_Cloth");
        private readonly Texture2D glowTex = AltVanillaFunction.GetGlowTexture("Tenshi_Glow");
        public override void SetStaticDefaults()
        {
            Main.projFrames[Type] = 22;
            Main.projPet[Type] = true;
        }
        public override TouhouPetID UniqueID => TouhouPetID.Tenshin;
        public override bool DrawPetSelf(ref Color lightColor)
        {
            DrawPetConfig config = drawConfig with
            {
                ShouldUseEntitySpriteDraw = true,
                AltTexture = clothTex,
            };

            Projectile.DrawPet(stoneFrame, lightColor, drawConfig, 1);

            Projectile.DrawPet(clothFrame + 4, lightColor, drawConfig, 1);
            Projectile.DrawPet(clothFrame + 4, lightColor, config, 1);
            Projectile.ResetDrawStateForPet();

            Projectile.DrawPet(Projectile.frame, lightColor, drawConfig);

            if (CurrentState == States.Blink)
                Projectile.DrawPet(blinkFrame, lightColor, drawConfig, 1);

            Projectile.DrawPet(Projectile.frame, lightColor, config);
            Projectile.DrawPet(clothFrame, lightColor,
                drawConfig with
                {
                    ShouldUseEntitySpriteDraw = true,
                }, 1);
            Projectile.ResetDrawStateForPet();

            Projectile.DrawPet(Projectile.frame, Color.White * 0.75f,
                drawConfig with
                {
                    AltTexture = glowTex,
                });
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
        public override WeightedRandom<string> RegularDialogText()
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
            if (ModUtils.HasModAndFindNPC("Gensokyo", boss, "TenshiHinanawi"))
            {
                Projectile.SetChat(ChatSettingConfig, 7);
            }
            else
            {
                Projectile.SetChat(ChatSettingConfig, 8);
            }
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

                case States.ShowingOff:
                    shouldNotTalking = true;
                    ShowingOff();
                    break;

                case States.AfterShowing:
                    shouldNotTalking = true;
                    AfterShowing();
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

            Vector2 point = new(-50 * Owner.direction, -30 + Owner.gfxOffY);
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
                    if (Main.rand.NextBool(5))
                    {
                        RandomCount = Main.rand.Next(45, 75);
                        CurrentState = States.ShowingOff;
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
        private void ShowingOff()
        {
            var count = Projectile.frame switch
            {
                4 or 5 or 6 or 7 or 8 => 8,
                9 or 10 or 11 or 12 => 5,
                13 => 60,
                _ => 7,
            };
            ;
            if (++Projectile.frameCounter > count)
            {
                Projectile.frameCounter = 0;
                Projectile.frame++;
            }
            if (Projectile.frame >= 17)
            {
                Projectile.frame = 17;
                Timer++;
            }
            if (OwnerIsMyPlayer && Timer > RandomCount)
            {
                Timer = 0;
                CurrentState = States.AfterShowing;
            }

            int strength = Projectile.frame;
            if (Projectile.frame >= 9)
            {
                strength = (int)MathHelper.Clamp(32 - Projectile.frame, 14, 32);
            }
            float lightFactor = 0.05f * strength;
            Lighting.AddLight(Projectile.Center, new Vector3(0.89f, 0.55f, 0.32f) * lightFactor);

            if (!ShouldExtraVFXActive)
                return;

            if (Projectile.frame == 9 && Projectile.frameCounter == 0)
            {
                for (int i = 0; i < 10; i++)
                {
                    Dust d = Dust.NewDustDirect(Projectile.Center, 2, 2, MyDustId.Fire, 0, 0, 100, default, 1f);
                    d.scale = Main.rand.NextFloat(1f, 2f);
                    d.velocity = new Vector2(Main.rand.Next(2, 5), 0).RotatedByRandom(MathHelper.TwoPi);
                    d.noGravity = true;
                }
            }
        }
        private void AfterShowing()
        {
            if (++Projectile.frameCounter > 5)
            {
                Projectile.frameCounter = 0;
                Projectile.frame++;
            }
            if (Projectile.frame > 21)
            {
                Projectile.frame = 0;
                if (OwnerIsMyPlayer)
                {
                    ActionCD = 3600;
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
            int count = 5;
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


