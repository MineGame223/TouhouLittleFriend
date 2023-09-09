using TouhouPets.Content.Projectiles.Pets;

namespace TouhouPets.Content.Buffs.PetBuffs
{
    public class SanaeBuff : BasicPetBuff
    {
        public override int PetType => ProjectileType<Sanae>();
        public override bool LightPet => true;
    }
}
