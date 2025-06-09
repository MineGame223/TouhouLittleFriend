using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.IO;
using Terraria;
using Terraria.DataStructures;
using Terraria.Graphics.Shaders;
using Terraria.ID;

namespace TouhouPets.Content.Projectiles
{
    public class StarTrail : ModProjectile
    {
        public override string Texture => $"Terraria/Images/Item_{ProjectileID.WoodenArrowFriendly}";
        public override void SetDefaults()
        {
            Projectile.width = 50;
            Projectile.height = 50;
            Projectile.friendly = true;
            Projectile.penetrate = -1;
            Projectile.ignoreWater = true;
            Projectile.timeLeft = 125;
        }
        public int TrailLength
        {
            get => (int)Projectile.ai[0];
            set => Projectile.ai[0] = value;
        }
        public float Radius
        {
            get => Projectile.ai[1];
            set => Projectile.ai[1] = value;
        }
        public int RotatingDrift
        {
            get => (int)Projectile.ai[2];
            set => Projectile.ai[2] = value;
        }
        private Color trailColor
        {
            get
            {
                return Projectile.localAI[0] switch
                {
                    /*1 => Color.Red,
                    2 => Color.Orange,
                    3 => Color.Yellow,
                    4 => Color.Green,
                    5 => Color.Cyan,
                    6 => Color.Blue,
                    _ => Color.Purple,*/
                    1 => Color.AliceBlue,
                    2 => Color.LightYellow,
                    3 => Color.Cyan,
                    4 => Color.LightGoldenrodYellow,
                    5 => Color.SkyBlue,
                    _ => Color.Goldenrod,
                };
            }
        }
        public override bool PreDraw(ref Color lightColor)
        {
            lightColor = trailColor * Projectile.Opacity;
            int rate = 1;
            for (int i = 0; i < Projectile.localAI[1] / rate; i++)
            {
                Texture2D line = AltVanillaFunction.ExtraTexture(ExtrasID.StardustTowerMark);
                Vector2 pos = Projectile.Center + new Vector2(0, -Radius).RotatedBy(MathHelper.ToRadians(rate * i) + Main.GlobalTimeWrappedHourly / 2 + MathHelper.ToRadians(RotatingDrift));
                int singleLineLength = (int)(Radius * 2 * MathHelper.Pi * 1.1f) / (360 / rate);
                Rectangle rect = new Rectangle(0, 0, line.Width, singleLineLength);
                Vector2 orig = new Vector2(line.Width / 2, 0);
                float rot = pos.DirectionTo(Projectile.Center).ToRotation();

                DrawData data = new DrawData(line, pos - Main.screenPosition, rect, lightColor, rot, orig, 1f, SpriteEffects.None, 0);

                if (GetInstance<MiscConfig>().CompatibilityMode)
                {
                    data.Draw(Main.spriteBatch);
                    return false;
                }

                Main.spriteBatch.QuickEndAndBegin(true);
                
                GameShaders.Armor.Apply(Main.player[Projectile.owner].cLight, Projectile, data);
                data.Draw(Main.spriteBatch);

                Main.spriteBatch.QuickEndAndBegin(false);
            }
            return false;
        }
        public override void OnSpawn(IEntitySource source)
        {
            if (Projectile.owner != Main.myPlayer)
                return;

            Projectile.localAI[0] = Main.rand.Next(7);
            Projectile.netUpdate = true;
            Projectile.Opacity = 0;
        }

        public override void AI()
        {
            Projectile master = Main.projectile[(int)Projectile.localAI[2]];
            if (master.active && master.type == ProjectileType<Pets.Star>())
            {
                Projectile.Center = master.Center;
            }
            else
            {
                Projectile.timeLeft = 0;
                Projectile.netUpdate = true;
                return;
            }
            if (Projectile.localAI[1] < TrailLength)
                Projectile.localAI[1] += 0.2f;

            if (Projectile.timeLeft > 45)
            {
                Projectile.Opacity += 0.05f;
            }
            if (Projectile.timeLeft <= 10)
            {
                Projectile.Opacity -= 0.1f;
            }
            Projectile.Opacity = MathHelper.Clamp(Projectile.Opacity, 0, 1 - (Radius / 800));
            Projectile.Opacity *= master.ToPetClass().mouseOpacity;
        }
        public override bool? CanDamage()
        {
            return false;
        }
        public override bool? CanCutTiles()
        {
            return false;
        }
        public override void ReceiveExtraAI(BinaryReader reader)
        {
            Projectile.localAI[0] = reader.ReadSingle();
        }
        public override void SendExtraAI(BinaryWriter writer)
        {
            writer.Write(Projectile.localAI[0]);
        }
    }
}
