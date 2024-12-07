using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.DataStructures;
using Terraria.Graphics.Shaders;

namespace TouhouPets.Content.Projectiles
{
    public class YuyukoButterfly : ModProjectile
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
            int height = tex.Height / Main.projFrames[Type];
            Rectangle rect = new Rectangle(0, Projectile.frame * height, tex.Width, height);
            Color clr = Projectile.GetAlpha(Color.White);
            Vector2 orig = rect.Size() / 2;
            Player player = Main.player[Projectile.owner];

            DrawData data = new DrawData(tex, pos, rect, clr, 0f, orig, 1f, SpriteEffects.None, 0);

            if (GetInstance<MiscConfig>().CompatibilityMode)
            {
                data.Draw(Main.spriteBatch);
                return false;
            }

            Main.spriteBatch.QuickEndAndBegin(true);

            GameShaders.Armor.Apply(player.cPet, Projectile, data);
            data.Draw(Main.spriteBatch);

            Main.spriteBatch.QuickEndAndBegin(false);
            return false;
        }
        public override void AI()
        {
            if (++Projectile.frameCounter > 4)
            {
                Projectile.frameCounter = 0;
                Projectile.frame++;
            }
            if (Projectile.frame > 1)
            {
                Projectile.frame = 0;
            }

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
                if (++Projectile.localAI[0] > 30)
                    Projectile.Opacity -= 0.1f;

                if (Projectile.Opacity <= 0)
                {
                    Projectile.active = false;
                    Projectile.netUpdate = true;
                }
            }

            float r = 1.47f;
            float g = 0.45f;
            float b = 1.03f;
            float brightness = Projectile.Opacity;
            Lighting.AddLight(Projectile.Center, r * brightness, g * brightness, b * brightness);
        }
    }
}
