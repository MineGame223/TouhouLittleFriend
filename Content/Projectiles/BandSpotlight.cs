using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria;
using Terraria.DataStructures;
using Terraria.GameContent.Drawing;

namespace TouhouPets.Content.Projectiles
{
    public class BandSpotlight : ModProjectile
    {
        public override void SetDefaults()
        {
            Projectile.width = Projectile.height = 22;
            Projectile.aiStyle = -1;
            Projectile.friendly = true;
            Projectile.Opacity = 0;
            Projectile.ignoreWater = true;
            Projectile.tileCollide = false;
        }
        public override bool? CanCutTiles()
        {
            return false;
        }
        public override bool? CanDamage()
        {
            return false;
        }
        private Color GetRayColor()
        {
            return Projectile.ai[0] switch
            {
                1 => Color.Cyan,
                2 => Color.Yellow,
                3 => Color.Lime,
                4 => Color.Red,
                5 => Color.HotPink,
                _ => Color.White,
            };
        }
        public override bool PreDraw(ref Color lightColor)
        {
            Main.spriteBatch.QuickEndAndBegin(true, false, BlendState.Additive);

            Texture2D tex = AltVanillaFunction.ProjectileTexture(Type);
            Vector2 pos = Projectile.Center - Main.screenPosition;
            Color clr = Projectile.GetAlpha(GetRayColor());
            Vector2 orig = new Vector2(tex.Width / 2, 0);
            Main.spriteBatch.TeaNPCDraw(tex, pos, null, clr, Projectile.rotation, orig, Projectile.scale, SpriteEffects.None, 0);

            Main.spriteBatch.QuickEndAndBegin(false);
            return false;
        }
        public override void OnSpawn(IEntitySource source)
        {
            Projectile.rotation = Projectile.ai[1];
            if (Projectile.ai[1] < 0)
            {
                Projectile.spriteDirection = -1;
            }
            else
            {
                Projectile.spriteDirection = 1;
            }
        }
        public override void AI()
        {
            Projectile.rotation = Projectile.localAI[0] + MathHelper.ToRadians(15) * Projectile.spriteDirection;
            if (Projectile.ai[1] < 0)
            {
                Projectile.localAI[0] += MathHelper.ToRadians(1);
            }
            else
            {
                Projectile.localAI[0] -= MathHelper.ToRadians(1);
            }

            if (Projectile.ai[2] == 0)
            {
                if (Projectile.Opacity < 1)
                {
                    Projectile.Opacity += 0.1f;
                }
                if (Math.Abs(Projectile.localAI[0]) > MathHelper.ToRadians(30))
                {
                    Projectile.ai[2]++;
                    Projectile.netUpdate = true;
                }
            }
            else
            {
                Projectile.Opacity -= 0.1f;
                if (Projectile.Opacity <= 0)
                {
                    Projectile.timeLeft = 0;
                    Projectile.netUpdate = true;
                }
            }
        }
    }
}
