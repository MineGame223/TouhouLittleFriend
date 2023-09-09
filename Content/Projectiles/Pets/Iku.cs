using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria;
using Terraria.GameContent.Events;
using Terraria.ID;
using Terraria.Utilities;
using TouhouPets.Content.Buffs.PetBuffs;

namespace TouhouPets.Content.Projectiles.Pets
{
    public class Iku : BasicTouhouPet
    {
        public override void SetStaticDefaults()
        {
            Main.projFrames[Type] = 15;
            Main.projPet[Type] = true;
            ProjectileID.Sets.LightPet[Type] = true;
        }
        public override bool PreDraw(ref Color lightColor)
        {
            DrawIku(clothFrame, lightColor, true, null, true);
            Projectile.DrawStateNormalizeForPet();
            DrawIku(Projectile.frame, lightColor);
            if (PetState == 1)
                DrawIku(blinkFrame, lightColor);
            DrawIku(Projectile.frame, lightColor, false, AltVanillaFunction.GetExtraTexture("Iku_Cloth"), true);
            return false;
        }
        private void DrawIku(int frame, Color lightColor, bool belt = false, Texture2D tex = null, bool entitySpriteDraw = false)
        {
            Texture2D t = tex ?? AltVanillaFunction.ProjectileTexture(Type);
            int height = t.Height / Main.projFrames[Type];
            Vector2 pos = Projectile.Center - Main.screenPosition + new Vector2(0, 7f * Main.essScale);
            int columns = 0;
            if (belt)
            {
                pos += new Vector2(0, 3f * Main.essScale);
                columns = 1;
            }
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
            if (blinkFrame < 12)
            {
                blinkFrame = 12;
            }
            if (++blinkFrameCounter > 3)
            {
                blinkFrameCounter = 0;
                blinkFrame++;
            }
            if (blinkFrame > 14)
            {
                blinkFrame = 12;
                PetState = 0;
            }
        }
        int blinkFrame, blinkFrameCounter;
        int clothFrame, clothFrameCounter;
        private void Spinning()
        {
            if (Projectile.frame < 4)
            {
                Projectile.frame = 4;
            }
            if (extraAI[0] == 0)
            {
                if (++Projectile.frameCounter > 2)
                {
                    Projectile.frameCounter = 0;
                    Projectile.frame++;
                }
                if (Projectile.frame > 7)
                {
                    Projectile.frame = 6;
                    extraAI[1]++;
                }
                if (extraAI[1] > Main.rand.Next(7, 14))
                {
                    extraAI[1] = 0;
                    extraAI[0] = 1;
                }
            }
            else if (extraAI[0] == 1)
            {
                if (++Projectile.frameCounter > 4)
                {
                    Projectile.frameCounter = 0;
                    Projectile.frame++;
                }
                if (Projectile.frame > 10)
                {
                    Projectile.frame = 9;
                    extraAI[1]++;
                }
                if (extraAI[1] > Main.rand.Next(30, 50))
                {
                    extraAI[1] = 0;
                    extraAI[0] = 2;
                }
                if (Projectile.frame >= 9)
                {
                    Dust.NewDustDirect(Projectile.Center + new Vector2(-16 * Projectile.spriteDirection, -10)
                        , 1, 1, Main.rand.NextBool(2) ? MyDustId.TrailingCyan : MyDustId.ElectricCyan,
                        Main.rand.Next(-4, 4), Main.rand.Next(-6, -1)
                        , 100, Color.White, Main.rand.NextFloat(0.3f, 1.2f)).noGravity = false;
                }
            }
            else
            {
                if (++Projectile.frameCounter > 6)
                {
                    Projectile.frameCounter = 0;
                    Projectile.frame++;
                }
                if (Projectile.frame > 11)
                {
                    Projectile.frame = 0;
                    extraAI[0] = 1200;
                    PetState = 0;
                }
            }
        }
        private void Idel()
        {
            if (++Projectile.frameCounter > 4)
            {
                Projectile.frameCounter = 0;
                Projectile.frame++;
            }
            if (Projectile.frame > 3)
            {
                Projectile.frame = 0;
            }
        }
        private void UpdateMiscFrame()
        {
            if (clothFrame < 0)
            {
                clothFrame = 0;
            }
            if (++clothFrameCounter > 6)
            {
                clothFrameCounter = 0;
                clothFrame++;
            }
            if (clothFrame > 0)
            {
                clothFrame = 0;
            }
        }
        Color myColor = new Color(79, 215, 239);
        public override string GetChatText(out string[] text)
        {
            Player player = Main.player[Projectile.owner];
            text = new string[11];
            WeightedRandom<string> chat = new WeightedRandom<string>();
            {
                if (Sandstorm.Happening && player.ZoneDesert && player.ZoneOverworldHeight)
                {
                    chat.Add("风沙肆虐...");
                }
                else if (Main.slimeRain)
                {
                    chat.Add("粘稠生物从天而降");
                }
                else if (Main.bloodMoon)
                {
                    chat.Add("月亮被诅咒侵蚀，鲜血染红大地...");
                }
                else if (Main.eclipse)
                {
                    chat.Add("太阳被月亮遮蔽，妖魔四处横行...");
                }
                else if (!Main.eclipse && !Main.bloodMoon)
                {
                    if (Main.cloudAlpha == 0 && Math.Abs(Main.windSpeedTarget) <= 0.1f)
                    {
                        chat.Add("气候并没有什么特别的变化");
                    }
                    else if (Main.cloudAlpha == 0 && Math.Abs(Main.windSpeedTarget) > 0.1f
                       && Math.Abs(Main.windSpeedTarget) < 0.25f)
                    {
                        chat.Add("和煦的微风吹拂着大地");
                    }
                    else if (Main.cloudAlpha == 0 && Math.Abs(Main.windSpeedTarget) > .25f
                       && Math.Abs(Main.windSpeedTarget) < 0.4f)
                    {
                        chat.Add("今天的风甚是清爽");
                    }
                    else if (Main.cloudAlpha == 0 && Math.Abs(Main.windSpeedTarget) >= .4f)
                    {
                        chat.Add("风在奔涌");
                    }
                    if (Main.cloudAlpha >= 0.4f && Math.Abs(Main.windSpeedTarget) >= 0.3f
                        && Main.cloudAlpha < 0.5f && Math.Abs(Main.windSpeedTarget) < 0.4f)
                    {
                        chat.Add("风暴将至...");
                    }
                    else if (Main.cloudAlpha >= 0.5f && Math.Abs(Main.windSpeedTarget) >= 0.4f
                        && Main.cloudAlpha < 0.8f && Math.Abs(Main.windSpeedTarget) < 0.65f)
                    {
                        chat.Add("猛烈的风雨正在席卷大地");
                    }
                    else if (Main.cloudAlpha >= 0.8f && Math.Abs(Main.windSpeedTarget) >= 0.65f)
                    {
                        chat.Add("一定是龙神在发怒了...");
                    }
                    else if (!player.AnyBosses())
                    {
                        chat.Add("希望今天也是相安无事的一天");
                        if (BirthdayParty.PartyIsUp)
                        {
                            chat.Add("世界充满着欢快的气氛");
                        }
                        if (LanternNight.LanternsUp)
                        {
                            chat.Add("和平的气息从四周升起");
                        }
                    }
                }
            }
            return chat;
        }
        private void UpdateTalking()
        {
            int type1 = ProjectileType<Tenshi>();
            if (FindChatIndex(out Projectile _, type1, 2, default, 0))
            {
                ChatCD = 1;
            }
            if (FindChatIndex(out Projectile p, type1, 2))
            {
                SetChatWithOtherOne(p, "天女大人您还是安分点吧...", myColor, 0, 360);
                p.ai[0] = 0;
            }
            else if (mainTimer % 720 == 0 && Main.rand.NextBool(9) && PetState != 2)
            {
                SetChat(myColor);
            }
        }
        public override void VisualEffectForPreview()
        {
            if (PetState != 2)
            {
                Idel();
            }
        }
        public override void AI()
        {
            Lighting.AddLight(Projectile.Center, 0.79f, 2.15f, 2.39f);
            Player player = Main.player[Projectile.owner];
            Projectile.SetPetActive(player, BuffType<IkuBuff>());
            UpdateMiscFrame();
            UpdateTalking();
            Vector2 point = new Vector2(45 * player.direction, -45 + player.gfxOffY);
            if (player.ownedProjectileCounts[ProjectileType<Tenshi>()] > 0)
            {
                point = new Vector2(-45 * player.direction, -45 + player.gfxOffY);
            }
            Projectile.tileCollide = false;
            Projectile.rotation = Projectile.velocity.X * 0.012f;

            ChangeDir(player, player.ownedProjectileCounts[ProjectileType<Tenshi>()] > 0);
            MoveToPoint(point, 15f);

            if (mainTimer % 270 == 0 && PetState != 2)
            {
                PetState = 1;
            }
            if (mainTimer >= 1200 && mainTimer < 3600 && PetState != 1)
            {
                if (mainTimer % 600 == 0 && Main.rand.NextBool(4) && extraAI[0] <= 0)
                {
                    PetState = 2;
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
            else if (PetState == 2)
            {
                Spinning();
            }
        }
    }
}


