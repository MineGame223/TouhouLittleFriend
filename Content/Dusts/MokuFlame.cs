using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using Terraria;

namespace TouhouPets.Content.Dusts
{
    public class MokuFlame : ModDust
    {
        private readonly Asset<Texture2D> dustTex = Request<Texture2D>("TouhouPets/Content/Dusts/MokuFlame");
        public override bool PreDraw(Dust dust)
        {
            Texture2D tex = dustTex.Value;
            Rectangle rect = dust.frame;
            Vector2 orig = rect.Size() / 2;
            Vector2 pos = dust.position + orig - Main.screenPosition;

            int alpha = 255 - dust.alpha;
            Color clr = new(alpha, alpha, alpha, 0);

            for (int i = 0; i < 3; i++)
            {
                Main.EntitySpriteDraw(tex,
                    pos + new Vector2(Main.rand.NextFloat(-1.2f, 1.2f), Main.rand.NextFloat(-1.2f, 1.2f))
                    , rect, clr * 0.5f, 0f, orig, dust.scale, SpriteEffects.None);
            }
            //Main.EntitySpriteDraw(tex, pos, rect, clr, 0f, orig, dust.scale * 0.8f, SpriteEffects.None);
            return false;
        }
        public override void OnSpawn(Dust dust)
        {
            dust.color = Color.White;
            dust.alpha = 255;
            dust.frame = new Rectangle(0, Main.rand.Next(0, 3) * 18, 14, 18);
        }
        public override bool Update(Dust dust)
        {
            float r = 1.79f;
            float g = 1.11f;
            float b = 0.55f;
            float brightness = (255 - dust.alpha) / 255;
            Lighting.AddLight(dust.position, r * brightness, g * brightness, b * brightness);

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

                if (dust.fadeIn >= 20)
                {
                    dust.scale -= 0.02f;

                    if (dust.scale <= 0)
                        dust.active = false;
                }
                else
                {
                    if (Main.rand.NextBool(7))
                    {
                        Dust.NewDustPerfect(dust.position
                            + new Vector2(Main.rand.Next(0, 14), Main.rand.Next(0, 18))
                            , MyDustId.Fire
                            , new Vector2(Main.rand.NextFloat(-1f, 1f), Main.rand.NextFloat(-1f, 1f))
                            , 100, default, Main.rand.NextFloat(1f, 2f)).noGravity = true;
                    }
                }
            }
            return false;
        }
    }
}
