using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ID;
using Terraria.Utilities;
using TouhouPets.Content.Buffs.PetBuffs;

namespace TouhouPets.Content.Projectiles.Pets
{
    public class Sunny : BasicTouhouPet
    {
        public override void SetStaticDefaults()
        {
            Main.projFrames[Type] = 13;
            Main.projPet[Type] = true;
            ProjectileID.Sets.LightPet[Type] = true;
        }
        public override bool PreDraw(ref Color lightColor)
        {
            DrawSunny_Full(lightColor);

            if (phantomDist >= 0)
            {
                for (int i = -1; i <= 1; i++)
                {
                    if (i != 0)
                    {
                        Vector2 dist = Main.player[Projectile.owner].Center - Projectile.Center;
                        Vector2 drift = new Vector2(dist.X * i * 2, dist.Y * 2).RotatedBy(Main.GlobalTimeWrappedHourly);
                        Color clr = lightColor * 0.4f * phantomDist;
                        DrawSunny_Full(clr, drift);
                        DrawSunny_Full(clr, -drift);
                    }
                }
            }

            return false;
        }
        private void DrawSunny_Full(Color lightColor, Vector2 extraPos = default)
        {
            if (extraPos == default)
            {
                extraPos = Vector2.Zero;
            }
            DrawSunny(wingsFrame, lightColor * 0.7f, 1, new Vector2(extraX, extraY) + extraPos);
            DrawSunny(hairFrame, lightColor, 1, new Vector2(extraX, extraY) + extraPos);
            DrawSunny(Projectile.frame, lightColor, 0, extraPos);
            if (PetState == 1 || PetState == 4)
                DrawSunny(blinkFrame, lightColor, 0, new Vector2(extraX, extraY) + extraPos);
            DrawSunny(Projectile.frame, lightColor, 0, extraPos, AltVanillaFunction.GetExtraTexture("Sunny_Cloth"), true);
            DrawSunny(clothFrame, lightColor, 1, new Vector2(extraX, extraY) + extraPos, null, true);

            Projectile.DrawStateNormalizeForPet();
            if (Projectile.frame == 4)
            {
                DrawSunny(5, lightColor, 0, extraPos);
                DrawSunny(5, lightColor, 0, extraPos, AltVanillaFunction.GetExtraTexture("Sunny_Cloth"), true);
            }
        }
        private void DrawSunny(int frame, Color lightColor, int columns = 0, Vector2 extraPos = default, Texture2D tex = null, bool entitySpriteDraw = false)
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
        private void Blink(bool alt = false)
        {
            int startFrame = alt ? 11 : 10;
            if (blinkFrame < startFrame)
            {
                blinkFrame = startFrame;
            }
            if (++blinkFrameCounter > 3)
            {
                blinkFrameCounter = 0;
                blinkFrame++;
            }
            if (blinkFrame > 12)
            {
                blinkFrame = startFrame;
                PetState = alt ? 3 : 0;
            }
        }
        int blinkFrame, blinkFrameCounter;
        int wingsFrame, wingsFrameCounter;
        int clothFrame, clothFrameCounter;
        int hairFrame, hairFrameCounter;
        float extraX, extraY, phantomDist;
        private void Happy()
        {
            if (++Projectile.frameCounter > 8)
            {
                Projectile.frameCounter = 0;
                Projectile.frame++;
            }
            if (extraAI[0] == 0)
            {
                if (Projectile.frame > 3)
                {
                    Projectile.frame = 2;
                    extraAI[1]++;
                }

                if (Projectile.owner == Main.myPlayer)
                {
                    if (extraAI[1] > Main.rand.Next(6, 12))
                    {
                        extraAI[1] = 0;
                        extraAI[0] = 1;
                        Projectile.netUpdate = true;
                    }
                }
            }
            else
            {
                if (Projectile.frame > 3)
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
            if (Projectile.frame >= 1 && Projectile.frame <= 3)
            {
                extraY = -2;
            }
            if (Projectile.frame == 4)
            {
                extraY = 2;
            }
            if (PetState == 2)
            {
                phantomDist += 0.1f;
            }
            else
            {
                phantomDist -= 0.1f;
            }
            phantomDist = MathHelper.Clamp(phantomDist, 0, 1);
        }
        private void UpdateMiscFrame()
        {
            if (wingsFrame < 9)
            {
                wingsFrame = 9;
            }
            if (++wingsFrameCounter > 3)
            {
                wingsFrameCounter = 0;
                wingsFrame++;
            }
            if (wingsFrame > 12)
            {
                wingsFrame = 9;
            }

            if (PetState >= 3)
            {
                hairFrame = 8;
                clothFrame = 0;
            }
            else
            {
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
                if (++clothFrameCounter > 6)
                {
                    clothFrameCounter = 0;
                    clothFrame++;
                }
                if (clothFrame > 3)
                {
                    clothFrame = 0;
                }
            }
        }
        Color myColor = new Color(240, 196, 48);
        public override string GetChatText(out string[] text)
        {
            text = new string[21];
            if (RainWet)
            {
                text[6] = ModUtils.GetChatText("Sunny", "6");
                text[7] = ModUtils.GetChatText("Sunny", "7");
                text[8] = ModUtils.GetChatText("Sunny", "8");
            }
            else
            {
                text[1] = ModUtils.GetChatText("Sunny", "1");
                text[2] = ModUtils.GetChatText("Sunny", "2");
                text[3] = ModUtils.GetChatText("Sunny", "3");
                if (UnderSunShine)
                {
                    text[4] = ModUtils.GetChatText("Sunny", "4");
                    text[5] = ModUtils.GetChatText("Sunny", "5");
                    text[9] = ModUtils.GetChatText("Sunny", "9");
                }
                text[12] = ModUtils.GetChatText("Sunny", "12");
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
            int type = ProjectileType<Luna>();
            int type2 = ProjectileType<Star>();
            if (FindChatIndex(out Projectile _, type, 9, default, 0)
                || FindChatIndex(out Projectile _, type2, 7, default, 0))
            {
                ChatCD = 1;
            }
            if (FindChatIndex(out Projectile p1, type, 9, default, 1, true))
            {
                SetChatWithOtherOne(p1, ModUtils.GetChatText("Sunny", "13"), myColor, 13);
            }
            else if (FindChatIndex(out Projectile p2, type, 10, default, 1, true))
            {
                SetChatWithOtherOne(p2, ModUtils.GetChatText("Sunny", "14"), myColor, 14);
                p2.localAI[2] = 0;
                p2.localAI[1] = 4800;//给一个超长CD防止开小差
            }
            else if (FindChatIndex(out Projectile p3, type2, 7, default, 1, true))
            {
                SetChatWithOtherOne(p3, ModUtils.GetChatText("Sunny", "15"), myColor, 0);
            }
            else if (FindChainedChat(9))
            {
                SetChatWithOtherOne(null, ModUtils.GetChatText("Sunny", "10"), myColor, 10);
            }
            else if (FindChainedChat(10))
            {
                SetChatWithOtherOne(null, ModUtils.GetChatText("Sunny", "11"), myColor, 11);
            }
            else if (mainTimer % 640 == 0 && Main.rand.NextBool(6) && PetState != 2)
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
        private bool RainWet => Main.raining &&
            (Main.player[Projectile.owner].ZoneOverworldHeight || Main.player[Projectile.owner].ZoneSkyHeight);

        private bool UnderSunShine => Main.cloudAlpha <= 0 && Main.dayTime &&
            (Main.player[Projectile.owner].ZoneOverworldHeight || Main.player[Projectile.owner].ZoneSkyHeight);
        private void GenDust()
        {
            if (RainWet)
            {
                if (Main.rand.NextBool(6) && !Main.player[Projectile.owner].behindBackWall)
                {
                    Dust.NewDustPerfect(Projectile.position + new Vector2(Main.rand.Next(10, Projectile.width - 10), Main.rand.Next(10, Projectile.height - 10)),
                            MyDustId.BlueThin, new Vector2(0, 0.1f), 100, Color.White).scale = Main.rand.NextFloat(0.5f, 1.2f);
                }
                return;
            }

            int dustID = MyDustId.YellowGoldenFire;
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
        }
        private void ControlMovement(Player player)
        {
            Projectile.tileCollide = false;
            Projectile.rotation = Projectile.velocity.X * 0.02f;

            ChangeDir(player, true, 200);

            Vector2 point = new Vector2(60 * player.direction, -40 + player.gfxOffY);
            if (player.HasBuff<TheThreeFairiesBuff>())
            {
                point = new Vector2(60 * player.direction, -70 + player.gfxOffY);
                point += new Vector2(0, -40).RotatedBy(MathHelper.ToRadians(360 / 3 * 0) + Main.GlobalTimeWrappedHourly);
            }
            MoveToPoint(point, 8.5f);
        }
        public override void AI()
        {
            Lighting.AddLight(Projectile.Center, 2.40f, 1.96f, 0.84f);
            Player player = Main.player[Projectile.owner];
            Projectile.SetPetActive(player, BuffType<SunnyBuff>());
            Projectile.SetPetActive(player, BuffType<TheThreeFairiesBuff>());

            UpdateTalking();
            ControlMovement(player);
            GenDust();

            if (Projectile.owner == Main.myPlayer)
            {
                if (mainTimer % 270 == 0)
                {
                    if (PetState == 3)
                    {
                        PetState = 4;
                    }
                    else if (PetState == 0)
                    {
                        PetState = 1;
                    }
                    Projectile.netUpdate = true;
                }
                if (RainWet && PetState < 3)
                {
                    PetState = 3;
                    Projectile.netUpdate = true;
                }
                else if (mainTimer >= 600 && mainTimer < 3600 && PetState == 0)
                {
                    if (mainTimer % 600 == 0 && Main.rand.NextBool(6) && extraAI[0] <= 0 && UnderSunShine && ChatTimeLeft <= 0)
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
            else if (PetState == 3 || PetState == 4)
            {
                Projectile.frame = 4;
                if (PetState == 4)
                    Blink(true);
                if (!RainWet)
                {
                    PetState = 0;
                }
            }
            UpdateExtraPos();
        }
    }
}


