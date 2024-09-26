using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ID;
using Terraria.Utilities;
using TouhouPets.Content.Buffs.PetBuffs;

namespace TouhouPets.Content.Projectiles.Pets
{
    public class Eirin : BasicTouhouPet
    {
        private enum States
        {
            Idle,
            Blink,
            Shooting,
            AfterShooting,
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
        private int lightFrame, lightFrameCounter;
        private int clothFrame, clothFrameCounter;
        private int hairFrame, hairFrameCounter;

        private bool showArrow = false;
        private float arrowPosX, arrowAlpha;

        private DrawPetConfig drawConfig = new(2);
        private readonly Texture2D clothTex = AltVanillaFunction.GetExtraTexture("Eirin_Cloth");
        private readonly Texture2D glowTex = AltVanillaFunction.GetGlowTexture("Eirin_Glow");
        public override void SetStaticDefaults()
        {
            Main.projFrames[Type] = 12;
            Main.projPet[Type] = true;
            ProjectileID.Sets.LightPet[Type] = true;
        }
        public override bool DrawPetSelf(ref Color lightColor)
        {
            DrawPetConfig config = drawConfig with
            {
                ShouldUseEntitySpriteDraw = true,
            };

            DrawPetConfig config2 = drawConfig with
            {
                PositionOffset = new Vector2(-34, 3 * Main.essScale),
            };

            Projectile.DrawPet(lightFrame, lightColor, config2, 1);
            Projectile.DrawPet(lightFrame, Color.White * .7f,
                config2 with
                {
                    AltTexture = glowTex,
                }, 1);

            Projectile.DrawPet(hairFrame, lightColor, drawConfig, 1);

            Projectile.DrawPet(Projectile.frame, lightColor, drawConfig);

            if (CurrentState == States.Blink)
                Projectile.DrawPet(blinkFrame, lightColor, drawConfig);

            Projectile.DrawPet(Projectile.frame, lightColor,
                config with
                {
                    AltTexture = clothTex,
                });
            Projectile.DrawPet(clothFrame, lightColor, config, 1);

            Projectile.ResetDrawStateForPet();
            if (showArrow)
            {
                DrawArrow();
            }
            return false;
        }
        private void DrawArrow()
        {
            Texture2D arrow = AltVanillaFunction.GetExtraTexture("SatoriEyeSpark");
            Vector2 pos = Projectile.DefaultDrawPetPosition() + new Vector2(arrowPosX * Projectile.spriteDirection, 0);
            Main.spriteBatch.TeaNPCDraw(arrow, pos, null, Projectile.GetAlpha(Color.White)
                , Projectile.rotation + MathHelper.PiOver2, arrow.Size() / 2, new Vector2(1.2f - arrowAlpha, 3 * arrowAlpha), SpriteEffects.None, 0);
        }
        public override Color ChatTextColor => new Color(237, 237, 237);
        public override void RegisterChat(ref string name, ref Vector2 indexRange)
        {
            name = "Eirin";
            indexRange = new Vector2(1, 5);
        }
        public override void SetRegularDialog(ref int timePerDialog, ref int chance, ref bool whenShouldStop)
        {
            timePerDialog = 860;
            chance = 8;
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
        private void UpdateTalking()
        {
        }
        public override void VisualEffectForPreview()
        {
            UpdateMiscFrame();
        }
        public override void SetPetLight(ref Vector2 position, ref Vector3 rgb, ref bool inactive)
        {
            rgb = new Vector3(0.73f, 1.76f, 2.32f);
        }
        public override void AI()
        {
            Projectile.SetPetActive(Owner, BuffType<EirinBuff>());

            UpdateTalking();

            ControlMovement(Owner);

            switch (CurrentState)
            {
                case States.Blink:
                    Blink();
                    break;

                case States.Shooting:
                    shouldNotTalking = true;
                    Shooting();
                    break;

                case States.AfterShooting:
                    shouldNotTalking = true;
                    AfterShooting();
                    break;

                default:
                    Idle();
                    break;
            }
            if (IsIdleState && ActionCD > 0)
            {
                ActionCD--;
            }
            UpdateArrow();
        }
        private void ControlMovement(Player player)
        {
            Projectile.tileCollide = false;
            if (IsIdleState)
            {
                Projectile.rotation = Projectile.velocity.X * 0.02f;
                ChangeDir(200);
            }
            else
            {
                Projectile.rotation = 0;
            }

            Vector2 point = new Vector2(60 * player.direction, -40 + player.gfxOffY);
            MoveToPoint(point, 12.5f);
        }
        private void UpdateArrow()
        {
            if (arrowPosX > 24 * 8)
            {
                showArrow = false;
            }
            if (showArrow)
            {
                arrowPosX += 24;
                if (arrowAlpha < 1)
                {
                    arrowAlpha += 0.2f;
                }
            }
            else
            {
                arrowPosX = 0;
                arrowAlpha = 0;
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
                if (mainTimer > 0 && mainTimer % 560 == 0 && currentChatRoom == null && ActionCD <= 0)
                {
                    if (Main.rand.NextBool(8))
                    {
                        RandomCount = Main.rand.Next(120, 180);
                        CurrentState = States.Shooting;
                    }
                }
            }
        }
        private void Blink()
        {
            Projectile.frame = 0;
            if (blinkFrame < 9)
            {
                blinkFrame = 9;
            }
            if (++blinkFrameCounter > 3)
            {
                blinkFrameCounter = 0;
                blinkFrame++;
            }
            if (blinkFrame > 11)
            {
                blinkFrame = 9;
                CurrentState = States.Idle;
            }
        }
        private void Shooting()
        {
            if (++Projectile.frameCounter > 6)
            {
                Projectile.frameCounter = 0;
                Projectile.frame++;
            }
            if (Projectile.frame >= 4)
            {
                Projectile.frame = 4;
                Timer++;
            }
            if (OwnerIsMyPlayer && Timer > RandomCount)
            {
                Timer = 0;
                CurrentState = States.AfterShooting;
            }
        }
        private void AfterShooting()
        {
            int count = 8;
            if (Projectile.frame == 6)
            {
                count = 30;
            }
            if (Projectile.frame == 5 && Projectile.frameCounter == 0)
            {
                showArrow = true;
            }
            if (++Projectile.frameCounter > count)
            {
                Projectile.frameCounter = 0;
                Projectile.frame++;
            }
            if (Projectile.frame > 8)
            {
                Projectile.frame = 0;
                if (OwnerIsMyPlayer)
                {
                    ActionCD = 1800;
                    CurrentState = States.Idle;
                }
            }
        }
        private void UpdateMiscFrame()
        {
            if (lightFrame < 8)
            {
                lightFrame = 8;
            }
            if (++lightFrameCounter > 15)
            {
                lightFrameCounter = 0;
                lightFrame++;
            }
            if (lightFrame > 11)
            {
                lightFrame = 8;
            }

            if (hairFrame < 4)
            {
                hairFrame = 4;
            }
            if (++hairFrameCounter > 7)
            {
                hairFrameCounter = 0;
                hairFrame++;
            }
            if (hairFrame > 7)
            {
                hairFrame = 4;
            }

            if (++clothFrameCounter > 6)
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


