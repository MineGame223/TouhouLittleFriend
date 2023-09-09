using TouhouPets.Content.Projectiles.Pets;

namespace TouhouPets.Content.Buffs.PetBuffs
{
    public class RinBuff : BasicPetBuff
    {      
        public override int PetType => ProjectileType<Rin>();
        public override bool LightPet => false;
    }
}
