using Terraria;
using Terraria.ID;

namespace TouhouPets.Content.Buffs
{
    public class ConcertBuff : ModBuff
    {
        public override void SetStaticDefaults()
        {
            Main.buffNoTimeDisplay[Type] = true;
            Main.buffNoSave[Type] = true;
            BuffID.Sets.TimeLeftDoesNotDecrease[Type] = true;
        }
        public override bool RightClick(int buffIndex)
        {
            Main.LocalPlayer.GetModPlayer<ConcertPlayer>().ConcertStart = false;
            return true;
        }
    }
}
