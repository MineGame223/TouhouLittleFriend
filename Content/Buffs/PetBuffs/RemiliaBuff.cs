using TouhouPets.Content.Projectiles.Pets;

namespace TouhouPets.Content.Buffs.PetBuffs
{
    public class RemiliaBuff : BasicPetBuff
    {      
        public override int PetType => ProjectileType<Remilia>();
        public override bool LightPet => false;
    }
}
