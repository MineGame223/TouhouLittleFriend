using Terraria;
using Terraria.ID;
using TouhouPets.Content.Buffs.PetBuffs;

namespace TouhouPets
{
    public class TouhouPetGlobalBuff : GlobalBuff
    {
        public override void Update(int type, Player player, ref int buffIndex)
        {
            if (type == BuffID.Gills)
            {
                if (player.HasBuff<MurasaBuff>() && Main.remixWorld)
                {
                    player.buffTime[buffIndex] -= 10;
                }
            }
        }
    }
}
