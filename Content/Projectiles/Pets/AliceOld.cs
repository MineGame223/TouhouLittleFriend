using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ID;
using Terraria.Utilities;
using TouhouPets.Content.Buffs.PetBuffs;

namespace TouhouPets.Content.Projectiles.Pets
{
    public class AliceOld : BasicTouhouPet
    {
        private enum States
        {
            Idle,
            Blink,
            Reading,
            ReadingBlink,
            AfterReading,
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
        private bool IsIdleState => CurrentState <= States.Blink;
        public override void SetStaticDefaults()
        {
            Main.projFrames[Type] = 10;
            Main.projPet[Type] = true;
            ProjectileID.Sets.LightPet[Type] = true;
        }
        public override TouhouPetID UniqueID => TouhouPetID.AliceLegacy;

        private int wingFrame, wingFrameCounter;
        private int clothFrame, clothFrameCounter;
        private int blinkFrame, blinkFrameCounter;

        private DrawPetConfig drawConfig = new(2);
        private readonly Texture2D clothTex = AltVanillaFunction.GetExtraTexture("AliceOld_Cloth");

        public override bool DrawPetSelf(ref Color lightColor)
        {
            DrawPetConfig config = drawConfig;
            DrawPetConfig config2 = config with
            {
                ShouldUseEntitySpriteDraw = true,
            };

            Projectile.DrawPet(wingFrame, lightColor, config2, 1);
            Projectile.DrawPet(wingFrame, Color.White * 0.8f, config2, 1);
            Projectile.ResetDrawStateForPet();

            Projectile.DrawPet(4, lightColor, config);
            Projectile.DrawPet(4, lightColor,
                config2 with
                {
                    AltTexture = clothTex,
                });
            Projectile.ResetDrawStateForPet();

            if (CurrentState >= States.Blink)
                Projectile.DrawPet(blinkFrame, lightColor, config);

            Projectile.DrawPet(clothFrame, lightColor, config2, 1);
            Projectile.ResetDrawStateForPet();

            Projectile.DrawPet(Projectile.frame, lightColor, config);
            Projectile.DrawPet(Projectile.frame, lightColor,
                config2 with
                {
                    AltTexture = clothTex,
                });
            return false;
        }
        public override ChatSettingConfig ChatSettingConfig => new ChatSettingConfig() with
        {
            TextColor = new Color(185, 228, 255),
        };
        public override void RegisterChat(ref string name, ref Vector2 indexRange)
        {
            name = "AliceOld";
            indexRange = new Vector2(1, 7);
        }
        public override void SetRegularDialog(ref int timePerDialog, ref int chance, ref bool whenShouldStop)
        {
            timePerDialog = 880;
            chance = 8;
            whenShouldStop = false;
        }
        public override WeightedRandom<string> RegularDialogText()
        {
            WeightedRandom<string> chat = new();
            {
                chat.Add(ChatDictionary[1]);
                chat.Add(ChatDictionary[2]);
                chat.Add(ChatDictionary[3]);
                chat.Add(ChatDictionary[4]);
                if (FindPet(ProjectileType<Yuka>(), false))
                {
                    chat.Add(ChatDictionary[5]);
                }
                if (FindPet(ProjectileType<Reimu>(), false))
                {
                    chat.Add(ChatDictionary[7]);
                }
            }
            return chat;
        }
        private void UpdateTalking()
        {
        }
        public override void VisualEffectForPreview()
        {
            UpdateMiscFrame();
        }
        public override void SetPetLight(ref Vector2 position, ref Vector3 rgb, ref bool inactive)
        {
            rgb = new Vector3(0.41f, 1.69f, 2.55f);
        }
        public override void AI()
        {
            Projectile.SetPetActive(Owner, BuffType<AliceOldBuff>());

            UpdateTalking();

            ControlMovement();

            switch (CurrentState)
            {
                case States.Blink:
                    Blink();
                    break;

                case States.Reading:
                    shouldNotTalking = true;
                    Reading();
                    break;

                case States.ReadingBlink:
                    shouldNotTalking = true;
                    Reading();
                    ReadingBlink();
                    break;

                case States.AfterReading:
                    shouldNotTalking = true;
                    AfterReading();
                    break;

                default:
                    Idle();
                    break;
            }

            if (IsIdleState && ActionCD > 0)
            {
                ActionCD--;
            }
        }
        private void ControlMovement()
        {
            Projectile.tileCollide = false;
            Projectile.rotation = Projectile.velocity.X * 0.012f;

            ChangeDir();

            Vector2 point = new Vector2(50 * Owner.direction, -30 + Owner.gfxOffY);
            MoveToPoint(point, 13f);
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
                if (mainTimer > 0 && mainTimer % 480 == 0 && currentChatRoom == null && ActionCD <= 0
                     && Projectile.velocity.Length() < 2f)
                {
                    if (Main.rand.NextBool(4))
                    {
                        RandomCount = Main.rand.Next(960, 1540);
                        CurrentState = States.Reading;
                    }
                }
            }
        }
        private void Reading()
        {
            if (mainTimer % 270 == 0)
            {
                CurrentState = States.ReadingBlink;
            }
            if (++Projectile.frameCounter > 10)
            {
                Projectile.frameCounter = 0;
                Projectile.frame++;
            }
            if (blinkFrame < 6)
            {
                blinkFrame = 6;
            }
            if (Projectile.frame >= 2)
            {
                Projectile.frame = 2;
                if (OwnerIsMyPlayer)
                {
                    if (Timer < RandomCount)
                    {
                        Timer++;
                    }
                    else
                    {
                        Timer = 0;
                        CurrentState = States.AfterReading;
                    }
                }
            }
        }
        private void ReadingBlink()
        {
            if (blinkFrame < 6)
            {
                blinkFrame = 6;
            }
            if (++blinkFrameCounter > 3)
            {
                blinkFrameCounter = 0;
                blinkFrame++;
            }
            if (blinkFrame > 7)
            {
                blinkFrame = 6;
                CurrentState = States.Reading;
            }
        }
        private void AfterReading()
        {
            blinkFrame = 7;
            if (++Projectile.frameCounter > 10)
            {
                Projectile.frameCounter = 0;
                Projectile.frame++;
            }
            if (Projectile.frame > 3)
            {
                Projectile.frame = 0;
                if (OwnerIsMyPlayer)
                {
                    ActionCD = 480;
                    CurrentState = States.Idle;
                }
            }
        }
        private void Blink()
        {
            if (blinkFrame < 5)
            {
                blinkFrame = 5;
            }
            if (++blinkFrameCounter > 3)
            {
                blinkFrameCounter = 0;
                blinkFrame++;
            }
            if (blinkFrame > 7)
            {
                blinkFrame = 5;
                CurrentState = States.Idle;
            }
        }
        private void UpdateMiscFrame()
        {
            if (wingFrame < 4)
            {
                wingFrame = 4;
            }
            if (++wingFrameCounter > 6)
            {
                wingFrameCounter = 0;
                wingFrame++;
            }
            if (wingFrame > 9)
            {
                wingFrame = 4;
            }

            if (++clothFrameCounter > 8)
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


