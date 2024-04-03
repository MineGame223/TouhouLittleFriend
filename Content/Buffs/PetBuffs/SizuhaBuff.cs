using TouhouPets.Content.Projectiles.Pets;

namespace TouhouPets.Content.Buffs.PetBuffs
{
    public class SizuhaBuff : BasicPetBuff
    {      
        public override int PetType => ProjectileType<Sizuha>();
        public override bool LightPet => true;
    }
}
