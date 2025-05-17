using Terraria;

namespace TouhouPets.Content.Buffs
{
    public class ConcertBuff : ModBuff
    {
        public override void SetStaticDefaults()
        {
            Main.buffNoTimeDisplay[Type] = true;
            Main.buffNoSave[Type] = true;
            Main.persistentBuff[Type] = true;
        }
    }
}
