using TouhouPets.Content.Projectiles.Pets;

namespace TouhouPets.Content.Buffs.PetBuffs
{
    public class YukaBuff : BasicPetBuff
    {      
        public override int PetType => ProjectileType<Yuka>();
        public override bool LightPet => false;
    }
}
