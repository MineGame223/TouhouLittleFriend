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
            if (!HateSunlight(Projectile))
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
        public static bool HateSunlight(Projectile projectile)
        {
            Player player = Main.player[projectile.owner];
            bool sunlight = Main.dayTime && (player.ZoneOverworldHeight || player.ZoneSkyHeight) && !player.behindBackWall;
            bool rain = Main.raining && (player.ZoneOverworldHeight || player.ZoneSkyHeight);
            if (sunlight || rain)
            {
                if (projectile.type == ProjectileType<Remilia>() && player.ownedProjectileCounts[ProjectileType<Sakuya>()] > 0
                    || projectile.type == ProjectileType<Flandre>() && player.ownedProjectileCounts[ProjectileType<Meirin>()] > 0)
                    return false;
                else
                    return true;
            }
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
                if (Projectile.owner == Main.myPlayer)
                {
                    if (extraAI[1] > Main.rand.Next(120, 360))
                    {
                        extraAI[1] = 0;
                        extraAI[0] = 1;
                        Projectile.netUpdate = true;
                    }
                }
            }
            if (extraAI[0] == 1)
            {
                if (Projectile.frame >= 6)
                {
                    Projectile.frame = 6;
                    extraAI[1]++;
                }
                if (Projectile.owner == Main.myPlayer)
                {
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
                        Projectile.netUpdate = true;
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
            text[1] = ModUtils.GetChatText("Remilia", "1");
            text[2] = ModUtils.GetChatText("Remilia", "2");
            if (PetState == 2)
            {
                text[3] = ModUtils.GetChatText("Remilia", "3");
                text[4] = ModUtils.GetChatText("Remilia", "4");
            }
            if (Main.bloodMoon)
            {
                text[5] = ModUtils.GetChatText("Remilia", "5");
            }
            if (FindPetState(out Projectile _, ProjectileType<Flandre>(), 0))
            {
                text[6] = ModUtils.GetChatText("Remilia", "6");
            }
            WeightedRandom<string> chat = new WeightedRandom<string>();
            {
                for (int i = 1; i < text.Length; i++)
                {
                    if (text[i] != null)
                    {
                        int weight = 1;
                        if (PetState == 2 && (i > 4 || i < 3))
                        {
                            weight = 0;
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
            if (FindChatIndex(out Projectile p4, type2, 6, default, 1, true))
            {
                SetChatWithOtherOne(p4, ModUtils.GetChatText("Remilia", "7"), myColor, 7, 600, -1);
            }
            else if (FindChatIndex(out Projectile p5, type2, 7, default, 1, true))
            {
                SetChatWithOtherOne(p5, ModUtils.GetChatText("Remilia", "8"), myColor, 8, 360, -1);
            }
            else if (FindChatIndex(out Projectile p6, type2, 3, default, 1, true))
            {
                SetChatWithOtherOne(p6, ModUtils.GetChatText("Remilia", "9"), myColor, 0, 360, -1, 10);
                p6.localAI[2] = 0;
            }
            else if (FindChatIndex(out Projectile p1, type3, 6, default, 1, true))
            {
                SetChatWithOtherOne(p1, ModUtils.GetChatText("Remilia", "10"), myColor, 10, 600, -1, 11);
            }
            else if (FindChatIndex(out Projectile p2, type3, 9, default, 1, true))
            {
                SetChatWithOtherOne(p2, ModUtils.GetChatText("Remilia", "11"), myColor, 11, 600, -1, 6);
            }
            else if (FindChatIndex(out Projectile p3, type3, 10, default, 1, true))
            {
                SetChatWithOtherOne(p3, ModUtils.GetChatText("Remilia", "12"), myColor, 12, 360, -1, 10);
            }
            else if (PetState == 2 && mainTimer % 120 == 0 && Main.rand.NextBool(5) && mainTimer > 0)
            {
                SetChat(myColor);
            }
            else if (mainTimer % 720 == 0 && Main.rand.NextBool(9) && mainTimer > 0)
            {
                SetChat(myColor);
            }
        }
        public override void VisualEffectForPreview()
        {
            UpdateWingFrame();
            UpdateClothFrame();
        }
        private void ControlMovement(Player player)
        {
            Projectile.tileCollide = false;
            if (PetState != 2)
                Projectile.rotation = Projectile.velocity.X * 0.03f;
            else
                Projectile.rotation = Projectile.velocity.X * 0.005f;

            Vector2 point = new Vector2(-50 * player.direction, -50 + player.gfxOffY);
            bool hasFlandre = player.ownedProjectileCounts[ProjectileType<Flandre>()] > 0;
            if (hasFlandre)
            {
                point = new Vector2(50 * player.direction, -50 + player.gfxOffY);
            }

            ChangeDir(player, !hasFlandre);
            MoveToPoint(point, 19f);
        }
        public override void AI()
        {
            Player player = Main.player[Projectile.owner];
            Projectile.SetPetActive(player, BuffType<RemiliaBuff>());
            Projectile.SetPetActive(player, BuffType<ScarletBuff>());

            if (!Main.dayTime)
                UpdateTalking();
            ControlMovement(player);

            if (HateSunlight(Projectile))
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
            if (Projectile.owner == Main.myPlayer)
            {
                if (mainTimer % 270 == 0 && PetState != 2)
                {
                    PetState = 1;
                    Projectile.netUpdate = true;
                }
                if (mainTimer >= 1200 && mainTimer < 3600 && PetState != 1)
                {
                    if (mainTimer % 900 == 0 && Main.rand.NextBool(3) && extraAI[0] <= 0 && player.velocity.Length() < 4f)
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


