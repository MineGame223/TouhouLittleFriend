using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ID;
using Terraria.Localization;
using Terraria.Utilities;
using TouhouPets.Content.Buffs.PetBuffs;

namespace TouhouPets.Content.Projectiles.Pets
{
    public class Kogasa : BasicTouhouPet
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

        private DrawPetConfig drawConfig = new(3);
        private readonly Texture2D clothTex = AltVanillaFunction.GetExtraTexture("Kogasa_Cloth");
        public override void SetStaticDefaults()
        {
            Main.projFrames[Type] = 15;
            Main.projPet[Type] = true;

            ProjectileID.Sets.CharacterPreviewAnimations[Type] =
                ProjectileID.Sets.SimpleLoop(0, 1)
                .WhenSelected(1, 1);
        }
        public override TouhouPetID UniqueID => TouhouPetID.Kogasa;
        public override bool DrawPetSelf(ref Color lightColor)
        {
            DrawPetConfig config = drawConfig with
            {
                AltTexture = clothTex,
                ShouldUseEntitySpriteDraw = true,
            };
            int eyeFramePlus = Projectile.spriteDirection == -1 ? 0 : 3;

            Projectile.DrawPet(umbrellaFrame, lightColor, drawConfig, 2);
            Projectile.DrawPet(umbrellaFrame, lightColor, config, 2);
            Projectile.ResetDrawStateForPet();

            Projectile.DrawPet(Projectile.frame, lightColor, drawConfig);

            if (IsIdleState || IsUmberllaState)
                Projectile.DrawPet(blinkFrame + eyeFramePlus, lightColor, drawConfig, 1);

            if (CurrentState == States.MakeFace || CurrentState == States.AfterMakeFace)
            {
                //用于扮鬼脸时闭眼/睁眼的时候
                if (Projectile.frame == 2 || Projectile.frame == 5)
                    Projectile.DrawPet(9 + eyeFramePlus, lightColor, drawConfig, 1);
            }

            Projectile.DrawPet(Projectile.frame, lightColor, config);
            Projectile.DrawPet(clothFrame, lightColor,
                drawConfig with
                {
                    ShouldUseEntitySpriteDraw = true,
                }, 1);
            Projectile.ResetDrawStateForPet();

            Projectile.DrawPet(handFrame, lightColor, drawConfig, 1);
            Projectile.DrawPet(handFrame, lightColor, config, 1);
            return false;
        }
        public override ChatSettingConfig ChatSettingConfig => new ChatSettingConfig() with
        {
            TextColor = new Color(172, 69, 191),
        };
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
        public override WeightedRandom<LocalizedText> RegularDialogText()
        {
            WeightedRandom<LocalizedText> chat = new ();
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
        public override void VisualEffectForPreview()
        {
            UpdateClothFrame();
            UpdateUmbrellaFrame();
            UpdateHandFrame();
        }
        public override void AI()
        {
            if (blinkFrame < 8)
            {
                blinkFrame = 8;
            }

            Projectile.SetPetActive(Owner, BuffType<KogasaBuff>());

            ControlMovement();

            if (FindBoss)
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
                        if (Main.rand.NextBool(3) && Projectile.CurrentlyNoDialog())
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
            if (++blinkFrameCounter > 3)
            {
                blinkFrameCounter = 0;
                blinkFrame++;
            }
            if (blinkFrame > 10)
            {
                blinkFrame = 8;
                CurrentState = States.Idle;
            }
        }
        private void Umbrella()
        {
            Projectile.frame = 1;
            if (OwnerIsMyPlayer && mainTimer % 270 == 0)
            {
                CurrentState = States.UmbrellaBlink;
            }
        }
        private void UmbrellaBlink()
        {
            Projectile.frame = 1;
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
            if (Projectile.frame < 2)
            {
                Projectile.frame = 2;
            }
            if (++Projectile.frameCounter > 7)
            {
                Projectile.frameCounter = 0;
                Projectile.frame++;
            }
            if (Projectile.frame > 4)
            {
                Projectile.frame = 3;
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
            if (Projectile.frame > 5)
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
            Projectile.frame = 6;
            umbrellaFrame = 14;
            handFrame = 7;
            if (OwnerIsMyPlayer && !FindBoss)
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
            if (Projectile.frame != 1)
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
            if (Projectile.frame == 1)
            {
                if (handFrame < 5)
                {
                    handFrame = 5;
                }
                if (++handFrameCounter > 5)
                {
                    handFrameCounter = 0;
                    handFrame++;
                }
                if (handFrame > 6)
                {
                    handFrame = 5;
                }
            }
            else
            {
                handFrame = 4;
            }
        }
        private void UpdateClothFrame()
        {
            if (++clothFrameCounter > 5)
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


