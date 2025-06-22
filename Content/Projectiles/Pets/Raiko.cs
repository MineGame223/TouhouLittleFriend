using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria;
using Terraria.DataStructures;
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
            get => Projectile.ai[2] == 1;
            set => Projectile.ai[2] = value ? 1 : 0;
        }
        private bool BandOn
        {
            get => Owner.GetModPlayer<ConcertPlayer>().ConcertStart && Owner.HasBuff<PoltergeistBuff>();
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

            ProjectileID.Sets.CharacterPreviewAnimations[Type] =
                ProjectileID.Sets.SimpleLoop(0, 1)
                .WhenSelected(14, 9, 4);
        }
        public override TouhouPetID UniqueID => TouhouPetID.Raiko;
        public override bool OnMouseHover(ref bool dontInvis)
        {
            dontInvis = IsBandState;
            return false;
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
        public override ChatSettingConfig ChatSettingConfig => new ChatSettingConfig() with
        {
            TextColor = new Color(249, 101, 101),
        };
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
        public override WeightedRandom<string> RegularDialogText()
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
            UpdateBackFrame();
            UpdateSkirtFrame();
            UpdateLegAndDrumFrame();
        }
        public override void AI()
        {
            Projectile.SetPetActive(Owner, BuffType<RaikoBuff>());

            ControlMovement(Owner);

            if (OwnerIsMyPlayer)
            {
                if (IsBandState && !BandOn)
                {
                    CurrentState = States.AfterPlaying;
                }
                if (BandOn && !IsBandState)
                {
                    Timer = 0;
                    CurrentState = States.BeforeBand;
                }
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
                    shouldNotTalking = true;
                    Playing(false);
                    break;

                case States.InBand:
                    shouldNotTalking = true;
                    Playing(true);
                    break;

                case States.Playing2:
                    shouldNotTalking = true;
                    Playing2(false);
                    break;

                case States.InBand2:
                    shouldNotTalking = true;
                    Playing2(true);
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
            if (CurrentState == States.Idle || CurrentState == States.Blink)
            {
                IdleAnimation();
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
        private void Playing(bool inBand)
        {
            if (inBand)
            {
                PlayingAnimation(2, 6);
            }
            else
            {
                PlayingAnimation();
            }
            if (Projectile.frame > 18)
            {
                Projectile.frame = 14;
                if (OwnerIsMyPlayer)
                {
                    CurrentState = inBand ? States.InBand2 : States.Playing2;
                }
            }
        }
        private void Playing2(bool inBand)
        {
            if (inBand)
            {
                PlayingAnimation(3, 4);
            }
            else
            {
                PlayingAnimation();
            }
            if (Projectile.frame > 23)
            {
                Projectile.frame = 14;
                if (!inBand)
                {
                    Timer++;
                }
                if (OwnerIsMyPlayer)
                {
                    if (Timer > RandomCount && !inBand)
                    {
                        Timer = 0;
                        CurrentState = States.AfterPlaying;
                    }
                    else
                    {
                        CurrentState = inBand ? States.InBand : States.Playing;
                        if (Main.rand.NextBool(3))
                        {
                            ShouldKick = true;
                            Projectile.netUpdate = true;
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
            if (++Projectile.frameCounter > 3)
            {
                Projectile.frameCounter = 0;
                Projectile.frame++;
            }
            if (Projectile.frame > 14)
            {
                Projectile.frame = 14;
            }
            if (OwnerIsMyPlayer)
            {
                Timer++;
                if (Timer > ConcertPlayer.BAND_COUNTDOWN_TIME)
                {
                    Timer = 0;
                    CurrentState = States.InBand;
                }
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
        private void PlayingAnimation(int frameInterval = 4, int frameInterval_2 = 20)
        {
            int count = frameInterval;
            if (Projectile.frame == 23)
            {
                count = frameInterval_2;
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


