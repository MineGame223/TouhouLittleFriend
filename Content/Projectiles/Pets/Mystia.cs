using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;
using Terraria;
using Terraria.ID;
using Terraria.Utilities;
using TouhouPets.Content.Buffs.PetBuffs;

namespace TouhouPets.Content.Projectiles.Pets
{
    public class Mystia : BasicTouhouPet
    {
        public override void SetStaticDefaults()
        {
            Main.projFrames[Type] = 13;
            Main.projPet[Type] = true;
            ProjectileID.Sets.LightPet[Type] = false;
        }
        DrawPetConfig drawConfig = new(2);
        readonly Texture2D clothTex = AltVanillaFunction.GetExtraTexture("Mystia_Cloth");
        readonly Texture2D patchTex = AltVanillaFunction.GetExtraTexture("Mystia_EyePatch");
        public override bool PreDraw(ref Color lightColor)
        {
            bool blackDye = Main.LocalPlayer.miscDyes[0].type == ItemID.BlackDye;
            DrawPetConfig config = drawConfig with
            {
                PositionOffset = new Vector2(extraAdjX, extraAdjY),
            };

            Projectile.DrawPet(wingFrame, lightColor, config, 1);

            Projectile.DrawPet(Projectile.frame, lightColor, drawConfig);

            if (PetState == 1)
                Projectile.DrawPet(blinkFrame, lightColor, drawConfig);

            Projectile.DrawPet(Projectile.frame, lightColor,
               drawConfig with
               {
                   ShouldUseEntitySpriteDraw = true,
                   AltTexture = clothTex,
               });
            Projectile.DrawPet(clothFrame, lightColor, 
                config with
                {
                    ShouldUseEntitySpriteDraw = true,
                });
            Projectile.DrawStateNormalizeForPet();

            if (blackDye)
                Projectile.DrawPet(clothFrame, lightColor,
                    config with
                    {
                        AltTexture = patchTex,
                    });
            return false;
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
        int wingFrame, wingFrameCounter;
        int blinkFrame, blinkFrameCounter;
        int clothFrame, clothFrameCounter;
        int extraAdjX, extraAdjY;
        private void Singing()
        {
            Projectile.velocity *= 0.8f;
            if (++Projectile.frameCounter > 7)
            {
                Projectile.frameCounter = 0;
                Projectile.frame++;
            }
            if (extraAI[0] == 0)
            {
                if (Projectile.frame > 4)
                {
                    Gore.NewGoreDirect(Projectile.GetSource_FromAI(), Projectile.Center + new Vector2(0, -27), new Vector2(Main.rand.Next(-5, 5), Main.rand.Next(-6, -3)) * 0.1f, Main.rand.Next(570, 573), Main.rand.NextFloat(0.9f, 1.1f));
                    Projectile.frame = 2;
                    extraAI[1]++;
                }
                if (Projectile.owner == Main.myPlayer)
                {
                    if (extraAI[1] > extraAI[2])
                    {
                        extraAI[1] = 0;
                        extraAI[0] = 1;
                        Projectile.netUpdate = true;
                    }
                }
            }
            else
            {
                if (Projectile.frame > 5)
                {
                    Projectile.frame = 0;
                    extraAI[0] = 3600;
                    extraAI[2] = 0;
                    PetState = 0;
                }
            }
        }
        private void UpdateWingFrame()
        {
            int count = 5;
            if (++wingFrameCounter > count)
            {
                wingFrameCounter = 0;
                wingFrame++;
            }
            if (wingFrame > 5)
            {
                wingFrame = 0;
            }
        }
        private void UpdateClothFrame()
        {
            if (clothFrame < 6)
            {
                clothFrame = 6;
            }
            int count = 5;
            if (++clothFrameCounter > count)
            {
                clothFrameCounter = 0;
                clothFrame++;
            }
            if (clothFrame > 9)
            {
                clothFrame = 6;
            }
        }
        Color myColor = new Color(246, 110, 169);
        public override string GetChatText(out string[] text)
        {
            text = new string[21];
            text[1] = ModUtils.GetChatText("Mystia", "1");
            text[2] = ModUtils.GetChatText("Mystia", "2");
            text[3] = ModUtils.GetChatText("Mystia", "3");
            text[4] = ModUtils.GetChatText("Mystia", "4");
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
            int type1 = ProjectileType<Wriggle>();
            if (FindChatIndex(out Projectile _, type1, 2, default, 0))
            {
                ChatCD = 1;
            }
            if (PetState != 2)
            {
                if (FindChatIndex(out Projectile p, type1, 1))
                {
                    SetChatWithOtherOne(p, ModUtils.GetChatText("Mystia", "9"), myColor, 0, 360);
                    p.localAI[2] = 0;
                }
                else if (mainTimer % 840 == 0 && Main.rand.NextBool(6) && mainTimer > 0)
                {
                    SetChat(myColor);
                }
            }
        }
        public override void VisualEffectForPreview()
        {
            UpdateWingFrame();
            UpdateClothFrame();
        }
        public override void AI()
        {
            Player player = Main.player[Projectile.owner];
            Projectile.SetPetActive(player, BuffType<MystiaBuff>());
            UpdateTalking();
            Vector2 point = new Vector2(-50 * player.direction, -50 + player.gfxOffY);
            Projectile.tileCollide = false;
            if (PetState != 2)
                Projectile.rotation = Projectile.velocity.X * 0.035f;
            else
                Projectile.rotation = Projectile.velocity.X * 0.005f;

            ChangeDir(player, true);
            MoveToPoint(point, 14f);
            if (Projectile.owner == Main.myPlayer)
            {
                if (mainTimer % 270 == 0 && PetState != 2)
                {
                    PetState = 1;
                    Projectile.netUpdate = true;
                }
                if (mainTimer >= 1200 && mainTimer < 3600 && PetState != 1 && extraAI[0] == 0)
                {
                    if (mainTimer % 1200 == 0 && Main.rand.NextBool(3) && PetState != 2)
                    {
                        PetState = 2;
                        extraAI[2] = Main.rand.Next(60, 180);
                        Projectile.netUpdate = true;
                        int chance = Main.rand.Next(4);
                        switch (chance)
                        {
                            case 1:
                                SetChat(myColor, ModUtils.GetChatText("Mystia", "6"), 6, 90, 30, true);
                                break;
                            case 2:
                                SetChat(myColor, ModUtils.GetChatText("Mystia", "7"), 7, 90, 30, true);
                                break;
                            case 3:
                                SetChat(myColor, ModUtils.GetChatText("Mystia", "8"), 8, 90, 30, true);
                                break;
                            default:
                                SetChat(myColor, ModUtils.GetChatText("Mystia", "5"), 5, 90, 30, true);
                                break;
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
                Singing();
            }
            extraAdjX = 0;
            extraAdjY = 0;
            if (Projectile.frame >= 1 && Projectile.frame <= 5)
            {
                extraAdjY = -2;
                if (Projectile.frame >= 2 && Projectile.frame <= 4)
                {
                    extraAdjY = -4;
                    extraAdjX = -2 * Projectile.spriteDirection;
                }
            }
        }
    }
}


