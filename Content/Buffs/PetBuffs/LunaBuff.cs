using TouhouPets.Content.Projectiles.Pets;

namespace TouhouPets.Content.Buffs.PetBuffs
{
    public class LunaBuff : BasicPetBuff
    {      
        public override int PetType => ProjectileType<Luna>();
        public override bool LightPet => true;
    }
}
