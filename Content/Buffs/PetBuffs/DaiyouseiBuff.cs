using TouhouPets.Content.Projectiles.Pets;

namespace TouhouPets.Content.Buffs.PetBuffs
{
    public class DaiyouseiBuff : BasicPetBuff
    {      
        public override int PetType => ProjectileType<Daiyousei>();
        public override bool LightPet => false;
    }
}
