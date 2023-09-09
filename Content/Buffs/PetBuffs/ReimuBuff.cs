using TouhouPets.Content.Projectiles.Pets;

namespace TouhouPets.Content.Buffs.PetBuffs
{
    public class ReimuBuff : BasicPetBuff
    {      
        public override int PetType => ProjectileType<Reimu>();
        public override bool LightPet => false;
    }
}
