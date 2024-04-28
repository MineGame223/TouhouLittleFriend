using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
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
            get => Projectile.ai[2] == 0;
            set => Projectile.ai[2] = value ? 0 : 1;
        }
        private bool IsIdleState => CurrentState <= States.Idle;

        private int blinkFrame, blinkFrameCounter;
        private int hairFrame, hairFrameCounter;
        private int legFrame, legFrameCounter;

        private DrawPetConfig drawConfig = new(2);
        private readonly Texture2D clothTex = AltVanillaFunction.GetExtraTexture("Murasa_Cloth");
        public override void SetStaticDefaults()
        {
            Main.projFrames[Type] = 22;
            Main.projPet[Type] = true;
        }
        public override bool PreDraw(ref Color lightColor)
        {
            DrawPetConfig config = drawConfig with
            {
                ShouldUseEntitySpriteDraw = true,
            };
            DrawPetConfig config2 = config with
            {
                AltTexture = clothTex,
            };

            Projectile.DrawPet(14, lightColor, config, 1);
            Projectile.DrawStateNormalizeForPet();

            Projectile.DrawPet(hairFrame, lightColor, drawConfig, 1);
            Projectile.DrawPet(Projectile.frame, lightColor, drawConfig);
            Projectile.DrawPet(legFrame, lightColor, drawConfig, 1);

            if (CurrentState == States.Blink)
                Projectile.DrawPet(blinkFrame, lightColor, drawConfig, 1);

            Projectile.DrawPet(Projectile.frame, lightColor, config2);
            Projectile.DrawPet(legFrame, lightColor, config2, 1);
            return false;
        }
        public override Color ChatTextColor => new Color(59, 176, 224);
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
        public override string GetRegularDialogText()
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
        }
        public override void AI()
        {
            Projectile.SetPetActive(Owner, BuffType<MurasaBuff>());

            UpdateTalking();

            ControlMovement();

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
                        ShouldKick = true;

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


