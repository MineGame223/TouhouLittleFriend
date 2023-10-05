using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ID;
using Terraria.Utilities;
using TouhouPets.Content.Buffs.PetBuffs;
using static TouhouPets.SolutionSpraySystem;

namespace TouhouPets.Content.Projectiles.Pets
{
    public class Yuka : BasicTouhouPet
    {
        public override void SetStaticDefaults()
        {
            Main.projFrames[Type] = 11;
            Main.projPet[Type] = true;
            ProjectileID.Sets.LightPet[Type] = false;
        }
        public override bool PreDraw(ref Color lightColor)
        {
            DrawYuka(Projectile.frame, lightColor);
            if (PetState == 1)
                DrawYuka(blinkFrame, lightColor, 1);
            DrawYuka(Projectile.frame, lightColor, 0, AltVanillaFunction.GetExtraTexture("Yuka_Cloth"), true);
            DrawYuka(clothFrame, lightColor, 1, null, true);
            Projectile.DrawStateNormalizeForPet();
            if (Projectile.frame == 8)
            {
                DrawHand(lightColor, null);
                DrawHand(lightColor, AltVanillaFunction.GetExtraTexture("Yuka_Cloth"), true);
            }
            Projectile.DrawStateNormalizeForPet();
            return false;
        }
        private void DrawYuka(int frame, Color lightColor, int columns = 0, Texture2D tex = null, bool entitySpriteDraw = false)
        {
            Texture2D t = tex ?? AltVanillaFunction.ProjectileTexture(Type);
            int width = t.Width / 2;
            int height = t.Height / Main.projFrames[Type];
            Vector2 pos = Projectile.Center + new Vector2(0, -10) - Main.screenPosition + new Vector2(0, 7f * Main.essScale);
            Rectangle rect = new(width * columns, frame * height, t.Width / 2, height);
            Vector2 orig = rect.Size() / 2;
            SpriteEffects effect = Projectile.spriteDirection == -1 ? SpriteEffects.None : SpriteEffects.FlipHorizontally;
            if (entitySpriteDraw)
                Main.EntitySpriteDraw(t, pos, rect, Projectile.GetAlpha(lightColor), Projectile.rotation, orig, Projectile.scale, effect, 0f);
            else
                Main.spriteBatch.TeaNPCDraw(t, pos, rect, Projectile.GetAlpha(lightColor), Projectile.rotation, orig, Projectile.scale, effect, 0f);
        }
        private void DrawHand(Color lightColor, Texture2D tex = null, bool entitySpriteDraw = false)
        {
            Texture2D t = tex ?? AltVanillaFunction.ProjectileTexture(Type);
            int width = t.Width / 2;
            int height = t.Height / Main.projFrames[Type];
            Vector2 pos = YukaHandOrigin - Main.screenPosition + new Vector2(0, 7f * Main.essScale);
            Rectangle rect = new(width, 7 * height, 16, 48);
            Vector2 orig = new(8, 48);
            float rot = MathHelper.ToRadians(angle);
            SpriteEffects effect = Projectile.spriteDirection == -1 ? SpriteEffects.None : SpriteEffects.FlipHorizontally;
            if (entitySpriteDraw)
                Main.EntitySpriteDraw(t, pos, rect, Projectile.GetAlpha(lightColor), rot, orig, Projectile.scale, effect, 0f);
            else
            {
                Main.spriteBatch.TeaNPCDraw(t, pos, rect, Projectile.GetAlpha(lightColor), rot, orig, Projectile.scale, effect, 0f);
            }
        }
        #region 溶液喷洒设置
        Item solution_Actually;
        public const int Phase_Spray_Mode1 = 3;
        public const int Phase_Spray_Mode2 = 4;
        public const int Phase_StopSpray = 5;
        private static int SolutionSprayType(int type)
        {
            return type switch
            {
                ItemID.GreenSolution => ProjectileID.PureSpray,
                ItemID.BlueSolution => ProjectileID.HallowSpray,
                ItemID.DarkBlueSolution => ProjectileID.MushroomSpray,
                ItemID.DirtSolution => ProjectileID.DirtSpray,
                ItemID.PurpleSolution => ProjectileID.CorruptSpray,
                ItemID.RedSolution => ProjectileID.CrimsonSpray,
                ItemID.SandSolution => ProjectileID.SandSpray,
                ItemID.SnowSolution => ProjectileID.SnowSpray,
                _ => SolutionSpraySystem.Sprayer.shoot,
            };
        }
        private static int SolutionSprayDust(int type)
        {
            return type switch
            {
                ProjectileID.PureSpray => MyDustId.GreenBubble,
                ProjectileID.HallowSpray => MyDustId.CyanBubble,
                ProjectileID.MushroomSpray => MyDustId.BlueIce,
                ProjectileID.DirtSpray => MyDustId.BrownBubble,
                ProjectileID.CorruptSpray => MyDustId.PinkBubble,
                ProjectileID.CrimsonSpray => MyDustId.PinkYellowBubble,
                ProjectileID.SandSpray => MyDustId.YellowBubble,
                ProjectileID.SnowSpray => MyDustId.WhiteBubble,
                _ => MyDustId.RedBubble,
            };
        }
        private void Spray(int mode)
        {
            Player player = Main.player[Projectile.owner];
            if (Projectile.frame < 5)
            {
                Projectile.frame = 5;
            }
            if (++Projectile.frameCounter > 4)
            {
                Projectile.frameCounter = 0;
                Projectile.frame++;
            }
            _solution = player.ChooseAmmo(Sprayer);
            if (_solution == null || _solution.IsAir)
            {
                PetState = Phase_StopSpray;
                return;
            }
            else
            {
                solution_Actually = new(_solution.type);
                solution_Actually.shoot = SolutionSprayType(_solution.type);
            }
            if (Projectile.frame >= 8)
            {
                Projectile.frame = 8;
                if (++extraAI[1] > 45)
                {
                    extraAI[1] = 0;
                    if (Main.rand.NextBool(2, 3) && _solution.consumable && _solution.ammo > AmmoID.None)
                        _solution.stack--;
                }

                if (mode == 1)
                {
                    angle += 2;
                    if (angle > 359)
                        angle = 0;
                }
                else
                {
                    angle = (int)MathHelper.ToDegrees((Main.MouseWorld - YukaHandOrigin).ToRotation() + MathHelper.PiOver2);
                }

                if (Projectile.frameCounter % 2 == 0)
                {
                    Vector2 pos = YukaHandOrigin + new Vector2(0, 7f * Main.essScale);
                    pos += new Vector2(0, -48).RotatedBy(MathHelper.ToRadians(angle));

                    for (int i = 0; i < 5; i++)
                    {
                        Dust.NewDustPerfect(pos, SolutionSprayDust(solution_Actually.shoot)
                        , new Vector2(0, Main.rand.NextFloat(2.4f, 4.8f)).RotatedByRandom(MathHelper.TwoPi), 100
                        , default, Main.rand.NextFloat(0.5f, 2f)).noGravity = true;
                    }

                    Projectile.NewProjectileDirect(Projectile.GetSource_FromThis()
                        , Projectile.Center + new Vector2(-2 * Projectile.spriteDirection, 12)
                        , new Vector2(0, -Sprayer.shootSpeed).RotatedBy(MathHelper.ToRadians(angle))
                        , solution_Actually.shoot, Sprayer.damage, Sprayer.knockBack, player.whoAmI);
                }
            }
            if (_solution.stack <= 0)
            {
                _solution.TurnToAir();
                solution_Actually.TurnToAir();
                extraAI[1] = 0;
                PetState = Phase_StopSpray;
            }
        }
        private void StopSpray()
        {
            if (Projectile.frame < 8)
            {
                Projectile.frame = 8;
            }
            if (++Projectile.frameCounter > 4 && angle <= 0)
            {
                Projectile.frameCounter = 0;
                Projectile.frame++;
            }
            if (angle > 359 || angle < 0)
            {
                angle = 0;
            }
            else
            {
                int rate = 9;
                if (angle <= 359 && angle >= 180)
                {
                    angle += rate;
                }
                else if (angle < 180)
                {
                    angle -= rate;
                }
            }
            if (Projectile.frame > 10 && angle <= 0)
            {
                extraAI[0] = 0;
                Projectile.frame = 0;
                PetState = 0;
            }
        }
        #endregion
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
        int clothFrame, clothFrameCounter;
        int blinkFrame, blinkFrameCounter;
        int angle;
        private Vector2 YukaHandOrigin
        {
            get
            {
                return Projectile.Center + new Vector2(-2 * Projectile.spriteDirection, 2);
            }
        }
        private void Blossom()
        {
            if (++Projectile.frameCounter > 7)
            {
                Projectile.frameCounter = 0;
                Projectile.frame++;
            }
            if (extraAI[0] == 0)
            {
                if (Projectile.frame >= 3)
                {
                    Projectile.frame = 3;
                    Vector2 pos = Projectile.Center + new Vector2(20 * Projectile.spriteDirection, -4);
                    int dustID = MyDustId.GreenTrans;
                    Dust.NewDustPerfect(pos, dustID
                        , new Vector2(Main.rand.NextFloat(-0.5f, 0.5f), Main.rand.NextFloat(-2.5f, -1.2f)), 100, default
                        , Main.rand.NextFloat(0.5f, 1.5f)).noGravity = true;
                }
                if (++extraAI[1] > extraAI[2])
                {
                    extraAI[1] = 0;
                    extraAI[0]++;
                    extraAI[2] = 0;
                }
            }
            else
            {
                if (Projectile.frame > 5)
                {
                    extraAI[0] = 0;
                    Projectile.frame = 0;
                    PetState = 0;
                }
            }
        }
        private void UpdateClothFrame()
        {
            int count = PetState == Phase_Spray_Mode1 ? 3 : 5;
            if (clothFrame < 3)
            {
                clothFrame = 3;
            }
            if (++clothFrameCounter > count)
            {
                clothFrameCounter = 0;
                clothFrame++;
            }
            if (clothFrame > 6)
            {
                clothFrame = 3;
            }
        }
        Color myColor = new(107, 252, 75);
        public override string GetChatText(out string[] text)
        {
            text = new string[21];
            text[1] = ModUtils.GetChatText("Yuka", "1");
            text[2] = ModUtils.GetChatText("Yuka", "2");
            text[3] = ModUtils.GetChatText("Yuka", "3");
            WeightedRandom<string> chat = new();
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
            if (mainTimer % 900 == 0 && Main.rand.NextBool(12) && mainTimer > 0 && PetState < 3)
            {
                SetChat(myColor);
            }
        }
        public override void VisualEffectForPreview()
        {
            UpdateClothFrame();
        }
        public override void AI()
        {
            Player player = Main.player[Projectile.owner];
            Projectile.SetPetActive(player, BuffType<YukaBuff>());
            UpdateTalking();
            Vector2 point = new(-50 * player.direction, -45 + player.gfxOffY);
            if (PetState == Phase_Spray_Mode2)
            {
                point = Main.MouseWorld - player.Center;
            }
            Projectile.tileCollide = false;
            Projectile.rotation = Projectile.velocity.X * 0.005f;

            ChangeDir(player, true);
            MoveToPoint(point, 12f);
            if (mainTimer % 270 == 0 && PetState == 0)
            {
                PetState = 1;
            }
            /*if (mainTimer >= 400 && mainTimer < 3600 && PetState <= 1 && extraAI[0] == 0)
            {
                if (mainTimer % 600 == 0 && Main.rand.NextBool(1))
                {
                    PetState = 2;
                    extraAI[2] = Main.rand.Next(600, 900);
                }
            }*/
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
                Blossom();
            }
            else if (PetState == Phase_Spray_Mode1)
            {
                Spray(0);
            }
            else if (PetState == Phase_Spray_Mode2)
            {
                Spray(1);
            }
            else if (PetState == Phase_StopSpray)
            {
                StopSpray();
            }
        }
    }
}


