using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ID;
using Terraria.Utilities;
using TouhouPets.Content.Buffs.PetBuffs;

namespace TouhouPets.Content.Projectiles.Pets
{
    public class Aya : BasicTouhouPet
    {
        private enum States
        {
            Idle,
            Blink,
            Shot,
            ShotBreak,
            ShotContinue,
            AfterShot,
        }
        private States CurrentState
        {
            get => (States)PetState;
            set => PetState = (int)value;
        }
        private int ActionCD
        {
            get => (int)Projectile.localAI[0];
            set => Projectile.localAI[0] = value;
        }
        private int Timer
        {
            get => (int)Projectile.localAI[1];
            set => Projectile.localAI[1] = value;
        }
        private int RandomCount
        {
            get => (int)Projectile.localAI[2];
            set => Projectile.localAI[2] = value;
        }
        private bool ReadyToShot
        {
            get => Projectile.ai[2] == 1;
            set => Projectile.ai[2] = value ? 1 : 0;
        }
        private bool IsIdleState => CurrentState <= States.Blink;

        private int wingFrame, wingFrameCounter;
        private int blinkFrame, blinkFrameCounter;
        private int clothFrame, clothFrameCounter;
        private int extraAdjX, extraAdjY;
        private float flash;
        private int flashChance, flashRandomCount;

