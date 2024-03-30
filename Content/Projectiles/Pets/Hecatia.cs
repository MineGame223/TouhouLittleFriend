using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.Utilities;
using TouhouPets.Content.Buffs.PetBuffs;

namespace TouhouPets.Content.Projectiles.Pets
{
    public class Hecatia : BasicTouhouPet
    {
        public override void SetStaticDefaults()
        {
            Main.projFrames[Type] = 7;
            Main.projPet[Type] = true;
        }
        #region 绘制，太繁杂了所以隐藏
        private void DrawPlantes(Vector2 pos, Color color, SpriteEffects effect)
        {
            Texture2D t2 = AltVanillaFunction.GetExtraTexture("HecatiaPlanets");
            int height2 = t2.Height / 3;
            Rectangle rect3 = new Rectangle(0, 0 * height2, t2.Width, height2);
            Rectangle rect4 = new Rectangle(0, 1 * height2, t2.Width, height2);
            Rectangle rect5 = new Rectangle(0, 2 * height2, t2.Width, height2);
            Vector2 orig2 = rect3.Size() / 2;
            //异界 -0
            Main.spriteBatch.TeaNPCDraw(t2, pos + new Vector2(plantePos[0].X * -Projectile.spriteDirection, plantePos[0].Y).RotatedBy(Projectile.rotation), rect3, color, Projectile.rotation, orig2, Projectile.scale * 1.12f, effect, 0f);
            //地球 -1
            Main.spriteBatch.TeaNPCDraw(t2, pos + new Vector2(plantePos[1].X * -Projectile.spriteDirection, plantePos[1].Y).RotatedBy(Projectile.rotation), rect4, color, Projectile.rotation, orig2, Projectile.scale, effect, 0f);
            //月球 -2
            Main.spriteBatch.TeaNPCDraw(t2, pos + new Vector2(plantePos[2].X * -Projectile.spriteDirection, plantePos[2].Y).RotatedBy(Projectile.rotation), rect5, color, Projectile.rotation, orig2, Projectile.scale, effect, 0f);
        }

        DrawPetConfig drawConfig = new(3);
        readonly Texture2D clothTex = AltVanillaFunction.GetExtraTexture("Hecatia_Cloth");
        public override bool PreDraw(ref Color lightColor)
        {
            Vector2 pos = Projectile.Center - Main.screenPosition + new Vector2(0, 7f * Main.essScale);
            SpriteEffects effect = Projectile.spriteDirection == -1 ? SpriteEffects.None : SpriteEffects.FlipHorizontally;

            DrawPlantes(pos + new Vector2(0, 4 * Main.essScale), Projectile.GetAlpha(lightColor), effect);

            Projectile.DrawPet(Projectile.frame, lightColor, drawConfig);
            Projectile.DrawPet(Projectile.frame, lightColor * bodyAlpha[0], drawConfig);
            Projectile.DrawPet(Projectile.frame, lightColor * bodyAlpha[1], drawConfig, 1);
            Projectile.DrawPet(Projectile.frame, lightColor * bodyAlpha[2], drawConfig, 2);
            if (PetState == 1)
            {
                Projectile.DrawPet(blinkFrame, lightColor, drawConfig);
                Projectile.DrawPet(blinkFrame, lightColor * bodyAlpha[0], drawConfig);
                Projectile.DrawPet(blinkFrame, lightColor * bodyAlpha[1], drawConfig, 1);
                Projectile.DrawPet(blinkFrame, lightColor * bodyAlpha[2], drawConfig, 2);
            }
            Projectile.DrawPet(Projectile.frame, lightColor, drawConfig with
            {
                AltTexture = clothTex,
                ShouldUseEntitySpriteDraw = true,
            });
            return false;
        }
        private void UpdateWorldState()
        {
            //懒得改
            float xSpeed = 1.5f;
            if (PlanteState > 2)
            {
                PlanteState = 0;
            }
            if (PlanteState == 1)
            {
                if (plantePos[1].X > 0)
                    plantePos[1].X--;
                if (plantePos[1].X < 0)
                    plantePos[1].X++;
                if (plantePos[1].Y > -27)
                    plantePos[1].Y--;

                plantePos[2].X += xSpeed;
                if (plantePos[2].Y > -5)
                    plantePos[2].Y--;
                if (plantePos[2].Y < -5)
                    plantePos[2].Y++;

                plantePos[0].X -= xSpeed;
                if (plantePos[0].Y > -5)
                    plantePos[0].Y--;
                if (plantePos[0].Y < -5)
                    plantePos[0].Y++;

                bodyAlpha[0] -= 0.02f;
                bodyAlpha[1] += 0.02f;
                bodyAlpha[2] -= 0.02f;
                myColor = new Color(79, 215, 239);
            }
            else if (PlanteState == 2)
            {
                if (plantePos[2].X > 0)
                    plantePos[2].X--;
                if (plantePos[2].X < 0)
                    plantePos[2].X++;
                if (plantePos[2].Y > -27)
                    plantePos[2].Y--;

                plantePos[0].X += xSpeed;
                if (plantePos[0].Y > -5)
                    plantePos[0].Y--;
                if (plantePos[0].Y < -5)
                    plantePos[0].Y++;

                plantePos[1].X -= xSpeed;
                if (plantePos[1].Y > -5)
                    plantePos[1].Y--;
                if (plantePos[1].Y < -5)
                    plantePos[1].Y++;

                bodyAlpha[0] -= 0.02f;
                bodyAlpha[1] -= 0.02f;
                bodyAlpha[2] += 0.02f;
                myColor = new Color(255, 249, 137);
            }
            else
            {
                if (plantePos[0].X > 0)
                    plantePos[0].X--;
                if (plantePos[0].X < 0)
                    plantePos[0].X++;
                if (plantePos[0].Y > -27)
                    plantePos[0].Y--;

                plantePos[1].X += xSpeed;
                if (plantePos[1].Y > -5)
                    plantePos[1].Y--;
                if (plantePos[1].Y < -5)
                    plantePos[1].Y++;

                plantePos[2].X -= xSpeed;
                if (plantePos[2].Y > -5)
                    plantePos[2].Y--;
                if (plantePos[2].Y < -5)
                    plantePos[2].Y++;

                bodyAlpha[0] += 0.02f;
                bodyAlpha[1] -= 0.02f;
                bodyAlpha[2] -= 0.02f;
                myColor = new Color(255, 120, 120);
            }
            for (int i = 0; i <= 2; i++)
            {
                if (bodyAlpha[i] > 1)
                {
                    bodyAlpha[i] = 1;
                }
                if (bodyAlpha[i] < 0)
                {
                    bodyAlpha[i] = 0;
                }
                if (plantePos[i].X > 20)
                {
                    plantePos[i].X = 20;
                }
                if (plantePos[i].X < -20)
                {
                    plantePos[i].X = -20;
                }
                if (plantePos[i].Y > -5)
                {
                    plantePos[i].Y = -5;
                }
                if (plantePos[i].Y < -27)
                {
                    plantePos[i].Y = -27;
                }
            }
        }

