using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria;
using Terraria.ID;
using Terraria.Utilities;
using TouhouPets.Content.Buffs.PetBuffs;

namespace TouhouPets.Content.Projectiles.Pets
{
    public class Piece : BasicTouhouPetNeo
    {
        public override void SetStaticDefaults()
        {
            Main.projFrames[Type] = 13;
            Main.projPet[Type] = true;
            ProjectileID.Sets.LightPet[Type] = true;
        }
        DrawPetConfig drawConfig = new(1);
        readonly Texture2D clothTex = AltVanillaFunction.GetExtraTexture("Piece_Cloth");
        public override bool PreDraw(ref Color lightColor)
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
            Projectile.DrawStateNormalizeForPet();

            DrawTorch(lightColor);
            return false;
        }
        private void DrawTorch(Color lightColor)
        {
            Projectile.DrawPet(11, lightColor, drawConfig);

            Main.spriteBatch.QuickToggleAdditiveMode(true, Projectile.isAPreviewDummy);
            for (int i = 0; i < 7; i++)
            {
                Projectile.DrawPet(11, Color.White * 0.5f,
                    drawConfig with
                    {
                        PositionOffset = new Vector2(Main.rand.Next(-10, 11) * 0.25f, Main.rand.Next(-10, 11) * 0.25f),
                    });
            }
            Main.spriteBatch.QuickToggleAdditiveMode(false, Projectile.isAPreviewDummy);
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
        int blinkFrame, blinkFrameCounter;
        int wingFrame, wingFrameCounter;
        private void Idel()
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
        public override string GetRegularDialogText()
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
            Idel();
        }
        public override void AI()
        {
            Lighting.AddLight(Projectile.Center, 1.38f, 0.41f, 1.55f);
            if (Main.rand.NextBool(30))
            {
                Dust d = Dust.NewDustDirect(Projectile.Center + new Vector2(26 * Projectile.spriteDirection, 0).RotatedBy(Projectile.rotation), 4, 4, MyDustId.PurpleTorch, 0f, 0f, 100);
                if (!Main.rand.NextBool(3))
                    d.noGravity = true;
                d.velocity *= 0.3f;
                d.velocity.Y -= 1.5f;
            }
            Player player = Main.player[Projectile.owner];
            Projectile.SetPetActive(player, BuffType<HecatiaBuff>());
            UpdateTalking();
            int xPos = 55;
            foreach (Projectile otherPets in Main.projectile)
            {
                if (otherPets != null && otherPets.active && otherPets.owner == Projectile.owner)
                {
                    if ((Main.projPet[otherPets.type] || ProjectileID.Sets.LightPet[otherPets.type])
                        && otherPets.type != Projectile.type && otherPets.type != ProjectileType<Hecatia>())
                    {
                        Projectile p = otherPets;
                        if (Math.Abs(p.position.X - player.position.X) < 180 && Math.Abs(p.position.Y - player.position.Y) < 180)
                        {
                            xPos = -115;
                        }
                    }
                }
            }
            Vector2 point = new Vector2(xPos * player.direction, -40 + player.gfxOffY);
            Projectile.tileCollide = false;
            Projectile.rotation = Projectile.velocity.X * 0.028f;

            ChangeDir(150);
            MoveToPoint(point, 13f);

            if (Projectile.owner == Main.myPlayer)
            {
                if (mainTimer % 270 == 0)
                {
                    PetState = 1;
                    Projectile.netUpdate = true;
                }
            }
            if (PetState == 1)
            {
                Blink();
            }
        }
    }
}


