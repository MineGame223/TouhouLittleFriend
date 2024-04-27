using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ID;
using Terraria.Utilities;
using TouhouPets.Content.Buffs.PetBuffs;

namespace TouhouPets.Content.Projectiles.Pets
{
    public class Nitori : BasicTouhouPet
    {
        private enum States
        {
            Idle,
            Blink,
            BeforeBreakdown,
            Alert,
            Breakdown,
            CleanAsh,
            AfterBreakdown,
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
        private bool IsIdleState => CurrentState <= States.Blink;

        private int blinkFrame, blinkFrameCounter;
        private int backFrame, backFrameCounter;

        private DrawPetConfig drawConfig = new(2);
        private readonly Texture2D clothTex = AltVanillaFunction.GetExtraTexture("Nitori_Cloth");
        private readonly Texture2D glowTex = AltVanillaFunction.GetGlowTexture("Nitori_Glow");
        public override void SetStaticDefaults()
        {
            Main.projFrames[Type] = 16;
            Main.projPet[Type] = true;
            ProjectileID.Sets.LightPet[Type] = true;
        }
        public override bool PreDraw(ref Color lightColor)
        {
            DrawPetConfig config = drawConfig with
            {
                ShouldUseEntitySpriteDraw = true,
                AltTexture = clothTex,
            };

            Projectile.DrawPet(backFrame, lightColor, drawConfig, 1);
            Projectile.DrawPet(backFrame, Color.White * 0.7f,
                drawConfig with
                {
                    AltTexture = glowTex,
                }, 1);

            Projectile.DrawPet(backFrame, lightColor, config, 1);
            Projectile.DrawStateNormalizeForPet();

            Projectile.DrawPet(Projectile.frame, lightColor, drawConfig);

            if (CurrentState == States.Blink)
                Projectile.DrawPet(blinkFrame, lightColor, drawConfig);

            Projectile.DrawPet(Projectile.frame, lightColor, config);
            return false;
        }
        public override Color ChatTextColor => new Color(74, 165, 255);
        public override void RegisterChat(ref string name, ref Vector2 indexRange)
        {
            name = "Nitori";
            indexRange = new Vector2(1, 8);
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
                chat.Add(ChatDictionary[3]);
            }
            return chat;
        }
        private void UpdateTalking()
        {
        }
        public override void VisualEffectForPreview()
        {
            if (IsIdleState)
            {
                IdleAnimation();
                UpdateBackFrame();
            }
        }
        public override void SetPetLight(ref Vector2 position, ref Vector3 rgb, ref bool inactive)
        {
            position += new Vector2(0, -20);
            rgb = new Vector3(1.95f, 1.64f, 0.67f);
            inactive = backFrame == 8;
        }
        public override void AI()
        {
            Projectile.SetPetActive(Owner, BuffType<NitoriBuff>());

            UpdateTalking();

            ControlMovement();

            switch (CurrentState)
            {
                case States.Blink:
                    Blink();
                    break;

                case States.BeforeBreakdown:
                    shouldNotTalking = true;
                    BeforeBreakdown();
                    break;

                case States.Alert:
                    shouldNotTalking = true;
                    Alert();
                    break;

                case States.Breakdown:
                    shouldNotTalking = true;
                    Breakdown();
                    break;

                case States.CleanAsh:
                    shouldNotTalking = true;
                    CleanAsh();
                    break;

                case States.AfterBreakdown:
                    shouldNotTalking = true;
                    AfterBreakdown();
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
            Projectile.rotation = Projectile.velocity.X * 0.012f;
            if (CurrentState == States.Breakdown)
            {
                Projectile.rotation = 0;
            }

            ChangeDir();

            Vector2 point = new Vector2(60 * Owner.direction, -40 + Owner.gfxOffY);
            MoveToPoint(point, 12.5f);
        }
        private void Idle()
        {
            if (OwnerIsMyPlayer)
            {
                if (mainTimer % 270 == 0)
                {
                    CurrentState = States.Blink;
                }
                if (mainTimer > 0 && mainTimer % 900 == 0 && currentChatRoom == null && ActionCD <= 0)
                {
                    if (Main.rand.NextBool(4))
                    {
                        CurrentState = States.BeforeBreakdown;
                    }
                }
            }
        }
        private void Blink()
        {
            if (blinkFrame < 13)
            {
                blinkFrame = 13;
            }
            if (++blinkFrameCounter > 3)
            {
                blinkFrameCounter = 0;
                blinkFrame++;
            }
            if (blinkFrame > 15)
            {
                blinkFrame = 13;
                CurrentState = States.Idle;
            }
        }
        private void BeforeBreakdown()
        {
            IdleAnimation();
            if (++backFrameCounter > 4)
            {
                backFrameCounter = 0;
                backFrame++;
            }
            if (backFrame > 7)
            {
                backFrame = 4;
                Timer++;
            }
            if (Main.rand.NextBool(8 - Timer))
            {
                Dust.NewDustPerfect(Projectile.Center + new Vector2(-6 * Projectile.spriteDirection, -8)
                    , MyDustId.Smoke
                    , new Vector2(Main.rand.Next(-3, 3) * 0.75f, Main.rand.Next(-6, -3) * 0.75f)
                    , 100, default, Main.rand.NextFloat(1.5f, 2.25f)).noGravity = true;
            }
            if (OwnerIsMyPlayer && Timer > 6)
            {
                Timer = 0;
                CurrentState = States.Alert;
            }
        }
        private void Alert()
        {
            if (++backFrameCounter > 4)
            {
                backFrameCounter = 0;
                backFrame++;
            }
            if (backFrame > 7)
            {
                backFrame = 4;
            }
            if (++Projectile.frameCounter > 6)
            {
                Projectile.frameCounter = 0;
                Projectile.frame++;
            }
            if (Projectile.frame > 7)
            {
                Projectile.frame = 4;
                Timer++;
            }
            if (Main.rand.NextBool(4 - Timer))
            {
                Dust.NewDustPerfect(Projectile.Center + new Vector2(-6 * Projectile.spriteDirection, -8)
                    , MyDustId.Smoke
                    , new Vector2(Main.rand.Next(-3, 3) * 0.75f, Main.rand.Next(-6, -3) * 0.75f)
                    , 100, Color.Black, Main.rand.NextFloat(1.5f, 2.25f)).noGravity = true;
                if (Main.rand.NextBool(2))
                {
                    Gore.NewGoreDirect(Projectile.GetSource_FromAI()
                        , Projectile.Center + new Vector2(0, -24)
                        , new Vector2(Main.rand.Next(-3, 3) * 0.15f, Main.rand.Next(-3, -1) * 0.15f)
                        , Main.rand.Next(GoreID.Smoke1, GoreID.Smoke3 + 1), Main.rand.NextFloat(0.25f, 0.5f));
                }
            }
            if (OwnerIsMyPlayer && Timer > 2)
            {
                Timer = 0;
                CurrentState = States.Breakdown;
            }
        }
        private void Breakdown()
        {
            backFrame = 8;
            if (Timer == 0)
            {
                for (int i = 0; i < 25; i++)
                    Dust.NewDustPerfect(Projectile.Center
                        , MyDustId.Smoke
                        , new Vector2(Main.rand.Next(-6, 6) * 0.75f, Main.rand.Next(-6, 6) * 0.75f)
                        , 20, Color.Black, Main.rand.NextFloat(2.5f, 4.25f)).noGravity = true;
                for (int i = 0; i < 10; i++)
                    Gore.NewGoreDirect(Projectile.GetSource_FromAI()
                        , Projectile.Center + new Vector2(Main.rand.Next(-8, 8), Main.rand.Next(-8, 8))
                        , new Vector2(Main.rand.Next(-3, 3) * 0.15f, Main.rand.Next(-3, 3) * 0.15f)
                        , Main.rand.Next(GoreID.Smoke1, GoreID.Smoke3 + 1), Main.rand.NextFloat(0.75f, 1.25f));
                AltVanillaFunction.PlaySound(SoundID.Item14, Projectile.position);
            }
            else if (Main.rand.NextBool(2))
            {
                Dust.NewDustPerfect(Projectile.Center + new Vector2(Main.rand.Next(-14, 14), Main.rand.Next(-8, 8))
                        , MyDustId.Smoke
                        , new Vector2(Main.rand.Next(-1, 1) * 0.75f, Main.rand.Next(-4, -2) * 0.75f)
                        , 90, Color.Black, Main.rand.NextFloat(1f, 2f)).noGravity = true;
            }
            Timer++;

            if (Timer > 120 && Timer <= 144)
                Projectile.frame = Timer % 6 == 0 ? 8 : 9;
            else
                Projectile.frame = 8;

            if (OwnerIsMyPlayer && Timer > 240)
            {
                Timer = 0;
                CurrentState = States.CleanAsh;
            }
        }
        private void CleanAsh()
        {
            if (Main.rand.NextBool(3))
            {
                for (int i = 0; i < 4; i++)
                    Dust.NewDustPerfect(Projectile.Center + new Vector2(Main.rand.Next(-14, 14), Main.rand.Next(-8, 8))
                        , MyDustId.Smoke
                        , new Vector2(Main.rand.Next(-6, 6) * 0.85f, Main.rand.Next(-6, 6) * 0.85f)
                        , 90, Color.Black, Main.rand.NextFloat(1f, 2f)).noGravity = true;
            }

            backFrame = 9;

            if (++Projectile.frameCounter > 6)
            {
                Projectile.frameCounter = 0;
                Projectile.frame++;
            }
            if (Projectile.frame > 11)
            {
                Projectile.frame = 10;
                Timer++;
            }
            if (Timer > 6 && OwnerIsMyPlayer)
            {
                Timer = 0;
                CurrentState = States.AfterBreakdown;
            }
        }
        private void AfterBreakdown()
        {
            if (++Projectile.frameCounter > 6)
            {
                Projectile.frameCounter = 0;
                Projectile.frame++;
            }
            if (Projectile.frame > 12)
            {
                Projectile.frame = 0;
                if (OwnerIsMyPlayer)
                {
                    Timer = 0;
                    ActionCD = 2400;
                    CurrentState = States.Idle;
                }
            }
        }
        private void IdleAnimation()
        {
            if (++Projectile.frameCounter > 6)
            {
                Projectile.frameCounter = 0;
                Projectile.frame++;
            }
            if (Projectile.frame > 3)
            {
                Projectile.frame = 0;
            }
        }
        private void UpdateBackFrame()
        {
            if (++backFrameCounter > 4)
            {
                backFrameCounter = 0;
                backFrame++;
            }
            if (backFrame > 3)
            {
                backFrame = 0;
            }
        }
    }
}


