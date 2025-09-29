using Microsoft.Xna.Framework;
using Terraria;

namespace TouhouPets.Content.Dusts
{
    public class WakasagihimeBubble : ModDust
    {
        public override void OnSpawn(Dust dust)
        {
            dust.scale = 0.1f;
            dust.frame = new Rectangle(0, 12 * Main.rand.Next(0, 3), 10, 12);
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
