﻿using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ID;
using Terraria.Utilities;
using TouhouPets.Content.Buffs.PetBuffs;

namespace TouhouPets.Content.Projectiles.Pets
{
    public class Wakasagihime : BasicTouhouPet
    {
        public override void SetStaticDefaults()
        {
            Main.projFrames[Type] = 10;
            Main.projPet[Type] = true;
            ProjectileID.Sets.LightPet[Type] = true;
        }
        public override bool PreDraw(ref Color lightColor)
        {
            int extraHeight = Projectile.frame == 2 ? -2 : 0;
            DrawWakasagihime(tailFrame, lightColor, 1);
            DrawWakasagihime(Projectile.frame, lightColor, 0);
            if (PetState == 1)
                DrawWakasagihime(blinkFrame, lightColor, 0);
            DrawWakasagihime(Projectile.frame, lightColor, 0, default, AltVanillaFunction.GetExtraTexture("Wakasagihime_Cloth"), true);
            for (int i = 0; i < 7; i++)
            {
                DrawWakasagihime(9, Color.White * 0.5f, 0
                    , new Vector2(0, extraHeight) + new Vector2(Main.rand.Next(-10, 11) * 0.2f, Main.rand.Next(-10, 11) * 0.2f), null, true);
            }
            DrawWakasagihime(8, lightColor, 0, new Vector2(0, extraHeight), null, true);
            DrawWakasagihime(8, lightColor, 0, new Vector2(0, extraHeight), AltVanillaFunction.GetExtraTexture("Wakasagihime_Cloth"), true);
            return false;
        }
        private void DrawWakasagihime(int frame, Color lightColor, int columns = 0, Vector2 extraPos = default, Texture2D tex = null, bool entitySpriteDraw = false)
        {
            if (extraPos == default)
                extraPos = Vector2.Zero;
            Texture2D t = tex ?? AltVanillaFunction.ProjectileTexture(Type);
            int height = t.Height / Main.projFrames[Type];
            Vector2 pos = Projectile.Center - Main.screenPosition + extraPos + new Vector2(0, 7f * Main.essScale);
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
            if (blinkFrame < 5)
            {
                blinkFrame = 5;
            }
            if (++blinkFrameCounter > 3)
            {
                blinkFrameCounter = 0;
                blinkFrame++;
            }
            if (blinkFrame > 7)
            {
                blinkFrame = 5;
                PetState = 0;
            }
        }
        int tailFrame, tailFrameCounter;
        int blinkFrame, blinkFrameCounter;
        private void Bubble()
        {
            if (++Projectile.frameCounter > 8)
            {
                Projectile.frameCounter = 0;
                Projectile.frame++;
            }
            if (Projectile.frame >= 2 && extraAI[2] > 0)
            {
                extraAI[2]--;
                Projectile.frame = 2;
            }
            if (Projectile.frame >= 3 && extraAI[1] > 0)
            {
                extraAI[1]--;
                Projectile.frame = 3;
            }
            if (Projectile.frame == 3 && Projectile.frameCounter == 0)
            {
                AltVanillaFunction.PlaySound(SoundID.Item85, Projectile.Center);
                if (Projectile.owner == Main.myPlayer)
                {
                    Projectile.NewProjectile(Projectile.GetSource_FromThis(), Projectile.Center + new Vector2(4 * Projectile.spriteDirection, 4)
                    , new Vector2(Main.rand.NextFloat(1f, 3f) * Projectile.spriteDirection, Main.rand.NextFloat(-0.4f, 0.2f)), ProjectileType<WakasagihimeBubble>(), 0, 0, Main.myPlayer
                    , Main.rand.Next(0, 3), Main.rand.NextFloat(0.6f, 1.2f));
                }
            }
            if (Projectile.frame > 4 && extraAI[1] <= 0)
            {
                extraAI[0] = 1200;
                extraAI[1] = 0;
                extraAI[2] = 0;
                PetState = 0;
                Projectile.frame = 0;
            }
        }
        private void UpdateTailFrame()
        {
            if (++tailFrameCounter > 5)
            {
                tailFrameCounter = 0;
                tailFrame++;
            }
            if (tailFrame > 5)
            {
                tailFrame = 0;
            }
        }
        Color myColor = new Color(87, 164, 255);
        public override string GetChatText(out string[] text)
        {
            text = new string[11];
            if (PetState != 2)
            {
                text[1] = ModUtils.GetChatText("Wakasagihime", "1");
                text[2] = ModUtils.GetChatText("Wakasagihime", "2");
                text[3] = ModUtils.GetChatText("Wakasagihime", "3");
            }
            WeightedRandom<string> chat = new();
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
            if (mainTimer % 600 == 0 && Main.rand.NextBool(7) && mainTimer > 0)
            {
                SetChat(myColor);
            }
        }
        public override void VisualEffectForPreview()
        {
            UpdateTailFrame();
        }
        public override void AI()
        {
            float brightness = Main.essScale * (Projectile.wet ? 1f : 2f);
            Lighting.AddLight(Projectile.Center, 0.87f * brightness, 1.64f * brightness, 1.55f * brightness);
            Player player = Main.player[Projectile.owner];
            Projectile.SetPetActive(player, BuffType<WakasagihimeBuff>());

            UpdateTalking();
            Vector2 point = new Vector2(40 * player.direction, -40 + player.gfxOffY);
            Projectile.tileCollide = false;
            Projectile.rotation = Projectile.velocity.X * 0.022f;

            ChangeDir(player);
            float speed = Projectile.wet ? 18f : 9f;
            MoveToPoint(point, speed);

            int dustID = MyDustId.BlueParticle;
            Dust.NewDustPerfect(Projectile.Center + new Vector2(Main.rand.NextFloat(-15f, 15f), 28), dustID
                , new Vector2(Main.rand.NextFloat(-0.2f, 0.2f), Main.rand.NextFloat(-1f, -0.2f)), 100, default
                , Main.rand.NextFloat(0.75f, 1.05f)).noGravity = true;
            if (Projectile.owner == Main.myPlayer)
            {
                if (mainTimer % 270 == 0 && PetState != 2)
                {
                    PetState = 1;
                    Projectile.netUpdate = true;
                }
                if (PetState == 0)
                {
                    if (mainTimer >= 300 && mainTimer % 800 == 0 && extraAI[0] <= 0 && Main.rand.NextBool(5))
                    {
                        PetState = 2;
                        extraAI[1] = Main.rand.Next(360, 480);
                        extraAI[2] = Main.rand.Next(30, 90);
                        Projectile.netUpdate = true;
                        SetChat(myColor, ModUtils.GetChatText("Wakasagihime", "4"), 4, 120, 30, true);
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
                Blink();
            }
            else if (PetState == 2)
            {
                Bubble();
            }
        }
    }
}


