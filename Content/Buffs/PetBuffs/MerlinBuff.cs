using TouhouPets.Content.Projectiles.Pets;

namespace TouhouPets.Content.Buffs.PetBuffs
{
    public class MerlinBuff : BasicPetBuff
    {      
        public override int PetType => ProjectileType<Merlin>();
        public override bool LightPet => true;
    }
}
