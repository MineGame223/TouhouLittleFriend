using TouhouPets.Content.Projectiles.Pets;

namespace TouhouPets
{
    internal static class GensokyoSupport
    {
        public static void Setup(Mod gsk)
        {
            if (gsk == null)
            {
                return;
            }
            gsk.Call("TimestopImmuneProjectile", ProjectileType<Sakuya>());
        }
    }
}
