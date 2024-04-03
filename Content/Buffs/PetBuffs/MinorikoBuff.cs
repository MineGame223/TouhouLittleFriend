using TouhouPets.Content.Projectiles.Pets;

namespace TouhouPets.Content.Buffs.PetBuffs
{
    public class MinorikoBuff : BasicPetBuff
    {      
        public override int PetType => ProjectileType<Minoriko>();
        public override bool LightPet => false;
    }
}
