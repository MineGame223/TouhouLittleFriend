using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ID;
using Terraria.Localization;
using Terraria.Utilities;
using TouhouPets.Content.Buffs.PetBuffs;

namespace TouhouPets.Content.Projectiles.Pets
{
    public class Minoriko : BasicTouhouPet
    {
        private enum States
        {
            Idle,
            Blink,
            Cold,
            ColdBlink,
        }
        private States CurrentState
        {
            get => (States)PetState;
            set => PetState = (int)value;
        }
        private bool IsIdleState => PetState <= 1;
        private bool FeelCold => Owner.ZoneSnow || Owner.GetModPlayer<TouhouPetPlayer>().lettyCold;

        private int blinkFrame, blinkFrameCounter;
        private int clothFrame, clothFrameCounter;

        private DrawPetConfig drawConfig = new(1);
        private readonly Texture2D clothTex = AltVanillaFunction.GetExtraTexture("Minoriko_Cloth");
        public override void PetStaticDefaults()
        {
            Main.projFrames[Type] = 12;
            Main.projPet[Type] = true;

            ProjectileID.Sets.CharacterPreviewAnimations[Type] =
                ProjectileID.Sets.SimpleLoop(0, 4, 8);
        }
        public override TouhouPetID UniqueID => TouhouPetID.Minoriko;
        public override bool DrawPetSelf(ref Color lightColor)
        {
            DrawPetConfig config = drawConfig with
            {
                ShouldUseEntitySpriteDraw = true,
            };

            Projectile.DrawPet(Projectile.frame, lightColor, drawConfig);

            if (CurrentState == States.Blink || CurrentState == States.ColdBlink)
                Projectile.DrawPet(blinkFrame, lightColor, drawConfig);

            Projectile.DrawPet(clothFrame, lightColor, config);
            Projectile.DrawPet(Projectile.frame, lightColor,
                config with
                {
                    AltTexture = clothTex,
                });
            return false;
        }
        public override ChatSettingConfig ChatSettingConfig => new ChatSettingConfig() with
        {
            TextColor = new Color(244, 150, 91),
        };
        public override void RegisterChat(ref string name, ref Vector2 indexRange)
        {
            name = "Minoriko";
            indexRange = new Vector2(1, 8);
        }
        public override void SetRegularDialog(ref int timePerDialog, ref int chance, ref bool whenShouldStop)
        {
            timePerDialog = 640;
            chance = 8;
            whenShouldStop = false;
        }
        public override WeightedRandom<LocalizedText> RegularDialogText()
        {
            WeightedRandom<LocalizedText> chat = new ();
            {
                if (!IsIdleState)
                {
                    chat.Add(ChatDictionary[8]);
                }
                else
                {
                    chat.Add(ChatDictionary[1]);
                    chat.Add(ChatDictionary[2]);
                    chat.Add(ChatDictionary[3]);
                    chat.Add(ChatDictionary[4]);
                    chat.Add(ChatDictionary[5]);
                }
            }
            return chat;
        }
        public override void VisualEffectForPreview()
        {
            UpdateClothFrame();
        }
        public override void SetPetLight(ref Vector2 position, ref Vector3 rgb, ref bool inactive)
        {
            rgb = new Vector3(0.54f, 0.34f, 0.34f);
            inactive = !IsIdleState;
        }
        public override void AI()
        {
            Projectile.SetPetActive(Owner, BuffType<MinorikoBuff>());

            ControlMovement();

            switch (CurrentState)
            {
                case States.Blink:
                    Blink();
                    break;

                case States.Cold:
                    shouldNotTalking = true;
                    Cold();
                    break;

                case States.ColdBlink:
                    shouldNotTalking = true;
                    ColdBlink();
                    break;

                default:
                    Idle();
                    break;
            }
            if (IsIdleState)
            {
                IdleAnimation();
            }
        }
        private void ControlMovement()
        {
            Projectile.tileCollide = false;
            Projectile.rotation = Projectile.velocity.X * 0.02f;

            ChangeDir();

            Vector2 point = new Vector2(-50 * Owner.direction, -30 + Owner.gfxOffY);
            MoveToPoint(point, 10f);
        }
        private void Idle()
        {
            if (OwnerIsMyPlayer)
            {
                if (FeelCold)
                {
                    CurrentState = States.Cold;
                }
                if (mainTimer % 270 == 0)
                {
                    CurrentState = States.Blink;
                }
            }
        }
        private void Blink()
        {
            int startFrame = 8;
            if (blinkFrame < startFrame)
            {
                blinkFrame = startFrame;
            }
            if (++blinkFrameCounter > 3)
            {
                blinkFrameCounter = 0;
                blinkFrame++;
            }
            if (blinkFrame > 10)
            {
                blinkFrame = startFrame;
                CurrentState = States.Idle;
            }
        }
        private void Cold()
        {
            Projectile.frame = 11;
            if (OwnerIsMyPlayer)
            {
                if (!FeelCold)
                {
                    CurrentState = States.Idle;
                }
                if (mainTimer % 270 == 0)
                {
                    CurrentState = States.ColdBlink;
                }
            }
        }
        private void ColdBlink()
        {
            Projectile.frame = 11;
            int startFrame = 9;
            if (blinkFrame < startFrame)
            {
                blinkFrame = startFrame;
            }
            if (++blinkFrameCounter > 3)
            {
                blinkFrameCounter = 0;
                blinkFrame++;
            }
            if (blinkFrame > 10)
            {
                blinkFrame = startFrame;
                CurrentState = States.Cold;
            }
        }
        private void IdleAnimation()
        {
            if (++Projectile.frameCounter > 8)
            {
                Projectile.frameCounter = 0;
                Projectile.frame++;
            }
            if (Projectile.frame > 3)
            {
                Projectile.frame = 0;
            }
        }
        private void UpdateClothFrame()
        {
            if (clothFrame < 4)
            {
                clothFrame = 4;
            }
            if (++clothFrameCounter > 6)
            {
                clothFrameCounter = 0;
                clothFrame++;
            }
            if (clothFrame > 7)
            {
                clothFrame = 4;
            }
        }
    }
}


