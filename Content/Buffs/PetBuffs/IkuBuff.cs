using TouhouPets.Content.Projectiles.Pets;

namespace TouhouPets.Content.Buffs.PetBuffs
{
    public class IkuBuff : BasicPetBuff
    {      
        public override int PetType => ProjectileType<Iku>();
        public override bool LightPet => true;
    }
}
