using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ID;
using Terraria.Utilities;
using TouhouPets.Content.Buffs.PetBuffs;

namespace TouhouPets.Content.Projectiles.Pets
{
    public class Merlin : BasicTouhouPet
    {
        private enum States
        {
            Idle,
            Blink,
            Playing,
            AfterPlaying,
            BeforeBand,
            InBand,
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
        private bool BandOn
        {
            get => Owner.GetModPlayer<ConcertPlayer>().ConcertStart && Owner.HasBuff<PoltergeistBuff>();
        }
        private bool IsIdleState => CurrentState <= States.Blink;
        private bool IsBandState => CurrentState >= States.BeforeBand && CurrentState <= States.InBand;

        private int blinkFrame, blinkFrameCounter;
        private int clothFrame, clothFrameCounter;
        private int hairFrame, hairFrameCounter;

        private DrawPetConfig drawConfig = new(2);
        private readonly Texture2D clothTex = AltVanillaFunction.GetExtraTexture("Merlin_Cloth");
        public override void SetStaticDefaults()
        {
            Main.projFrames[Type] = 14;
            Main.projPet[Type] = true;
            ProjectileID.Sets.LightPet[Type] = true;
        }
        public override TouhouPetID UniqueID => TouhouPetID.Merlin;
        public override bool OnMouseHover(ref bool dontInvis)
        {
            dontInvis = IsBandState;
            return false;
        }
        public override bool DrawPetSelf(ref Color lightColor)
        {
            for (int i = 0; i < 4; i++)
            {
                Color clr = Color.AliceBlue * mouseOpacity;
                clr.A *= 0;
                DrawMerlin(clr * 0.8f, new Vector2(2, 0).RotatedBy(MathHelper.ToRadians(90 * i)));
            }
            DrawMerlin(lightColor, Vector2.Zero);
            return false;
        }
        private void DrawMerlin(Color lightColor, Vector2 extraPos)
        {
            DrawPetConfig config = drawConfig with
            {
                PositionOffset = extraPos,
            };
            DrawPetConfig config2 = config with
            {
                ShouldUseEntitySpriteDraw = true,
            };

            Projectile.DrawPet(hairFrame, lightColor, config, 1);

            Projectile.DrawPet(Projectile.frame, lightColor, config);

            if (CurrentState == States.Blink)
                Projectile.DrawPet(blinkFrame, lightColor, config, 1);

            Projectile.DrawPet(Projectile.frame, lightColor,
                drawConfig with
                {
                    AltTexture = clothTex,
                    ShouldUseEntitySpriteDraw = true,
                });
            Projectile.DrawPet(clothFrame, lightColor, config2, 1);
            Projectile.DrawPet(3, lightColor, config2, 1);
            Projectile.ResetDrawStateForPet();

            Projectile.DrawPet(Projectile.frame, lightColor, config, 2);
        }
        public override ChatSettingConfig ChatSettingConfig => new ChatSettingConfig() with
        {
            TextColor = new Color(144, 206, 237),
        };
        public override void RegisterChat(ref string name, ref Vector2 indexRange)
        {
            name = "Merlin";
            indexRange = new Vector2(1, 5);
        }
        public override void SetRegularDialog(ref int timePerDialog, ref int chance, ref bool whenShouldStop)
        {
            timePerDialog = 640;
            chance = 5;
            whenShouldStop = !IsIdleState;
        }
        public override WeightedRandom<string> RegularDialogText()
        {
            WeightedRandom<string> chat = new WeightedRandom<string>();
            {
                chat.Add(ChatDictionary[1]);
                chat.Add(ChatDictionary[2]);
                chat.Add(ChatDictionary[3]);
                chat.Add(ChatDictionary[4]);
                if (Main.eclipse)
                {
                    chat.Add(ChatDictionary[5], 3);
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
            Lighting.AddLight(Projectile.Center, 1.0f, 1.0f, 1.0f);
            rgb = new Vector3(0.88f, 1.87f, 2.37f);
        }
        public override void AI()
        {
            Projectile.SetPetActive(Owner, BuffType<MerlinBuff>());
            Projectile.SetPetActive(Owner, BuffType<PoltergeistBuff>());

            UpdateTalking();

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

                case States.Playing:
                    shouldNotTalking = true;
                    Playing();
                    break;

                case States.InBand:
                    shouldNotTalking = true;
                    InBandPlaying();
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
            Projectile.rotation = Projectile.velocity.X * 0.02f;

            ChangeDir(200);

            Vector2 point = new Vector2(50 * player.direction, -30 + player.gfxOffY);
            if (player.HasBuff<PoltergeistBuff>())
            {
                if (IsBandState)
                {
                    point = new Vector2(160 * player.direction, -90 + player.gfxOffY);
                }
                else
                {
                    point = new Vector2(60 * player.direction, -70 + player.gfxOffY);
                    point += new Vector2(0, -40).RotatedBy(MathHelper.ToRadians(360 / 3 * 2) + Main.GlobalTimeWrappedHourly);
                }
            }
            MoveToPoint(point, 6.5f);
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
                if (mainTimer > 0 && mainTimer % 720 == 0
                    && currentChatRoom == null && ActionCD <= 0)
                {
                    if (Main.rand.NextBool(4))
                    {
                        RandomCount = Main.rand.Next(30, 60);
                        CurrentState = States.Playing;
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
            if (blinkFrame > 2)
            {
                blinkFrame = 0;
                CurrentState = States.Idle;
            }
        }
        private void PlayingAnimation(bool shouldCount)
        {
            if (++Projectile.frameCounter > 9)
            {
                Projectile.frameCounter = 0;
                Projectile.frame++;
            }
            if (Projectile.frame > 11)
            {
                Projectile.frame = 4;
                if (shouldCount)
                {
                    Timer++;
                }
            }
        }
        private void InBandPlaying()
        {
            PlayingAnimation(false);
        }
        private void Playing()
        {
            PlayingAnimation(true);
            if (Timer > 0 && mainTimer % 6 == 0 && Main.rand.NextBool(5))
            {
                Gore.NewGoreDirect(Projectile.GetSource_FromAI()
                    , Projectile.Center + new Vector2(24 * Projectile.spriteDirection, -14)
                    , new Vector2(Main.rand.Next(-5, 5), Main.rand.Next(-6, -3)) * 0.1f, Main.rand.Next(570, 573)
                    , Main.rand.NextFloat(0.9f, 1.1f));
            }

            if (OwnerIsMyPlayer && Timer > RandomCount)
            {
                Timer = 0;
                CurrentState = States.AfterPlaying;
            }
        }
        private void AfterPlaying()
        {
            if (++Projectile.frameCounter > 6)
            {
                Projectile.frameCounter = 0;
                Projectile.frame++;
            }
            if (Projectile.frame > 13)
            {
                Projectile.frame = 0;
                if (OwnerIsMyPlayer)
                {
                    ActionCD = 10800;
                    CurrentState = States.Idle;
                }
            }
        }
        private void BeforeBand()
        {
            if (++Projectile.frameCounter > 6)
            {
                Projectile.frameCounter = 0;
                Projectile.frame++;
            }
            if (Projectile.frame > 4)
            {
                Projectile.frame = 4;
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
        private void UpdateMiscFrame()
        {
            if (hairFrame < 8)
            {
                hairFrame = 8;
            }
            if (++hairFrameCounter > 7)
            {
                hairFrameCounter = 0;
                hairFrame++;
            }
            if (hairFrame > 11)
            {
                hairFrame = 8;
            }

            if (clothFrame < 4)
            {
                clothFrame = 4;
            }
            if (++clothFrameCounter > 8)
            {
                clothFrameCounter = 0;
                clothFrame++;
            }
            if (clothFrame > 7)
            {
                clothFrame = 4;
            }
        }
    }
}


