using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;

namespace TouhouPets.Content.Dusts
{
    public class WriggleFirefly : ModDust
    {
        public override void OnSpawn(Dust dust)
        {
            dust.color = Color.White;
            dust.scale = 0.1f;

            dust.frame = Texture2D.Frame(4, 2, 0, Main.rand.Next(2));
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
            if (dust.customData != null && dust.customData is int type)
            {
                dust.frame.X = type * dust.frame.Width;
            }
            dust.position += dust.velocity;
            float r;
            float g;
            float b;
            int frame = dust.frame.X / dust.frame.Width;
            switch (frame)
            {
                case 1:
                    r = 0.65f;
                    g = 1.48f;
                    b = 0.44f;
                    break;
                case 2:
                    r = 0.68f;
                    g = 1.04f;
                    b = 1.55f;
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
            float brightness = (255 - dust.alpha) / 255f;
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
                    dust.scale -= 0.3f;

                    if (dust.scale <= 0)
                        dust.active = false;
                }
            }

            return false;
        }
    }
}
