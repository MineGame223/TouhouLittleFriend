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
        public override void SetStaticDefaults()
        {
            Main.projFrames[Type] = 11;
            Main.projPet[Type] = true;
            ProjectileID.Sets.LightPet[Type] = false;
        }
        public override bool PreDraw(ref Color lightColor)
        {
            DrawRumia(Projectile.frame, lightColor);
            if (PetState == 1)
                DrawRumia(blinkFrame, lightColor);
            DrawRumia(Projectile.frame, lightColor, AltVanillaFunction.GetExtraTexture("Rumia_Cloth"), true);
            DrawRumia(clothFrame, lightColor, null, true);
            DrawDark();
            return false;
        }
        private void DrawRumia(int frame, Color lightColor, Texture2D tex = null, bool entitySpriteDraw = false)
        {
            Texture2D t = tex ?? AltVanillaFunction.ProjectileTexture(Type);
            int height = t.Height / Main.projFrames[Type];
            Vector2 pos = Projectile.Center - Main.screenPosition + new Vector2(0, 7f * Main.essScale);
            Rectangle rect = new Rectangle(0, frame * height, t.Width, height);
            Vector2 orig = rect.Size() / 2;
            SpriteEffects effect = Projectile.spriteDirection == -1 ? SpriteEffects.None : SpriteEffects.FlipHorizontally;
            if (entitySpriteDraw)
                Main.EntitySpriteDraw(t, pos, rect, Projectile.GetAlpha(lightColor), Projectile.rotation, orig, Projectile.scale, effect, 0f);
            else
                Main.spriteBatch.TeaNPCDraw(t, pos, rect, Projectile.GetAlpha(lightColor), Projectile.rotation, orig, Projectile.scale, effect, 0f);
        }
        private void DrawDark()
        {
            Texture2D tex = AltVanillaFunction.GetExtraTexture("SatoriEyeAura");
            Vector2 pos = Projectile.Center - Main.screenPosition;
            Color clr = Projectile.GetAlpha(Color.Black);
            Vector2 orig = tex.Size() / 2;
            for (int i = 0; i < 3; i++)
                Main.spriteBatch.TeaNPCDraw(tex, pos, null, clr * darkAuraScale, 0f, orig, darkAuraScale * 1.7f * Main.essScale, SpriteEffects.None, 0);
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
        int clothFrame, clothFrameCounter;
        int blinkFrame, blinkFrameCounter;
        float darkAuraScale;
        private void Darkin()
        {
            Projectile.velocity *= 0.7f;
            if (++Projectile.frameCounter > 7)
            {
                Projectile.frameCounter = 0;
                Projectile.frame++;
            }
            if (extraAI[0] == 0)
            {
                if (Projectile.frame >= 1 && Projectile.frameCounter > 4 || Projectile.frame >= 2)
                {
                    darkAuraScale = Math.Clamp(darkAuraScale + 0.03f, 0, 1);
                }
                if (Projectile.frame > 2)
                {
                    Projectile.frame = 2;
                }
                extraAI[1]++;
                if (extraAI[1] > extraAI[2])
                {
                    extraAI[1] = 0;
                    extraAI[0] = 1;
                }
            }
            else
            {
                darkAuraScale = Math.Clamp(darkAuraScale - 0.01f, 0, 1);
                if (darkAuraScale > 0.3f)
                {
                    Projectile.frame = 2;
                }
                if (Projectile.frame > 3)
                {
                    Projectile.frame = 0;
                    extraAI[0] = 1200;
                    extraAI[2] = 0;
                    PetState = 0;
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
        Color myColor = Color.Black;
        public override Color ChatTextBoardColor => Color.White;
        public override string GetChatText(out string[] text)
        {
            text = new string[21];           
            if (PetState == 2)
            {
                text[5] = ModUtils.GetChatText("Rumia", "5");
            }
            else
            {
                text[1] = ModUtils.GetChatText("Rumia", "1");
                text[2] = ModUtils.GetChatText("Rumia", "2");
                text[3] = ModUtils.GetChatText("Rumia", "3");
                text[4] = ModUtils.GetChatText("Rumia", "4");
                if (Main.dayTime)
                {
                    text[6] = ModUtils.GetChatText("Rumia", "6");
                }
            }            
            WeightedRandom<string> chat = new WeightedRandom<string>();
            {
                for (int i = 1; i < text.Length; i++)
                {
                    if (text[i] != null)
                    {
                        int weight = 1;
                        if (i == 1 || i == 6)
                        {
                            weight = 5;
                        }
                        if (i == 5)
                        {
                            weight = 3;
                        }
                        chat.Add(text[i], weight);
                    }
                }
            }
            return chat;
        }
        private void UpdateTalking()
        {
            if (mainTimer % 480 == 0 && Main.rand.NextBool(10) && mainTimer > 0)
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
            Player player = Main.player[Projectile.owner];
            Projectile.SetPetActive(player, BuffType<RumiaBuff>());
            UpdateTalking();
            Vector2 point = new Vector2(-50 * player.direction, -40 + player.gfxOffY);
            Projectile.tileCollide = false;
            Projectile.rotation = Projectile.velocity.X * 0.035f;

            ChangeDir(player, true);
            MoveToPoint(point, 14f);
            if (mainTimer % 270 == 0 && PetState == 0)
            {
                PetState = 1;
            }
            if (mainTimer >= 1200 && mainTimer < 3600 && PetState != 2 && extraAI[0] == 0)
            {
                if (mainTimer % 600 == 0 && Main.rand.NextBool(1))
                {
                    PetState = 2;
                    extraAI[2] = Main.rand.Next(600, 2400);
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
                Darkin();
            }
            if (PetState != 2)
            {
                darkAuraScale = Math.Clamp(darkAuraScale - 0.1f, 0, 1);
            }
        }
    }
}


