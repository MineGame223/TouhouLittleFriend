using TouhouPets.Content.Projectiles.Pets;

namespace TouhouPets.Content.Buffs.PetBuffs
{
    public class KoishiBuff : BasicPetBuff
    {      
        public override int PetType => ProjectileType<Koishi>();
        public override bool LightPet => false;
    }
}
