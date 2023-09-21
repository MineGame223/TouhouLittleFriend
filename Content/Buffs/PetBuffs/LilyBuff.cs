using TouhouPets.Content.Projectiles.Pets;

namespace TouhouPets.Content.Buffs.PetBuffs
{
    public class LilyBuff : BasicPetBuff
    {      
        public override int PetType => ProjectileType<Lily>();
        public override bool LightPet => true;
    }
}
