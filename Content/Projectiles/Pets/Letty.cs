using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ID;
using Terraria.Localization;
using Terraria.Utilities;
using TouhouPets.Content.Buffs.PetBuffs;

namespace TouhouPets.Content.Projectiles.Pets
{
    public class Letty : BasicTouhouPet
    {
        private enum States
        {
            Idle,
            Blink,
            Snowing,
            AfterSnowing,
            Hot,
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
        private bool InHotZone => (Owner.ZoneDesert && Main.dayTime)
            || Owner.ZoneUnderworldHeight || Owner.ZoneJungle;
        private bool FeelHot => Owner.ZoneUnderworldHeight || Owner.HasBuff<UtsuhoBuff>();

        private int backFrame, backFrameCounter;
        private int blinkFrame, blinkFrameCounter;
        private int clothFrame, clothFrameCounter;
        private int extraAdjY;

        private DrawPetConfig drawConfig = new(2);
        private readonly Texture2D clothTex = AltVanillaFunction.GetExtraTexture("Letty_Cloth");
        public override void PetStaticDefaults()
        {
            Main.projFrames[Type] = 8;
            Main.projPet[Type] = true;

            ProjectileID.Sets.CharacterPreviewAnimations[Type] =
                ProjectileID.Sets.SimpleLoop(0, 1)
                .WhenSelected(2, 0);
        }
        public override TouhouPetID UniqueID => TouhouPetID.Letty;
        public override bool DrawPetSelf(ref Color lightColor)
        {
            Vector2 extraPos = new Vector2(0, extraAdjY);
            DrawPetConfig config = drawConfig with
            {
                ShouldUseEntitySpriteDraw = true,
                AltTexture = clothTex,
            };
            DrawPetConfig config2 = drawConfig with
            {
                ShouldUseEntitySpriteDraw = true,
                PositionOffset = extraPos,
            };

            Projectile.DrawPet(backFrame, lightColor * 0.5f, config2, 1);
            Projectile.ResetDrawStateForPet();

            Projectile.DrawPet(Projectile.frame, lightColor, drawConfig);

            if (CurrentState == States.Blink)
                Projectile.DrawPet(blinkFrame, lightColor, drawConfig);

            Projectile.DrawPet(Projectile.frame, lightColor, config);
            Projectile.DrawPet(clothFrame, lightColor, config2, 1);
            return false;
        }
        public override ChatSettingConfig ChatSettingConfig => new ChatSettingConfig() with
        {
            TextColor = new Color(236, 223, 255),
        };
        public override void RegisterChat(ref string name, ref Vector2 indexRange)
        {
            name = "Letty";
            indexRange = new Vector2(1, 9);
        }
        public override void SetRegularDialog(ref int timePerDialog, ref int chance, ref bool whenShouldStop)
        {
            timePerDialog = 870;
            chance = 8;
            whenShouldStop = !IsIdleState;
        }
        public override WeightedRandom<LocalizedText> RegularDialogText()
        {
            WeightedRandom<LocalizedText> chat = new ();
            {
                if (CurrentState == States.Hot)
                {
                    chat.Add(ChatDictionary[8]);
                    if (Owner.HasBuff<UtsuhoBuff>())
                    {
                        chat.Add(ChatDictionary[9]);
                    }
                }
                else
                {
                    for (int j = 1; j <= 5; j++)
                    {
                        chat.Add(ChatDictionary[j]);
                    }
                    if (Owner.ZoneSnow)
                    {
                        chat.Add(ChatDictionary[6]);
                        chat.Add(ChatDictionary[7]);
                    }
                }
            }
            return chat;
        }
        public override void VisualEffectForPreview()
        {
            UpdateMiscFrame();
            UpdateMiscData();
        }
        public override void AI()
        {
            Projectile.SetPetActive(Owner, BuffType<LettyBuff>());

            ControlMovement();

            if (ShouldExtraVFXActive)
                GenDust();

            if (FeelHot)
            {
                CurrentState = States.Hot;
            }
            else if (!InHotZone && GetInstance<PetAbilitiesConfig>().SpecialAbility_Letty)
            {
                Owner.GetModPlayer<TouhouPetPlayer>().lettyCold = true;
            }

            switch (CurrentState)
            {
                case States.Blink:
                    Blink();
                    break;

                case States.Snowing:
                    shouldNotTalking = true;
                    Snowing();
                    break;

                case States.AfterSnowing:
                    shouldNotTalking = true;
                    AfterSnowing();
                    break;

                case States.Hot:
                    shouldNotTalking = true;
                    Hot();
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
        private void GenDust()
        {
            if (CurrentState >= States.Hot || InHotZone)
                return;

            int dustID = MyDustId.Snow;
            if (Main.rand.NextBool(25))
            {
                Dust.NewDustPerfect(Projectile.Center
                + new Vector2(Main.rand.NextFloat(-30f, 30f), Main.rand.NextFloat(-5f, 25f)), dustID
                , new Vector2(Main.rand.NextFloat(-0.2f, 0.2f), Main.rand.NextFloat(0.8f, 1.2f)), Main.rand.Next(0, 100), default
                , Main.rand.NextFloat(1f, 1.2f));
            }
        }
        private void ControlMovement()
        {
            Projectile.tileCollide = false;
            if (CurrentState != States.Snowing)
                Projectile.rotation = Projectile.velocity.X * 0.035f;
            else
                Projectile.rotation = Projectile.velocity.X * 0.005f;

            ChangeDir();

            Vector2 point = new Vector2(-60 * Owner.direction, -30 + Owner.gfxOffY);
            MoveToPoint(point, 10f);
        }
        private void UpdateMiscData()
        {
            extraAdjY = 0;
            if (Projectile.frame >= 1 && Projectile.frame <= 3)
            {
                extraAdjY = -2;
            }
            if (Projectile.frame >= 4)
            {
                extraAdjY = 2;
            }
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
                if (mainTimer > 0 && mainTimer % 700 == 0 && currentChatRoom == null && ActionCD <= 0)
                {
                    if (Owner.ZoneOverworldHeight && Main.rand.NextBool(5) && !InHotZone)
                    {
                        RandomCount = Main.rand.Next(600, 1200);
                        CurrentState = States.Snowing;
                    }
                }
            }
        }
        private void Blink()
        {
            Projectile.frame = 0;
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
        private void Snowing()
        {
            if (++Projectile.frameCounter > 7)
            {
                Projectile.frameCounter = 0;
                Projectile.frame++;
            }
            if (Projectile.frame > 2)
                Projectile.frame = 2;

            if (ShouldExtraVFXActive)
                SnowingEffect();

            Timer++;
            if (OwnerIsMyPlayer)
            {
                if (Timer > RandomCount || InHotZone)
                {
                    Timer = 0;
                    CurrentState = States.AfterSnowing;
                }
            }
        }
        private void SnowingEffect()
        {
            int dustID = MyDustId.IceTorch;
            if (Main.rand.NextBool(2))
            {
                Dust.NewDustPerfect(Projectile.Center
                + new Vector2(Main.rand.NextFloat(-30f, 30f), Main.rand.NextFloat(-25f, 25f)), dustID
                , new Vector2(Main.rand.NextFloat(-0.2f, 0.2f), -Main.rand.NextFloat(0.8f, 1.2f)), Main.rand.Next(0, 100), default
                , Main.rand.NextFloat(1f, 2f)).noGravity = true;
            }

            dustID = MyDustId.Snow;
            if (Main.rand.NextBool(2))
            {
                for (int i = 0; i < 3; i++)
                {
                    Gore.NewGorePerfect(Projectile.GetSource_FromAI(), Projectile.Center
               + new Vector2(Main.rand.NextFloat(-200f, 200f), Main.rand.NextFloat(-140f, -110f)),
               new Vector2(Main.rand.NextFloat(-0.5f, 0.5f), 0)
               , GoreID.AmbientAirborneCloud2, Main.rand.NextFloat(1f, 2f));
                }

                Dust.NewDustPerfect(Projectile.Center
                + new Vector2(Main.rand.NextFloat(-200f, 200f), Main.rand.NextFloat(-120f, -110f)), dustID
                , new Vector2(Main.rand.NextFloat(-0.2f, 0.2f) + Main.windSpeedCurrent, Main.rand.NextFloat(0.8f, 1.2f)), Main.rand.Next(0, 100), default
                , Main.rand.NextFloat(1f, 1.2f));
            }
        }
        private void AfterSnowing()
        {
            if (++Projectile.frameCounter > 7)
            {
                Projectile.frameCounter = 0;
                Projectile.frame++;
            }
            if (Projectile.frame > 3)
            {
                Projectile.frame = 0;
                if (OwnerIsMyPlayer)
                {
                    ActionCD = 4800;
                    CurrentState = States.Idle;
                }
            }
        }
        private void Hot()
        {
            if (mainTimer % Main.rand.Next(20, 50) == 0 && Main.rand.NextBool(2))
            {
                Gore.NewGorePerfect(Projectile.GetSource_FromAI(), Projectile.Center
                + new Vector2(Main.rand.NextFloat(-30f, 20f), Main.rand.NextFloat(-5f, 10f)),
                new Vector2(Main.rand.NextFloat(-0.2f, 0.2f), -Main.rand.NextFloat(1f, 2f))
                , 11, Main.rand.NextFloat(0.5f, 1f));
            }

            Projectile.frame = 4;
            if (OwnerIsMyPlayer && !FeelHot)
            {
                CurrentState = States.Idle;
            }
        }
        private void UpdateMiscFrame()
        {
            int count = 12;
            if (backFrame < 4)
            {
                backFrame = 4;
            }
            if (++backFrameCounter > count)
            {
                backFrameCounter = 0;
                backFrame++;
            }
            if (backFrame > 7)
            {
                backFrame = 4;
            }

            count = 7;
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


