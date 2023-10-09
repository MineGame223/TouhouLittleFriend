using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria;
using Terraria.ID;
using Terraria.Utilities;
using TouhouPets.Content.Buffs.PetBuffs;

namespace TouhouPets.Content.Projectiles.Pets
{
    public class Piece : BasicTouhouPet
    {
        public override void SetStaticDefaults()
        {
            Main.projFrames[Type] = 13;
            Main.projPet[Type] = true;
            ProjectileID.Sets.LightPet[Type] = true;
        }
        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D t = AltVanillaFunction.ProjectileTexture(Type);
            int height = t.Height / Main.projFrames[Type];
            Vector2 pos = Projectile.Center - Main.screenPosition + new Vector2(0, 7f * Main.essScale);
            Rectangle rect = new Rectangle(0, Projectile.frame * height, t.Width, height);
            Rectangle rect2 = new Rectangle(0, wingFrame * height, t.Width, height);
            Rectangle rect4 = new Rectangle(0, blinkFrame * height, t.Width, height);
            Vector2 orig = rect.Size() / 2;
            SpriteEffects effect = Projectile.spriteDirection == -1 ? SpriteEffects.None : SpriteEffects.FlipHorizontally;
            Main.spriteBatch.TeaNPCDraw(t, pos, rect2, Projectile.GetAlpha(lightColor), Projectile.rotation, orig, Projectile.scale, effect, 0f);
            Main.spriteBatch.TeaNPCDraw(t, pos, rect, Projectile.GetAlpha(lightColor), Projectile.rotation, orig, Projectile.scale, effect, 0f);
            if (PetState == 1)
                Main.spriteBatch.TeaNPCDraw(t, pos, rect4, Projectile.GetAlpha(lightColor), Projectile.rotation, orig, Projectile.scale, effect, 0f);

            Main.EntitySpriteDraw(AltVanillaFunction.GetExtraTexture("Piece_Cloth"), pos, rect, Projectile.GetAlpha(lightColor), Projectile.rotation, orig, Projectile.scale, effect, 0f);
            Projectile.DrawStateNormalizeForPet();
            
            DrawTorch(t, pos, Projectile.GetAlpha(lightColor), orig, effect);
            return false;
        }
        private void DrawTorch(Texture2D t, Vector2 pos, Color color, Vector2 orig, SpriteEffects effect)
        {
            int height = t.Height / Main.projFrames[Type];
            Main.spriteBatch.TeaNPCDraw(t, pos, new Rectangle(0, 11 * height, t.Width, height), color, Projectile.rotation, orig, Projectile.scale, effect, 0f);

            Main.spriteBatch.QuickToggleAdditiveMode(true);
            for (int i = 0; i < 7; i++)
            {
                Main.spriteBatch.TeaNPCDraw(t, pos + new Vector2(Main.rand.Next(-10, 11) * 0.25f, Main.rand.Next(-10, 11) * 0.25f), new Rectangle(0, 12 * height, t.Width, height), Projectile.GetAlpha(Color.White) * 0.5f, Projectile.rotation, orig, Projectile.scale, effect, 0f);
            }
            Main.spriteBatch.QuickToggleAdditiveMode(false);
        }
        private void Blink()
        {
            if (blinkFrame < 8)
            {
                blinkFrame = 8;
            }
            if (++blinkFrameCounter > 3)
            {
                blinkFrameCounter = 0;
                blinkFrame++;
            }
            if (blinkFrame > 10)
            {
                blinkFrame = 8;
                PetState = 0;
            }
        }
        int blinkFrame, blinkFrameCounter;
        int wingFrame, wingFrameCounter;
        private void Idel()
        {
            if (++Projectile.frameCounter > 6)
            {
                Projectile.frameCounter = 0;
                Projectile.frame++;
            }
            if (Projectile.frame > 3)
            {
                Projectile.frame = 0;
            }
        }
        private void UpdateWingFrame()
        {
            if (wingFrame < 4)
            {
                wingFrame = 4;
            }
            if (++wingFrameCounter > 3)
            {
                wingFrameCounter = 0;
                wingFrame++;
            }
            if (wingFrame > 7)
            {
                wingFrame = 4;
            }
        }
        Color myColor = new Color(255, 119, 187);
        public override string GetChatText(out string[] text)
        {
            text = new string[11];
            text[1] = ModUtils.GetChatText("Piece", "1");
            text[2] = ModUtils.GetChatText("Piece", "2");
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
            int type1 = ProjectileType<Hecatia>();
            if (FindChatIndex(out Projectile _, type1, 2, default, 0))
            {
                ChatCD = 1;
            }
            if (FindChatIndex(out Projectile p, type1, 2))
            {
                SetChatWithOtherOne(p, ModUtils.GetChatText("Piece", "3"), myColor, 0, 360);
                p.localAI[2] = 0;
            }
            else if (mainTimer % 720 == 0 && Main.rand.NextBool(6) && mainTimer > 0)
            {
                SetChat(myColor);
            }
        }
        public override void VisualEffectForPreview()
        {
            UpdateWingFrame();
            Idel();
        }
        public override void AI()
        {
            Lighting.AddLight(Projectile.Center, 1.38f, 0.41f, 1.55f);
            if (Main.rand.NextBool(30))
            {
                Dust d = Dust.NewDustDirect(Projectile.Center + new Vector2(26 * Projectile.spriteDirection, 0).RotatedBy(Projectile.rotation), 4, 4, MyDustId.PurpleTorch, 0f, 0f, 100);
                if (!Main.rand.NextBool(3))
                    d.noGravity = true;
                d.velocity *= 0.3f;
                d.velocity.Y -= 1.5f;
            }
            Player player = Main.player[Projectile.owner];
            Projectile.SetPetActive(player, BuffType<HecatiaBuff>());
            UpdateTalking();
            int xPos = 55;
            foreach (Projectile otherPets in Main.projectile)
            {
                if (otherPets != null && otherPets.active)
                {
                    if ((Main.projPet[otherPets.type] || ProjectileID.Sets.LightPet[otherPets.type])
                        && otherPets.type != Projectile.type && otherPets.type != ProjectileType<Hecatia>())
                    {
                        Projectile p = otherPets;
                        if (Math.Abs(p.position.X - player.position.X) < 180 && Math.Abs(p.position.Y - player.position.Y) < 180)
                        {
                            xPos = -115;
                        }
                    }
                }
            }
            Vector2 point = new Vector2(xPos * player.direction, -40 + player.gfxOffY);
            Projectile.tileCollide = false;
            Projectile.rotation = Projectile.velocity.X * 0.028f;

            ChangeDir(player, true, 150);
            MoveToPoint(point, 13f);

            if (mainTimer % 270 == 0 && PetState != 2)
            {
                PetState = 1;
            }
            if (PetState == 1)
            {
                Blink();
            }
        }
    }
}


