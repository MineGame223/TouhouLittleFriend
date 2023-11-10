using Terraria;
using TouhouPets.Content.Projectiles.Pets;

namespace TouhouPets.Content.Buffs.PetBuffs
{
    public class KomeijiBuff : BasicPetBuff
    {
        public override void Update(Player player, ref int buffIndex)
        {
            player.SpawnPetAndSetBuffTime(buffIndex, ProjectileType<Satori>());
            player.SpawnPetAndSetBuffTime(buffIndex, ProjectileType<Koishi>());
        }
    }
}
