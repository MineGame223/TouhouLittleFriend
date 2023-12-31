using TouhouPets.Content.Projectiles.Pets;

namespace TouhouPets.Content.Buffs.PetBuffs
{
    public class MeirinBuff : BasicPetBuff
    {      
        public override int PetType => ProjectileType<Meirin>();
        public override bool LightPet => true;
    }
}
