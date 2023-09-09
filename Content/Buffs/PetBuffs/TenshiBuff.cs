using TouhouPets.Content.Projectiles.Pets;

namespace TouhouPets.Content.Buffs.PetBuffs
{
    public class TenshiBuff : BasicPetBuff
    {      
        public override int PetType => ProjectileType<Tenshi>();
        public override bool LightPet => false;
    }
}
