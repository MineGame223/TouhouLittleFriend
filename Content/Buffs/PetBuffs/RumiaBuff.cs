using TouhouPets.Content.Projectiles.Pets;

namespace TouhouPets.Content.Buffs.PetBuffs
{
    public class RumiaBuff : BasicPetBuff
    {      
        public override int PetType => ProjectileType<Rumia>();
        public override bool LightPet => false;
    }
}
