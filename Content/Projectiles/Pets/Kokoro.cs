using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.IO;
using Terraria;
using Terraria.ID;
using Terraria.Utilities;
using TouhouPets.Content.Buffs.PetBuffs;

namespace TouhouPets.Content.Projectiles.Pets
{
    public class Kokoro : BasicTouhouPet
    {
        private enum States
        {
            Idle,
            Blink,
            Dancing,
            AfterDancing,
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

        private int blinkFrame, blinkFrameCounter;
        private int clothFrame, clothFrameCounter;
        private int hairFrame, hairFrameCounter;
        private int[] maskFrame = new int[4];

        private DrawPetConfig drawConfig = new(2);
        private readonly Texture2D clothTex = AltVanillaFunction.GetExtraTexture("Kokoro_Cloth");
        public override void SetStaticDefaults()
        {
            Main.projFrames[Type] = 14;
            Main.projPet[Type] = true;
            ProjectileID.Sets.LightPet[Type] = true;
        }
        public override bool DrawPetSelf(ref Color lightColor)
        {
            DrawPetConfig config = drawConfig with
            {
                ShouldUseEntitySpriteDraw = true,
            };

            for (int i = 1; i < 4; i++)
            {
                Main.spriteBatch.QuickEndAndBegin(true, Projectile.isAPreviewDummy, BlendState.Additive);
                for (int j = 0; j < 6; j++)
                {
                    DrawMask(Color.Cyan * 0.7f, i, new Vector2(4, 0)
                        .RotatedBy(MathHelper.ToRadians(60 * j) + Main.GlobalTimeWrappedHourly * 2));
                }
                Main.spriteBatch.QuickEndAndBegin(false, Projectile.isAPreviewDummy);

                DrawMask(lightColor, i);
            }

            Projectile.DrawPet(hairFrame, lightColor, drawConfig, 1);

            Projectile.DrawPet(clothFrame, lightColor, config, 1);
            Projectile.ResetDrawStateForPet();

            Projectile.DrawPet(Projectile.frame, lightColor, drawConfig);

            if (CurrentState == States.Blink)
                Projectile.DrawPet(blinkFrame, lightColor, drawConfig);

            Projectile.DrawPet(Projectile.frame, lightColor,
                config with
                {
                    AltTexture = clothTex,
                });
            Projectile.DrawPet(12, lightColor, config, 1);
            Projectile.ResetDrawStateForPet();

            Projectile.DrawPet(maskFrame[0] + 8, lightColor, drawConfig, 1);

            return false;
        }
        private void DrawMask(Color lightColor, int maskIndex, Vector2 posOffset = default)
        {
            if (posOffset == default)
            {
                posOffset = Vector2.Zero;
            }
            Projectile.DrawPet(maskFrame[maskIndex] + 8, lightColor,
                drawConfig with
                {
                    PositionOffset = new Vector2(24, 0)
                        .RotatedBy(MathHelper.ToRadians(120 * maskIndex) + Main.GlobalTimeWrappedHourly)
                        + posOffset
                }, 1);
        }
        public override Color ChatTextColor => new Color(255, 149, 170);
        public override void RegisterChat(ref string name, ref Vector2 indexRange)
        {
            name = "Kokoro";
            indexRange = new Vector2(1, 6);
        }
        public override void SetRegularDialog(ref int timePerDialog, ref int chance, ref bool whenShouldStop)
        {
            timePerDialog = 1020;
            chance = 8;
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
            rgb = new Vector3(1.15f, 1.95f, 2.34f);
        }
        public override void AI()
        {
            Projectile.SetPetActive(Owner, BuffType<KokoroBuff>());

            UpdateTalking();

            ControlMovement();

            switch (CurrentState)
            {
                case States.Blink:
                    Blink();
                    break;

                case States.Dancing:
                    shouldNotTalking = true;
                    Dancing();
                    break;

                case States.AfterDancing:
                    shouldNotTalking = true;
                    AfterDancing();
                    break;

                default:
                    Idle();
                    break;
            }

            if (IsIdleState && ActionCD > 0)
            {
                ActionCD--;
            }

            UpdateMaskState();
        }
        public override void SendExtraAI(BinaryWriter writer)
        {
            base.SendExtraAI(writer);
            for (int i = 0; i < 4; i++)
            {
                writer.Write(maskFrame[i]);
            }
        }
        public override void ReceiveExtraAI(BinaryReader reader)
        {
            base.ReceiveExtraAI(reader);
            for (int i = 0; i < 4; i++)
            {
                maskFrame[i] = reader.ReadInt32();
            }
        }
        private void SetMask(int type)
        {
            maskFrame[0] = type;
            switch (maskFrame[0])
            {
                case 1:
                    maskFrame[1] = 0;
                    maskFrame[2] = 2;
                    maskFrame[3] = 3;
                    break;
                case 2:
                    maskFrame[1] = 1;
                    maskFrame[2] = 0;
                    maskFrame[3] = 3;
                    break;
                case 3:
                    maskFrame[1] = 1;
                    maskFrame[2] = 2;
                    maskFrame[3] = 0;
                    break;
                default:
                    maskFrame[1] = 1;
                    maskFrame[2] = 2;
                    maskFrame[3] = 3;
                    break;
            }
            Projectile.netUpdate = true;
        }
        private void UpdateMaskState()
        {
            if (!OwnerIsMyPlayer)
                return;

            if (Timer == 0)
            {
                SetMask(Main.rand.Next(0, 4));
                RandomCount = Main.rand.Next(600, 1200);
            }
            Timer++;

            if (Timer > RandomCount)
            {
                Timer = 0;
            }
        }
        private void ControlMovement()
        {
            Projectile.tileCollide = false;
            Projectile.rotation = Projectile.velocity.X * 0.01f;

            ChangeDir(150);

            Vector2 point = new Vector2(54 * Owner.direction, -34 + Owner.gfxOffY);
            if (Owner.HasBuff<KomeijiBuff>())
                point = new Vector2(-90 * Owner.direction, -60 + Owner.gfxOffY);
            MoveToPoint(point, 11f);
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
                if (mainTimer > 0 && mainTimer % 600 == 0 && currentChatRoom == null && ActionCD <= 0)
                {
                    if (Main.rand.NextBool(6))
                    {
                        CurrentState = States.Dancing;
                    }
                }
            }
        }
        private void Blink()
        {
            Projectile.frame = 0;
            if (blinkFrame < 11)
            {
                blinkFrame = 11;
            }
            if (++blinkFrameCounter > 3)
            {
                blinkFrameCounter = 0;
                blinkFrame++;
            }
            if (blinkFrame > 13)
            {
                blinkFrame = 11;
                CurrentState = States.Idle;
            }
        }
        private void Dancing()
        {
            int count;
            switch (Projectile.frame)
            {
                case 4:
                    count = 60;
                    break;
                case 6:
                    count = 120;
                    break;
                default:
                    count = 10;
                    break;
            }
            if (++Projectile.frameCounter > count)
            {
                Projectile.frameCounter = 0;
                Projectile.frame++;
            }
            if (Projectile.frame > 6 && OwnerIsMyPlayer)
            {
                CurrentState = States.AfterDancing;
            }
        }
        private void AfterDancing()
        {
            if (++Projectile.frameCounter > 6)
            {
                Projectile.frameCounter = 0;
                Projectile.frame++;
            }
            if (Projectile.frame > 10)
            {
                Projectile.frame = 0;
                if (OwnerIsMyPlayer)
                {
                    ActionCD = 1200;
                    CurrentState = States.Idle;
                }
            }
        }
        private void UpdateMiscFrame()
        {
            int count = 8;
            if (++clothFrameCounter > count)
            {
                clothFrameCounter = 0;
                clothFrame++;
            }
            if (clothFrame > 3)
            {
                clothFrame = 0;
            }

            if (hairFrame < 4)
            {
                hairFrame = 4;
            }
            if (++hairFrameCounter > count)
            {
                hairFrameCounter = 0;
                hairFrame++;
            }
            if (hairFrame > 7)
            {
                hairFrame = 4;
            }
        }
    }
}


