using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ID;
using TouhouPets.Content.Projectiles.Pets;

namespace TouhouPets.Content.Projectiles
{
    public class SakuyaKnife : ModProjectile
    {
        public override void SetDefaults()
        {
            Projectile.width = 6;
            Projectile.height = 6;
            Projectile.aiStyle = -1;
            Projectile.friendly = true;
            Projectile.alpha = 255;
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
        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D tex = AltVanillaFunction.ProjectileTexture(Type);
            Vector2 pos = Projectile.Center - Main.screenPosition;
            int height = tex.Height / Main.projFrames[Type];
            Rectangle rect = new Rectangle(0, Projectile.frame * height, tex.Width, height);
            Color clr = Projectile.GetAlpha(Color.White);
            Vector2 orig = rect.Size() / 2;
            Main.spriteBatch.TeaNPCDraw(tex, pos, rect, clr, Projectile.rotation, orig, Projectile.scale, SpriteEffects.None, 0);
            return false;
        }
        public override void AI()
        {
            if (Projectile.localAI[0] < 600)
            {
                if (Projectile.alpha > 10)
                {
                    Projectile.alpha -= 40;
                }
            }
            else
            {
                Projectile.alpha += 5;
                if (Projectile.alpha > 255)
                {
                    Projectile.active = false;
                    Projectile.netUpdate = true;
                    return;
                }
            }
            foreach (Projectile t in Main.projectile)
            {
                if (t != null && t.active)
                {
                    if (t.type == ProjectileType<Meirin>() && t.owner == Projectile.owner
                        && t.Hitbox.Intersects(Projectile.Hitbox))
                    {
                        Projectile.timeLeft = 0;
                        Projectile.netUpdate = true;
                        t.ToPetClass().PetState = 7;
                        t.netUpdate = true;
                        return;
                    }
                }
            }
            Projectile.rotation = Projectile.velocity.ToRotation() - MathHelper.PiOver2;
            Projectile.localAI[0] += 1f;
        }
        public override void OnKill(int timeLeft)
        {
            AltVanillaFunction.PlaySound(SoundID.Dig, Projectile.position);
            for (int i = 0; i < 3; i++)
            {
                int d = Dust.NewDust(new Vector2(Projectile.position.X, Projectile.position.Y), Projectile.width, Projectile.height, MyDustId.BlueMagic, Projectile.oldVelocity.X, Projectile.oldVelocity.Y, 100, default(Color), 1.8f);
                Main.dust[d].noGravity = true;
                Dust dust = Main.dust[d];
                dust.scale *= 1.45f;
                dust = Main.dust[d];
                dust.velocity *= 0.5f;
            }
        }
    }
}

