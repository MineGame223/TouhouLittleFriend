using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Threading;
using Terraria;
using Terraria.Utilities;
using TouhouPets.Content.Buffs.PetBuffs;

namespace TouhouPets.Content.Projectiles.Pets
{
    public class Suika : BasicTouhouPet
    {
        private enum States
        {
            Idle,
            Blink,
            Drinking,
            AfterDrinking,
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

        private int skirtFrame, skirtFrameCounter;
        private int hairFrame, hairFrameCounter;
        private int bowFrame, bowFrameCounter;
        private int blinkFrame, blinkFrameCounter;
        private int decorFrame, decorFrameCounter;

        private DrawPetConfig drawConfig = new(2);
        private readonly Texture2D clothTex = AltVanillaFunction.GetExtraTexture("Suika_Cloth");
        private readonly Texture2D decorTex = AltVanillaFunction.GetExtraTexture("Suika_Decoration");
        public override void SetStaticDefaults()
        {
            Main.projFrames[Type] = 11;
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
            DrawDecor();

            Projectile.DrawPet(bowFrame, lightColor, config, 1);
            Projectile.ResetDrawStateForPet();

            Projectile.DrawPet(hairFrame, lightColor, drawConfig, 1);

            Projectile.DrawPet(skirtFrame, lightColor, drawConfig);
            Projectile.DrawPet(skirtFrame, lightColor, config2);
            Projectile.ResetDrawStateForPet();

            Projectile.DrawPet(Projectile.frame, lightColor, drawConfig);
            if (CurrentState == States.Blink)
                Projectile.DrawPet(blinkFrame, lightColor, drawConfig, 1);

            Projectile.DrawPet(Projectile.frame, lightColor, config2);
            return false;
        }
        private void DrawDecor()
        {
            Vector2 pos = Projectile.DefaultDrawPetPosition() + new Vector2(3 * Projectile.spriteDirection, 1);
            int height = decorTex.Height / 4;
            Rectangle frame = new Rectangle(0, height * decorFrame, decorTex.Width, height);
            SpriteEffects effect = Projectile.spriteDirection == -1 ? SpriteEffects.None : SpriteEffects.FlipHorizontally;
            Main.EntitySpriteDraw(decorTex, pos, frame, Projectile.GetAlpha(Color.White), Projectile.rotation, frame.Size() / 2, Projectile.scale, effect, 0f);
        }
        public override Color ChatTextColor => new Color(255, 220, 118);
        public override void RegisterChat(ref string name, ref Vector2 indexRange)
        {
            name = "Suika";
            indexRange = new Vector2(1, 10);
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
                chat.Add(ChatDictionary[4]);
                chat.Add(ChatDictionary[5]);
                chat.Add(ChatDictionary[6]);
                chat.Add(ChatDictionary[7]);
            }
            return chat;
        }
        public override void VisualEffectForPreview()
        {
            UpdateMiscFrames();
        }
        private void UpdateTalking()
        {
        }
        public override void AI()
        {
            Projectile.SetPetActive(Owner, BuffType<SuikaBuff>());

            UpdateTalking();

            ControlMovement();

            switch (CurrentState)
            {
                case States.Blink:
                    Blink();
                    break;

                case States.Drinking:
                    shouldNotTalking = true;
                    Drinking();
                    break;

                case States.AfterDrinking:
                    shouldNotTalking = true;
                    AfterDrinking();
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
            Projectile.rotation = Projectile.velocity.X * 0.012f;

            if (IsIdleState)
                ChangeDir();

            Vector2 point = new Vector2(-60 * Owner.direction, -40 + Owner.gfxOffY);
            MoveToPoint(point, 14f);
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
                if (mainTimer > 0 && mainTimer % 720 == 0 && currentChatRoom == null && ActionCD <= 0)
                {
                    if (Main.rand.NextBool(5))
                    {
                        RandomCount = Main.rand.Next(10, 20);
                        CurrentState = States.Drinking;

                        if (Main.rand.NextBool(2) && chatTimeLeft <= 0)
                            Projectile.SetChat(ChatSettingConfig, Main.rand.Next(8, 11), 20);
                    }
                }
            }
        }
        private void Blink()
        {
            if (blinkFrame < 8)
            {
                blinkFrame = 8;
            }
            if (++blinkFrameCounter > 3)
            {
                blinkFrameCounter = 0;
                blinkFrame++;
            }
            if (blinkFrame > 10)
            {
                blinkFrame = 8;
                CurrentState = States.Idle;
            }
        }
        private void Drinking()
        {
            int count = 6;
            if (Projectile.frame >= 3 && Projectile.frame <= 4)
            {
                count = 18;
            }
            if (++Projectile.frameCounter > count)
            {
                Projectile.frameCounter = 0;
                Projectile.frame++;
            }
            if (Projectile.frame > 4)
            {
                Projectile.frame = 3;
                Timer++;
            }
            if (OwnerIsMyPlayer && Timer > RandomCount)
            {
                Timer = 0;
                CurrentState = States.AfterDrinking;
            }
        }
        private void AfterDrinking()
        {
            int count = 18;
            if (Projectile.frame == 5)
            {
                count = 48;
            }
            if (++Projectile.frameCounter > count)
            {
                Projectile.frameCounter = 0;
                Projectile.frame++;
            }
            if (Projectile.frame > 6)
            {
                Projectile.frame = 0;
                if (OwnerIsMyPlayer)
                {
                    ActionCD = 320;
                    CurrentState = States.Idle;
                }
            }
        }
        private void UpdateMiscFrames()
        {
            if (skirtFrame < 7)
            {
                skirtFrame = 7;
            }
            int count = 6;
            if (++skirtFrameCounter > count)
            {
                skirtFrameCounter = 0;
                skirtFrame++;
            }
            if (skirtFrame > 10)
            {
                skirtFrame = 7;
            }

            count = 8;
            if (++hairFrameCounter > count)
            {
                hairFrameCounter = 0;
                hairFrame++;
            }
            if (hairFrame > 3)
            {
                hairFrame = 0;
            }

            if (bowFrame < 4)
            {
                bowFrame = 4;
            }
            count = 6;
            if (++bowFrameCounter > count)
            {
                bowFrameCounter = 0;
                bowFrame++;
            }
            if (bowFrame > 7)
            {
                bowFrame = 4;
            }

            count = 14;
            if (++decorFrameCounter > count)
            {
                decorFrameCounter = 0;
                decorFrame++;
            }
            if (decorFrame > 3)
            {
                decorFrame = 0;
            }
        }
    }
}


