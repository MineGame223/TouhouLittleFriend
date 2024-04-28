using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ID;
using Terraria.Utilities;
using TouhouPets.Content.Buffs.PetBuffs;

namespace TouhouPets.Content.Projectiles.Pets
{
    public class Wakasagihime : BasicTouhouPet
    {
        private enum States
        {
            Idle,
            Blink,
            BlowingBubble,
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
        private int RandomCount
        {
            get => (int)Projectile.localAI[1];
            set => Projectile.localAI[1] = value;
        }
        private int RandomCount2
        {
            get => (int)Projectile.localAI[2];
            set => Projectile.localAI[2] = value;
        }
        private bool IsIdleState => CurrentState <= States.Blink;

        private int tailFrame, tailFrameCounter;
        private int blinkFrame, blinkFrameCounter;

        private DrawPetConfig drawConfig = new(2);
        private readonly Texture2D clothTex = AltVanillaFunction.GetExtraTexture("Wakasagihime_Cloth");
        public override void SetStaticDefaults()
        {
            Main.projFrames[Type] = 10;
            Main.projPet[Type] = true;
            ProjectileID.Sets.LightPet[Type] = true;
        }
        public override bool PreDraw(ref Color lightColor)
        {
            int extraHeight = Projectile.frame == 2 ? -2 : 0;
            DrawPetConfig config = drawConfig with
            {
                ShouldUseEntitySpriteDraw = true,
            };
            DrawPetConfig config2 = config with
            {
                AltTexture = clothTex,
            };

            Projectile.DrawPet(tailFrame, lightColor, drawConfig, 1);

            Projectile.DrawPet(Projectile.frame, lightColor, drawConfig);

            if (CurrentState == States.Blink)
                Projectile.DrawPet(blinkFrame, lightColor, drawConfig);

            Projectile.DrawPet(Projectile.frame, lightColor, config2);

            for (int i = 0; i < 7; i++)
            {
                Projectile.DrawPet(9, Color.White * 0.5f,
                    config with
                    {
                        PositionOffset = new Vector2(0, extraHeight) + new Vector2(Main.rand.Next(-10, 11) * 0.2f, Main.rand.Next(-10, 11) * 0.2f),
                    });
            }
            Projectile.DrawStateNormalizeForPet();

            Projectile.DrawPet(8, lightColor,
                    drawConfig with
                    {
                        PositionOffset = new Vector2(0, extraHeight),
                    });
            Projectile.DrawPet(8, lightColor,
                    config2 with
                    {
                        PositionOffset = new Vector2(0, extraHeight),
                    });
            return false;
        }
        public override Color ChatTextColor => new Color(87, 164, 255);
        public override void RegisterChat(ref string name, ref Vector2 indexRange)
        {
            name = "Wakasagihime";
            indexRange = new Vector2(1, 8);
        }
        public override void SetRegularDialog(ref int timePerDialog, ref int chance, ref bool whenShouldStop)
        {
            timePerDialog = 600;
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
            }
            return chat;
        }
        private void UpdateTalking()
        {

        }
        public override void VisualEffectForPreview()
        {
            UpdateTailFrame();
        }
        public override void SetPetLight(ref Vector2 position, ref Vector3 rgb, ref bool inactive)
        {
            float brightness = Main.essScale * (Projectile.wet ? 1f : 2f);
            rgb = new Vector3(0.87f, 1.64f, 1.55f) * brightness;
        }
        public override void AI()
        {
            Projectile.SetPetActive(Owner, BuffType<WakasagihimeBuff>());

            UpdateTalking();

            ControlMovement();

            GenDust();

            switch (CurrentState)
            {
                case States.Blink:
                    Blink();
                    break;

                case States.BlowingBubble:
                    shouldNotTalking = true;
                    BlowingBubble();
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
            if (!Projectile.wet)
            {
                int dustID = MyDustId.BlueParticle;
                Dust.NewDustPerfect(Projectile.Center + new Vector2(Main.rand.NextFloat(-15f, 15f), 28), dustID
                    , new Vector2(Main.rand.NextFloat(-0.2f, 0.2f), Main.rand.NextFloat(-1f, -0.2f)), 100, default
                    , Main.rand.NextFloat(0.75f, 1.05f)).noGravity = true;
            }
        }
        private void ControlMovement()
        {
            Projectile.tileCollide = false;
            Projectile.rotation = Projectile.velocity.X * 0.022f;

            ChangeDir();

            Vector2 point = new Vector2(40 * Owner.direction, -40 + Owner.gfxOffY);
            float speed = Projectile.wet ? 18f : 9f;
            MoveToPoint(point, speed);
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
                if (mainTimer > 0 && mainTimer % 800 == 0 && currentChatRoom == null && ActionCD <= 0)
                {
                    if (Main.rand.NextBool(5))
                    {
                        RandomCount = Main.rand.Next(360, 480);
                        RandomCount2 = Main.rand.Next(30, 90);
                        CurrentState = States.BlowingBubble;

                        Projectile.SetChat(ChatSettingConfig, 4, 120);
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
        private void BlowingBubble()
        {
            if (++Projectile.frameCounter > 8)
            {
                Projectile.frameCounter = 0;
                Projectile.frame++;
            }
            if (Projectile.frame >= 2 && RandomCount2 > 0)
            {
                RandomCount2--;
                Projectile.frame = 2;
            }
            if (Projectile.frame >= 3 && RandomCount > 0)
            {
                RandomCount--;
                Projectile.frame = 3;
            }
            if (Projectile.frame == 3 && Projectile.frameCounter == 0)
            {
                AltVanillaFunction.PlaySound(SoundID.Item85, Projectile.Center);
                if (OwnerIsMyPlayer)
                {
                    Projectile.NewProjectile(Projectile.GetSource_FromThis(), Projectile.Center + new Vector2(4 * Projectile.spriteDirection, 4)
                    , new Vector2(Main.rand.NextFloat(1f, 3f) * Projectile.spriteDirection, Main.rand.NextFloat(-0.4f, 0.2f)), ProjectileType<WakasagihimeBubble>(), 0, 0, Main.myPlayer
                    , Main.rand.Next(0, 3), Main.rand.NextFloat(0.6f, 1.2f));
                }
            }
            if (Projectile.frame > 4 && RandomCount <= 0)
            {
                ActionCD = 1200;
                CurrentState = States.Idle;
            }
        }
        private void UpdateTailFrame()
        {
            if (++tailFrameCounter > 5)
            {
                tailFrameCounter = 0;
                tailFrame++;
            }
            if (tailFrame > 5)
            {
                tailFrame = 0;
            }
        }
    }
}


