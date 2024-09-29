using TouhouPets.Content.Projectiles.Pets;

namespace TouhouPets.Content.Buffs.PetBuffs
{
    public class RukotoBuff : BasicPetBuff
    {      
        public override int PetType => ProjectileType<Rukoto>();
        public override bool LightPet => false;
    }
}
