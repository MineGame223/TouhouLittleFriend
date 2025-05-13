using TouhouPets.Content.Projectiles.Pets;

namespace TouhouPets.Content.Buffs.PetBuffs
{
    public class LunasaBuff : BasicPetBuff
    {      
        public override int PetType => ProjectileType<Lunasa>();
        public override bool LightPet => true;
    }
}
