using TouhouPets.Content.Projectiles.Pets;

namespace TouhouPets.Content.Buffs.PetBuffs
{
    public class HinaBuff : BasicPetBuff
    {      
        public override int PetType => ProjectileType<Hina>();
        public override bool LightPet => false;
    }
}
