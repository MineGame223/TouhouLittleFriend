using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ID;
using Terraria.Utilities;
using TouhouPets.Content.Buffs.PetBuffs;

namespace TouhouPets.Content.Projectiles.Pets
{
    public class Lily : BasicTouhouPet
    {
        private enum States
        {
            Idle,
            Blink,
        }
        private States CurrentState
        {
            get => (States)PetState;
            set => PetState = (int)value;
        }
        public override void SetStaticDefaults()
        {
            Main.projFrames[Type] = 11;
            Main.projPet[Type] = true;
            ProjectileID.Sets.LightPet[Type] = true;
        }

        private int wingFrame, wingFrameCounter;
        private int blinkFrame, blinkFrameCounter;
        private bool useDye, blackDye;

        private DrawPetConfig drawConfig = new(1);
        private readonly Texture2D clothTex = AltVanillaFunction.GetExtraTexture("Lily_Cloth");
        private readonly Texture2D clothTexAlt = AltVanillaFunction.GetExtraTexture("Lily_Cloth_Alt");
        public override bool DrawPetSelf(ref Color lightColor)
        {
            Projectile.DrawPet(wingFrame, Color.White * 0.7f, drawConfig);

            Projectile.DrawPet(Projectile.frame, lightColor, drawConfig);

            if (CurrentState == States.Blink)
            {
                Projectile.DrawPet(blinkFrame, lightColor, drawConfig);
            }

            Texture2D tex = clothTex;
            int clothFrame = Projectile.frame;
            if (useDye)
            {
                if (blackDye)
                    clothFrame += 4;
                tex = clothTexAlt;
            }
            Projectile.DrawPet(clothFrame, lightColor,
                drawConfig with
                {
                    AltTexture = tex,
                    ShouldUseEntitySpriteDraw = true,
                });
            return false;
        }
        public override Color ChatTextColor => Color.White;
        public override void RegisterChat(ref string name, ref Vector2 indexRange)
        {
            name = "Lily";
            indexRange = new Vector2(1, 4);
        }
        public override void SetRegularDialog(ref int timePerDialog, ref int chance, ref bool whenShouldStop)
        {
            timePerDialog = 720;
            chance = 9;
            whenShouldStop = false;
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
        private void UpdateTalking()
        {
        }
        public override void VisualEffectForPreview()
        {
            IdleAnimation();
            UpdateWingFrame();
        }
        public override void SetPetLight(ref Vector2 position, ref Vector3 rgb, ref bool inactive)
        {
            rgb = new Vector3(1.62f, 0.98f, 1.32f);
        }
        public override void AI()
        {
            Projectile.SetPetActive(Owner, BuffType<LilyBuff>());

            UpdateTalking();

            ControlMovement();

            GenDust();

            switch (CurrentState)
            {
                case States.Blink:
                    Blink();
                    break;

                default:
                    Idle();
                    break;
            }
            useDye = !Owner.miscDyes[1].IsAir;
            blackDye = Owner.miscDyes[1].type == ItemID.BlackDye;
        }
        private void ControlMovement()
        {
            Projectile.tileCollide = false;
            Projectile.rotation = Projectile.velocity.X * 0.026f;

            ChangeDir();

            Vector2 point = new Vector2(40 * Owner.direction, -30 + Owner.gfxOffY);
            MoveToPoint(point, 10.5f);
        }
        private void GenDust()
        {
            int dustID = MyDustId.WhiteTransparent;
            if (Main.rand.NextBool(15))
            {
                Dust d = Dust.NewDustPerfect(Projectile.Center
                + new Vector2(Main.rand.NextFloat(-30f, -10f) * Projectile.spriteDirection, Main.rand.NextFloat(-5f, 25f)), dustID
                , new Vector2(Main.rand.NextFloat(-0.2f, 0.2f), Main.rand.NextFloat(0.8f, 1.2f)), 100, default
                , Main.rand.NextFloat(1f, 1.2f));
                d.noGravity = true;
                //d.shader = GameShaders.Armor.GetSecondaryShader(Owner.cLight, Owner);
            }
            if (Main.rand.NextBool(15))
            {
                Dust d = Dust.NewDustPerfect(Projectile.Center
                    + new Vector2(Main.rand.NextFloat(20f, 22f) * Projectile.spriteDirection, Main.rand.NextFloat(-5f, 25f)), dustID
                    , new Vector2(Main.rand.NextFloat(-0.2f, 0.2f), Main.rand.NextFloat(0.8f, 1.2f)), 100, default
                    , Main.rand.NextFloat(1f, 1.2f));
                d.noGravity = true;
                //d.shader = GameShaders.Armor.GetSecondaryShader(Owner.cLight, Owner);
            }
        }
        private void Idle()
        {
            if (OwnerIsMyPlayer && mainTimer % 270 == 0)
            {
                CurrentState = States.Blink;
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
        private void UpdateWingFrame()
        {
            if (wingFrame < 4)
            {
                wingFrame = 4;
            }
            if (++wingFrameCounter > 4)
            {
                wingFrameCounter = 0;
                wingFrame++;
            }
            if (wingFrame > 7)
            {
                wingFrame = 4;
            }
        }
    }
}


