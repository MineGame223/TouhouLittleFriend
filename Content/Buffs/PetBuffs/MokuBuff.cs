using TouhouPets.Content.Projectiles.Pets;

namespace TouhouPets.Content.Buffs.PetBuffs
{
    public class MokuBuff : BasicPetBuff
    {      
        public override int PetType => ProjectileType<Moku>();
        public override bool LightPet => true;
    }
}
