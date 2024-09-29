using TouhouPets.Content.Projectiles.Pets;

namespace TouhouPets.Content.Buffs.PetBuffs
{
    public class KokoroBuff : BasicPetBuff
    {
        public override int PetType => ProjectileType<Kokoro>();
        public override bool LightPet => true;
    }
}
