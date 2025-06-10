using Terraria;
using TouhouPets.Content.Projectiles.Pets;

namespace TouhouPets.Content.Buffs.PetBuffs
{
    public class StarBuff : BasicPetBuff
    {      
        public override int PetType => ProjectileType<StarPet>();
        public override bool LightPet => true;
        public override void Update(Player player, ref int buffIndex)
        {
            base.Update(player, ref buffIndex);
            player.GetModPlayer<SpecialAbilityPlayer>().treasureShine = true;
        }
    }
}
