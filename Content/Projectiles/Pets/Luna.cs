using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.Enums;
using Terraria.Graphics.Shaders;
using Terraria.ID;
using Terraria.Utilities;
using TouhouPets.Content.Buffs.PetBuffs;

namespace TouhouPets.Content.Projectiles.Pets
{
    public class Luna : BasicTouhouPet
    {
        private enum States
        {
            Idle,
            Blink,
            ReadingPaper,
            ReadingPaperBlink,
            AfterReadingPaper,
            Yawn1,
            Yawn2,
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
        private static bool IsFullMoon => Main.GetMoonPhase() == MoonPhase.Full
            && !Main.dayTime && !Main.bloodMoon;

        private int blinkFrame, blinkFrameCounter;
        private int wingsFrame, wingsFrameCounter;
        private int clothFrame, clothFrameCounter;

        private DrawPetConfig drawConfig = new(2);
        private readonly Texture2D clothTex = AltVanillaFunction.GetExtraTexture("Luna_Cloth");
        public override void SetStaticDefaults()
        {
            Main.projFrames[Type] = 13;
            Main.projPet[Type] = true;
            ProjectileID.Sets.LightPet[Type] = true;
        }
        public override TouhouPetID UniqueID => TouhouPetID.Luna;
        public override bool DrawPetSelf(ref Color lightColor)
        {
            DrawPetConfig config = drawConfig with
            {
                ShouldUseEntitySpriteDraw = true,
                AltTexture = clothTex,
            };

            Projectile.DrawPet(wingsFrame, lightColor * 0.8f, drawConfig);

            Projectile.DrawPet(12, lightColor, drawConfig);
            Projectile.DrawPet(12, lightColor, config);
            Projectile.ResetDrawStateForPet();

            if (CurrentState > States.Idle)
                Projectile.DrawPet(blinkFrame, lightColor, drawConfig, 1);

            Projectile.DrawPet(clothFrame, lightColor,
                drawConfig with
                {
                    ShouldUseEntitySpriteDraw = true,
                }, 1);
            Projectile.ResetDrawStateForPet();

            Projectile.DrawPet(Projectile.frame, lightColor, drawConfig);
            Projectile.DrawPet(Projectile.frame, lightColor, config);
            return false;
        }
        public override ChatSettingConfig ChatSettingConfig => new ChatSettingConfig() with
        {
            TextColor = new Color(255, 225, 110),
        };
        public override void RegisterChat(ref string name, ref Vector2 indexRange)
        {
            name = "Luna";
            indexRange = new Vector2(1, 11);
        }
        public override void SetRegularDialog(ref int timePerDialog, ref int chance, ref bool whenShouldStop)
        {
            timePerDialog = 960;
            chance = 8;
            whenShouldStop = CurrentState >= States.Yawn1;
        }
        public override WeightedRandom<string> RegularDialogText()
        {
            WeightedRandom<string> chat = new WeightedRandom<string>();
            {
                if (IsIdleState)
                {
                    chat.Add(ChatDictionary[1]);
                    chat.Add(ChatDictionary[2]);
                    chat.Add(ChatDictionary[3]);
                }
                else
                {
                    chat.Add(ChatDictionary[6]);
                    chat.Add(ChatDictionary[7]);
                }
                if (IsFullMoon)
                {
                    chat.Add(ChatDictionary[8]);
                }
            }
            return chat;
        }
        public override void VisualEffectForPreview()
        {
            UpdateMiscFrame();
        }
        public override void SetPetLight(ref Vector2 position, ref Vector3 rgb, ref bool inactive)
        {
            rgb = new Vector3(1.52f, 1.50f, 1.15f);
        }
        private void UpdateTalking()
        {
        }
        public override void AI()
        {
            Projectile.SetPetActive(Owner, BuffType<LunaBuff>());
            Projectile.SetPetActive(Owner, BuffType<TheThreeFairiesBuff>());

            UpdateTalking();

            ControlMovement(Owner);

            if (ShouldExtraVFXActive)
                GenDust();

            switch (CurrentState)
            {
                case States.Blink:
                    Blink();
                    break;

                case States.ReadingPaper:
                    ReadingPaper();
                    break;

                case States.ReadingPaperBlink:
                    ReadingPaper();
                    ReadingPaperBlink();
                    break;

                case States.AfterReadingPaper:
                    AfterReadingPaper();
                    break;

                case States.Yawn1:
                    shouldNotTalking = true;
                    Yawn();
                    YawnBlink();
                    break;

                case States.Yawn2:
                    shouldNotTalking = true;
                    Projectile.frame = 0;
                    YawnBlink();
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
            int dustID = MyDustId.WhiteTransparent;
            if (Main.rand.NextBool(10))
            {
                Dust d = Dust.NewDustPerfect(Projectile.Center
                + new Vector2(Main.rand.NextFloat(-30f, -10f) * Projectile.spriteDirection, Main.rand.NextFloat(-5f, 25f)), dustID
                , new Vector2(Main.rand.NextFloat(-0.2f, 0.2f), Main.rand.NextFloat(0.8f, 1.2f)), 100, Color.Yellow
                , Main.rand.NextFloat(0.5f, 0.9f));
                d.noGravity = true;
                //d.shader = GameShaders.Armor.GetSecondaryShader(Owner.cLight, Owner);
            }
            if (Main.rand.NextBool(10))
            {
                Dust d = Dust.NewDustPerfect(Projectile.Center
                    + new Vector2(Main.rand.NextFloat(20f, 22f) * Projectile.spriteDirection, Main.rand.NextFloat(-5f, 25f)), dustID
                    , new Vector2(Main.rand.NextFloat(-0.2f, 0.2f), Main.rand.NextFloat(0.8f, 1.2f)), 100, Color.LightGoldenrodYellow
                    , Main.rand.NextFloat(0.5f, 0.9f));
                d.noGravity = true;
                //d.shader = GameShaders.Armor.GetSecondaryShader(Owner.cLight, Owner);
            }
        }
        private void ControlMovement(Player player)
        {
            Projectile.tileCollide = false;
            Projectile.rotation = Projectile.velocity.X * 0.02f;

            ChangeDir(200);

            Vector2 point = new Vector2(50 * player.direction, -40 + player.gfxOffY);
            if (player.HasBuff<TheThreeFairiesBuff>())
            {
                point = new Vector2(60 * player.direction, -70 + player.gfxOffY);
                point += new Vector2(0, -40).RotatedBy(MathHelper.ToRadians(360 / 3 * 2) + Main.GlobalTimeWrappedHourly);
            }
            MoveToPoint(point, 7.5f);
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
                if (Main.dayTime)
                {
                    if (mainTimer > 0 && mainTimer % 420 == 0
                    && currentChatRoom == null && ActionCD <= 0)
                    {
                        if (Main.rand.NextBool(4))
                        {
                            CurrentState = States.Yawn1;
                            int chance = Main.rand.Next(2);
                            switch (chance)
                            {
                                case 1:
                                    Projectile.SetChat(ChatSettingConfig, 4);
                                    break;
                                default:
                                    Projectile.SetChat(ChatSettingConfig, 5);
                                    break;
                            }
                        }
                    }
                }
                else if (Projectile.velocity.Length() < 2f)
                {
                    if (mainTimer > 0 && mainTimer % 120 == 0 && ActionCD <= 0)
                    {
                        if (Main.rand.NextBool(12))
                        {
                            RandomCount = Main.rand.Next(3200, 7200);
                            CurrentState = States.ReadingPaper;
                        }
                    }
                }
            }
        }
        private void Blink()
        {
            int startFrame = 4;
            if (blinkFrame < startFrame)
            {
                blinkFrame = startFrame;
            }
            if (++blinkFrameCounter > 3)
            {
                blinkFrameCounter = 0;
                blinkFrame++;
            }
            if (blinkFrame > 6)
            {
                blinkFrame = startFrame;
                CurrentState = States.Idle;
            }
        }
        private void ReadingPaper()
        {
            if (blinkFrame < 5)
            {
                blinkFrame = 5;
            }
            if (++Projectile.frameCounter > 6)
            {
                Projectile.frameCounter = 0;
                Projectile.frame++;
            }
            if (Projectile.frame > 3)
            {
                Projectile.frame = 3;
            }
            Timer++;
            if (OwnerIsMyPlayer)
            {
                if (Timer > RandomCount || Projectile.velocity.Length() > 4.5f)
                {
                    Timer = 0;
                    CurrentState = States.AfterReadingPaper;
                    return;
                }
                else if (mainTimer % 270 == 0 && CurrentState == States.ReadingPaper)
                {
                    CurrentState = States.ReadingPaperBlink;
                }
            }
        }
        private void ReadingPaperBlink()
        {
            int startFrame = 5;
            if (blinkFrame < startFrame)
            {
                blinkFrame = startFrame;
            }
            if (++blinkFrameCounter > 3)
            {
                blinkFrameCounter = 0;
                blinkFrame++;
            }
            if (blinkFrame > 6)
            {
                blinkFrame = startFrame;
                CurrentState = States.ReadingPaper;
            }
        }
        private void AfterReadingPaper()
        {
            if (++Projectile.frameCounter > 6)
            {
                Projectile.frameCounter = 0;
                Projectile.frame++;
            }
            if (Projectile.frame > 4)
            {
                Projectile.frame = 0;
                if (OwnerIsMyPlayer)
                {
                    ActionCD = 1200;
                    CurrentState = States.Idle;
                }
            }
        }
        private void Yawn()
        {
            if (Projectile.frame < 5)
            {
                Projectile.frame = 5;
            }
            int count = 8;
            if (Projectile.frame == 6)
            {
                count = 180;
            }
            if (++Projectile.frameCounter > count)
            {
                Projectile.frameCounter = 0;
                Projectile.frame++;
            }
            if (Projectile.frame > 7)
            {
                Projectile.frame = 0;
                if (OwnerIsMyPlayer)
                {
                    CurrentState = States.Yawn2;
                }
            }
        }
        private void YawnBlink()
        {
            int startFrame = 4;
            if (blinkFrame < startFrame)
            {
                blinkFrame = startFrame;
            }
            int count = 9;
            if (++blinkFrameCounter > count)
            {
                blinkFrameCounter = 0;
                blinkFrame++;
            }
            if (blinkFrame > 8)
            {
                if (CurrentState == States.Yawn1)
                {
                    blinkFrame = 7;
                }
                else if (CurrentState == States.Yawn2)
                {
                    if (Timer < 120)
                    {
                        blinkFrame = 10;
                        Timer++;
                        return;
                    }
                    if (blinkFrame > 10)
                    {
                        blinkFrame = 9;
                        Timer++;
                    }
                    if (Timer > 123 && OwnerIsMyPlayer)
                    {
                        Timer = 0;
                        ActionCD = 1200;
                        CurrentState = States.Idle;
                    }
                }
            }
        }
        private void UpdateMiscFrame()
        {
            if (wingsFrame < 8)
            {
                wingsFrame = 8;
            }
            if (++wingsFrameCounter > 6)
            {
                wingsFrameCounter = 0;
                wingsFrame++;
            }
            if (wingsFrame > 11)
            {
                wingsFrame = 8;
            }

            if (++clothFrameCounter > 6)
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


