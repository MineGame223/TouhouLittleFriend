using TouhouPets.Content.Projectiles.Pets;

namespace TouhouPets.Content.Buffs.PetBuffs
{
    public class AyaBuff : BasicPetBuff
    {      
        public override int PetType => ProjectileType<Aya>();
        public override bool LightPet => false;
    }
}
