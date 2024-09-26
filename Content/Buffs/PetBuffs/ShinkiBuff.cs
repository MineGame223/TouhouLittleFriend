using TouhouPets.Content.Projectiles.Pets;

namespace TouhouPets.Content.Buffs.PetBuffs
{
    public class ShinkiBuff : BasicPetBuff
    {      
        public override int PetType => ProjectileType<Shinki>();
        public override bool LightPet => true;
    }
}
