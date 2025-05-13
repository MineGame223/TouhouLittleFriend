using Terraria;
using TouhouPets.Content.Projectiles.Pets;

namespace TouhouPets.Content.Buffs.PetBuffs
{
    public class PoltergeistBuff : BasicPetBuff
    {
        public override bool LightPet => true;
        public override void Update(Player player, ref int buffIndex)
        {
            player.SpawnPetAndSetBuffTime(buffIndex, ProjectileType<Lunasa>());
            player.SpawnPetAndSetBuffTime(buffIndex, ProjectileType<Merlin>());
            player.SpawnPetAndSetBuffTime(buffIndex, ProjectileType<Lyrica>());
        }
    }
}
