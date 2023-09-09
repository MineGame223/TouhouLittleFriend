using TouhouPets.Content.Projectiles.Pets;

namespace TouhouPets.Content.Buffs.PetBuffs
{
    public class FlandreBuff : BasicPetBuff
    {
        public override int PetType => ProjectileType<Flandre>();
        public override bool LightPet => true;
    }
}
