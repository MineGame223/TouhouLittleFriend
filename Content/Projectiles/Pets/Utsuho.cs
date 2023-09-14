using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ID;
using Terraria.Utilities;
using TouhouPets.Content.Buffs.PetBuffs;

namespace TouhouPets.Content.Projectiles.Pets
{
    public class Utsuho : BasicTouhouPet
    {
        public override void SetStaticDefaults()
        {
            Main.projFrames[Type] = 11;
            Main.projPet[Type] = true;
            ProjectileID.Sets.LightPet[Type] = true;
        }
        public override bool PreDraw(ref Color lightColor)
        {
            DrawSun();
            DrawUtsuho(wingFrame, Projectile.GetAlpha(lightColor), null, true);
            DrawUtsuho(wingFrame, Projectile.GetAlpha(Color.White * 0.7f), AltVanillaFunction.GetGlowTexture("UtsuhoGlow"), true);
            Projectile.DrawStateNormalizeForPet();
            DrawUtsuho(Projectile.frame, Projectile.GetAlpha(lightColor));
            DrawUtsuho(Projectile.frame, Projectile.GetAlpha(lightColor), AltVanillaFunction.GetExtraTexture("Utsuho_Cloth"), true);
            //Projectile.DrawStateNormalizeForPet();
            DrawEye();
            return false;
        }
        private void DrawEye()
        {
            Texture2D t = AltVanillaFunction.GetGlowTexture("UtsuhoGlow_Eye");
            int height = t.Height / Main.projFrames[Type];
            Vector2 pos = Projectile.Center - Main.screenPosition + new Vector2(0, 7f * Main.essScale);
            Rectangle rect = new Rectangle(0, Projectile.frame * height, t.Width, height);
            Vector2 orig = rect.Size() / 2;
            SpriteEffects effect = Projectile.spriteDirection == -1 ? SpriteEffects.None : SpriteEffects.FlipHorizontally;
            for (int i = 0; i < 12; i++)
            {
                Vector2 spinningpoint = new Vector2(0f, -2f);
                Main.spriteBatch.TeaNPCDraw(t, pos + spinningpoint.RotatedBy(MathHelper.TwoPi * Main.GlobalTimeWrappedHourly + MathHelper.TwoPi / 6 * i * 0.6f), rect, Projectile.GetAlpha(Color.White) * 0.3f, Projectile.rotation, orig, Projectile.scale * 1.02f, effect, 0f);
            }
            Main.spriteBatch.TeaNPCDraw(t, pos, rect, Projectile.GetAlpha(Color.White), Projectile.rotation, orig, Projectile.scale, effect, 0f);
        }
        private void DrawSun()
        {
            Texture2D t3 = AltVanillaFunction.GetExtraTexture("UtsuhoSun");
            Vector2 pos = Projectile.Center - Main.screenPosition + new Vector2(0, 7f * Main.essScale);
            for (int i = 0; i < 5; i++)
            {
                Main.EntitySpriteDraw(t3, pos + sunPos + new Vector2(Main.rand.Next(-10, 11) * 0.2f, Main.rand.Next(-10, 11) * 0.2f), null, Projectile.GetAlpha(Color.White) * 0.5f, -mainTimer * 0.09f, t3.Size() / 2, Projectile.scale * 1.02f, SpriteEffects.None, 0f);
            }
            Main.EntitySpriteDraw(t3, pos + sunPos, null, Projectile.GetAlpha(Color.White), mainTimer * 0.05f, t3.Size() / 2, Projectile.scale, SpriteEffects.None, 0f);
        }
        private void DrawUtsuho(int frame, Color lightColor, Texture2D tex = null, bool entitySpriteDraw = false)
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
        private void Blink()
        {
            if (++Projectile.frameCounter > 3)
            {
                Projectile.frameCounter = 0;
                Projectile.frame++;
            }
            if (Projectile.frame > 2)
            {
                Projectile.frame = 0;
                PetState = 0;
            }
        }
        int wingFrame, wingFrameCounter;
        Vector2 sunPos;
        private void Fire()
        {
            if (extraAI[1] > 0)
            {
                extraAI[1]--;
            }
            if (Projectile.frame == 3 && Projectile.frameCounter >= 12)
            {
                Projectile.frame = 4;
                extraAI[1] = 60;
            }
            if (extraAI[1] > 0)
            {
                if (Main.rand.NextBool(2))
                    Dust.NewDustPerfect(Projectile.Center + new Vector2(26 * Projectile.spriteDirection, 6), MyDustId.OrangeFire2, new Vector2(Main.rand.Next(-3, 3) * 0.75f, Main.rand.Next(-3, 3) * 0.75f), 100, default, Main.rand.NextFloat(0.5f, 1.25f)).noGravity = true;
            }

            if (Projectile.frame < 3)
            {
                Projectile.frame = 3;
            }
            if (extraAI[1] <= 0)
                Projectile.frameCounter++;

            if (Projectile.frameCounter > 12)
            {
                Projectile.frameCounter = 0;
                Projectile.frame++;
            }
            if (Projectile.frame == 5 && Projectile.frameCounter == 1)
            {
                for (int i = 0; i < 7; i++)
                {
                    Vector2 center = Projectile.Center;
                    Vector2 vel = (new Vector2(6 * Projectile.spriteDirection, -4) * Main.rand.NextFloat(0.5f, 2f)).RotatedBy(MathHelper.ToRadians(Main.rand.Next(-30, 30)));
                    Dust.NewDustDirect(center, 4, 4, MyDustId.Smoke, vel.X, vel.Y, 100, default, Main.rand.NextFloat(0.5f, 1.5f)).noGravity = true;
                    vel = (new Vector2(6 * Projectile.spriteDirection, -4) * 2f * Main.rand.NextFloat(0.5f, 2f)).RotatedBy(MathHelper.ToRadians(Main.rand.Next(-10, 10)));
                    Dust.NewDustDirect(center, 4, 4, MyDustId.OrangeFire2, vel.X, vel.Y, 100, default, Main.rand.NextFloat(1.4f, 1.9f)).noGravity = true;
                }
                Projectile.velocity.X = 9 * -Projectile.spriteDirection;
                Projectile.velocity.Y = 4;
            }
            if (Projectile.frame >= 5)
            {
                Projectile.velocity *= 0.85f;
            }
            if (Projectile.frame > 6)
            {
                Projectile.frame = 0;
                extraAI[1] = 0;
                extraAI[0] = 600;
                PetState = 0;
            }
        }
        private void UpdateWingFrame()
        {
            if (wingFrame < 7)
            {
                wingFrame = 7;
            }
            if (++wingFrameCounter > 6)
            {
                wingFrameCounter = 0;
                wingFrame++;
            }
            if (wingFrame > 10)
            {
                wingFrame = 7;
            }
        }
        Color myColor = new Color(228, 184, 75);
        public override string GetChatText(out string[] text)
        {
            Player player = Main.player[Projectile.owner];
            text = new string[11];
            text[1] = ModUtils.GetChatText("Utsuho", "1");
            text[2] = ModUtils.GetChatText("Utsuho", "2");
            text[3] = ModUtils.GetChatText("Utsuho", "3");
            if (player.ZoneUnderworldHeight)
            {
                text[4] = ModUtils.GetChatText("Utsuho", "4");
            }
            if (player.ownedProjectileCounts[ProjectileType<Satori>()] > 0)
            {
                text[5] = ModUtils.GetChatText("Utsuho", "5");
            }
            WeightedRandom<string> chat = new WeightedRandom<string>();
            {
                for (int i = 1; i < text.Length; i++)
                {
                    if (text[i] != null)
                    {
                        int weight = 1;
                        if (i == 4)
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
            int type3 = ProjectileType<Rin>();
            if (FindChatIndex(out Projectile _, type3, 5, default, 0))
            {
                ChatCD = 1;
            }
            if (FindChatIndex(out Projectile p1, type3, 7, default, 1, true))
            {
                SetChatWithOtherOne(p1, ModUtils.GetChatText("Utsuho", "6"), myColor, 6, 600);
                p1.ai[0] = 0;
                talkInterval = 1200;
            }
            if (mainTimer % 720 == 0 && Main.rand.NextBool(7))
            {
                SetChat(myColor);
            }
        }
        public override void VisualEffectForPreview()
        {
            UpdateWingFrame();
        }
        public override void AI()
        {
            sunPos = new Vector2(0, -40);

            Lighting.AddLight(Projectile.Center + sunPos, 1.95f, 1.64f, 0.67f);
            Player player = Main.player[Projectile.owner];
            Projectile.SetPetActive(player, BuffType<UtsuhoBuff>());
            Projectile.SetPetActive(player, BuffType<HellPetsBuff>());

            UpdateTalking();          
            Vector2 point = new Vector2(0, -60 + player.gfxOffY);
            if (player.ownedProjectileCounts[ProjectileType<Rin>()] > 0)
            {
                point = new Vector2(50 * player.direction, -30 + player.gfxOffY);
                if (player.ownedProjectileCounts[ProjectileType<Satori>()] > 0 || player.ownedProjectileCounts[ProjectileType<Koishi>()] > 0)
                    point = new Vector2(70 * player.direction, 0 + player.gfxOffY);
            }
            Projectile.tileCollide = false;
            Projectile.rotation = Projectile.velocity.X * 0.012f;
            if (Projectile.frame < 5)
            {
                ChangeDir(player);
            }

            if (Projectile.frame < 5)
                MoveToPoint(point, 13.5f);

            if (mainTimer % 270 == 0 && PetState != 2)
            {
                PetState = 1;
            }
            if (mainTimer >= 1200 && mainTimer < 3600 && PetState != 1)
            {
                if (mainTimer % 300 == 0 && Main.rand.NextBool(7) && extraAI[0] <= 0)
                {
                    PetState = 2;
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
                Blink();
            }
            else if (PetState == 2)
            {
                Fire();
            }
            if (extraAI[0] >= 480)
            {
                if (Main.rand.NextBool(4))
                    Dust.NewDustPerfect(Projectile.Center + new Vector2(24 * Projectile.spriteDirection, 20), MyDustId.Smoke, new Vector2(0, Main.rand.Next(-4, -2) * 0.5f), 100, default, Main.rand.NextFloat(0.5f, 1.25f)).noGravity = true;
            }
        }
    }
}


