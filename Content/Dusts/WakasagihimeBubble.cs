using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;

namespace TouhouPets.Content.Dusts
{
    public class WakasagihimeBubble : ModDust
    {
        public override void OnSpawn(Dust dust)
        {
            dust.scale = 0.1f;
            dust.frame = Texture2D.Frame(1, 3, 0, Main.rand.Next(0, 3));
        }
        public override bool PreDraw(Dust dust)
        {
            Texture2D tex = Texture2D.Value;
            Vector2 orig = dust.frame.Size() / 2;
            Vector2 pos = dust.position + orig - Main.screenPosition;

            Color clr = dust.color * ((255 - dust.alpha) / 255f);

            Main.EntitySpriteDraw(tex, pos, dust.frame, clr, dust.rotation, orig, dust.scale, SpriteEffects.None);
            return false;
        }
        public override bool Update(Dust dust)
        {
            dust.position += dust.velocity;
            dust.velocity *= 0.995f;

            if (dust.velocity.Y > -4f)
                dust.velocity.Y -= 0.03f;

            if (dust.scale <= Main.rand.NextFloat(0.6f, 1.2f))
            {
                dust.scale += 0.1f;
            }
            else
            {
                dust.fadeIn++;

                if (dust.fadeIn >= 30)
                {
                    dust.alpha += 255 / 15;

                    if (dust.alpha >= 255)
                        dust.active = false;
                }
            }

            return false;
        }
    }
}
