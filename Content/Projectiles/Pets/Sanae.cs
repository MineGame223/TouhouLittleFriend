using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria;
using Terraria.ID;
using Terraria.Utilities;
using TouhouPets.Content.Buffs.PetBuffs;

namespace TouhouPets.Content.Projectiles.Pets
{
    public class Sanae : BasicTouhouPet
    {
        public override void SetStaticDefaults()
        {
            Main.projFrames[Type] = 15;
            Main.projPet[Type] = true;
            ProjectileID.Sets.LightPet[Type] = true;
        }
        DrawPetConfig drawConfig = new(2);
        readonly Texture2D clothTex = AltVanillaFunction.GetExtraTexture("Sanae_Cloth");
        public override bool PreDraw(ref Color lightColor)
        {
            if (PetState == 2)
            {
                float time = Main.GlobalTimeWrappedHourly * 6f;
                Main.spriteBatch.QuickToggleAdditiveMode(true, Projectile.isAPreviewDummy);
                for (int o = 0; o < 8; o++)
                {
                    for (int i = -1; i <= 1; i++)
                    {
                        Vector2 auraPos = new Vector2(0, 2 * auraScale * i * (float)Math.Sin(time)).RotatedBy(MathHelper.PiOver2 * i);
                        DrawSanaeAura(Color.SeaGreen * 0.4f, auraPos);
                    }
                }
                Main.spriteBatch.QuickToggleAdditiveMode(false, Projectile.isAPreviewDummy);
            }
            Projectile.DrawStateNormalizeForPet();

            DrawSanae(lightColor);
            return false;
        }
        private void DrawSanae(Color lightColor)
        {
            DrawPetConfig config = drawConfig with
            {
                ShouldUseEntitySpriteDraw = true,
            };

            if (PetState < 3)
                Projectile.DrawPet(hairFrame, lightColor,
                    drawConfig with
                    {
                        PositionOffset = new Vector2(0, extraAdjY),
                    }, 1);

            Projectile.DrawPet(Projectile.frame, lightColor, drawConfig);

            if (PetState == 1 || PetState == 4)
                Projectile.DrawPet(blinkFrame, lightColor, drawConfig);

            Projectile.DrawPet(Projectile.frame, lightColor,
                config with
                {
                    AltTexture = clothTex,
                });
            Projectile.DrawStateNormalizeForPet();

            if (PetState < 3)
            {
                Projectile.DrawPet(itemFrame, lightColor, drawConfig, 1);
                Projectile.DrawPet(clothFrame, lightColor,
                    config with
                    {
                        PositionOffset = new Vector2(0, extraAdjY),
                    });
            }
        }
        private void DrawSanaeAura(Color lightColor, Vector2? posOffset = default)
        {
            Vector2 offset = posOffset ?? Vector2.Zero;
            DrawPetConfig config = drawConfig with
            {
                PositionOffset = new Vector2(0, extraAdjY) + offset,
            };
            Projectile.DrawPet(hairFrame, lightColor, config, 1);
            Projectile.DrawPet(Projectile.frame, lightColor,
                drawConfig with
                {
                    PositionOffset = offset,
                });
            Projectile.DrawPet(clothFrame, lightColor, config);
        }
        private void Blink()
        {
            if (blinkFrame < 7)
            {
                blinkFrame = 7;
            }
            if (++blinkFrameCounter > 3)
            {
                blinkFrameCounter = 0;
                blinkFrame++;
            }
            if (blinkFrame > 9)
            {
                blinkFrame = 7;
                if (PetState == 4)
                {
                    PetState = 3;
                }
                else
                {
                    PetState = 0;
                }
            }
        }
        int blinkFrame, blinkFrameCounter;
        int clothFrame, clothFrameCounter;
        int itemFrame, itemFrameCounter;
        int hairFrame, hairFrameCounter;
        float auraScale;
        int extraAdjY;
        private void UpdateClothFrame()
        {
            if (clothFrame < 10)
            {
                clothFrame = 10;
            }
            int count = PetState == 2 ? 3 : 5;
            if (++clothFrameCounter > count)
            {
                clothFrameCounter = 0;
                clothFrame++;
            }
            if (clothFrame > 13)
            {
                clothFrame = 10;
            }
        }
        private void UpdateHairFrame()
        {
            if (hairFrame < 8)
            {
                hairFrame = 8;
            }
            int count = PetState == 2 ? 3 : 6;
            if (++hairFrameCounter > count)
            {
                hairFrameCounter = 0;
                hairFrame++;
            }
            if (hairFrame > 11)
            {
                hairFrame = 8;
            }
        }
        private void UpdateItemFrame()
        {
            int count = PetState == 2 ? 3 : 7;
            if (++itemFrameCounter > count)
            {
                itemFrameCounter = 0;
                itemFrame++;
            }
            if (PetState == 2)
            {
                if (Projectile.frame > 0)
                {
                    itemFrame = Projectile.frame + 3;
                }
                return;
            }
            if (itemFrame > 3)
            {
                itemFrame = 0;
            }
        }
        private void Pray()
        {
            chatFuncIsOccupied = true;
            if (Projectile.frame < 1)
            {
                Projectile.frame = 1;
            }
            if (auraScale < 2)
            {
                auraScale += 0.02f;
            }
            if (++Projectile.frameCounter > ((Projectile.frame >= 2 && Projectile.frame <= 3) ? 3 : 6))
            {
                Projectile.frameCounter = 0;
                Projectile.frame++;
            }
            if (Projectile.frame > 3 && extraAI[1] < extraAI[2])
            {
                Projectile.frame = 2;
                extraAI[1]++;
            }
            if (Projectile.frame > 4)
            {
                Projectile.frame = 0;
                extraAI[0] = 4800;
                extraAI[1] = 0;
                extraAI[2] = 0;
                PetState = 0;
            }
        }
        private void Flying()
        {
            if (Projectile.frame < 5)
            {
                Projectile.frame = 5;
            }
            if (++Projectile.frameCounter > 3)
            {
                Projectile.frameCounter = 0;
                Projectile.frame++;
            }
            if (Projectile.frame > 6)
            {
                Projectile.frame = 5;
            }
        }
        Color myColor = new Color(83, 241, 146);
        public override string GetChatText(out string[] text)
        {
            text = new string[21];
            text[1] = ModUtils.GetChatText("Sanae", "1");
            text[2] = ModUtils.GetChatText("Sanae", "2");
            text[3] = ModUtils.GetChatText("Sanae", "3");
            text[4] = ModUtils.GetChatText("Sanae", "4");
            if (Main.IsItAHappyWindyDay)
            {
                text[5] = ModUtils.GetChatText("Sanae", "5");
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
            if (mainTimer % 960 == 0 && Main.rand.NextBool(7) && PetState != 2)
            {
                SetChat(myColor);
            }
        }
        int flyTimeleft = 0;
        public override void VisualEffectForPreview()
        {
            UpdateClothFrame();
            UpdateHairFrame();
            UpdateItemFrame();
        }
        public override void AI()
        {
            float lightPlus = 1 + auraScale * Main.essScale;
            Lighting.AddLight(Projectile.Center, 0.55f * lightPlus, 2.14f * lightPlus, 1.53f * lightPlus);
            if (flyTimeleft > 0)
            {
                flyTimeleft--;
            }
            Player player = Main.player[Projectile.owner];
            Projectile.SetPetActive(player, BuffType<SanaeBuff>());
            if (PetState != 2)
            {
                UpdateTalking();
            }
            Vector2 point = new Vector2(50 * player.direction, -30 + player.gfxOffY);
            Projectile.tileCollide = false;
            Projectile.rotation = Projectile.velocity.X * 0.012f;
            if (PetState != 2)
            {
                ChangeDir(player, PetState < 3);
                Lighting.AddLight(Projectile.Center, 0.62f, 1.02f, 0.95f);
            }

            MoveToPoint(point, 22f);

            if (Projectile.owner == Main.myPlayer)
            {
                if (mainTimer % 270 == 0)
                {
                    if (PetState == 0)
                        PetState = 1;
                    else if (PetState == 3)
                        PetState = 4;
                    Projectile.netUpdate = true;
                }
                if (mainTimer >= 1200 && mainTimer < 3600 && PetState != 1 && PetState < 3)
                {
                    if (mainTimer % 480 == 0 && Main.rand.NextBool(5) && extraAI[0] <= 0)
                    {
                        PetState = 2;
                        extraAI[2] = Main.rand.Next(120, 240);
                        Projectile.netUpdate = true;
                    }
                }
            }
            if (player.velocity.Length() > 15f)
            {
                flyTimeleft = 5;
                if (PetState < 3)
                {
                    PetState = 3;
                }
            }
            else if (flyTimeleft <= 0)
            {
                if (PetState >= 3)
                {
                    extraAI[0] = 960;
                    extraAI[1] = 0;
                    extraAI[2] = 0;
                    PetState = 0;
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
                Pray();
            }
            else if (PetState == 3)
            {
                Flying();
            }
            else if (PetState == 4)
            {
                Flying();
                Blink();
            }
            if (PetState != 2)
            {
                if (auraScale > 0)
                {
                    auraScale -= 0.01f;
                }
            }
            else
            {
                int dustType;
                switch (Main.rand.Next(4))
                {
                    default:
                        dustType = MyDustId.GreenTrans;
                        break;
                    case 1:
                        dustType = MyDustId.TrailingGreen1;
                        break;
                }
                if (Main.rand.NextBool(3))
                    Dust.NewDustDirect(Projectile.position, Projectile.width, Projectile.height, dustType,
                        0, Main.rand.Next(-3, -1), 100, default, auraScale * 0.45f).noGravity = true;
            }
            extraAdjY = 0;
            if (Projectile.frame >= 2 && Projectile.frame <= 3)
            {
                extraAdjY = -2;
            }
        }
    }
}


