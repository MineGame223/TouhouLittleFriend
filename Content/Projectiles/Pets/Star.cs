using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria;
using Terraria.GameContent.Drawing;
using Terraria.ID;
using Terraria.Utilities;
using TouhouPets.Content.Buffs.PetBuffs;

namespace TouhouPets.Content.Projectiles.Pets
{
    public class Star : BasicTouhouPet
    {
        public override void SetStaticDefaults()
        {
            Main.projFrames[Type] = 12;
            Main.projPet[Type] = true;
            ProjectileID.Sets.LightPet[Type] = true;
        }
        public override bool PreDraw(ref Color lightColor)
        {
            DrawStar(wingsFrame, lightColor * 0.7f, 1, new Vector2(extraX, extraY));
            DrawStar(hairFrame, lightColor, 1, new Vector2(extraX, extraY));
            DrawStar(Projectile.frame, lightColor);
            if (PetState == 1)
                DrawStar(blinkFrame, lightColor);
            DrawStar(Projectile.frame, lightColor, 0, default, AltVanillaFunction.GetExtraTexture("Star_Cloth"), true);
            DrawStar(clothFrame, lightColor, 1, new Vector2(extraX, extraY));

            /*for (int i = 1; i <= 10; i++)
            {
                Color clr = Color.AliceBlue;
                clr.R *= (byte)i;
                clr.G *= (byte)i;
                clr.B *= (byte)i;
                int step = 20 * i;
                float rot_speed = 1.5f - i / 6;
                DrawStarTrack(clr, 40, 30 * i, 45 + step, rot_speed);
                DrawStarTrack(clr, 20, 30 * i, 120 + step, rot_speed);
                DrawStarTrack(clr, 40, 30 * i, 180 + step, rot_speed);
                DrawStarTrack(clr, 80, 30 * i, 270 + step, rot_speed);
            }*/
            return false;
        }
        private void DrawStar(int frame, Color lightColor, int columns = 0, Vector2 extraPos = default, Texture2D tex = null, bool entitySpriteDraw = false)
        {
            if (extraPos == default)
            {
                extraPos = Vector2.Zero;
            }
            Texture2D t = tex ?? AltVanillaFunction.ProjectileTexture(Type);
            int height = t.Height / Main.projFrames[Type];
            Vector2 pos = Projectile.Center - Main.screenPosition + new Vector2(0, 7f * Main.essScale) + extraPos;
            Rectangle rect = new Rectangle(t.Width / 2 * columns, frame * height, t.Width / 2, height);
            Vector2 orig = rect.Size() / 2;
            SpriteEffects effect = Projectile.spriteDirection == -1 ? SpriteEffects.None : SpriteEffects.FlipHorizontally;
            if (entitySpriteDraw)
                Main.EntitySpriteDraw(t, pos, rect, Projectile.GetAlpha(lightColor), Projectile.rotation, orig, Projectile.scale, effect, 0f);
            else
                Main.spriteBatch.TeaNPCDraw(t, pos, rect, Projectile.GetAlpha(lightColor), Projectile.rotation, orig, Projectile.scale, effect, 0f);
        }
        private void DrawStarTrack(Color color = default, int arcLength = 360, float distanceFromCenter = 100, float rotationAdjust = 0, float rotationSpeedRate = 1)
        {
            if (color == default)
            {
                color = Color.White;
            }
            int rate = 1;
            for (int i = 0; i < arcLength; i++)
            {
                Texture2D line = AltVanillaFunction.ExtraTexture(ExtrasID.StardustTowerMark);
                Vector2 pos = Projectile.Center + new Vector2(0, -distanceFromCenter).RotatedBy(MathHelper.ToRadians(rate * i) + Main.GlobalTimeWrappedHourly * rotationSpeedRate + MathHelper.ToRadians(rotationAdjust));
                int singleLineLength = (int)(distanceFromCenter * 2 * MathHelper.Pi * 1.1f) / (360 / rate);
                Rectangle rect = new Rectangle(0, 0, line.Width, singleLineLength);
                Vector2 orig = new Vector2(line.Width / 2, 0);
                float rot = pos.DirectionTo(Projectile.Center).ToRotation();
                Main.spriteBatch.TeaNPCDraw(line, pos - Main.screenPosition, rect, color, rot, orig, 1f, SpriteEffects.None, 0);
            }
        }
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
        int wingsFrame, wingsFrameCounter;
        int clothFrame, clothFrameCounter;
        int hairFrame, hairFrameCounter;
        float extraX, extraY;
        private void StarMagic()
        {
            if (++Projectile.frameCounter > 6)
            {
                Projectile.frameCounter = 0;
                Projectile.frame++;
            }
            if (extraAI[0] == 0)
            {
                if (Projectile.frame > 4)
                {
                    Projectile.frame = 3;
                    extraAI[1]++;
                }
                if (Main.rand.NextBool(2))
                {
                    ParticleOrchestraSettings settings = new ParticleOrchestraSettings
                    {
                        PositionInWorld = Projectile.Center + new Vector2(Main.rand.Next(-120, 120), Main.rand.Next(-160, -120)),
                        MovementVector = new Vector2(0, Main.rand.Next(1, 3)),
                    };
                    ParticleOrchestrator.SpawnParticlesDirect(ParticleOrchestraType.ShimmerBlock, settings);
                }
                if (Projectile.owner == Main.myPlayer)
                {
                    if (extraAI[1] > Main.rand.Next(40, 80))
                    {
                        extraAI[1] = 0;
                        extraAI[0] = 1;
                        Projectile.netUpdate = true;
                    }
                }
            }
            else
            {
                if (Projectile.frame > 5)
                {
                    Projectile.frame = 0;
                    extraAI[0] = 3600;
                    PetState = 0;
                }
            }
        }
        private void UpdateExtraPos()
        {
            extraX = 0;
            extraY = 0;
            if (Projectile.frame >= 1 && Projectile.frame <= 5)
            {
                extraY = -2;
                if (Projectile.frame >= 2 && Projectile.frame <= 4)
                {
                    extraX = -2 * Projectile.spriteDirection;
                }
            }
        }
        private void UpdateMiscFrame()
        {
            if (wingsFrame < 8)
            {
                wingsFrame = 8;
            }
            if (++wingsFrameCounter > 5)
            {
                wingsFrameCounter = 0;
                wingsFrame++;
            }
            if (wingsFrame > 11)
            {
                wingsFrame = 8;
            }

            if (hairFrame < 4)
            {
                hairFrame = 4;
            }
            if (++hairFrameCounter > 7)
            {
                hairFrameCounter = 0;
                hairFrame++;
            }
            if (hairFrame > 7)
            {
                hairFrame = 4;
            }

            if (++clothFrameCounter > 8)
            {
                clothFrameCounter = 0;
                clothFrame++;
            }
            if (clothFrame > 3)
            {
                clothFrame = 0;
            }
        }
        Color myColor = new Color(135, 143, 237);
        public override string GetChatText(out string[] text)
        {
            Player player = Main.player[Projectile.owner];
            text = new string[21];
            text[1] = ModUtils.GetChatText("Star", "1");
            text[2] = ModUtils.GetChatText("Star", "2");
            text[3] = ModUtils.GetChatText("Star", "3");
            if (Main.dayTime)
                text[4] = ModUtils.GetChatText("Star", "4");
            else if (player.ZoneForest && Main.cloudAlpha == 0 && !Main.bloodMoon)
                text[5] = ModUtils.GetChatText("Star", "5");

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
            if (PetState < 2)
            {
                Projectile.frame = 0;
            }
        }
        private void GenDust()
        {
            int dustID = MyDustId.BlueMagic;
            if (Main.rand.NextBool(10))
            {
                Dust.NewDustPerfect(Projectile.Center
                + new Vector2(Main.rand.NextFloat(-30f, -10f) * Projectile.spriteDirection, Main.rand.NextFloat(-5f, 25f)), dustID
                , new Vector2(Main.rand.NextFloat(-0.2f, 0.2f), Main.rand.NextFloat(0.8f, 1.2f)), 100, default
                , Main.rand.NextFloat(1f, 2f)).noGravity = true;
            }
            if (Main.rand.NextBool(10))
            {
                Dust.NewDustPerfect(Projectile.Center
                    + new Vector2(Main.rand.NextFloat(20f, 22f) * Projectile.spriteDirection, Main.rand.NextFloat(-5f, 25f)), dustID
                    , new Vector2(Main.rand.NextFloat(-0.2f, 0.2f), Main.rand.NextFloat(0.8f, 1.2f)), 100, default
                    , Main.rand.NextFloat(1f, 2f)).noGravity = true;
            }
            if (Projectile.velocity.Length() > 4 && Main.rand.NextBool(6))
            {
                Gore.NewGoreDirect(Projectile.GetSource_FromAI(), Projectile.position + new Vector2(Main.rand.Next(0, Projectile.width), Main.rand.Next(0, Projectile.height)), Vector2.Normalize(Projectile.velocity) * -2, Main.rand.Next(16, 18), Main.rand.NextFloat(0.9f, 1.1f));
            }
        }
        public override void AI()
        {
            Lighting.AddLight(Projectile.Center, 1.35f, 1.43f, 2.37f);
            Player player = Main.player[Projectile.owner];
            Projectile.SetPetActive(player, BuffType<StarBuff>());

            UpdateTalking();
            Vector2 point = new Vector2(50 * player.direction, -30 + player.gfxOffY);
            Projectile.tileCollide = false;
            Projectile.rotation = Projectile.velocity.X * 0.02f;

            ChangeDir(player);
            MoveToPoint(point, 7.5f);

            GenDust();

            if (Projectile.owner == Main.myPlayer)
            {
                if (mainTimer % 270 == 0 && PetState != 2)
                {
                    PetState = 1;
                    Projectile.netUpdate = true;
                }
                if (mainTimer >= 600 && mainTimer < 3600 && PetState == 0)
                {
                    if (mainTimer % 600 == 0 && Main.rand.NextBool(7) && extraAI[0] <= 0 && !Main.dayTime)
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
                StarMagic();
            }
            UpdateExtraPos();
        }
    }
}


