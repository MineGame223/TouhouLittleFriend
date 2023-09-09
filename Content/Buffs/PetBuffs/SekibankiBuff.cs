using TouhouPets.Content.Projectiles.Pets;

namespace TouhouPets.Content.Buffs.PetBuffs
{
    public class SekibankiBuff : BasicPetBuff
    {      
        public override int PetType => ProjectileType<Sekibanki>();
        public override bool LightPet => false;
    }
}
