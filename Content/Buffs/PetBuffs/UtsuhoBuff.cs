using TouhouPets.Content.Projectiles.Pets;

namespace TouhouPets.Content.Buffs.PetBuffs
{
    public class UtsuhoBuff : BasicPetBuff
    {      
        public override int PetType => ProjectileType<Utsuho>();
        public override bool LightPet => true;
    }
}
