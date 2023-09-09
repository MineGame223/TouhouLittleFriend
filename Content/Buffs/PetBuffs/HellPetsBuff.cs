using Terraria;
using TouhouPets.Content.Projectiles.Pets;

namespace TouhouPets.Content.Buffs.PetBuffs
{
    public class HellPetsBuff : BasicPetBuff
    {
        public override bool LightPet => true;
        public override void Update(Player player, ref int buffIndex)
        {
            int type1 = ProjectileType<Rin>();
            int type2 = ProjectileType<Utsuho>();
            player.buffTime[buffIndex] = 18000;
            bool flag = player.ownedProjectileCounts[type1] <= 0;
            if (flag)
            {
                if (player.whoAmI == Main.myPlayer)
                    Projectile.NewProjectile(player.GetSource_Buff(buffIndex), player.position.X + player.width / 2, player.position.Y + player.height / 2, 0f, 0f, type1, 0, 0f, player.whoAmI, 0f, 0f);
            }
            bool flag2 = player.ownedProjectileCounts[type2] <= 0;
            if (flag2)
            {
                if (player.whoAmI == Main.myPlayer)
                    Projectile.NewProjectile(player.GetSource_Buff(buffIndex), player.position.X + player.width / 2, player.position.Y + player.height / 2, 0f, 0f, type2, 0, 0f, player.whoAmI, 0f, 0f);
            }
        }
    }
}
