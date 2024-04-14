using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ID;
using Terraria.Utilities;
using TouhouPets.Content.Buffs.PetBuffs;

namespace TouhouPets.Content.Projectiles.Pets
{
    public class Mystia : BasicTouhouPetNeo
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
        public override Color ChatTextColor => new Color(246, 110, 169);
        public override void RegisterChat(ref string name, ref Vector2 indexRange)
        {
            name = "Mystia";
            indexRange = new Vector2(1, 9);
        }
        public override void SetRegularDialog(ref int timePerDialog, ref int chance, ref bool whenShouldStop)
        {
            timePerDialog = 840;
            chance = 6;
            whenShouldStop = PetState > 1;
        }
        public override string GetRegularDialogText()
        {
            WeightedRandom<string> chat = new WeightedRandom<string>();
            {
                chat.Add(ChatDictionary[1]);
                chat.Add(ChatDictionary[2]);
                chat.Add(ChatDictionary[3]);
                chat.Add(ChatDictionary[4]);
            }
            return chat;
        }
        public override void VisualEffectForPreview()
        {
            UpdateWingFrame();
            UpdateClothFrame();
        }
        private void UpdateTalking()
        {
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

            ChangeDir();
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
                                Projectile.SetChat(ChatSettingConfig, 5, 90);
                                break;
                            case 2:
                                Projectile.SetChat(ChatSettingConfig, 6, 90);
                                break;
                            case 3:
                                Projectile.SetChat(ChatSettingConfig, 7, 90);
                                break;
                            default:
                                Projectile.SetChat(ChatSettingConfig, 8, 90);
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


