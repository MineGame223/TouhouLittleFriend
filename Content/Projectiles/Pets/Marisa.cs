using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ID;
using Terraria.Utilities;
using TouhouPets.Content.Buffs.PetBuffs;

namespace TouhouPets.Content.Projectiles.Pets
{
    public class Marisa : BasicTouhouPet
    {
        public override void SetStaticDefaults()
        {
            Main.projFrames[Type] = 15;
            Main.projPet[Type] = true;
            ProjectileID.Sets.LightPet[Type] = true;
        }
        public override bool PreDraw(ref Color lightColor)
        {
            DrawMarisa(broomFrame, lightColor, 1, null, true);
            Projectile.DrawStateNormalizeForPet();
            DrawMarisa(Projectile.frame, lightColor);
            if (PetState == 1)
                DrawMarisa(blinkFrame, lightColor);
            DrawMarisa(Projectile.frame, lightColor, 0, AltVanillaFunction.GetExtraTexture("Marisa_Cloth"), true);
            Projectile.DrawStateNormalizeForPet();
            DrawMarisa(lightFrame, Color.White, 1);
            return false;
        }
        private void DrawMarisa(int frame, Color lightColor, int columns = 0, Texture2D tex = null, bool entitySpriteDraw = false)
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
        private void Blink()
        {
            if (blinkFrame < 12)
            {
                blinkFrame = 12;
            }
            if (++blinkFrameCounter > 3)
            {
                blinkFrameCounter = 0;
                blinkFrame++;
            }
            if (blinkFrame > 14)
            {
                blinkFrame = 12;
                PetState = 0;
            }
        }
        int blinkFrame, blinkFrameCounter;
        int broomFrame, broomFrameCounter;
        int lightFrame, lightFrameCounter;
        private void Happy()
        {
            if (Projectile.frame < 4)
            {
                Projectile.frame = 4;
            }
            if (++Projectile.frameCounter > 6)
            {
                Projectile.frameCounter = 0;
                Projectile.frame++;
            }
            if (extraAI[0] == 0)
            {
                if (Projectile.frame > 11)
                {
                    Gore.NewGoreDirect(Projectile.GetSource_FromAI(), Projectile.Center + new Vector2(0, -27), new Vector2(Main.rand.Next(-5, 5), Main.rand.Next(-6, -3)) * 0.1f, Main.rand.Next(570, 573), Main.rand.NextFloat(0.9f, 1.1f));
                    Projectile.frame = 4;
                    extraAI[1]++;
                }
                if (Projectile.owner == Main.myPlayer)
                {
                    if (extraAI[1] > Main.rand.Next(10, 20))
                    {
                        extraAI[1] = 0;
                        extraAI[0] = 1;
                        Projectile.netUpdate = true;
                    }
                }
            }
            else
            {
                if (Projectile.frame > 11)
                {
                    Projectile.frame = 0;
                    extraAI[0] = 3600;
                    PetState = 0;
                }
            }
        }
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
        private void UpdateMiscFrame()
        {
            if (broomFrame < 4)
            {
                broomFrame = 4;
            }
            if (++broomFrameCounter > 6)
            {
                broomFrameCounter = 0;
                broomFrame++;
            }
            if (broomFrame > 7)
            {
                broomFrame = 4;
            }

            if (++lightFrameCounter > 10)
            {
                lightFrameCounter = 0;
                lightFrame++;
            }
            if (lightFrame > 3)
            {
                lightFrame = 0;
            }
        }
        Color myColor = new Color(255, 249, 137);
        public override string GetChatText(out string[] text)
        {
            Player player = Main.player[Projectile.owner];
            text = new string[21];
            text[1] = ModUtils.GetChatText("Marisa", "1");
            text[2] = ModUtils.GetChatText("Marisa", "2");
            text[3] = ModUtils.GetChatText("Marisa", "3");
            if (player.ZoneForest && Main.dayTime && Main.cloudAlpha == 0)
                text[4] = ModUtils.GetChatText("Marisa", "4");
            if (player.ZoneForest && !Main.dayTime && Main.cloudAlpha == 0)
                text[5] = ModUtils.GetChatText("Marisa", "5");
            if ((FindPetState(out Projectile _, ProjectileType<Reimu>(), 0, 1)
                || FindPetState(out Projectile _, ProjectileType<Reimu>(), 3, 4)))
            {
                text[6] = ModUtils.GetChatText("Marisa", "6");
            }
            if (Main.bloodMoon || Main.eclipse)
                text[7] = ModUtils.GetChatText("Marisa", "7");
            WeightedRandom<string> chat = new WeightedRandom<string>();
            {
                for (int i = 1; i < text.Length; i++)
                {
                    if (text[i] != null)
                    {
                        int weight = 1;
                        if (i == 10)
                        {
                            weight = 10;
                        }
                        chat.Add(text[i], weight);
                    }
                }
            }
            return chat;
        }
        private void UpdateTalking()
        {
            int type = ProjectileType<Alice>();
            int type2 = ProjectileType<Reimu>();
            if (FindChatIndex(out Projectile _, type, 4, default, 0))
            {
                ChatCD = 1;
            }
            if (FindChatIndex(out Projectile p1, type2, 5, default, 1, true))
            {
                SetChatWithOtherOne(p1, ModUtils.GetChatText("Marisa", "8"), myColor, 8, 600, -1, 9);
            }
            else if (FindChatIndex(out Projectile p2, type2, 6, default, 1, true))
            {
                SetChatWithOtherOne(p2, ModUtils.GetChatText("Marisa", "9"), myColor, 9, 600, -1, 9);
            }
            else if (FindChatIndex(out Projectile p3, type2, 7, default, 1, true))
            {
                SetChatWithOtherOne(p3, ModUtils.GetChatText("Marisa", "10"), myColor, 10, 360, -1, 9);
            }
            else if (FindChatIndex(out Projectile p4, type2, 8, default, 1, true))
            {
                SetChatWithOtherOne(p4, ModUtils.GetChatText("Marisa", "11"), myColor, 0, 360, -1);
                p4.localAI[2] = 0;
            }
            else if (FindChatIndex(out Projectile p5, type, 4, default, 1, true))
            {
                SetChatWithOtherOne(p5, ModUtils.GetChatText("Marisa", "12"), myColor, 12, 600, -1);
            }
            else if (FindChatIndex(out Projectile p6, type, 5, default, 1, true))
            {
                SetChatWithOtherOne(p6, ModUtils.GetChatText("Marisa", "13"), myColor, 13, 600, -1);
            }
            else if (FindChatIndex(out Projectile p7, type, 6, default, 1, true))
            {
                SetChatWithOtherOne(p7, ModUtils.GetChatText("Marisa", "14"), myColor, 14, 600, -1);
            }
            else if (mainTimer % 720 == 0 && Main.rand.NextBool(5) && PetState != 2)
            {
                SetChat(myColor);
            }
        }
        public override void VisualEffectForPreview()
        {
            UpdateMiscFrame();
            if (PetState != 2)
            {
                Idel();
            }
        }
        public override void AI()
        {
            Lighting.AddLight(Projectile.Center, 1.95f, 1.90f, 1.03f);
            Player player = Main.player[Projectile.owner];
            Projectile.SetPetActive(player, BuffType<MarisaBuff>());

            UpdateTalking();
            Vector2 point = new Vector2(50 * player.direction, -30 + player.gfxOffY);
            Projectile.tileCollide = false;
            Projectile.rotation = Projectile.velocity.X * 0.012f;

            ChangeDir(player);
            MoveToPoint(point, 12.5f);

            if (Projectile.owner == Main.myPlayer)
            {
                if (mainTimer % 270 == 0 && PetState != 2)
                {
                    PetState = 1;
                    Projectile.netUpdate = true;
                }
                if (mainTimer >= 1200 && mainTimer < 3600 && PetState != 1 && Projectile.frame == 3)
                {
                    if (mainTimer % 120 == 0 && Main.rand.NextBool(6) && extraAI[0] <= 0)
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
                Happy();
            }
        }
    }
}


