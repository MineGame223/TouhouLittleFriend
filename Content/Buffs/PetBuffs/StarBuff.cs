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
            player.spelunkerTimer++;
            if (player.spelunkerTimer >= 10)
            {
                player.spelunkerTimer = 0;
                Main.instance.SpelunkerProjectileHelper.AddSpotToCheck(player.Center);
            }
        }
    }
}
