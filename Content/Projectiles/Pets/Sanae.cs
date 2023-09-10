using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria;
using Terraria.ID;
using Terraria.Utilities;
using TouhouPets.Content.Buffs.PetBuffs;

namespace TouhouPets.Content.Projectiles.Pets
{
    public class Sanae : BasicTouhouPet
    {
        public override void SetStaticDefaults()
        {
            Main.projFrames[Type] = 15;
            Main.projPet[Type] = true;
            ProjectileID.Sets.LightPet[Type] = true;
        }
        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D t = AltVanillaFunction.ProjectileTexture(Type);
            int height = t.Height / Main.projFrames[Type];
            Vector2 pos = Projectile.Center - Main.screenPosition + new Vector2(0, 7f * Main.essScale);
            Rectangle rect = new Rectangle(0, Projectile.frame * height, t.Width / 2, height);
            Rectangle rect2 = new Rectangle(0, blinkFrame * height, t.Width / 2, height);
            Rectangle rect3 = new Rectangle(0, clothFrame * height, t.Width / 2, height);
            Rectangle rect4 = new Rectangle(t.Width / 2, hairFrame * height, t.Width / 2, height);
            Rectangle rect5 = new Rectangle(t.Width / 2, itemFrame * height, t.Width / 2, height);
            Vector2 orig = rect.Size() / 2;
            SpriteEffects effect = Projectile.spriteDirection == -1 ? SpriteEffects.None : SpriteEffects.FlipHorizontally;

            if (PetState == 2)
            {
                float t2 = Main.GlobalTimeWrappedHourly * 6f;
                Main.spriteBatch.QuickToggleAdditiveMode(true);
                for (int o = 0; o < 8; o++)
                {
                    for (int i = -1; i < 2; i++)
                    {
                        DrawSanaeAura(Projectile.GetAlpha(Color.SeaGreen) * 0.4f,
                                pos + new Vector2(0, 2 * auraScale * i * (float)Math.Sin(t2)),
                                orig, effect, rect4, rect, rect3);
                        DrawSanaeAura(Projectile.GetAlpha(Color.SeaGreen) * 0.4f,
                            pos + new Vector2(2 * auraScale * i * (float)Math.Sin(t2), 0),
                            orig, effect, rect4, rect, rect3);
                    }
                }
                Main.spriteBatch.QuickToggleAdditiveMode(false);
            }
            Projectile.DrawStateNormalizeForPet();

            if (PetState < 3)
                Main.spriteBatch.TeaNPCDraw(t, pos + new Vector2(0, extraAdjY), rect4, Projectile.GetAlpha(lightColor), Projectile.rotation, orig, Projectile.scale, effect, 0f);

