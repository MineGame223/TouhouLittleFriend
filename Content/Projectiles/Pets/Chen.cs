using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ID;
using Terraria.Utilities;
using TouhouPets.Content.Buffs.PetBuffs;

namespace TouhouPets.Content.Projectiles.Pets
{
    public class Chen : BasicTouhouPet
    {
        private enum States
        {
            Idle,
            Blink,
            Meow,
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
        private bool IsIdleState => CurrentState <= States.Blink;

        private int tailFrame, tailFrameCounter;
        private int blinkFrame, blinkFrameCounter;
        private int clothFrame, clothFrameCounter;

        private DrawPetConfig drawConfig = new(2);
        private readonly Texture2D clothTex = AltVanillaFunction.GetExtraTexture("Chen_Cloth");
        public override void SetStaticDefaults()
        {
            Main.projFrames[Type] = 11;
            Main.projPet[Type] = true;
        }
        public override TouhouPetID UniqueID => TouhouPetID.Chen;
        public override bool DrawPetSelf(ref Color lightColor)
        {
            Projectile.DrawPet(tailFrame, lightColor, drawConfig, 1);

            Projectile.DrawPet(Projectile.frame, lightColor, drawConfig);

            if (CurrentState == States.Blink)
                Projectile.DrawPet(blinkFrame, lightColor, drawConfig, 1);

            Projectile.DrawPet(Projectile.frame, lightColor,
                drawConfig with
                {
                    AltTexture = clothTex,
                    ShouldUseEntitySpriteDraw = true,
                });
            Projectile.DrawPet(clothFrame, lightColor,
                drawConfig with
                {
                    ShouldUseEntitySpriteDraw = true,
                }, 1);
            return false;
        }
        public override ChatSettingConfig ChatSettingConfig => new ChatSettingConfig() with
        {
            TextColor = new Color(89, 196, 108),
        };
        public override void RegisterChat(ref string name, ref Vector2 indexRange)
        {
            name = "Chen";
            indexRange = new Vector2(1, 3);
        }
        public override void SetRegularDialog(ref int timePerDialog, ref int chance, ref bool whenShouldStop)
        {
            timePerDialog = 640;
            chance = 7;
            whenShouldStop = !IsIdleState;
        }
        public override WeightedRandom<string> RegularDialogText()
        {
            WeightedRandom<string> chat = new WeightedRandom<string>();
            {
                chat.Add(ChatDictionary[1]);
                chat.Add(ChatDictionary[2]);
            }
            return chat;
        }
        public override void VisualEffectForPreview()
        {
            UpdateTailFrame();
            UpdateClothFrame();
            if (IsIdleState)
                IdleAnimation();
        }
        public override void AI()
        {
            Projectile.SetPetActive(Owner, BuffType<YukariBuff>());

            ControlMovement();

            switch (CurrentState)
            {
                case States.Blink:
                    Blink();
                    break;

                case States.Meow:
                    shouldNotTalking = true;
                    Meow();
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
            Projectile.rotation = Projectile.velocity.X * 0.02f;

            ChangeDir(130);

            Vector2 point = new Vector2(-120 * Owner.direction, -40 + Owner.gfxOffY);
            MoveToPoint(point, 17f);
        }
        private void Idle()
        {
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
                        CurrentState = States.Meow;

                        if (Main.rand.NextBool(2) && chatTimeLeft <= 0)
                            Projectile.SetChat(ChatSettingConfig, 1, 20);
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
                PetState = 0;
            }
        }
        private void Meow()
        {
            int count = 7;
            if (Projectile.frame == 6)
                count = 18;

            if (++Projectile.frameCounter > count)
            {
                Projectile.frameCounter = 0;
                Projectile.frame++;
            }
            if (Projectile.frame == 6 && Projectile.frameCounter == 0)
            {
                AltVanillaFunction.PlaySound(SoundID.Meowmere, Projectile.Center);
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
        private void IdleAnimation()
        {
            if (++Projectile.frameCounter > 6)
            {
                Projectile.frameCounter = 0;
                Projectile.frame++;
            }
            if (Projectile.frame > 3)
            {
                Projectile.frame = 0;
            }
        }
        private void UpdateTailFrame()
        {
            if (tailFrame < 4)
            {
                tailFrame = 4;
            }
            int count = 6;
            if (++tailFrameCounter > count)
            {
                tailFrameCounter = 0;
                tailFrame++;
            }
            if (tailFrame > 7)
            {
                tailFrame = 4;
            }
        }
        private void UpdateClothFrame()
        {
            int count = 5;
            if (++clothFrameCounter > count)
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


