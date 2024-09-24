using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ID;
using Terraria.Utilities;
using TouhouPets.Content.Buffs.PetBuffs;

namespace TouhouPets.Content.Projectiles.Pets
{
    public class Utsuho : BasicTouhouPet
    {
        private enum States
        {
            Idle,
            Blink,
            BeforeFire,
            Firing,
            AfterFire,
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

        private int wingFrame, wingFrameCounter;
        private Vector2 sunPos;

        private DrawPetConfig drawConfig = new(1);
        private readonly Texture2D clothTex = AltVanillaFunction.GetExtraTexture("Utsuho_Cloth");
        private readonly Texture2D glowTex = AltVanillaFunction.GetGlowTexture("Utsuho_Glow");
        private readonly Texture2D eyeTex = AltVanillaFunction.GetGlowTexture("Utsuho_Glow_Eye");
        private readonly Texture2D sunTex = AltVanillaFunction.GetExtraTexture("UtsuhoSun");
        public override void SetStaticDefaults()
        {
            Main.projFrames[Type] = 11;
            Main.projPet[Type] = true;
            ProjectileID.Sets.LightPet[Type] = true;
        }
        public override bool DrawPetSelf(ref Color lightColor)
        {
            DrawPetConfig config = drawConfig with
            {
                ShouldUseEntitySpriteDraw = true,
            };

            DrawSun();

            Projectile.DrawPet(wingFrame, lightColor, config);
            Projectile.DrawPet(wingFrame, Color.White * 0.7f,
                config with
                {
                    AltTexture = glowTex,
                });
            Projectile.ResetDrawStateForPet();

            Projectile.DrawPet(Projectile.frame, lightColor, drawConfig);
            Projectile.DrawPet(Projectile.frame, lightColor,
                config with
                {
                    AltTexture = clothTex,
                });
            Projectile.ResetDrawStateForPet();

            DrawEye();
            return false;
        }
        private void DrawEye()
        {
            DrawPetConfig config = drawConfig with
            {
                AltTexture = eyeTex,
            };
            for (int i = -1; i <= 1; i++)
            {
                Vector2 eyePos = new Vector2(0f, 2 * i).RotatedBy(MathHelper.PiOver2 * i);
                Projectile.DrawPet(Projectile.frame, Color.White * 0.3f,
                    config with
                    {
                        PositionOffset = eyePos.RotatedBy(MathHelper.TwoPi * Main.GlobalTimeWrappedHourly),
                    });
            }
            Projectile.DrawPet(Projectile.frame, Color.White, config);
        }
        private void DrawSun()
        {
            Vector2 pos = Projectile.DefaultDrawPetPosition();
            for (int i = 0; i < 5; i++)
            {
                Main.EntitySpriteDraw(sunTex, pos + sunPos + new Vector2(Main.rand.Next(-10, 11) * 0.2f, Main.rand.Next(-10, 11) * 0.2f), null, Projectile.GetAlpha(Color.White) * 0.5f, -mainTimer * 0.09f, sunTex.Size() / 2, Projectile.scale * 1.02f, SpriteEffects.None, 0f);
            }
            Main.EntitySpriteDraw(sunTex, pos + sunPos, null, Projectile.GetAlpha(Color.White), mainTimer * 0.05f, sunTex.Size() / 2, Projectile.scale, SpriteEffects.None, 0f);
        }
        public override Color ChatTextColor => new Color(228, 184, 75);
        public override void RegisterChat(ref string name, ref Vector2 indexRange)
        {
            name = "Utsuho";
            indexRange = new Vector2(1, 6);
        }
        public override void SetRegularDialog(ref int timePerDialog, ref int chance, ref bool whenShouldStop)
        {
            timePerDialog = 721;
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
                if (Owner.ZoneUnderworldHeight)
                {
                    chat.Add(ChatDictionary[4]);
                }
                if (FindPet(ProjectileType<Satori>()))
                {
                    chat.Add(ChatDictionary[5], 3);
                }
            }
            return chat;
        }
        private void UpdateTalking()
        {
            if (FindChatIndex(5))
            {
                Chatting1(currentChatRoom ?? Projectile.CreateChatRoomDirect());
            }
        }
        private void Chatting1(PetChatRoom chatRoom)
        {
            int type = ProjectileType<Satori>();
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
            Projectile utsuho = chatRoom.initiator;
            Projectile satori = chatRoom.member[0];
            int turn = chatRoom.chatTurn;
            if (turn == -1)
            {
                //阿空：觉大人最好了！
                satori.CloseCurrentDialog();

                if (utsuho.CurrentDialogFinished())
                    chatRoom.chatTurn++;
            }
            else if (turn == 0)
            {
                //觉：阿空也是我最喜欢的乌鸦哦。
                satori.SetChat(ChatSettingConfig, 6, 20);

                if (satori.CurrentDialogFinished())
                    chatRoom.chatTurn++;
            }
            else
            {
                chatRoom.CloseChatRoom();
            }
        }
        public override void VisualEffectForPreview()
        {
            UpdateWingFrame();
        }
        public override void SetPetLight(ref Vector2 position, ref Vector3 rgb, ref bool inactive)
        {
            sunPos = new Vector2(0, -40);
            position += sunPos;
            rgb = new Vector3(1.95f, 1.64f, 0.67f);
        }
        public override void AI()
        {
            Projectile.SetPetActive(Owner, BuffType<UtsuhoBuff>());
            Projectile.SetPetActive(Owner, BuffType<KomeijiBuff>());

            UpdateTalking();

            ControlMovement();

            SmokeDust();

            switch (CurrentState)
            {
                case States.Blink:
                    Blink();
                    break;

                case States.BeforeFire:
                    shouldNotTalking = true;
                    BeforeFire();
                    break;

                case States.Firing:
                    shouldNotTalking = true;
                    Firing();
                    break;

                case States.AfterFire:
                    shouldNotTalking = true;
                    AfterFire();
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
        private void SmokeDust()
        {
            if (ActionCD >= 480)
            {
                if (Main.rand.NextBool(4))
                    Dust.NewDustPerfect(Projectile.Center + new Vector2(24 * Projectile.spriteDirection, 20), MyDustId.Smoke, new Vector2(0, Main.rand.Next(-4, -2) * 0.5f), 100, default, Main.rand.NextFloat(0.5f, 1.25f)).noGravity = true;
            }
        }
        private void ControlMovement()
        {
            Projectile.tileCollide = false;
            Projectile.rotation = Projectile.velocity.X * 0.012f;

            if (CurrentState == States.Firing)
                return;

            ChangeDir();

            Vector2 point = new Vector2(0, -60 + Owner.gfxOffY);
            if (FindPet(ProjectileType<Rin>(), false))
            {
                point = new Vector2(50 * Owner.direction, -30 + Owner.gfxOffY);
                if (Owner.HasBuff<KomeijiBuff>())
                    point = new Vector2(70 * Owner.direction, 0 + Owner.gfxOffY);
            }
            MoveToPoint(point, 13.5f);
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
                if (mainTimer > 0 && mainTimer % 300 == 0 && currentChatRoom == null && ActionCD <= 0)
                {
                    if (Main.rand.NextBool(7))
                    {
                        RandomCount = Main.rand.Next(50, 80);
                        CurrentState = States.BeforeFire;
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
        private void BeforeFire()
        {
            if (Projectile.frame < 3)
            {
                Projectile.frame = 3;
            }

            if (++Projectile.frameCounter > 12)
            {
                Projectile.frameCounter = 0;
                Projectile.frame++;
            }

            if (Projectile.frame >= 4)
            {
                Projectile.frame = 4;
                if (Main.rand.NextBool(2))
                    Dust.NewDustPerfect(Projectile.Center + new Vector2(26 * Projectile.spriteDirection, 6)
                        , MyDustId.OrangeFire2, new Vector2(Main.rand.Next(-3, 3) * 0.75f, Main.rand.Next(-3, 3) * 0.75f)
                        , 100, default, Main.rand.NextFloat(0.5f, 1.25f)).noGravity = true;

                Timer++;
            }
            if (OwnerIsMyPlayer && Timer > RandomCount)
            {
                Timer = 0;
                CurrentState = States.Firing;
            }
        }
        private void Firing()
        {
            Projectile.frame = 5;
            if (Timer == 0)
            {
                for (int i = 0; i < 7; i++)
                {
                    Vector2 center = Projectile.Center;
                    Vector2 vel = (new Vector2(6 * Projectile.spriteDirection, -4) * Main.rand.NextFloat(0.5f, 2f)).RotatedBy(MathHelper.ToRadians(Main.rand.Next(-30, 30)));
                    Dust.NewDustDirect(center, 4, 4, MyDustId.Smoke, vel.X, vel.Y, 100, default, Main.rand.NextFloat(0.5f, 1.5f)).noGravity = true;
                    vel = (new Vector2(6 * Projectile.spriteDirection, -4) * 2f * Main.rand.NextFloat(0.5f, 2f)).RotatedBy(MathHelper.ToRadians(Main.rand.Next(-10, 10)));
                    Dust.NewDustDirect(center, 4, 4, MyDustId.OrangeFire2, vel.X, vel.Y, 100, default, Main.rand.NextFloat(1.4f, 1.9f)).noGravity = true;
                }
                if (OwnerIsMyPlayer)
                {
                    Projectile.velocity.X = 9 * -Projectile.spriteDirection;
                    Projectile.velocity.Y = 4;
                    Projectile.netUpdate = true;
                }
            }
            Timer++;
            if (Timer > 30 && OwnerIsMyPlayer)
            {
                Timer = 0;
                CurrentState = States.AfterFire;
            }

            Projectile.velocity *= 0.85f;
        }
        private void AfterFire()
        {
            if (++Projectile.frameCounter > 12)
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
        private void UpdateWingFrame()
        {
            if (wingFrame < 7)
            {
                wingFrame = 7;
            }
            if (++wingFrameCounter > 6)
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


