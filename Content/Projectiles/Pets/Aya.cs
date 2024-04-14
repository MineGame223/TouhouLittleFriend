using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ID;
using Terraria.Utilities;
using TouhouPets.Content.Buffs.PetBuffs;

namespace TouhouPets.Content.Projectiles.Pets
{
    public class Aya : BasicTouhouPetNeo
    {
        public override void SetStaticDefaults()
        {
            Main.projFrames[Type] = 15;
            Main.projPet[Type] = true;
            ProjectileID.Sets.LightPet[Type] = false;
        }
        DrawPetConfig drawConfig = new(2);
        readonly Texture2D clothTex = AltVanillaFunction.GetExtraTexture("Aya_Cloth");
        public override bool PreDraw(ref Color lightColor)
        {
            DrawPetConfig config = drawConfig with
            {
                PositionOffset = new Vector2(extraAdjX, extraAdjY),
            };
            DrawPetConfig config2 = config with
            {
                ShouldUseEntitySpriteDraw = true,
            };

            Projectile.DrawPet(wingFrame, lightColor, config);
            Projectile.DrawPet(clothFrame + 4, lightColor,
                drawConfig with
                {
                    ShouldUseEntitySpriteDraw = true,
                }, 1);
            Projectile.DrawStateNormalizeForPet();

            Projectile.DrawPet(Projectile.frame, lightColor, drawConfig);

            if (PetState == 1)
                Projectile.DrawPet(blinkFrame, lightColor, drawConfig);

            Projectile.DrawPet(clothFrame, lightColor, config2, 1);
            Projectile.DrawPet(Projectile.frame, lightColor,
                config2 with
                {
                    AltTexture = clothTex,
                });
            Projectile.DrawPet(clothFrame + 8, lightColor, config2, 1);

            if (Projectile.frame == 3)
            {
                Projectile.DrawStateNormalizeForPet();
                DrawShotSpark();
            }
            return false;
        }
        private void DrawShotSpark()
        {
            Main.spriteBatch.QuickToggleAdditiveMode(true, Projectile.isAPreviewDummy);
            Texture2D t = AltVanillaFunction.ExtraTexture(ExtrasID.ThePerfectGlow);
            Vector2 pos = Projectile.Center + new Vector2(14 * Projectile.spriteDirection, -10) - Main.screenPosition + new Vector2(0, 7f * Main.essScale);
            Rectangle rect = new Rectangle(0, 0, t.Width, t.Height);
            Vector2 orig = rect.Size() / 2;
            Color clr = Projectile.GetAlpha(Color.White) * flash;
            SpriteEffects effect = Projectile.spriteDirection == -1 ? SpriteEffects.None : SpriteEffects.FlipHorizontally;
            Main.spriteBatch.TeaNPCDraw(t, pos, rect, clr, Projectile.rotation, orig, new Vector2(0.4f, 0.5f) * flash * 1.6f, effect, 0f);
            Main.spriteBatch.TeaNPCDraw(t, pos, rect, clr, Projectile.rotation + MathHelper.Pi / 2, orig, new Vector2(0.5f, 1f) * flash * 1.6f, effect, 0f);
            Main.spriteBatch.QuickToggleAdditiveMode(false, Projectile.isAPreviewDummy);
        }
        private void Blink()
        {
            if (blinkFrame < 6)
            {
                blinkFrame = 6;
            }
            if (++blinkFrameCounter > 3)
            {
                blinkFrameCounter = 0;
                blinkFrame++;
            }
            if (blinkFrame > 8)
            {
                blinkFrame = 6;
                PetState = 0;
            }
        }
        int wingFrame, wingFrameCounter;
        int blinkFrame, blinkFrameCounter;
        int clothFrame, clothFrameCounter;
        int extraAdjX, extraAdjY;
        float flash;
        int flashChance;
        private void Shot()
        {
            Projectile.velocity *= 0.9f;
            if (++Projectile.frameCounter > 7)
            {
                Projectile.frameCounter = 0;
                Projectile.frame++;
            }
            if (extraAI[0] > 0)
            {
                if (extraAI[1] == 0)
                {
                    if (Projectile.frame >= 3)
                    {
                        Projectile.frame = 3;
                    }
                    if (Projectile.ai[2] > 0)
                    {
                        flash = 1;
                        AltVanillaFunction.PlaySound(SoundID.Camera, Projectile.Center);
                        Projectile.ai[2] = 0;
                    }
                    if (Projectile.owner == Main.myPlayer)
                    {
                        if (extraAI[2] == 0 && Main.rand.NextBool(3))
                        {
                            SetShotChat();
                        }
                        extraAI[2]++;
                        int chance = Main.rand.Next(180, 600);
                        if (extraAI[2] >= chance)
                        {
                            extraAI[1] = 1;
                            if (extraAI[0] <= 1)
                            {
                                extraAI[2] = Main.rand.Next(30, 60);
                            }
                            Projectile.netUpdate = true;
                        }
                        else if (extraAI[2] % 30 == 0 && Main.rand.NextBool(7 - flashChance))
                        {
                            flashChance -= 2;
                            Projectile.ai[2]++;
                            Projectile.netUpdate = true;
                        }
                    }
                }
                if (extraAI[1] == 1)
                {
                    if (Projectile.frame >= 5)
                    {
                        Projectile.frame = 5;
                    }
                    if (Projectile.owner == Main.myPlayer)
                    {
                        extraAI[2]--;
                        if (extraAI[2] < 0)
                        {
                            extraAI[2] = 0;
                            extraAI[0]--;
                            flashChance = 6;
                            if (extraAI[0] > 0)
                                extraAI[1] = 2;
                            Projectile.netUpdate = true;
                        }
                    }
                }
                if (extraAI[1] == 2)
                {
                    if (Projectile.frame >= 1)
                    {
                        Projectile.frame = 1;
                        if (Projectile.owner == Main.myPlayer)
                        {
                            extraAI[1] = 0;
                            Projectile.netUpdate = true;
                        }
                    }
                }
            }
            else
            {
                if (Projectile.frame > 5)
                {
                    Projectile.frame = 0;
                    extraAI[0] = 1800;
                    extraAI[2] = 0;
                    PetState = 0;
                    Projectile.netUpdate = true;
                }
            }
        }
        private void UpdateWingFrame()
        {
            if (wingFrame < 9)
            {
                wingFrame = 9;
            }
            int count = 5;
            if (++wingFrameCounter > count)
            {
                wingFrameCounter = 0;
                wingFrame++;
            }
            if (wingFrame > 14)
            {
                wingFrame = 9;
            }
        }
        private void UpdateClothFrame()
        {
            int count = 5;
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
        public override Color ChatTextColor => new Color(255, 102, 85);
        public override void RegisterChat(ref string name, ref Vector2 indexRange)
        {
            name = "Aya";
            indexRange = new Vector2(1, 8);
        }
        public override void SetRegularDialog(ref int timePerDialog, ref int chance, ref bool whenShouldStop)
        {
            timePerDialog = 720;
            chance = 7;
            whenShouldStop = PetState == 2;
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
        private void UpdateTalking()
        {
        }
        private void SetShotChat()
        {
            int chance = Main.rand.Next(6);
            switch (chance)
            {
                case 1:
                    Projectile.SetChat(ChatSettingConfig, 5);
                    break;
                case 2:
                    Projectile.SetChat(ChatSettingConfig, 6);
                    break;
                case 3:
                    Projectile.SetChat(ChatSettingConfig, 7);
                    break;
                case 4:
                    Projectile.SetChat(ChatSettingConfig, 8);
                    break;
                default:
                    break;
            }
        }
        public override void VisualEffectForPreview()
        {
            UpdateWingFrame();
            UpdateClothFrame();
        }
        public override void AI()
        {
            if (flash > 0)
            {
                flash -= 0.1f;
            }
            Lighting.AddLight(Projectile.Center + new Vector2(14 * Projectile.spriteDirection, -10)
                , 2.55f * flash, 2.55f * flash, 2.55f * flash);
            Player player = Main.player[Projectile.owner];
            Projectile.SetPetActive(player, BuffType<AyaBuff>());
            UpdateTalking();
            Vector2 point = new Vector2(-70 * player.direction, -50 + player.gfxOffY);
            Projectile.tileCollide = false;
            if (PetState != 2)
                Projectile.rotation = Projectile.velocity.X * 0.035f;
            else
                Projectile.rotation = Projectile.velocity.X * 0.002f;

            ChangeDir(true);
            MoveToPoint(point, 30f);
            if (Projectile.owner == Main.myPlayer)
            {
                if (mainTimer % 270 == 0 && PetState != 2)
                {
                    PetState = 1;
                    Projectile.netUpdate = true;
                }
                if (mainTimer >= 1200 && mainTimer < 3600 && PetState != 1 && extraAI[0] == 0)
                {
                    if (mainTimer % 600 == 0 && Main.rand.NextBool(3) && PetState != 2)
                    {
                        PetState = 2;
                        extraAI[0] = Main.rand.Next(1, 5);
                        flashChance = 6;
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
                Shot();
            }
            extraAdjX = 0;
            extraAdjY = 0;
            if (Projectile.frame == 3)
            {
                extraAdjY = -2;
            }
        }
    }
}


