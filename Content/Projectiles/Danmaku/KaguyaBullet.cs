﻿using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ID;

namespace TouhouPets.Content.Projectiles.Danmaku
{
    public class KaguyaBullet : ModProjectile
    {
        public override void SetStaticDefaults()
        {
            Main.projFrames[Type] = 5;
        }
        public override void SetDefaults()
        {
            Projectile.width = 16;
            Projectile.height = 16;
            Projectile.friendly = true;
            Projectile.Opacity = 0f;
            Projectile.ignoreWater = true;
            Projectile.tileCollide = false;
            Projectile.timeLeft = 360;
            Projectile.extraUpdates = 1;
            Projectile.GetGlobalProjectile<TouhouPetGlobalProj>().isADanmaku = true;
            Projectile.GetGlobalProjectile<TouhouPetGlobalProj>().isDanmakuDestorible = true;
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
            Color clr = Projectile.GetAlpha(Color.White);
            Vector2 orig = rect.Size() / 2;
            Main.spriteBatch.MyDraw(tex, pos, rect, clr, Projectile.rotation, orig, Projectile.scale, SpriteEffects.None, 0);
            return false;
        }
        public int DustType
        {
            get
            {
                return Projectile.frame switch
                {
                    1 => MyDustId.RedTrans,
                    2 => MyDustId.BlueTrans,
                    3 => MyDustId.GreenTrans,
                    4 => MyDustId.TransparentPurple,
                    _ => MyDustId.YellowTrans,
                };
            }
        }
        public override void AI()
        {
            Projectile.HandleDanmakuCollide();
            if (Projectile.localAI[0] < 90)
            {
                Projectile.Opacity = MathHelper.Clamp(Projectile.Opacity + 0.1f, 0, 1);
            }
            else
            {
                Projectile.Opacity -= 0.05f;
                if (Projectile.Opacity <= 0f)
                {
                    Projectile.active = false;
                    Projectile.netUpdate = true;
                    return;
                }
            }
            Projectile.frame = (int)Projectile.ai[0];
            Projectile.rotation -= 1.104719758f;
            Projectile.localAI[0] += 1f;
            if (Main.rand.NextBool(5))
            {
                int d = Dust.NewDust(new Vector2(Projectile.position.X, Projectile.position.Y), Projectile.width, Projectile.height, DustType, Projectile.velocity.X, Projectile.velocity.Y, 50, default(Color), 1.2f);
                Main.dust[d].noGravity = true;
                Dust dust = Main.dust[d];
                dust.velocity *= 0.3f;
            }
        }
        public override void OnKill(int timeLeft)
        {
            AltVanillaFunction.PlaySound(SoundID.Dig, Projectile.position);
            for (int i = 0; i < 11; i++)
            {
                int d = Dust.NewDust(new Vector2(Projectile.position.X, Projectile.position.Y), Projectile.width, Projectile.height, DustType, Projectile.oldVelocity.X, Projectile.oldVelocity.Y, 50, default(Color), 1.2f);
                Main.dust[d].noGravity = true;
                Dust dust = Main.dust[d];
                dust.scale *= 1.45f;
                dust = Main.dust[d];
                dust.velocity *= 0.5f;
            }
        }
    }
}

