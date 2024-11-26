using TouhouPets.Content.Projectiles.Pets;

namespace TouhouPets.Content.Buffs.PetBuffs
{
    public class AliceOldBuff : BasicPetBuff
    {      
        public override int PetType => ProjectileType<AliceOld>();
        public override bool LightPet => true;
    }
}
