using Microsoft.Xna.Framework;
using Terraria;
using Terraria.GameContent.Drawing;
using Terraria.ID;
using TouhouPets.Content.Buffs.PetBuffs;
using TouhouPets.Content.Projectiles;

namespace TouhouPets
{
    public class BandPlayer : ModPlayer
    {
        public const int BAND_COUNTDOWN_TIME = 204;

        public bool prismriverBand = false;
        public bool ticketUsed = false;
        public bool rerollMusic = false;

        private int bandCountdown = 0;
        private int bandTimer = 0;
        private int musicID = -1;
        public int BandMusicID { get => musicID; }
        public bool ShouldBandPlaying { get => bandTimer > 0; }
        private void ConcertVisualEffect()
        {
            if (Main.rand.NextBool(5) && bandTimer % 10 == 0)
            {
                Gore.NewGoreDirect(Player.GetSource_FromAI()
                , Player.Center + new Vector2(Main.rand.Next(-240, 240), -100 + Main.rand.Next(-80, 40))
                , new Vector2(Main.rand.Next(-2, 2), Main.rand.Next(-6, -3)) * 0.1f, Main.rand.Next(570, 573)
                , Main.rand.NextFloat(0.9f, 1.1f));
            }
            if (Main.rand.NextBool(12) && bandTimer > 360)
            {
                Vector2 p = Player.Center + new Vector2(Main.rand.Next(-240, 240), -100 + Main.rand.Next(-80, 40));
                ParticleOrchestraSettings settings = new ParticleOrchestraSettings
                {
                    PositionInWorld = p,
                    MovementVector = Vector2.Zero
                };
                var particleType = Main.rand.Next(5) switch
                {
                    1 => ParticleOrchestraType.StardustPunch,
                    _ => ParticleOrchestraType.PrincessWeapon,
                };
                ParticleOrchestrator.SpawnParticlesDirect(particleType, settings);

                if (Main.rand.NextBool(12) && bandTimer > 720)
                {
                    ParticleOrchestrator.SpawnParticlesDirect(ParticleOrchestraType.StellarTune, settings);
                }
            }
            if (!Player.HasBuff<RaikoBuff>())
            {
                return;
            }
            if (Main.rand.NextBool(10) && bandTimer > 1080 && bandTimer % 5 == 0)
            {
                Vector2 p = Player.Center + new Vector2(Main.rand.Next(-270, 270), -180 + Main.rand.Next(-180, -160));
                if (Player.ownedProjectileCounts[ProjectileType<BandSpotlight>()] < 10)
                {
                    Projectile ray = Projectile.NewProjectileDirect(Player.GetSource_FromThis(), p, Vector2.Zero, ProjectileType<BandSpotlight>(),
                        0, 0f, Main.myPlayer, Main.rand.Next(0, 6), Main.rand.NextFloat(MathHelper.ToRadians(-30), MathHelper.ToRadians(30)));
                    ray.netUpdate = true;
                }
            }
            if (bandTimer > 1440 && bandTimer % 30 == 0 && Main.rand.NextBool(6))
            {
                Vector2 p = Player.Center + new Vector2(Main.rand.Next(-270, 270), -70 + Main.rand.Next(10, 20));
                Projectile rocket = Projectile.NewProjectileDirect(Player.GetSource_FromThis(), p, new Vector2(0, -8)
                    , Main.rand.Next(ProjectileID.RocketFireworksBoxRed, ProjectileID.RocketFireworksBoxYellow + 1)
                    , 0, 0f, Main.myPlayer);
                rocket.tileCollide = false;
                rocket.netUpdate = true;
            }
        }
        public override void PostUpdateMiscEffects()
        {
            if (!Player.HasBuff<PoltergeistBuff>())
            {
                ticketUsed = false;
            }
            if (!ticketUsed && Player.afkCounter <= 0 || !Player.HasBuff<PoltergeistBuff>())
            {
                prismriverBand = false;
            }
            if (prismriverBand)
            {
                bandCountdown = (int)MathHelper.Clamp(bandCountdown - 1, 0, BAND_COUNTDOWN_TIME);
                if (bandCountdown <= 0)
                {
                    bandTimer++;
                    if (!rerollMusic)
                    {
                        musicID = Main.rand.Next(MusicID.OtherworldlyRain, MusicID.OtherworldlyHallow + 1);
                        rerollMusic = true;
                    }
                }
            }
            else
            {
                bandTimer = 0;
                bandCountdown = BAND_COUNTDOWN_TIME;
                rerollMusic = false;
            }
            if (bandCountdown <= 0)
            {
                ConcertVisualEffect();
            }
        }
    }
}
