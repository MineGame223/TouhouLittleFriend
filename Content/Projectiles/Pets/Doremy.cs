using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.Graphics.Shaders;
using Terraria.ID;
using Terraria.Utilities;
using TouhouPets.Content.Buffs.PetBuffs;

namespace TouhouPets.Content.Projectiles.Pets
{
    public class Doremy : BasicTouhouPet
    {
        private enum States
        {
            Idle,
            Blink,
            Dreaming,
            AfterDreaming,
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
        private bool IsIdleState => CurrentState <= States.Blink;
        private bool CanDreaming => (!Main.dayTime || Owner.sleeping.isSleeping) && !Insomnia;
        private static bool Insomnia => Main.bloodMoon || Main.pumpkinMoon || Main.snowMoon;

        private int blinkFrame, blinkFrameCounter;
        private int hatFrame, hatFrameCounter;
        private int tailFrame, tailFrameCounter;
        private float extraX, extraY;
        private static readonly Vector2 totalOffset = new Vector2(0, -10);

        private DrawPetConfig drawConfig = new(2)
        {
            PositionOffset = totalOffset,
        };
        private readonly Texture2D clothTex = AltVanillaFunction.GetExtraTexture("Doremy_Cloth");
        private readonly Texture2D glowTex = AltVanillaFunction.GetGlowTexture("Doremy_Glow");
        public override void SetStaticDefaults()
        {
            Main.projFrames[Type] = 13;
            Main.projPet[Type] = true;
            ProjectileID.Sets.LightPet[Type] = true;
        }
        public override bool DrawPetSelf(ref Color lightColor)
        {
            Vector2 extraPos = new Vector2(extraX, extraY);
            DrawPetConfig config = drawConfig with
            {
                PositionOffset = totalOffset + extraPos,
            };

            Projectile.DrawPet(tailFrame, lightColor, config, 1);

            Projectile.DrawPet(hatFrame, lightColor,
                config with
                {
                    ShouldUseEntitySpriteDraw = true,
                }, 1);
            Projectile.ResetDrawStateForPet();

            Projectile.DrawPet(Projectile.frame, lightColor, drawConfig);

            if (CurrentState == States.Blink || (Main.bloodMoon && CurrentState == States.Idle))
                Projectile.DrawPet(blinkFrame, lightColor, drawConfig, 1);

            Projectile.DrawPet(Projectile.frame, lightColor,
                drawConfig with
                {
                    AltTexture = clothTex,
                    ShouldUseEntitySpriteDraw = true,
                });
            Projectile.ResetDrawStateForPet();

            Projectile.DrawPet(Projectile.frame, Color.White * 0.75f,
                drawConfig with
                {
                    AltTexture = glowTex,
                });
            return false;
        }
        public override void SetDefaults()
        {
            base.SetDefaults();
            Projectile.width = Projectile.height = 30;
            Projectile.tileCollide = true;
        }
        public override bool OnTileCollide(Vector2 oldVelocity)
        {
            return false;
        }
        public override Color ChatTextColor => new Color(255, 127, 222);
        public override void RegisterChat(ref string name, ref Vector2 indexRange)
        {
            name = "Doremy";
            indexRange = new Vector2(1, 13);
        }
        public override void SetRegularDialog(ref int timePerDialog, ref int chance, ref bool whenShouldStop)
        {
            timePerDialog = 540;
            chance = 12;
            if (CurrentState == States.Dreaming)
            {
                timePerDialog = 1020;
                chance = 8;
            }
            whenShouldStop = false;
        }
        public override string GetRegularDialogText()
        {
            WeightedRandom<string> chat = new WeightedRandom<string>();
            {
                if (CurrentState == States.Dreaming)
                {
                    chat.Add(ChatDictionary[7]);
                    chat.Add(ChatDictionary[8]);
                    chat.Add(ChatDictionary[9]);
                }
                else if (Insomnia)
                {
                    chat.Add(ChatDictionary[10]);
                    chat.Add(ChatDictionary[11]);
                    chat.Add(ChatDictionary[12]);
                }
                else
                {
                    chat.Add(ChatDictionary[1]);
                    chat.Add(ChatDictionary[2]);
                    chat.Add(ChatDictionary[3]);
                    chat.Add(ChatDictionary[4]);
                    chat.Add(ChatDictionary[5]);
                    chat.Add(ChatDictionary[6]);
                    if (Main.eclipse)
                    {
                        chat.Add(ChatDictionary[13]);
                    }
                }
            }
            return chat;
        }
        private void UpdateTalking()
        {
        }
        public override void VisualEffectForPreview()
        {
            UpdateMiscFrame();
        }
        public override void SetPetLight(ref Vector2 position, ref Vector3 rgb, ref bool inactive)
        {
            rgb = new Vector3(1.97f, 0.67f, 1.64f);
        }
        public override void AI()
        {
            Projectile.SetPetActive(Owner, BuffType<DoremyBuff>());

            UpdateTalking();

            ControlMovement(Owner);

            if (CurrentState == States.Dreaming)
            {
                GenDust();
            }

            switch (CurrentState)
            {
                case States.Blink:
                    Blink();
                    break;

                case States.Dreaming:
                    shouldNotTalking = true;
                    Dreaming();
                    break;

                case States.AfterDreaming:
                    shouldNotTalking = true;
                    AfterDreaming();
                    break;

                default:
                    Idle();
                    break;
            }
            if (IsIdleState)
            {
                IdleAnimation();
            }
            if (IsIdleState && ActionCD > 0)
            {
                ActionCD--;
            }
            UpdateExtraPos();
        }
        private void UpdateExtraPos()
        {
            extraY = 0;
            extraX = Projectile.frame switch
            {
                5 or 9 => 2,
                6 or 7 or 8 => 4,
                _ => 0,
            };
            extraX *= Projectile.spriteDirection;
        }
        private void GenDust()
        {
            if (Main.rand.NextBool(17))
            {
                Dust d = Dust.NewDustPerfect(Projectile.position + new Vector2(Main.rand.Next(0, Projectile.width)
                    , Main.rand.Next(0, Projectile.height)), MyDustId.PinkBubble
                    , new Vector2(Main.rand.NextFloat(-1f, 1f), Main.rand.NextFloat(-3f, -1f)), 100, default
                    , Main.rand.NextFloat(1f, 2f));
                d.noGravity = true;
                d.shader = GameShaders.Armor.GetSecondaryShader(Owner.cLight, Owner);
            }
        }
        private void ControlMovement(Player player)
        {
            Projectile.rotation = Projectile.velocity.X * 0.003f;

            if (IsIdleState)
            {
                ChangeDir();
            }

            Vector2 point = new Vector2(46 * player.direction, -37 + player.gfxOffY);
            if (CurrentState != States.Dreaming || !Owner.sleeping.isSleeping)
            {
                MoveToPoint(point, 10.5f);
                Projectile.tileCollide = false;
            }
            else
            {
                Projectile.tileCollide = true;
                Projectile.velocity.X *= 0.9f;
            }
        }
        private void Idle()
        {
            if (Main.bloodMoon && blinkFrame < 1)
            {
                blinkFrame = 1;
            }
            if (OwnerIsMyPlayer)
            {
                if (mainTimer % 270 == 0)
                {
                    CurrentState = States.Blink;
                }
                if (CanDreaming && CurrentState == States.Idle)
                {
                    CurrentState = States.Dreaming;
                }
            }
        }
        private void Blink()
        {
            if (++blinkFrameCounter > 3)
            {
                blinkFrameCounter = 0;
                blinkFrame++;
            }
            if (blinkFrame > 2)
            {
                blinkFrame = Insomnia ? 1 : 0;
                CurrentState = States.Idle;
            }
        }
        private void Dreaming()
        {
            if (++Projectile.frameCounter > 8)
            {
                Projectile.frameCounter = 0;
                Projectile.frame++;
            }
            if (Projectile.frame >= 6)
            {
                if (Projectile.Center.Distance(Owner.Center) < 200)
                {
                    if (Owner.sleeping.isSleeping && Projectile.velocity.Y < 3)
                    {
                        Projectile.velocity.Y += 0.02f;
                    }
                }
                else
                {
                    Projectile.velocity *= 0.95f;
                }
            }
            if (Projectile.frame > 7)
            {
                Projectile.frame = 6;
            }

            if (OwnerIsMyPlayer && !CanDreaming)
            {
                CurrentState = States.AfterDreaming;
            }
        }
        private void AfterDreaming()
        {
            int count = Projectile.frame switch
            {
                8 or 9 => 120,
                _ => 8,
            };
            if (++Projectile.frameCounter > count)
            {
                Projectile.frameCounter = 0;
                Projectile.frame++;
            }
            if (Projectile.frame > 12)
            {
                Projectile.frame = 0;
                if (OwnerIsMyPlayer)
                {
                    ActionCD = 1800;
                    CurrentState = States.Idle;
                }
            }
        }
        private void IdleAnimation()
        {
            if (++Projectile.frameCounter > 8)
            {
                Projectile.frameCounter = 0;
                Projectile.frame++;
            }
            if (Projectile.frame > 3)
            {
                Projectile.frame = 0;
            }
        }
        private void UpdateMiscFrame()
        {
            if (hatFrame < 3)
            {
                hatFrame = 3;
            }
            if (++hatFrameCounter > 8)
            {
                hatFrameCounter = 0;
                hatFrame++;
            }
            if (hatFrame > 6)
            {
                hatFrame = 3;
            }

            if (tailFrame < 7)
            {
                tailFrame = 7;
            }
            if (++tailFrameCounter > 10)
            {
                tailFrameCounter = 0;
                tailFrame++;
            }
            if (tailFrame > 11)
            {
                tailFrame = 7;
            }
        }
    }
}


