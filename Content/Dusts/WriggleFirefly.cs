using Microsoft.Xna.Framework;
using Terraria;

namespace TouhouPets.Content.Dusts
{
    public class WriggleFirefly : ModDust
    {
        public override void OnSpawn(Dust dust)
        {
            dust.color = Color.White;
            dust.scale = 0.1f;
        }
        public override bool Update(Dust dust)
        {
            dust.position += dust.velocity;
            float r;
            float g;
            float b;
            int frame = dust.frame.Y / dust.frame.Height;
            switch (frame)
            {
                case 1:
                    r = 0.65f;
                    g = 1.48f;
                    b = 0.44f;
                    break;
                case 2:
                    r = 0.44f;
                    g = 1.45f;
                    b = 1.48f;
                    break;
                case 3:
                    r = 1.48f;
                    g = 0.65f;
                    b = 0.44f;
                    break;
                default:
                    r = 1.48f;
                    g = 1.44f;
                    b = 0.44f;
                    break;
            }
            float brightness = ((255 - dust.alpha) / 255) * (1 + dust.frame.Y / dust.frame.Height * .5f);
            Lighting.AddLight(dust.position, r * brightness, g * brightness, b * brightness);

            if (dust.fadeIn == 0)
            {
                if (dust.scale < 1)
                    dust.scale += 0.2f;
                else
                    dust.fadeIn++;
            }
            else
            {
                dust.fadeIn++;
                dust.velocity *= 0.995f;
                if (dust.fadeIn >= 80)
                {
                    dust.scale -= 0.1f;

                    if (dust.scale <= 0)
                        dust.active = false;
                }
            }

            return false;
        }
    }
}
