using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.DataStructures;
using Terraria.Graphics.Shaders;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace TouhouPets.Content.Projectiles
{
    public class MokuFlame : ModProjectile
    {
        public override void SetStaticDefaults()
        {
            Main.projFrames[Type] = 3;
        }
        public override void SetDefaults()
        {
            Projectile.width = 14;
            Projectile.height = 14;
            Projectile.friendly = true;
            Projectile.penetrate = -1;
            Projectile.tileCollide = false;
            Projectile.Opacity = 0;
        }
        public override bool? CanDamage()
        {
            return false;
        }
        private void DrawFlame(DrawData data, Texture2D tex, int height)
        {
            for (int i = 0; i < 3; i++)
            {
                data.sourceRect = new Rectangle(0, Main.rand.Next(3) * height, tex.Width, height);
                data.position += new Vector2(Main.rand.NextFloat(-1.2f, 1.2f), Main.rand.NextFloat(-1.2f, 1.2f));
                data.Draw(Main.spriteBatch);
            }
        }
        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D tex = AltVanillaFunction.ProjectileTexture(Type);
            Vector2 pos = Projectile.Center - Main.screenPosition;
            int height = tex.Height / Main.projFrames[Type];
            Rectangle rect = new Rectangle(0, Projectile.frame * height, tex.Width, height);
            Color clr = Projectile.GetAlpha(Color.White) * 0.5f;
            clr.A *= 0;
            Vector2 orig = rect.Size() / 2;

            Player player = Main.player[Projectile.owner];
            DrawData data = new DrawData(tex, pos, rect, clr, 0f, orig, Projectile.scale, SpriteEffects.None, 0);

            if (GetInstance<MiscConfig>().CompatibilityMode)
            {
                DrawFlame(data, tex, height);
                return false;
            }

            Main.spriteBatch.QuickEndAndBegin(true);

            GameShaders.Armor.Apply(player.cLight, Projectile, data);
            DrawFlame(data, tex, height);

            Main.spriteBatch.QuickEndAndBegin(false);
            return false;
        }
        public override void AI()
        {
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
                if (++Projectile.localAI[0] > 20)
                    Projectile.scale -= 0.02f;
                else
                {
                    if (Main.rand.NextBool(7))
                    {
                        Dust.NewDustPerfect(Projectile.position + new Vector2(Main.rand.Next(0, Projectile.width)
                        , Main.rand.Next(0, Projectile.height)), MyDustId.Fire
                    , new Vector2(Main.rand.NextFloat(-1f, 1f), Main.rand.NextFloat(-1f, 1f)), 100, default
                    , Main.rand.NextFloat(1f, 2f)).noGravity = true;
                    }
                }

                if (Projectile.scale <= 0)
                {
                    Projectile.active = false;
                    Projectile.netUpdate = true;
                }
            }

            float r = 1.79f;
            float g = 1.11f;
            float b = 0.55f;
            float brightness = Projectile.Opacity;
            Lighting.AddLight(Projectile.Center, r * brightness, g * brightness, b * brightness);
        }
    }
}
