using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.Graphics.Shaders;
using Terraria.ID;
using Terraria.Utilities;
using TouhouPets.Content.Buffs.PetBuffs;

namespace TouhouPets.Content.Projectiles.Pets
{
    public class Lily : BasicTouhouPet
    {
        public override void SetStaticDefaults()
        {
            Main.projFrames[Type] = 11;
            Main.projPet[Type] = true;
            ProjectileID.Sets.LightPet[Type] = true;
        }
        public override bool PreDraw(ref Color lightColor)
        {
            DrawLily(wingFrame, Color.White, 0.6f);
            DrawLily(Projectile.frame, lightColor);
            DrawLily(blinkFrame, lightColor);
            Texture2D tex = AltVanillaFunction.GetExtraTexture("Lily_Cloth");
            int clothFrame = Projectile.frame;
            if (!Main.LocalPlayer.miscDyes[1].IsAir)
            {
                if (Main.LocalPlayer.miscDyes[1].type == ItemID.BlackDye)
                    clothFrame += 4;
                tex = AltVanillaFunction.GetExtraTexture("Lily_Cloth_Alt");
            }
            DrawLily(clothFrame, lightColor, 1f, tex, true);
            return false;
        }
        private void DrawLily(int frame, Color lightColor, float alpha = 1f, Texture2D tex = null, bool entitySpriteDraw = false)
        {
            Texture2D t = tex ?? AltVanillaFunction.ProjectileTexture(Type);
            int height = t.Height / Main.projFrames[Type];
            Vector2 pos = Projectile.Center - Main.screenPosition + new Vector2(0, 7f * Main.essScale);
            Rectangle rect = new Rectangle(0, frame * height, t.Width, height);
            Vector2 orig = rect.Size() / 2;
            SpriteEffects effect = Projectile.spriteDirection == -1 ? SpriteEffects.None : SpriteEffects.FlipHorizontally;
            if (entitySpriteDraw)
                Main.EntitySpriteDraw(t, pos, rect, Projectile.GetAlpha(lightColor) * alpha, Projectile.rotation, orig, Projectile.scale, effect, 0f);
            else
                Main.spriteBatch.TeaNPCDraw(t, pos, rect, Projectile.GetAlpha(lightColor) * alpha, Projectile.rotation, orig, Projectile.scale, effect, 0f);
        }
        private void Blink()
        {
            if (blinkFrame < 8)
            {
                blinkFrame = 8;
            }
            if (++blinkFrameCounter > 3)
            {
                blinkFrameCounter = 0;
                blinkFrame++;
            }
            if (blinkFrame > 10)
            {
                blinkFrame = 8;
                PetState = 0;
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
        int wingFrame, wingFrameCounter;
        int blinkFrame, blinkFrameCounter;
        private void UpdateWingFrame()
        {
            if (wingFrame < 4)
            {
                wingFrame = 4;
            }
            if (++wingFrameCounter > 4)
            {
                wingFrameCounter = 0;
                wingFrame++;
            }
            if (wingFrame > 7)
            {
                wingFrame = 4;
            }
        }
        Color myColor = new Color(255, 255, 255);
        public override string GetChatText(out string[] text)
        {
            text = new string[11];
            text[1] = ModUtils.GetChatText("Lily", "1");
            text[2] = ModUtils.GetChatText("Lily", "2");
            text[3] = ModUtils.GetChatText("Lily", "3");
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
            if (mainTimer % 720 == 0 && Main.rand.NextBool(9) && mainTimer > 0)
            {
                SetChat(myColor);
            }
        }
        public override void VisualEffectForPreview()
        {
            Idel();
            UpdateWingFrame();
        }
        public override void AI()
        {
            Lighting.AddLight(Projectile.Center, 1.62f, 0.98f, 1.32f);
            Player player = Main.player[Projectile.owner];
            Projectile.SetPetActive(player, BuffType<LilyBuff>());

            UpdateTalking();
            Vector2 point = new Vector2(40 * player.direction, -30 + player.gfxOffY);
            Projectile.tileCollide = false;
            Projectile.rotation = Projectile.velocity.X * 0.026f;

            ChangeDir(player, false);
            MoveToPoint(point, 10.5f);

            int dustID = MyDustId.WhiteTransparent;
            if (Main.rand.NextBool(15))
            {
                Dust.NewDustPerfect(Projectile.Center
                + new Vector2(Main.rand.NextFloat(-30f, -10f) * Projectile.spriteDirection, Main.rand.NextFloat(-5f, 25f)), dustID
                , new Vector2(Main.rand.NextFloat(-0.2f, 0.2f), Main.rand.NextFloat(0.8f, 1.2f)), 100, default
                , Main.rand.NextFloat(1f, 1.2f)).noGravity = true;
            }
            if (Main.rand.NextBool(15))
            {
                Dust.NewDustPerfect(Projectile.Center
                    + new Vector2(Main.rand.NextFloat(20f, 22f) * Projectile.spriteDirection, Main.rand.NextFloat(-5f, 25f)), dustID
                    , new Vector2(Main.rand.NextFloat(-0.2f, 0.2f), Main.rand.NextFloat(0.8f, 1.2f)), 100, default
                    , Main.rand.NextFloat(1f, 1.2f)).noGravity = true;
            }

            if (mainTimer % 270 == 0 && PetState != 2)
            {
                PetState = 1;
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
        }
    }
}


