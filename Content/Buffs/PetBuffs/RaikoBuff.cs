using TouhouPets.Content.Projectiles.Pets;

namespace TouhouPets.Content.Buffs.PetBuffs
{
    public class RaikoBuff : BasicPetBuff
    {      
        public override int PetType => ProjectileType<Raiko>();
        public override bool LightPet => false;
    }
}
