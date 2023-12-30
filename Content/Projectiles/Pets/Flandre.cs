using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ID;
using Terraria.Utilities;
using TouhouPets.Content.Buffs.PetBuffs;

namespace TouhouPets.Content.Projectiles.Pets
{
    public class Flandre : BasicTouhouPet
    {
        public override void SetStaticDefaults()
        {
            Main.projFrames[Type] = 23;
            Main.projPet[Type] = true;
            ProjectileID.Sets.LightPet[Type] = true;
        }
        public override bool PreDraw(ref Color lightColor)
        {
            DrawWing(lightColor);
            Projectile.DrawStateNormalizeForPet();
            DrawFlandre(Projectile.frame, lightColor);
            if (PetState == 1)
                DrawFlandre(blinkFrame, lightColor);
            DrawFlandre(Projectile.frame, lightColor, default, AltVanillaFunction.GetExtraTexture("Flandre_Cloth"), true);
            if (!Remilia.HateSunlight(Projectile))
            {
                DrawFlandre(clothFrame, lightColor, new Vector2(extraAdjX, extraAdjY), null, true);
            }
            return false;
        }
        private void DrawFlandre(int frame, Color lightColor, Vector2 extraPos = default, Texture2D tex = null, bool entitySpriteDraw = false)
        {
            if (extraPos == default)
                extraPos = Vector2.Zero;
            Texture2D t = tex ?? AltVanillaFunction.ProjectileTexture(Type);
            int height = t.Height / Main.projFrames[Type];
            Vector2 pos = Projectile.Center - Main.screenPosition + new Vector2(0, 7f * Main.essScale) + extraPos;
            Rectangle rect = new Rectangle(0, frame * height, t.Width, height);
            Vector2 orig = rect.Size() / 2;
            SpriteEffects effect = Projectile.spriteDirection == -1 ? SpriteEffects.None : SpriteEffects.FlipHorizontally;
            if (entitySpriteDraw)
                Main.EntitySpriteDraw(t, pos, rect, Projectile.GetAlpha(lightColor), Projectile.rotation, orig, Projectile.scale, effect, 0f);
            else
                Main.spriteBatch.TeaNPCDraw(t, pos, rect, Projectile.GetAlpha(lightColor), Projectile.rotation, orig, Projectile.scale, effect, 0f);
        }
        private void DrawWing(Color lightColor)
        {
            Texture2D t = AltVanillaFunction.ProjectileTexture(Type);
            Texture2D t2 = AltVanillaFunction.GetGlowTexture("FlandreGlow");
            int height = t.Height / Main.projFrames[Type];
            Vector2 pos = Projectile.Center - Main.screenPosition + new Vector2(0, 7 * Main.essScale);
            Rectangle rect2 = new Rectangle(0, wingFrame * height, t.Width, height);
            Vector2 orig = rect2.Size() / 2;
            SpriteEffects effect = Projectile.spriteDirection == -1 ? SpriteEffects.None : SpriteEffects.FlipHorizontally;

            Main.spriteBatch.TeaNPCDraw(t, pos + new Vector2(extraAdjX, extraAdjY), rect2, Projectile.GetAlpha(lightColor), Projectile.rotation, orig, Projectile.scale, effect, 0f);

            Main.spriteBatch.QuickToggleAdditiveMode(true, Projectile.isAPreviewDummy);
            for (int i = 0; i < 8; i++)
            {
                Vector2 spinningpoint = new Vector2(0f, -2f);
                Main.spriteBatch.TeaNPCDraw(t2, pos + new Vector2(extraAdjX, extraAdjY) + spinningpoint.RotatedBy(MathHelper.TwoPi * Main.GlobalTimeWrappedHourly + MathHelper.TwoPi / 8 * i * 0.6f), rect2, Projectile.GetAlpha(Color.White) * 0.4f, Projectile.rotation, orig, Projectile.scale * 0.95f, effect, 0f);
            }
            Main.spriteBatch.QuickToggleAdditiveMode(false, Projectile.isAPreviewDummy);
            Main.spriteBatch.TeaNPCDraw(t2, pos + new Vector2(extraAdjX, extraAdjY), rect2, Projectile.GetAlpha(Color.White) * 0.6f, Projectile.rotation, orig, Projectile.scale, effect, 0f);
        }
        private void Blink()
        {
            if (blinkFrame < 20)
            {
                blinkFrame = 20;
            }
            if (++blinkFrameCounter > 3)
            {
                blinkFrameCounter = 0;
                blinkFrame++;
            }
            if (blinkFrame > 22)
            {
                blinkFrame = 20;
                PetState = 0;
            }
        }
        int wingFrame, wingFrameCounter;
        int blinkFrame, blinkFrameCounter;
        int clothFrame, clothFrameCounter;
        int extraAdjX, extraAdjY;
        private void Eatting()
        {
            if (Projectile.frame < 1)
            {
                Projectile.frame = 1;
            }
            if (++Projectile.frameCounter > 10)
            {
                Projectile.frameCounter = 0;
                Projectile.frame++;
            }
            if (extraAI[0] == 0)
            {
                if (Projectile.frame >= 3)
                {
                    Projectile.frame = 3;
                    extraAI[1]++;
                    if (Projectile.owner == Main.myPlayer)
                    {
                        if (Main.rand.NextBool(3) && extraAI[2] == 0)
                        {
                            int chance = Main.rand.Next(2);
                            switch (chance)
                            {
                                case 1:
                                    SetChat(myColor, ModUtils.GetChatText("Flandre", "4"), 4, 90, 30, true);
                                    break;
                                default:
                                    SetChat(myColor, ModUtils.GetChatText("Flandre", "5"), 5, 90, 30, true);
                                    break;
                            }
                            extraAI[2] = 1;
                        }
                    }
                }
                if (Projectile.owner == Main.myPlayer)
                {
                    if (extraAI[1] > Main.rand.Next(90, 120))
                    {
                        extraAI[1] = 0;
                        extraAI[0] = 1;
                        extraAI[2] = 0;
                        Projectile.netUpdate = true;
                    }
                }
            }
            else if (extraAI[0] == 1)
            {
                if (Projectile.frame >= 7)
                {
                    Projectile.frame = 7;
                    extraAI[1]++;
                }
                if (Projectile.owner == Main.myPlayer)
                {
                    if (extraAI[1] > Main.rand.Next(120, 360))
                    {
                        extraAI[1] = 0;
                        extraAI[0] = 2;
                        Projectile.netUpdate = true;
                    }
                }
            }
            else
            {
                Projectile.frameCounter += 1;
                if (Projectile.frame > 9)
                {
                    Projectile.frame = 0;
                    extraAI[0] = 1200;
                    PetState = 0;
                }
            }
        }
        private void UpdateWingFrame()
        {
            if (wingFrame < 15)
            {
                wingFrame = 15;
            }
            int count = 6;
            if (++wingFrameCounter > count)
            {
                wingFrameCounter = 0;
                wingFrame++;
            }
            if (wingFrame > 19)
            {
                wingFrame = 15;
            }
        }
        private void UpdateClothFrame()
        {
            if (clothFrame < 11)
            {
                clothFrame = 11;
            }
            int count = 4;
            if (++clothFrameCounter > count)
            {
                clothFrameCounter = 0;
                clothFrame++;
            }
            if (clothFrame > 14)
            {
                clothFrame = 11;
            }
        }
        Color myColor = new Color(255, 10, 10);
        public override string GetChatText(out string[] text)
        {
            text = new string[21];
            text[1] = ModUtils.GetChatText("Flandre", "1");
            text[2] = ModUtils.GetChatText("Flandre", "2");
            text[3] = ModUtils.GetChatText("Flandre", "3");
            WeightedRandom<string> chat = new WeightedRandom<string>();
            {
                for (int i = 1; i < text.Length; i++)
                {
                    if (text[i] != null)
                    {
                        int weight = 1;
                        if (PetState == 2 && i < 4)
                        {
                            weight = 0;
                        }
                        chat.Add(text[i], weight);
                    }
                }
            }
            return chat;
        }
        private void UpdateTalking()
        {
            if (Main.dayTime)
                return;

            int type2 = ProjectileType<Remilia>();
            if (FindChatIndex(out Projectile _, type2, 6, default, 0))
            {
                ChatCD = 1;
            }
            if (FindChatIndex(out Projectile p1, type2, 6, default, 1, true))
            {
                SetChatWithOtherOne(p1, ModUtils.GetChatText("Flandre", "6"), myColor, 6, 600);
            }
            else if (FindChatIndex(out Projectile p2, type2, 7, default, 1, true))
            {
                SetChatWithOtherOne(p2, ModUtils.GetChatText("Flandre", "7"), myColor, 7, 600);
            }
            else if (FindChatIndex(out Projectile p3, type2, 8, default, 1, true))
            {
                SetChatWithOtherOne(p3, ModUtils.GetChatText("Flandre", "8"), myColor, 0, 360);
                p3.localAI[2] = 0;
            }
            else if (mainTimer % 720 == 0 && Main.rand.NextBool(7) && mainTimer > 0)
            {
                SetChat(myColor);
            }
        }
        public override void VisualEffectForPreview()
        {
            UpdateWingFrame();
            UpdateClothFrame();
        }
        private void SetFlandreLight()
        {
            float r = Main.DiscoR / 255f;
            float g = Main.DiscoG / 255f;
            float b = Main.DiscoB / 255f;
            float strength = 2f;
            r = (strength + r) / 2f;
            g = (strength + g) / 2f;
            b = (strength + b) / 2f;
            Lighting.AddLight(Projectile.Center, r, g, b);
            Lighting.AddLight(Projectile.Center, 0.90f, 0.31f, 0.68f);
        }
        private void ControlMovement(Player player)
        {
            Projectile.tileCollide = false;
            if (PetState != 2)
                Projectile.rotation = Projectile.velocity.X * 0.03f;
            else
                Projectile.rotation = Projectile.velocity.X * 0.005f;

            Vector2 point = new Vector2(50 * player.direction, -40 + player.gfxOffY);
            bool hasRemilia = player.ownedProjectileCounts[ProjectileType<Remilia>()] > 0;
            if (hasRemilia)
            {
                point = new Vector2(-50 * player.direction, -40 + player.gfxOffY);
            }

            ChangeDir(player, hasRemilia);
            MoveToPoint(point, 19);
        }
        public override void AI()
        {
            SetFlandreLight();
            Player player = Main.player[Projectile.owner];
            Projectile.SetPetActive(player, BuffType<FlandreBuff>());
            Projectile.SetPetActive(player, BuffType<ScarletBuff>());

            if (!Main.dayTime)
                UpdateTalking();
            ControlMovement(player);

            if (Remilia.HateSunlight(Projectile))
            {
                extraAI[0] = 0;
                extraAI[1] = 0;
                extraAI[2] = 0;
                Projectile.rotation = 0f;
                PetState = 0;
                Projectile.frame = 10;
                chatFuncIsOccupied = true;
                return;
            }
            if (Projectile.owner == Main.myPlayer)
            {
                if (mainTimer % 270 == 0 && PetState != 2)
                {
                    PetState = 1;
                    Projectile.netUpdate = true;
                }
                if (mainTimer >= 1200 && mainTimer < 3600 && PetState != 1)
                {
                    if (mainTimer % 600 == 0 && Main.rand.NextBool(4) && extraAI[0] <= 0)
                    {
                        PetState = 2;
                        Projectile.netUpdate = true;
                    }
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
                Eatting();
            }
            extraAdjX = 0;
            extraAdjY = 0;
            if (Projectile.frame >= 1 && Projectile.frame <= 9)
            {
                extraAdjY = -2;
                if (Projectile.frame != 1 && Projectile.frame != 6 && Projectile.frame != 9)
                {
                    extraAdjX = -2 * Projectile.spriteDirection;
                }
            }
        }
    }
}


