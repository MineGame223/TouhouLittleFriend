using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.Enums;
using Terraria.ID;
using Terraria.Utilities;
using TouhouPets.Content.Buffs.PetBuffs;

namespace TouhouPets.Content.Projectiles.Pets
{
    public class Junko : BasicTouhouPet
    {
        public override void SetStaticDefaults()
        {
            Main.projFrames[Type] = 11;
            Main.projPet[Type] = true;
            ProjectileID.Sets.LightPet[Type] = true;
        }
        #region 绘制，太繁杂了所以隐藏
        float auraValue;
        private void DrawAuraSingle(Texture2D t, Vector2 pos, Rectangle rect, Color color, float rot, Vector2 orig, SpriteEffects effect)
        {
            for (int i = 0; i < 4; i++)
            {
                Main.EntitySpriteDraw(t, pos + new Vector2(-4 * Projectile.spriteDirection, -6) + new Vector2(0, 3f * Main.essScale) + new Vector2(Main.rand.Next(-10, 11) * 0.25f, Main.rand.Next(-10, 11) * 0.25f), rect, color * 0.1f * auraValue, Projectile.rotation + rot, orig, Projectile.scale * 1.75f * auraValue * new Vector2(1, 1 + 1.1f * Main.essScale * auraValue), effect, 0f);
                Main.EntitySpriteDraw(t, pos + new Vector2(-4 * Projectile.spriteDirection, -6) + new Vector2(0, 3f * Main.essScale) + new Vector2(Main.rand.Next(-10, 11) * 0.25f, Main.rand.Next(-10, 11) * 0.25f), rect, color * 0.25f * auraValue, Projectile.rotation + rot, orig, Projectile.scale * 1.5f * auraValue * new Vector2(1, 1 + 1.1f * Main.essScale * auraValue), effect, 0f);
                Main.EntitySpriteDraw(t, pos + new Vector2(-4 * Projectile.spriteDirection, -6) + new Vector2(0, 3f * Main.essScale) + new Vector2(Main.rand.Next(-10, 11) * 0.25f, Main.rand.Next(-10, 11) * 0.25f), rect, color * 0.5f, Projectile.rotation + rot, orig, Projectile.scale * 1.25f * new Vector2(1, 1 + 1.1f * Main.essScale * auraValue), effect, 0f);
                Main.EntitySpriteDraw(t, pos + new Vector2(-4 * Projectile.spriteDirection, -6) + new Vector2(0, 3f * Main.essScale) + new Vector2(Main.rand.Next(-10, 11) * 0.25f, Main.rand.Next(-10, 11) * 0.25f), rect, color * 0.75f, Projectile.rotation + rot, orig, Projectile.scale * 0.75f * new Vector2(1, 1 + 1.1f * Main.essScale * auraValue), effect, 0f);
            }
        }
        private void DrawAuraSingle2(Texture2D t, Vector2 pos, Rectangle rect, Color color, float rot, Vector2 orig, SpriteEffects effect)
        {
            for (int i = 0; i < 4; i++)
            {
                Main.EntitySpriteDraw(t, pos + new Vector2(-4 * Projectile.spriteDirection, -6) + new Vector2(0, 3f * Main.essScale) + new Vector2(Main.rand.Next(-10, 11) * 0.25f, Main.rand.Next(-10, 11) * 0.25f), rect, color * 0.1f * auraValue, Projectile.rotation + rot, orig, Projectile.scale * 1.75f * auraValue * new Vector2(2f, 0.75f + 1.1f * Main.essScale * auraValue), effect, 0f);
                Main.EntitySpriteDraw(t, pos + new Vector2(-4 * Projectile.spriteDirection, -6) + new Vector2(0, 3f * Main.essScale) + new Vector2(Main.rand.Next(-10, 11) * 0.25f, Main.rand.Next(-10, 11) * 0.25f), rect, color * 0.25f * auraValue, Projectile.rotation + rot, orig, Projectile.scale * 1.5f * auraValue * new Vector2(2f, 0.75f + 1.1f * Main.essScale * auraValue), effect, 0f);
                Main.EntitySpriteDraw(t, pos + new Vector2(-4 * Projectile.spriteDirection, -6) + new Vector2(0, 3f * Main.essScale) + new Vector2(Main.rand.Next(-10, 11) * 0.25f, Main.rand.Next(-10, 11) * 0.25f), rect, color * 0.5f, Projectile.rotation + rot, orig, Projectile.scale * 1.25f * new Vector2(2f, 0.75f + 1.1f * Main.essScale * auraValue), effect, 0f);
                Main.EntitySpriteDraw(t, pos + new Vector2(-4 * Projectile.spriteDirection, -6) + new Vector2(0, 3f * Main.essScale) + new Vector2(Main.rand.Next(-10, 11) * 0.25f, Main.rand.Next(-10, 11) * 0.25f), rect, color * 0.75f, Projectile.rotation + rot, orig, Projectile.scale * 0.75f * new Vector2(2f, 0.75f + 1.1f * Main.essScale * auraValue), effect, 0f);
            }
        }
        private void DrawAura(Texture2D t, Vector2 pos, Rectangle rect, Vector2 orig, SpriteEffects effect)
        {
            Main.spriteBatch.QuickToggleAdditiveMode(true, Projectile.isAPreviewDummy);
            float rot2 = 20;
            DrawAuraSingle(t, pos, rect, Projectile.GetAlpha(Color.White), 0, orig, effect);
            DrawAuraSingle2(t, pos + new Vector2(0, 6), rect, Projectile.GetAlpha(Color.White) * 0.5f, MathHelper.ToRadians(10) + MathHelper.ToRadians(rot2) * Main.essScale, orig, effect);
            DrawAuraSingle(t, pos + new Vector2(0, 6), rect, Projectile.GetAlpha(Color.White), MathHelper.ToRadians(25) + MathHelper.ToRadians(rot2) * Main.essScale, orig, effect);
            DrawAuraSingle2(t, pos + new Vector2(0, 6), rect, Projectile.GetAlpha(Color.White) * 0.5f, MathHelper.ToRadians(55) + MathHelper.ToRadians(rot2) * Main.essScale, orig, effect);
            DrawAuraSingle(t, pos + new Vector2(0, 16), rect, Projectile.GetAlpha(Color.White), MathHelper.ToRadians(60) + MathHelper.ToRadians(rot2) * Main.essScale, orig, effect);
            DrawAuraSingle2(t, pos + new Vector2(-4 * -Projectile.spriteDirection, 6), rect, Projectile.GetAlpha(Color.White) * 0.5f, MathHelper.ToRadians(110) + MathHelper.ToRadians(rot2) * Main.essScale, orig, effect);
            DrawAuraSingle(t, pos + new Vector2(-4 * -Projectile.spriteDirection, 6), rect, Projectile.GetAlpha(Color.White), MathHelper.ToRadians(120) + MathHelper.ToRadians(rot2) * Main.essScale, orig, effect);

            DrawAuraSingle2(t, pos + new Vector2(0, 6), rect, Projectile.GetAlpha(Color.White) * 0.5f, MathHelper.ToRadians(-10) + MathHelper.ToRadians(-rot2) * Main.essScale, orig, effect);
            DrawAuraSingle(t, pos + new Vector2(0, 6), rect, Projectile.GetAlpha(Color.White), MathHelper.ToRadians(-25) + MathHelper.ToRadians(-rot2) * Main.essScale, orig, effect);
            DrawAuraSingle2(t, pos + new Vector2(0, 6), rect, Projectile.GetAlpha(Color.White) * 0.5f, MathHelper.ToRadians(-55) + MathHelper.ToRadians(-rot2) * Main.essScale, orig, effect);
            DrawAuraSingle(t, pos + new Vector2(0, 16), rect, Projectile.GetAlpha(Color.White), MathHelper.ToRadians(-60) + MathHelper.ToRadians(-rot2) * Main.essScale, orig, effect);
            DrawAuraSingle2(t, pos + new Vector2(4 * -Projectile.spriteDirection, 6), rect, Projectile.GetAlpha(Color.White) * 0.5f, MathHelper.ToRadians(-110) + MathHelper.ToRadians(-rot2) * Main.essScale, orig, effect);
            DrawAuraSingle(t, pos + new Vector2(4 * -Projectile.spriteDirection, 6), rect, Projectile.GetAlpha(Color.White), MathHelper.ToRadians(-120) + MathHelper.ToRadians(-rot2) * Main.essScale, orig, effect);
            Main.spriteBatch.QuickToggleAdditiveMode(false, Projectile.isAPreviewDummy);
        }
        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D t = AltVanillaFunction.ProjectileTexture(Type);
            int height = t.Height / Main.projFrames[Type];
            Vector2 pos = Projectile.Center - Main.screenPosition + new Vector2(0, 7f * Main.essScale);
            Rectangle rect = new Rectangle(0, Projectile.frame * height, t.Width / 2, height);
            Rectangle rect2 = new Rectangle(t.Width / 2, 1 * height, t.Width / 2, height);
            Rectangle rect4 = new Rectangle(0, blinkFrame * height, t.Width / 2, height);
            Vector2 orig = rect.Size() / 2;
            SpriteEffects effect = Projectile.spriteDirection == -1 ? SpriteEffects.None : SpriteEffects.FlipHorizontally;

