using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.GameContent.Drawing;

namespace TouhouPets.Content.Projectiles.Danmaku
{
    public class KaguyaWave : ModProjectile
    {
        public override void SetStaticDefaults()
        {
            Main.projFrames[Type] = 1;
        }
        public override void SetDefaults()
        {
            Projectile.width = 128;
            Projectile.height = 128;
            Projectile.aiStyle = -1;
            Projectile.friendly = true;
            Projectile.Opacity = 0f;
            Projectile.ignoreWater = true;
            Projectile.tileCollide = false;
            Projectile.timeLeft = 360;
            Projectile.GetGlobalProjectile<TouhouPetGlobalProj>().isADanmaku = true;
            Projectile.GetGlobalProjectile<TouhouPetGlobalProj>().isDanmakuDestorible = false;
            Projectile.GetGlobalProjectile<TouhouPetGlobalProj>().belongsToPlayerA = true;
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
            Color clr = Projectile.GetAlpha(lightColor) * 0.8f;
            Vector2 orig = rect.Size() / 2;
            Main.spriteBatch.MyDraw(tex, pos, rect, clr, Projectile.rotation, orig, Projectile.scale, SpriteEffects.None, 0);
            return false;
        }
        public override void AI()
        {
            Projectile.HandleDanmakuCollide();

            Projectile.position += new Vector2(Projectile.width, Projectile.height) / 2f;
            Projectile.width = Projectile.height = (int)(128 * Projectile.scale);
            Projectile.position -= new Vector2(Projectile.width, Projectile.height) / 2f;

            if (Projectile.localAI[0] < 60)
            {
                if (Projectile.Opacity < 1)
                {
                    Projectile.Opacity += 0.1f;
                }
            }
            else
            {
                Projectile.Opacity -= 0.05f;
                if (Projectile.Opacity <= 0.1f)
                {
                    Projectile.active = false;
                    Projectile.netUpdate = true;
                    return;
                }
            }
            Projectile.scale = Projectile.localAI[0] / 20;
            Projectile.rotation -= 1.104719758f;
            Projectile.localAI[0] += 1f;
            if (Main.rand.NextBool(3))
            {
                int dustType = Main.rand.Next(5) switch
                {
                    1 => MyDustId.RedTrans,
                    2 => MyDustId.BlueTrans,
                    3 => MyDustId.GreenTrans,
                    4 => MyDustId.TransparentPurple,
                    _ => MyDustId.YellowTrans,
                };
                int d = Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, dustType, Projectile.velocity.X, Projectile.velocity.Y, 50, default(Color), 1.2f);
                Main.dust[d].noGravity = true;
                Dust dust = Main.dust[d];
                dust.velocity *= 0.3f;

                if (Main.rand.NextBool(2))
                {
                    ParticleOrchestraSettings settings = new ParticleOrchestraSettings
                    {
                        PositionInWorld = Projectile.Center + new Vector2(Main.rand.Next(0, Projectile.height / 2), 0).RotatedByRandom(MathHelper.ToRadians(360)),
                        MovementVector = Vector2.Zero,
                    };
                    ParticleOrchestrator.SpawnParticlesDirect(ParticleOrchestraType.PrincessWeapon, settings);
                }
            }
        }
        public override Color? GetAlpha(Color lightColor)
        {
            return Main.DiscoColor * Projectile.Opacity;
        }
    }
}

