using Terraria;

namespace TouhouPets.Content.Buffs
{
    public class KoishiCalling : ModBuff
    {
        public override void SetStaticDefaults()
        {
            Main.buffNoTimeDisplay[Type] = true;
            Main.debuff[Type] = true;
        }
        public override bool RightClick(int buffIndex)
        {
            return false;
        }
    }
}
