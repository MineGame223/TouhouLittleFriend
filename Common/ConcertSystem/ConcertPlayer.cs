using Microsoft.Xna.Framework;
using System.Collections.Generic;
using Terraria;
using Terraria.ID;
using TouhouPets.Content.Buffs.PetBuffs;
using TouhouPets.Content.Items;
using TouhouPets.Content.Projectiles;

namespace TouhouPets
{
    public class ConcertPlayer : ModPlayer
    {
        public const int BAND_COUNTDOWN_TIME = 204;
        public const int MAX_BANDTIME = 2040;

        public bool prismriverBand = false;
        public bool manualStartBand = false;
        public bool musicRerolled = false;
        public bool manualRerolled = false;
        public bool customMode = false;

        private int bandCountdown = BAND_COUNTDOWN_TIME;
        private int bandTimer = 0;
        private int bandTimerForVisual = 0;

        private int musicID = -1;
        private int lastMusicID = -1;

        private readonly List<int> constantMusicList = [MusicID.Title, MusicID.ConsoleMenu, MusicID.Credits];
        private readonly List<int> bannedMusicList = [MusicID.RainSoundEffect, 45];
        public int BandMusicID { get => musicID; }
        public bool ShouldBandPlaying { get => bandCountdown <= 0; }
        public bool IsConcertStarted { get => prismriverBand; }
        private void UpdateVisualTimer()
        {
            if (++bandTimerForVisual > 4800)
            {
                bandTimerForVisual = 0;
            }
        }
        private void ConcertVisualEffect()
        {
            UpdateVisualTimer();

            /*if (Main.rand.NextBool(5) && bandTimerForVisual % 10 == 0)
            {
                Gore.NewGoreDirect(Player.GetSource_FromThis()
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
            }*/

            if (!Player.HasBuff<RaikoBuff>())
                return;

            if (Main.rand.NextBool(10) && bandTimer > 720 && bandTimerForVisual % 5 == 0)
            {
                Vector2 p = Player.Center + new Vector2(Main.rand.Next(-270, 270), -180 + Main.rand.Next(-180, -160));
                if (Player.ownedProjectileCounts[ProjectileType<BandSpotlight>()] < 10)
                {
                    Projectile ray = Projectile.NewProjectileDirect(Player.GetSource_FromThis(), p, Vector2.Zero, ProjectileType<BandSpotlight>(),
                        0, 0f, Main.myPlayer, Main.rand.Next(0, 6), Main.rand.NextFloat(MathHelper.ToRadians(-30), MathHelper.ToRadians(30)));
                    ray.netUpdate = true;
                }
            }
            if (bandTimer > 1080 && bandTimerForVisual % 30 == 0 && Main.rand.NextBool(6))
            {
                Vector2 p = Player.Center + new Vector2(Main.rand.Next(-270, 270), -70 + Main.rand.Next(10, 20));
                Projectile rocket = Projectile.NewProjectileDirect(Player.GetSource_FromThis(), p, new Vector2(0, -8)
                    , Main.rand.Next(ProjectileID.RocketFireworksBoxRed, ProjectileID.RocketFireworksBoxYellow + 1)
                    , 0, 0f, Main.myPlayer);
                rocket.tileCollide = false;
                rocket.netUpdate = true;
            }
        }
        private void RerollMusic()
        {
            bool chooseMenuMusic = Main.rand.NextBool(10);
            List<int> musicList = [];
            for (int i = 0; i < MusicID.Count; i++)
            {
                bool drunkReroll = i >= MusicID.OtherworldlyRain && i <= MusicID.OtherworldlyHallow;
                bool regularReroll = i <= MusicID.OtherworldlyRain && i >= MusicID.OtherworldlyHallow;

                if (bannedMusicList.Contains(i))
                {
                    continue;
                }
                if (chooseMenuMusic && !constantMusicList.Contains(i))
                {
                    continue;
                }
                if (Main.drunkWorld && drunkReroll)
                {
                    continue;
                }
                if (!Main.drunkWorld && regularReroll)
                {
                    continue;
                }
                if (i == lastMusicID)
                {
                    continue;
                }
                musicList.Add(i);
            }
            int randomID = Main.rand.NextFromCollection(musicList);
            lastMusicID = randomID;
            musicID = randomID;
        }
        public override void PostUpdateMiscEffects()
        {
            if (!Player.HasBuff<PoltergeistBuff>())
            {
                manualStartBand = false;
            }
            if ((!manualStartBand && Player.afkCounter <= 0) || !Player.HasBuff<PoltergeistBuff>())
            {
                prismriverBand = false;
            }
            //防止因为手动开启演唱会导致已有演唱会被中断，或者使用右键手动开启演唱会
            bool duringConcert = (bandCountdown > 0 && bandCountdown < BAND_COUNTDOWN_TIME) || ShouldBandPlaying;
            bool playerUsingItem = Player.itemAnimation > 0 && Player.altFunctionUse != 2;

            bool holdStick = Player.HeldItem.type == ItemType<SupportStick>() && manualStartBand;

            if (holdStick && duringConcert && playerUsingItem)
            {
                prismriverBand = true;
            }
            if (prismriverBand)
            {
                bandCountdown = (int)MathHelper.Clamp(bandCountdown - 1, 0, BAND_COUNTDOWN_TIME);
                if (bandCountdown <= 0)
                {
                    if (!musicRerolled)
                    {
                        if (customMode)
                        {
                            CustomMusicManager.RollListedMusic(manualRerolled, false);
                            manualRerolled = false;
                        }
                        else
                        {
                            RerollMusic();
                        }
                        musicRerolled = true;
                    }
                }
            }
            else
            {
                bandTimer = 0;
                bandTimerForVisual = 0;
                bandCountdown = BAND_COUNTDOWN_TIME;
                musicRerolled = false;
                manualRerolled = false;
            }
            if (bandCountdown <= 0 && Player.IsStandingStillForSpecialEffects)
            {
                bandTimer = (int)MathHelper.Clamp(bandTimer + 1, 0, MAX_BANDTIME);
                ConcertVisualEffect();
            }
        }
    }
}
