using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ID;
using Terraria.Utilities;
using TouhouPets.Content.Buffs.PetBuffs;

namespace TouhouPets.Content.Projectiles.Pets
{
    public class Raiko : BasicTouhouPetNeo
    {
        public override void SetStaticDefaults()
        {
            Main.projFrames[Type] = 25;
            Main.projPet[Type] = true;
        }
        DrawPetConfig drawConfig = new(2);
        readonly Texture2D clothTex = AltVanillaFunction.GetExtraTexture("Raiko_Cloth");
        public override bool PreDraw(ref Color lightColor)
        {
            DrawPetConfig config = drawConfig with
            {
                ShouldUseEntitySpriteDraw = true,
            };
            DrawPetConfig config2 = config with
            {
                AltTexture = clothTex,
            };

            Projectile.DrawPet(backFrame, lightColor, config, 1);
            Projectile.DrawPet(drumFrame, lightColor, config, 1);
            Projectile.DrawStateNormalizeForPet();

            Projectile.DrawPet(legFrame, lightColor, drawConfig, 1);
            Projectile.DrawPet(legFrame, lightColor, config2, 1);
            Projectile.DrawStateNormalizeForPet();

            Projectile.DrawPet(Projectile.frame, lightColor, drawConfig);

            if (PetState == 1)
                Projectile.DrawPet(blinkFrame, lightColor, drawConfig, 1);

            Projectile.DrawPet(Projectile.frame, lightColor, config2);
            Projectile.DrawPet(skritFrame, lightColor, config, 1);
            return false;
        }
        private void Blink()
        {
            if (++blinkFrameCounter > 3)
            {
                blinkFrameCounter = 0;
                blinkFrame++;
            }
            if (blinkFrame > 3)
            {
                blinkFrame = 0;
                if (PetState == 3)
                {
                    PetState = 2;
                }
                else
                {
                    PetState = 0;
                }
            }
        }
        int blinkFrame, blinkFrameCounter;
        int backFrame, backFrameCounter;
        int legFrame, drumFrame, legFrameCounter;
        int skritFrame, skritFrameCounter;
        private void UpdateBackFrame()
        {
            if (backFrame < 16)
            {
                backFrame = 16;
            }
            if (++backFrameCounter > 40)
            {
                backFrameCounter = 0;
                backFrame++;
            }
            if (backFrame > 19)
            {
                backFrame = 16;
            }
        }
        private void UpdateSkirtFrame()
        {
            if (skritFrame < 3)
            {
                skritFrame = 3;
            }
            if (++skritFrameCounter > 5)
            {
                skritFrameCounter = 0;
                skritFrame++;
            }
            if (skritFrame > 6)
            {
                skritFrame = 3;
            }
        }
        private void UpdateLegAndDrumFrame()
        {
            if (legFrame < 7)
            {
                legFrame = 7;
            }
            if (PetState < 5)
            {
                legFrame = 7;
            }
            else
            {
                if (++legFrameCounter > 5)
                {
                    legFrameCounter = 0;
                    legFrame++;
                }
                if (legFrame == 10 && legFrameCounter == 1)
                {
                    //AltVanillaFunction.PlaySound(SoundID.DrumKick, Projectile.Center);
                }
                if (legFrame > 11)
                {
                    legFrame = 7;
                    PetState = 4;
                }
            }

            drumFrame = legFrame + 3;
            if (drumFrame < 12)
            {
                drumFrame = 12;
            }
            if (drumFrame > 14)
            {
                drumFrame = 12;
            }
        }
        private void Playing()
        {
            int count = 4;
            if (Projectile.frame == 23)
            {
                count = 20;
            }
            if (++Projectile.frameCounter > count)
            {
                Projectile.frameCounter = 0;
                Projectile.frame++;
            }
            if (Projectile.frameCounter == 1)
            {
                if (Projectile.frame == 16)
                    AltVanillaFunction.PlaySound(SoundID.DrumFloorTom, Projectile.Center);
                else if (Projectile.frame == 20)
                    AltVanillaFunction.PlaySound(SoundID.DrumHiHat, Projectile.Center);
            }
            if (extraAI[0] == 0)
            {
                if (Projectile.frame > 18)
                {
                    Projectile.frame = 14;
                    if (Projectile.owner == Main.myPlayer)
                    {
                        extraAI[0]++;
                        Projectile.netUpdate = true;
                    }
                }
            }
            else if (extraAI[0] == 1)
            {
                if (Projectile.frame > 23)
                {
                    Projectile.frame = 14;
                    extraAI[1]++;
                    if (Projectile.owner == Main.myPlayer)
                    {
                        if (extraAI[1] > extraAI[2])
                        {
                            extraAI[1] = 0;
                            extraAI[2] = 0;
                            extraAI[0]++;
                            Projectile.netUpdate = true;
                        }
                        else
                        {
                            extraAI[0] = 0;
                            Projectile.netUpdate = true;
                        }
                        if (extraAI[1] <= extraAI[2] && Main.rand.NextBool(3) && PetState == 4)
                        {
                            PetState = 5;
                            Projectile.netUpdate = true;
                        }
                    }
                }
            }
            else
            {
                if (Projectile.frame > 24)
                {
                    Projectile.frame = 0;
                    extraAI[0] = 1200;
                    PetState = 0;
                }
            }
        }
        private void Idel()
        {
            int count = 7;
            if (Projectile.frame == 0)
            {
                count = 120;
            }
            if (PetState >= 2)
            {
                count = 5;
            }
            if (++Projectile.frameCounter > count)
            {
                Projectile.frameCounter = 0;
                Projectile.frame++;
            }
            if (Projectile.frame > (PetState >= 2 ? 10 : 3))
            {
                Projectile.frame = 0;
                if (PetState >= 2)
                {
                    PetState = 0;
                }
            }
        }
        public override Color ChatTextColor => new Color(249, 101, 101);
        public override void RegisterChat(ref string name, ref Vector2 indexRange)
        {
            name = "Raiko";
            indexRange = new Vector2(1, 3);
        }
        public override void SetRegularDialog(ref int timePerDialog, ref int chance, ref bool whenShouldStop)
        {
            timePerDialog = 555;
            chance = 9;
            whenShouldStop = PetState >= 4;
        }
        public override string GetRegularDialogText()
        {
            WeightedRandom<string> chat = new WeightedRandom<string>();
            {
                chat.Add(ChatDictionary[1]);
                chat.Add(ChatDictionary[2]);
                chat.Add(ChatDictionary[3]);
            }
            return chat;
        }
        private void UpdateTalking()
        {
        }
        public override void VisualEffectForPreview()
        {
            UpdateBackFrame();
            UpdateSkirtFrame();
            UpdateLegAndDrumFrame();
            if (PetState <= 3)
            {
                Idel();
            }
        }
        private void ControlMovement(Player player)
        {
            Projectile.tileCollide = false;
            Projectile.rotation = Projectile.velocity.X * 0.005f;

            ChangeDir();

            Vector2 point = new Vector2(-60 * player.direction, -40 + player.gfxOffY);
            MoveToPoint(point, 12.5f);
        }
        public override void AI()
        {
            Player player = Main.player[Projectile.owner];
            Projectile.SetPetActive(player, BuffType<RaikoBuff>());

            UpdateTalking();
            ControlMovement(player);

            if (Projectile.owner == Main.myPlayer)
            {
                if (mainTimer % 270 == 0)
                {
                    if (PetState == 0)
                        PetState = 1;
                    else if (PetState == 2)
                        PetState = 3;

                    Projectile.netUpdate = true;
                }
                if (mainTimer >= 600 && mainTimer < 3600 && PetState < 1)
                {
                    if (mainTimer % 720 == 0 && Main.rand.NextBool(2) && extraAI[0] <= 0)
                    {
                        if (Main.rand.NextBool(4))
                        {
                            extraAI[2] = Main.rand.Next(10, 30);
                            PetState = 4;
                        }
                        else
                            PetState = 2;
                        Projectile.netUpdate = true;
                    }
                }
            }
            if (PetState <= 3)
            {
                if (PetState == 1 || PetState == 3)
                {
                    Blink();
                }
                if (extraAI[0] >= 1 && PetState <= 1)
                {
                    extraAI[0]--;
                }
            }
            else if (PetState == 4 || PetState == 5)
            {
                Playing();
            }
        }
    }
}


