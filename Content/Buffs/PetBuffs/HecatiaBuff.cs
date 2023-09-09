using Terraria;
using TouhouPets.Content.Projectiles.Pets;

namespace TouhouPets.Content.Buffs.PetBuffs
{
    public class HecatiaBuff : BasicPetBuff
    {      
        public override int PetType => ProjectileType<Hecatia>();
        public override bool LightPet => false;
        public override void Update(Player player, ref int buffIndex)
        {
            base.Update(player, ref buffIndex);
            int type = ProjectileType<Piece>();
            bool flag = player.ownedProjectileCounts[type] <= 0;
            if (flag)
            {
                if (player.whoAmI == Main.myPlayer)
                    Projectile.NewProjectile(player.GetSource_Buff(buffIndex), player.position.X + player.width / 2, player.position.Y + player.height / 2, 0f, 0f, type, 0, 0f, player.whoAmI, 0f, 0f);
            }
        }
    }
}
