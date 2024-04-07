using Terraria;

namespace TouhouPets.Content.Buffs.PetBuffs
{
    public class StarBuff : BasicPetBuff
    {      
        public override int PetType => ProjectileType<Projectiles.Pets.Star>();
        public override bool LightPet => true;
        public override void Update(Player player, ref int buffIndex)
        {
            base.Update(player, ref buffIndex);
            player.GetModPlayer<TouhouPetPlayer>().treasureShine = true;
        }
    }
}
