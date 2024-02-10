using TouhouPets.Content.Projectiles.Pets;

namespace TouhouPets.Content.Buffs.PetBuffs
{
    public class NitoriBuff : BasicPetBuff
    {      
        public override int PetType => ProjectileType<Nitori>();
        public override bool LightPet => true;
    }
}
