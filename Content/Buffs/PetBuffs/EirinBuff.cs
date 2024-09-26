using TouhouPets.Content.Projectiles.Pets;

namespace TouhouPets.Content.Buffs.PetBuffs
{
    public class EirinBuff : BasicPetBuff
    {      
        public override int PetType => ProjectileType<Eirin>();
        public override bool LightPet => true;
    }
}
