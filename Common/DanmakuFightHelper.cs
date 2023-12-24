using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;

namespace TouhouPets
{
    public static class DanmakuFightHelper
    {
        public static int PlayerA_Source;
        public static int PlayerB_Source;
        public static int Round;
        public static void InitializeFightData()
        {
            PlayerA_Source = 0;
            PlayerB_Source = 0;
            Round = 0;
        }
        public static void HandleDanmakuCollide(this Projectile projectile)
        {
            foreach (Projectile p in Main.projectile)
            {
                if (p.active && p.GetGlobalProjectile<TouhouPetGlobalProj>().isADanmaku && p.type != projectile.type)
                {
                    if (projectile.getRect().Intersects(p.getRect()) && p.timeLeft > 0
                        && p.GetGlobalProjectile<TouhouPetGlobalProj>().isDanmakuDestorible)
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
                            Dust d = Dust.NewDustPerfect(projectile.Center, dustType, null, 100, default, Main.rand.NextFloat(0.7f, 1.7f));
                            d.velocity = new Vector2(0, -Main.rand.NextFloat(2, 4)).RotatedBy(MathHelper.ToRadians(360 / circle * i));
                        }
                    }
                }
            }
        }
        public static void HandleHurting(this Projectile projectile, int hostileType, ref int health)
        {
            foreach (Projectile p in Main.projectile)
            {
                if (p.active && p.type == hostileType)
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
