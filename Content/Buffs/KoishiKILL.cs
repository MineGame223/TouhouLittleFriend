using Terraria;

namespace TouhouPets.Content.Buffs
{
    public class KoishiKILL : ModBuff
    {
        public override void SetStaticDefaults()
        {
            Main.buffNoTimeDisplay[Type] = true;
            Main.buffNoSave[Type] = true;
        }
        public override bool RightClick(int buffIndex)
        {
            return false;
        }
    }
}
