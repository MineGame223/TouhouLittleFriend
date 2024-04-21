using Microsoft.Xna.Framework;
using Terraria;
using Terraria.GameContent;
using Terraria.GameContent.Drawing;
using Terraria.ID;

namespace TouhouPets
{
    public static class DanmakuFightHelper
    {
        private static int playerA_Source;
        private static int playerB_Source;
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
        public static int PlayerA_Source
        {
            get => playerA_Source; 
            set => playerA_Source = value;
        }
        public static int PlayerB_Source
        {
            get => playerB_Source; 
            set => playerB_Source = value;
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
            PlayerA_Source = 0;
            PlayerB_Source = 0;
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
        public static void DrawIndividualSource(this Projectile projectile, int source, int offsetY = 36)
        {
            string sourceText = "Win: " + source.ToString();
            Vector2 pos = new Vector2(projectile.Center.X - FontAssets.MouseText.Value.MeasureString(sourceText).X / 2, projectile.Center.Y + offsetY) - Main.screenPosition;
            Utils.DrawBorderStringFourWay(Main.spriteBatch, FontAssets.MouseText.Value, sourceText
                , pos.X, pos.Y, Color.White, Color.Black, Vector2.Zero, 1f);
        }
        public static void DrawBattleSource()
        {
            Player player = Main.LocalPlayer;
            string source = PlayerB_Source + " : " + PlayerA_Source;
            Color clr = Color.Yellow;
            Vector2 pos = new Vector2(player.Center.X - FontAssets.DeathText.Value.MeasureString(source).X / 2, player.Center.Y - Main.screenHeight / 2) - Main.screenPosition;
            Utils.DrawBorderStringFourWay(Main.spriteBatch, FontAssets.DeathText.Value, source
            , pos.X, pos.Y, clr, Color.Black, Vector2.Zero, 1f);

            source = "Round " + Round.ToString();
            clr = Color.AliceBlue;
            Utils.DrawBorderStringFourWay(Main.spriteBatch, FontAssets.MouseText.Value, source
            , pos.X + 14, pos.Y + 52, clr, Color.Black, Vector2.Zero, 1f);
        }
        public static void DrawBattleRound()
        {
            Player player = Main.LocalPlayer;
            string source = "Round " + Round.ToString();
            Color clr = Color.SkyBlue;
            int xOffset = 0;
            int yOffset = 0;

            if (RoundTimer <= 1)
            {
                textAlpha = 0;
                textScale = 3;
            }
            else if (RoundTimer <= 180 && RoundTimer > 1)
            {
                if (!Main.gamePaused)
                {
                    textAlpha += 0.02f;
                    textScale -= 0.03f;
                }
                textScale = MathHelper.Clamp(textScale, 1, 3);
                textAlpha = MathHelper.Clamp(textAlpha, 0, 1);
            }
            else if (RoundTimer >= 360)
            {
                if (!Main.gamePaused)
                {
                    textAlpha -= 0.04f;
                    textScale += 0.05f;
                }
                textScale = MathHelper.Clamp(textScale, 1, 5);
                textAlpha = MathHelper.Clamp(textAlpha, 0, 1);
            }
            else
            {
                textAlpha = 1;
                textScale = 1;
            }
            if (RoundTimer > 180 && RoundTimer <= 300)
            {
                source = "READY...";
                clr = Color.Yellow;
            }
            else if (RoundTimer > 300)
            {
                source = "FIGHT!";
                clr = Color.Red;
                xOffset = Main.rand.Next(-1, 1);
                yOffset = Main.rand.Next(-1, 1);
            }
            Vector2 pos = new Vector2(player.Center.X - FontAssets.DeathText.Value.MeasureString(source).X / 2 * textScale
                , player.Center.Y - FontAssets.DeathText.Value.MeasureString(source).Y / 2 * textScale - 202) - Main.screenPosition;
            if (RoundTimer > 180)
            {
                for (int i = 0; i < 4; i++)
                {
                    Utils.DrawBorderStringFourWay(Main.spriteBatch, FontAssets.DeathText.Value, source
                    , pos.X + Main.rand.Next(-5, 5), pos.Y + Main.rand.Next(-5, 5), clr * 0.4f * textAlpha, Color.Black * 0.2f * textAlpha, Vector2.Zero, textScale);
                }
            }
            Utils.DrawBorderStringFourWay(Main.spriteBatch, FontAssets.DeathText.Value, source
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
            foreach (Projectile p in Main.projectile)
            {
                if (p.TryGetGlobalProjectile(out TouhouPetGlobalProj gp))
                {
                    if (p.active && (isPlayerA && gp.belongsToPlayerB || !isPlayerA && gp.belongsToPlayerA))
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
    }
}
