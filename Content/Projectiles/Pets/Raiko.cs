using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ID;
using Terraria.Utilities;
using TouhouPets.Content.Buffs.PetBuffs;

namespace TouhouPets.Content.Projectiles.Pets
{
    public class Raiko : BasicTouhouPet
    {
        public override void SetStaticDefaults()
        {
            Main.projFrames[Type] = 25;
            Main.projPet[Type] = true;
        }
        public override bool PreDraw(ref Color lightColor)
        {
            DrawRaiko(backFrame, lightColor, 1, null, true);
            DrawRaiko(drumFrame, lightColor, 1, null, true);
            Projectile.DrawStateNormalizeForPet();

            DrawRaiko(legFrame, lightColor, 1);
            DrawRaiko(legFrame, lightColor, 1, AltVanillaFunction.GetExtraTexture("Raiko_Cloth"), true);
            Projectile.DrawStateNormalizeForPet();

            DrawRaiko(Projectile.frame, lightColor);
            if (PetState == 1)
                DrawRaiko(blinkFrame, lightColor, 1);

            DrawRaiko(Projectile.frame, lightColor, 0, AltVanillaFunction.GetExtraTexture("Raiko_Cloth"), true);
            DrawRaiko(skritFrame, lightColor, 1, null, true);
            return false;
        }
        private void DrawRaiko(int frame, Color lightColor, int columns = 0, Texture2D tex = null, bool entitySpriteDraw = false)
        {
            Texture2D t = tex ?? AltVanillaFunction.ProjectileTexture(Type);
            int height = t.Height / Main.projFrames[Type];
            Vector2 pos = Projectile.Center - Main.screenPosition + new Vector2(0, 7f * Main.essScale);
            Rectangle rect = new Rectangle(t.Width / 2 * columns, frame * height, t.Width / 2, height);
            Vector2 orig = rect.Size() / 2;
            SpriteEffects effect = Projectile.spriteDirection == -1 ? SpriteEffects.None : SpriteEffects.FlipHorizontally;
            if (entitySpriteDraw)
                Main.EntitySpriteDraw(t, pos, rect, Projectile.GetAlpha(lightColor), Projectile.rotation, orig, Projectile.scale, effect, 0f);
            else
                Main.spriteBatch.TeaNPCDraw(t, pos, rect, Projectile.GetAlpha(lightColor), Projectile.rotation, orig, Projectile.scale, effect, 0f);
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
        Color myColor = new Color(249, 101, 101);
        public override string GetChatText(out string[] text)
        {
            text = new string[21];
            text[1] = ModUtils.GetChatText("Raiko", "1");
            text[2] = ModUtils.GetChatText("Raiko", "2");
            text[3] = ModUtils.GetChatText("Raiko", "3");
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
            if (mainTimer % 555 == 0 && Main.rand.NextBool(9) && PetState != 2)
            {
                SetChat(myColor);
            }
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

            ChangeDir(player, true);

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
                            extraAI[2] = Main.rand.Next(40, 80);
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


