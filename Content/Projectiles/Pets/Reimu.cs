using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria;
using Terraria.Utilities;
using TouhouPets.Content.Buffs.PetBuffs;

namespace TouhouPets.Content.Projectiles.Pets
{
    public class Reimu : BasicTouhouPetNeo
    {
        public override void SetStaticDefaults()
        {
            Main.projFrames[Type] = 18;
            Main.projPet[Type] = true;
        }
        DrawPetConfig drawConfig = new(1);
        readonly Texture2D clothTex = AltVanillaFunction.GetExtraTexture("Reimu_Cloth");
        readonly Texture2D newYearClothTex = AltVanillaFunction.GetExtraTexture("Reimu_Cloth_NewYear");
        public override bool PreDraw(ref Color lightColor)
        {
            bool isFebrary = DateTime.Now.Month == 2;
            Texture2D cloth = isFebrary ? newYearClothTex : clothTex;

            DrawPetConfig config = drawConfig with
            {
                ShouldUseEntitySpriteDraw = true,
                AltTexture = cloth,
            };

            Projectile.DrawPet(Projectile.frame, lightColor, drawConfig);

            if (PetState == 1 || PetState == 4)
                Projectile.DrawPet(blinkFrame, lightColor, drawConfig);

            Projectile.DrawPet(Projectile.frame, lightColor, config);
            Projectile.DrawStateNormalizeForPet();

            if (PetState < 3)
            {
                Projectile.DrawPet(clothFrame, lightColor, drawConfig);
                Projectile.DrawPet(clothFrame, lightColor, config);
            }
            return false;
        }
        private void Blink()
        {
            if (blinkFrame < 13)
            {
                blinkFrame = 13;
            }
            if (++blinkFrameCounter > 3)
            {
                blinkFrameCounter = 0;
                blinkFrame++;
            }
            if (blinkFrame > 15)
            {
                blinkFrame = 13;
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
        private void UpdateClothFrame()
        {
            if (clothFrame < 9)
            {
                clothFrame = 9;
            }
            int count = 5;
            if (++clothFrameCounter > count)
            {
                clothFrameCounter = 0;
                clothFrame++;
            }
            if (clothFrame > 12)
            {
                clothFrame = 9;
            }
        }
        private void Nap()
        {
            if (Projectile.frame < 1)
            {
                Projectile.frame = 1;
            }
            if (++Projectile.frameCounter > 6)
            {
                Projectile.frameCounter = 0;
                Projectile.frame++;
            }
            if (extraAI[0] == 0)
            {
                Projectile.velocity *= 0.5f;
                Projectile.frame = 1;
                if (Projectile.owner == Main.myPlayer)
                {
                    extraAI[1]++;
                    if (extraAI[1] > Main.rand.Next(320, 560))
                    {
                        extraAI[1] = 0;
                        extraAI[0] = 1;
                        Projectile.netUpdate = true;
                    }
                }
            }
            else if (extraAI[0] == 1)
            {
                Projectile.velocity *= 0.1f;
                if (Projectile.frame >= 3)
                {
                    Projectile.frame = 3;
                }
                if (Projectile.owner == Main.myPlayer)
                {
                    if (mainTimer % 320 == 0 && Main.rand.NextBool(3) && !Main.player[Projectile.owner].sleeping.FullyFallenAsleep)
                    {
                        if (Main.rand.NextBool(2))
                        {
                            extraAI[0] = 2;
                        }
                        else
                        {
                            Projectile.frame = 1;
                            extraAI[0] = 0;
                        }
                        Projectile.netUpdate = true;
                    }
                }
            }
            else if (extraAI[0] == 2)
            {
                Projectile.velocity *= 0.5f;
                if (Projectile.frame == 5)
                {
                    Projectile.frame = 6;
                }
                if (Projectile.frame > 6)
                {
                    Projectile.frame = 4;
                    extraAI[1]++;
                }
                if (Projectile.owner == Main.myPlayer)
                {
                    if (extraAI[1] > Main.rand.Next(2, 4))
                    {
                        extraAI[1] = 0;
                        extraAI[0] = 3;
                        Projectile.netUpdate = true;
                    }
                }
            }
            else if (extraAI[0] == 3)
            {
                Projectile.frame = 5;
                Projectile.velocity *= 0.75f;
                if (Projectile.owner == Main.myPlayer)
                {
                    if (extraAI[1] > Main.rand.Next(40, 60))
                    {
                        extraAI[1] = 0;
                        extraAI[0] = 4;
                        Projectile.netUpdate = true;
                    }
                    else
                    {
                        extraAI[1]++;
                    }
                }
            }
            else if (extraAI[0] == 4)
            {
                Projectile.frameCounter += 2;
                if (Projectile.frame > 8)
                {
                    Projectile.frame = 7;
                    extraAI[1]++;
                }
                if (Projectile.owner == Main.myPlayer)
                {
                    if (extraAI[1] > Main.rand.Next(6, 9))
                    {
                        extraAI[1] = 0;
                        extraAI[0] = 5;
                        Projectile.netUpdate = true;
                    }
                }
            }
            else
            {
                Projectile.frame = 0;
                extraAI[0] = 600;
                PetState = 0;
            }
        }
        private void Flying()
        {
            if (Projectile.frame < 16)
            {
                Projectile.frame = 16;
            }
            if (++Projectile.frameCounter > 3)
            {
                Projectile.frameCounter = 0;
                Projectile.frame++;
            }
            if (Projectile.frame > 17)
            {
                Projectile.frame = 16;
            }
        }
        public override Color ChatTextColor => new Color(255, 120, 120);
        public override void RegisterChat(ref string name, ref Vector2 indexRange)
        {
            name = "Reimu";
            indexRange = new Vector2(1, 11);
        }
        public override void SetRegularDialog(ref int timePerDialog, ref int chance, ref bool whenShouldStop)
        {
            timePerDialog = 840;
            chance = 6;
            whenShouldStop = PetState == 2;
        }
        public override string GetRegularDialogText()
        {
            WeightedRandom<string> chat = new WeightedRandom<string>();
            {
                if (Owner.AnyBosses())
                {
                    chat.Add(ChatDictionary[3]);
                }
                else if (Main.bloodMoon || Main.eclipse || Main.slimeRain)
                {
                    chat.Add(ChatDictionary[4]);
                    chat.Add(ChatDictionary[10]);
                }
                else
                {
                    chat.Add(ChatDictionary[1]);
                    chat.Add(ChatDictionary[2]);
                }
            }
            return chat;
        }
        public override void VisualEffectForPreview()
        {
            UpdateClothFrame();
        }
        private void UpdateTalking()
        {
        }
        int flyTimeleft = 0;
        public override void AI()
        {
            if (flyTimeleft > 0)
            {
                flyTimeleft--;
            }
            Player player = Main.player[Projectile.owner];
            Projectile.SetPetActive(player, BuffType<ReimuBuff>());
            UpdateTalking();
            Vector2 point = new Vector2(-50 * player.direction, -30 + player.gfxOffY);
            Projectile.tileCollide = false;
            Projectile.rotation = Projectile.velocity.X * 0.012f;
            if (PetState != 2)
            {
                ChangeDir(PetState < 3);
            }

            MoveToPoint(point, 22f);

            if (Projectile.owner == Main.myPlayer)
            {
                if (mainTimer % 270 == 0)
                {
                    if (PetState != 2 && PetState < 3)
                        PetState = 1;
                    else if (PetState == 3)
                        PetState = 4;
                    Projectile.netUpdate = true;
                }
                if (mainTimer >= 1200 && mainTimer < 3600 && PetState != 1 && PetState < 3
                    && !player.AnyBosses() && currentChatRoom == null)
                {
                    int chance = 11;
                    if (Main.bloodMoon || Main.eclipse)
                    {
                        chance = 30;
                    }
                    else if (Main.dayTime || Main.raining)
                    {
                        chance = 6;
                    }
                    else if (player.sleeping.FullyFallenAsleep)
                    {
                        chance = 2;
                    }
                    if (mainTimer % 240 == 0 && Main.rand.NextBool(chance) && extraAI[0] <= 0 && player.velocity.Length() == 0)
                    {
                        if (Main.rand.NextBool(8) && chatTimeLeft <= 0)
                            Projectile.SetChat(ChatSettingConfig, 11);
                        PetState = 2;
                        Projectile.netUpdate = true;
                    }
                }
            }
            if (player.velocity.Length() > 4f && PetState == 2)
            {
                PetState = 0;
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
                Nap();
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
            if (PetState == 2 && player.AnyBosses())
            {
                PetState = 0;
            }
        }
    }
}


