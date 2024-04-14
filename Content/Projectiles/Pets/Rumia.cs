using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria;
using Terraria.ID;
using Terraria.Utilities;
using TouhouPets.Content.Buffs.PetBuffs;

namespace TouhouPets.Content.Projectiles.Pets
{
    public class Rumia : BasicTouhouPetNeo
    {
        public override void SetStaticDefaults()
        {
            Main.projFrames[Type] = 11;
            Main.projPet[Type] = true;
            ProjectileID.Sets.LightPet[Type] = false;
        }
        DrawPetConfig drawConfig = new(1);
        readonly Texture2D clothTex = AltVanillaFunction.GetExtraTexture("Rumia_Cloth");
        public override bool PreDraw(ref Color lightColor)
        {
            DrawPetConfig config = drawConfig with
            {
                ShouldUseEntitySpriteDraw = true,
            };

            Projectile.DrawPet(Projectile.frame, lightColor, drawConfig);

            if (PetState == 1)
                Projectile.DrawPet(blinkFrame, lightColor, drawConfig);

            Projectile.DrawPet(Projectile.frame, lightColor,
                config with
                {
                    AltTexture = clothTex,
                });
            Projectile.DrawPet(clothFrame, lightColor, config);

            DrawDark();
            return false;
        }
        private void DrawDark()
        {
            Texture2D tex = AltVanillaFunction.GetExtraTexture("SatoriEyeAura");
            Vector2 pos = Projectile.Center - Main.screenPosition;
            Color clr = Projectile.GetAlpha(Color.Black);
            Vector2 orig = tex.Size() / 2;
            for (int i = 0; i < 3; i++)
                Main.spriteBatch.TeaNPCDraw(tex, pos, null, clr * darkAuraScale, 0f, orig, darkAuraScale * 1.2f * Main.essScale, SpriteEffects.None, 0);
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
        int clothFrame, clothFrameCounter;
        int blinkFrame, blinkFrameCounter;
        float darkAuraScale;
        private void Darkin()
        {
            if (++Projectile.frameCounter > 7)
            {
                Projectile.frameCounter = 0;
                Projectile.frame++;
            }
            if (extraAI[0] == 0)
            {
                if (Projectile.frame >= 1 && Projectile.frameCounter > 4 || Projectile.frame >= 2)
                {
                    darkAuraScale = Math.Clamp(darkAuraScale + 0.03f, 0, 1);
                    if (darkAuraScale > 0.36f)
                        Projectile.scale -= 0.02f;
                }
                if (Projectile.frame > 2)
                {
                    Projectile.frame = 2;
                }
                if (Projectile.owner == Main.myPlayer)
                {
                    extraAI[1]++;
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
                darkAuraScale = Math.Clamp(darkAuraScale - 0.01f, 0, 1);
                if (Projectile.scale < 1)
                    Projectile.scale += 0.05f;
                if (darkAuraScale > 0.3f)
                {
                    Projectile.frame = 2;
                }
                if (Projectile.frame > 3)
                {
                    Projectile.frame = 0;
                    extraAI[0] = 1200;
                    extraAI[2] = 0;
                    PetState = 0;
                }
            }
        }
        private void UpdateClothFrame()
        {
            int count = 5;
            if (clothFrame < 7)
            {
                clothFrame = 7;
            }
            if (++clothFrameCounter > count)
            {
                clothFrameCounter = 0;
                clothFrame++;
            }
            if (clothFrame > 10)
            {
                clothFrame = 7;
            }
        }
        public override Color ChatTextBoardColor => Color.White;
        public override Color ChatTextColor => Color.Black;
        public override void RegisterChat(ref string name, ref Vector2 indexRange)
        {
            name = "Rumia";
            indexRange = new Vector2(1, 6);
        }
        public override void SetRegularDialog(ref int timePerDialog, ref int chance, ref bool whenShouldStop)
        {
            timePerDialog = 480;
            chance = 10;
            whenShouldStop = false;
        }
        public override string GetRegularDialogText()
        {
            WeightedRandom<string> chat = new WeightedRandom<string>();
            {
                if (PetState == 2)
                {
                    chat.Add(ChatDictionary[5]);
                }
                else
                {
                    chat.Add(ChatDictionary[1], 5);
                    chat.Add(ChatDictionary[2]);
                    chat.Add(ChatDictionary[3]);
                    chat.Add(ChatDictionary[4]);
                    if (Main.dayTime)
                    {
                        chat.Add(ChatDictionary[6], 3);
                    }
                }
            }
            return chat;
        }
        private void UpdateTalking()
        {
        }
        public override void VisualEffectForPreview()
        {
            UpdateClothFrame();
        }
        public override void AI()
        {
            Projectile.scale = Math.Clamp(Projectile.scale, 0, 1);

            Player player = Main.player[Projectile.owner];
            Projectile.SetPetActive(player, BuffType<RumiaBuff>());
            UpdateTalking();
            Vector2 point = new Vector2(-50 * player.direction, -40 + player.gfxOffY);
            Projectile.tileCollide = false;
            Projectile.rotation = Projectile.velocity.X * 0.035f;

            ChangeDir();
            MoveToPoint(point, 14f);
            if (Projectile.owner == Main.myPlayer)
            {
                if (mainTimer % 270 == 0 && PetState == 0)
                {
                    PetState = 1;
                    Projectile.netUpdate = true;
                }
                if (mainTimer >= 600 && mainTimer < 3600 && PetState != 2 && extraAI[0] == 0)
                {
                    if (mainTimer % 600 == 0 && Main.rand.NextBool(3))
                    {
                        PetState = 2;
                        extraAI[2] = Main.rand.Next(600, 2400);
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
                Darkin();
            }
            if (PetState != 2)
            {
                darkAuraScale = Math.Clamp(darkAuraScale - 0.1f, 0, 1);
            }
        }
    }
}


