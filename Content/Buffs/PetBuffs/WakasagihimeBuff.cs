using TouhouPets.Content.Projectiles.Pets;

namespace TouhouPets.Content.Buffs.PetBuffs
{
    public class WakasagihimeBuff : BasicPetBuff
    {      
        public override int PetType => ProjectileType<Wakasagihime>();
        public override bool LightPet => true;
    }
}
