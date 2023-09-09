using TouhouPets.Content.Projectiles.Pets;

namespace TouhouPets.Content.Buffs.PetBuffs
{
    public class CirnoBuff : BasicPetBuff
    {      
        public override int PetType => ProjectileType<Cirno>();
        public override bool LightPet => true;
    }
}
