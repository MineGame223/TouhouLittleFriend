using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.DataStructures;
using Terraria.GameContent.Drawing;
using Terraria.ID;
using TouhouPets.Content.Items.PetItems;

namespace TouhouPets.Content.Projectiles
{
    public class SanaeCoinSpawnAnimation : ModProjectile
    {
        public override string Texture => GetNamespace<SanaeCoinRare>();
        private readonly Texture2D silverCoinTex = AltVanillaFunction.GetTexture(GetNamespace<SanaeCoin>());
        public override void SetDefaults()
        {
            Projectile.width = 50;
            Projectile.height = 50;
            Projectile.friendly = true;
            Projectile.penetrate = -1;
            Projectile.ignoreWater = true;
            Projectile.timeLeft = 240;
        }
        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D glow = AltVanillaFunction.ExtraTexture(ExtrasID.ThePerfectGlow);
            Vector2 glowPos = Projectile.Center - Main.screenPosition;
            float glowScale = Utils.Remap(Timer, 160, 210, 0, 1);
            float coinAlpha = Utils.Remap(Timer, 0, 15, 0, 1);
            float coinDist = Timer switch
            {
                < 30 => Utils.Remap(Timer, 0, 30, 1, 0),
                _ => Utils.Remap(Timer, 60, 180, 0, 1)
            };
            Color coinColor = Projectile.GetAlpha(Color.White) * coinAlpha;
            Vector2 coinOrig = silverCoinTex.Size() / 2;

            Color glowClr = Projectile.GetAlpha(Color.Yellow);
            glowClr.A *= 0;
            Color glowClr2 = Projectile.GetAlpha(Color.White);
            glowClr2.A *= 0;
            for (int i = 0; i < 10; i++)
            {
                Vector2 coinOffset = new Vector2(MathHelper.SmoothStep(80, 0, coinDist), 0).RotatedBy(MathHelper.ToRadians(36 * i + Timer));
                for (int j = 0; j < 4; j++)
                {
                    Vector2 shadowPos = new Vector2(0, 4 * Main.essScale).RotatedBy(MathHelper.ToRadians(90 * j));
                    Main.spriteBatch.MyDraw(silverCoinTex, glowPos + shadowPos + coinOffset
                    , null, glowClr2 * coinAlpha * 0.8f, 0f, coinOrig, 1f, SpriteEffects.None, 0);
                }
                Main.spriteBatch.MyDraw(silverCoinTex, glowPos + coinOffset
                    , null, coinColor * (1f - glowScale), 0f, coinOrig, 1f, SpriteEffects.None, 0);
            }
            if (Timer >= 60)
            {
                float glowScale1 = glowScale * Main.rand.NextFloat(0.9f, 1.2f);
                float glowScale2 = glowScale * Main.rand.NextFloat(0.9f, 1.2f);
                Main.spriteBatch.MyDraw(glow, glowPos, null, glowClr, MathHelper.PiOver2, glow.Size() / 2, new Vector2(1f, 4f) * glowScale1, SpriteEffects.None, 0);
                Main.spriteBatch.MyDraw(glow, glowPos, null, glowClr, 0, glow.Size() / 2, new Vector2(1f, 3f) * glowScale2, SpriteEffects.None, 0);

                Main.spriteBatch.MyDraw(glow, glowPos, null, glowClr2, MathHelper.PiOver2, glow.Size() / 2, new Vector2(1f, 4f) * glowScale1 * 0.75f, SpriteEffects.None, 0);
                Main.spriteBatch.MyDraw(glow, glowPos, null, glowClr2, 0, glow.Size() / 2, new Vector2(1f, 3f) * glowScale2 * 0.75f, SpriteEffects.None, 0);
            }
            return false;
        }
        public int State
        {
            get => (int)Projectile.ai[0];
            set
            {
                Projectile.ai[0] = value;
                Projectile.netUpdate = true;
            }
        }
        public int Timer
        {
            get => (int)Projectile.localAI[0];
            set => Projectile.localAI[0] = value;
        }
        public override void OnKill(int timeLeft)
        {
            for (int i = 0; i < 18; i++)
            {
                Dust d = Dust.NewDustDirect(Projectile.Center, 1, 1, MyDustId.TrailingYellow, 0, 0, 100, default, Main.rand.NextFloat(1f, 2.5f));
                d.noGravity = false;
                d.velocity = new Vector2(0, Main.rand.Next(4, 16)).RotateRandom(MathHelper.TwoPi);
            }
            for (int i = 0; i < 36; i++)
            {
                Dust d = Dust.NewDustDirect(Projectile.Center, 1, 1, MyDustId.YellowGoldenFire, 0, 0, 100, default, Main.rand.NextFloat(3f, 5.5f));
                d.noGravity = true;
                d.velocity = new Vector2(0, -12).RotatedBy(MathHelper.ToRadians(10 * i));
            }

            ParticleOrchestraSettings settings;
            for (int z = 0; z < 8; z++)
            {
                settings = new ParticleOrchestraSettings
                {
                    PositionInWorld = Projectile.Center + new Vector2(0, Main.rand.Next(0, 50)).RotatedByRandom(MathHelper.TwoPi),
                    MovementVector = Vector2.Zero,
                };
                ParticleOrchestrator.SpawnParticlesDirect(ParticleOrchestraType.StardustPunch, settings);
            }
            settings = new ParticleOrchestraSettings
            {
                PositionInWorld = Projectile.Center,
                MovementVector = Vector2.Zero,
            };
            ParticleOrchestrator.SpawnParticlesDirect(ParticleOrchestraType.ShimmerTownNPC, settings);

            Item.NewItem(new EntitySource_Gift(Projectile), Projectile.getRect(), ItemType<SanaeCoinRare>());
        }
        public override void AI()
        {
            if (State == 0)
            {
                Projectile.velocity = Vector2.Zero;
                Projectile.velocity.Y = -12;

                if (Main.myPlayer == Projectile.owner)
                    State++;
            }
            Projectile.velocity *= 0.95f;
            Timer++;

            if (Timer > 60)
            {
                float dustScale = Utils.Remap(Timer, 60, 210, 0, 2);
                Vector2 pos = Projectile.Center + new Vector2(0, Main.rand.Next(45, 80)).RotateRandom(MathHelper.TwoPi);
                Dust d = Dust.NewDustDirect(pos, 1, 1, MyDustId.YellowTrans, 0, 0, 100, default, Main.rand.NextFloat(0.5f, 1f) + dustScale);
                d.noGravity = true;
                d.velocity = Vector2.Normalize(Projectile.Center - pos) * (Main.rand.Next(5, 9) + dustScale);
            }
        }
    }
}
