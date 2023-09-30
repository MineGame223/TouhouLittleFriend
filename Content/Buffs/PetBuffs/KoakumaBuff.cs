using TouhouPets.Content.Projectiles.Pets;

namespace TouhouPets.Content.Buffs.PetBuffs
{
    public class KoakumaBuff : BasicPetBuff
    {      
        public override int PetType => ProjectileType<Koakuma>();
        public override bool LightPet => false;
    }
}
