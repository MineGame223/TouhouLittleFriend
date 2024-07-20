using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.GameContent.Drawing;
using Terraria.Graphics.Shaders;
using Terraria.ID;
using Terraria.Utilities;
using TouhouPets.Content.Buffs.PetBuffs;

namespace TouhouPets.Content.Projectiles.Pets
{
    public class Star : BasicTouhouPet
    {
        private enum States
        {
            Idle,
            Blink,
            StarMagic,
            AfterStarMagic,
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
        private bool CanSeeStar => Main.cloudAlpha <= 0 && !Main.dayTime && !Main.bloodMoon
            && (Owner.ZoneOverworldHeight || Owner.ZoneSkyHeight);

        private int blinkFrame, blinkFrameCounter;
        private int wingsFrame, wingsFrameCounter;
        private int clothFrame, clothFrameCounter;
        private int hairFrame, hairFrameCounter;
        private float extraX, extraY;

        private DrawPetConfig drawConfig = new(2);
        private readonly Texture2D clothTex = AltVanillaFunction.GetExtraTexture("Star_Cloth");
        public override void SetStaticDefaults()
        {
            Main.projFrames[Type] = 12;
            Main.projPet[Type] = true;
            ProjectileID.Sets.LightPet[Type] = true;
        }
        public override bool DrawPetSelf(ref Color lightColor)
        {
            Vector2 extraPos = new Vector2(extraX, extraY);
            DrawPetConfig config = drawConfig with
            {
                PositionOffset = extraPos,
            };

            Projectile.DrawPet(wingsFrame, lightColor * 0.8f, config, 1);

            Projectile.DrawPet(hairFrame, lightColor, config, 1);

            Projectile.DrawPet(Projectile.frame, lightColor, drawConfig);

            if (CurrentState == States.Blink)
                Projectile.DrawPet(blinkFrame, lightColor, drawConfig);

            Projectile.DrawPet(Projectile.frame, lightColor,
                drawConfig with
                {
                    AltTexture = clothTex,
                    ShouldUseEntitySpriteDraw = true,
                });
            Projectile.DrawPet(clothFrame, lightColor,
                config with
                {
                    ShouldUseEntitySpriteDraw = true,
                }, 1);
            return false;
        }
        public override Color ChatTextColor => new Color(135, 143, 237);
        public override void RegisterChat(ref string name, ref Vector2 indexRange)
        {
            name = "Star";
            indexRange = new Vector2(1, 8);
        }
        public override void SetRegularDialog(ref int timePerDialog, ref int chance, ref bool whenShouldStop)
        {
            timePerDialog = 720;
            chance = 7;
            whenShouldStop = !IsIdleState;
        }
        public override string GetRegularDialogText()
        {
            WeightedRandom<string> chat = new WeightedRandom<string>();
            {
                chat.Add(ChatDictionary[1]);
                chat.Add(ChatDictionary[2]);
                chat.Add(ChatDictionary[3]);
                chat.Add(ChatDictionary[8]);
                if (Main.dayTime)
                    chat.Add(ChatDictionary[4]);
                else if (CanSeeStar)
                    chat.Add(ChatDictionary[5]);
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
            rgb = new Vector3(1.35f, 1.43f, 2.37f);
        }
        public override void AI()
        {
            Projectile.SetPetActive(Owner, BuffType<StarBuff>());
            Projectile.SetPetActive(Owner, BuffType<TheThreeFairiesBuff>());

            UpdateTalking();

            ControlMovement(Owner);

            GenDust();

            switch (CurrentState)
            {
                case States.Blink:
                    Blink();
                    break;

                case States.StarMagic:
                    shouldNotTalking = true;
                    StarMagic();
                    break;

                case States.AfterStarMagic:
                    shouldNotTalking = true;
                    AfterStarMagic();
                    break;

                default:
                    Idle();
                    break;
            }
            if (IsIdleState && ActionCD > 0)
            {
                ActionCD--;
            }
            UpdateExtraPos();
        }
        private void UpdateExtraPos()
        {
            extraX = 0;
            extraY = 0;
            if (Projectile.frame >= 1 && Projectile.frame <= 5)
            {
                extraY = -2;
                if (Projectile.frame >= 2 && Projectile.frame <= 4)
                {
                    extraX = -2 * Projectile.spriteDirection;
                }
            }
        }
        private void GenDust()
        {
            int dustID = MyDustId.BlueMagic;
            if (Main.rand.NextBool(10))
            {
                Dust d = Dust.NewDustPerfect(Projectile.Center
                + new Vector2(Main.rand.NextFloat(-30f, -10f) * Projectile.spriteDirection, Main.rand.NextFloat(-5f, 25f)), dustID
                , new Vector2(Main.rand.NextFloat(-0.2f, 0.2f), Main.rand.NextFloat(0.8f, 1.2f)), 100, default
                , Main.rand.NextFloat(1f, 2f));
                d.noGravity = true;
                //d.shader = GameShaders.Armor.GetSecondaryShader(Owner.cLight, Owner);
            }
            if (Main.rand.NextBool(10))
            {
                Dust d = Dust.NewDustPerfect(Projectile.Center
                    + new Vector2(Main.rand.NextFloat(20f, 22f) * Projectile.spriteDirection, Main.rand.NextFloat(-5f, 25f)), dustID
                    , new Vector2(Main.rand.NextFloat(-0.2f, 0.2f), Main.rand.NextFloat(0.8f, 1.2f)), 100, default
                    , Main.rand.NextFloat(1f, 2f));
                d.noGravity = true;
                //d.shader = GameShaders.Armor.GetSecondaryShader(Owner.cLight, Owner);
            }
            if (Projectile.velocity.Length() > 4)
            {
                ParticleOrchestraSettings settings = new ParticleOrchestraSettings
                {
                    PositionInWorld = Projectile.position + new Vector2(Main.rand.Next(0, Projectile.width), Main.rand.Next(0, Projectile.height)),
                    MovementVector = Vector2.Zero,
                };
                if (Main.rand.NextBool(3))
                    ParticleOrchestrator.SpawnParticlesDirect(ParticleOrchestraType.StardustPunch, settings);
                //if (Main.rand.NextBool(6))
                //Gore.NewGoreDirect(Projectile.GetSource_FromAI(), Projectile.position + new Vector2(Main.rand.Next(0, Projectile.width), Main.rand.Next(0, Projectile.height)), Vector2.Normalize(Projectile.velocity) * -2, Main.rand.Next(16, 18), Main.rand.NextFloat(0.9f, 1.1f));
            }
        }
        private void ControlMovement(Player player)
        {
            Projectile.tileCollide = false;
            Projectile.rotation = Projectile.velocity.X * 0.02f;

            ChangeDir(200);

            Vector2 point = new Vector2(50 * player.direction, -30 + player.gfxOffY);
            if (player.HasBuff<TheThreeFairiesBuff>())
            {
                point = new Vector2(60 * player.direction, -70 + player.gfxOffY);
                point += new Vector2(0, -40).RotatedBy(MathHelper.ToRadians(360 / 3 * 1) + Main.GlobalTimeWrappedHourly);
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
                if (mainTimer > 0 && mainTimer % 660 == 0 && CanSeeStar
                    && currentChatRoom == null && ActionCD <= 0)
                {
                    if (Main.rand.NextBool(6))
                    {
                        RandomCount = Main.rand.Next(120, 180);
                        CurrentState = States.StarMagic;
                    }
                }
            }
        }
        private void Blink()
        {
            Projectile.frame = 0;
            if (blinkFrame < 9)
            {
                blinkFrame = 9;
            }
            if (++blinkFrameCounter > 3)
            {
                blinkFrameCounter = 0;
                blinkFrame++;
            }
            if (blinkFrame > 11)
            {
                blinkFrame = 9;
                CurrentState = States.Idle;
            }
        }
        private void StarMagic()
        {
            if (++Projectile.frameCounter > 6)
            {
                Projectile.frameCounter = 0;
                Projectile.frame++;
            }
            if (Projectile.frame > 4)
            {
                Projectile.frame = 3;
                Timer++;
            }

            if (Timer % 2 == 0)
            {
                Projectile starTrail = Projectile.NewProjectileDirect(Projectile.GetSource_FromAI(), Projectile.position,
                    Vector2.Zero, ProjectileType<StarTrail>(), 0, 0, Projectile.owner
                    , Main.rand.Next(30, 60), Main.rand.Next(50, 100 + Timer), Main.rand.Next(0, 360));
                starTrail.localAI[2] = Projectile.whoAmI;
                starTrail.netUpdate = true;
            }

            if (OwnerIsMyPlayer && Timer > RandomCount)
            {
                Timer = 0;
                CurrentState = States.AfterStarMagic;
            }
        }
        private void AfterStarMagic()
        {
            if (++Projectile.frameCounter > 6)
            {
                Projectile.frameCounter = 0;
                Projectile.frame++;
            }
            if (Projectile.frame > 5)
            {
                Projectile.frame = 0;
                if (OwnerIsMyPlayer)
                {
                    ActionCD = 10800;
                    CurrentState = States.Idle;
                }
            }
        }
        private void UpdateMiscFrame()
        {
            if (wingsFrame < 8)
            {
                wingsFrame = 8;
            }
            if (++wingsFrameCounter > 5)
            {
                wingsFrameCounter = 0;
                wingsFrame++;
            }
            if (wingsFrame > 11)
            {
                wingsFrame = 8;
            }

            if (hairFrame < 4)
            {
                hairFrame = 4;
            }
            if (++hairFrameCounter > 7)
            {
                hairFrameCounter = 0;
                hairFrame++;
            }
            if (hairFrame > 7)
            {
                hairFrame = 4;
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


