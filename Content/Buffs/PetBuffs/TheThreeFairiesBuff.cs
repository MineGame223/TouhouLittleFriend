using Terraria;
using TouhouPets.Content.Projectiles.Pets;

namespace TouhouPets.Content.Buffs.PetBuffs
{
    public class TheThreeFairiesBuff : BasicPetBuff
    {
        public override bool LightPet => true;
        public override void Update(Player player, ref int buffIndex)
        {
            player.SpawnPetAndSetBuffTime(buffIndex, ProjectileType<Sunny>());
            player.SpawnPetAndSetBuffTime(buffIndex, ProjectileType<Luna>());
            player.SpawnPetAndSetBuffTime(buffIndex, ProjectileType<Projectiles.Pets.StarPet>());

            player.GetModPlayer<SpecialAbilityPlayer>().treasureShine = true;
        }
    }
}
