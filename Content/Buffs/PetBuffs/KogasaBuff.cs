using TouhouPets.Content.Projectiles.Pets;

namespace TouhouPets.Content.Buffs.PetBuffs
{
    public class KogasaBuff : BasicPetBuff
    {      
        public override int PetType => ProjectileType<Kogasa>();
        public override bool LightPet => false;
    }
}