        float[] bodyAlpha = new float[3];
        Vector2[] plantePos = new Vector2[3];
        private int PlanteState
        {
            get => (int)Projectile.ai[2];
            set => Projectile.ai[2] = value;
        }
        #endregion
        private void Blink()
        {
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
                PetState = 0;
            }
        }
        private void Idle()
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
        int blinkFrame, blinkFrameCounter;
        Color myColor;
        public override string GetChatText(out string[] text)
        {
            text = new string[21];
            text[1] = ModUtils.GetChatText("Hecatia", "1");
            text[2] = ModUtils.GetChatText("Hecatia", "2");
            WeightedRandom<string> chat = new WeightedRandom<string>();
            {
                for (int i = 1; i < text.Length; i++)
                {
                    if (text[i] != null)
                    {
                        int weight = 1;
                        chat.Add(text[i], weight);
                    }
                }
            }
            return chat;
        }
        private void UpdateTalking()
        {
            if (mainTimer % 666 == 0 && Main.rand.NextBool(6))
            {
                SetChat(myColor);
            }
        }
        int dummyTimer = 0;
        public override void VisualEffectForPreview()
        {
            UpdateWorldState();
            Idle();

            if (Projectile.isAPreviewDummy)
            {
                dummyTimer++;
                if (dummyTimer >= 120)
                {
                    dummyTimer = 0;
                    PlanteState++;
                }
            }
        }
        public override void AI()
        {
            Player player = Main.player[Projectile.owner];
            Projectile.SetPetActive(player, BuffType<HecatiaBuff>());
            UpdateTalking();
            Vector2 point = new Vector2(-55 * player.direction, -40 + player.gfxOffY);
            Projectile.tileCollide = false;
            Projectile.rotation = Projectile.velocity.X * 0.030f;
            if (PetState != 2)
            {
                ChangeDir(player, true);
            }

            MoveToPoint(point, 14.5f);

            if (Projectile.owner == Main.myPlayer)
            {
                if (mainTimer % 270 == 0)
                {
                    PetState = 1;
                    Projectile.netUpdate = true;
                }
                if (mainTimer == 4798 && !Projectile.isAPreviewDummy)
                {
                    PlanteState++;
                    Projectile.netUpdate = true;
                }
            }
            if (PetState == 1)
            {
                Blink();
            }
        }
    }
}


