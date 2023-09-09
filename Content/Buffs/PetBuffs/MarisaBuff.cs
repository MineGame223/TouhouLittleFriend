using TouhouPets.Content.Projectiles.Pets;

namespace TouhouPets.Content.Buffs.PetBuffs
{
    public class MarisaBuff : BasicPetBuff
    {      
        public override int PetType => ProjectileType<Marisa>();
        public override bool LightPet => true;
    }
}
