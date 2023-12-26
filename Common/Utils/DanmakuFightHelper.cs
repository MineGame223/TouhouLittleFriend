using Microsoft.Xna.Framework;
using Terraria;
using Terraria.GameContent;
using Terraria.ID;

namespace TouhouPets
{
    public static class DanmakuFightHelper
    {
        public static int PlayerA_Source;
        public static int PlayerB_Source;
        public static int Round;
        public static int RoundTimer;

        private static float myEssScale;
        private static int myEssDir;
        public static float DanmakuRingScale { get => myEssScale; }
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
            int circle = 50;
            for (int i = 0; i < circle; i++)
            {
                Dust d = Dust.NewDustPerfect(projectile.Center, MyDustId.TrailingRed1, null, 100, default, Main.rand.NextFloat(0.7f, 1.7f));
                d.velocity = new Vector2(0, -Main.rand.NextFloat(3, 8)).RotatedBy(MathHelper.ToRadians(360 / circle * i));
            }
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
            if (RoundTimer > 180 && RoundTimer <= 300)
            {
                source = "READY...";
                clr = Color.Yellow;
            }
            else if (RoundTimer > 300)
            {
                source = "FIGHT!";
                clr = Color.Red;
            }
            Vector2 pos = new Vector2(player.Center.X - FontAssets.DeathText.Value.MeasureString(source).X / 2, player.Center.Y - 232) - Main.screenPosition;
            Utils.DrawBorderStringFourWay(Main.spriteBatch, FontAssets.DeathText.Value, source
            , pos.X, pos.Y, clr, Color.Black, Vector2.Zero, 1f);
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
                            int circle = 6;
                            for (int i = 0; i < circle; i++)
                            {
                                Dust d = Dust.NewDustPerfect(p.Center, dustType, null, 100, default, Main.rand.NextFloat(0.7f, 1.7f));
                                d.velocity = new Vector2(0, -Main.rand.NextFloat(2, 4)).RotatedBy(MathHelper.ToRadians(360 / circle * i));
                            }
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
