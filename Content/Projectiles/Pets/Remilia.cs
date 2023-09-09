using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ID;
using Terraria.Utilities;
using TouhouPets.Content.Buffs.PetBuffs;

namespace TouhouPets.Content.Projectiles.Pets
{
    public class Remilia : BasicTouhouPet
    {
        public override void SetStaticDefaults()
        {
            Main.projFrames[Type] = 25;
            Main.projPet[Type] = true;
            ProjectileID.Sets.LightPet[Type] = false;
        }
        public override bool PreDraw(ref Color lightColor)
        {
            DrawRemilia(wingFrame, lightColor, new Vector2(extraAdjX, extraAdjY));
            DrawRemilia(Projectile.frame, lightColor);
            if (PetState == 1)
                DrawRemilia(blinkFrame, lightColor);
            DrawRemilia(Projectile.frame, lightColor, default, AltVanillaFunction.GetExtraTexture("Remilia_Cloth"), true);
            if (!HateSunlight())
                DrawRemilia(clothFrame, lightColor, new Vector2(extraAdjX, extraAdjY), null, true);
            return false;
        }
        private void DrawRemilia(int frame, Color lightColor, Vector2 extraPos = default, Texture2D tex = null, bool entitySpriteDraw = false)
        {
            if (extraPos == default)
                extraPos = Vector2.Zero;
            Texture2D t = tex ?? AltVanillaFunction.ProjectileTexture(Type);
            int height = t.Height / Main.projFrames[Type];
            Vector2 pos = Projectile.Center - Main.screenPosition + new Vector2(0, 7f * Main.essScale) + extraPos;
            Rectangle rect = new Rectangle(0, frame * height, t.Width, height);
            Vector2 orig = rect.Size() / 2;
            SpriteEffects effect = Projectile.spriteDirection == -1 ? SpriteEffects.None : SpriteEffects.FlipHorizontally;
            if (entitySpriteDraw)
                Main.EntitySpriteDraw(t, pos, rect, Projectile.GetAlpha(lightColor), Projectile.rotation, orig, Projectile.scale, effect, 0f);
            else
                Main.spriteBatch.TeaNPCDraw(t, pos, rect, Projectile.GetAlpha(lightColor), Projectile.rotation, orig, Projectile.scale, effect, 0f);
        }
        private bool HateSunlight()
        {
            Player player = Main.player[Projectile.owner];
            if (Main.dayTime && (player.ZoneOverworldHeight || player.ZoneSkyHeight) && !player.behindBackWall || Main.raining)
                return true;
            return false;
        }
        private void Blink()
        {
            if (blinkFrame < 22)
            {
                blinkFrame = 22;
            }
            if (++blinkFrameCounter > 3)
            {
                blinkFrameCounter = 0;
                blinkFrame++;
            }
            if (blinkFrame > 24)
            {
                blinkFrame = 22;
                PetState = 0;
            }
        }
        int wingFrame, wingFrameCounter;
        int blinkFrame, blinkFrameCounter;
        int clothFrame, clothFrameCounter;
        int extraAdjX, extraAdjY;
        private void DrinkingTea()
        {
            if (Projectile.frame < 1)
            {
                Projectile.frame = 1;
            }
            if (++Projectile.frameCounter > 9)
            {
                Projectile.frameCounter = 0;
                Projectile.frame++;
            }
            if (extraAI[0] == 0)
            {
                if (Projectile.frame >= 5)
                {
                    Projectile.frame = 5;
                    extraAI[1]++;
                }
                if (extraAI[1] > Main.rand.Next(120, 360))
                {
                    extraAI[1] = 0;
                    extraAI[0] = 1;
                }
            }
            if (extraAI[0] == 1)
            {
                if (Projectile.frame >= 6)
                {
                    Projectile.frame = 6;
                    extraAI[1]++;
                }
                if (extraAI[1] > Main.rand.Next(120, 360))
                {
                    extraAI[1] = 0;
                    extraAI[2]++;
                    if (extraAI[2] > Main.rand.Next(3, 9))
                    {
                        extraAI[2] = 0;
                        extraAI[0] = 2;
                    }
                    else
                    {
                        extraAI[0] = 0;
                    }
                }
            }
            else
            {
                if (Projectile.frame > 10)
                {
                    Projectile.frame = 0;
                    extraAI[0] = 1800;
                    PetState = 0;
                }
            }
        }
        private void UpdateWingFrame()
        {
            if (wingFrame < 16)
            {
                wingFrame = 16;
            }
            int count = 6;
            if (++wingFrameCounter > count)
            {
                wingFrameCounter = 0;
                wingFrame++;
            }
            if (wingFrame > 21)
            {
                wingFrame = 16;
            }
        }
        private void UpdateClothFrame()
        {
            if (clothFrame < 12)
            {
                clothFrame = 12;
            }
            int count = 4;
            if (++clothFrameCounter > count)
            {
                clothFrameCounter = 0;
                clothFrame++;
            }
            if (clothFrame > 15)
            {
                clothFrame = 12;
            }
        }
        Color myColor = new Color(255, 10, 10);
        public override string GetChatText(out string[] text)
        {
            //Player player = Main.player[Projectile.owner];
            text = new string[21];
            text[3] = "人类，臣服于我！";
            text[4] = "终有一日世界将化为红魔之乡！";
            if (PetState == 2)
            {
                text[8] = "嗯...温度还差点...";
                text[9] = "感觉味道可以再浓一些...";
            }
            if (Main.bloodMoon)
            {
                text[10] = "多么美妙的夜晚！";
            }
            if (talkInterval <= 0 && FindPetState(out Projectile _, ProjectileType<Flandre>(), 0))
            {
                text[11] = "我亲爱的芙兰哟...";
            }
            WeightedRandom<string> chat = new WeightedRandom<string>();
            {
                for (int i = 1; i < text.Length; i++)
                {
                    if (text[i] != null)
                    {
                        int weight = 1;
                        if (i >= 8 && i <= 10)
                        {
                            weight = 5;
                        }
                        chat.Add(text[i], weight);
                    }
                }
            }
            return chat;
        }
        private void UpdateTalking()
        {
            int type2 = ProjectileType<Flandre>();
            int type3 = ProjectileType<Patchouli>();
            if (FindChatIndex(out Projectile _, type2, 3, default, 0)
                || FindChatIndex(out Projectile _, type3, 16, default, 0))
            {
                ChatCD = 1;
            }
            if (FindChatIndex(out Projectile p4, type2, 5, default, 1, true))
            {
                SetChatWithOtherOne(p4, "没什么...只是想叫你一下", myColor, 12, 600, -1);
            }
            else if (FindChatIndex(out Projectile p5, type2, 6, default, 1, true))
            {
                SetChatWithOtherOne(p5, "有空会陪你的啦", myColor, 13, 360, -1);
            }
            else if (FindChatIndex(out Projectile p6, type2, 3, default, 1, true))
            {
                SetChatWithOtherOne(p6, "芙兰不能总是依赖姐姐哦", myColor, 14, 360, -1, 10);
                p6.ai[0] = 0;
                talkInterval = 600;
            }
            else if (FindChatIndex(out Projectile p1, type3, 16, default, 1, true))
            {
                SetChatWithOtherOne(p1, "嗯？帕琪？有啥事么？", myColor, 5, 600, -1, 11);
            }
            else if (FindChatIndex(out Projectile p2, type3, 17, default, 1, true))
            {
                SetChatWithOtherOne(p2, "哈哈，那都是瞎扯，吸血鬼怕十字架不过是人类打不过吸血鬼而臆想出来的心理安慰", myColor, 6, 600, -1, 6);
            }
            else if (FindChatIndex(out Projectile p3, type3, 18, default, 1, true))
            {
                SetChatWithOtherOne(p3, "当然了，帕琪你也要多出来走走嘛", myColor, 7, 360, -1, 10);
            }
            else if (mainTimer % 720 == 0 && Main.rand.NextBool(9))
            {
                SetChat(myColor);
            }
        }
        public override void VisualEffectForPreview()
        {
            UpdateWingFrame();
            UpdateClothFrame();
        }
        public override void AI()
        {
            Player player = Main.player[Projectile.owner];
            Projectile.SetPetActive(player, BuffType<RemiliaBuff>());
            Projectile.SetPetActive(player, BuffType<ScarletBuff>());
            if (!Main.dayTime)
                UpdateTalking();
            Vector2 point = new Vector2(-50 * player.direction, -50 + player.gfxOffY);
            if (player.ownedProjectileCounts[ProjectileType<Flandre>()] > 0)
            {
                point = new Vector2(50 * player.direction, -50 + player.gfxOffY);
            }
            Projectile.tileCollide = false;
            if (PetState != 2)
                Projectile.rotation = Projectile.velocity.X * 0.03f;
            else
                Projectile.rotation = Projectile.velocity.X * 0.005f;

            ChangeDir(player, player.ownedProjectileCounts[ProjectileType<Flandre>()] <= 0);
            MoveToPoint(point, 19f);
            if (HateSunlight())
            {
                extraAI[0] = 0;
                extraAI[1] = 0;
                extraAI[2] = 0;
                Projectile.rotation = 0f;
                PetState = 0;
                Projectile.frame = 11;
                chatFuncIsOccupied = true;
                return;
            }
            else
            {
                chatFuncIsOccupied = false;
            }
            if (mainTimer % 270 == 0 && PetState != 2)
            {
                PetState = 1;
            }
            if (mainTimer >= 1200 && mainTimer < 3600 && PetState != 1)
            {
                if (mainTimer % 900 == 0 && Main.rand.NextBool(6) && extraAI[0] <= 0 && player.velocity.Length() < 4f)
                {
                    PetState = 2;
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
                DrinkingTea();
            }
            extraAdjX = 0;
            extraAdjY = 0;
            if (Projectile.frame >= 1 && Projectile.frame <= 10)
            {
                extraAdjY = -2;
                if (Projectile.frame != 10)
                {
                    extraAdjX = -2 * Projectile.spriteDirection;
                }
            }
        }
    }
}


