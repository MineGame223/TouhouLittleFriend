using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ID;
using Terraria.Utilities;
using TouhouPets.Content.Buffs.PetBuffs;

namespace TouhouPets.Content.Projectiles.Pets
{
    public class Kogasa : BasicTouhouPet
    {
        public override void SetStaticDefaults()
        {
            Main.projFrames[Type] = 20;
            Main.projPet[Type] = true;
        }
        public override bool PreDraw(ref Color lightColor)
        {
            int eyeFramePlus = Projectile.spriteDirection == -1 ? 0 : 3;
            DrawKogasa(umbrellaFrame, lightColor, 1);
            DrawKogasa(umbrellaFrame, lightColor, 1, AltVanillaFunction.GetExtraTexture("Kogasa_Cloth"), true);
            Projectile.DrawStateNormalizeForPet();
            DrawKogasa(Projectile.frame, lightColor);

            if (PetState == 1 || PetState == 4)
                DrawKogasa(blinkFrame, lightColor);
            if (PetState == 0 || PetState == 3)
                DrawKogasa(14 + eyeFramePlus, lightColor);
            if (Projectile.frame == 1 || Projectile.frame == 4)
                DrawKogasa(15 + eyeFramePlus, lightColor);

            DrawKogasa(Projectile.frame, lightColor, 0, AltVanillaFunction.GetExtraTexture("Kogasa_Cloth"), true);
            DrawKogasa(clothFrame, lightColor, 0, null, true);
            Projectile.DrawStateNormalizeForPet();
            DrawKogasa(handFrame, lightColor);
            DrawKogasa(handFrame, lightColor, 0, AltVanillaFunction.GetExtraTexture("Kogasa_Cloth"), true);
            return false;
        }
        private void DrawKogasa(int frame, Color lightColor, int columns = 0, Texture2D tex = null, bool entitySpriteDraw = false)
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
            if (blinkFrame < 14)
            {
                blinkFrame = 14;
            }
            if (++blinkFrameCounter > 3)
            {
                blinkFrameCounter = 0;
                blinkFrame++;
            }
            if (blinkFrame > 16)
            {
                blinkFrame = 14;
                if (PetState == 4)
                    PetState = 3;
                else
                    PetState = 0;
            }
        }
        int blinkFrame, blinkFrameCounter;
        int handFrame, handFrameCounter;
        int clothFrame, clothFrameCounter;
        int umbrellaFrame, umbrellaFrameCounter;
        private void UpdateUmbrellaFrame()
        {
            if (++umbrellaFrameCounter > 5)
            {
                umbrellaFrameCounter = 0;
                umbrellaFrame++;
                if (PetState == 3 || PetState == 4)
                {
                    extraAI[1]++;
                }
            }
            if (PetState <= 2)
            {
                if (umbrellaFrame > 6)
                {
                    umbrellaFrame = 0;
                }
            }
            else
            {
                if (PetState >= 5)
                {
                    umbrellaFrame = 14;
                    return;
                }
                if (umbrellaFrame < 7)
                {
                    umbrellaFrame = 7;
                }
                if (umbrellaFrame > 13)
                {
                    umbrellaFrame = 7;
                }
            }
        }
        private void UpdateHandFrame()
        {
            if (PetState >= 5)
            {
                handFrame = 13;
                return;
            }
            if (PetState <= 1)
            {
                handFrame = 10;
            }
            else if (PetState == 3 || PetState == 4)
            {
                if (handFrame < 11)
                {
                    handFrame = 11;
                }
                if (++handFrameCounter > 5)
                {
                    handFrameCounter = 0;
                    handFrame++;
                }
                if (handFrame > 12)
                {
                    handFrame = 11;
                }
            }
        }
        private void UpdateClothFrame()
        {
            if (clothFrame < 6)
            {
                clothFrame = 6;
            }
            if (++clothFrameCounter > 5)
            {
                clothFrameCounter = 0;
                clothFrame++;
            }
            if (clothFrame > 9)
            {
                clothFrame = 6;
            }
        }
        private void Scare()
        {
            if (Projectile.frame < 2)
            {
                Projectile.frame = 2;
            }
            if (++Projectile.frameCounter > 7)
            {
                Projectile.frameCounter = 0;
                Projectile.frame++;
                extraAI[1]++;
            }
            extraAI[0] = (extraAI[1] > extraAI[2]) ? 1 : 0;
            if (extraAI[0] == 1)
            {
                if (Projectile.frame > 4)
                {
                    extraAI[1] = 0;
                    extraAI[2] = 0;
                    extraAI[0] = 1200;
                    Projectile.frame = 0;
                    PetState = 0;
                }
            }
            else
            {
                if (Projectile.frame > 3)
                {
                    Projectile.frame = 2;
                }
            }
        }
        private void SpingUmbrella()
        {
            if (extraAI[1] > extraAI[2])
            {
                if (umbrellaFrame == 7)
                {
                    extraAI[1] = 0;
                    extraAI[2] = 0;
                    extraAI[0] = 1200;
                    PetState = 0;
                }
            }
        }
        Color myColor = new Color(172, 69, 191);
        public override string GetChatText(out string[] text)
        {
            text = new string[21];
            text[1] = ModUtils.GetChatText("Kogasa", "1");
            text[2] = ModUtils.GetChatText("Kogasa", "2");
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
            if (mainTimer % 720 == 0 && Main.rand.NextBool(9) && PetState != 5)
            {
                SetChat(myColor);
            }
        }
        public override void VisualEffectForPreview()
        {
            UpdateClothFrame();
            UpdateUmbrellaFrame();
            UpdateHandFrame();
        }
        public override void AI()
        {
            Player player = Main.player[Projectile.owner];
            Projectile.SetPetActive(player, BuffType<KogasaBuff>());

            UpdateTalking();
            Vector2 point = new Vector2(-50 * player.direction, -30 + player.gfxOffY);
            Projectile.tileCollide = false;
            Projectile.rotation = Projectile.velocity.X * 0.012f;

            ChangeDir(player, true);
            MoveToPoint(point, 12.5f);

            if (mainTimer % 270 == 0)
            {
                if (PetState == 0)
                    PetState = 1;
                else if (PetState == 3)
                    PetState = 4;
            }
            if (mainTimer >= 1200 && mainTimer < 3600 && PetState < 1)
            {
                if (mainTimer % 120 == 0 && Main.rand.NextBool(2) && extraAI[0] <= 0)
                {
                    if (Main.rand.NextBool(2))
                    {
                        PetState = 3;
                        extraAI[2] = Main.rand.Next(30, 40);
                    }
                    else
                    {
                        PetState = 2;
                        extraAI[2] = Main.rand.Next(14, 20);
                    }
                }
            }
            if (PetState == 0)
            {
                if (extraAI[0] >= 1)
                {
                    extraAI[0]--;
                }
                Projectile.frame = 0;
            }
            else if (PetState == 1 || PetState == 4)
            {
                Projectile.frame = 0;
                Blink();
            }
            else if (PetState == 2)
            {
                Scare();
            }
            else if (PetState == 3 || PetState == 4)
            {
                Projectile.frame = 0;
                SpingUmbrella();
            }
            else if (PetState == 5)
            {
                chatFuncIsOccupied = true;
                Projectile.frame = 5;
                if (!player.AnyBosses())
                    PetState = 0;
            }
            if (player.AnyBosses())
            {
                PetState = 5;
            }
        }
    }
}


