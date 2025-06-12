using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ID;
using Terraria.Utilities;
using TouhouPets.Content.Buffs.PetBuffs;

namespace TouhouPets.Content.Projectiles.Pets
{
    public class Reisen : BasicTouhouPet
    {
        private enum States
        {
            Idle,
            Blink,
            Staring,
            AfterStaring,
            Nerves,
            NervesBlink,
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

        private int earFrame, earFrameCounter;
        private int blinkFrame, blinkFrameCounter;
        private int clothFrame, clothFrameCounter;
        private int hairFrame, hairFrameCounter;
        private int legFrame, legFrameCounter;
        private float eyeScale = 1;
        private bool blackDye;

        private DrawPetConfig drawConfig = new(3);
        private readonly Texture2D eyeTex = AltVanillaFunction.GetGlowTexture("Reisen_Glow");
        private readonly Texture2D clothTex = AltVanillaFunction.GetExtraTexture("Reisen_Cloth");
        private readonly Texture2D altClothTex = AltVanillaFunction.GetExtraTexture("Reisen_Cloth_Alt");
        public override void SetStaticDefaults()
        {
            Main.projFrames[Type] = 12;
            Main.projPet[Type] = true;

            ProjectileID.Sets.CharacterPreviewAnimations[Type] =
                ProjectileID.Sets.SimpleLoop(0, 1)
                .WhenSelected(2, 1);
        }
        public override TouhouPetID UniqueID => TouhouPetID.Reisen;
        public override bool DrawPetSelf(ref Color lightColor)
        {
            if (eyeScale > 0)
            {
                Color clr = Projectile.GetAlpha(Color.Red * (1 - eyeScale) * 0.3f);
                clr.A *= 0;
                DrawReisen(clr, 1 + eyeScale * 1.1f);
                for (int i = -1; i <= 1; i++)
                {
                    if (i == 0)
                        continue;
                    DrawReisen(clr, 1f, new Vector2(4 * i, 0));
                    DrawReisen(clr, 1f, new Vector2(0, 4 * i));
                }
            }

            DrawReisen(lightColor);

            if (eyeScale > 0)
            {
                DrawPetConfig config = drawConfig with
                {
                    Scale = 1 + eyeScale * 2,
                    AltTexture = eyeTex,
                };
                Color clr = Projectile.GetAlpha(Color.White * (0.9f - eyeScale));
                clr.A *= 0;
                Projectile.DrawPet(Projectile.frame, clr, config, 0);
            }
            return false;
        }
        private void DrawReisen(Color lightColor, float scale = 1, Vector2? posOffset = default)
        {
            DrawPetConfig config = drawConfig with
            {
                Scale = scale,
                PositionOffset = posOffset ?? Vector2.Zero,
            };
            DrawPetConfig config2 = config with
            {
                AltTexture = blackDye ? altClothTex : clothTex,
                ShouldUseEntitySpriteDraw = !blackDye,
            };

            DrawPetConfig config3 = config with
            {
                AltTexture = eyeTex,
            };

            Projectile.DrawPet(hairFrame, lightColor, config, 1);
            Projectile.DrawPet(earFrame, lightColor, config, 0);

            Projectile.DrawPet(legFrame, lightColor, config, 2);
            Projectile.DrawPet(legFrame, lightColor, config2, 2);
            Projectile.ResetDrawStateForPet();

            Projectile.DrawPet(Projectile.frame, lightColor, config);
            Projectile.DrawPet(Projectile.frame, Projectile.GetAlpha(Color.White * 0.6f), config3);

            if (CurrentState == States.Blink)
            {
                Projectile.DrawPet(blinkFrame, lightColor, config, 1);
                Projectile.DrawPet(blinkFrame, Projectile.GetAlpha(Color.White * 0.6f), config3, 1);
            }
            Projectile.DrawPet(Projectile.frame, lightColor, config2, 0);
            Projectile.DrawPet(clothFrame, lightColor, config2, 1);
            Projectile.ResetDrawStateForPet();
        }
        public override ChatSettingConfig ChatSettingConfig => new ChatSettingConfig() with
        {
            TextColor = new Color(255, 10, 10),
        };
        public override void RegisterChat(ref string name, ref Vector2 indexRange)
        {
            name = "Reisen";
            indexRange = new Vector2(1, 11);
        }
        public override void SetRegularDialog(ref int timePerDialog, ref int chance, ref bool whenShouldStop)
        {
            timePerDialog = 610;
            chance = 8;
            whenShouldStop = !IsIdleState;
        }
        public override WeightedRandom<string> RegularDialogText()
        {
            WeightedRandom<string> chat = new WeightedRandom<string>();
            {
                for (int j = 1; j <= 9; j++)
                {
                    chat.Add(ChatDictionary[j]);
                }
                if (Main.bloodMoon)
                {
                    chat.Add(ChatDictionary[10]);
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
        public override void AI()
        {
            Projectile.SetPetActive(Owner, BuffType<ReisenBuff>());
            Projectile.SetPetActive(Owner, BuffType<EienteiBuff>());

            UpdateTalking();

            ControlMovement();

            if (IsIdleState && FindPet(ProjectileType<Junko>(), false))
            {
                CurrentState = States.Nerves;
            }

            switch (CurrentState)
            {
                case States.Blink:
                    Blink();
                    break;

                case States.Staring:
                    shouldNotTalking = true;
                    Staring();
                    break;

                case States.AfterStaring:
                    shouldNotTalking = true;
                    AfterStaring();
                    break;

                case States.Nerves:
                    Nerves();
                    break;

                case States.NervesBlink:
                    NervesBlink();
                    break;

                default:
                    Idle();
                    break;
            }

            if (IsIdleState && ActionCD > 0)
            {
                ActionCD--;
                eyeScale = 1f;
            }
            blackDye = Owner.miscDyes[0].type == ItemID.BlackDye;
        }
        private void ControlMovement()
        {
            Projectile.tileCollide = false;
            Projectile.rotation = Projectile.velocity.X * 0.014f;

            ChangeDir();

            Vector2 point = new Vector2(-50 * Owner.direction, -40 + Owner.gfxOffY);
            if (Owner.HasBuff<EienteiBuff>())
            {
                point = new Vector2(40 * Owner.direction, -70 + Owner.gfxOffY);
            }
            MoveToPoint(point, 13f);
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
                    if (Main.rand.NextBool(5))
                    {
                        RandomCount = Main.rand.Next(300, 600);
                        CurrentState = States.Staring;
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
        private void Staring()
        {
            int count = 9;
            if (++Projectile.frameCounter > count)
            {
                Projectile.frameCounter = 0;
                Projectile.frame++;
            }
            if (Projectile.frame >= 2)
            {
                Projectile.frame = 2;
                eyeScale += 0.05f;
                if (eyeScale > 1)
                {
                    eyeScale = 0;
                }
                Lighting.AddLight(Projectile.Center, 0.3f * eyeScale, 0, 0);
            }
            Timer++;
            if (OwnerIsMyPlayer && Timer >= RandomCount)
            {
                Timer = 0;
                CurrentState = States.AfterStaring;
            }
        }
        private void AfterStaring()
        {
            int count = 9;
            if (++Projectile.frameCounter > count)
            {
                Projectile.frameCounter = 0;
                Projectile.frame++;
            }

            if (eyeScale < 1)
                eyeScale += 0.1f;

            if (Projectile.frame > 4)
            {
                Projectile.frame = 0;
                if (OwnerIsMyPlayer)
                {
                    ActionCD = 900;
                    CurrentState = States.Idle;
                }
            }
        }
        private void Nerves()
        {
            Projectile.frame = 5;
            if (OwnerIsMyPlayer)
            {
                if (Owner.ownedProjectileCounts[ProjectileType<Junko>()] <= 0)
                {
                    Timer = 0;
                    CurrentState = States.Idle;
                    return;
                }
                if (++Timer > 270)
                {
                    Timer = 0;
                    CurrentState = States.NervesBlink;
                }
            }
        }
        private void NervesBlink()
        {
            Projectile.frame = 6;
            if (OwnerIsMyPlayer)
            {
                if (++Timer > 3)
                {
                    Timer = 0;
                    CurrentState = States.Nerves;
                }
            }
        }
        private void UpdateMiscFrame()
        {
            if (clothFrame < 3)
            {
                clothFrame = 3;
            }
            int count = 6;
            if (++clothFrameCounter > count)
            {
                clothFrameCounter = 0;
                clothFrame++;
            }
            if (clothFrame > 6)
            {
                clothFrame = 3;
            }

            if (CurrentState < States.Nerves)
            {
                if (earFrame < 7)
                {
                    earFrame = 7;
                }
                count = 6;
                if (++earFrameCounter > count)
                {
                    earFrameCounter = 0;
                    earFrame++;
                }
                if (earFrame > 10)
                {
                    earFrame = 7;
                }
            }
            else
            {
                earFrame = 11;
            }

            if (hairFrame < 7)
            {
                hairFrame = 7;
            }
            count = 6;
            if (++hairFrameCounter > count)
            {
                hairFrameCounter = 0;
                hairFrame++;
            }
            if (hairFrame > 10)
            {
                hairFrame = 7;
            }

            if (hairFrame < 7)
            {
                hairFrame = 7;
            }
            count = 7;
            if (++legFrameCounter > count)
            {
                legFrameCounter = 0;
                legFrame++;
            }
            if (legFrame > 3)
            {
                legFrame = 0;
            }
        }
    }
}


