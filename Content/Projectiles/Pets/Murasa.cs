using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria;
using Terraria.Graphics.Shaders;
using Terraria.ID;
using Terraria.Utilities;
using TouhouPets.Content.Buffs.PetBuffs;

namespace TouhouPets.Content.Projectiles.Pets
{
    public class Murasa : BasicTouhouPet
    {
        private enum States
        {
            Idle,
            Blink,
            Throw,
            TakeIt,
            TakeItFail,
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
            get => (int)Projectile.localAI[2];
            set => Projectile.localAI[2] = value;
        }
        private bool ShouldKick
        {
            get => Projectile.ai[2] == 1;
            set => Projectile.ai[2] = value ? 1 : 0;
        }
        private bool IsIdleState => CurrentState <= States.Idle;

        private int blinkFrame, blinkFrameCounter;
        private int hairFrame, hairFrameCounter;
        private int legFrame, legFrameCounter;
        private int shipFrame, shipFrameCounter;

        private DrawPetConfig drawConfig = new(2);
        private readonly Texture2D clothTex = AltVanillaFunction.GetExtraTexture("Murasa_Cloth");
        public override void SetStaticDefaults()
        {
            Main.projFrames[Type] = 22;
            Main.projPet[Type] = true;

            ProjectileID.Sets.CharacterPreviewAnimations[Type] =
                ProjectileID.Sets.SimpleLoop(0, 1)
                .WhenSelected(2, 8, 6);
        }
        public override TouhouPetID UniqueID => TouhouPetID.Murasa;
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

            Projectile.DrawPet(shipFrame, lightColor,
                config with
                {
                    PositionOffset = new Vector2(0, 2)
                }, 1);
            DrawAnchor(lightColor);
            Projectile.ResetDrawStateForPet();

            Projectile.DrawPet(hairFrame, lightColor, drawConfig, 1);
            Projectile.DrawPet(Projectile.frame, lightColor, drawConfig);
            Projectile.DrawPet(legFrame, lightColor, drawConfig, 1);

            if (CurrentState == States.Blink)
                Projectile.DrawPet(blinkFrame, lightColor, drawConfig, 1);

