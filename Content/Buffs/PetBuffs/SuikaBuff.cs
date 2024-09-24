using TouhouPets.Content.Projectiles.Pets;

namespace TouhouPets.Content.Buffs.PetBuffs
{
    public class SuikaBuff : BasicPetBuff
    {      
        public override int PetType => ProjectileType<Suika>();
        public override bool LightPet => false;
    }
}