        private DrawPetConfig drawConfig = new(2);
        private readonly Texture2D clothTex = AltVanillaFunction.GetExtraTexture("Aya_Cloth");
        public override void SetStaticDefaults()
        {
            Main.projFrames[Type] = 15;
            Main.projPet[Type] = true;

            ProjectileID.Sets.CharacterPreviewAnimations[Type] =
                ProjectileID.Sets.SimpleLoop(0, 1)
                .WhenSelected(3, 1);
        }
        public override TouhouPetID UniqueID => TouhouPetID.Aya;
        public override bool DrawPetSelf(ref Color lightColor)
        {
            DrawPetConfig config = drawConfig with
            {
                PositionOffset = new Vector2(extraAdjX, extraAdjY),
            };
            DrawPetConfig config2 = config with
            {
                ShouldUseEntitySpriteDraw = true,
            };

            Projectile.DrawPet(wingFrame, lightColor, config);
            Projectile.DrawPet(clothFrame + 4, lightColor,
                drawConfig with
                {
                    ShouldUseEntitySpriteDraw = true,
                }, 1);
            Projectile.ResetDrawStateForPet();

            Projectile.DrawPet(Projectile.frame, lightColor, drawConfig);

            if (CurrentState == States.Blink)
                Projectile.DrawPet(blinkFrame, lightColor, drawConfig);

            Projectile.DrawPet(clothFrame, lightColor, config2, 1);
            Projectile.DrawPet(Projectile.frame, lightColor,
                config2 with
                {
                    AltTexture = clothTex,
                });
            Projectile.DrawPet(clothFrame + 8, lightColor, config2, 1);

            if (Projectile.frame == 3)
            {
                Projectile.ResetDrawStateForPet();
                DrawShotSpark();
            }
            return false;
        }
        private void DrawShotSpark()
        {
            Texture2D t = AltVanillaFunction.ExtraTexture(ExtrasID.ThePerfectGlow);
            Vector2 pos = Projectile.Center + new Vector2(14 * Projectile.spriteDirection, -10) - Main.screenPosition + new Vector2(0, 7f * Main.essScale);
            Rectangle rect = new Rectangle(0, 0, t.Width, t.Height);
            Vector2 orig = rect.Size() / 2;
            Color clr = Projectile.GetAlpha(Color.White * flash).ModifiedAlphaColor() * mouseOpacity;
            SpriteEffects effect = Projectile.spriteDirection == -1 ? SpriteEffects.None : SpriteEffects.FlipHorizontally;
            Main.spriteBatch.MyDraw(t, pos, rect, clr, Projectile.rotation, orig, new Vector2(0.4f, 0.5f) * flash * 1.6f, effect, 0f);
            Main.spriteBatch.MyDraw(t, pos, rect, clr, Projectile.rotation + MathHelper.Pi / 2, orig, new Vector2(0.5f, 1f) * flash * 1.6f, effect, 0f);
        }
        public override ChatSettingConfig ChatSettingConfig => new ChatSettingConfig() with
        {
            TextColor = new Color(255, 102, 85),
        };
        public override void RegisterChat(ref string name, ref Vector2 indexRange)
        {
            name = "Aya";
            indexRange = new Vector2(1, 8);
        }
        public override void SetRegularDialog(ref int timePerDialog, ref int chance, ref bool whenShouldStop)
        {
            timePerDialog = 720;
            chance = 7;
            whenShouldStop = IsIdleState;
        }
        public override WeightedRandom<string> RegularDialogText()
        {
            WeightedRandom<string> chat = new();
            {
                chat.Add(ChatDictionary[1]);
                chat.Add(ChatDictionary[2]);
                chat.Add(ChatDictionary[3]);
                chat.Add(ChatDictionary[4]);
            }
            return chat;
        }
        private void SetShotChat()
        {
            int chance = Main.rand.Next(6);
            switch (chance)
            {
                case 1:
                    Projectile.SetChat(ChatSettingConfig, 5);
                    break;
                case 2:
                    Projectile.SetChat(ChatSettingConfig, 6);
                    break;
                case 3:
                    Projectile.SetChat(ChatSettingConfig, 7);
                    break;
                case 4:
                    Projectile.SetChat(ChatSettingConfig, 8);
                    break;
                default:
                    break;
            }
        }
        public override void VisualEffectForPreview()
        {
            UpdateWingFrame();
            UpdateClothFrame();
        }
        public override void AI()
        {
            Projectile.SetPetActive(Owner, BuffType<AyaBuff>());

            ControlMovement();

            switch (CurrentState)
            {
                case States.Blink:
                    Blink();
                    break;

                case States.Shot:
                    shouldNotTalking = true;
                    Shot();
                    break;

                case States.ShotBreak:
                    shouldNotTalking = true;
                    ShotBreak();
                    break;

                case States.ShotContinue:
                    shouldNotTalking = true;
                    ShotContinue();
                    break;

                case States.AfterShot:
                    shouldNotTalking = true;
                    AfterShot();
                    break;

                default:
                    Idle();
                    break;
            }

            if (IsIdleState && ActionCD > 0)
            {
                ActionCD--;
            }

            UpdateMiscData();

            Lighting.AddLight(Projectile.Center + new Vector2(14 * Projectile.spriteDirection, -10)
                , 2.55f * flash, 2.55f * flash, 2.55f * flash);
        }
        private void UpdateMiscData()
        {
            if (flash > 0)
            {
                flash -= 0.1f;
            }
            extraAdjX = 0;
            extraAdjY = 0;
            if (Projectile.frame == 3)
            {
                extraAdjY = -2;
            }
        }
        private void ControlMovement()
        {
            Projectile.tileCollide = false;
            if (IsIdleState)
                Projectile.rotation = Projectile.velocity.X * 0.035f;
            else
                Projectile.rotation = Projectile.velocity.X * 0.002f;

            ChangeDir();

            Vector2 point = new Vector2(-70 * Owner.direction, -50 + Owner.gfxOffY);
            MoveToPoint(point, 30f);
        }
        private void Idle()
        {
            Projectile.frame = 0;
            if (OwnerIsMyPlayer)
            {
                if (mainTimer % 270 == 0)
                {
                    CurrentState = States.Blink;
                }
                if (mainTimer > 0 && mainTimer % 600 == 0 && currentChatRoom == null && ActionCD <= 0)
                {
                    if (Main.rand.NextBool(3))
                    {
                        RandomCount = Main.rand.Next(1, 5);
                        flashChance = 6;
                        CurrentState = States.Shot;
                    }
                }
            }
        }
        private void Blink()
        {
            Projectile.frame = 0;
            if (blinkFrame < 6)
            {
                blinkFrame = 6;
            }
            if (++blinkFrameCounter > 3)
            {
                blinkFrameCounter = 0;
                blinkFrame++;
            }
            if (blinkFrame > 8)
            {
                blinkFrame = 6;
                PetState = 0;
            }
        }
        private void Shot()
        {
            Projectile.velocity *= 0.9f;
            if (++Projectile.frameCounter > 7)
            {
                Projectile.frameCounter = 0;
                Projectile.frame++;
            }
            if (Projectile.frame >= 3)
            {
                Projectile.frame = 3;
            }
            if (ReadyToShot)//准备拍照
            {
                flash = 1;//设置闪光
                AltVanillaFunction.PlaySound(SoundID.Camera, Projectile.Center);
                ReadyToShot = false;
            }
            if (OwnerIsMyPlayer)
            {
                if (Timer == 0)
                {
                    //设置举相机的持续时长
                    flashRandomCount = Main.rand.Next(180, 600);

                    if (Main.rand.NextBool(3))
                        SetShotChat();
                }
                Timer++;
                //大于时长则进入休息阶段
                if (Timer >= flashRandomCount)
                {
                    Timer = Main.rand.Next(30, 60);//设置休息阶段时长
                    CurrentState = States.ShotBreak;
                }
                else if (Timer % 30 == 0 && Main.rand.NextBool(7 - flashChance))//每30刻就有概率进行一次拍照
                {
                    flashChance -= 2;//减少下次拍照的概况
                    ReadyToShot = true;//准备拍照
                    Projectile.netUpdate = true;
                }
            }
        }
        private void ShotBreak()
        {
            Projectile.velocity *= 0.9f;
            if (++Projectile.frameCounter > 7)
            {
                Projectile.frameCounter = 0;
                Projectile.frame++;
            }
            if (Projectile.frame >= 5)
            {
                Projectile.frame = 5;
            }

            Timer--;

            if (OwnerIsMyPlayer && Timer < 0)
            {
                RandomCount--;
                Timer = 0;
                CurrentState = (RandomCount > 0) ? States.ShotContinue : States.AfterShot;
            }
        }
        private void ShotContinue()
        {
            Projectile.frame = 1;
            if (OwnerIsMyPlayer)
            {
                flashChance = 6;//使拍照概率回到最大
                CurrentState = States.Shot;
            }
        }
        private void AfterShot()
        {
            if (++Projectile.frameCounter > 7)
            {
                Projectile.frameCounter = 0;
                Projectile.frame++;
            }
            if (Projectile.frame > 5)
            {
                Projectile.frame = 0;
                if (OwnerIsMyPlayer)
                {
                    ActionCD = 1800;
                    CurrentState = States.Idle;
                }
            }
        }
        private void UpdateWingFrame()
        {
            if (wingFrame < 9)
            {
                wingFrame = 9;
            }
            int count = 5;
            if (++wingFrameCounter > count)
            {
                wingFrameCounter = 0;
                wingFrame++;
            }
            if (wingFrame > 14)
            {
                wingFrame = 9;
            }
        }
        private void UpdateClothFrame()
        {
            int count = 5;
            if (++clothFrameCounter > count)
            {
                clothFrameCounter = 0;
                clothFrame++;
            }
            if (clothFrame > 3)
            {
                clothFrame = 0;
            }
        }
    }
}


