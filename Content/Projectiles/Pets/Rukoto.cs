using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ID;
using Terraria.Utilities;
using TouhouPets.Content.Buffs.PetBuffs;

namespace TouhouPets.Content.Projectiles.Pets
{
    public class Rukoto : BasicTouhouPet
    {
        private enum States
        {
            Idle,
            Blink,
            Sweeping,
            AfterSweeping,
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

        private int blinkFrame, blinkFrameCounter;
        private int backFrame, backFrameCounter;
        private int bodyFrame, bodyFrameCounter;

        private DrawPetConfig drawConfig = new(2);
        private readonly Texture2D clothTex = AltVanillaFunction.GetExtraTexture("Rukoto_Cloth");
        private readonly Texture2D glowTex = AltVanillaFunction.GetGlowTexture("Rukoto_Glow");
        public override void SetStaticDefaults()
        {
            Main.projFrames[Type] = 8;
            Main.projPet[Type] = true;

            ProjectileID.Sets.CharacterPreviewAnimations[Type] =
                ProjectileID.Sets.SimpleLoop(0, 1)
                .WhenSelected(1, 4, 10);
        }
        public override bool DrawPetSelf(ref Color lightColor)
        {
            DrawPetConfig config = drawConfig with
            {
                ShouldUseEntitySpriteDraw = true,
            };

            DrawPetConfig config2 = config with
            {
                AltTexture = clothTex,
            };

            Projectile.DrawPet(backFrame, lightColor, config, 1);
            Projectile.ResetDrawStateForPet();
            Projectile.DrawPet(backFrame, Color.White * 0.7f,
                drawConfig with
                {
                    AltTexture = glowTex,
                }, 1);

            Projectile.DrawPet(bodyFrame, lightColor, drawConfig, 1);
            Projectile.DrawPet(bodyFrame, lightColor, config2, 1);
            Projectile.ResetDrawStateForPet();

            if (CurrentState == States.Blink || Projectile.frame > 0)
                Projectile.DrawPet(blinkFrame, lightColor, drawConfig);

            Projectile.DrawPet(Projectile.frame, lightColor, drawConfig);
            Projectile.DrawPet(Projectile.frame, lightColor, config2);
            return false;
        }
        public override Color ChatTextColor => new Color(88, 209, 115);
        public override void RegisterChat(ref string name, ref Vector2 indexRange)
        {
            name = "Rukoto";
            indexRange = new Vector2(1, 5);
        }
        public override void SetRegularDialog(ref int timePerDialog, ref int chance, ref bool whenShouldStop)
        {
            timePerDialog = 660;
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
            }
            return chat;
        }
        public override void VisualEffectForPreview()
        {
            UpdateMiscFrames();
            if (Projectile.frame > 0)
            {
                blinkFrame = 7;
            }
        }
        private void UpdateTalking()
        {
        }
        public override void AI()
        {
            Projectile.SetPetActive(Owner, BuffType<RukotoBuff>());

            UpdateTalking();

            if (ShouldExtraVFXActive)
                GenDust();

            ControlMovement();

            switch (CurrentState)
            {
                case States.Blink:
                    Blink();
                    break;

                case States.Sweeping:
                    shouldNotTalking = true;
                    Sweeping();
                    break;

                case States.AfterSweeping:
                    shouldNotTalking = true;
                    AfterSweeping();
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
        private void GenDust()
        {
            Dust d = Dust.NewDustPerfect(Projectile.Center + new Vector2(14 * Projectile.spriteDirection, 14)
                , MyDustId.Fire, new Vector2(0, 3), 100, default, Main.rand.NextFloat(1, 2));
            d.noGravity = true;
            if (Main.rand.NextBool(3))
            {
                d.type = MyDustId.Smoke;
            }
            d = Dust.NewDustPerfect(Projectile.Center + new Vector2(-14 * Projectile.spriteDirection, 14)
                , MyDustId.Fire, new Vector2(0, 3), 100, default, Main.rand.NextFloat(1, 2));
            d.noGravity = true;
            if (Main.rand.NextBool(3))
            {
                d.type = MyDustId.Smoke;
            }

            if (CurrentState == States.Sweeping && Main.rand.NextBool(3))
            {
                d = Dust.NewDustPerfect(Projectile.Center + new Vector2(Main.rand.Next(-20, 20), 30)
                    , MyDustId.Smoke, new Vector2(0, -1), Main.rand.Next(0, 100), default, Main.rand.NextFloat(1, 2));
                d.noGravity = true;
            }
        }
        private void ControlMovement()
        {
            Projectile.tileCollide = false;
            if (IsIdleState)
            {
                Projectile.rotation = Projectile.velocity.X * 0.032f;
            }
            else
            {
                Projectile.rotation = Projectile.velocity.X * 0.002f;
            }

            ChangeDir();

            Vector2 point = new Vector2(-50 * Owner.direction, -30 + Owner.gfxOffY);
            MoveToPoint(point, 16f);
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
                if (mainTimer > 0 && mainTimer % 720 == 0 && currentChatRoom == null && ActionCD <= 0)
                {
                    if (Main.rand.NextBool(8))
                    {
                        RandomCount = Main.rand.Next(30, 60);
                        CurrentState = States.Sweeping;
                    }
                }
            }
        }
        private void Blink()
        {
            if (blinkFrame < 5)
            {
                blinkFrame = 5;
            }
            if (++blinkFrameCounter > 3)
            {
                blinkFrameCounter = 0;
                blinkFrame++;
            }
            if (blinkFrame > 7)
            {
                blinkFrame = 5;
                CurrentState = States.Idle;
            }
        }
        private void Sweeping()
        {
            if (++Projectile.frameCounter > 10)
            {
                Projectile.frameCounter = 0;
                Projectile.frame++;
            }
            if (Projectile.frame > 4)
            {
                Projectile.frame = 1;
                Timer++;
            }
            if (OwnerIsMyPlayer && Timer > RandomCount)
            {
                Timer = 0;
                CurrentState = States.AfterSweeping;
            }
        }
        private void AfterSweeping()
        {
            if (++Projectile.frameCounter > 7)
            {
                Projectile.frameCounter = 0;
                Projectile.frame++;
            }
            if (Projectile.frame > 4)
            {
                Projectile.frame = 0;
                if (OwnerIsMyPlayer)
                {
                    ActionCD = 800;
                    CurrentState = States.Idle;
                }
            }
        }
        private void UpdateMiscFrames()
        {
            if (++bodyFrameCounter > 6)
            {
                bodyFrameCounter = 0;
                bodyFrame++;
            }
            if (bodyFrame > 3)
            {
                bodyFrame = 0;
            }

            if (backFrame < 4)
            {
                backFrame = 4;
            }
            if (++backFrameCounter > 4)
            {
                backFrameCounter = 0;
                backFrame++;
            }
            if (backFrame > 7)
            {
                backFrame = 4;
            }
        }
    }
}


