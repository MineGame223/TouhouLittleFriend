using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.DataStructures;
using Terraria.Graphics.Shaders;

namespace TouhouPets.Content.Projectiles
{
    public class WriggleFirefly : ModProjectile
    {
        public override void SetStaticDefaults()
        {
            Main.projFrames[Type] = 2;
        }
        public override void SetDefaults()
        {
            Projectile.width = 8;
            Projectile.height = 8;
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
            int width = tex.Width / 4;
            int height = tex.Height / Main.projFrames[Type];
            Rectangle rect = new Rectangle((int)Projectile.ai[0] * width, Projectile.frame * height, width, height);
            Color clr = Projectile.GetAlpha(Color.White);
            Vector2 orig = rect.Size() / 2;

            Main.spriteBatch.QuickEndAndBegin(true);

            DrawData data = new DrawData(tex, pos, rect, clr, 0f, orig, 1f, SpriteEffects.None, 0);
            GameShaders.Armor.Apply(Main.player[Projectile.owner].cLight, Projectile, data);
            data.Draw(Main.spriteBatch);

            Main.spriteBatch.QuickEndAndBegin(false);

            return false;
        }
        public override void AI()
        {
            float r;
            float g;
            float b;
            switch (Projectile.ai[0])
            {
                case 1:
                    r = 0.65f;
                    g = 1.48f;
                    b = 0.44f;
                    break;
                case 2:
                    r = 0.44f;
                    g = 1.45f;
                    b = 1.48f;
                    break;
                case 3:
                    r = 1.48f;
                    g = 0.65f;
                    b = 0.44f;
                    break;
                default:
                    r = 1.48f;
                    g = 1.44f;
                    b = 0.44f;
                    break;
            }
            float brightness = Projectile.Opacity * (1 + Projectile.ai[1] * .5f);
            Lighting.AddLight(Projectile.Center, r * brightness, g * brightness, b * brightness);
            Projectile.frame = (int)Projectile.ai[1];
            if (Projectile.ai[2] == 0)
            {
                Projectile.Opacity += 0.2f;
                if (Projectile.Opacity >= 1)
                {
                    Projectile.Opacity = 1;
                    Projectile.ai[2]++;
                }
            }
            else
            {
                Projectile.velocity *= 0.995f;
                if (++Projectile.localAI[0] > 80)
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
