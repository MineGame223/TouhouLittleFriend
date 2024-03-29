using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;
using Terraria;
using Terraria.ID;
using Terraria.Utilities;
using TouhouPets.Content.Buffs.PetBuffs;

namespace TouhouPets.Content.Projectiles.Pets
{
    public class Alice : BasicTouhouPet
    {
        public override void SetStaticDefaults()
        {
            Main.projFrames[Type] = 13;
            Main.projPet[Type] = true;
            ProjectileID.Sets.LightPet[Type] = false;
        }
        DrawPetConfig drawConfig = new(2);
        readonly Texture2D clothTex = AltVanillaFunction.GetExtraTexture("Alice_Cloth");
        public override bool PreDraw(ref Color lightColor)
        {
            DrawPetConfig config = drawConfig with
            {
                ShouldUseEntitySpriteDraw = true,
            };

            DrawAura();
            Projectile.DrawStateNormalizeForPet();

            Projectile.DrawPet(Projectile.frame, lightColor, drawConfig);

            if (PetState == 1)
                Projectile.DrawPet(blinkFrame, lightColor, drawConfig);

            Projectile.DrawPet(clothFrame, lightColor, config, 1);
            Projectile.DrawPet(Projectile.frame, lightColor,
                config with
                {
                    AltTexture = clothTex,
                });
            return false;
        }
        private void DrawAura()
        {
            DrawPetConfig config = drawConfig with
            {
                ShouldUseEntitySpriteDraw = true,
            };
            Main.spriteBatch.QuickToggleAdditiveMode(true, Projectile.isAPreviewDummy);
            for (int i = 0; i < 8; i++)
            {
                Vector2 spinningpoint = new Vector2(0f, -1f);
                Projectile.DrawPet(auraFrame, Projectile.GetAlpha(Color.White) * 0.4f,
                    config with
                    {
                        PositionOffset = spinningpoint.RotatedBy(MathHelper.TwoPi * Main.GlobalTimeWrappedHourly
                        + MathHelper.TwoPi / 8 * i * 0.6f)
                    }
                    , 1);
            }
            Main.spriteBatch.QuickToggleAdditiveMode(false, Projectile.isAPreviewDummy);
            Projectile.DrawStateNormalizeForPet();

            Projectile.DrawPet(auraFrame, Projectile.GetAlpha(Color.White) * 0.4f, config, 1);
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
                Projectile.netUpdate = true;
            }
        }
        int auraFrame, auraFrameCounter;
        int blinkFrame, blinkFrameCounter;
        int clothFrame, clothFrameCounter;
        private void Puppetia()
        {
            Projectile.velocity *= 0.8f;
            if (++Projectile.frameCounter > 7)
            {
                Projectile.frameCounter = 0;
                Projectile.frame++;
            }
            if (extraAI[0] == 0)
            {
                if (Projectile.frame == 2 && Projectile.frameCounter % 4 == 0 && Projectile.frameCounter <= 7)
                {
                    float posX = Main.rand.NextFloat(Main.rand.NextFloat(-210f, -90f), Main.rand.NextFloat(90f, 210f));
                    float posY = Main.rand.NextFloat(Main.rand.NextFloat(-210f, -90f), Main.rand.NextFloat(90f, 210f));
                    if (Projectile.owner == Main.myPlayer)
                    {
                        Projectile.NewProjectile(Projectile.GetSource_FromThis(), Projectile.Center
                            , new Vector2(Main.rand.NextFloat(-8f, 8f), Main.rand.NextFloat(-8f, 8f)), ProjectileType<AliceDoll_Proj>(), 0, 0
                            , Main.myPlayer, Projectile.whoAmI, posX, posY);
                    }
                }
                if (Projectile.frame > 3)
                {
                    Projectile.frame = 3;
                }
                if (Projectile.owner == Main.myPlayer)
                {
                    extraAI[1]++;
                    if (extraAI[1] > extraAI[2])
                    {
                        extraAI[1] = 0;
                        extraAI[0]++;
                        extraAI[2] = Main.rand.Next(400, 600);
                        Projectile.netUpdate = true;
                    }
                }
            }
            else if (extraAI[0] == 1)
            {
                if (Projectile.frame == 6 && Projectile.frameCounter % 4 == 0 && Projectile.frameCounter <= 7)
                {
                    float posX = Main.rand.NextFloat(Main.rand.NextFloat(-210f, -90f), Main.rand.NextFloat(90f, 210f));
                    float posY = Main.rand.NextFloat(Main.rand.NextFloat(-210f, -90f), Main.rand.NextFloat(90f, 210f));
                    if (Projectile.owner == Main.myPlayer)
                    {
                        Projectile.NewProjectile(Projectile.GetSource_FromThis(), Projectile.Center
                            , new Vector2(Main.rand.NextFloat(-8f, 8f), Main.rand.NextFloat(-8f, 8f)), ProjectileType<AliceDoll_Proj>(), 0, 0
                            , Main.myPlayer, Projectile.whoAmI, posX, posY);
                    }
                }
                if (Projectile.frame > 7)
                {
                    Projectile.frame = 7;
                }
                if (Projectile.owner == Main.myPlayer)
                {
                    extraAI[1]++;
                    if (extraAI[1] > extraAI[2])
                    {
                        extraAI[1] = 0;
                        extraAI[0]++;
                        Projectile.netUpdate = true;
                    }
                }
            }
            else
            {
                if (Projectile.frame > 9)
                {
                    Projectile.frame = 0;
                    extraAI[0] = 240;
                    extraAI[2] = 0;
                    PetState = 0;
                }
            }
        }
        private void UpdateAuraFrame()
        {
            int count = 3;
            if (auraFrame < 4)
            {
                auraFrame = 4;
            }
            if (++auraFrameCounter > count)
            {
                auraFrameCounter = 0;
                auraFrame++;
            }
            if (auraFrame > 7)
            {
                auraFrame = 4;
            }
        }
        private void UpdateClothFrame()
        {
            int count = 6;
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
        Color myColor = new Color(185, 228, 255);
        public override string GetChatText(out string[] text)
        {
            text = new string[21];
            text[1] = ModUtils.GetChatText("Alice", "1");
            text[2] = ModUtils.GetChatText("Alice", "2");
            text[3] = ModUtils.GetChatText("Alice", "3");
            if (FindPetState(out Projectile _, ProjectileType<Marisa>(), 0))
            {
                text[4] = ModUtils.GetChatText("Alice", "4");
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
            int type = ProjectileType<Marisa>();
            int type2 = ProjectileType<Patchouli>();
            if (FindChatIndex(out Projectile _, type2, 12, default, 0))
            {
                ChatCD = 1;
            }
            if (FindChatIndex(out Projectile p1, type, 12, default, 1, true))
            {
                SetChatWithOtherOne(p1, ModUtils.GetChatText("Alice", "5"), myColor, 5, 600);
            }
            else if (FindChatIndex(out Projectile p2, type, 13, default, 1, true))
            {
                SetChatWithOtherOne(p2, ModUtils.GetChatText("Alice", "6"), myColor, 6, 600);
            }
            else if (FindChatIndex(out Projectile p3, type, 14, default, 1, true))
            {
                SetChatWithOtherOne(p3, ModUtils.GetChatText("Alice", "7"), myColor, 0, 360);
                p3.localAI[2] = 0;
            }
            else if (FindChatIndex(out Projectile p4, type2, 12, default, 1, true))
            {
                SetChatWithOtherOne(p4, ModUtils.GetChatText("Alice", "8"), myColor, 8, 600, -1, 12);
            }
            else if (FindChatIndex(out Projectile p5, type2, 13, default, 1, true))
            {
                SetChatWithOtherOne(p5, ModUtils.GetChatText("Alice", "9"), myColor, 9, 600);
            }
            else if (FindChatIndex(out Projectile p6, type2, 14, default, 1, true))
            {
                SetChatWithOtherOne(p6, ModUtils.GetChatText("Alice", "10"), myColor, 10, 360, -1, 20, false, 80);
            }
            if (mainTimer % 1200 == 0 && Main.rand.NextBool(4) && mainTimer > 0)
            {
                if (PetState <= 1)
                    SetChat(myColor);
            }
        }
        public override void VisualEffectForPreview()
        {
            UpdateAuraFrame();
            UpdateClothFrame();
        }
        public override void AI()
        {
            Player player = Main.player[Projectile.owner];
            Projectile.SetPetActive(player, BuffType<AliceBuff>());
            UpdateTalking();
            Vector2 point = new Vector2(-50 * player.direction, -40 + player.gfxOffY);
            Projectile.tileCollide = false;
            Projectile.rotation = Projectile.velocity.X * 0.006f;

            ChangeDir(player, true);
            MoveToPoint(point, 12f);
            if (Projectile.owner == Main.myPlayer)
            {
                if (mainTimer % 270 == 0 && PetState == 0)
                {
                    PetState = 1;
                    Projectile.netUpdate = true;
                }
                if (mainTimer >= 600 && mainTimer < 3600 && PetState <= 1 && extraAI[0] == 0)
                {
                    if (mainTimer % 600 == 0 && Main.rand.NextBool(12) && Projectile.velocity.Length() <= 3f)
                    {
                        PetState = 2;
                        extraAI[2] = Main.rand.Next(400, 600);
                        Projectile.netUpdate = true;
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
                Puppetia();
            }
        }
    }
}


