using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ID;
using Terraria.Utilities;
using TouhouPets.Content.Buffs.PetBuffs;

namespace TouhouPets.Content.Projectiles.Pets
{
    public class Tenshi : BasicTouhouPet
    {
        public override void SetStaticDefaults()
        {
            Main.projFrames[Type] = 14;
            Main.projPet[Type] = true;
            ProjectileID.Sets.LightPet[Type] = false;
        }
        DrawPetConfig drawConfig = new(2);
        readonly Texture2D clothTex = AltVanillaFunction.GetExtraTexture("Tenshi_Cloth");
        public override bool PreDraw(ref Color lightColor)
        {
            DrawPetConfig config = drawConfig with
            {
                ShouldUseEntitySpriteDraw = true,
                AltTexture = clothTex,
            };

            Projectile.DrawPet(stoneFrame, lightColor, drawConfig, 1);
            Projectile.DrawPet(stoneFrame, lightColor, config, 1);
            Projectile.DrawStateNormalizeForPet();

            Projectile.DrawPet(clothFrame + 4, lightColor, drawConfig, 1);
            Projectile.DrawPet(clothFrame + 4, lightColor, config, 1);
            Projectile.DrawStateNormalizeForPet();

            Projectile.DrawPet(Projectile.frame, lightColor, drawConfig);

            if (PetState == 1)
                Projectile.DrawPet(blinkFrame, lightColor, drawConfig);

            Projectile.DrawPet(Projectile.frame, lightColor, config);
            Projectile.DrawPet(clothFrame, lightColor, 
                drawConfig with
                {
                    ShouldUseEntitySpriteDraw= true,
                }, 1);
            return false;
        }
        private void Blink()
        {
            if (blinkFrame < 11)
            {
                blinkFrame = 11;
            }
            if (++blinkFrameCounter > 3)
            {
                blinkFrameCounter = 0;
                blinkFrame++;
            }
            if (blinkFrame > 13)
            {
                blinkFrame = 11;
                PetState = 0;
            }
        }
        int stoneFrame, stoneFrameCounter;
        int blinkFrame, blinkFrameCounter;
        int clothFrame, clothFrameCounter;
        private void EatingPeach()
        {
            if (Projectile.frame < 1)
            {
                Projectile.frame = 1;
            }
            if (++Projectile.frameCounter > 7)
            {
                Projectile.frameCounter = 0;
                Projectile.frame++;
            }
            if (extraAI[0] == 0)
            {
                if (Projectile.frame > 8)
                {
                    Projectile.frame = 7;
                    extraAI[1]++;
                }
                if (Projectile.owner == Main.myPlayer)
                {
                    if (extraAI[1] > Main.rand.Next(3, 6))
                    {
                        extraAI[1] = 0;
                        extraAI[0] = 1;
                        Projectile.netUpdate = true;
                    }
                }
            }
            else if (extraAI[0] == 1)
            {
                if (Projectile.frame > 9)
                {
                    Projectile.frame = 5;
                    extraAI[0] = 2;
                }
            }
            else if (extraAI[0] == 2)
            {
                Projectile.frame = 5;
                if (Projectile.owner == Main.myPlayer)
                {
                    extraAI[1]++;
                    if (extraAI[1] > Main.rand.Next(180, 360))
                    {
                        extraAI[1] = 0;
                        extraAI[2]++;
                        if (extraAI[2] > Main.rand.Next(3, 6))
                        {
                            extraAI[2] = 0;
                            extraAI[0] = 3;
                        }
                        else
                        {
                            extraAI[0] = 0;
                        }
                        Projectile.netUpdate = true;
                    }
                }
            }
            else
            {
                if (Projectile.frame == 9)
                    Projectile.frame = 10;
                if (Projectile.frame > 10)
                {
                    Projectile.frame = 0;
                    extraAI[1] = 0;
                    extraAI[2] = 0;
                    extraAI[0] = 1800;
                    PetState = 0;
                }
            }
        }
        private void UpdateStoneFrame()
        {
            if (stoneFrame < 8)
            {
                stoneFrame = 8;
            }
            int count = 6;
            if (++stoneFrameCounter > count)
            {
                stoneFrameCounter = 0;
                stoneFrame++;
            }
            if (stoneFrame > 10)
            {
                stoneFrame = 8;
            }
        }
        private void UpdateClothAndHairFrame()
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
        Color myColor = new Color(69, 170, 234);
        public override string GetChatText(out string[] text)
        {
            text = new string[21];
            text[1] = ModUtils.GetChatText("Tenshin", "1");
            text[2] = ModUtils.GetChatText("Tenshin", "2");
            text[3] = ModUtils.GetChatText("Tenshin", "3");
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
            if (mainTimer % 720 == 0 && Main.rand.NextBool(7))
            {
                SetChat(myColor);
            }
        }
        public override void VisualEffectForPreview()
        {
            UpdateStoneFrame();
            UpdateClothAndHairFrame();
        }
        public override void AI()
        {
            Player player = Main.player[Projectile.owner];
            Projectile.SetPetActive(player, BuffType<TenshiBuff>());

            UpdateTalking();
            Vector2 point = new Vector2(-50 * player.direction, -30 + player.gfxOffY);
            if (player.ownedProjectileCounts[ProjectileType<Iku>()] > 0)
            {
                point = new Vector2(50 * player.direction, -30 + player.gfxOffY);
            }
            Projectile.tileCollide = false;
            Projectile.rotation = Projectile.velocity.X * 0.005f;

            ChangeDir(player, player.ownedProjectileCounts[ProjectileType<Iku>()] <= 0);
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
                    if (mainTimer % 860 == 0 && Main.rand.NextBool(6) && extraAI[0] <= 0 && player.velocity.Length() < 4f)
                    {
                        PetState = 2;
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
                EatingPeach();
            }
        }
    }
}


