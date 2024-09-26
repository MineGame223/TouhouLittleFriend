using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria;
using Terraria.ID;
using Terraria.Utilities;
using TouhouPets.Content.Buffs.PetBuffs;

namespace TouhouPets.Content.Projectiles.Pets
{
    public class Rumia : BasicTouhouPet
    {
        private enum States
        {
            Idle,
            Blink,
            Darken,
            AfterDarken,
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

        private int clothFrame, clothFrameCounter;
        private int blinkFrame, blinkFrameCounter;
        private float darkAuraScale;

        private DrawPetConfig drawConfig = new(1);
        private readonly Texture2D clothTex = AltVanillaFunction.GetExtraTexture("Rumia_Cloth");
        public override void SetStaticDefaults()
        {
            Main.projFrames[Type] = 11;
            Main.projPet[Type] = true;
            ProjectileID.Sets.LightPet[Type] = false;
        }
        public override bool DrawPetSelf(ref Color lightColor)
        {
            DrawPetConfig config = drawConfig with
            {
                ShouldUseEntitySpriteDraw = true,
            };

            Projectile.DrawPet(Projectile.frame, lightColor, drawConfig);

            if (CurrentState == States.Blink)
                Projectile.DrawPet(blinkFrame, lightColor, drawConfig);

            Projectile.DrawPet(Projectile.frame, lightColor,
                config with
                {
                    AltTexture = clothTex,
                });
            Projectile.DrawPet(clothFrame, lightColor, config);

            DrawDark();
            return false;
        }
        private void DrawDark()
        {
            Texture2D tex = AltVanillaFunction.GetExtraTexture("SatoriEyeAura");
            Vector2 pos = Projectile.Center - Main.screenPosition;
            Color clr = Projectile.GetAlpha(Color.Black);
            Vector2 orig = tex.Size() / 2;
            for (int i = 0; i < 3; i++)
                Main.spriteBatch.TeaNPCDraw(tex, pos, null, clr * darkAuraScale, 0f, orig, darkAuraScale * 1.2f * Main.essScale, SpriteEffects.None, 0);
        }
        public override Color ChatTextBoardColor => Color.White;
        public override Color ChatTextColor => Color.Black;
        public override void RegisterChat(ref string name, ref Vector2 indexRange)
        {
            name = "Rumia";
            indexRange = new Vector2(1, 6);
        }
        public override void SetRegularDialog(ref int timePerDialog, ref int chance, ref bool whenShouldStop)
        {
            timePerDialog = 480;
            chance = 10;
            whenShouldStop = false;
        }
        public override string GetRegularDialogText()
        {
            WeightedRandom<string> chat = new WeightedRandom<string>();
            {
                if (CurrentState == States.Darken)
                {
                    chat.Add(ChatDictionary[5]);
                }
                else
                {
                    chat.Add(ChatDictionary[1], 5);
                    chat.Add(ChatDictionary[2]);
                    chat.Add(ChatDictionary[3]);
                    chat.Add(ChatDictionary[4]);
                    if (Main.dayTime)
                    {
                        chat.Add(ChatDictionary[6], 3);
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
            UpdateClothFrame();
        }
        public override void AI()
        {
            Projectile.SetPetActive(Owner, BuffType<RumiaBuff>());

            UpdateTalking();

            ControlMovement();

            switch (CurrentState)
            {
                case States.Blink:
                    Blink();
                    break;

                case States.Darken:
                    shouldNotTalking = true;
                    Darken();
                    break;

                case States.AfterDarken:
                    shouldNotTalking = true;
                    AfterDarken();
                    break;

                default:
                    Idle();
                    break;
            }

            if (IsIdleState)
            {
                if (ActionCD > 0)
                {
                    ActionCD--;
                }
                darkAuraScale = Math.Clamp(darkAuraScale - 0.1f, 0, 1);
            }
            Projectile.scale = Math.Clamp(Projectile.scale, 0, 1);
        }
        private void ControlMovement()
        {
            Projectile.tileCollide = false;
            Projectile.rotation = Projectile.velocity.X * 0.035f;

            ChangeDir();

            Vector2 point = new Vector2(-50 * Owner.direction, -40 + Owner.gfxOffY);
            MoveToPoint(point, 14f);
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
                if (mainTimer > 0 && mainTimer % 600 == 0 && currentChatRoom == null && ActionCD <= 0)
                {
                    if (Main.rand.NextBool(3))
                    {
                        RandomCount = Main.rand.Next(600, 2400);
                        CurrentState = States.Darken;

                        if (Main.rand.NextBool(2) && chatTimeLeft <= 0)
                            Projectile.SetChat(ChatSettingConfig, 3, 20);
                    }
                }
            }
        }
        private void Blink()
        {
            Projectile.frame = 0;
            if (blinkFrame < 4)
            {
                blinkFrame = 4;
            }
            if (++blinkFrameCounter > 3)
            {
                blinkFrameCounter = 0;
                blinkFrame++;
            }
            if (blinkFrame > 6)
            {
                blinkFrame = 4;
                CurrentState = States.Idle;
            }
        }
        private void Darken()
        {
            if (++Projectile.frameCounter > 7)
            {
                Projectile.frameCounter = 0;
                Projectile.frame++;
            }
            if (Projectile.frame >= 1 && Projectile.frameCounter > 4 || Projectile.frame >= 2)
            {
                darkAuraScale = Math.Clamp(darkAuraScale + 0.03f, 0, 1);
                if (darkAuraScale > 0.36f)
                    Projectile.scale -= 0.02f;
            }
            if (Projectile.frame > 2)
            {
                Projectile.frame = 2;
            }
            if (OwnerIsMyPlayer)
            {
                Timer++;
                if (Timer > RandomCount)
                {
                    Timer = 0;
                    CurrentState = States.AfterDarken;
                }
            }
        }
        private void AfterDarken()
        {
            if (++Projectile.frameCounter > 7)
            {
                Projectile.frameCounter = 0;
                Projectile.frame++;
            }

            darkAuraScale = Math.Clamp(darkAuraScale - 0.01f, 0, 1);
            if (Projectile.scale < 1)
                Projectile.scale += 0.05f;
            if (darkAuraScale > 0.3f)
            {
                Projectile.frame = 2;
            }
            if (Projectile.frame > 3)
            {
                Projectile.frame = 0;
                if (OwnerIsMyPlayer)
                {
                    ActionCD = 1200;
                    CurrentState = States.Idle;
                }
            }
        }
        private void UpdateClothFrame()
        {
            int count = 5;
            if (clothFrame < 7)
            {
                clothFrame = 7;
            }
            if (++clothFrameCounter > count)
            {
                clothFrameCounter = 0;
                clothFrame++;
            }
            if (clothFrame > 10)
            {
                clothFrame = 7;
            }
        }
    }
}


