using Microsoft.Xna.Framework;
using Terraria;

namespace TouhouPets.Content.Dusts
{
    public class YuyukoButterfly : ModDust
    {
        public override void OnSpawn(Dust dust)
        {
            dust.color = Color.White;
            dust.alpha = 255;
            dust.scale = 1f;
        }
        public override bool Update(Dust dust)
        {
            dust.position += dust.velocity;

            int frameY = (dust.fadeIn % 10 <= 5) ? 1 : 0;
            dust.frame = new Rectangle(0, frameY * 10, 10, 10);

            float r = 1.47f;
            float g = 0.45f;
            float b = 1.03f;
            float brightness = ((255 - dust.alpha) / 255) * 0.5f;
            Lighting.AddLight(dust.position, r * brightness, g * brightness, b * brightness);

            dust.fadeIn++;
            if (dust.fadeIn >= 60)
            {
                dust.alpha += 255 / 20;

                if (dust.alpha >= 255)
                    dust.active = false;
            }
            else
            {
                dust.alpha -= 255 / 10;

                if (dust.alpha <= 0)
                    dust.alpha = 0;
            }
            return false;
        }
    }
}
