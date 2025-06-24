using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;

namespace TouhouPets.Content.Projectiles.Danmaku
{
    public class MokuFireball : ModProjectile
    {
        public override void SetStaticDefaults()
        {
            Main.projFrames[Type] = 4;
        }
        public override void SetDefaults()
        {
            Projectile.width = 40;
            Projectile.height = 40;
            Projectile.friendly = true;
            Projectile.Opacity = 0f;
            Projectile.ignoreWater = true;
            Projectile.tileCollide = false;
            Projectile.scale = 1.2f;
            Projectile.GetGlobalProjectile<TouhouPetGlobalProj>().isADanmaku = true;
            Projectile.GetGlobalProjectile<TouhouPetGlobalProj>().isDanmakuDestorible = false;
            Projectile.GetGlobalProjectile<TouhouPetGlobalProj>().belongsToPlayerB = true;
        }
        public override bool? CanCutTiles()
        {
            return false;
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
            Main.spriteBatch.MyDraw(tex, pos, rect, clr, Projectile.rotation, orig, Projectile.scale, SpriteEffects.None, 0);
            return false;
        }
        public override void AI()
        {
            Projectile.velocity *= 0.95f;
            Projectile.HandleDanmakuCollide();
            if (Projectile.localAI[0] < 240)
            {
                Projectile.Opacity = MathHelper.Clamp(Projectile.Opacity + 0.1f, 0, 1);
            }
            else
            {
                Projectile.Opacity -= 0.12f;
                if (Projectile.Opacity <= 0f)
                {
                    Projectile.active = false;
                    Projectile.netUpdate = true;
                    return;
                }
            }
            Projectile.localAI[0] += 1f;
            if (Main.rand.NextBool(3))
            {
                for (int i = 0; i < 2; i++)
                {
                    int d = Dust.NewDust(new Vector2(Projectile.position.X, Projectile.position.Y), Projectile.width, Projectile.height, MyDustId.Fire, Projectile.velocity.X, Projectile.velocity.Y, 50, default(Color), 1.2f);
                    Main.dust[d].noGravity = true;
                    Dust dust = Main.dust[d];
                    dust.velocity *= 0.3f;
                }
            }
            if (++Projectile.frameCounter > 5)
            {
                Projectile.frameCounter = 0;
                Projectile.frame++;
            }
            if (Projectile.frame > 3)
            {
                Projectile.frame = 0;
            }
        }
        public override void OnKill(int timeLeft)
        {
            for (int i = 0; i < 11; i++)
            {
                int d = Dust.NewDust(new Vector2(Projectile.position.X, Projectile.position.Y), Projectile.width, Projectile.height, MyDustId.Fire, Projectile.oldVelocity.X, Projectile.oldVelocity.Y, 50, default(Color), 1.2f);
                Main.dust[d].noGravity = true;
                Dust dust = Main.dust[d];
                dust.scale *= 1.45f;
                dust = Main.dust[d];
                dust.velocity *= 0.5f;
            }
        }
    }
}

