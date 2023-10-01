using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.Utilities;
using TouhouPets.Content.Buffs.PetBuffs;

namespace TouhouPets.Content.Projectiles.Pets
{
    public class Yuyuko : BasicTouhouPet
    {
        public override void SetStaticDefaults()
        {
            Main.projFrames[Type] = 20;
            Main.projPet[Type] = true;
            ProjectileID.Sets.LightPet[Type] = false;
        }
        public override bool PreDraw(ref Color lightColor)
        {
            DrawYuyuko(Projectile.frame, lightColor);
            DrawYuyuko(hatFrame, lightColor, 1, null, true);
            Projectile.DrawStateNormalizeForPet();
            if (PetState == 1)
                DrawYuyuko(blinkFrame, lightColor, 1);
            DrawYuyuko(Projectile.frame, lightColor, 0, AltVanillaFunction.GetExtraTexture("Yuyuko_Cloth"), true);
            DrawYuyuko(clothFrame, lightColor, 1, null, true);
            Projectile.DrawStateNormalizeForPet();
            if (Projectile.frame >= 2 && Projectile.frame <= 4)
            {
                DrawFood(lightColor);
            }
            return false;
        }
        private void DrawYuyuko(int frame, Color lightColor, int columns = 0, Texture2D tex = null, bool entitySpriteDraw = false)
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
        private void DrawFood(Color lightColor)
        {
            if (food == null || food.type == ItemID.None)
                return;

            Main.instance.LoadItem(food.type);
            Texture2D t = AltVanillaFunction.ItemTexture(food.type);
            Vector2 pos = Projectile.Center + new Vector2(10 * Projectile.spriteDirection, -3) + new Vector2(extraAdjX, extraAdjY) - Main.screenPosition + new Vector2(0, 7f * Main.essScale);
            int height = t.Height / 3;
            Rectangle rect = new Rectangle(0, height * (food.type == ItemID.Ale ? 2 : 1), t.Width, height);
            Vector2 orig = rect.Size() / 2;
            Color clr = Projectile.GetAlpha(lightColor);
            SpriteEffects effect = Projectile.spriteDirection == -1 ? SpriteEffects.None : SpriteEffects.FlipHorizontally;
            Main.spriteBatch.TeaNPCDraw(t, pos, rect, clr, Projectile.rotation, orig, 1f, effect, 0f);
        }
        private void Blink()
        {
            if (++blinkFrameCounter > 3)
            {
                blinkFrameCounter = 0;
                blinkFrame++;
            }
            if (blinkFrame > 2)
            {
                blinkFrame = 0;
                PetState = 0;
            }
        }
        int hatFrame, hatFrameCounter;
        int blinkFrame, blinkFrameCounter;
        int clothFrame, clothFrameCounter;
        int extraAdjX, extraAdjY;
        Item food;
        List<Item> foodList = new List<Item>();
        private void Fan()
        {
            if (Projectile.frame < 7)
                Projectile.frame = 7;

            if (++Projectile.frameCounter > 12)
            {
                Projectile.frameCounter = 0;
                Projectile.frame++;
            }
            if (extraAI[0] == 0)
            {
                if (Projectile.frame > 15)
                {
                    Projectile.frame = 11;
                    extraAI[1]++;
                }
                if (extraAI[1] > extraAI[2])
                {
                    extraAI[1] = 0;
                    extraAI[0] = 1;
                }
            }
            else
            {
                Projectile.frameCounter++;
                if (Projectile.frame > 19)
                {
                    Projectile.frame = 0;
                    extraAI[0] = 600;
                    extraAI[2] = 0;
                    PetState = 0;
                }
            }
        }
        private void FoodSelect(Player player)
        {
            foodList.Clear();

            for (int j = 0; j < player.inventory.Length; j++)
            {
                Item fd = player.inventory[j];
                if (fd != null && !fd.IsAir && ItemID.Sets.IsFood[fd.type]
                    && fd != player.inventory[player.selectedItem]
                    && !fd.favorited)
                {
                    foodList.Add(fd);
                }
                if (j < player.bank4.item.Length)
                {
                    if (player.IsVoidVaultEnabled)
                    {
                        Item fd2 = player.bank4.item[j];
                        if (fd2 != null && !fd2.IsAir && ItemID.Sets.IsFood[fd2.type]
                            && !fd2.favorited)
                        {
                            foodList.Add(fd2);
                        }
                    }
                }
            }

            if (foodList.Count > 0)
            {
                Item fd = foodList[Main.rand.Next(foodList.Count)];
                food = new Item(fd.type);
                fd.stack--;
                if (fd.stack <= 0)
                {
                    fd.TurnToAir(true);
                }

                PetState = 3;
                if (ChatIndex < 9 || ChatIndex > 10)
                {
                    int chance = Main.rand.Next(3);
                    switch (chance)
                    {
                        case 1:
                            SetChat(myColor, ModUtils.GetChatText("Yuyuko", "6"), 6, 60, 30, true);
                            break;
                        case 2:
                            SetChat(myColor, ModUtils.GetChatText("Yuyuko", "7"), 7, 60, 30, true);
                            break;
                        default:
                            SetChat(myColor, ModUtils.GetChatText("Yuyuko", "5"), 5, 60, 30, true);
                            break;
                    }
                }
            }
            else
            {
                if (ChatIndex < 9 || ChatIndex > 10)
                {
                    int chance = Main.rand.Next(3);
                    switch (chance)
                    {
                        case 1:
                            SetChat(myColor, ModUtils.GetChatText("Yuyuko", "6-1"), 13, 60, 30, true);
                            break;
                        case 2:
                            SetChat(myColor, ModUtils.GetChatText("Yuyuko", "7-1"), 12, 60, 30, true);
                            break;
                        default:
                            SetChat(myColor, ModUtils.GetChatText("Yuyuko", "5-1"), 11, 60, 30, true);
                            break;
                    }
                }
            }
        }
        private void EmitFoodParticles(Item sItem)
        {
            Color[] array = ItemID.Sets.FoodParticleColors[sItem.type];
            if (array != null && array.Length != 0 && Main.rand.NextBool(2))
            {
                Vector2? mouthPosition = Projectile.Center + new Vector2(10 * Projectile.spriteDirection, -3)
                    + new Vector2(extraAdjX, extraAdjY) + new Vector2(0, 7f * Main.essScale);
                if (mouthPosition.HasValue)
                {
                    Vector2 vector = mouthPosition.Value + Main.rand.NextVector2Square(-4f, 4f);
                    Vector2 spinningpoint = new Vector2(Projectile.spriteDirection, 0);
                    Dust.NewDustPerfect(vector, 284, 1.3f * spinningpoint.RotatedBy((float)Math.PI / 5f * Main.rand.NextFloatDirection()), 0, array[Main.rand.Next(array.Length)], 0.8f + 0.2f * Main.rand.NextFloat()).fadeIn = 0f;
                }
            }

            Color[] array2 = ItemID.Sets.DrinkParticleColors[sItem.type];
            if (array2 != null && array2.Length != 0)
            {
                Vector2? mouthPosition = Projectile.Center + new Vector2(10 * Projectile.spriteDirection, -3)
                    + new Vector2(extraAdjX, extraAdjY) + new Vector2(0, 7f * Main.essScale);
                if (mouthPosition.HasValue)
                {
                    Vector2 vector = mouthPosition.Value + Main.rand.NextVector2Square(-4f, 4f);
                    Vector2 spinningpoint = new Vector2(Projectile.spriteDirection * 0.1f, 0);
                    Dust.NewDustPerfect(vector, 284, 1.3f * spinningpoint.RotatedBy(-(float)Math.PI / 5f * Main.rand.NextFloatDirection()), 0, array2[Main.rand.Next(array2.Length)] * 0.7f, 0.8f + 0.2f * Main.rand.NextFloat()).fadeIn = 0f;
                }
            }
        }
        private void Eat()
        {
            if (food == null || food.type == ItemID.None)
            {
                Projectile.frame = 0;
                extraAI[0] = 60;
                PetState = 0;
                return;
            }
            if (++Projectile.frameCounter > 12)
            {
                Projectile.frameCounter = 0;
                Projectile.frame++;
            }
            if (extraAI[0] == 0)
            {
                if (Projectile.frame >= 3)
                {
                    Projectile.frame = 3;
                    extraAI[1]++;
                }
                if (extraAI[1] > 180)
                {
                    extraAI[1] = 0;
                    extraAI[0] = 1;
                }
            }
            else
            {
                if (Projectile.frame == 5)
                {
                    EmitFoodParticles(food);
                    if (Projectile.frameCounter == 1 && food.UseSound != null)
                        AltVanillaFunction.PlaySound((SoundStyle)food.UseSound, Projectile.Center);
                }
                if (Projectile.frame > 6)
                {
                    Projectile.frame = 0;
                    extraAI[0] = 60;
                    PetState = 0;
                }
            }
        }
        private void UpdateHatFrame()
        {
            if (hatFrame < 3)
            {
                hatFrame = 3;
            }
            int count = 7;
            if (++hatFrameCounter > count)
            {
                hatFrameCounter = 0;
                hatFrame++;
            }
            if (hatFrame > 6)
            {
                hatFrame = 3;
            }
        }
        private void UpdateClothFrame()
        {
            if (clothFrame < 7)
            {
                clothFrame = 7;
            }
            int count = 5;
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
        Color myColor = new Color(255, 112, 214);
        public override string GetChatText(out string[] text)
        {
            Player player = Main.player[Projectile.owner];
            text = new string[21];
            text[1] = ModUtils.GetChatText("Yuyuko", "1");
            text[2] = ModUtils.GetChatText("Yuyuko", "2");
            text[3] = ModUtils.GetChatText("Yuyuko", "3");
            text[4] = ModUtils.GetChatText("Yuyuko", "4");
            if (player.HasBuff(BuffType<YoumuBuff>())
                && FindPetState(out Projectile _, ProjectileType<Youmu>(), 0, 1))
            {
                text[8] = ModUtils.GetChatText("Yuyuko", "8");
                text[9] = ModUtils.GetChatText("Yuyuko", "9");
            }
            WeightedRandom<string> chat = new WeightedRandom<string>();
            {
                for (int i = 1; i < text.Length; i++)
                {
                    if (text[i] != null)
                    {
                        int weight = 1;
                        if (i == 9)
                            weight = 10;
                        chat.Add(text[i], weight);
                    }
                }
            }
            return chat;
        }
        private void UpdateTalking()
        {
            int type1 = ProjectileType<Youmu>();
            if (FindChatIndex(out Projectile p, type1, 6, default, 1, true))
            {
                SetChatWithOtherOne(p, ModUtils.GetChatText("Yuyuko", "10"), myColor, 10, 600);
            }
            else if (mainTimer % 960 == 0 && Main.rand.NextBool(9) && mainTimer > 0 && PetState < 2)
            {
                SetChat(myColor);
            }
        }
        public override void VisualEffectForPreview()
        {
            UpdateHatFrame();
            UpdateClothFrame();
        }
        public override void AI()
        {
            Player player = Main.player[Projectile.owner];
            Projectile.SetPetActive(player, BuffType<YuyukoBuff>());
            UpdateTalking();
            Vector2 point = new Vector2(-70 * player.direction, -60 + player.gfxOffY);
            if (player.ownedProjectileCounts[ProjectileType<Youmu>()] > 0)
            {
                point = new Vector2(60 * player.direction, -50 + player.gfxOffY);
            }
            Projectile.tileCollide = false;
            Projectile.rotation = Projectile.velocity.X * 0.003f;

            ChangeDir(player, player.ownedProjectileCounts[ProjectileType<Youmu>()] <= 0);
            MoveToPoint(point, 16f);
            if (mainTimer % 20 == 0)
            {
                Projectile.NewProjectile(Projectile.GetSource_FromThis(), Projectile.Center + new Vector2(Main.rand.Next(-40, 40), Main.rand.Next(-20, 50))
                            , new Vector2(0, Main.rand.NextFloat(-0.3f, -0.7f)), ProjectileType<YuyukoButterfly>(), 0, 0, Main.myPlayer);
            }

            if (mainTimer % 270 == 0 && PetState < 1)
            {
                PetState = 1;
            }
            if (mainTimer >= 600 && extraAI[0] == 0)
            {
                if (mainTimer % 600 == 0 && PetState < 2)
                {
                    if (Main.rand.NextBool(6))
                    {
                        PetState = 2;
                        extraAI[2] = Main.rand.Next(10, 30);
                    }
                    else if (Main.rand.NextBool(3))
                    {
                        FoodSelect(player);
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
                Fan();
            }
            else if (PetState == 3)
            {
                Eat();
            }
            extraAdjX = 0;
            extraAdjY = 0;
            switch (Projectile.frame)
            {
                case 2:
                    extraAdjY = 2;
                    break;
                case 4:
                    extraAdjX = -4 * Projectile.spriteDirection;
                    extraAdjY = -4;
                    break;
                default:
                    break;
            }
        }
    }
}


