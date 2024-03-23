using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.Enums;
using Terraria.ID;
using Terraria.Utilities;
using TouhouPets.Content.Buffs.PetBuffs;

namespace TouhouPets.Content.Projectiles.Pets
{
    public class Luna : BasicTouhouPet
    {
        public override void SetStaticDefaults()
        {
            Main.projFrames[Type] = 13;
            Main.projPet[Type] = true;
            ProjectileID.Sets.LightPet[Type] = true;
        }
        public override bool PreDraw(ref Color lightColor)
        {
            DrawLuna(wingsFrame, lightColor * 0.7f);

            DrawLuna(12, lightColor);
            DrawLuna(12, lightColor, 0, AltVanillaFunction.GetExtraTexture("Luna_Cloth"), true);
            Projectile.DrawStateNormalizeForPet();

            if (PetState > 0)
                DrawLuna(blinkFrame, lightColor, 1);

            DrawLuna(clothFrame, lightColor, 1, null, true);
            Projectile.DrawStateNormalizeForPet();

            DrawLuna(Projectile.frame, lightColor);
            DrawLuna(Projectile.frame, lightColor, 0, AltVanillaFunction.GetExtraTexture("Luna_Cloth"), true);
            return false;
        }
        private void DrawLuna(int frame, Color lightColor, int columns = 0, Texture2D tex = null, bool entitySpriteDraw = false)
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
        private void Blink(bool alt = false)
        {
            int startFrame = alt ? 5 : 4;
            if (blinkFrame < startFrame)
            {
                blinkFrame = startFrame;
            }
            int count = PetState == 4 ? 9 : 3;
            if (++blinkFrameCounter > count)
            {
                blinkFrameCounter = 0;
                blinkFrame++;
            }
            if (PetState == 4)
            {
                if (blinkFrame > 8)
                {
                    if (Projectile.frame <= 6 && Projectile.frame > 0)
                        blinkFrame = 7;
                    else
                    {
                        if (extraAI[0] == 1)
                        {
                            if (extraAI[1] < 120)
                            {
                                blinkFrame = 10;
                                extraAI[1]++;
                                return;
                            }
                            if (blinkFrame > 10)
                            {
                                blinkFrame = 9;
                                extraAI[1]++;
                            }
                            if (extraAI[1] > 123)
                            {
                                if (Projectile.owner == Main.myPlayer)
                                {
                                    blinkFrame = startFrame;
                                    extraAI[1] = 0;
                                    extraAI[0]++;
                                    Projectile.netUpdate = true;
                                }
                            }
                        }
                    }
                }
            }
            else
            {
                if (blinkFrame > 6)
                {
                    blinkFrame = startFrame;
                    PetState = alt ? 2 : 0;
                }
            }
        }
        int blinkFrame, blinkFrameCounter;
        int wingsFrame, wingsFrameCounter;
        int clothFrame, clothFrameCounter;
        private void ReadingPaper()
        {
            if (++Projectile.frameCounter > 6)
            {
                Projectile.frameCounter = 0;
                Projectile.frame++;
            }
            if (extraAI[0] == 0)
            {
                if (Projectile.frame > 3)
                {
                    Projectile.frame = 3;
                }
                extraAI[1]++;
                if (Projectile.owner == Main.myPlayer)
                {
                    if (extraAI[1] > Main.rand.Next(3200, 7200) || Projectile.velocity.Length() > 4.5f)
                    {
                        extraAI[1] = 0;
                        extraAI[0]++;
                        Projectile.netUpdate = true;
                    }
                }
            }
            else
            {
                if (Projectile.frame > 4)
                {
                    Projectile.frame = 0;
                    extraAI[0] = 1200;
                    PetState = 0;
                }
            }
        }
        private void Yawn()
        {
            if (Projectile.frame < 5)
            {
                Projectile.frame = 5;
            }
            int count = 8;
            if (Projectile.frame == 6)
            {
                count = 180;
            }
            if (extraAI[0] == 0)
            {
                if (++Projectile.frameCounter > count)
                {
                    Projectile.frameCounter = 0;
                    Projectile.frame++;
                }
                if (Projectile.frame > 7)
                {
                    Projectile.frame = 0;
                    if (Projectile.owner == Main.myPlayer)
                    {
                        extraAI[0]++;
                        Projectile.netUpdate = true;
                    }
                }
            }
            else if (extraAI[0] == 1)
            {
                Projectile.frame = 0;
            }
            else
            {
                extraAI[0] = 1200;
                PetState = 0;
            }
        }
        private void UpdateMiscFrame()
        {
            if (wingsFrame < 8)
            {
                wingsFrame = 8;
            }
            if (++wingsFrameCounter > 6)
            {
                wingsFrameCounter = 0;
                wingsFrame++;
            }
            if (wingsFrame > 11)
            {
                wingsFrame = 8;
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
        Color myColor = new Color(255, 225, 110);
        public override string GetChatText(out string[] text)
        {
            text = new string[21];
            if (PetState <= 1)
            {
                text[1] = ModUtils.GetChatText("Luna", "1");
                text[2] = ModUtils.GetChatText("Luna", "2");
                text[3] = ModUtils.GetChatText("Luna", "3");
            }
            else
            {
                text[6] = ModUtils.GetChatText("Luna", "6");
                text[7] = ModUtils.GetChatText("Luna", "7");
            }
            if (Main.GetMoonPhase() == MoonPhase.Full && !Main.dayTime)
            {
                text[8] = ModUtils.GetChatText("Luna", "8");
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
            if (PetState == 4)
            {
                return;
            }
            int type = ProjectileType<Sunny>();
            int type2 = ProjectileType<Star>();
            if (FindChatIndex(out Projectile _, type2, 6, 7, 0))
            {
                ChatCD = 1;
            }
            if (FindChatIndex(out Projectile p1, type2, 6, default, 1, true))
            {
                SetChatWithOtherOne(p1, ModUtils.GetChatText("Luna", "9"), myColor, 9);
                p1.localAI[2] = 0;
                p1.localAI[1] = 4800;
            }
            else if (FindChatIndex(out Projectile p2, type, 13, default, 1, true))
            {
                SetChatWithOtherOne(p2, ModUtils.GetChatText("Luna", "10"), myColor, 10);
            }
            else if (FindChatIndex(out Projectile p3, type2, 7, default, 1, true))
            {
                SetChatWithOtherOne(p3, ModUtils.GetChatText("Luna", "11"), myColor, 0);
                p3.localAI[2] = 0;
            }
            else if (mainTimer % 960 == 0 && Main.rand.NextBool(8))
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
            int dustID = MyDustId.WhiteTransparent;
            if (Main.rand.NextBool(10))
            {
                Dust.NewDustPerfect(Projectile.Center
                + new Vector2(Main.rand.NextFloat(-30f, -10f) * Projectile.spriteDirection, Main.rand.NextFloat(-5f, 25f)), dustID
                , new Vector2(Main.rand.NextFloat(-0.2f, 0.2f), Main.rand.NextFloat(0.8f, 1.2f)), 100, Color.Yellow
                , Main.rand.NextFloat(0.5f, 0.9f)).noGravity = true;
            }
            if (Main.rand.NextBool(10))
            {
                Dust.NewDustPerfect(Projectile.Center
                    + new Vector2(Main.rand.NextFloat(20f, 22f) * Projectile.spriteDirection, Main.rand.NextFloat(-5f, 25f)), dustID
                    , new Vector2(Main.rand.NextFloat(-0.2f, 0.2f), Main.rand.NextFloat(0.8f, 1.2f)), 100, Color.LightGoldenrodYellow
                    , Main.rand.NextFloat(0.5f, 0.9f)).noGravity = true;
            }
        }
        private void ControlMovement(Player player)
        {
            Projectile.tileCollide = false;
            Projectile.rotation = Projectile.velocity.X * 0.02f;

            ChangeDir(player, true, 200);

            Vector2 point = new Vector2(50 * player.direction, -40 + player.gfxOffY);
            if (player.HasBuff<TheThreeFairiesBuff>())
            {
                point = new Vector2(60 * player.direction, -70 + player.gfxOffY);
                point += new Vector2(0, -40).RotatedBy(MathHelper.ToRadians(360 / 3 * 2) + Main.GlobalTimeWrappedHourly);
            }
            MoveToPoint(point, 7.5f);
        }
        public override void AI()
        {
            Lighting.AddLight(Projectile.Center, 1.52f, 1.50f, 1.15f);
            Player player = Main.player[Projectile.owner];
            Projectile.SetPetActive(player, BuffType<LunaBuff>());
            Projectile.SetPetActive(player, BuffType<TheThreeFairiesBuff>());

            UpdateTalking();
            ControlMovement(player);
            GenDust();

            if (Projectile.owner == Main.myPlayer)
            {
                if (mainTimer % 270 == 0)
                {
                    if (PetState == 2)
                    {
                        PetState = 3;
                    }
                    else if (PetState == 0)
                    {
                        PetState = 1;
                    }
                    Projectile.netUpdate = true;
                }
                if (mainTimer >= 600 && mainTimer < 3600 && PetState == 0)
                {
                    if (Main.dayTime)
                    {
                        if (mainTimer % 600 == 0 && Main.rand.NextBool(6) && extraAI[0] <= 0 && ChatTimeLeft <= 0)
                        {
                            PetState = 4;
                            int chance = Main.rand.Next(2);
                            switch (chance)
                            {
                                case 1:
                                    SetChat(myColor, ModUtils.GetChatText("Luna", "4"), 4, 60, 30, true);
                                    break;
                                default:
                                    SetChat(myColor, ModUtils.GetChatText("Luna", "5"), 5, 60, 30, true);
                                    break;
                            }
                            Projectile.netUpdate = true;
                        }
                    }
                    else if (Projectile.velocity.Length() < 2f)
                    {
                        if (mainTimer % 120 == 0 && Main.rand.NextBool(12) && extraAI[0] <= 0)
                        {
                            if (blinkFrame < 5)
                                blinkFrame = 5;
                            PetState = 2;
                            Projectile.netUpdate = true;
                        }
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
            else if (PetState == 2 || PetState == 3)
            {
                ReadingPaper();
                if (PetState == 3)
                    Blink(true);
            }
            else if (PetState == 4)
            {
                Yawn();
                Blink();
            }
        }
    }
}


