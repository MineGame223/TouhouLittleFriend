using TouhouPets.Content.Projectiles.Pets;

namespace TouhouPets.Content.Buffs.PetBuffs
{
    public class DoremyBuff : BasicPetBuff
    {      
        public override int PetType => ProjectileType<Doremy>();
        public override bool LightPet => true;
    }
}
