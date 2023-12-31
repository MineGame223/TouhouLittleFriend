using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.Localization;
using Terraria.Utilities;
using TouhouPets.Content.Buffs.PetBuffs;

namespace TouhouPets.Content.Projectiles.Pets
{
    public class Koishi : BasicTouhouPet
    {
        public override void SetStaticDefaults()
        {
            Main.projFrames[Type] = 18;
            Main.projPet[Type] = true;
            ProjectileID.Sets.LightPet[Type] = false;
        }
        public override bool PreDraw(ref Color lightColor)
        {
            float t2 = Main.GlobalTimeWrappedHourly * 2f;
            Vector2 eyeAdj = new Vector2(1.2f * (float)Math.Cos(t2), 0.35f * (float)Math.Sin(t2)) * 26f;
            Vector2 eyePos = Projectile.Center - Main.screenPosition + eyeAdj + new Vector2(0, 8);

            if (eyeAdj.Y <= 0)
                DrawEye(eyePos, lightColor);

            DrawKoishi(Projectile.frame, lightColor);
            if (PetState == 1)
                DrawKoishi(blinkFrame, lightColor, 1);

            DrawKoishi(Projectile.frame, lightColor, 0, AltVanillaFunction.GetExtraTexture("Koishi_Cloth"), true);
            DrawKoishi(clothFrame, lightColor, 1, null, true);

            Projectile.DrawStateNormalizeForPet();
            if (PetState == 3)
                DrawKoishi(annoyingFrame, lightColor, 1);

            if (eyeAdj.Y > 0)
                DrawEye(eyePos, lightColor);
            return false;
        }
        private void DrawKoishi(int frame, Color lightColor, int columns = 0, Texture2D tex = null, bool entitySpriteDraw = false)
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
        private void DrawEye(Vector2 eyePos, Color lightColor)
        {
            Texture2D t = AltVanillaFunction.ProjectileTexture(Type);
            int height = t.Height / Main.projFrames[Type];
            Rectangle rect = new Rectangle(t.Width / 2, 7 * height, t.Width / 2, height);
            Vector2 orig = rect.Size() / 2;
            Main.spriteBatch.TeaNPCDraw(t, eyePos, rect, Projectile.GetAlpha(lightColor), Projectile.rotation, orig, Projectile.scale, SpriteEffects.None, 0f);
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
        private void Fade()
        {
            extraAI[0]++;
            if (extraAI[0] > extraAI[1] - 255 / 2)
            {
                Projectile.Opacity += 0.01f;
            }
            else
            {
                Projectile.Opacity -= 0.005f;
            }
            if (Projectile.owner == Main.myPlayer)
            {
                if (extraAI[0] > extraAI[1])
                {
                    extraAI[0] = 3600;
                    extraAI[1] = 0;
                    PetState = 0;
                    Projectile.netUpdate = true;
                }
            }
        }
        private void Annoying()
        {
            if (++annoyingFrameCounter > 4)
            {
                annoyingFrameCounter = 0;
                annoyingFrame++;
            }
            if (annoyingFrame < 8)
            {
                annoyingFrame = 8;
            }
            if (annoyingFrame > 9)
            {
                annoyingFrame = 8;
            }
            if (++Projectile.frameCounter > 5)
            {
                Projectile.frameCounter = 0;
                Projectile.frame++;
            }
            if (Projectile.frame < 14)
            {
                Projectile.frame = 14;
            }
            if (Projectile.owner == Main.myPlayer)
            {
                if (extraAI[1] > extraAI[2])
                {
                    extraAI[0] = 1;
                    Projectile.netUpdate = true;
                }
            }
            if (extraAI[0] >= 1)
            {
                if (Projectile.frame > 17)
                {
                    extraAI[0] = 1800;
                    extraAI[1] = 0;
                    extraAI[2] = 0;
                    PetState = 0;
                    Projectile.frame = 0;
                }
            }
            else
            {
                if (Projectile.frame > 16)
                {
                    extraAI[1]++;
                    Projectile.frame = 15;
                }
            }
            chatFuncIsOccupied = true;
        }
        int blinkFrame, blinkFrameCounter;
        int clothFrame, clothFrameCounter;
        int annoyingFrame, annoyingFrameCounter;
        private void UpdateClothFrame()
        {
            int count = 4;
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
        private void KillingPhoneCall()
        {
            Player player = Main.player[Projectile.owner];
            if (++Projectile.frameCounter > 12)
            {
                Projectile.frameCounter = 0;
                Projectile.frame++;
            }
            if (extraAI[0] == 0)
            {
                if (Projectile.frame < 5)
                {
                    chatFuncIsOccupied = true;
                }
                else
                {
                    if (extraAI[2] <= 0)
                    {
                        if (extraAI[1] == 0)
                        {
                            SetChatWithOtherOne(null, ModUtils.GetChatText("Koishi", "-1"), myColor, -1, cd: 60, -1, 60);
                            extraAI[2] = 240;
                            extraAI[1]++;
                        }
                        if (extraAI[1] == 1 && extraAI[2] <= 0)
                        {
                            extraAI[2] = 240;
                            extraAI[1]++;
                        }
                        if (extraAI[1] == 2)
                        {
                            SetChatWithOtherOne(null, ModUtils.GetChatText("Koishi", "-2"), myColor, -2, cd: 60, -1, 60);
                            extraAI[2] = 540;
                            extraAI[1]++;
                        }
                        if (Projectile.owner == Main.myPlayer)
                        {
                            if (extraAI[1] == 3 && extraAI[2] <= 0)
                            {
                                extraAI[1]++;
                                Projectile.netUpdate = true;
                            }
                        }
                    }
                }
                if (Projectile.frame > 6)
                {
                    Projectile.frame = 5;
                }
                if (extraAI[1] >= 4)
                {
                    Projectile.frame = 6;
                    Projectile.Opacity -= 0.01f;
                    if (Projectile.Opacity <= 0f)
                    {
                        if (Projectile.owner == Main.myPlayer)
                        {
                            extraAI[0] = 1;
                            extraAI[1] = 0;
                            Projectile.netUpdate = true;
                        }
                    }
                }
            }
            else if (extraAI[0] == 1)
            {
                textShaking = true;
                Projectile.Opacity += 0.01f;
                if (Projectile.Opacity < 1)
                {
                    Projectile.frame = 7;
                }
                if (extraAI[1] == 0 && Projectile.frame == 8)
                {
                    SetChat(Color.Red, ModUtils.GetChatText("Koishi", "-3"), -3, 0, 45, true, 300);
                    extraAI[1]++;
                    extraAI[2] = 420;
                }
                if (Projectile.frame > 10)
                {
                    Projectile.frame = 9;
                }
                if (Projectile.owner == Main.myPlayer)
                {
                    if (extraAI[1] > 0 && extraAI[2] <= 0)
                    {
                        extraAI[0]++;
                        extraAI[1] = 0;
                        Projectile.netUpdate = true;
                    }
                }
            }
            else if (extraAI[0] == 2)
            {
                textShaking = true;
                Projectile.frameCounter += 2;
                if (Projectile.frame > 13)
                {
                    Projectile.frame = 13;
                    if (Projectile.owner == Main.myPlayer)
                    {
                        player.KillMe(PlayerDeathReason.ByCustomReason(Language.GetTextValue("Mods.TouhouPets.DeathReason.KilledByKoishi", player.name)), 0, 0, false);
                        extraAI[2] = 0;
                        extraAI[0] = 0;
                        PetState = 0;
                        Projectile.netUpdate = true;
                    }
                }
            }
            if (Projectile.owner == Main.myPlayer)
            {
                if (extraAI[2] > 0)
                {
                    extraAI[2]--;
                }
            }
            if (player.active && extraAI[0] > 0 && !player.dead)
            {
                Projectile.Center = player.Center + new Vector2(-30 * player.direction, player.gfxOffY);
            }
        }
        Color myColor = new Color(145, 255, 183);
        public override string GetChatText(out string[] text)
        {
            text = new string[21];
            text[1] = ModUtils.GetChatText("Koishi", "1");
            text[2] = ModUtils.GetChatText("Koishi", "2");
            text[3] = ModUtils.GetChatText("Koishi", "3");
            text[4] = ModUtils.GetChatText("Koishi", "4");
            text[5] = ModUtils.GetChatText("Koishi", "5");
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
            if (Projectile.owner != Main.myPlayer)
                return;

            Player player = Main.player[Projectile.owner];
            if (!player.dead && player.statLife < player.statLifeMax2 / 10
                && !player.HasBuff<SatoriBuff>() && !player.HasBuff<KomeijiBuff>())
            {
                if (mainTimer % 120 == 0 && mainTimer > 0 && Main.rand.NextBool(3) && PetState == 0 && ChatCD <= 0)
                {
                    PetState = State_Kill;
                    extraAI[0] = 0;
                    Projectile.netUpdate = true;
                }
            }
            else if (mainTimer % 720 == 0 && mainTimer > 0 && Main.rand.NextBool(7) && PetState <= 1)
            {
                SetChat(myColor);
            }
        }
        private void SetKoishiActive(Player player)
        {
            if (!player.HasBuff(BuffType<KoishiBuff>()) && !player.HasBuff(BuffType<KomeijiBuff>())
                && PetState != State_Kill || player.dead)
            {
                Projectile.velocity *= 0;
                Projectile.frame = Projectile.frame == 13 ? 13 : 0;
                Projectile.Opacity -= 0.009f;
                if (Projectile.Opacity <= 0)
                {
                    Projectile.active = false;
                    return;
                }
            }
            else if (PetState <= 1)
            {
                Projectile.Opacity += 0.009f;
            }
        }
        const int State_Kill = 4;
        public override void VisualEffectForPreview()
        {
            UpdateClothFrame();
        }
        public override void AI()
        {
            Player player = Main.player[Projectile.owner];
            Projectile.timeLeft = 2;

            UpdateTalking();
            Vector2 point = new Vector2(-54 * player.direction, -34 + player.gfxOffY);
            if (player.ownedProjectileCounts[ProjectileType<Rin>()] > 0)
                point = new Vector2(-44 * player.direction, -80 + player.gfxOffY);
            Projectile.tileCollide = false;
            Projectile.rotation = Projectile.velocity.X * 0.01f;

            ChangeDir(player, true);
            if (!player.dead)
                MoveToPoint(point, 13f);
            SetKoishiActive(player);

            if (Projectile.owner == Main.myPlayer)
            {
                if (mainTimer % 270 == 0 && PetState < 2)
                {
                    PetState = 1;
                    Projectile.netUpdate = true;
                }
                if (mainTimer >= 600 && mainTimer < 4200 && PetState == 0)
                {
                    if (mainTimer % 360 == 0 && Main.rand.NextBool(4) && extraAI[0] <= 0)
                    {
                        if (Main.rand.NextBool(2) && player.ownedProjectileCounts[ProjectileType<Satori>()] <= 0)
                        {
                            PetState = 2;
                            extraAI[1] = Main.rand.Next(1800, 3600);
                            Projectile.netUpdate = true;
                        }
                        else if (ChatTimeLeft <= 0)
                        {
                            PetState = 3;
                            extraAI[2] = Main.rand.Next(5, 14);
                            Projectile.netUpdate = true;
                        }
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
                Projectile.frame = 0;
                Fade();
            }
            else if (PetState == 3)
            {
                Annoying();
            }
            else if (PetState == State_Kill)
            {
                KillingPhoneCall();
            }
        }
    }
}


