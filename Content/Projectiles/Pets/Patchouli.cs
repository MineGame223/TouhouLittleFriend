using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ID;
using Terraria.Utilities;
using TouhouPets.Content.Buffs.PetBuffs;

namespace TouhouPets.Content.Projectiles.Pets
{
    public class Patchouli : BasicTouhouPet
    {
        public override void SetStaticDefaults()
        {
            Main.projFrames[Type] = 16;
            Main.projPet[Type] = true;
            ProjectileID.Sets.LightPet[Type] = true;
        }
        public override bool PreDraw(ref Color lightColor)
        {
            DrawAura();
            DrawPatchouli(clothFrame, lightColor, 0, null, true);
            Projectile.DrawStateNormalizeForPet();
            DrawPatchouli(clothFrame, lightColor, 1, null);
            DrawPatchouli(clothFrame, lightColor, 0, AltVanillaFunction.GetExtraTexture("Patchouli_Cloth"));
            DrawPatchouli(Projectile.frame, lightColor);
            if (PetState == 1 || PetState == 3)
                DrawPatchouli(blinkFrame, lightColor);
            DrawPatchouli(Projectile.frame, lightColor, 0, AltVanillaFunction.GetExtraTexture("Patchouli_Cloth"), true);
            return false;
        }
        private void DrawPatchouli(int frame, Color lightColor, int columns = 0, Texture2D tex = null, bool entitySpriteDraw = false)
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
        private void DrawAura()
        {
            Texture2D t = AltVanillaFunction.ProjectileTexture(Type);
            int height = t.Height / Main.projFrames[Type];
            Vector2 pos = Projectile.Center - Main.screenPosition + new Vector2(0, 7f * Main.essScale);
            Rectangle rect4 = new Rectangle(t.Width / 2, auraFrame * height, t.Width / 2, height);
            Vector2 orig = rect4.Size() / 2;
            SpriteEffects effect = Projectile.spriteDirection == -1 ? SpriteEffects.None : SpriteEffects.FlipHorizontally;

            Main.spriteBatch.QuickToggleAdditiveMode(true);
            for (int i = 0; i < 8; i++)
            {
                Vector2 spinningpoint = new Vector2(0f, -1f);
                Main.EntitySpriteDraw(t, pos + spinningpoint.RotatedBy(MathHelper.TwoPi * Main.GlobalTimeWrappedHourly + MathHelper.TwoPi / 8 * i * 0.6f), rect4, Projectile.GetAlpha(Color.White) * 0.4f, Projectile.rotation, orig, Projectile.scale, effect, 0f);
            }
            Main.spriteBatch.QuickToggleAdditiveMode(false);
            Projectile.DrawStateNormalizeForPet();
            Main.EntitySpriteDraw(t, pos, rect4, Projectile.GetAlpha(Color.White) * 0.8f, Projectile.rotation, orig, Projectile.scale, effect, 0f);
        }
        private void Reading()
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
            if (extraAI[0] <= 0)
            {
                extraAI[0] = Main.rand.Next(360, 540);
            }
            if (extraAI[0] < 999)
            {
                if (Projectile.frame >= 3 && extraAI[1] < extraAI[0])
                {
                    extraAI[1]++;
                    Projectile.frame = 3;
                }
                if (Projectile.frame > 6)
                {
                    extraAI[0] = Main.rand.Next(360, 540);
                    extraAI[1] = 0;
                    Projectile.frame = 3;
                }
            }
            if (Projectile.velocity.Length() > 4.5f)
            {
                extraAI[0] = 999;
            }
            if (extraAI[0] >= 999)
            {
                if (Projectile.frame < 7)
                {
                    Projectile.frame = 7;
                }
                if (Projectile.frame > 8)
                {
                    Projectile.frame = 0;
                    extraAI[0] = 180;
                    extraAI[1] = 0;
                    PetState = 0;
                }
            }
        }
        private void Blink(bool alt = false)
        {
            if (alt)
            {
                if (blinkFrame < 10)
                {
                    blinkFrame = 10;
                }
                if (++blinkFrameCounter > 6)
                {
                    blinkFrameCounter = 0;
                    blinkFrame++;
                }
                if (blinkFrame > 11)
                {
                    blinkFrame = 10;
                    PetState = 2;
                }
            }
            else
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
        }
        int blinkFrame, blinkFrameCounter;
        int clothFrame, clothFrameCounter;
        int auraFrame, auraFrameCounter;
        private void UpdateClothFrame()
        {
            if (clothFrame < 12)
            {
                clothFrame = 12;
            }
            if (++clothFrameCounter > 10)
            {
                clothFrameCounter = 0;
                clothFrame++;
            }
            if (clothFrame > 15)
            {
                clothFrame = 12;
            }
        }
        private void UpdateAuraFrame()
        {
            if (++auraFrameCounter > 3)
            {
                auraFrameCounter = 0;
                auraFrame++;
            }
            if (auraFrame > 4)
            {
                auraFrame = 0;
            }
        }
        Color myColor = new Color(252, 197, 238);
        public override string GetChatText(out string[] text)
        {
            text = new string[21];
            if (Projectile.velocity.Length() >= 4f)
            {
                text[1] = ModUtils.GetChatText("Patchouli", "1");
            }
            if (PetState > 1)
            {
                text[2] = ModUtils.GetChatText("Patchouli", "2");
                text[3] = ModUtils.GetChatText("Patchouli", "3");
                text[4] = ModUtils.GetChatText("Patchouli", "4");
                text[5] = ModUtils.GetChatText("Patchouli", "5");
                if (FindPetState(out Projectile _, ProjectileType<Remilia>(), 0) && !Main.dayTime)
                {
                    text[6] = ModUtils.GetChatText("Patchouli", "6");
                }
            }
            else
            {
                text[7] = ModUtils.GetChatText("Patchouli", "7");
            }
            text[8] = ModUtils.GetChatText("Patchouli", "8");
            if (FindPetState(out Projectile _, ProjectileType<Alice>(), 0))
            {
                text[12] = ModUtils.GetChatText("Patchouli", "12");
            }
            WeightedRandom<string> chat = new WeightedRandom<string>();
            {
                for (int i = 1; i < text.Length; i++)
                {
                    if (text[i] != null)
                    {
                        int weight = 1;
                        if (i == 13)
                            weight = 2;
                        if (i == 16)
                            weight = 10;
                        chat.Add(text[i], weight);
                    }
                }
            }
            return chat;
        }
        private void UpdateTalking()
        {
            int type = ProjectileType<Alice>();
            int type2 = ProjectileType<Remilia>();
            int type3 = ProjectileType<Koakuma>();
            if (FindChatIndex(out Projectile _, type3, 4, default, 0)
                || FindChatIndex(out Projectile _, type3, 6, default, 0))
            {
                ChatCD = 1;
            }

            if (FindChatIndex(out Projectile p1, type2, 10, default, 1, true))
            {
                SetChatWithOtherOne(p1, ModUtils.GetChatText("Patchouli", "9"), myColor, 9, 600, -1, 7);
            }
            else if (FindChatIndex(out Projectile p2, type2, 11, default, 1, true))
            {
                SetChatWithOtherOne(p2, ModUtils.GetChatText("Patchouli", "10"), myColor, 10, 600, -1, 7);
            }
            else if (FindChatIndex(out Projectile p3, type2, 12, default, 1, true))
            {
                SetChatWithOtherOne(p3, ModUtils.GetChatText("Patchouli", "11"), myColor, 0, 360, -1);
                p3.localAI[2] = 0;
            }
            else if (FindChatIndex(out Projectile p4, type, 8, default, 1, true))
            {
                SetChatWithOtherOne(p4, ModUtils.GetChatText("Patchouli", "13"), myColor, 13, 600, -1, 12);
            }
            else if (FindChatIndex(out Projectile p5, type, 9, default, 0, true))
            {
                ChatCD = 0;
                SetChat(myColor, ModUtils.GetChatText("Patchouli", "14"), 14, 0);
                ChatCD = 600;
                p5.localAI[2] = 0;
            }
            else if (FindChatIndex(out Projectile p6, type, 10, default, 1, true))
            {
                SetChatWithOtherOne(p6, ModUtils.GetChatText("Patchouli", "15"), myColor, 0, 360);
                p6.localAI[2] = 0;
            }
            else if (FindChatIndex(out Projectile p7, type3, 4, default, 1, true))
            {
                SetChatWithOtherOne(p7, ModUtils.GetChatText("Patchouli", "16"), myColor, 16, 600);
            }
            else if (FindChatIndex(out Projectile p8, type3, 5, default, 1, true))
            {
                SetChatWithOtherOne(p8, ModUtils.GetChatText("Patchouli", "17"), myColor, 17, 600);
                p8.localAI[2] = 0;
            }
            else if (FindChainedChat(17))
            {
                SetChatWithOtherOne(p8, ModUtils.GetChatText("Patchouli", "18"), myColor, 0, 360);
            }
            else if (FindChatIndex(out Projectile p9, type3, 6, default, 1, true))
            {
                int chance = Main.rand.Next(19, 36);
                SetChatWithOtherOne(p9, ModUtils.GetChatText("Patchouli", chance.ToString()), myColor, chance, 600);
            }
            else if (mainTimer % 720 == 0 && Main.rand.NextBool(12) && mainTimer > 0)
            {
                SetChat(myColor);
            }
        }
        public override void VisualEffectForPreview()
        {
            UpdateClothFrame();
            UpdateAuraFrame();
        }
        public override void AI()
        {
            Lighting.AddLight(Projectile.Center, 2.52f, 1.97f, 2.38f);
            Player player = Main.player[Projectile.owner];
            Projectile.SetPetActive(player, BuffType<PatchouliBuff>());
            UpdateTalking();
            Vector2 point = new Vector2(50 * player.direction, -20 + player.gfxOffY);
            if (player.ownedProjectileCounts[ProjectileType<Flandre>()] > 0)
            {
                point = new Vector2(0, -60 + player.gfxOffY);
            }
            Projectile.tileCollide = false;
            Projectile.rotation = Projectile.velocity.X * 0.012f;

            ChangeDir(player);
            MoveToPoint(point, 4.5f);

            if (mainTimer % 270 == 0)
            {
                if (PetState <= 1)
                {
                    PetState = 1;
                }
                else
                {
                    PetState = 3;
                }
            }
            if (PetState == 0)
            {
                if (mainTimer % 180 == 0 && Main.rand.NextBool(4) && extraAI[0] <= 0 && Projectile.velocity.Length() < 2f)
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
                Projectile.frame = 0;
                Blink();
            }
            else
            {
                Reading();
                if (PetState == 1)
                {
                    PetState = 3;
                }
                if (PetState == 3)
                {
                    Blink(true);
                }
            }
        }
    }
}


