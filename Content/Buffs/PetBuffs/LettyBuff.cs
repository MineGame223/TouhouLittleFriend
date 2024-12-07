using TouhouPets.Content.Projectiles.Pets;

namespace TouhouPets.Content.Buffs.PetBuffs
{
    public class LettyBuff : BasicPetBuff
    {      
        public override int PetType => ProjectileType<Letty>();
        public override bool LightPet => false;
    }
}
