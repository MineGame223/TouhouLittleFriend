using TouhouPets.Content.Projectiles.Pets;

namespace TouhouPets.Content.Buffs.PetBuffs
{
    public class SatoriBuff : BasicPetBuff
    {
        public override int PetType => ProjectileType<Satori>();
        public override bool LightPet => true;
    }
}
