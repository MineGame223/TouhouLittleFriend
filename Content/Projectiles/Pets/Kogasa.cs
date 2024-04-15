using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.Utilities;
using TouhouPets.Content.Buffs.PetBuffs;

namespace TouhouPets.Content.Projectiles.Pets
{
    public class Kogasa : BasicTouhouPetNeo
    {
        private enum States
        {
            Idle,
            Blink,
            Umbrella,
            UmbrellaBlink,
            MakeFace,
            AfterMakeFace,
            Afraid,
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
        private bool IsIdleState => PetState <= 1;
        private bool IsUmberllaState => CurrentState == States.Umbrella || CurrentState == States.UmbrellaBlink;
        private bool RainWet => Main.raining && (Owner.ZoneOverworldHeight || Owner.ZoneSkyHeight);

        private int blinkFrame, blinkFrameCounter;
        private int handFrame, handFrameCounter;
        private int clothFrame, clothFrameCounter;
        private int umbrellaFrame, umbrellaFrameCounter;

        private DrawPetConfig drawConfig = new(2);
        private readonly Texture2D clothTex = AltVanillaFunction.GetExtraTexture("Kogasa_Cloth");
        public override void SetStaticDefaults()
        {
            Main.projFrames[Type] = 20;
            Main.projPet[Type] = true;
        }
        public override bool PreDraw(ref Color lightColor)
        {
            DrawPetConfig config = drawConfig with
            {
                AltTexture = clothTex,
                ShouldUseEntitySpriteDraw = true,
            };
            int eyeFramePlus = Projectile.spriteDirection == -1 ? 0 : 3;

            Projectile.DrawPet(umbrellaFrame, lightColor, drawConfig, 1);
            Projectile.DrawPet(umbrellaFrame, lightColor, config, 1);
            Projectile.DrawStateNormalizeForPet();

            Projectile.DrawPet(Projectile.frame, lightColor, drawConfig);

            if (IsIdleState || IsUmberllaState)
                Projectile.DrawPet(blinkFrame + eyeFramePlus, lightColor, drawConfig);

            if (CurrentState == States.MakeFace || CurrentState == States.AfterMakeFace)
            {
                if (Projectile.frame == 1 || Projectile.frame == 4)
                    Projectile.DrawPet(15 + eyeFramePlus, lightColor, drawConfig);
            }

            Projectile.DrawPet(Projectile.frame, lightColor, config);
            Projectile.DrawPet(clothFrame, lightColor,
                drawConfig with
                {
                    ShouldUseEntitySpriteDraw = true,
                });
            Projectile.DrawStateNormalizeForPet();

            Projectile.DrawPet(handFrame, lightColor, drawConfig);
            Projectile.DrawPet(handFrame, lightColor, config);
            return false;
        }
        public override Color ChatTextColor => new Color(172, 69, 191);
        public override void RegisterChat(ref string name, ref Vector2 indexRange)
        {
            name = "Kogasa";
            indexRange = new Vector2(1, 7);
        }
        public override void SetRegularDialog(ref int timePerDialog, ref int chance, ref bool whenShouldStop)
        {
            timePerDialog = 888;
            chance = 8;
            whenShouldStop = !IsIdleState && !IsUmberllaState;
        }
        public override string GetRegularDialogText()
        {
            WeightedRandom<string> chat = new WeightedRandom<string>();
            {
                chat.Add(ChatDictionary[1]);
                chat.Add(ChatDictionary[2]);
                chat.Add(ChatDictionary[3]);
                if (RainWet)
                {
                    chat.Add(ChatDictionary[4]);
                    chat.Add(ChatDictionary[5]);
                    chat.Add(ChatDictionary[6]);
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
            UpdateClothFrame();
            UpdateUmbrellaFrame();
            UpdateHandFrame();
        }
        public override void AI()
        {
            if (blinkFrame < 14)
            {
                blinkFrame = 14;
            }

            Projectile.SetPetActive(Owner, BuffType<KogasaBuff>());

            UpdateTalking();

            ControlMovement();

            if (Owner.AnyBosses())
            {
                CurrentState = States.Afraid;
            }

            switch (CurrentState)
            {
                case States.Blink:
                    Blink();
                    break;

                case States.Umbrella:
                    Umbrella();
                    break;

                case States.UmbrellaBlink:
                    UmbrellaBlink();
                    break;

                case States.MakeFace:
                    shouldNotTalking = true;
                    MakeFace();
                    break;

                case States.AfterMakeFace:
                    shouldNotTalking = true;
                    AfterMakeFace();
                    break;

                case States.Afraid:
                    shouldNotTalking = true;
                    Afraid();
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

            Vector2 point = new Vector2(-50 * Owner.direction, -30 + Owner.gfxOffY);
            MoveToPoint(point, 12.5f);
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
                if (mainTimer > 0 && mainTimer % 320 == 0 && ActionCD <= 0)
                {
                    if (Main.rand.NextBool(2))
                    {
                        if (Main.rand.NextBool(3) && chatTimeLeft <= 0)
                        {
                            RandomCount = Main.rand.Next(6, 12);
                            CurrentState = States.MakeFace;
                        }
                        else
                        {
                            RandomCount = Main.rand.Next(17, 30);
                            CurrentState = States.Umbrella;
                        }
                    }
                }
            }
        }
        private void Blink()
        {
            Projectile.frame = 0;
            if (blinkFrame < 14)
            {
                blinkFrame = 14;
            }
            if (++blinkFrameCounter > 3)
            {
                blinkFrameCounter = 0;
                blinkFrame++;
            }
            if (blinkFrame > 16)
            {
                blinkFrame = 14;
                CurrentState = States.Idle;
            }
        }
        private void Umbrella()
        {
            Projectile.frame = 0;
            if (OwnerIsMyPlayer && mainTimer % 270 == 0)
            {
                CurrentState = States.UmbrellaBlink;
            }
        }
        private void UmbrellaBlink()
        {
            Projectile.frame = 0;
            if (blinkFrame < 14)
            {
                blinkFrame = 14;
            }
            if (++blinkFrameCounter > 3)
            {
                blinkFrameCounter = 0;
                blinkFrame++;
            }
            if (blinkFrame > 16)
            {
                blinkFrame = 14;
                CurrentState = States.Umbrella;
            }
        }
        private void MakeFace()
        {
            if (++Projectile.frameCounter > 7)
            {
                Projectile.frameCounter = 0;
                Projectile.frame++;
            }
            if (Projectile.frame > 3)
            {
                Projectile.frame = 2;
                Timer++;
                if (OwnerIsMyPlayer && Timer > RandomCount)
                {
                    Timer = 0;
                    CurrentState = States.AfterMakeFace;
                }
            }
        }
        private void AfterMakeFace()
        {
            if (++Projectile.frameCounter > 7)
            {
                Projectile.frameCounter = 0;
                Projectile.frame++;
            }
            if (Projectile.frame > 4)
            {
                Projectile.frame = 0;
                if (OwnerIsMyPlayer)
                {
                    ActionCD = 600;
                    CurrentState = States.Idle;
                }
            }
        }
        private void Afraid()
        {
            Projectile.frame = 5;
            umbrellaFrame = 14;
            handFrame = 13;
            if (OwnerIsMyPlayer && !Owner.AnyBosses())
            {
                CurrentState = States.Idle;
            }
        }
        private void UpdateUmbrellaFrame()
        {
            if (CurrentState == States.Afraid)
            {
                return;
            }
            if (++umbrellaFrameCounter > 5)
            {
                umbrellaFrameCounter = 0;
                umbrellaFrame++;
            }
            if (!IsUmberllaState)
            {
                if (umbrellaFrame > 6)
                {
                    umbrellaFrame = 0;
                }
            }
            else
            {
                if (umbrellaFrame > 13)
                {
                    Timer++;
                    umbrellaFrame = 7;
                    if (Timer > RandomCount && OwnerIsMyPlayer)
                    {
                        Timer = 0;
                        ActionCD = 1200;
                        CurrentState = States.Idle;
                    }
                }
            }
        }
        private void UpdateHandFrame()
        {
            if (CurrentState == States.Afraid)
            {
                return;
            }
            if (IsUmberllaState)
            {
                if (handFrame < 11)
                {
                    handFrame = 11;
                }
                if (++handFrameCounter > 5)
                {
                    handFrameCounter = 0;
                    handFrame++;
                }
                if (handFrame > 12)
                {
                    handFrame = 11;
                }
            }
            else
            {
                handFrame = 10;
            }
        }
        private void UpdateClothFrame()
        {
            if (clothFrame < 6)
            {
                clothFrame = 6;
            }
            if (++clothFrameCounter > 5)
            {
                clothFrameCounter = 0;
                clothFrame++;
            }
            if (clothFrame > 9)
            {
                clothFrame = 6;
            }
        }
    }
}


