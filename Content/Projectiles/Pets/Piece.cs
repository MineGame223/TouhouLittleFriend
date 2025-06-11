using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria;
using Terraria.ID;
using Terraria.Utilities;
using TouhouPets.Content.Buffs.PetBuffs;

namespace TouhouPets.Content.Projectiles.Pets
{
    public class Piece : BasicTouhouPet
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

        private int blinkFrame, blinkFrameCounter;
        private int wingFrame, wingFrameCounter;

        private DrawPetConfig drawConfig = new(1);
        private readonly Texture2D clothTex = AltVanillaFunction.GetExtraTexture("Piece_Cloth");
        public override void SetStaticDefaults()
        {
            Main.projFrames[Type] = 13;
            Main.projPet[Type] = true;
            ProjectileID.Sets.LightPet[Type] = true;
        }
        public override TouhouPetID UniqueID => TouhouPetID.Piece;
        public override bool DrawPetSelf(ref Color lightColor)
        {
            Projectile.DrawPet(wingFrame, lightColor * 0.7f, drawConfig);

            Projectile.DrawPet(Projectile.frame, lightColor, drawConfig);

            if (PetState == 1)
                Projectile.DrawPet(blinkFrame, lightColor, drawConfig);

            Projectile.DrawPet(Projectile.frame, lightColor,
                drawConfig with
                {
                    AltTexture = clothTex,
                    ShouldUseEntitySpriteDraw = true,
                });
            Projectile.ResetDrawStateForPet();

            DrawTorch(lightColor);
            return false;
        }
        private void DrawTorch(Color lightColor)
        {
            Projectile.DrawPet(11, lightColor, drawConfig);

            Color clr = (Color.White * 0.2f).ModifiedAlphaColor();
            for (int i = 0; i < 7; i++)
            {
                Projectile.DrawPet(11, clr,
                    drawConfig with
                    {
                        PositionOffset = new Vector2(Main.rand.Next(-10, 11) * 0.25f, Main.rand.Next(-10, 11) * 0.25f),
                    });
            }
        }
        public override Color ChatTextColor => new Color(255, 119, 187);
        public override void RegisterChat(ref string name, ref Vector2 indexRange)
        {
            name = "Piece";
            indexRange = new Vector2(1, 3);
        }
        public override void SetRegularDialog(ref int timePerDialog, ref int chance, ref bool whenShouldStop)
        {
            timePerDialog = 740;
            chance = 5;
            whenShouldStop = false;
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
        private void UpdateTalking()
        {
        }
        public override void VisualEffectForPreview()
        {
            UpdateWingFrame();
            IdleAnimation();
        }
        public override void SetPetLight(ref Vector2 position, ref Vector3 rgb, ref bool inactive)
        {
            rgb = new Vector3(1.38f, 0.41f, 1.55f);
        }
        public override void AI()
        {
            Projectile.SetPetActive(Owner, BuffType<HecatiaBuff>());

            UpdateTalking();

            ControlMovement();

            switch (CurrentState)
            {
                case States.Blink:
                    Blink();
                    break;

                default:
                    Idle();
                    break;
            }

            if (ShouldExtraVFXActive)
                TorchDust();
        }
        private void ControlMovement()
        {
            Projectile.tileCollide = false;
            Projectile.rotation = Projectile.velocity.X * 0.028f;

            ChangeDir(150);

            int xPos = 55;
            foreach (Projectile otherPets in Main.projectile)
            {
                if (otherPets != null && otherPets.active && otherPets.owner == Projectile.owner)
                {
                    if ((Main.projPet[otherPets.type] || ProjectileID.Sets.LightPet[otherPets.type])
                        && otherPets.type != Projectile.type && otherPets.type != ProjectileType<Hecatia>())
                    {
                        Projectile p = otherPets;
                        if (Math.Abs(p.position.X - Owner.position.X) < 180 && Math.Abs(p.position.Y - Owner.position.Y) < 180)
                        {
                            xPos = -115;
                        }
                    }
                }
            }
            Vector2 point = new Vector2(xPos * Owner.direction, -40 + Owner.gfxOffY);
            MoveToPoint(point, 13f);
        }
        private void TorchDust()
        {
            if (Main.rand.NextBool(30))
            {
                Dust d = Dust.NewDustDirect(Projectile.Center + new Vector2(26 * Projectile.spriteDirection, 0).RotatedBy(Projectile.rotation), 4, 4, MyDustId.PurpleTorch, 0f, 0f, 100);
                if (!Main.rand.NextBool(3))
                    d.noGravity = true;
                d.velocity *= 0.3f;
                d.velocity.Y -= 1.5f;
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
            if (++wingFrameCounter > 3)
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


