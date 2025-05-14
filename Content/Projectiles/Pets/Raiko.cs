using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ID;
using Terraria.Utilities;
using TouhouPets.Content.Buffs.PetBuffs;

namespace TouhouPets.Content.Projectiles.Pets
{
    public class Raiko : BasicTouhouPet
    {
        private enum States
        {
            Idle,
            Blink,
            Idle2,
            Playing,
            Playing2,
            AfterPlaying,
            BeforeBand,
            InBand,
            InBand2,
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
        private bool ShouldKick
        {
            get => Projectile.ai[2] == 0;
            set => Projectile.ai[2] = value ? 0 : 1;
        }
        private bool BandOn
        {
            get => Owner.GetModPlayer<ConcertPlayer>().prismriverBand && Owner.HasBuff<PoltergeistBuff>();
        }
        private bool IsIdleState => CurrentState <= States.Idle2;
        private bool IsBandState => CurrentState >= States.BeforeBand && CurrentState <= States.InBand2;

        private int blinkFrame, blinkFrameCounter;
        private int backFrame, backFrameCounter;
        private int legFrame, drumFrame, legFrameCounter;
        private int skritFrame, skritFrameCounter;

        private DrawPetConfig drawConfig = new(2);
        private readonly Texture2D clothTex = AltVanillaFunction.GetExtraTexture("Raiko_Cloth");
        public override void SetStaticDefaults()
        {
            Main.projFrames[Type] = 25;
            Main.projPet[Type] = true;
        }
        public override bool DrawPetSelf(ref Color lightColor)
        {
            DrawPetConfig config = drawConfig with
            {
                ShouldUseEntitySpriteDraw = true,
            };
            DrawPetConfig config2 = config with
            {
                AltTexture = clothTex,
            };

            Projectile.DrawPet(backFrame, lightColor, config, 1);
            Projectile.DrawPet(drumFrame, lightColor, config, 1);
            Projectile.ResetDrawStateForPet();

            Projectile.DrawPet(legFrame, lightColor, drawConfig, 1);
            Projectile.DrawPet(legFrame, lightColor, config2, 1);
            Projectile.ResetDrawStateForPet();

            Projectile.DrawPet(Projectile.frame, lightColor, drawConfig);

            if (CurrentState == States.Blink)
                Projectile.DrawPet(blinkFrame, lightColor, drawConfig, 1);

            Projectile.DrawPet(Projectile.frame, lightColor, config2);
            Projectile.DrawPet(skritFrame, lightColor, config, 1);
            return false;
        }
        public override Color ChatTextColor => new Color(249, 101, 101);
        public override void RegisterChat(ref string name, ref Vector2 indexRange)
        {
            name = "Raiko";
            indexRange = new Vector2(1, 3);
        }
        public override void SetRegularDialog(ref int timePerDialog, ref int chance, ref bool whenShouldStop)
        {
            timePerDialog = 555;
            chance = 9;
            whenShouldStop = !IsIdleState;
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
        private void UpdateTalking()
        {
        }
        public override void VisualEffectForPreview()
        {
            UpdateBackFrame();
            UpdateSkirtFrame();
            UpdateLegAndDrumFrame();
            if (CurrentState == States.Idle || CurrentState == States.Blink)
            {
                IdleAnimation();
            }
        }
        public override void AI()
        {
            Projectile.SetPetActive(Owner, BuffType<RaikoBuff>());

            UpdateTalking();

            ControlMovement(Owner);

            if (IsBandState && !BandOn)
            {
                Timer = 0;
                CurrentState = States.AfterPlaying;
            }
            if (BandOn && !IsBandState)
            {
                Timer = ConcertPlayer.BAND_COUNTDOWN_TIME;
                RandomCount = 2;
                CurrentState = States.BeforeBand;
            }

            switch (CurrentState)
            {
                case States.Blink:
                    Blink();
                    break;

                case States.Idle2:
                    Idle2();
                    break;

                case States.Playing:
                case States.InBand:
                    shouldNotTalking = true;
                    Playing();
                    break;

                case States.Playing2:
                case States.InBand2:
                    shouldNotTalking = true;
                    Playing2();
                    break;

                case States.AfterPlaying:
                    shouldNotTalking = true;
                    AfterPlaying();
                    break;

                case States.BeforeBand:
                    shouldNotTalking = true;
                    BeforeBand();
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
        private void ControlMovement(Player player)
        {
            Projectile.tileCollide = false;
            Projectile.rotation = Projectile.velocity.X * 0.005f;

            ChangeDir(200);

            Vector2 point = new Vector2(-60 * player.direction, -40 + player.gfxOffY);
            if (IsBandState)
            {
                point = new Vector2(-50 * player.direction, -120 + player.gfxOffY);
            }
            MoveToPoint(point, 12.5f);
        }
        private void Idle()
        {
            if (OwnerIsMyPlayer)
            {
                if (mainTimer % 270 == 0)
                {
                    CurrentState = States.Blink;
                }
                if (mainTimer > 0 && mainTimer % 720 == 0 && currentChatRoom == null && ActionCD <= 0)
                {
                    if (Main.rand.NextBool(3))
                    {
                        RandomCount = Main.rand.Next(15, 30);
                        CurrentState = States.Playing;
                    }
                }
            }
        }
        private void Idle2()
        {
            int count = 5;
            if (++Projectile.frameCounter > count)
            {
                Projectile.frameCounter = 0;
                Projectile.frame++;
            }
            if (Projectile.frame > 10)
            {
                Projectile.frame = 0;
                CurrentState = States.Idle;
            }
        }
        private void Blink()
        {
            if (++blinkFrameCounter > 3)
            {
                blinkFrameCounter = 0;
                blinkFrame++;
            }
            if (blinkFrame > 3)
            {
                blinkFrame = 0;
                if (OwnerIsMyPlayer && Main.rand.NextBool(3))
                {
                    CurrentState = States.Idle2;
                }
                else
                {
                    CurrentState = States.Idle;
                }
            }
        }
        private void Playing()
        {
            PlayingAnimation();
            if (Projectile.frame > 18)
            {
                Projectile.frame = 14;
                if (OwnerIsMyPlayer)
                {
                    CurrentState = IsBandState ? States.InBand2 : States.Playing2;
                }
            }
        }
        private void Playing2()
        {
            PlayingAnimation();
            if (Projectile.frame > 23)
            {
                Projectile.frame = 14;
                if (!IsBandState)
                {
                    Timer++;
                }
                if (OwnerIsMyPlayer)
                {
                    if (Timer > RandomCount)
                    {
                        Timer = 0;
                        CurrentState = States.AfterPlaying;
                    }
                    else
                    {
                        CurrentState = IsBandState ? States.InBand : States.Playing;
                        if (Main.rand.NextBool(3))
                        {
                            ShouldKick = true;
                        }
                    }
                }
            }
        }
        private void AfterPlaying()
        {
            PlayingAnimation();
            if (Projectile.frame > 24)
            {
                Projectile.frame = 0;
                if (OwnerIsMyPlayer)
                {
                    ActionCD = 1200;
                    CurrentState = States.Idle;
                }
            }
        }
        private void BeforeBand()
        {
            if (Timer >= ConcertPlayer.BAND_COUNTDOWN_TIME)
            {
                Projectile.frame = 0;
            }
            if (++Projectile.frameCounter > 3)
            {
                Projectile.frameCounter = 0;
                Projectile.frame++;
            }
            if (Projectile.frame > 14)
            {
                Projectile.frame = 14;
            }
            Timer--;

            if (OwnerIsMyPlayer && Timer <= 0)
            {
                Timer = 1;
                CurrentState = States.InBand;
            }
        }
        private void IdleAnimation()
        {
            int count = 7;
            if (Projectile.frame == 0)
            {
                count = 120;
            }
            if (++Projectile.frameCounter > count)
            {
                Projectile.frameCounter = 0;
                Projectile.frame++;
            }
            if (Projectile.frame > 3)
            {
                Projectile.frame = 0;
            }
        }
        private void PlayingAnimation()
        {
            int count = 4;
            if (Projectile.frame == 23)
            {
                count = 20;
            }
            if (++Projectile.frameCounter > count)
            {
                Projectile.frameCounter = 0;
                Projectile.frame++;
            }
            if (Projectile.frameCounter == 1 && !IsBandState)
            {
                if (Projectile.frame == 16)
                    AltVanillaFunction.PlaySound(SoundID.DrumFloorTom, Projectile.Center);
                else if (Projectile.frame == 20)
                    AltVanillaFunction.PlaySound(SoundID.DrumHiHat, Projectile.Center);
            }
        }
        private void UpdateBackFrame()
        {
            if (backFrame < 16)
            {
                backFrame = 16;
            }
            if (++backFrameCounter > 40)
            {
                backFrameCounter = 0;
                backFrame++;
            }
            if (backFrame > 19)
            {
                backFrame = 16;
            }
        }
        private void UpdateSkirtFrame()
        {
            if (skritFrame < 3)
            {
                skritFrame = 3;
            }
            if (++skritFrameCounter > 5)
            {
                skritFrameCounter = 0;
                skritFrame++;
            }
            if (skritFrame > 6)
            {
                skritFrame = 3;
            }
        }
        private void UpdateLegAndDrumFrame()
        {
            if (legFrame < 7)
            {
                legFrame = 7;
            }
            if (!ShouldKick)
            {
                legFrame = 7;
            }
            else
            {
                if (++legFrameCounter > 5)
                {
                    legFrameCounter = 0;
                    legFrame++;
                }
                if (legFrame == 10 && legFrameCounter == 1)
                {
                    //AltVanillaFunction.PlaySound(SoundID.DrumKick, Projectile.Center);
                }
                if (legFrame > 11)
                {
                    legFrame = 7;
                    ShouldKick = false;
                }
            }

            drumFrame = legFrame + 3;
            if (drumFrame < 12)
            {
                drumFrame = 12;
            }
            if (drumFrame > 14)
            {
                drumFrame = 12;
            }
        }
    }
}


