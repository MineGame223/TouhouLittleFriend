using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria;
using Terraria.ID;
using Terraria.Utilities;
using TouhouPets.Content.Buffs.PetBuffs;

namespace TouhouPets.Content.Projectiles.Pets
{
    public class Satori : BasicTouhouPet
    {
        public override void SetStaticDefaults()
        {
            Main.projFrames[Type] = 7;
            Main.projPet[Type] = true;
            ProjectileID.Sets.LightPet[Type] = true;
        }
        public override bool PreDraw(ref Color lightColor)
        {
            float t2 = Main.GlobalTimeWrappedHourly * 2.3f;
            Vector2 eyeAdj = new Vector2(1.2f * (float)Math.Cos(t2), 0.35f * (float)Math.Sin(t2)) * -26f;
            eyePos = Projectile.Center + eyeAdj + new Vector2(0, 8);

            if (eyeAdj.Y <= 0)
                DrawEye(eyePos - Main.screenPosition);

            Projectile.DrawStateNormalizeForPet();
            DrawSatori(Projectile.frame, lightColor);
            if (PetState == 1)
                DrawSatori(blinkFrame, lightColor);

            DrawSatori(Projectile.frame, lightColor, 0, AltVanillaFunction.GetExtraTexture("Satori_Cloth"), true);
            DrawSatori(clothFrame, lightColor, 1, null, true);

            Projectile.DrawStateNormalizeForPet();
            if (eyeAdj.Y > 0)
                DrawEye(eyePos - Main.screenPosition);
            return false;
        }
        private void DrawSatori(int frame, Color lightColor, int columns = 0, Texture2D tex = null, bool entitySpriteDraw = false)
        {
            Texture2D t = tex ?? AltVanillaFunction.ProjectileTexture(Type);
            int height = t.Height / Main.projFrames[Type];
            Vector2 pos = Projectile.Center - Main.screenPosition + new Vector2(0, 7f * Main.essScale);
            Rectangle rect = new Rectangle(t.Width / 2 * columns, frame * height, t.Width / 2, height);
            Vector2 orig = rect.Size() / 2;
            SpriteEffects effect = Projectile.spriteDirection == -1 ? SpriteEffects.None : SpriteEffects.FlipHorizontally;
            if (entitySpriteDraw)
                Main.EntitySpriteDraw(t, pos, rect, Projectile.GetAlpha(lightColor), Projectile.rotation, orig, Projectile.scale, effect, 0f);
            else
                Main.spriteBatch.TeaNPCDraw(t, pos, rect, Projectile.GetAlpha(lightColor), Projectile.rotation, orig, Projectile.scale, effect, 0f);
        }
        private void DrawEye(Vector2 eyePos)
        {
            Texture2D t = AltVanillaFunction.ProjectileTexture(Type);
            int height = t.Height / Main.projFrames[Type];
            Rectangle rect = new Rectangle(t.Width / 2, 4 * height, t.Width / 2, height);
            Vector2 orig = rect.Size() / 2;

            float s = 1 + Main.rand.NextFloat(0.9f, 1.1f);
            Texture2D glow = AltVanillaFunction.GetExtraTexture("SatoriEyeSpark");
            Texture2D aura = AltVanillaFunction.GetExtraTexture("SatoriEyeAura");
            Main.spriteBatch.QuickToggleAdditiveMode(true);
            for (int i = 0; i < 2; i++)
            {
                Main.spriteBatch.TeaNPCDraw(aura, eyePos + new Vector2(0, 2), null, Projectile.GetAlpha(Color.DeepPink) * 0.5f, Projectile.rotation, aura.Size() / 2, Projectile.scale * 0.38f * eyeSparkScale, SpriteEffects.None, 0f);
            }
            Main.spriteBatch.QuickToggleAdditiveMode(false);

            Main.spriteBatch.TeaNPCDraw(t, eyePos, rect, Projectile.GetAlpha(Color.White), Projectile.rotation, orig, Projectile.scale, SpriteEffects.None, 0f);

            Main.spriteBatch.QuickToggleAdditiveMode(true);
            for (int i = 0; i < 8; i++)
            {
                Main.spriteBatch.TeaNPCDraw(glow, eyePos + new Vector2(0, 2), null, Projectile.GetAlpha(Color.DeepPink) * 0.5f, Projectile.rotation + MathHelper.PiOver2, glow.Size() / 2, Projectile.scale * new Vector2(0.14f, 0.4f) * s * eyeSparkScale, SpriteEffects.None, 0f);
                Main.spriteBatch.TeaNPCDraw(glow, eyePos + new Vector2(0, 2), null, Projectile.GetAlpha(Color.DeepPink) * 0.5f, Projectile.rotation, glow.Size() / 2, Projectile.scale * new Vector2(0.14f, 0.26f) * s * eyeSparkScale, SpriteEffects.None, 0f);
            }
            Main.spriteBatch.QuickToggleAdditiveMode(false);
        }
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
        int blinkFrame, blinkFrameCounter;
        int clothFrame, clothFrameCounter;
        float eyeSparkScale;
        Vector2 eyePos;
        private void MindReading()
        {
            if (++Projectile.frameCounter > 8)
            {
                Projectile.frameCounter = 0;
                Projectile.frame++;
            }
            if (Projectile.frame > 2 && extraAI[1] > 0)
            {
                Projectile.frame = 2;
            }
            if (Projectile.frame >= 2)
            {
                extraAI[1]--;
                if (eyeSparkScale < 1)
                    eyeSparkScale += 0.01f;
            }
            if (Projectile.frame > 3)
            {
                extraAI[0] = 1200;
                extraAI[1] = 0;
                PetState = 0;
                Projectile.frame = 0;
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
        Color myColor = new Color(255, 149, 170);
        public override string GetChatText(out string[] text)
        {
            text = new string[21];
            text[1] = ModUtils.GetChatText("Satori", "1");
            text[2] = ModUtils.GetChatText("Satori", "2");
            text[3] = ModUtils.GetChatText("Satori", "3");
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
            int type2 = ProjectileType<Koishi>();
            int type3 = ProjectileType<Utsuho>();
            int type4 = ProjectileType<Rin>();
            if (FindChatIndex(out Projectile _, type2, 5, default, 0)
                || FindChatIndex(out Projectile _, type3, 5, default, 0)
                || FindChatIndex(out Projectile _, type4, 5, default, 0))
            {
                ChatCD = 1;
            }
            if (FindChatIndex(out Projectile p1, type2, 5, default, 1, true))
            {
                SetChatWithOtherOne(p1, ModUtils.GetChatText("Satori", "4"), myColor, 0, 600);
                p1.ai[0] = 0;
                talkInterval = 1200;
            }
            else if (FindChatIndex(out Projectile p2, type4, 5, default, 1, true))
            {
                SetChatWithOtherOne(p2, ModUtils.GetChatText("Satori", "5"), myColor, 0, 600);
                p2.ai[0] = 0;
                talkInterval = 2400;
            }
            else if (FindChatIndex(out Projectile p3, type3, 5, default, 1, true))
            {
                SetChatWithOtherOne(p3, ModUtils.GetChatText("Satori", "6"), myColor, 0, 600);
                p3.ai[0] = 0;
                talkInterval = 2400;
            }
            else if (FindChatIndex(out Projectile p4, type4, 6, default, 1, true))
            {
                SetChatWithOtherOne(p4, ModUtils.GetChatText("Satori", "7"), myColor, 0, 600);
                p4.ai[0] = 0;
                talkInterval = 2400;
            }
            else if (mainTimer % 960 == 0 && Main.rand.NextBool(8))
            {
                SetChat(myColor);
            }
        }
        public override void VisualEffectForPreview()
        {
            UpdateClothFrame();
        }
        public override void AI()
        {
            float lightPlus = 1 + eyeSparkScale;
            Lighting.AddLight(eyePos, 1.72f * lightPlus, 0.69f * lightPlus, 0.89f * lightPlus);
            Player player = Main.player[Projectile.owner];
            Projectile.SetPetActive(player, BuffType<SatoriBuff>());
            Projectile.SetPetActive(player, BuffType<KomeijiBuff>());

            UpdateTalking();
            Vector2 point = new Vector2(54 * player.direction, -34 + player.gfxOffY);
            if (player.ownedProjectileCounts[ProjectileType<Rin>()] > 0 && player.ownedProjectileCounts[ProjectileType<Utsuho>()] > 0)
                point = new Vector2(44 * player.direction, -80 + player.gfxOffY);
            Projectile.tileCollide = false;
            Projectile.rotation = Projectile.velocity.X * 0.01f;

            ChangeDir(player);
            MoveToPoint(point, 11f);

            if (mainTimer % 270 == 0 && PetState != 2)
            {
                PetState = 1;
            }
            if (PetState == 0)
            {
                if (mainTimer >= 600 && mainTimer % 120 == 0 && extraAI[0] <= 0)
                {
                    PetState = 2;
                    extraAI[1] = Main.rand.Next(360, 480);
                }
            }
            if (PetState == 0)
            {
                Projectile.frame = 0;
                if (extraAI[0] >= 1)
                {
                    extraAI[0]--;
                }
            }
            else if (PetState == 1)
            {
                Projectile.frame = 0;
                Blink();
            }
            else if (PetState == 2)
            {
                MindReading();
                if (eyeSparkScale >= 1)
                {
                    player.detectCreature = true;
                }
            }
            if (PetState != 2)
            {
                if (eyeSparkScale > 0)
                    eyeSparkScale -= 0.02f;
            }
            if (eyeSparkScale < 0)
                eyeSparkScale = 0;
        }
    }
}


