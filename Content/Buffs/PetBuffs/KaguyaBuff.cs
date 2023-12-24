using TouhouPets.Content.Projectiles.Pets;

namespace TouhouPets.Content.Buffs.PetBuffs
{
    public class KaguyaBuff : BasicPetBuff
    {      
        public override int PetType => ProjectileType<Kaguya>();
        public override bool LightPet => false;
    }
}
