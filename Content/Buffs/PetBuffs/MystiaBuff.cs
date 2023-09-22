using TouhouPets.Content.Projectiles.Pets;

namespace TouhouPets.Content.Buffs.PetBuffs
{
    public class MystiaBuff : BasicPetBuff
    {      
        public override int PetType => ProjectileType<Mystia>();
        public override bool LightPet => false;
    }
}
