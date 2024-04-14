using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ID;
using Terraria.Utilities;
using TouhouPets.Content.Buffs.PetBuffs;

namespace TouhouPets.Content.Projectiles.Pets
{
    public class Reisen : BasicTouhouPetNeo
    {
        public override void SetStaticDefaults()
        {
            Main.projFrames[Type] = 12;
            Main.projPet[Type] = true;
            ProjectileID.Sets.LightPet[Type] = false;
        }
        DrawPetConfig drawConfig = new(3);
        readonly Texture2D eyeTex = AltVanillaFunction.GetGlowTexture("Reisen_Glow");
        readonly Texture2D clothTex = AltVanillaFunction.GetExtraTexture("Reisen_Cloth");
        readonly Texture2D altClothTex = AltVanillaFunction.GetExtraTexture("Reisen_Cloth_Alt");
        public override bool PreDraw(ref Color lightColor)
        {
            if (eyeScale > 0)
            {
                Color clr = Projectile.GetAlpha(Color.Red * (1 - eyeScale) * 0.3f);
                clr.A *= 0;
                DrawReisen(clr, 1 + eyeScale * 1.1f);
                for (int i = -1; i <= 1; i++)
                {
                    if (i == 0)
                        continue;
                    DrawReisen(clr, 1f, new Vector2(4 * i, 0));
                    DrawReisen(clr, 1f, new Vector2(0, 4 * i));
                }
            }

            DrawReisen(lightColor);

            if (eyeScale > 0)
            {
                DrawPetConfig config = drawConfig with
                {
                    Scale = 1 + eyeScale * 2,
                    AltTexture = eyeTex,
                };
                Color clr = Projectile.GetAlpha(Color.White * (0.9f - eyeScale));
                clr.A *= 0;
                Projectile.DrawPet(Projectile.frame, clr, config, 0);
            }
            return false;
        }
        private void DrawReisen(Color lightColor, float scale = 1, Vector2? posOffset = default)
        {
            bool blackDye = Main.LocalPlayer.miscDyes[0].type == ItemID.BlackDye;
            DrawPetConfig config = drawConfig with
            {
                Scale = scale,
                PositionOffset = posOffset ?? Vector2.Zero,
            };
            DrawPetConfig config2 = config with
            {
                AltTexture = blackDye ? altClothTex : clothTex,
                ShouldUseEntitySpriteDraw = !blackDye,
            };

            DrawPetConfig config3 = config with
            {
                AltTexture = eyeTex,
            };

            Projectile.DrawPet(hairFrame, lightColor, config, 1);
            Projectile.DrawPet(earFrame, lightColor, config, 0);

            Projectile.DrawPet(legFrame, lightColor, config, 2);
            Projectile.DrawPet(legFrame, lightColor, config2, 2);
            Projectile.DrawStateNormalizeForPet();

            Projectile.DrawPet(Projectile.frame, lightColor, config);
            Projectile.DrawPet(Projectile.frame, Projectile.GetAlpha(Color.White * 0.6f), config3);

            if (PetState == 1)
            {
                Projectile.DrawPet(blinkFrame, lightColor, config, 1);
                Projectile.DrawPet(blinkFrame, Projectile.GetAlpha(Color.White * 0.6f), config3, 1);
            }
            Projectile.DrawPet(Projectile.frame, lightColor, config2, 0);
            Projectile.DrawPet(clothFrame, lightColor, config2, 1);
        }
        private void Blink()
        {
            if (++blinkFrameCounter > 3)
            {
                blinkFrameCounter = 0;
                blinkFrame++;
            }
            if (blinkFrame > 2)
            {
                blinkFrame = 0;
                PetState = 0;
            }
        }
        int earFrame, earFrameCounter;
        int blinkFrame, blinkFrameCounter;
        int clothFrame, clothFrameCounter;
        int hairFrame, hairFrameCounter;
        int legFrame, legFrameCounter;
        float eyeScale = 1;
        private void UpdateMiscFrame()
        {
            if (clothFrame < 3)
            {
                clothFrame = 3;
            }
            int count = 6;
            if (++clothFrameCounter > count)
            {
                clothFrameCounter = 0;
                clothFrame++;
            }
            if (clothFrame > 6)
            {
                clothFrame = 3;
            }

            if (PetState < 3)
            {
                if (earFrame < 7)
                {
                    earFrame = 7;
                }
                count = 6;
                if (++earFrameCounter > count)
                {
                    earFrameCounter = 0;
                    earFrame++;
                }
                if (earFrame > 10)
                {
                    earFrame = 7;
                }
            }

            if (hairFrame < 7)
            {
                hairFrame = 7;
            }
            count = 6;
            if (++hairFrameCounter > count)
            {
                hairFrameCounter = 0;
                hairFrame++;
            }
            if (hairFrame > 10)
            {
                hairFrame = 7;
            }

            if (hairFrame < 7)
            {
                hairFrame = 7;
            }
            count = 7;
            if (++legFrameCounter > count)
            {
                legFrameCounter = 0;
                legFrame++;
            }
            if (legFrame > 3)
            {
                legFrame = 0;
            }
        }
        private void Shooting()
        {
            int count = 9;

            if (++Projectile.frameCounter > count)
            {
                Projectile.frameCounter = 0;
                Projectile.frame++;
            }
            if (extraAI[0] == 0)
            {
                if (Projectile.frame >= 2 && extraAI[1] > 0)
                {
                    eyeScale += 0.05f;
                    if (eyeScale > 1)
                    {
                        eyeScale = 0;
                    }
                    Lighting.AddLight(Projectile.Center, 0.3f * eyeScale, 0, 0);
                    Projectile.frame = 2;
                }
                if (Projectile.owner == Main.myPlayer)
                {
                    extraAI[1]--;
                    if (extraAI[1] <= 0)
                    {
                        extraAI[1] = 0;
                        extraAI[0]++;
                        Projectile.netUpdate = true;
                    }
                }
            }
            else
            {
                if (eyeScale < 1)
                    eyeScale += 0.1f;
                if (Projectile.frame > 4)
                {
                    Projectile.frame = 0;
                    PetState = 0;
                    extraAI[0] = 900;
                }
            }
        }
        private void Nerves()
        {
            earFrame = 11;
            extraAI[1]++;
            if (extraAI[0] == 0)
            {
                Projectile.frame = 5;
                if (extraAI[1] > 270)
                {
                    Projectile.frame = 6;
                    if (Projectile.owner == Main.myPlayer)
                    {
                        extraAI[1] = 0;
                        extraAI[0]++;
                        Projectile.netUpdate = true;
                    }
                }
            }
            else if (extraAI[1] > 3)
            {
                extraAI[1] = 0;
                extraAI[0] = 0;
                Projectile.frame = 5;
            }
        }
        public override Color ChatTextColor => new Color(255, 10, 10);
        public override void RegisterChat(ref string name, ref Vector2 indexRange)
        {
            name = "Reisen";
            indexRange = new Vector2(1, 11);
        }
        public override void SetRegularDialog(ref int timePerDialog, ref int chance, ref bool whenShouldStop)
        {
            timePerDialog = 610;
            chance = 8;
            whenShouldStop = PetState > 1;
        }
        public override string GetRegularDialogText()
        {
            WeightedRandom<string> chat = new WeightedRandom<string>();
            {
                for (int j = 1; j <= 9; j++)
                {
                    chat.Add(ChatDictionary[j]);
                }
                if (Main.bloodMoon)
                {
                    chat.Add(ChatDictionary[10]);
                }
            }
            return chat;
        }
        Color myColor = new Color(255, 10, 10);
        private void UpdateTalking()
        {
        }
        public override void VisualEffectForPreview()
        {
            UpdateMiscFrame();
            if (PetState < 2)
                Projectile.frame = 0;
        }
        public override void AI()
        {
            Player player = Main.player[Projectile.owner];
            Projectile.SetPetActive(player, BuffType<ReisenBuff>());
            UpdateTalking();
            Vector2 point = new Vector2(-50 * player.direction, -40 + player.gfxOffY);
            Projectile.tileCollide = false;
            Projectile.rotation = Projectile.velocity.X * 0.014f;

            ChangeDir();
            MoveToPoint(point, 13f);

            if (Projectile.owner == Main.myPlayer && PetState == 0)
            {
                if (mainTimer % 270 == 0)
                {
                    PetState = 1;
                    Projectile.netUpdate = true;
                }
                if (mainTimer >= 600 && mainTimer < 3600)
                {
                    if (mainTimer % 600 == 0 && Main.rand.NextBool(5) && extraAI[0] <= 0)
                    {
                        extraAI[1] = Main.rand.Next(300, 600);
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
                eyeScale = 1;
            }
            else if (PetState == 1)
            {
                Blink();
            }
            else if (PetState == 2)
            {
                Shooting();
            }
            else if (PetState == 3)
            {
                Nerves();
            }
            if (PetState <= 1 && player.ownedProjectileCounts[ProjectileType<Junko>()] > 0)
            {
                PetState = 3;
            }
            if (PetState == 3 && player.ownedProjectileCounts[ProjectileType<Junko>()] <= 0)
            {
                PetState = 0;
                if (Projectile.owner == Main.myPlayer)
                {
                    extraAI[1] = 0;
                    extraAI[0] = 0;
                    Projectile.netUpdate = true;
                }
            }
        }
    }
}


