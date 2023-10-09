using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.Utilities;
using TouhouPets.Content.Buffs.PetBuffs;

namespace TouhouPets.Content.Projectiles.Pets
{
    /// <summary>
    /// 假面骑士赤蛮骑
    /// </summary>
    public class Sekibanki : BasicTouhouPet
    {
        public override void SetStaticDefaults()
        {
            Main.projFrames[Type] = 17;
            Main.projPet[Type] = true;
        }
        public override bool PreDraw(ref Color lightColor)
        {
            Vector2 headPosAdj = new Vector2(0, -2 * headBaseY) + new Vector2(headAdjX, headAdjY);
            DrawSekibanki(headFrame, lightColor, 1, headPosAdj);
            DrawSekibanki(headFrame, lightColor, 1, headPosAdj, AltVanillaFunction.GetExtraTexture("Sekibanki_Cloth"), true);
            Projectile.DrawStateNormalizeForPet();
            if (PetState == 1 || extraAI[2] > 0)
                DrawSekibanki(blinkFrame, lightColor, 1, headPosAdj);
            DrawSekibanki(Projectile.frame, lightColor, 0);
            DrawSekibanki(Projectile.frame, lightColor, 0, default, AltVanillaFunction.GetExtraTexture("Sekibanki_Cloth"), true);
            return false;
        }
        private void DrawSekibanki(int frame, Color lightColor, int columns = 0, Vector2 extraPos = default, Texture2D tex = null, bool entitySpriteDraw = false)
        {
            if (extraPos == default)
                extraPos = Vector2.Zero;
            Texture2D t = tex ?? AltVanillaFunction.ProjectileTexture(Type);
            int height = t.Height / Main.projFrames[Type];
            Vector2 pos = Projectile.Center - Main.screenPosition + new Vector2(0, 7f * Main.essScale) + extraPos;
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
            if (blinkFrame < 11)
            {
                blinkFrame = 11;
            }
            if (++blinkFrameCounter > 3)
            {
                blinkFrameCounter = 0;
                blinkFrame++;
            }
            if (blinkFrame > 12)
            {
                blinkFrame = 11;
                PetState = 0;
            }
        }
        private void Idle()
        {
            if (++Projectile.frameCounter > 7)
            {
                Projectile.frameCounter = 0;
                Projectile.frame++;
            }
            if (Projectile.frame > 4)
            {
                Projectile.frame = 0;
            }
        }
        private void Pose()
        {
            if (++Projectile.frameCounter > 6)
            {
                Projectile.frameCounter = 0;
                Projectile.frame++;
            }
            if (extraAI[0] == 0)
            {
                if (Projectile.frame == 6)
                    Projectile.frame = 9;
                if (Projectile.frame > 13)
                {
                    Projectile.frame = 11;
                    extraAI[1]++;
                }
                if (Projectile.owner == Main.myPlayer)
                {
                    if (extraAI[1] > Main.rand.Next(5, 10))
                    {
                        extraAI[1] = 0;
                        extraAI[0] = 1;
                        Projectile.netUpdate = true;
                    }
                }
            }
            else
            {
                if (Projectile.frame > 16)
                {
                    Projectile.frame = 0;
                    PetState = 0;
                    extraAI[0] = 3600;
                }
            }
        }
        private void Henshin()
        {
            if (++Projectile.frameCounter > 4)
            {
                Projectile.frameCounter = 0;
                Projectile.frame++;
            }
            if (extraAI[0] == 0)
            {
                if (Projectile.frame > 8)
                {
                    Projectile.frame = 7;
                }
            }
            else
            {
                if (Projectile.frame == 10)
                    Projectile.frame = 15;
                if (Projectile.frame > 16)
                {
                    Projectile.frame = 0;
                    PetState = 0;
                }
            }
        }
        int blinkFrame, blinkFrameCounter;
        int headFrame, headFrameCounter, headBaseY, headAdjX, headAdjY;
        private void UpdateHeadFrame()
        {
            if (++headFrameCounter > 6)
            {
                headFrameCounter = 0;
                headFrame++;
            }
            if (PetState == 2 && Projectile.frame >= 6)
            {
                if (headFrame > 10)
                {
                    headFrame = 5;
                }
            }
            else
            {
                if (headFrame > 5)
                {
                    if (headFrame > 10)
                    {
                        headFrame = 0;
                    }
                }
                else if (headFrame > 4)
                {
                    headFrame = 0;
                }
            }
        }
        private void UpdateHeadPosition()
        {
            if (PetState != 2)
            {
                if (headBaseY > 0 && headFrame < 5)
                    headBaseY -= 2;
            }
            else
            {
                if (headBaseY < 15 && headFrame >= 5)
                    headBaseY++;
            }
            if (headBaseY < 0)
            {
                headBaseY = 0;
            }
            headAdjX = 0;
            headAdjY = 0;
            if (Projectile.frame >= 5 && Projectile.frame <= 16)
            {
                headAdjY = -2;
                if (Projectile.frame >= 10 && Projectile.frame <= 15)
                {
                    headAdjX = -2 * Projectile.spriteDirection;
                }
            }
        }
        Color myColor = new Color(255, 105, 105);
        public override string GetChatText(out string[] text)
        {
            text = new string[21];
            text[1] = ModUtils.GetChatText("Sekibanki", "1");
            text[2] = ModUtils.GetChatText("Sekibanki", "2");
            if (talkInterval <= 0)
                text[3] = ModUtils.GetChatText("Sekibanki", "3");
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
            if (Projectile.owner != Main.myPlayer)
                return;

            if (FindChainedChat(1) && PetState <= 1)
            {
                if (Main.rand.NextBool(4) && extraAI[0] <= 0)
                {
                    extraAI[0] = 0;
                    Projectile.netUpdate = true;
                    SetChatWithOtherOne(null, ModUtils.GetChatText("Sekibanki", "7"), myColor, 7, 600);
                }
                else
                    ChatIndex = 0;
            }
            else if (FindChainedChat(7))
            {
                if (PetState != 3)
                {
                    PetState = 3;
                    Projectile.netUpdate = true;
                }
                SetChatWithOtherOne(null, ModUtils.GetChatText("Sekibanki", "8"), myColor, 8, 600, -1, 30, true);
            }
            else if (FindChainedChat(8))
            {
                SetChatWithOtherOne(null, ModUtils.GetChatText("Sekibanki", "9"), myColor, 9, 600, -1, 40);
            }
            else if (FindChainedChat(9))
            {
                if (extraAI[0] < 10800)
                {
                    extraAI[0] = 10800;
                    Projectile.netUpdate = true;
                }
                SetChatWithOtherOne(null, ModUtils.GetChatText("Sekibanki", "10"), myColor, 0, 600);
            }
            if (FindChainedChat(3))
            {
                SetChatWithOtherOne(null, ModUtils.GetChatText("Sekibanki", "4"), myColor, 4, 600, -1, 9);
            }
            else if (FindChainedChat(4))
            {
                SetChatWithOtherOne(null, ModUtils.GetChatText("Sekibanki", "5"), myColor, 5, 600, -1, 9);
            }
            else if (FindChainedChat(5))
            {
                SetChatWithOtherOne(null, ModUtils.GetChatText("Sekibanki", "6"), myColor, 0, 360);
                talkInterval = 3600;
            }
            else if (mainTimer % 960 == 0 && Main.rand.NextBool(7))
            {
                SetChat(myColor);
            }
        }
        public override void VisualEffectForPreview()
        {
            UpdateHeadFrame();
            if (PetState <= 1)
            {
                Idle();
            }
        }
        public override void AI()
        {
            Player player = Main.player[Projectile.owner];
            Projectile.SetPetActive(player, BuffType<SekibankiBuff>());
            UpdateTalking();
            Vector2 point = new Vector2(-40 * player.direction, -30 + player.gfxOffY);
            Projectile.tileCollide = false;
            Projectile.rotation = Projectile.velocity.X * 0.032f;

            ChangeDir(player, true);
            MoveToPoint(point, 7f);

            if (Projectile.owner == Main.myPlayer)
            {
                if (mainTimer % 270 == 0 && PetState == 0)
                {
                    PetState = 1;
                    Projectile.netUpdate = true;
                }
                if (mainTimer >= 1200 && mainTimer < 3600 && PetState == 0)
                {
                    if (mainTimer % 555 == 0 && Main.rand.NextBool(7) && extraAI[0] <= 0)
                    {
                        PetState = 2;
                        Projectile.netUpdate = true;
                    }
                }
            }
            if (PetState <= 1)
            {
                if (extraAI[2] > 0)
                {
                    PetState = 0;
                    extraAI[2]--;
                }
                if (PetState == 1)
                {
                    Blink();
                }
                if (extraAI[0] > 0)
                {
                    extraAI[0]--;
                }
            }
            else if (PetState == 2)
            {
                Pose();
            }
            else if (PetState == 3)
            {
                Henshin();
            }
            if (extraAI[2] > 0)
            {
                blinkFrame = 12;
            }
            UpdateHeadPosition();
        }
    }
}


