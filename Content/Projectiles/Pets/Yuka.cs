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
    public class Yuka : BasicTouhouPet
    {
        private enum States
        {
            Idle,
            Blink,
            Spraying = Phase_Spray_Mode1,
            Spraying2 = Phase_Spray_Mode2,
            StopSpraying = Phase_StopSpray,
        }
        private States CurrentState
        {
            get => (States)PetState;
            set => PetState = (int)value;
        }
        private int Angle
        {
            get => (int)Projectile.localAI[0];
            set => Projectile.localAI[0] = value;
        }
        private int Timer
        {
            get => (int)Projectile.localAI[1];
            set => Projectile.localAI[1] = value;
        }
        private bool IsIdleState => CurrentState <= States.Blink;
        private Vector2 YukaHandOrigin => Projectile.Center + new Vector2(-2 * Projectile.spriteDirection, 2);

        private int clothFrame, clothFrameCounter;
        private int blinkFrame, blinkFrameCounter;
        private Vector2 mousePos = Vector2.Zero;

        private Item solutionClone;

        private DrawPetConfig drawConfig = new(2)
        {
            PositionOffset = new Vector2(0, -10),
        };
        private readonly Texture2D clothTex = AltVanillaFunction.GetExtraTexture("Yuka_Cloth");
        public override void SetStaticDefaults()
        {
            Main.projFrames[Type] = 11;
            Main.projPet[Type] = true;
            ProjectileID.Sets.LightPet[Type] = false;
        }
        public override bool DrawPetSelf(ref Color lightColor)
        {
            DrawPetConfig config = drawConfig with
            {
                ShouldUseEntitySpriteDraw = true,
            };

            Projectile.DrawPet(Projectile.frame, lightColor, drawConfig);

            if (CurrentState == States.Blink)
                Projectile.DrawPet(blinkFrame, lightColor, drawConfig, 1);

            Projectile.DrawPet(Projectile.frame, lightColor,
                config with
                {
                    AltTexture = clothTex,
                });
            Projectile.DrawPet(clothFrame, lightColor, config, 1);
            Projectile.ResetDrawStateForPet();

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
            float rot = MathHelper.ToRadians(Angle);
            SpriteEffects effect = Projectile.spriteDirection == -1 ? SpriteEffects.None : SpriteEffects.FlipHorizontally;
            if (entitySpriteDraw)
                Main.EntitySpriteDraw(t, pos, rect, Projectile.GetAlpha(lightColor), rot, orig, Projectile.scale, effect, 0f);
            else
            {
                Main.spriteBatch.TeaNPCDraw(t, pos, rect, Projectile.GetAlpha(lightColor), rot, orig, Projectile.scale, effect, 0f);
            }
        }
        public override Color ChatTextColor => new(107, 252, 75);
        public override void RegisterChat(ref string name, ref Vector2 indexRange)
        {
            name = "Yuka";
            indexRange = new Vector2(1, 6);
        }
        public override void SetRegularDialog(ref int timePerDialog, ref int chance, ref bool whenShouldStop)
        {
            timePerDialog = 900;
            chance = 12;
            whenShouldStop = !IsIdleState;
        }
        public override string GetRegularDialogText()
        {
            WeightedRandom<string> chat = new WeightedRandom<string>();
            {
                chat.Add(ChatDictionary[1]);
                chat.Add(ChatDictionary[2]);
                chat.Add(ChatDictionary[3]);
                chat.Add(ChatDictionary[5]);
                chat.Add(ChatDictionary[6]);
                if (FindPet(ProjectileType<AliceOld>(), false))
                {
                    chat.Add(ChatDictionary[4]);
                }
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
            if (OwnerIsMyPlayer)
            {
                mousePos = Main.MouseWorld;
                if (mainTimer % 5 == 0)
                    Projectile.netUpdate = true;
            }

            Projectile.SetPetActive(Owner, BuffType<YukaBuff>());

            UpdateTalking();

            ControlMovement();

            switch (CurrentState)
            {
                case States.Blink:
                    Blink();
                    break;

                case States.Spraying:
                    shouldNotTalking = true;
                    Spraying(0);
                    break;

                case States.Spraying2:
                    shouldNotTalking = true;
                    Spraying(1);
                    break;

                case States.StopSpraying:
                    shouldNotTalking = true;
                    StopSpraying();
                    break;

                default:
                    Idle();
                    break;
            }
        }
        public override void SendExtraAI(BinaryWriter writer)
        {
            base.SendExtraAI(writer);
            writer.WriteVector2(mousePos);
            writer.Write(Angle);
        }
        public override void ReceiveExtraAI(BinaryReader reader)
        {
            base.ReceiveExtraAI(reader);
            mousePos = reader.ReadVector2();
            Angle = reader.ReadInt32();
        }
        private void ControlMovement()
        {
            Projectile.tileCollide = false;
            Projectile.rotation = Projectile.velocity.X * 0.005f;

            ChangeDir();

            Vector2 point = new(-50 * Owner.direction, -45 + Owner.gfxOffY);
            if (PetState == Phase_Spray_Mode2)
            {
                point = mousePos - Owner.Center;
            }
            MoveToPoint(point, 12f);
        }
        private void Idle()
        {
            Projectile.frame = 0;
            if (OwnerIsMyPlayer && mainTimer % 270 == 0)
            {
                CurrentState = States.Blink;
            }
        }
        private void Blink()
        {
            Projectile.frame = 0;
            if (++blinkFrameCounter > 3)
            {
                blinkFrameCounter = 0;
                blinkFrame++;
            }
            if (blinkFrame > 2)
            {
                blinkFrame = 0;
                CurrentState = States.Idle;
            }
        }

        #region 溶液喷洒相关
        private void Spraying(int mode)
        {
            if (Projectile.frame < 5)
            {
                Projectile.frame = 5;
            }
            if (++Projectile.frameCounter > 4)
            {
                Projectile.frameCounter = 0;
                Projectile.frame++;
            }
            Solution = Owner.ChooseAmmo(Sprayer);
            if (Solution == null || Solution.IsAir)
            {
                solutionClone.TurnToAir();
                CurrentState = States.StopSpraying;
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
                if (++Timer > 45)
                {
                    Timer = 0;
                    if (OwnerIsMyPlayer)
                    {
                        if (Main.rand.NextBool(2, 3) && Solution.consumable && Solution.ammo > AmmoID.None)
                            Solution.stack--;
                    }
                }

                if (mode == 1)
                {
                    Angle += 2;
                    if (Angle > 359)
                        Angle = 0;
                }
                else
                {
                    Angle = (int)MathHelper.ToDegrees((mousePos - YukaHandOrigin).ToRotation() + MathHelper.PiOver2);
                }

                if (Projectile.frameCounter % 2 == 0)
                {
                    Vector2 pos = YukaHandOrigin + new Vector2(0, 7f * Main.essScale);
                    pos += new Vector2(0, -48).RotatedBy(MathHelper.ToRadians(Angle));

                    for (int i = 0; i < 5; i++)
                    {
                        Dust.NewDustPerfect(pos, SolutionSprayDust(solutionClone.shoot)
                        , new Vector2(0, Main.rand.NextFloat(2.4f, 4.8f)).RotatedByRandom(MathHelper.TwoPi), 100
                        , default, Main.rand.NextFloat(0.5f, 2f)).noGravity = true;
                    }

                    if (OwnerIsMyPlayer)
                    {
                        Projectile.NewProjectileDirect(Projectile.GetSource_FromThis()
                        , Projectile.Center + new Vector2(-2 * Projectile.spriteDirection, 12)
                        , new Vector2(0, -Sprayer.shootSpeed).RotatedBy(MathHelper.ToRadians(Angle))
                        , solutionClone.shoot, Sprayer.damage, Sprayer.knockBack, Owner.whoAmI);
                    }
                }
            }
        }
        private void StopSpraying()
        {
            if (Projectile.frame < 8)
            {
                Projectile.frame = 8;
            }
            if (++Projectile.frameCounter > 4 && Angle <= 0)
            {
                Projectile.frameCounter = 0;
                Projectile.frame++;
            }
            if (Angle > 359 || Angle < 0)
            {
                Angle = 0;
            }
            else
            {
                int rate = 9;
                if (Angle <= 359 && Angle >= 180)
                {
                    Angle += rate;
                }
                else if (Angle < 180)
                {
                    Angle -= rate;
                }
            }
            if (Projectile.frame > 10)
            {
                Projectile.frame = 0;
                if (OwnerIsMyPlayer)
                {
                    Timer = 0;
                    CurrentState = States.Idle;
                }
            }
        }
        #endregion
        private void UpdateClothFrame()
        {
            int count = IsSpraying ? 3 : 5;
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
    }
}


