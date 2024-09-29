using Terraria;
using TouhouPets.Content.Projectiles.Pets;

namespace TouhouPets.Content.Buffs.PetBuffs
{
    public class EienteiBuff : BasicPetBuff
    {
        public override void Update(Player player, ref int buffIndex)
        {           
            player.SpawnPetAndSetBuffTime(buffIndex, ProjectileType<Kaguya>());
            player.SpawnPetAndSetBuffTime(buffIndex, ProjectileType<Eirin>());
            player.SpawnPetAndSetBuffTime(buffIndex, ProjectileType<Reisen>());

            player.SpawnPetAndSetBuffTime(buffIndex, ProjectileType<Moku>());
            player.SpawnPetAndSetBuffTime(buffIndex, ProjectileType<Keine>());
        }
    }
}
