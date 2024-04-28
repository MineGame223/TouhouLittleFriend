using TouhouPets.Content.Projectiles.Pets;

namespace TouhouPets.Content.Buffs.PetBuffs
{
    public class MurasaBuff : BasicPetBuff
    {      
        public override int PetType => ProjectileType<Murasa>();
        public override bool LightPet => false;
    }
}
