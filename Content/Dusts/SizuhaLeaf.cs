using Microsoft.Xna.Framework;
using Terraria;

namespace TouhouPets.Content.Dusts
{
    public class SizuhaLeaf : ModDust
    {
        public override void OnSpawn(Dust dust)
        {
            dust.alpha = 255;
            dust.frame = Texture2D.Frame(1, 3, 0, Main.rand.Next(0, 3));
        }
        public override bool Update(Dust dust)
        {
            dust.position += dust.velocity;
            dust.velocity.X += Main.windSpeedCurrent * 0.1f;
            dust.rotation += 0.05f * Vector2.Normalize(dust.velocity).X;

            if (dust.fadeIn == 0)
            {
                dust.alpha -= 255 / 10;

                if (dust.alpha <= 0)
                {
                    dust.alpha = 0;
                    dust.fadeIn++;
                }
            }
            else
            {
                dust.fadeIn++;

                if (dust.fadeIn >= 40)
                {
                    dust.alpha += 255 / 20;

                    if (dust.alpha >= 255)
                        dust.active = false;
                }
            }

            return false;
        }
    }
}
