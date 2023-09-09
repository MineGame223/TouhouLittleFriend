using Terraria;
using TouhouPets.Content.Buffs.PetBuffs;

namespace TouhouPets
{
    public class LuckPlayer : ModPlayer
    {
        public override void ModifyLuck(ref float luck)
        {
            if (Player.HasBuff(BuffType<HinaBuff>()))
            {
                if (luck < 0)
                {
                    luck = 0;
                }
            }
            if (Player.HasBuff(BuffType<TenshiBuff>()))
            {
                luck += 0.5f;
            }
        }
    }
}
