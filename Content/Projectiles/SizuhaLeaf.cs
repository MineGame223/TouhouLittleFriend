using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.DataStructures;
using Terraria.Graphics.Shaders;

namespace TouhouPets.Content.Projectiles
{
    public class SizuhaLeaf : ModProjectile
    {
        public override void SetStaticDefaults()
        {
            Main.projFrames[Type] = 3;
        }
        public override void SetDefaults()
        {
            Projectile.width = 14;
            Projectile.height = 18;
            Projectile.friendly = true;
            Projectile.penetrate = -1;
            Projectile.tileCollide = false;
            Projectile.Opacity = 0;
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

            Main.spriteBatch.QuickEndAndBegin(true);

            DrawData data = new DrawData(tex, pos, rect, clr, Projectile.rotation, orig, 1f, SpriteEffects.None, 0);
            GameShaders.Armor.Apply(Main.player[Projectile.owner].cLight, Projectile, data);
            data.Draw(Main.spriteBatch);

            Main.spriteBatch.QuickEndAndBegin(false);
            return false;
        }
        public override void AI()
        {
            Projectile.frame = (int)Projectile.ai[0];
            Projectile.velocity.X += Main.windSpeedCurrent * 0.1f;
            Projectile.rotation += 0.05f * Projectile.direction;

            if (Projectile.ai[2] == 0)
            {
                Projectile.Opacity += 0.1f;
                if (Projectile.Opacity >= 1)
                {
                    Projectile.Opacity = 1;
                    Projectile.ai[2]++;
                }
            }
            else
            {
                if (++Projectile.localAI[0] > 40)
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
