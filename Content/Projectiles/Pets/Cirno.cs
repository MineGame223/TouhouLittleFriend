using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ID;
using Terraria.Utilities;
using TouhouPets.Content.Buffs.PetBuffs;

namespace TouhouPets.Content.Projectiles.Pets
{
    /// <summary>
    /// 示范好孩子琪露诺
    /// </summary>
    public class Cirno : BasicTouhouPet
    {
        public override void SetStaticDefaults()
        {
            Main.projFrames[Type] = 11;
            Main.projPet[Type] = true;
            ProjectileID.Sets.LightPet[Type] = true;
        }
        public override bool PreDraw(ref Color lightColor)
        {
            DrawCirno(wingFrame, Color.White * 0.7f);
            DrawCirno(Projectile.frame, lightColor);
            DrawCirno(Projectile.frame, lightColor, AltVanillaFunction.GetExtraTexture("Cirno_Cloth"), true);
            return false;
        }
        private void DrawCirno(int frame, Color lightColor, Texture2D tex = null, bool entitySpriteDraw = false)
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
            if (++Projectile.frameCounter > 3)
            {
                Projectile.frameCounter = 0;
                Projectile.frame++;
            }
            if (Projectile.frame > 2)
            {
                Projectile.frame = 0;
                PetState = 0;
            }
        }
        int wingFrame, wingFrameCounter;
        private void Laugh()
        {
            if (Projectile.frame < 3)
            {
                Projectile.frame = 3;
            }
            if (++Projectile.frameCounter > (Projectile.frame >= 4 && Projectile.frame <= 5 ? 12 : 7))
            {
                Projectile.frameCounter = 0;
                Projectile.frame++;
            }
            if (extraAI[0] == 0)
            {
                if (Projectile.frame > 5)
                {
                    Projectile.frame = 4;
                    extraAI[1]++;
                }
                if (Projectile.owner == Main.myPlayer)//拥有随机数的操作需要在本端选择完成后同步到其他端
                {
                    if (extraAI[1] > Main.rand.Next(10, 20))
                    {
                        extraAI[1] = 0;
                        extraAI[0] = 1;
                        Projectile.netUpdate = true;
                    }
                }               
            }
            else
            {
                if (Projectile.frame > 6)
                {
                    Projectile.frame = 0;
                    extraAI[0] = 600;
                    PetState = 0;
                }
            }
        }
        private void UpdateWingFrame()
        {
            if (!Main.player[Projectile.owner].ZoneUnderworldHeight)
            {
                if (wingFrame < 7 || wingFrame >= 10)
                {
                    wingFrame = 7;
                }
            }
            else
            {
                if (wingFrame < 7)
                {
                    wingFrame = 7;
                }
            }
            int count = Main.player[Projectile.owner].ZoneUnderworldHeight ? 8 : 4;
            if (++wingFrameCounter > count)
            {
                wingFrameCounter = 0;
                wingFrame++;
            }
            if (Main.player[Projectile.owner].ZoneUnderworldHeight)
            {
                if (wingFrame > 10)
                {
                    wingFrame = 7;
                }
            }
        }
        /// <summary>
        /// 对话文本颜色
        /// </summary>
        Color myColor = new Color(76, 207, 239);
        public override string GetChatText(out string[] text)
        {
            Player player = Main.player[Projectile.owner];
            text = new string[11];
            if (!player.ZoneUnderworldHeight)
            {
                text[1] = ModUtils.GetChatText("Cirno", "1");
                text[2] = ModUtils.GetChatText("Cirno", "2");
                text[3] = ModUtils.GetChatText("Cirno", "3");
                if (player.HasBuff<DaiyouseiBuff>())
                {
                    text[4] = ModUtils.GetChatText("Cirno", "4");//会被大妖精检测到的对话
                }
                text[6] = ModUtils.GetChatText("Cirno", "6");
            }
            if ((player.ZoneDesert && Main.dayTime) || player.ZoneUnderworldHeight || player.ZoneJungle)
            {
                if (player.ZoneJungle && player.ZoneOverworldHeight)
                {
                    text[8] = ModUtils.GetChatText("Cirno", "8");
                }
                else
                {
                    text[7] = ModUtils.GetChatText("Cirno", "7");//会被大妖精检测到的对话
                }
            }
            WeightedRandom<string> chat = new WeightedRandom<string>();
            {
                for (int i = 1; i < text.Length; i++)
                {
                    if (text[i] != null)
                    {
                        int weight = 1;
                        if (i == 7 || i == 8)
                        {
                            weight = 3;//增加这些对话出现的权重
                        }
                        if (i == 4)
                        {
                            weight = 5;//增加这些对话出现的权重
                        }
                        chat.Add(text[i], weight);
                    }
                }
            }
            return chat;
        }
        /// <summary>
        /// 更新对话文本，包含接应对话和常规讲话
        /// </summary>
        private void UpdateTalking()
        {
            int type1 = ProjectileType<Daiyousei>();
            //为了尽可能确保对话接应成功，在检测到可接应对话的第一刻起就保持CD以避免出现其他对话
            //只适用于最开始的对话，进入对话后无需继续检测
            if (FindChatIndex(out Projectile _, type1, 4, default, 0)
                || FindChatIndex(out Projectile _, type1, 5, default, 0))
            {
                ChatCD = 1;
            }

            if (FindChatIndex(out Projectile p, type1, 4))
            {
                //同时给对方与自己设置ChatCD以确保对话不会“走神”
                SetChatWithOtherOne(p, ModUtils.GetChatText("Cirno", "10"), myColor, 0);//作为收尾的对话，ChatIndex通常为0
                p.localAI[2] = 0;//作为收尾的对话，将对方的ChatIndex设为0，防止重复检测并接话
            }
            else if (FindChatIndex(out Projectile p1, type1, 5))
            {
                SetChatWithOtherOne(p1, ModUtils.GetChatText("Cirno", "9"), myColor, 9);
            }
            //无视对方的ChatCD，避免对话被无视，常用于交互中的第三句话及以后
            else if (FindChatIndex(out Projectile p2, type1, 6, default, 1, true))
            {
                SetChatWithOtherOne(p2, ModUtils.GetChatText("Cirno", "11"), myColor, 0);
                p2.localAI[2] = 0;
            }
            else if (mainTimer % 480 == 0 && Main.rand.NextBool(6) && mainTimer > 0 && PetState != 2)
            {
                SetChat(myColor);
            }
        }
        public override void VisualEffectForPreview()
        {
            UpdateWingFrame();
        }
        public override void AI()
        {            
            Lighting.AddLight(Projectile.Center, 0.57f, 1.61f, 1.84f);
            Player player = Main.player[Projectile.owner];
            Projectile.SetPetActive(player, BuffType<CirnoBuff>());

            UpdateTalking();
            Vector2 point = new Vector2(40 * player.direction, -30 + player.gfxOffY);
            Projectile.tileCollide = false;
            Projectile.rotation = Projectile.velocity.X * 0.032f;

            ChangeDir(player);
            MoveToPoint(point, 9f);

            if (Projectile.owner == Main.myPlayer)//仅当处于本端时进行状态更新并同步到其他客户端
            {
                if (mainTimer % 270 == 0 && PetState != 2)
                {
                    PetState = 1;
                    Projectile.netUpdate = true;
                }
                if (mainTimer >= 1200 && mainTimer < 3600 && PetState != 1)
                {
                    if (mainTimer % 300 == 0 && Main.rand.NextBool(2) && extraAI[0] <= 0)
                    {
                        PetState = 2;
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
                Blink();
            }
            else if (PetState == 2)
            {
                Laugh();
            }
            //处于沙漠，地狱或丛林时琪露诺不会再大笑；处于地狱时琪露诺会半闭着眼且减少对话
            if ((player.ZoneDesert && Main.dayTime) || player.ZoneUnderworldHeight || player.ZoneJungle)
            {
                if (PetState == 2)
                {
                    PetState = 0;
                }
                if (player.ZoneUnderworldHeight)
                {
                    if (Projectile.frame < 1)
                        Projectile.frame = 1;

                    if (Main.rand.NextBool(12))
                    {
                        for (int i = 0; i < Main.rand.Next(1, 4); i++)
                        {
                            Dust.NewDustPerfect(Projectile.position + new Vector2(Main.rand.Next(10, Projectile.width - 10), Main.rand.Next(10, Projectile.height - 10)),
                                MyDustId.Water, null, 100, Color.White).scale = Main.rand.NextFloat(0.5f, 1.2f);
                        }
                    }
                }
            }
        }
    }
}


