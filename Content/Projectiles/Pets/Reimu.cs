using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.Utilities;
using TouhouPets.Content.Buffs.PetBuffs;

namespace TouhouPets.Content.Projectiles.Pets
{
    public class Reimu : BasicTouhouPet
    {
        public override void SetStaticDefaults()
        {
            Main.projFrames[Type] = 18;
            Main.projPet[Type] = true;
        }
        public override bool PreDraw(ref Color lightColor)
        {
            DrawReimu(Projectile.frame, lightColor);
            if (PetState == 1 || PetState == 4)
                DrawReimu(blinkFrame, lightColor);
            DrawReimu(Projectile.frame, lightColor, AltVanillaFunction.GetExtraTexture("Reimu_Cloth"), true);
            Projectile.DrawStateNormalizeForPet();
            if (PetState < 3)
            {
                DrawReimu(clothFrame, lightColor);
                DrawReimu(clothFrame, lightColor, AltVanillaFunction.GetExtraTexture("Reimu_Cloth"), true);
            }
            return false;
        }
        private void DrawReimu(int frame, Color lightColor, Texture2D tex = null, bool entitySpriteDraw = false)
        {
            Texture2D t = tex ?? AltVanillaFunction.ProjectileTexture(Type);
            int height = t.Height / Main.projFrames[Type];
            Vector2 pos = Projectile.Center - Main.screenPosition + new Vector2(0, 7f * Main.essScale);
            Rectangle rect = new Rectangle(0, frame * height, t.Width, height);
            Vector2 orig = rect.Size() / 2;
            SpriteEffects effect = Projectile.spriteDirection == -1 ? SpriteEffects.None : SpriteEffects.FlipHorizontally;
            if (entitySpriteDraw)
                Main.EntitySpriteDraw(t, pos, rect, Projectile.GetAlpha(lightColor), Projectile.rotation, orig, Projectile.scale, effect, 0f);
            else
                Main.spriteBatch.TeaNPCDraw(t, pos, rect, Projectile.GetAlpha(lightColor), Projectile.rotation, orig, Projectile.scale, effect, 0f);
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
            chatFuncIsOccupied = true;
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
                extraAI[1]++;
                if (extraAI[1] > Main.rand.Next(60, 180))
                {
                    extraAI[1] = 0;
                    extraAI[0] = 1;
                }
            }
            else if (extraAI[0] == 1)
            {
                Projectile.velocity *= 0.1f;
                if (Projectile.frame >= 3)
                {
                    Projectile.frame = 3;
                }
                if (mainTimer % 240 == 0 && Main.rand.NextBool(3) && !Main.player[Projectile.owner].sleeping.FullyFallenAsleep)
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
                if (extraAI[1] > Main.rand.Next(2, 4))
                {
                    extraAI[1] = 0;
                    extraAI[0] = 3;
                }
            }
            else if (extraAI[0] == 3)
            {
                Projectile.frame = 5;
                Projectile.velocity *= 0.75f;
                if (extraAI[1] > Main.rand.Next(40, 60))
                {
                    extraAI[1] = 0;
                    extraAI[0] = 4;
                }
                else
                {
                    extraAI[1]++;
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
                if (extraAI[1] > Main.rand.Next(6, 9))
                {
                    extraAI[1] = 0;
                    extraAI[0] = 5;
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
        Color myColor = new Color(255, 120, 120);
        public override string GetChatText(out string[] text)
        {
            Player player = Main.player[Projectile.owner];
            text = new string[21];
            if (player.AnyBosses())
            {
                text[8] = "说麻烦麻烦就到...";
            }
            else if (Main.bloodMoon || Main.eclipse || Main.slimeRain)
            {
                text[3] = "啧，看来有“异变”出现了...";
            }
            else
            {
                text[1] = "今天也很闲呢...";
                text[2] = "谁可以给我投点赛钱啊...";
            }
            WeightedRandom<string> chat = new WeightedRandom<string>();
            {
                for (int i = 1; i < text.Length; i++)
                {
                    if (text[i] != null)
                    {
                        int weight = 1;
                        if (i == 3)
                        {
                            weight = 10;
                        }
                        chat.Add(text[i], weight);
                    }
                }
            }
            return chat;
        }
        private void UpdateTalking()
        {
            int type2 = ProjectileType<Marisa>();
            int type3 = ProjectileType<Sanae>();
            if (FindChatIndex(out Projectile _, type2, 6, default, 0)
                || FindChatIndex(out Projectile _, type2, 10, default, 0)
                || FindChatIndex(out Projectile _, type3, 4, default, 0))
            {
                ChatCD = 1;
            }
            if (FindChatIndex(out Projectile p0, type2, 10))
            {
                SetChatWithOtherOne(p0, "这是“异变”啊...", myColor, 0, 600);
                p0.ai[0] = 0;
                talkInterval = 1200;
            }
            else if (FindChatIndex(out Projectile p1, type2, 6, default, 1, true))
            {
                SetChatWithOtherOne(p1, "...咋了？", myColor, 4, 600);
                extraAI[0] = 600;
            }
            else if (FindChatIndex(out Projectile p2, type2, 7, default, 1, true))
            {
                SetChatWithOtherOne(p2, "是什么？不会又是从图书馆偷来的书吧...", myColor, 5, 600, -1, 9);
                extraAI[0] = 600;
            }
            else if (FindChatIndex(out Projectile p3, type2, 8, default, 1, true))
            {
                SetChatWithOtherOne(p3, "...那东西不是一到夜晚满地都是么？", myColor, 6, 360, -1, 9);
                extraAI[0] = 600;
            }
            else if (FindChatIndex(out Projectile p4, type2, 9, default, 1, true))
            {
                SetChatWithOtherOne(p4, "不觉得，一定是你又闲得慌了...", myColor, 7, 360, -1, 9);
            }
            else if (FindChatIndex(out Projectile p5, type3, 4, default, 1, true))
            {
                SetChatWithOtherOne(p5, "给我适可而止啊喂！", myColor, 8, 360);
                p5.ai[0] = 0;
                talkInterval = 600;
            }
            else if (mainTimer % 720 == 0 && Main.rand.NextBool(7))
            {
                SetChat(myColor);
            }
        }
        int flyTimeleft = 0;
        public override void VisualEffectForPreview()
        {
            UpdateClothFrame();
        }
        public override void AI()
        {
            if (flyTimeleft > 0)
            {
                flyTimeleft--;
            }
            Player player = Main.player[Projectile.owner];
            Projectile.SetPetActive(player, BuffType<ReimuBuff>());
            if (PetState != 2)
            {
                UpdateTalking();
            }
            Vector2 point = new Vector2(-50 * player.direction, -30 + player.gfxOffY);
            Projectile.tileCollide = false;
            Projectile.rotation = Projectile.velocity.X * 0.012f;
            if (PetState != 2)
            {
                ChangeDir(player, PetState < 3);
            }

            MoveToPoint(point, 22f);

            if (mainTimer % 270 == 0 && PetState != 2 && PetState < 3)
            {
                PetState = 1;
            }
            else if (mainTimer % 270 == 0 && PetState == 3)
            {
                PetState = 4;
            }
            if (mainTimer >= 1200 && mainTimer < 3600 && PetState != 1 && PetState < 3 && !player.AnyBosses())
            {
                int chance = 6;
                if (Main.bloodMoon || Main.eclipse)
                {
                    chance = 11;
                }
                else if (Main.dayTime || Main.raining)
                {
                    chance = 3;
                }
                else if (player.sleeping.FullyFallenAsleep)
                {
                    chance = 1;
                }
                if (mainTimer % 120 == 0 && Main.rand.NextBool(chance) && extraAI[0] <= 0 && player.velocity.Length() == 0 && ChatCD <= 0)
                {
                    SetChat(myColor, "好困...", 3);
                    PetState = 2;
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
            if (PetState != 2 && chatFuncIsOccupied)
            {
                chatFuncIsOccupied = false;
            }
        }
    }
}


