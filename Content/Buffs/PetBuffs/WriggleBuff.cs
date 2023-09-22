using TouhouPets.Content.Projectiles.Pets;

namespace TouhouPets.Content.Buffs.PetBuffs
{
    public class WriggleBuff : BasicPetBuff
    {      
        public override int PetType => ProjectileType<Wriggle>();
        public override bool LightPet => true;
    }
}
