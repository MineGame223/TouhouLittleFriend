using TouhouPets.Content.Projectiles.Pets;

namespace TouhouPets.Content.Buffs.PetBuffs
{
    public class PatchouliBuff : BasicPetBuff
    {      
        public override int PetType => ProjectileType<Patchouli>();
        public override bool LightPet => true;
    }
}
