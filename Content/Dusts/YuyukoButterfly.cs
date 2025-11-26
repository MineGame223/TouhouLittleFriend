using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
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

            int frameY = (dust.fadeIn % 10 <= 5) ? 1 : 0;
            dust.frame = Texture2D.Frame(1, 2, 0, frameY);

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
