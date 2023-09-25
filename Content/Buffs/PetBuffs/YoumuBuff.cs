using TouhouPets.Content.Projectiles.Pets;

namespace TouhouPets.Content.Buffs.PetBuffs
{
    public class YoumuBuff : BasicPetBuff
    {      
        public override int PetType => ProjectileType<Youmu>();
        public override bool LightPet => true;
    }
}
