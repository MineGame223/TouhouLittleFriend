using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ID;
using Terraria.Utilities;
using TouhouPets.Content.Buffs.PetBuffs;

namespace TouhouPets.Content.Projectiles.Pets
{
    public class Rin : BasicTouhouPet
    {
        public override void SetStaticDefaults()
        {
            Main.projFrames[Type] = 13;
            Main.projPet[Type] = true;
            ProjectileID.Sets.LightPet[Type] = false;
        }
        public override bool PreDraw(ref Color lightColor)
        {
            DrawRin(swingFrame + 8, lightColor, 1);
            DrawRin(swingFrame + 4, lightColor, 1);
            DrawRin(Projectile.frame, lightColor);
            if (PetState == 1)
                DrawRin(blinkFrame, lightColor);
            DrawRin(swingFrame, lightColor, 1);
            DrawRin(swingFrame + 4, lightColor, 1, AltVanillaFunction.GetExtraTexture("Rin_Cloth"), true);
            DrawRin(Projectile.frame, lightColor, 0, AltVanillaFunction.GetExtraTexture("Rin_Cloth"), true);
            DrawRin(swingFrame, lightColor, 1, AltVanillaFunction.GetExtraTexture("Rin_Cloth"), true);
            Projectile.DrawStateNormalizeForPet();
            DrawRin(Projectile.frame, Color.White * 0.8f, 2);
            return false;
        }
        private void DrawRin(int frame, Color lightColor, int columns = 0, Texture2D tex = null, bool entitySpriteDraw = false)
        {
            Texture2D t = tex ?? AltVanillaFunction.ProjectileTexture(Type);
            int height = t.Height / Main.projFrames[Type];
            Vector2 pos = Projectile.Center - Main.screenPosition + new Vector2(0, 7f * Main.essScale);
            Rectangle rect = new Rectangle(t.Width / 3 * columns, frame * height, t.Width / 3, height);
            Vector2 orig = rect.Size() / 2;
            SpriteEffects effect = Projectile.spriteDirection == -1 ? SpriteEffects.None : SpriteEffects.FlipHorizontally;
            if (entitySpriteDraw)
                Main.EntitySpriteDraw(t, pos, rect, Projectile.GetAlpha(lightColor), Projectile.rotation, orig, Projectile.scale, effect, 0f);
            else
                Main.spriteBatch.TeaNPCDraw(t, pos, rect, Projectile.GetAlpha(lightColor), Projectile.rotation, orig, Projectile.scale, effect, 0f);
        }
        private void Blink()
        {
            if (blinkFrame < 10)
            {
                blinkFrame = 10;
            }
            if (++blinkFrameCounter > 3)
            {
                blinkFrameCounter = 0;
                blinkFrame++;
            }
            if (blinkFrame > 12)
            {
                blinkFrame = 10;
                PetState = 0;
            }
        }
        int blinkFrame, blinkFrameCounter;
        int swingFrame, swingFrameCounter;
        private void Playing()
        {
            if (Projectile.frame < 1)
            {
                Projectile.frame = 1;
            }
            if (++Projectile.frameCounter > 4)
            {
                Projectile.frameCounter = 0;
                Projectile.frame++;
            }
            if (extraAI[0] == 0)
            {
                if (Projectile.frame > 7)
                {
                    Projectile.frame = 3;
                    extraAI[1]++;
                }
                if (Projectile.owner == Main.myPlayer)
                {
                    if (extraAI[1] > extraAI[2])
                    {
                        extraAI[2] = 0;
                        extraAI[1] = 0;
                        extraAI[0] = 1;
                        Projectile.netUpdate = true;
                    }
                }
            }
            else
            {
                if (Projectile.frame > 9)
                {
                    Projectile.frame = 0;
                    extraAI[0] = 3600;
                    PetState = 0;
                }
            }
        }
        private void UpdateMiscFrame()
        {
            if (++swingFrameCounter > 6)
            {
                swingFrameCounter = 0;
                swingFrame++;
            }
            if (swingFrame > 3)
            {
                swingFrame = 0;
            }
        }
        Color myColor = new Color(227, 59, 59);
        public override string GetChatText(out string[] text)
        {
            Player player = Main.player[Projectile.owner];
            text = new string[11];
            text[1] = ModUtils.GetChatText("Rin", "1");
            text[2] = ModUtils.GetChatText("Rin", "2");
            text[3] = ModUtils.GetChatText("Rin", "3");
            if (player.ZoneUnderworldHeight)
            {
                text[4] = ModUtils.GetChatText("Rin", "4");
            }
            if (player.ownedProjectileCounts[ProjectileType<Satori>()] > 0)
            {
                text[5] = ModUtils.GetChatText("Rin", "5");
                text[6] = ModUtils.GetChatText("Rin", "6");
            }
            if (player.ownedProjectileCounts[ProjectileType<Utsuho>()] > 0)
            {
                text[7] = ModUtils.GetChatText("Rin", "7");
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
            if (mainTimer % 640 == 0 && Main.rand.NextBool(6) && PetState != 2)
            {
                SetChat(myColor);
            }
        }
        public override void VisualEffectForPreview()
        {
            UpdateMiscFrame();
        }
        public override void AI()
        {
            Player player = Main.player[Projectile.owner];
            Projectile.SetPetActive(player, BuffType<RinBuff>());
            Projectile.SetPetActive(player, BuffType<HellPetsBuff>());

            UpdateTalking();
            Vector2 point = new Vector2(-50 * player.direction, -30 + player.gfxOffY);
            if (player.ownedProjectileCounts[ProjectileType<Koishi>()] > 0)
                point = new Vector2(-70 * player.direction, 0 + player.gfxOffY);
            Projectile.tileCollide = false;
            Projectile.rotation = Projectile.velocity.X * 0.012f;

            ChangeDir(player, true);
            MoveToPoint(point, 12.5f);

            int dustID = MyDustId.CyanBubble;
            Dust.NewDustPerfect(Projectile.Center + new Vector2(-28, 8), dustID
                , new Vector2(Main.rand.NextFloat(-0.5f, 0.5f), Main.rand.NextFloat(-2.5f, -1.2f)), 100, default
                , Main.rand.NextFloat(0.5f, 1.5f)).noGravity = true;
            Dust.NewDustPerfect(Projectile.Center + new Vector2(28, 8), dustID
                , new Vector2(Main.rand.NextFloat(-0.5f, 0.5f), Main.rand.NextFloat(-2.5f, -1.2f)), 100, default
                , Main.rand.NextFloat(0.5f, 1.5f)).noGravity = true;
            Dust.NewDustPerfect(Projectile.Center + new Vector2(Main.rand.NextFloat(-15f, 15f), 28), dustID
                , new Vector2(Main.rand.NextFloat(-0.2f, 0.2f), Main.rand.NextFloat(-1f, -0.2f)), 100, default
                , Main.rand.NextFloat(0.5f, 0.75f)).noGravity = true;

            if (Projectile.owner == Main.myPlayer)
            {
                if (mainTimer % 270 == 0 && PetState != 2)
                {
                    PetState = 1;
                    Projectile.netUpdate = true;
                }
                if (mainTimer >= 1200 && mainTimer < 3600 && PetState != 1)
                {
                    if (mainTimer % 120 == 0 && Main.rand.NextBool(6) && extraAI[0] <= 0)
                    {
                        extraAI[2] = Main.rand.Next(20, 45);
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
                Projectile.frame = 0;
            }
            else if (PetState == 1)
            {
                Blink();
            }
            else if (PetState == 2)
            {
                Playing();
            }
        }
    }
}


