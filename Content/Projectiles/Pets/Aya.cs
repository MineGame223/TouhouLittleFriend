using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.IO;
using Terraria;
using Terraria.ID;
using Terraria.Utilities;
using TouhouPets.Content.Buffs.PetBuffs;

namespace TouhouPets.Content.Projectiles.Pets
{
    public class Aya : BasicTouhouPet
    {
        public override void SetStaticDefaults()
        {
            Main.projFrames[Type] = 15;
            Main.projPet[Type] = true;
            ProjectileID.Sets.LightPet[Type] = false;
        }
        public override bool PreDraw(ref Color lightColor)
        {
            DrawAya(wingFrame, lightColor, 0, new Vector2(extraAdjX, extraAdjY));
            DrawAya(clothFrame + 4, lightColor, 1, new Vector2(extraAdjX, extraAdjY), null, true);
            Projectile.DrawStateNormalizeForPet();
            DrawAya(Projectile.frame, lightColor);
            if (PetState == 1)
                DrawAya(blinkFrame, lightColor);
            DrawAya(clothFrame, lightColor, 1, new Vector2(extraAdjX, extraAdjY), null, true);
            DrawAya(Projectile.frame, lightColor, 0, default, AltVanillaFunction.GetExtraTexture("Aya_Cloth"), true);
            DrawAya(clothFrame + 8, lightColor, 1, new Vector2(extraAdjX, extraAdjY), null, true);
            if (Projectile.frame == 3)
            {
                Projectile.DrawStateNormalizeForPet();
                DrawShotSpark();
            }
            return false;
        }
        private void DrawAya(int frame, Color lightColor, int columns = 0, Vector2 extraPos = default, Texture2D tex = null, bool entitySpriteDraw = false)
        {
            if (extraPos == default)
                extraPos = Vector2.Zero;
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
        private void DrawShotSpark()
        {
            Main.spriteBatch.QuickToggleAdditiveMode(true);
            Texture2D t = AltVanillaFunction.ExtraTexture(ExtrasID.ThePerfectGlow);
            Vector2 pos = Projectile.Center + new Vector2(14 * Projectile.spriteDirection, -10) - Main.screenPosition + new Vector2(0, 7f * Main.essScale);
            Rectangle rect = new Rectangle(0, 0, t.Width, t.Height);
            Vector2 orig = rect.Size() / 2;
            Color clr = Projectile.GetAlpha(Color.White) * flash;
            SpriteEffects effect = Projectile.spriteDirection == -1 ? SpriteEffects.None : SpriteEffects.FlipHorizontally;
            Main.spriteBatch.TeaNPCDraw(t, pos, rect, clr, Projectile.rotation, orig, new Vector2(0.4f, 0.5f) * flash * 1.6f, effect, 0f);
            Main.spriteBatch.TeaNPCDraw(t, pos, rect, clr, Projectile.rotation + MathHelper.Pi / 2, orig, new Vector2(0.5f, 1f) * flash * 1.6f, effect, 0f);
            Main.spriteBatch.QuickToggleAdditiveMode(false);
        }
        private void Blink()
        {
            if (blinkFrame < 6)
            {
                blinkFrame = 6;
            }
            if (++blinkFrameCounter > 3)
            {
                blinkFrameCounter = 0;
                blinkFrame++;
            }
            if (blinkFrame > 8)
            {
                blinkFrame = 6;
                PetState = 0;
            }
        }
        int wingFrame, wingFrameCounter;
        int blinkFrame, blinkFrameCounter;
        int clothFrame, clothFrameCounter;
        int extraAdjX, extraAdjY;
        float flash;
        int flashChance;
        private void Shot()
        {
            Projectile.velocity *= 0.9f;
            if (++Projectile.frameCounter > 7)
            {
                Projectile.frameCounter = 0;
                Projectile.frame++;
            }
            if (extraAI[0] > 0)
            {
                if (extraAI[1] > extraAI[2])
                {
                    if (Projectile.frame >= 5)
                    {
                        Projectile.frame = 5;
                        extraAI[2]--;
                        if (extraAI[2] <= 0)
                        {
                            extraAI[0]--;
                            flashChance = 6;
                            extraAI[1] = 0;
                            extraAI[2] = Main.rand.Next(180, 600);
                            if (extraAI[0] > 1)
                            {
                                Projectile.frame = 1;
                                SetShotChat();
                            }
                            else
                            {
                                extraAI[2] = Main.rand.Next(30, 60);
                            }
                            Projectile.netUpdate = true;
                        }
                    }
                }
                else if (Projectile.frame >= 3)
                {
                    Projectile.frame = 3;
                    extraAI[1]++;
                    if (extraAI[1] % 30 == 0)
                    {
                       Projectile.ai[2] = Main.rand.Next(7 - flashChance);
                        Projectile.netUpdate = true;
                        if (Projectile.ai[2] == 0)
                        {
                            flash = 1;
                            flashChance -= 2;
                            AltVanillaFunction.PlaySound(SoundID.Camera, Projectile.Center);
                        }
                    }
                }
            }
            else
            {
                if (Projectile.frame > 5)
                {
                    Projectile.frame = 0;
                    extraAI[0] = 1800;
                    extraAI[2] = 0;
                    PetState = 0;
                    Projectile.netUpdate = true;
                }
            }
        }
        private void UpdateWingFrame()
        {
            if (wingFrame < 9)
            {
                wingFrame = 9;
            }
            int count = 7;
            if (++wingFrameCounter > count)
            {
                wingFrameCounter = 0;
                wingFrame++;
            }
            if (wingFrame > 14)
            {
                wingFrame = 9;
            }
        }
        private void UpdateClothFrame()
        {
            int count = 5;
            if (++clothFrameCounter > count)
            {
                clothFrameCounter = 0;
                clothFrame++;
            }
            if (clothFrame > 3)
            {
                clothFrame = 0;
            }
        }
        Color myColor = new Color(255, 102, 85);
        public override string GetChatText(out string[] text)
        {
            text = new string[21];
            text[1] = ModUtils.GetChatText("Aya", "1");
            text[2] = ModUtils.GetChatText("Aya", "2");
            text[3] = ModUtils.GetChatText("Aya", "3");
            text[4] = ModUtils.GetChatText("Aya", "4");
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
            if (mainTimer % 720 == 0 && Main.rand.NextBool(7) && mainTimer > 0 && PetState != 2)
            {
                SetChat(myColor);
            }
        }
        private void SetShotChat()
        {
            int chance = Main.rand.Next(6);
            switch (chance)
            {
                case 1:
                    SetChat(myColor, ModUtils.GetChatText("Aya", "5"), 5);
                    break;
                case 2:
                    SetChat(myColor, ModUtils.GetChatText("Aya", "6"), 6);
                    break;
                case 3:
                    SetChat(myColor, ModUtils.GetChatText("Aya", "7"), 7);
                    break;
                case 4:
                    SetChat(myColor, ModUtils.GetChatText("Aya", "8"), 8);
                    break;
                default:
                    break;
            }
        }
        public override void VisualEffectForPreview()
        {
            UpdateWingFrame();
            UpdateClothFrame();
        }
        public override void AI()
        {
            if (flash > 0)
            {
                flash -= 0.1f;
            }
            Lighting.AddLight(Projectile.Center + new Vector2(14 * Projectile.spriteDirection, -10)
                , 2.55f * flash, 2.55f * flash, 2.55f * flash);
            Player player = Main.player[Projectile.owner];
            Projectile.SetPetActive(player, BuffType<AyaBuff>());
            UpdateTalking();
            Vector2 point = new Vector2(-70 * player.direction, -50 + player.gfxOffY);
            Projectile.tileCollide = false;
            if (PetState != 2)
                Projectile.rotation = Projectile.velocity.X * 0.035f;
            else
                Projectile.rotation = Projectile.velocity.X * 0.002f;

            ChangeDir(player, true);
            MoveToPoint(point, 30f);
            if (Projectile.owner == Main.myPlayer)
            {
                if (mainTimer % 270 == 0 && PetState != 2)
                {
                    PetState = 1;
                    Projectile.netUpdate = true;
                }
                if (mainTimer >= 1200 && mainTimer < 3600 && PetState != 1 && extraAI[0] == 0)
                {
                    if (mainTimer % 600 == 0 && Main.rand.NextBool(3) && PetState != 2)
                    {
                        PetState = 2;
                        extraAI[2] = Main.rand.Next(180, 600);
                        extraAI[0] = Main.rand.Next(1, 5);
                        flashChance = 6;
                        Projectile.netUpdate = true;
                        SetShotChat();
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
                Shot();
            }
            extraAdjX = 0;
            extraAdjY = 0;
            if (Projectile.frame == 3)
            {
                extraAdjY = -2;
            }
        }
        public override void SendExtraAI(BinaryWriter writer)
        {
            base.SendExtraAI(writer);
            writer.Write(flashChance);
        }
        public override void ReceiveExtraAI(BinaryReader reader)
        {
            base.ReceiveExtraAI(reader);
            flashChance = reader.ReadInt32();
        }
    }
}


