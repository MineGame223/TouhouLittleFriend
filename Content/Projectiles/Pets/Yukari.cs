using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ID;
using Terraria.Utilities;
using TouhouPets.Content.Buffs.PetBuffs;

namespace TouhouPets.Content.Projectiles.Pets
{
    public class Yukari : BasicTouhouPet
    {
        public override void SetStaticDefaults()
        {
            Main.projFrames[Type] = 10;
            Main.projPet[Type] = true;
            ProjectileID.Sets.LightPet[Type] = true;
        }
        public override bool PreDraw(ref Color lightColor)
        {
            DrawGap(lightColor);
            Projectile.DrawStateNormalizeForPet();

            DrawYukari(hairFrame, lightColor, 0);
            DrawYukari(Projectile.frame, lightColor);
            if (PetState == 1)
                DrawYukari(blinkFrame, lightColor, 1);
            DrawYukari(Projectile.frame, lightColor, 0, default, AltVanillaFunction.GetExtraTexture("Yukari_Cloth"), true);
            DrawYukari(clothFrame, lightColor, 1, default, null, true);
            return false;
        }
        private void DrawGap(Color lightColor)
        {
            for (int i = 0; i < 4; i++)
            {
                DrawYukari(gapFrame, Projectile.GetAlpha(Color.Purple * 0.2f), 0, new Vector2(0, -2 * Main.essScale).RotatedBy(MathHelper.ToRadians(90 * i)));
                DrawYukari(gapFrame, Projectile.GetAlpha(Color.Purple * 0.2f), 0, new Vector2(0, -2 * Main.essScale).RotatedBy(MathHelper.ToRadians(90 * i)), AltVanillaFunction.GetExtraTexture("Yukari_Cloth"), true);
            }
            DrawYukari(gapFrame, Projectile.GetAlpha(Color.White * 0.9f * Main.essScale), 0);
            DrawYukari(gapFrame, lightColor, 0, default, AltVanillaFunction.GetExtraTexture("Yukari_Cloth"), true);
        }
        private void DrawYukari(int frame, Color lightColor, int columns = 0, Vector2 extraPos = default, Texture2D tex = null, bool entitySpriteDraw = false)
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
        private void Blink()
        {
            if (blinkFrame < 4)
            {
                blinkFrame = 4;
            }
            if (++blinkFrameCounter > 3)
            {
                blinkFrameCounter = 0;
                blinkFrame++;
            }
            if (blinkFrame > 6)
            {
                blinkFrame = 4;
                PetState = 0;
            }
        }
        int gapFrame, gapFrameCounter;
        int blinkFrame, blinkFrameCounter;
        int clothFrame, clothFrameCounter;
        int hairFrame, hairFrameCounter;
        private void UpdateGapFrame()
        {
            if (gapFrame < 8)
            {
                gapFrame = 8;
            }
            int count = 11;
            if (++gapFrameCounter > count)
            {
                gapFrameCounter = 0;
                gapFrame++;
            }
            if (gapFrame > 9)
            {
                gapFrame = 8;
            }
        }
        private void UpdateMiscFrame()
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

            if (hairFrame < 4)
            {
                hairFrame = 4;
            }
            if (++hairFrameCounter > count)
            {
                hairFrameCounter = 0;
                hairFrame++;
            }
            if (hairFrame > 7)
            {
                hairFrame = 4;
            }
        }
        Color myColor = new Color(156, 91, 250);
        public override string GetChatText(out string[] text)
        {
            text = new string[5];
            text[1] = ModUtils.GetChatText("Yukari", "1");
            text[2] = ModUtils.GetChatText("Yukari", "2");
            text[3] = ModUtils.GetChatText("Yukari", "3");
            text[4] = ModUtils.GetChatText("Yukari", "4");
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
            int type1 = ProjectileType<Ran>();
            if (FindChatIndex(out Projectile _, type1, 3, default, 0))
            {
                ChatCD = 1;
            }
            if (FindChatIndex(out Projectile p, type1, 3))
            {
                SetChatWithOtherOne(p, ModUtils.GetChatText("Yukari", "5"), myColor, 5, 600);
            }
            else if (FindChatIndex(out Projectile p1, type1, 5, default, 1, true))
            {
                SetChatWithOtherOne(p1, ModUtils.GetChatText("Yukari", "6"), myColor, 6, 600);
            }
            else if (mainTimer % 960 == 0 && Main.rand.NextBool(9) && mainTimer > 0)
            {
                SetChat(myColor);
            }
        }
        public override void VisualEffectForPreview()
        {
            UpdateGapFrame();
            UpdateMiscFrame();
        }
        public override void AI()
        {
            Lighting.AddLight(Projectile.Center, 1.56f, 0.91f, 2.50f);
            Player player = Main.player[Projectile.owner];
            Projectile.SetPetActive(player, BuffType<YukariBuff>());
            UpdateTalking();
            Vector2 point = new Vector2(60 * player.direction, -30 + player.gfxOffY);
            Projectile.tileCollide = false;
            Projectile.rotation = Projectile.velocity.X * 0.002f;

            if (Main.rand.NextBool(7))
                Dust.NewDustPerfect(Projectile.position + new Vector2(Main.rand.Next(0, Projectile.width), Main.rand.Next(0, Projectile.height)), MyDustId.PurpleLight
                    , new Vector2(Main.rand.NextFloat(-1f, 1f), Main.rand.NextFloat(-1f, 1f)), 100, default
                    , Main.rand.NextFloat(1f, 2f)).noGravity = true;

            ChangeDir(player);
            MoveToPoint(point, 18f);
            if (Projectile.owner == Main.myPlayer)
            {
                if (mainTimer % 270 == 0 && PetState != 2)
                {
                    PetState = 1;
                    Projectile.netUpdate = true;
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
        }
    }
}


