using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ID;
using Terraria.Utilities;
using TouhouPets.Content.Buffs.PetBuffs;

namespace TouhouPets.Content.Projectiles.Pets
{
    public class Nitori : BasicTouhouPet
    {
        public override void SetStaticDefaults()
        {
            Main.projFrames[Type] = 16;
            Main.projPet[Type] = true;
        }
        public override bool PreDraw(ref Color lightColor)
        {
            DrawNitori(backFrame, lightColor, 1);
            DrawNitori(backFrame, lightColor, 1, AltVanillaFunction.GetExtraTexture("Nitori_Cloth"), true);
            Projectile.DrawStateNormalizeForPet();
            DrawNitori(Projectile.frame, lightColor);
            if (PetState == 1)
                DrawNitori(blinkFrame, lightColor);
            DrawNitori(Projectile.frame, lightColor, 0, AltVanillaFunction.GetExtraTexture("Nitori_Cloth"), true);
            return false;
        }
        private void DrawNitori(int frame, Color lightColor, int columns = 0, Texture2D tex = null, bool entitySpriteDraw = false)
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
            if (blinkFrame < 13)
            {
                blinkFrame = 13;
            }
            if (++blinkFrameCounter > 3)
            {
                blinkFrameCounter = 0;
                blinkFrame++;
            }
            if (blinkFrame > 15)
            {
                blinkFrame = 13;
                PetState = 0;
            }
        }
        int blinkFrame, blinkFrameCounter;
        int backFrame, backFrameCounter;
        private void UpdateBackFrame()
        {
            if (++backFrameCounter > 4)
            {
                backFrameCounter = 0;
                backFrame++;
            }
            if (backFrame > 3)
            {
                backFrame = 0;
            }
        }
        private void Breakdown()
        {
            if (extraAI[0] == 0)
            {
                if (++backFrameCounter > 4)
                {
                    backFrameCounter = 0;
                    backFrame++;
                }
                if (backFrame > 7)
                {
                    backFrame = 4;
                    extraAI[1]++;
                }
                if (Main.rand.NextBool(8 - extraAI[1]))
                {
                    Dust.NewDustPerfect(Projectile.Center + new Vector2(-6 * Projectile.spriteDirection, -8)
                        , MyDustId.Smoke
                        , new Vector2(Main.rand.Next(-3, 3) * 0.75f, Main.rand.Next(-6, -3) * 0.75f)
                        , 100, default, Main.rand.NextFloat(1.5f, 2.25f)).noGravity = true;
                }
                if (extraAI[1] > 6)
                {
                    extraAI[1] = 4;
                    extraAI[1] = 0;
                    extraAI[0]++;
                }
            }
            else if (extraAI[0] == 1)
            {
                if (++backFrameCounter > 4)
                {
                    backFrameCounter = 0;
                    backFrame++;
                }
                if (backFrame > 7)
                {
                    backFrame = 4;
                }
                if (++Projectile.frameCounter > 6)
                {
                    Projectile.frameCounter = 0;
                    Projectile.frame++;
                }
                if (Projectile.frame > 7)
                {
                    Projectile.frame = 4;
                    extraAI[1]++;
                }
                if (Main.rand.NextBool(4 - extraAI[1]))
                {
                    Dust.NewDustPerfect(Projectile.Center + new Vector2(-6 * Projectile.spriteDirection, -8)
                        , MyDustId.Smoke
                        , new Vector2(Main.rand.Next(-3, 3) * 0.75f, Main.rand.Next(-6, -3) * 0.75f)
                        , 100, Color.Black, Main.rand.NextFloat(1.5f, 2.25f)).noGravity = true;
                    if (Main.rand.NextBool(2))
                    {
                        Gore.NewGoreDirect(Projectile.GetSource_FromAI()
                            , Projectile.Center + new Vector2(0, -24)
                            , new Vector2(Main.rand.Next(-3, 3) * 0.15f, Main.rand.Next(-3, -1) * 0.15f)
                            , Main.rand.Next(GoreID.Smoke1, GoreID.Smoke3 + 1), Main.rand.NextFloat(0.25f, 0.5f));
                    }
                }
                if (extraAI[1] > 2)
                {
                    extraAI[1] = 0;
                    extraAI[0]++;
                }
            }
            else if (extraAI[0] == 2)
            {
                backFrame = 8;
                if (extraAI[1] == 0)
                {
                    for (int i = 0; i < 25; i++)
                        Dust.NewDustPerfect(Projectile.Center
                            , MyDustId.Smoke
                            , new Vector2(Main.rand.Next(-6, 6) * 0.75f, Main.rand.Next(-6, 6) * 0.75f)
                            , 20, Color.Black, Main.rand.NextFloat(2.5f, 4.25f)).noGravity = true;
                    for (int i = 0; i < 10; i++)
                        Gore.NewGoreDirect(Projectile.GetSource_FromAI()
                            , Projectile.Center + new Vector2(Main.rand.Next(-8, 8), Main.rand.Next(-8, 8))
                            , new Vector2(Main.rand.Next(-3, 3) * 0.15f, Main.rand.Next(-3, 3) * 0.15f)
                            , Main.rand.Next(GoreID.Smoke1, GoreID.Smoke3 + 1), Main.rand.NextFloat(0.75f, 1.25f));
                    AltVanillaFunction.PlaySound(SoundID.Item14, Projectile.position);
                }
                else if (Main.rand.NextBool(2))
                {
                    Dust.NewDustPerfect(Projectile.Center + new Vector2(Main.rand.Next(-14, 14), Main.rand.Next(-8, 8))
                            , MyDustId.Smoke
                            , new Vector2(Main.rand.Next(-1, 1) * 0.75f, Main.rand.Next(-4, -2) * 0.75f)
                            , 90, Color.Black, Main.rand.NextFloat(1f, 2f)).noGravity = true;
                }
                extraAI[1]++;
                if (extraAI[1] > 120 && extraAI[1] <= 144)
                    Projectile.frame = extraAI[1] % 6 == 0 ? 8 : 9;
                else
                    Projectile.frame = 8;
                if (extraAI[1] > 240)
                {
                    extraAI[1] = 0;
                    extraAI[0]++;
                }
            }
            else if (extraAI[0] == 3)
            {
                if (Main.rand.NextBool(3))
                {
                    for (int i = 0; i < 4; i++)
                        Dust.NewDustPerfect(Projectile.Center + new Vector2(Main.rand.Next(-14, 14), Main.rand.Next(-8, 8))
                            , MyDustId.Smoke
                            , new Vector2(Main.rand.Next(-6, 6) * 0.85f, Main.rand.Next(-6, 6) * 0.85f)
                            , 90, Color.Black, Main.rand.NextFloat(1f, 2f)).noGravity = true;
                }
                int maxTime = 6;
                backFrame = 9;
                extraAI[1]++;
                if (extraAI[1] <= maxTime * 6)
                    Projectile.frame = extraAI[1] % maxTime == 0 ? 10 : 11;
                else
                    Projectile.frame = 12;
                if (extraAI[1] > maxTime * 6 + 6)
                {
                    extraAI[1] = 0;
                    extraAI[0] = 2400;
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
        Color myColor = new Color(74, 165, 255);
        public override string GetChatText(out string[] text)
        {
            text = new string[21];
            text[1] = ModUtils.GetChatText("Nitori", "1");
            text[2] = ModUtils.GetChatText("Nitori", "2");
            text[3] = ModUtils.GetChatText("Nitori", "3");
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
            if (mainTimer % 720 == 0 && Main.rand.NextBool(9) && PetState != 2)
            {
                SetChat(myColor);
            }
        }
        public override void VisualEffectForPreview()
        {
            if (PetState != 2)
            {
                Idel();
                UpdateBackFrame();
            }
            else if (extraAI[0] == 0)
                Idel();
        }
        public override void AI()
        {
            Player player = Main.player[Projectile.owner];
            Projectile.SetPetActive(player, BuffType<NitoriBuff>());

            UpdateTalking();
            Vector2 point = new Vector2(-60 * player.direction, -40 + player.gfxOffY);
            Projectile.tileCollide = false;
            Projectile.rotation = Projectile.velocity.X * 0.012f;
            if (PetState == 2 && extraAI[0] == 2)
            {
                Projectile.rotation = 0;
            }

            ChangeDir(player, true);
            MoveToPoint(point, 12.5f);

            if (mainTimer % 270 == 0)
            {
                if (PetState == 0)
                    PetState = 1;
            }
            if (mainTimer >= 1200 && mainTimer < 3600 && PetState < 1)
            {
                if (mainTimer % 900 == 0 && Main.rand.NextBool(4) && extraAI[0] <= 0)
                {
                    PetState = 2;
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
                Breakdown();
            }
        }
    }
}


