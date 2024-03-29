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
        DrawPetConfig drawConfig = new(2);
        readonly Texture2D clothTex = AltVanillaFunction.GetExtraTexture("Iku_Cloth");
        public override bool PreDraw(ref Color lightColor)
        {
            Projectile.DrawPet(clothFrame, lightColor,
                drawConfig with
                {
                    PositionOffset = new Vector2(0, 3f * Main.essScale),
                    ShouldUseEntitySpriteDraw = true,
                }, 1);
            Projectile.DrawStateNormalizeForPet();

            Projectile.DrawPet(Projectile.frame, lightColor, drawConfig);

            if (PetState == 1)
                Projectile.DrawPet(blinkFrame, lightColor, drawConfig);

            Projectile.DrawPet(Projectile.frame, lightColor,
                drawConfig with
                {
                    AltTexture = clothTex,
                    ShouldUseEntitySpriteDraw = true,
                });
            return false;
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
                if (Projectile.owner == Main.myPlayer)
                {
                    if (extraAI[1] > extraAI[2])
                    {
                        extraAI[1] = 0;
                        extraAI[0] = 1;
                        extraAI[2] = Main.rand.Next(30, 50);
                        Projectile.netUpdate = true;
                    }
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
                if (Projectile.owner == Main.myPlayer)
                {
                    if (extraAI[1] > extraAI[2])
                    {
                        extraAI[1] = 0;
                        extraAI[0] = 2;
                        Projectile.netUpdate = true;
                    }
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
        private void UpdateClothFrame()
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
                if (Main.bloodMoon)
                {
                    chat.Add(ModUtils.GetChatText("Iku", "1"));
                }
                if (Main.eclipse)
                {
                    chat.Add(ModUtils.GetChatText("Iku", "2"));
                }
                if (!Main.eclipse && !Main.bloodMoon)
                {
                    if (Sandstorm.Happening && player.ZoneDesert && player.ZoneOverworldHeight)
                    {
                        chat.Add(ModUtils.GetChatText("Iku", "3"));
                    }
                    else if (Main.slimeRain)
                    {
                        chat.Add(ModUtils.GetChatText("Iku", "4"));
                    }
                    else if (!player.AnyBosses())
                    {
                        chat.Add(ModUtils.GetChatText("Iku", "5"));
                        if (BirthdayParty.PartyIsUp)
                        {
                            chat.Add(ModUtils.GetChatText("Iku", "6"));
                        }
                        if (LanternNight.LanternsUp)
                        {
                            chat.Add(ModUtils.GetChatText("Iku", "7"));
                        }
                    }
                    if (Main.cloudAlpha == 0 && Math.Abs(Main.windSpeedTarget) <= 0.1f)
                    {
                        chat.Add(ModUtils.GetChatText("Iku", "8"));
                    }
                    if (Main.cloudAlpha == 0 && Math.Abs(Main.windSpeedTarget) > 0.1f
                       && Math.Abs(Main.windSpeedTarget) < 0.25f)
                    {
                        chat.Add(ModUtils.GetChatText("Iku", "9"));
                    }
                    if (Main.cloudAlpha == 0 && Math.Abs(Main.windSpeedTarget) > .25f
                       && Math.Abs(Main.windSpeedTarget) < 0.4f)
                    {
                        chat.Add(ModUtils.GetChatText("Iku", "10"));
                    }
                    if (Main.cloudAlpha == 0 && Math.Abs(Main.windSpeedTarget) >= .4f)
                    {
                        chat.Add(ModUtils.GetChatText("Iku", "11"));
                    }
                    if (Main.cloudAlpha > 0f && Main.cloudAlpha < 0.3f)
                    {
                        chat.Add(ModUtils.GetChatText("Iku", "12"));
                    }
                    if (Main.cloudAlpha >= 0.4f && Math.Abs(Main.windSpeedTarget) >= 0.3f
                        && Main.cloudAlpha < 0.5f && Math.Abs(Main.windSpeedTarget) < 0.4f)
                    {
                        chat.Add(ModUtils.GetChatText("Iku", "13"));
                    }
                    if (Main.cloudAlpha >= 0.5f && Math.Abs(Main.windSpeedTarget) >= 0.4f
                        && Main.cloudAlpha < 0.8f && Math.Abs(Main.windSpeedTarget) < 0.65f)
                    {
                        chat.Add(ModUtils.GetChatText("Iku", "14"));
                    }
                    if (Main.cloudAlpha >= 0.8f && Math.Abs(Main.windSpeedTarget) >= 0.65f)
                    {
                        chat.Add(ModUtils.GetChatText("Iku", "15"));
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
                SetChatWithOtherOne(p, ModUtils.GetChatText("Iku", "16"), myColor, 0, 360);
                p.localAI[2] = 0;
            }
            else if (mainTimer % 720 == 0 && Main.rand.NextBool(3) && PetState != 2)
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
            UpdateClothFrame();
        }
        public override void AI()
        {
            Lighting.AddLight(Projectile.Center, 0.79f, 2.15f, 2.39f);
            Player player = Main.player[Projectile.owner];
            Projectile.SetPetActive(player, BuffType<IkuBuff>());
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
                        extraAI[2] = Main.rand.Next(7, 14);
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


