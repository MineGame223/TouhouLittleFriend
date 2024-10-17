using TouhouPets.Content.Projectiles.Pets;

namespace TouhouPets.Content.Buffs.PetBuffs
{
    public class MomoyoBuff : BasicPetBuff
    {      
        public override int PetType => ProjectileType<Momoyo>();
        public override bool LightPet => false;
    }
}
