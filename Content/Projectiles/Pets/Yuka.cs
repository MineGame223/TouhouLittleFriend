using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.IO;
using Terraria;
using Terraria.ID;
using Terraria.Utilities;
using TouhouPets.Content.Buffs.PetBuffs;
using static TouhouPets.SolutionSpraySystem;

namespace TouhouPets.Content.Projectiles.Pets
{
    public class Yuka : BasicTouhouPetNeo
    {
        public override void SetStaticDefaults()
        {
            Main.projFrames[Type] = 11;
            Main.projPet[Type] = true;
            ProjectileID.Sets.LightPet[Type] = false;
        }
        DrawPetConfig drawConfig = new(2)
        {
            PositionOffset = new Vector2(0, -10),
        };
        readonly Texture2D clothTex = AltVanillaFunction.GetExtraTexture("Yuka_Cloth");
        public override bool PreDraw(ref Color lightColor)
        {
            DrawPetConfig config = drawConfig with
            {
                ShouldUseEntitySpriteDraw = true,
            };

            Projectile.DrawPet(Projectile.frame, lightColor, drawConfig);

            if (PetState == 1)
                Projectile.DrawPet(blinkFrame, lightColor, drawConfig, 1);

            Projectile.DrawPet(Projectile.frame, lightColor,
                config with
                {
                    AltTexture = clothTex,
                });
            Projectile.DrawPet(clothFrame, lightColor, config, 1);
            Projectile.DrawStateNormalizeForPet();

            if (Projectile.frame == 8)
            {
                DrawHand(lightColor, null);
                DrawHand(lightColor, AltVanillaFunction.GetExtraTexture("Yuka_Cloth"), true);
            }
            return false;
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
        #region 溶液喷洒相关
        private Item solutionClone;
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
            Solution = player.ChooseAmmo(Sprayer);
            if (Solution == null || Solution.IsAir)
            {
                PetState = Phase_StopSpray;
                Projectile.netUpdate = true;
                return;
            }
            else
            {
                solutionClone = new(Solution.type);
                solutionClone.shoot = SolutionSprayType(Solution.type);
            }
            if (Projectile.frame >= 8)
            {
                Projectile.frame = 8;
                if (++extraAI[1] > 45)
                {
                    extraAI[1] = 0;
                    if (Projectile.owner == Main.myPlayer)
                    {
                        if (Main.rand.NextBool(2, 3) && Solution.consumable && Solution.ammo > AmmoID.None)
                            Solution.stack--;
                    }
                }

                if (mode == 1)
                {
                    angle += 2;
                    if (angle > 359)
                        angle = 0;
                }
                else
                {
                    angle = (int)MathHelper.ToDegrees((mousePos - YukaHandOrigin).ToRotation() + MathHelper.PiOver2);
                }

                if (Projectile.frameCounter % 2 == 0)
                {
                    Vector2 pos = YukaHandOrigin + new Vector2(0, 7f * Main.essScale);
                    pos += new Vector2(0, -48).RotatedBy(MathHelper.ToRadians(angle));

                    for (int i = 0; i < 5; i++)
                    {
                        Dust.NewDustPerfect(pos, SolutionSprayDust(solutionClone.shoot)
                        , new Vector2(0, Main.rand.NextFloat(2.4f, 4.8f)).RotatedByRandom(MathHelper.TwoPi), 100
                        , default, Main.rand.NextFloat(0.5f, 2f)).noGravity = true;
                    }

                    if (Projectile.owner == Main.myPlayer)
                    {
                        Projectile.NewProjectileDirect(Projectile.GetSource_FromThis()
                        , Projectile.Center + new Vector2(-2 * Projectile.spriteDirection, 12)
                        , new Vector2(0, -Sprayer.shootSpeed).RotatedBy(MathHelper.ToRadians(angle))
                        , solutionClone.shoot, Sprayer.damage, Sprayer.knockBack, player.whoAmI);
                    }
                }
            }
            if (Solution.stack <= 0)
            {
                Solution.TurnToAir();
                solutionClone.TurnToAir();
                extraAI[1] = 0;
                PetState = Phase_StopSpray;
                Projectile.netUpdate = true;
                SprayState = -1;
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
                Projectile.netUpdate = true;
                SprayState = -1;
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
        Vector2 mousePos = Vector2.Zero;
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
        public override Color ChatTextColor => new(107, 252, 75);
        public override void RegisterChat(ref string name, ref Vector2 indexRange)
        {
            name = "Yuka";
            indexRange = new Vector2(1, 3);
        }
        public override void SetRegularDialog(ref int timePerDialog, ref int chance, ref bool whenShouldStop)
        {
            timePerDialog = 900;
            chance = 12;
            whenShouldStop = PetState > 1;
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
        public override void VisualEffectForPreview()
        {
            UpdateClothFrame();
        }
        private void UpdateTalking()
        {
        }
        
        public override void AI()
        {
            if (Projectile.owner == Main.myPlayer)
            {
                mousePos = Main.MouseWorld;
                if (mainTimer % 5 == 0)
                    Projectile.netUpdate = true;
            }
            Player player = Main.player[Projectile.owner];
            Projectile.SetPetActive(player, BuffType<YukaBuff>());
            UpdateTalking();
            Vector2 point = new(-50 * player.direction, -45 + player.gfxOffY);
            if (PetState == Phase_Spray_Mode2)
            {
                point = mousePos - player.Center;
            }
            Projectile.tileCollide = false;
            Projectile.rotation = Projectile.velocity.X * 0.005f;

            ChangeDir(true);
            MoveToPoint(point, 12f);
            if (Projectile.owner == Main.myPlayer)
            {
                if (mainTimer % 270 == 0 && PetState == 0)
                {
                    PetState = 1;
                    Projectile.netUpdate = true;
                }
                if (PetState != SprayState && SprayState > 0)
                {
                    PetState = SprayState;
                    Projectile.netUpdate = true;
                }
                /*if (mainTimer >= 400 && mainTimer < 3600 && PetState <= 1 && extraAI[0] == 0)
                {
                    if (mainTimer % 600 == 0 && Main.rand.NextBool(1))
                    {
                        PetState = 2;
                        extraAI[2] = Main.rand.Next(600, 900);
                    }
                }*/
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
        public override void SendExtraAI(BinaryWriter writer)
        {
            base.SendExtraAI(writer);
            writer.WriteVector2(mousePos);

            if (PetState == Phase_Spray_Mode2)
                writer.Write(angle);
        }
        public override void ReceiveExtraAI(BinaryReader reader)
        {
            base.ReceiveExtraAI(reader);
            mousePos = reader.ReadVector2();

            if (PetState == Phase_Spray_Mode2)
                angle = reader.ReadInt32();
        }
    }
}


