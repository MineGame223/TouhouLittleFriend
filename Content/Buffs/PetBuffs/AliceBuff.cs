using TouhouPets.Content.Projectiles.Pets;

namespace TouhouPets.Content.Buffs.PetBuffs
{
    public class AliceBuff : BasicPetBuff
    {      
        public override int PetType => ProjectileType<Alice>();
        public override bool LightPet => false;
    }
}
