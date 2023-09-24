using TouhouPets.Content.Projectiles.Pets;

namespace TouhouPets.Content.Buffs.PetBuffs
{
    public class YuyukoBuff : BasicPetBuff
    {      
        public override int PetType => ProjectileType<Yuyuko>();
        public override bool LightPet => false;
    }
}
