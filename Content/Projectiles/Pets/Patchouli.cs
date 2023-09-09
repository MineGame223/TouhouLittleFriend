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
            Projectile.DrawStateNormalizeForPet();
            DrawPatchouli(clothFrame, lightColor, null, true);
            Projectile.DrawStateNormalizeForPet();
            DrawPatchouli(clothFrame, lightColor, AltVanillaFunction.GetExtraTexture("Patchouli_Cloth"));
            DrawPatchouli(Projectile.frame, lightColor);
            if (PetState == 1 || PetState == 3)
                DrawPatchouli(blinkFrame, lightColor);
            DrawPatchouli(Projectile.frame, lightColor, AltVanillaFunction.GetExtraTexture("Patchouli_Cloth"), true);
            return false;
        }
        private void DrawPatchouli(int frame, Color lightColor, Texture2D tex = null, bool entitySpriteDraw = false)
        {
            Texture2D t = tex ?? AltVanillaFunction.ProjectileTexture(Type);
            int height = t.Height / Main.projFrames[Type];
            Vector2 pos = Projectile.Center - Main.screenPosition + new Vector2(0, 7f * Main.essScale);
            Rectangle rect = new Rectangle(0, frame * height, t.Width / 2, height);
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
            //Player player = Main.player[Projectile.owner];
            text = new string[21];
            if (Projectile.velocity.Length() >= 4f)
            {
                text[1] = "不想动...";
            }
            if (PetState > 1)
            {
                text[2] = "是没读到过的内容呢...";
                text[3] = "这里...似乎和我之前了解的不太一样？";
                text[4] = "有意思的东西...";
                text[5] = "...真的是这样吗？";
                if (talkInterval <= 0 && FindPetState(out Projectile _, ProjectileType<Remilia>(), 0) && !Main.dayTime)
                {
                    text[16] = "唔...蕾米？";
                }
                /*if (talkInterval <= 0 && FindPetState(out Projectile _, ProjectileType<Hina>(), 0))
                {
                    text[13] = "呐，雏？";
                }*/
            }
            else
            {
                text[7] = "上次看到哪儿了来着...?";
            }
            text[6] = "咳...咳咳...";
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
            int type2 = ProjectileType<Remilia>();
            //int type3 = ProjectileType<Hina>();
            if (FindChatIndex(out Projectile p1, type2, 5, default, 1, true))
            {
                SetChatWithOtherOne(p1, "你身为吸血鬼，为什么不像书里说的一样怕十字架？", myColor, 17, 600, -1, 7);
            }
            else if (FindChatIndex(out Projectile p2, type2, 6, default, 1, true))
            {
                SetChatWithOtherOne(p2, "好吧...看来书里说的不全是正确的。", myColor, 18, 600, -1, 7);
            }
            else if (FindChatIndex(out Projectile p3, type2, 7, default, 1, true))
            {
                SetChatWithOtherOne(p3, "不要...", myColor, 19, 360, -1);
                p3.ai[0] = 0;
                talkInterval = 3600;
            }
            /*else if (FindChatIndex(out Projectile p4, type3, 11, default, 1, true))
            {
                SetChatWithOtherOne(p4, "唔...我似乎找到了一个可以削弱你的厄运光环的办法", myColor, 14, 600, -1, 7);
            }
            else if (FindChatIndex(out Projectile p5, type3, 12, default, 1, true))
            {
                SetChatWithOtherOne(p5, "嗯，有空和你说说吧", myColor, 15, 360, -1, 12);
                talkInterval = 3600;
            }*/
            else if (mainTimer % 720 == 0 && Main.rand.NextBool(12))
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
            Vector2 point = new Vector2(40 * player.direction, -20 + player.gfxOffY);
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