            Main.spriteBatch.TeaNPCDraw(t, pos, rect, Projectile.GetAlpha(lightColor), Projectile.rotation, orig, Projectile.scale, effect, 0f);
            if (PetState == 1 || PetState == 4)
                Main.spriteBatch.TeaNPCDraw(t, pos, rect2, Projectile.GetAlpha(lightColor), Projectile.rotation, orig, Projectile.scale, effect, 0f);
            Main.EntitySpriteDraw(AltVanillaFunction.GetExtraTexture("Sanae_Cloth"), pos, rect, Projectile.GetAlpha(lightColor), Projectile.rotation, orig, Projectile.scale, effect, 0f);
            Projectile.DrawStateNormalizeForPet();
            if (PetState < 3)
            {
                Main.spriteBatch.TeaNPCDraw(t, pos, rect5, Projectile.GetAlpha(lightColor), Projectile.rotation, orig, Projectile.scale, effect, 0f);
                Main.EntitySpriteDraw(t, pos + new Vector2(0, extraAdjY), rect3, Projectile.GetAlpha(lightColor), Projectile.rotation, orig, Projectile.scale, effect, 0f);
            }
            Projectile.DrawStateNormalizeForPet();
            return false;
        }
        private void DrawSanaeAura(Color lightColor, Vector2 pos, Vector2 orig, SpriteEffects effect, Rectangle rect1, Rectangle rect2, Rectangle rect3)
        {
            Texture2D t = AltVanillaFunction.ProjectileTexture(Type);
            Main.spriteBatch.TeaNPCDraw(t, pos + new Vector2(0, extraAdjY), rect1, lightColor, Projectile.rotation, orig, Projectile.scale, effect, 0f);
            Main.spriteBatch.TeaNPCDraw(t, pos, rect2, lightColor, Projectile.rotation, orig, Projectile.scale, effect, 0f);
            Main.spriteBatch.TeaNPCDraw(t, pos + new Vector2(0, extraAdjY), rect3, lightColor, Projectile.rotation, orig, Projectile.scale, effect, 0f);
        }
        private void Blink()
        {
            if (blinkFrame < 7)
            {
                blinkFrame = 7;
            }
            if (++blinkFrameCounter > 3)
            {
                blinkFrameCounter = 0;
                blinkFrame++;
            }
            if (blinkFrame > 9)
            {
                blinkFrame = 7;
                if (PetState == 4)
                {
                    PetState = 3;
                }
                else
                {
                    PetState = 0;
                }
            }
        }
        int blinkFrame, blinkFrameCounter;
        int clothFrame, clothFrameCounter;
        int itemFrame, itemFrameCounter;
        int hairFrame, hairFrameCounter;
        float auraScale;
        int extraAdjY;
        private void UpdateClothFrame()
        {
            if (clothFrame < 10)
            {
                clothFrame = 10;
            }
            int count = PetState == 2 ? 3 : 5;
            if (++clothFrameCounter > count)
            {
                clothFrameCounter = 0;
                clothFrame++;
            }
            if (clothFrame > 13)
            {
                clothFrame = 10;
            }
        }
        private void UpdateHairFrame()
        {
            if (hairFrame < 8)
            {
                hairFrame = 8;
            }
            int count = PetState == 2 ? 3 : 6;
            if (++hairFrameCounter > count)
            {
                hairFrameCounter = 0;
                hairFrame++;
            }
            if (hairFrame > 11)
            {
                hairFrame = 8;
            }
        }
        private void UpdateItemFrame()
        {
            int count = PetState == 2 ? 3 : 7;
            if (++itemFrameCounter > count)
            {
                itemFrameCounter = 0;
                itemFrame++;
            }
            if (PetState == 2)
            {
                if (Projectile.frame > 0)
                {
                    itemFrame = Projectile.frame + 3;
                }
                return;
            }
            if (itemFrame > 3)
            {
                itemFrame = 0;
            }
        }
        private void Pray()
        {
            chatFuncIsOccupied = true;
            if (Projectile.frame < 1)
            {
                Projectile.frame = 1;
            }
            if (auraScale < 2)
            {
                auraScale += 0.02f;
            }
            if (++Projectile.frameCounter > ((Projectile.frame >= 2 && Projectile.frame <= 3) ? 3 : 6))
            {
                Projectile.frameCounter = 0;
                Projectile.frame++;
            }
            if (Projectile.frame > 3 && extraAI[1] < extraAI[2])
            {
                Projectile.frame = 2;
                extraAI[1]++;
            }
            if (Projectile.frame > 4)
            {
                Projectile.frame = 0;
                extraAI[0] = 4800;
                extraAI[1] = 0;
                extraAI[2] = 0;
                PetState = 0;
            }
        }
        private void Flying()
        {
            if (Projectile.frame < 5)
            {
                Projectile.frame = 5;
            }
            if (++Projectile.frameCounter > 3)
            {
                Projectile.frameCounter = 0;
                Projectile.frame++;
            }
            if (Projectile.frame > 6)
            {
                Projectile.frame = 5;
            }
        }
        Color myColor = new Color(83, 241, 146);
        public override string GetChatText(out string[] text)
        {
            text = new string[21];
            text[1] = ModUtils.GetChatText("Sanae", "1");
            text[2] = ModUtils.GetChatText("Sanae", "2");
            text[3] = ModUtils.GetChatText("Sanae", "3");
            text[4] = ModUtils.GetChatText("Sanae", "4");
            if (Main.IsItAHappyWindyDay)
            {
                text[5] = ModUtils.GetChatText("Sanae", "5");
            }
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
            if (mainTimer % 960 == 0 && Main.rand.NextBool(7) && PetState != 2)
            {
                SetChat(myColor);
            }
        }
        int flyTimeleft = 0;
        public override void VisualEffectForPreview()
        {
            UpdateClothFrame();
            UpdateHairFrame();
            UpdateItemFrame();
        }
        public override void AI()
        {
            float lightPlus = 1 + auraScale * Main.essScale;
            Lighting.AddLight(Projectile.Center, 0.55f * lightPlus, 2.14f * lightPlus, 1.53f * lightPlus);
            if (flyTimeleft > 0)
            {
                flyTimeleft--;
            }
            Player player = Main.player[Projectile.owner];
            Projectile.SetPetActive(player, BuffType<SanaeBuff>());
            if (PetState != 2)
            {
                UpdateTalking();
            }
            Vector2 point = new Vector2(50 * player.direction, -30 + player.gfxOffY);
            Projectile.tileCollide = false;
            Projectile.rotation = Projectile.velocity.X * 0.012f;
            if (PetState != 2)
            {
                ChangeDir(player, PetState < 3);
                Lighting.AddLight(Projectile.Center, 0.62f, 1.02f, 0.95f);
            }

            MoveToPoint(point, 22f);

            if (mainTimer % 270 == 0 && PetState == 0)
            {
                PetState = 1;
            }
            else if (mainTimer % 270 == 0 && PetState == 3)
            {
                PetState = 4;
            }
            if (mainTimer >= 1200 && mainTimer < 3600 && PetState != 1 && PetState < 3)
            {
                if (mainTimer % 480 == 0 && Main.rand.NextBool(5) && extraAI[0] <= 0)
                {
                    PetState = 2;
                    extraAI[2] = Main.rand.Next(120, 240);
                }
            }
            if (player.velocity.Length() > 15f)
            {
                flyTimeleft = 5;
                if (PetState < 3)
                {
                    PetState = 3;
                }
            }
            else if (flyTimeleft <= 0)
            {
                if (PetState >= 3)
                {
                    extraAI[0] = 960;
                    extraAI[1] = 0;
                    extraAI[2] = 0;
                    PetState = 0;
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
                Pray();
            }
            else if (PetState == 3)
            {
                Flying();
            }
            else if (PetState == 4)
            {
                Flying();
                Blink();
            }
            if (PetState != 2)
            {
                if (chatFuncIsOccupied)
                    chatFuncIsOccupied = false;
                if (auraScale > 0)
                {
                    auraScale -= 0.01f;
                }
            }
            else
            {
                int dustType;
                switch (Main.rand.Next(4))
                {
                    default:
                        dustType = MyDustId.GreenTrans;
                        break;
                    case 1:
                        dustType = MyDustId.TrailingGreen1;
                        break;
                }
                if (Main.rand.NextBool(3))
                    Dust.NewDustDirect(Projectile.position, Projectile.width, Projectile.height, dustType,
                        0, Main.rand.Next(-3, -1), 100, default, auraScale * 0.45f).noGravity = true;
            }
            extraAdjY = 0;
            if (Projectile.frame >= 2 && Projectile.frame <= 3)
            {
                extraAdjY = -2;
            }
        }
    }
}


