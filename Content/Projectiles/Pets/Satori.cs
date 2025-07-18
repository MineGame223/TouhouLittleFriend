﻿using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria;
using Terraria.ID;
using Terraria.Localization;
using Terraria.Utilities;
using TouhouPets.Content.Buffs.PetBuffs;

namespace TouhouPets.Content.Projectiles.Pets
{
    public class Satori : BasicTouhouPet
    {
        private enum States
        {
            Idle,
            Blink,
            MindReading,
            AfterMindReading,
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
        private int clothFrame, clothFrameCounter;
        private float eyeSparkScale;
        private Vector2 eyePostion, eyePositionOffset;

        private DrawPetConfig drawConfig = new(2);
        private readonly Texture2D clothTex = AltVanillaFunction.GetExtraTexture("Satori_Cloth");
        public override void PetStaticDefaults()
        {
            Main.projFrames[Type] = 7;
            Main.projPet[Type] = true;
            ProjectileID.Sets.LightPet[Type] = true;

            ProjectileID.Sets.CharacterPreviewAnimations[Type] =
                ProjectileID.Sets.SimpleLoop(0, 1)
                .WhenSelected(2, 1);
        }
        public override TouhouPetID UniqueID => TouhouPetID.Satori;
        public override bool DrawPetSelf(ref Color lightColor)
        {
            DrawPetConfig config = drawConfig with
            {
                ShouldUseEntitySpriteDraw = true,
            };
            if (eyePositionOffset.Y <= 0)
            {
                DrawEye(eyePostion - Main.screenPosition);
                Projectile.ResetDrawStateForPet();
            }

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

            if (eyePositionOffset.Y > 0)
                DrawEye(eyePostion - Main.screenPosition);

            return false;
        }
        private void DrawEye(Vector2 eyePos)
        {
            Texture2D t = AltVanillaFunction.ProjectileTexture(Type);
            int height = t.Height / Main.projFrames[Type];
            Rectangle rect = new(t.Width / 2, 4 * height, t.Width / 2, height);
            Vector2 orig = rect.Size() / 2;

            float s = 1 + Main.rand.NextFloat(0.9f, 1.1f);
            Texture2D spark = AssetLoader.GlowSpark.Value;
            Texture2D aura = AssetLoader.GlowAura.Value;

            Color clr = Projectile.GetAlpha(Color.DeepPink) * mouseOpacity;
            clr.A *= 0;
            for (int i = 0; i < 2; i++)
            {
                Main.spriteBatch.MyDraw(aura, eyePos + new Vector2(0, 2), null, clr * 0.15f, Projectile.rotation, aura.Size() / 2, Projectile.scale * 0.38f * eyeSparkScale, SpriteEffects.None, 0f);
            }

            Main.spriteBatch.MyDraw(t, eyePos, rect, Projectile.GetAlpha(Color.White) * mouseOpacity, Projectile.rotation, orig, Projectile.scale, SpriteEffects.None, 0f);

            for (int i = 0; i < 8; i++)
            {
                Main.spriteBatch.MyDraw(spark, eyePos + new Vector2(0, 2), null, clr * 0.5f, Projectile.rotation + MathHelper.PiOver2, spark.Size() / 2, Projectile.scale * new Vector2(0.14f, 0.4f) * s * eyeSparkScale, SpriteEffects.None, 0f);
                Main.spriteBatch.MyDraw(spark, eyePos + new Vector2(0, 2), null, clr * 0.5f, Projectile.rotation, spark.Size() / 2, Projectile.scale * new Vector2(0.14f, 0.26f) * s * eyeSparkScale, SpriteEffects.None, 0f);
            }
        }
        public override ChatSettingConfig ChatSettingConfig => new ChatSettingConfig() with
        {
            TextColor = new Color(255, 149, 170),
        };
        public override void RegisterChat(ref string name, ref Vector2 indexRange)
        {
            name = "Satori";
            indexRange = new Vector2(1, 7);
        }
        public override void SetRegularDialog(ref int timePerDialog, ref int chance, ref bool whenShouldStop)
        {
            timePerDialog = 961;
            chance = 8;
            whenShouldStop = !IsIdleState;
        }
        public override WeightedRandom<LocalizedText> RegularDialogText()
        {
            WeightedRandom<LocalizedText> chat = new ();
            {
                chat.Add(ChatDictionary[1]);
                chat.Add(ChatDictionary[2]);
                chat.Add(ChatDictionary[3]);
            }
            return chat;
        }
        public override void VisualEffectForPreview()
        {
            UpdateClothFrame();
            UpdateEyePosition();
            UpdateMiscData();
        }
        public override void SetPetLight(ref Vector2 position, ref Vector3 rgb, ref bool inactive)
        {
            float lightPlus = 1 + eyeSparkScale;

            position = eyePostion;
            rgb = new Vector3(1.72f, 0.69f, 0.89f) * lightPlus;
        }
        public override void AI()
        {
            Projectile.SetPetActive(Owner, BuffType<SatoriBuff>());
            Projectile.SetPetActive(Owner, BuffType<KomeijiBuff>());

            ControlMovement();

            switch (CurrentState)
            {
                case States.Blink:
                    Blink();
                    break;

                case States.MindReading:
                    shouldNotTalking = true;
                    MindReading();
                    break;

                case States.AfterMindReading:
                    shouldNotTalking = true;
                    AfterMindReading();
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
        private void UpdateMiscData()
        {
            float muti = Projectile.isAPreviewDummy ? 2f : 1f;
            if (Projectile.frame == 2)
            {
                eyeSparkScale = MathHelper.Clamp(eyeSparkScale += 0.01f * muti * 2, 0, 1);
            }
            else
            {
                eyeSparkScale = MathHelper.Clamp(eyeSparkScale -= 0.02f * muti, 0, 1);
            }
        }
        private void UpdateEyePosition()
        {
            float time = Main.GlobalTimeWrappedHourly * 2f;
            eyePositionOffset = new Vector2(1.2f * (float)Math.Cos(time), 0.35f * (float)Math.Sin(time)) * 26f;
            eyePostion = Projectile.Center + eyePositionOffset + new Vector2(0, 8);
        }
        private void ControlMovement()
        {
            Projectile.tileCollide = false;
            Projectile.rotation = Projectile.velocity.X * 0.01f;

            ChangeDir();

            Vector2 point = new Vector2(54 * Owner.direction, -34 + Owner.gfxOffY);
            if (Owner.HasBuff<KomeijiBuff>())
                point = new Vector2(44 * Owner.direction, -80 + Owner.gfxOffY);
            MoveToPoint(point, 11f);
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
                    RandomCount = Main.rand.Next(360, 480);
                    CurrentState = States.MindReading;
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
        private void MindReading()
        {
            if (SpecialAbility_Satori)
            {
                Owner.detectCreature = true;
            }
            if (++Projectile.frameCounter > 8)
            {
                Projectile.frameCounter = 0;
                Projectile.frame++;
            }
            if (Projectile.frame >= 2)
            {
                Projectile.frame = 2;
            }
            Timer++;
            if (OwnerIsMyPlayer && Timer > RandomCount)
            {
                Timer = 0;
                CurrentState = States.AfterMindReading;
            }
        }
        private void AfterMindReading()
        {
            if (++Projectile.frameCounter > 8)
            {
                Projectile.frameCounter = 0;
                Projectile.frame++;
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


