using TouhouPets.Content.Projectiles.Pets;

namespace TouhouPets.Content.Buffs.PetBuffs
{
    public class ReisenBuff : BasicPetBuff
    {      
        public override int PetType => ProjectileType<Reisen>();
        public override bool LightPet => false;
    }
}
