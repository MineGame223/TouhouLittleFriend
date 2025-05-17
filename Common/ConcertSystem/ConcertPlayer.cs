using Microsoft.Xna.Framework;
using System.Collections.Generic;
using Terraria;
using Terraria.ID;
using TouhouPets.Content.Buffs;
using TouhouPets.Content.Buffs.PetBuffs;
using TouhouPets.Content.Items;
using TouhouPets.Content.Projectiles;

namespace TouhouPets
{
    public class ConcertPlayer : ModPlayer
    {
        public const int BAND_COUNTDOWN_TIME = 204;
        public const int MAX_BANDTIME = 2040;

        private bool prismriverBand = false;
        private bool customMode = false;
        private bool musicRerolled = false;
        private bool manualRerolled = false;
        private bool exitedFromCustomMode = true;

        private int bandTimer = 0;

        private int musicID = -1;
        private int lastMusicID = -1;

        private readonly List<int> constantMusicList = [MusicID.Title, MusicID.ConsoleMenu, MusicID.Credits];
        private readonly List<int> bannedMusicList = [MusicID.RainSoundEffect, 45];
        public bool ManualConcert { get => Player.HasBuff<ConcertBuff>(); }
        public int BandMusicID { get => musicID; }
        public bool ShouldBandPlaying { get => bandTimer > BAND_COUNTDOWN_TIME; }
        public bool ConcertStart { get => prismriverBand; set => prismriverBand = value; }
        public bool CustomModeOn { get => customMode; set => customMode = value; }
        public bool MusicRerolled { get => musicRerolled; set => musicRerolled = value; }
        public bool ManualRerolled { get => manualRerolled; set => manualRerolled = value; }
        private void ConcertVisualEffect()
        {
            if (!Player.HasBuff<RaikoBuff>())
                return;

            int time = bandTimer - BAND_COUNTDOWN_TIME;
            if (Main.rand.NextBool(10) && time > 720 && bandTimer % 5 == 0)
            {
                Vector2 p = Player.Center + new Vector2(Main.rand.Next(-270, 270), -180 + Main.rand.Next(-180, -160));
                if (Player.ownedProjectileCounts[ProjectileType<BandSpotlight>()] < 10)
                {
                    Projectile ray = Projectile.NewProjectileDirect(Player.GetSource_FromThis(), p, Vector2.Zero, ProjectileType<BandSpotlight>(),
                        0, 0f, Main.myPlayer, Main.rand.Next(0, 6), Main.rand.NextFloat(MathHelper.ToRadians(-30), MathHelper.ToRadians(30)));
                    ray.netUpdate = true;
                }
            }
            if (time > 1080 && bandTimer % 30 == 0 && Main.rand.NextBool(6))
            {
                Vector2 p = Player.Center + new Vector2(Main.rand.Next(-270, 270), -70 + Main.rand.Next(10, 20));
                Projectile rocket = Projectile.NewProjectileDirect(Player.GetSource_FromThis(), p, new Vector2(0, -8)
                    , Main.rand.Next(ProjectileID.RocketFireworksBoxRed, ProjectileID.RocketFireworksBoxYellow + 1)
                    , 0, 0f, Main.myPlayer);
                rocket.tileCollide = false;
                rocket.netUpdate = true;
            }
        }
        private void RerollMusic(bool playLastMusic)
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
            if (lastMusicID < 0)
            {
                lastMusicID = randomID;
            }
            musicID = playLastMusic ? lastMusicID : randomID;
            lastMusicID = musicID;
        }
        public override void PostUpdateMiscEffects()
        {
            if (ConcertStart)
            {
                bandTimer++;
                if (bandTimer > MAX_BANDTIME)
                {
                    bandTimer = BAND_COUNTDOWN_TIME + 10;
                }
                if (Player.IsStandingStillForSpecialEffects)
                {
                    ConcertVisualEffect();
                }
                if (bandTimer > BAND_COUNTDOWN_TIME)
                {
                    if (!MusicRerolled)
                    {
                        if (CustomModeOn)
                        {
                            CustomMusicManager.RollListedMusic(ManualRerolled, false);
                            ManualRerolled = false;
                            exitedFromCustomMode = false;
                        }
                        else
                        {
                            RerollMusic(!exitedFromCustomMode);
                            exitedFromCustomMode = true;
                        }
                        MusicRerolled = true;
                    }
                }
            }
            else
            {
                bandTimer = 0;
                MusicRerolled = false;
                ManualRerolled = false;
                exitedFromCustomMode = true;
                Player.ClearBuff(BuffType<ConcertBuff>());
            }

            if (!Player.HasBuff<PoltergeistBuff>())
            {
                Player.ClearBuff(BuffType<ConcertBuff>());
                return;
            }
            if ((!ManualConcert && Player.afkCounter <= 0) || !Player.HasBuff<PoltergeistBuff>())
            {
                ConcertStart = false;
            }
            //防止因为使用应援棒导致已有演唱会被中断
            bool playerUsingItem = Player.itemAnimation > 0;
            bool holdStick = Player.HeldItem.type == ItemType<SupportStick>();

            if (holdStick && playerUsingItem)
            {
                ConcertStart = true;
            }
        }
    }
}
