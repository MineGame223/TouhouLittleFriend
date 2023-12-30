using Terraria;
using TouhouPets.Content.Projectiles.Pets;

namespace TouhouPets.Content.Buffs.PetBuffs
{
    public class ScarletBuff : BasicPetBuff
    {
        public override void Update(Player player, ref int buffIndex)
        {
            player.SpawnPetAndSetBuffTime(buffIndex, ProjectileType<Remilia>());
            player.SpawnPetAndSetBuffTime(buffIndex, ProjectileType<Flandre>());
            player.SpawnPetAndSetBuffTime(buffIndex, ProjectileType<Sakuya>());
            player.SpawnPetAndSetBuffTime(buffIndex, ProjectileType<Meirin>());
            player.SpawnPetAndSetBuffTime(buffIndex, ProjectileType<Patchouli>());
            player.SpawnPetAndSetBuffTime(buffIndex, ProjectileType<Koakuma>());
        }
    }
}
