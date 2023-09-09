using TouhouPets.Content.Projectiles.Pets;

namespace TouhouPets.Content.Buffs.PetBuffs
{
    public class JunkoBuff : BasicPetBuff
    {      
        public override int PetType => ProjectileType<Junko>();
        public override bool LightPet => true;
    }
}
