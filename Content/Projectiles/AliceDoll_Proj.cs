using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Net;
using Terraria;
using Terraria.ID;
using TouhouPets.Content.Projectiles.Pets;

namespace TouhouPets.Content.Projectiles
{
    public class AliceDoll_Proj : ModProjectile
    {
        public override void SetStaticDefaults()
        {
            Main.projFrames[Type] = 4;
            Main.projPet[Type] = true;
        }
        public override void SetDefaults()
        {
            Projectile.width = 22;
            Projectile.height = 22;
            Projectile.friendly = true;
            Projectile.penetrate = -1;
            Projectile.tileCollide = false;
        }
        public override bool? CanDamage()
        {
            return false;
        }
        public override bool PreDraw(ref Color lightColor)
        {
            if (IsAliceExited)
            {
                DrawLine(Color.White);
            }
            Texture2D tex = AltVanillaFunction.ProjectileTexture(Type);
            Vector2 pos = Projectile.Center - Main.screenPosition;
            int height = tex.Height / Main.projFrames[Type];
            Rectangle rect = new Rectangle(0, Projectile.frame * height, tex.Width, height);

            Color clr = Projectile.GetAlpha(lightColor);
            if (IsAliceExited)
            {
                clr *= Alice.AsTouhouPet().mouseOpacity;
            }

            Vector2 orig = rect.Size() / 2;
            SpriteEffects effect = Projectile.spriteDirection == -1 ? SpriteEffects.None : SpriteEffects.FlipHorizontally;
            Main.spriteBatch.MyDraw(tex, pos, rect, clr, 0f, orig, 1f, effect, 0);
            tex = AltVanillaFunction.GetExtraTexture("AliceDoll_Proj_Cloth");
            if (GetInstance<MiscConfig>().CompatibilityMode)
            {
                Main.spriteBatch.MyDraw(tex, pos, rect, clr, 0f, orig, 1f, effect, 0);
            }
            else
            {
                Main.EntitySpriteDraw(tex, pos, rect, clr, 0f, orig, 1f, effect, 0);
            }
            return false;
        }
        private void DrawLine(Color lightColor)
        {
            Vector2 startP = Alice.Center + AliceHandPoint() + new Vector2(0, 7f * Main.essScale);
            Texture2D tex = AssetLoader.GlowSpark.Value;

            Vector2 pos = Projectile.Center - Main.screenPosition;
            float dist = Vector2.Distance(startP, Projectile.Center);

            Color clr = Projectile.GetAlpha(Lighting.GetColor(Projectile.Center.ToTileCoordinates(), lightColor));
            clr *= Alice.AsTouhouPet().mouseOpacity;

            Vector2 orig = new Vector2(tex.Width / 2, 0);
            float rot = startP.DirectionTo(Projectile.Center).ToRotation() + MathHelper.PiOver2;

            Vector2 scale = new Vector2(0.1f, dist / tex.Height);
            if (GetInstance<MiscConfig>().CompatibilityMode)
            {
                Main.spriteBatch.MyDraw(tex, pos, null, clr, rot, orig, scale, SpriteEffects.None, 0);
            }
            else
            {
                Main.EntitySpriteDraw(tex, pos, null, clr, rot, orig, scale, SpriteEffects.None, 0);
            }
            Projectile.ResetDrawStateForPet();
        }
        Projectile Alice
        {
            get
            {
                return Main.projectile[(int)Projectile.ai[0]];
            }
        }
        Vector2 AliceHandPoint()
        {
            return Alice.frame switch
            {
                0 => new Vector2(-10 * Alice.spriteDirection, 4),
                2 => new Vector2(-14 * Alice.spriteDirection, 0),
                3 => new Vector2(-14 * Alice.spriteDirection, -4),
                4 => new Vector2(-10 * Alice.spriteDirection, 0),
                5 => new Vector2(-4 * Alice.spriteDirection, 0),
                6 => new Vector2(0 * Alice.spriteDirection, -2),
                7 => new Vector2(6 * Alice.spriteDirection, -4),
                8 => new Vector2(0 * Alice.spriteDirection, -2),
                9 => new Vector2(-10 * Alice.spriteDirection, 6),
                _ => Vector2.Zero,
            };
        }
        private bool IsAliceExited
        {
            get
            {
                return Alice != null && Alice.active && Alice.type == ProjectileType<Alice>() && Alice.owner == Projectile.owner;
            }
        }
        internal void MoveToPoint(Vector2 point, float speed)
        {
            Vector2 pos = point;
            float dist = Vector2.Distance(Projectile.Center, pos);
            Vector2 vel = pos - Projectile.Center;

            float actualSpeed = 1;

            if (dist < actualSpeed)
                Projectile.velocity *= 0.25f;

            if (vel != Vector2.Zero)
            {
                if (vel.Length() < actualSpeed)
                    Projectile.velocity = vel;
                else
                    Projectile.velocity = vel * 0.01f * speed;
            }
        }
        public override void AI()
        {
            Projectile.timeLeft = 2;
            if (Projectile.Opacity < 1)
                Projectile.Opacity = 1;

            if (Projectile.velocity.X > 0.25f)
            {
                Projectile.spriteDirection = 1;
            }
            else if (Projectile.velocity.X < -0.25f)
            {
                Projectile.spriteDirection = -1;
            }
            if (++Projectile.frameCounter > 8)
            {
                Projectile.frameCounter = 0;
                Projectile.frame++;
            }
            if (Projectile.frame > 3)
            {
                Projectile.frame = 0;
            }
            if (IsAliceExited)
            {
                int state = Alice.AsTouhouPet().PetState;
                if (state < 2 || state > 3)
                {
                    Projectile.velocity = Vector2.Normalize(Alice.Center - Projectile.Center) * (5f + Alice.velocity.Length());
                    if (Projectile.Distance(Alice.Center) <= 5f)
                    {
                        Projectile.active = false;
                        Projectile.netUpdate = true;
                        return;
                    }
                }
                else
                {
                    Vector2 point = Alice.Center + new Vector2(Projectile.ai[1], Projectile.ai[2]);
                    MoveToPoint(point, 18f);
                }
            }
            else
            {
                Projectile.active = false;
                Projectile.netUpdate = true;
            }
        }
    }
}
