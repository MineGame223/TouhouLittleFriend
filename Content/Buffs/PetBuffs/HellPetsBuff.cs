using Terraria;
using TouhouPets.Content.Projectiles.Pets;

namespace TouhouPets.Content.Buffs.PetBuffs
{
    public class HellPetsBuff : BasicPetBuff
    {
        public override bool LightPet => true;
        public override void Update(Player player, ref int buffIndex)
        {
            player.SpawnPetAndSetBuffTime(buffIndex, ProjectileType<Rin>());
            player.SpawnPetAndSetBuffTime(buffIndex, ProjectileType<Utsuho>());
        }
    }
}
