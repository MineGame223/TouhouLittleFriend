using TouhouPets.Content.Projectiles.Pets;

namespace TouhouPets.Content.Buffs.PetBuffs
{
    public class LyricaBuff : BasicPetBuff
    {      
        public override int PetType => ProjectileType<Lyrica>();
        public override bool LightPet => true;
    }
}
