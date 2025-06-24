using Microsoft.Xna.Framework;
using System;
using Terraria;
using Terraria.GameContent;
using Terraria.GameContent.Drawing;
using Terraria.ID;

namespace TouhouPets
{
    public static class DanmakuFightHelper
    {
        private static int playerA_Score;
        private static int playerB_Score;
        private static int round;
        private static int roundTimer;

        private static float myEssScale;
        private static int myEssDir;
        private static float textScale;
        private static float textAlpha;
        public static float DanmakuRingScale
        {
            get => myEssScale;
        }
        public static int PlayerA_Score
        {
            get => playerA_Score;
            set => playerA_Score = value;
        }
        public static int PlayerB_Score
        {
            get => playerB_Score;
            set => playerB_Score = value;
        }
        public static int Round
        {
            get => round;
            set => round = value;
        }
        public static int RoundTimer
        {
            get => roundTimer;
            set => roundTimer = value;
        }
        public static void UpdateDanmakuRingScale()
        {
            if (!Main.gamePaused)
            {
                myEssScale += myEssDir * 0.003f;
                if (myEssScale > 1f)
                {
                    myEssDir = -1;
                    myEssScale = 1f;
                }

                if (myEssScale < 0.85f)
                {
                    myEssDir = 1;
                    myEssScale = 0.85f;
                }
            }
        }
        public static void InitializeFightData()
        {
            PlayerA_Score = 0;
            PlayerB_Score = 0;
            Round = 0;
            RoundTimer = 0;
        }
        public static void FailEffect(this Projectile projectile)
        {
            int circle = 30;
            for (int i = 0; i < circle; i++)
            {
                Dust d = Dust.NewDustPerfect(projectile.Center, MyDustId.TrailingRed1, null, 100, default, Main.rand.NextFloat(1f, 1.7f));
                d.velocity = new Vector2(0, -Main.rand.NextFloat(2, 5)).RotatedBy(MathHelper.ToRadians(360 / circle * i));
            }
        }
        public static void DrawIndividualScore(this Projectile projectile, int score, bool win, int offsetY = 36)
        {
            string sourceText = "Win: " + score.ToString();
            Vector2 pos = new Vector2(projectile.Center.X - FontAssets.MouseText.Value.MeasureString(sourceText).X / 2, projectile.Center.Y + offsetY) - Main.screenPosition;
            Utils.DrawBorderStringFourWay(Main.spriteBatch, FontAssets.MouseText.Value, sourceText
                , pos.X, pos.Y, win ? Color.Yellow : Color.White, Color.Black, Vector2.Zero, 1f);
        }
        [Obsolete]
        public static void DrawBattleScore()
        {
            Player player = Main.LocalPlayer;
            string score = PlayerB_Score + " : " + PlayerA_Score;
            Color clr = Color.Yellow;
            Vector2 pos = new Vector2(player.Center.X - FontAssets.DeathText.Value.MeasureString(score).X / 2, player.Center.Y - Main.screenHeight / 2) - Main.screenPosition;
            Utils.DrawBorderStringFourWay(Main.spriteBatch, FontAssets.DeathText.Value, score
            , pos.X, pos.Y, clr, Color.Black, Vector2.Zero, 1f);

            score = "Round " + Round.ToString();
            clr = Color.AliceBlue;
            Utils.DrawBorderStringFourWay(Main.spriteBatch, FontAssets.MouseText.Value, score
            , pos.X + 14, pos.Y + 52, clr, Color.Black, Vector2.Zero, 1f);
        }
        public static void DrawBattleRound()
        {
            Player player = Main.LocalPlayer;
            string roundText = "Round " + Round.ToString();
            Color clr = Color.SkyBlue;
            int xOffset = 0;
            int yOffset = 0;

            if (RoundTimer <= 180)
            {
                if (!Main.gamePaused)
                {
                    float count = MathHelper.Clamp(RoundTimer / 60f, 0, 1);
                    textScale = MathHelper.SmoothStep(3, 1, count);
                    textAlpha = MathHelper.SmoothStep(0, 1, count);
                }
            }
            else if (RoundTimer >= 360)
            {
                if (!Main.gamePaused)
                {
                    float count = MathHelper.Clamp((RoundTimer - 360) / 20f, 0, 1);
                    textScale = MathHelper.SmoothStep(1, 3, count);
                    textAlpha = MathHelper.SmoothStep(1, 0, count);
                }
            }
            if (RoundTimer > 180 && RoundTimer <= 300)
            {
                roundText = "READY...";
                clr = Color.Yellow;
            }
            else if (RoundTimer > 300)
            {
                roundText = "FIGHT!";
                clr = Color.Red;
                xOffset = Main.rand.Next(-2, 2);
                yOffset = Main.rand.Next(-2, 2);
            }
            Vector2 pos = new Vector2(player.Center.X - FontAssets.DeathText.Value.MeasureString(roundText).X / 2 * textScale
                , player.Center.Y - FontAssets.DeathText.Value.MeasureString(roundText).Y / 2 * textScale - 202) - Main.screenPosition;
            if (RoundTimer > 180)
            {
                for (int i = 0; i < 4; i++)
                {
                    Utils.DrawBorderStringFourWay(Main.spriteBatch, FontAssets.DeathText.Value, roundText
                    , pos.X + Main.rand.Next(-5, 5), pos.Y + Main.rand.Next(-5, 5), clr * 0.4f * textAlpha, Color.Black * 0.2f * textAlpha, Vector2.Zero, textScale);
                }
            }
            Utils.DrawBorderStringFourWay(Main.spriteBatch, FontAssets.DeathText.Value, roundText
                , pos.X + xOffset, pos.Y + yOffset, clr * textAlpha, Color.Black * 0.5f * textAlpha, Vector2.Zero, textScale);
        }
        public static void HandleDanmakuCollide(this Projectile projectile)
        {
            foreach (Projectile p in Main.projectile)
            {
                if (p.TryGetGlobalProjectile(out TouhouPetGlobalProj gp) && projectile.TryGetGlobalProjectile(out TouhouPetGlobalProj gp2))
                {
                    if (p.active && gp.isADanmaku && p.type != projectile.type)
                    {
                        if (projectile.getRect().Intersects(p.getRect()) && p.timeLeft > 0
                            && gp.isDanmakuDestorible
                            && (gp.belongsToPlayerA && gp2.belongsToPlayerB || gp.belongsToPlayerB && gp2.belongsToPlayerA))
                        {
                            p.timeLeft = 0;
                            p.netUpdate = true;

                            var dustType = Main.rand.Next(4) switch
                            {
                                1 => MyDustId.TrailingYellow,
                                2 => MyDustId.TrailingGreen1,
                                3 => MyDustId.TrailingBlue,
                                _ => MyDustId.TrailingRed1,
                            };
                            int circle = Main.rand.Next(2, 5);
                            for (int i = 0; i < circle; i++)
                            {
                                Dust d = Dust.NewDustPerfect(p.Center, dustType, null, 100, default, Main.rand.NextFloat(0.7f, 1.7f));
                                d.velocity = new Vector2(0, -Main.rand.NextFloat(2, 4)).RotatedBy(MathHelper.ToRadians(360 / circle * i));
                            }
                            ParticleOrchestraSettings settings = new ParticleOrchestraSettings
                            {
                                PositionInWorld = p.Center,
                                MovementVector = Vector2.Zero
                            };
                            ParticleOrchestrator.SpawnParticlesDirect(ParticleOrchestraType.ShimmerArrow, settings);
                        }
                    }
                }
            }
        }
        public static void HandleHurt(this Projectile projectile, ref int health, bool isPlayerA = true)
        {
            foreach (Projectile p in Main.ActiveProjectiles)
            {
                if (p.TryGetGlobalProjectile(out TouhouPetGlobalProj gp))
                {
                    if (isPlayerA && gp.belongsToPlayerB || !isPlayerA && gp.belongsToPlayerA)
                    {
                        if (projectile.getRect().Intersects(p.getRect()) && p.timeLeft > 0)
                        {
                            int dmg = p.damage;
                            p.timeLeft = 0;
                            p.netUpdate = true;

                            CombatText.NewText(projectile.getRect(), Color.Orange, dmg, false, false);
                            AltVanillaFunction.PlaySound(SoundID.NPCHit1, projectile.position);

                            if (projectile.owner == Main.myPlayer)
                            {
                                health -= dmg;
                                projectile.netUpdate = true;
                            }
                        }
                    }
                }
            }
        }
        public static void ClearDanmaku(this Projectile projectile)
        {
            foreach (Projectile p in Main.ActiveProjectiles)
            {
                if (p.owner == projectile.owner && p.GetGlobalProjectile<TouhouPetGlobalProj>().isADanmaku)
                {
                    p.timeLeft = 0;
                    p.netUpdate = true;
                }
            }
        }
    }
}
