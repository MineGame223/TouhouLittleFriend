using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;

namespace TouhouPets.Content.Projectiles
{
    public class WakasagihimeBubble : ModProjectile
    {
        public override void SetStaticDefaults()
        {
            Main.projFrames[Type] = 3;
        }
        public override void SetDefaults()
        {
            Projectile.width = 10;
            Projectile.height = 10;
            Projectile.friendly = true;
            Projectile.penetrate = -1;
            Projectile.tileCollide = false;
            Projectile.scale = 0.1f;
        }
        public override bool? CanDamage()
        {
            return false;
        }
        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D tex = AltVanillaFunction.ProjectileTexture(Type);
            Vector2 pos = Projectile.Center - Main.screenPosition;
            int height = tex.Height / Main.projFrames[Type];
            Rectangle rect = new Rectangle(0, Projectile.frame * height, tex.Width, height);
            Color clr = Projectile.GetAlpha(lightColor);
            Vector2 orig = rect.Size() / 2;
            Main.spriteBatch.TeaNPCDraw(tex, pos, rect, clr, 0f, orig, 1f, SpriteEffects.None, 0);
            return false;
        }
        public override void AI()
        {
            Projectile.velocity.X *= 0.995f;
            if (Projectile.velocity.Y > -4f)
                Projectile.velocity.Y -= 0.03f;
            Projectile.frame = (int)Projectile.ai[0];
            if (Projectile.ai[2] == 0)
            {
                Projectile.scale += 0.2f;
                if (Projectile.scale >= Projectile.ai[1])
                {
                    Projectile.scale = Projectile.ai[1];
                    Projectile.ai[2]++;
                }
            }
            else
            {
                if (++Projectile.localAI[0] > 30)
                    Projectile.Opacity -= 0.1f;
                if (Projectile.Opacity <= 0)
                {
                    Projectile.active = false;
                    Projectile.netUpdate = true;
                }
            }
        }
    }
}
