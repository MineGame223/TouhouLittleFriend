using TouhouPets.Content.Projectiles.Pets;

namespace TouhouPets.Content.Buffs.PetBuffs
{
    public class SakuyaBuff : BasicPetBuff
    {      
        public override int PetType => ProjectileType<Sakuya>();
        public override bool LightPet => false;
    }
}
