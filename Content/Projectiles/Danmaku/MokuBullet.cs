using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;
using Terraria;
using Terraria.ID;

namespace TouhouPets.Content.Projectiles.Danmaku
{
    public class MokuBullet : ModProjectile
    {
        public override void SetStaticDefaults()
        {
            Main.projFrames[Type] = 6;
        }
        public override void SetDefaults()
        {
            Projectile.width = 16;
            Projectile.height = 16;
            Projectile.aiStyle = -1;
            Projectile.friendly = true;
            Projectile.alpha = 255;
            Projectile.ignoreWater = true;
            Projectile.tileCollide = false;
            Projectile.timeLeft = 360;
            Projectile.extraUpdates = 1;
            Projectile.GetGlobalProjectile<TouhouPetGlobalProj>().isADanmaku = true;
            Projectile.GetGlobalProjectile<TouhouPetGlobalProj>().isDanmakuDestorible = true;
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
            Projectile.HandleDanmakuCollide();
            if (Projectile.localAI[0] < 90)
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
            Projectile.rotation = Projectile.velocity.ToRotation() - MathHelper.PiOver2;
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
            if (Projectile.frame > 5)
            {
                Projectile.frame = 0;
            }
        }
        public override void OnKill(int timeLeft)
        {
            AltVanillaFunction.PlaySound(SoundID.Dig, Projectile.position);
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