            DrawAura(t, pos, rect2, orig, effect);
            Projectile.DrawStateNormalizeForPet();
            Main.spriteBatch.TeaNPCDraw(t, pos, rect, Projectile.GetAlpha(lightColor), Projectile.rotation, orig, Projectile.scale, effect, 0f);
            if (PetState == 1)
                Main.spriteBatch.TeaNPCDraw(t, pos, rect4, Projectile.GetAlpha(lightColor), Projectile.rotation, orig, Projectile.scale, effect, 0f);

            Main.EntitySpriteDraw(AltVanillaFunction.GetExtraTexture("Junko_Cloth"), pos, rect, Projectile.GetAlpha(lightColor), Projectile.rotation, orig, Projectile.scale, effect, 0f);
            return false;
        }
        #endregion
        private void Blink()
        {
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
                PetState = 0;
            }
        }
        int blinkFrame, blinkFrameCounter;
        private void Wrath()
        {
            if (Projectile.frame < 4)
            {
                Projectile.frame = 4;
            }
            if (extraAI[0] == 0)
            {
                if (++Projectile.frameCounter > 3)
                {
                    Projectile.frameCounter = 0;
                    Projectile.frame++;
                }
                if (Projectile.frame > 6)
                {
                    Projectile.frame = 5;
                    extraAI[1]++;
                }
                if (Projectile.owner == Main.myPlayer)
                {
                    if (extraAI[1] > Main.rand.Next(48, 72))
                    {
                        extraAI[1] = 0;
                        extraAI[0] = 1;
                        Projectile.netUpdate = true;
                    }
                }
            }
            else
            {
                if (++Projectile.frameCounter > 6)
                {
                    Projectile.frameCounter = 0;
                    Projectile.frame++;
                }
                if (Projectile.frame > 7)
                {
                    Projectile.frame = 0;
                    extraAI[0] = 2400;
                    PetState = 0;
                }
            }
        }
        private void Idel()
        {
            if (++Projectile.frameCounter > 4)
            {
                Projectile.frameCounter = 0;
                Projectile.frame++;
            }
            if (Projectile.frame > 3)
            {
                Projectile.frame = 0;
            }
        }
        Color myColor = new Color(254, 159, 75);
        public override string GetChatText(out string[] text)
        {
            text = new string[11];
            if (!Main.dayTime && Main.cloudAlpha <= 0 && Main.GetMoonPhase() == MoonPhase.Full)
                text[1] = ModUtils.GetChatText("Junko", "1");
            text[2] = "......";
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
            if (mainTimer % 960 == 0 && mainTimer > 0 && PetState != 2 && Main.rand.NextBool(9))
            {
                SetChat(myColor);
            }
        }
        public override void VisualEffectForPreview()
        {
            if (PetState != 2)
                Idel();
        }
        public override void AI()
        {
            Lighting.AddLight(Projectile.Center, 2.38f + 5 * auraValue, 1.41f + 5 * auraValue, 2.55f + 5 * auraValue);
            Player player = Main.player[Projectile.owner];
            Projectile.SetPetActive(player, BuffType<JunkoBuff>());
            UpdateTalking();
            Vector2 point = new Vector2(45 * player.direction, -65 + player.gfxOffY);
            Projectile.tileCollide = false;
            Projectile.rotation = Projectile.velocity.X * 0.018f;

            ChangeDir(player);
            MoveToPoint(point, 17f);

            if (Projectile.owner == Main.myPlayer)
            {
                if (mainTimer % 270 == 0 && PetState != 2)
                {
                    PetState = 1;
                    Projectile.netUpdate = true;
                }
                if (mainTimer >= 1200 && mainTimer < 3600 && PetState != 1)
                {
                    if (mainTimer % 450 == 0 && Main.rand.NextBool(6) && extraAI[0] <= 0)
                    {
                        PetState = 2;
                        Projectile.netUpdate = true;
                    }
                }
            }
            if (PetState == 0)
            {
                if (extraAI[0] >= 1)
                {
                    extraAI[0]--;
                }
            }
            else if (PetState == 1)
            {
                Blink();
            }
            else if (PetState == 2)
            {
                Wrath();
            }
            if (PetState == 2)
            {
                auraValue += 0.08f;
            }
            else
            {
                auraValue -= 0.01f;
            }
            auraValue = MathHelper.Clamp(auraValue, 0, 1);
        }
    }
}


