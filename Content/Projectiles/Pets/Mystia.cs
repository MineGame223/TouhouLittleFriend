using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ID;
using Terraria.Utilities;
using TouhouPets.Content.Buffs.PetBuffs;

namespace TouhouPets.Content.Projectiles.Pets
{
    public class Mystia : BasicTouhouPet
    {
        private enum States
        {
            Idle,
            Blink,
            Singing,
            AfterSinging,
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
        public override void SetStaticDefaults()
        {
            Main.projFrames[Type] = 13;
            Main.projPet[Type] = true;

            ProjectileID.Sets.CharacterPreviewAnimations[Type] =
                ProjectileID.Sets.SimpleLoop(0, 1)
                .WhenSelected(2, 3, 7);
        }
        private int wingFrame, wingFrameCounter;
        private int blinkFrame, blinkFrameCounter;
        private int clothFrame, clothFrameCounter;
        private int extraAdjX, extraAdjY;
        private bool blackDye;

        private DrawPetConfig drawConfig = new(2);
        private readonly Texture2D clothTex = AltVanillaFunction.GetExtraTexture("Mystia_Cloth");
        private readonly Texture2D patchTex = AltVanillaFunction.GetExtraTexture("Mystia_EyePatch");
        public override bool DrawPetSelf(ref Color lightColor)
        {
            DrawPetConfig config = drawConfig with
            {
                PositionOffset = new Vector2(extraAdjX, extraAdjY),
            };

            Projectile.DrawPet(wingFrame, lightColor, config, 1);

            Projectile.DrawPet(Projectile.frame, lightColor, drawConfig);

            if (CurrentState == States.Blink)
                Projectile.DrawPet(blinkFrame, lightColor, drawConfig);

            Projectile.DrawPet(Projectile.frame, lightColor,
               drawConfig with
               {
                   ShouldUseEntitySpriteDraw = true,
                   AltTexture = clothTex,
               });
            Projectile.DrawPet(clothFrame, lightColor,
                config with
                {
                    ShouldUseEntitySpriteDraw = true,
                });
            Projectile.ResetDrawStateForPet();

            if (blackDye && !GetInstance<MiscConfig>().CompatibilityMode)
                Projectile.DrawPet(clothFrame, lightColor,
                    config with
                    {
                        AltTexture = patchTex,
                    });
            return false;
        }
        public override Color ChatTextColor => new Color(246, 110, 169);
        public override void RegisterChat(ref string name, ref Vector2 indexRange)
        {
            name = "Mystia";
            indexRange = new Vector2(1, 9);
        }
        public override void SetRegularDialog(ref int timePerDialog, ref int chance, ref bool whenShouldStop)
        {
            timePerDialog = 840;
            chance = 6;
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
            }
            return chat;
        }
        public override void VisualEffectForPreview()
        {
            UpdateWingFrame();
            UpdateClothFrame();
            UpdateMiscData();
        }
        private void UpdateTalking()
        {
        }
        public override void AI()
        {
            Projectile.SetPetActive(Owner, BuffType<MystiaBuff>());
            UpdateTalking();

            ControlMovement();

            switch (CurrentState)
            {
                case States.Blink:
                    Blink();
                    break;

                case States.Singing:
                    shouldNotTalking = true;
                    Singing();
                    break;

                case States.AfterSinging:
                    shouldNotTalking = true;
                    AfterSinging();
                    break;

                default:
                    Idle();
                    break;
            }

            if (IsIdleState && ActionCD > 0)
            {
                ActionCD--;
            }
            blackDye = Owner.miscDyes[0].type == ItemID.BlackDye;
        }
        private void ControlMovement()
        {
            Projectile.tileCollide = false;
            if (CurrentState != States.Singing)
                Projectile.rotation = Projectile.velocity.X * 0.035f;
            else
                Projectile.rotation = Projectile.velocity.X * 0.005f;

            ChangeDir();

            Vector2 point = new Vector2(-50 * Owner.direction, -50 + Owner.gfxOffY);
            MoveToPoint(point, 14f);
        }
        private void UpdateMiscData()
        {
            extraAdjX = 0;
            extraAdjY = 0;
            if (Projectile.frame >= 1 && Projectile.frame <= 5)
            {
                extraAdjY = -2;
                if (Projectile.frame >= 2 && Projectile.frame <= 4)
                {
                    extraAdjY = -4;
                    extraAdjX = -2 * Projectile.spriteDirection;
                }
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
                if (mainTimer > 0 && mainTimer % 1200 == 0 && currentChatRoom == null && ActionCD <= 0)
                {
                    if (Main.rand.NextBool(3))
                    {
                        RandomCount = Main.rand.Next(60, 180);
                        CurrentState = States.Singing;

                        int chance = Main.rand.Next(4);
                        switch (chance)
                        {
                            case 1:
                                Projectile.SetChat(ChatSettingConfig, 5, 90);
                                break;
                            case 2:
                                Projectile.SetChat(ChatSettingConfig, 6, 90);
                                break;
                            case 3:
                                Projectile.SetChat(ChatSettingConfig, 7, 90);
                                break;
                            default:
                                Projectile.SetChat(ChatSettingConfig, 8, 90);
                                break;
                        }
                    }
                }
            }
        }
        private void Blink()
        {
            Projectile.frame = 0;
            if (blinkFrame < 10)
            {
                blinkFrame = 10;
            }
            if (++blinkFrameCounter > 3)
            {
                blinkFrameCounter = 0;
                blinkFrame++;
            }
            if (blinkFrame > 12)
            {
                blinkFrame = 10;
                CurrentState = States.Idle;
            }
        }
        private void Singing()
        {
            Projectile.velocity *= 0.8f;
            if (++Projectile.frameCounter > 7)
            {
                Projectile.frameCounter = 0;
                Projectile.frame++;
            }
            if (Projectile.frame > 4)
            {
                if (Main.rand.NextBool(4) && ShouldExtraVFXActive)
                    Gore.NewGoreDirect(Projectile.GetSource_FromAI(), Projectile.Center + new Vector2(0, -27), new Vector2(Main.rand.Next(-5, 5), Main.rand.Next(-6, -3)) * 0.1f, Main.rand.Next(570, 573), Main.rand.NextFloat(0.9f, 1.1f));

                Projectile.frame = 2;
                Timer++;
            }
            if (OwnerIsMyPlayer && Timer > RandomCount)
            {
                Timer = 0;
                CurrentState = States.AfterSinging;
            }
        }
        private void AfterSinging()
        {
            if (++Projectile.frameCounter > 7)
            {
                Projectile.frameCounter = 0;
                Projectile.frame++;
            }
            if (Projectile.frame > 5)
            {
                Projectile.frame = 0;
                if (OwnerIsMyPlayer)
                {
                    ActionCD = 3600;
                    CurrentState = States.Idle;
                }
            }
        }
        private void UpdateWingFrame()
        {
            int count = 5;
            if (++wingFrameCounter > count)
            {
                wingFrameCounter = 0;
                wingFrame++;
            }
            if (wingFrame > 5)
            {
                wingFrame = 0;
            }
        }
        private void UpdateClothFrame()
        {
            if (clothFrame < 6)
            {
                clothFrame = 6;
            }
            int count = 5;
            if (++clothFrameCounter > count)
            {
                clothFrameCounter = 0;
                clothFrame++;
            }
            if (clothFrame > 9)
            {
                clothFrame = 6;
            }
        }
    }
}


