using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria;
using Terraria.Utilities;
using TouhouPets.Content.Buffs.PetBuffs;

namespace TouhouPets.Content.Projectiles.Pets
{
    public class Reimu : BasicTouhouPet
    {
        private enum States
        {
            Idle,
            Blink,
            BeforeNap,
            Nap,
            WakeUp,
            WakeUp2,
            AfterWakeUp,
            Flying,
            FlyingBlink,
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
        private bool IsNapState => CurrentState >= States.BeforeNap && CurrentState <= States.AfterWakeUp;

        private DrawPetConfig drawConfig = new(1);
        private readonly Texture2D clothTex = AltVanillaFunction.GetExtraTexture("Reimu_Cloth");
        private readonly Texture2D newYearClothTex = AltVanillaFunction.GetExtraTexture("Reimu_Cloth_NewYear");

        private int blinkFrame, blinkFrameCounter;
        private int clothFrame, clothFrameCounter;
        private int flyTimeleft = 0;
        public override void SetStaticDefaults()
        {
            Main.projFrames[Type] = 18;
            Main.projPet[Type] = true;
        }
        public override bool PreDraw(ref Color lightColor)
        {
            bool isFebrary = DateTime.Now.Month == 2;
            Texture2D cloth = isFebrary ? newYearClothTex : clothTex;

            DrawPetConfig config = drawConfig with
            {
                ShouldUseEntitySpriteDraw = true,
                AltTexture = cloth,
            };

            Projectile.DrawPet(Projectile.frame, lightColor, drawConfig);

            if (CurrentState == States.Blink || CurrentState == States.FlyingBlink)
                Projectile.DrawPet(blinkFrame, lightColor, drawConfig);

            Projectile.DrawPet(Projectile.frame, lightColor, config);
            Projectile.DrawStateNormalizeForPet();

            if (CurrentState < States.Flying)
            {
                Projectile.DrawPet(clothFrame, lightColor, drawConfig);
                Projectile.DrawPet(clothFrame, lightColor, config);
            }
            return false;
        }
        public override Color ChatTextColor => new Color(255, 120, 120);
        public override void RegisterChat(ref string name, ref Vector2 indexRange)
        {
            name = "Reimu";
            indexRange = new Vector2(1, 11);
        }
        public override void SetRegularDialog(ref int timePerDialog, ref int chance, ref bool whenShouldStop)
        {
            timePerDialog = 840;
            chance = 6;
            whenShouldStop = !IsIdleState;
        }
        public override string GetRegularDialogText()
        {
            WeightedRandom<string> chat = new WeightedRandom<string>();
            {
                if (Owner.AnyBosses())
                {
                    chat.Add(ChatDictionary[3]);
                }
                else if (Main.bloodMoon || Main.eclipse || Main.slimeRain)
                {
                    chat.Add(ChatDictionary[4]);
                    chat.Add(ChatDictionary[10]);
                }
                else
                {
                    chat.Add(ChatDictionary[1]);
                    chat.Add(ChatDictionary[2]);
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
            Projectile.SetPetActive(Owner, BuffType<ReimuBuff>());

            UpdateTalking();

            ControlMovement();

            if (IsNapState && Owner.AnyBosses())
            {
                CurrentState = States.Idle;
            }
            if (Owner.velocity.Length() > 15f)
            {
                flyTimeleft = 5;
                if (OwnerIsMyPlayer && CurrentState < States.Flying)
                {
                    Timer = 0;
                    CurrentState = States.Flying;
                }
            }

            switch (CurrentState)
            {
                case States.Blink:
                    Blink();
                    break;

                case States.BeforeNap:
                    shouldNotTalking = true;
                    BeforeNap();
                    break;

                case States.Nap:
                    shouldNotTalking = true;
                    Nap();
                    break;

                case States.WakeUp:
                    shouldNotTalking = true;
                    WakeUp();
                    break;

                case States.WakeUp2:
                    shouldNotTalking = true;
                    WakeUp2();
                    break;

                case States.AfterWakeUp:
                    shouldNotTalking = true;
                    AfterWakeUp();
                    break;

                case States.Flying:
                    Flying();
                    break;

                case States.FlyingBlink:
                    Flying();
                    FlyingBlink();
                    break;

                default:
                    Idle();
                    break;
            }

            if (IsIdleState && ActionCD > 0)
            {
                ActionCD--;
            }

            if (flyTimeleft > 0)
            {
                flyTimeleft--;
            }
        }
        private void ControlMovement()
        {
            Projectile.tileCollide = false;
            Projectile.rotation = Projectile.velocity.X * 0.012f;

            if (CurrentState == States.Nap)
                return;

            ChangeDir();

            Vector2 point = new Vector2(-50 * Owner.direction, -30 + Owner.gfxOffY);
            MoveToPoint(point, 22f);
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
                if (mainTimer > 0 && mainTimer % 240 == 0 && currentChatRoom == null && ActionCD <= 0
                     && Owner.velocity.Length() == 0 && !Owner.AnyBosses())
                {
                    int chance = 11;
                    if (Main.bloodMoon || Main.eclipse)
                    {
                        chance = 30;
                    }
                    else if (Main.dayTime || Main.raining)
                    {
                        chance = 6;
                    }
                    else if (Owner.sleeping.FullyFallenAsleep)
                    {
                        chance = 2;
                    }
                    if (Main.rand.NextBool(chance))
                    {
                        RandomCount = Main.rand.Next(320, 560);
                        CurrentState = States.BeforeNap;

                        if (Main.rand.NextBool(8) && chatTimeLeft <= 0)
                            Projectile.SetChat(ChatSettingConfig, 11);
                    }
                }
            }
        }
        private void Blink()
        {
            Projectile.frame = 0;
            if (blinkFrame < 13)
            {
                blinkFrame = 13;
            }
            if (++blinkFrameCounter > 3)
            {
                blinkFrameCounter = 0;
                blinkFrame++;
            }
            if (blinkFrame > 15)
            {
                blinkFrame = 13;
                CurrentState = States.Idle;
            }
        }
        private void BeforeNap()
        {
            Projectile.velocity *= 0.5f;
            Projectile.frame = 1;

            Timer++;
            if (OwnerIsMyPlayer && Timer > RandomCount)
            {
                Timer = 0;
                CurrentState = States.Nap;
            }
        }
        private void Nap()
        {
            if (++Projectile.frameCounter > 6)
            {
                Projectile.frameCounter = 0;
                Projectile.frame++;
            }
            Projectile.velocity *= 0.1f;
            if (Projectile.frame >= 3)
            {
                Projectile.frame = 3;
            }
            if (OwnerIsMyPlayer)
            {
                if(Projectile.Distance(Owner.position) > 300f)
                {
                    RandomCount = Main.rand.Next(2, 4);
                    CurrentState = States.WakeUp;
                    return;
                }
                if (mainTimer % 320 == 0 && Main.rand.NextBool(3) && !Owner.sleeping.FullyFallenAsleep)
                {
                    if (Main.rand.NextBool(2))
                    {
                        RandomCount = Main.rand.Next(2, 4);
                        CurrentState = States.WakeUp;
                    }
                    else
                    {
                        RandomCount = Main.rand.Next(320, 560);
                        CurrentState = States.BeforeNap;
                    }
                }
            }
        }
        private void WakeUp()
        {
            if (++Projectile.frameCounter > 6)
            {
                Projectile.frameCounter = 0;
                Projectile.frame++;
            }
            Projectile.velocity *= 0.5f;
            if (Projectile.frame == 5)
            {
                Projectile.frame = 6;
            }
            if (Projectile.frame > 6)
            {
                Projectile.frame = 4;
                Timer++;
            }
            if (OwnerIsMyPlayer && Timer > RandomCount)
            {
                Timer = 0;
                RandomCount = Main.rand.Next(40, 60);
                CurrentState = States.WakeUp2;
            }
        }
        private void WakeUp2()
        {
            Projectile.frame = 5;
            Projectile.velocity *= 0.75f;

            Timer++;
            if (OwnerIsMyPlayer && Timer > RandomCount)
            {
                Timer = 0;
                RandomCount = Main.rand.Next(6, 9);
                CurrentState = States.AfterWakeUp;
            }
        }
        private void AfterWakeUp()
        {
            if (++Projectile.frameCounter > 2)
            {
                Projectile.frameCounter = 0;
                Projectile.frame++;
            }
            if (Projectile.frame > 8)
            {
                Projectile.frame = 7;
                Timer++;
            }
            if (OwnerIsMyPlayer && Timer > RandomCount)
            {
                Timer = 0;
                ActionCD = 600;
                CurrentState = States.Idle;
            }
        }
        private void Flying()
        {
            if (OwnerIsMyPlayer && flyTimeleft <= 0)
            {
                ActionCD = 600;
                CurrentState = States.Idle;
                return;
            }
            if (Projectile.frame < 16)
            {
                Projectile.frame = 16;
            }
            if (++Projectile.frameCounter > 3)
            {
                Projectile.frameCounter = 0;
                Projectile.frame++;
            }
            if (Projectile.frame > 17)
            {
                Projectile.frame = 16;
            }
        }
        private void FlyingBlink()
        {
            if (blinkFrame < 13)
            {
                blinkFrame = 13;
            }
            if (++blinkFrameCounter > 3)
            {
                blinkFrameCounter = 0;
                blinkFrame++;
            }
            if (blinkFrame > 15)
            {
                blinkFrame = 13;
                CurrentState = States.Flying;
            }
        }
        private void UpdateClothFrame()
        {
            if (clothFrame < 9)
            {
                clothFrame = 9;
            }
            int count = 5;
            if (++clothFrameCounter > count)
            {
                clothFrameCounter = 0;
                clothFrame++;
            }
            if (clothFrame > 12)
            {
                clothFrame = 9;
            }
        }
    }
}