            Projectile.DrawPet(Projectile.frame, lightColor, config2);
            Projectile.DrawPet(legFrame, lightColor, config2, 1);
            return false;
        }
        private void DrawAnchor(Color lightColor)
        {
            Texture2D tex = AltVanillaFunction.ProjectileTexture(Type);
            int frameX = tex.Width / 2;
            int frameY = tex.Height / Main.projFrames[Type];
            Vector2 pos = Projectile.DefaultDrawPetPosition() + new Vector2(-20 * Projectile.spriteDirection, 20);
            Rectangle rect = new Rectangle(frameX, frameY * 18, 14, 18);

            float rotation = MathHelper.Clamp(Projectile.velocity.X * 0.1f, -0.9f, 0.9f);
            rotation += (float)Math.Sin(Main.GlobalTimeWrappedHourly) * 0.1f;

            Main.EntitySpriteDraw(tex, pos, rect, lightColor * mouseOpacity, rotation, new Vector2(rect.Width / 2, 0)
                , Projectile.scale, SpriteEffects.None);
        }
        public override ChatSettingConfig ChatSettingConfig => new ChatSettingConfig() with
        {
            TextColor = new Color(59, 176, 224),
        };
        public override void RegisterChat(ref string name, ref Vector2 indexRange)
        {
            name = "Murasa";
            indexRange = new Vector2(1, 8);
        }
        public override void SetRegularDialog(ref int timePerDialog, ref int chance, ref bool whenShouldStop)
        {
            timePerDialog = 753;
            chance = 8;
            whenShouldStop = !IsIdleState;
        }
        public override WeightedRandom<string> RegularDialogText()
        {
            WeightedRandom<string> chat = new WeightedRandom<string>();
            {
                for (int i = 1; i <= 8; i++)
                {
                    chat.Add(ChatDictionary[i]);
                }
            }
            return chat;
        }
        private void UpdateTalking()
        {
        }
        public override void VisualEffectForPreview()
        {
            UpdateHairFrame();
            UpdateLegFrame();
            UpdateShipFrame();
        }
        public override void AI()
        {
            Projectile.SetPetActive(Owner, BuffType<MurasaBuff>());

            UpdateTalking();

            ControlMovement();

            if (ShouldExtraVFXActive)
                GenDust();

            switch (CurrentState)
            {
                case States.Blink:
                    Blink();
                    break;

                case States.Throw:
                    shouldNotTalking = true;
                    Throw();
                    break;

                case States.TakeIt:
                    shouldNotTalking = true;
                    TakeIt();
                    break;

                case States.TakeItFail:
                    shouldNotTalking = true;
                    TakeItFail();
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
            Projectile.rotation = Projectile.velocity.X * 0.005f;

            ChangeDir();

            Vector2 point = new Vector2(-50 * Owner.direction, -50 + Owner.gfxOffY);
            MoveToPoint(point, 10.5f);
        }
        private void GenDust()
        {
            for (int i = 0; i < 3; i++)
            {
                Dust d = Dust.NewDustPerfect(Projectile.Center, MyDustId.BlueParticle, Vector2.Zero, 100);
                d.position = Projectile.Bottom + new Vector2(Main.rand.Next(-2, 24) * -Projectile.spriteDirection, 10);
                d.noGravity = true;
                d.shader = GameShaders.Armor.GetSecondaryShader(Owner.cPet, Owner);
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
                    if (Main.rand.NextBool(3))
                    {
                        RandomCount = Main.rand.Next(0, 3);
                        CurrentState = States.Throw;
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
            if (blinkFrame > 3)
            {
                blinkFrame = 0;
                if (OwnerIsMyPlayer)
                {
                    if (Main.rand.NextBool(3))
                    {
                        ShouldKick = true;
                        Projectile.netUpdate = true;
                    }

                    CurrentState = States.Idle;
                }
            }
        }
        private void Throw()
        {
            int count = 5;
            if (++Projectile.frameCounter > count)
            {
                Projectile.frameCounter = 0;
                Projectile.frame++;
            }
            if (Projectile.frame > 7)
            {
                if (OwnerIsMyPlayer)
                {
                    bool fail = RandomCount == 0;
                    if (fail)
                    {
                        Gore.NewGore(Projectile.GetSource_FromAI(), Projectile.Center
                            , Vector2.Zero, Mod.Find<ModGore>("MurasaBailer_Gore").Type, Projectile.scale);
                    }
                    CurrentState = fail ? States.TakeItFail : States.TakeIt;
                }
            }
        }
        private void TakeIt()
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
                if (OwnerIsMyPlayer)
                {
                    ActionCD = 600;
                    CurrentState = States.Idle;
                }
            }
        }
        private void TakeItFail()
        {
            int count = 6;
            if (Projectile.frame == 14)
            {
                count = 180;
            }
            if (++Projectile.frameCounter > count)
            {
                Projectile.frameCounter = 0;
                Projectile.frame++;
            }
            if (Projectile.frame < 11)
            {
                Projectile.frame = 11;
            }
            if (Projectile.frame > 21)
            {
                Projectile.frame = 0;
                if (OwnerIsMyPlayer)
                {
                    ActionCD = 1200;
                    CurrentState = States.Idle;
                }
            }
        }
        private void UpdateHairFrame()
        {
            if (hairFrame < 10)
            {
                hairFrame = 10;
            }
            if (++hairFrameCounter > 6)
            {
                hairFrameCounter = 0;
                hairFrame++;
            }
            if (hairFrame > 13)
            {
                hairFrame = 10;
            }
        }
        private void UpdateShipFrame()
        {
            if (shipFrame < 14)
            {
                shipFrame = 14;
            }
            if (++shipFrameCounter > 3)
            {
                shipFrameCounter = 0;
                shipFrame++;
            }
            if (shipFrame > 17)
            {
                shipFrame = 14;
            }
        }
        private void UpdateLegFrame()
        {
            if (legFrame < 3)
            {
                legFrame = 3;
            }
            if (!ShouldKick)
            {
                legFrame = 3;
            }
            else
            {
                if (++legFrameCounter > 5)
                {
                    legFrameCounter = 0;
                    legFrame++;
                }
                if (legFrame > 9)
                {
                    legFrame = 3;
                    if (!Main.gameMenu)
                        ShouldKick = false;
                }
            }
        }
    }
}


