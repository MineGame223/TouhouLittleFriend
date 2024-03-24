using TouhouPets.Content.Projectiles.Pets;

namespace TouhouPets.Content.Buffs.PetBuffs
{
    public class SunnyBuff : BasicPetBuff
    {      
        public override int PetType => ProjectileType<Sunny>();
        public override bool LightPet => true;
    }
}
