using Terraria;
using Terraria.ID;

namespace TouhouPets.Content.Items
{
    public class CustomSupportStick : SupportStick
    {
        public override void SetStaticDefaults()
        {
            ItemID.Sets.ShimmerTransformToItem[Type] = ItemType<SupportStick>();
        }
        public override void SetDefaults()
        {
            base.SetDefaults();
            Item.rare = ItemRarityID.Green;
        }
        public override bool CanUseItem(Player player)
        {
            ConcertPlayer bp = player.GetModPlayer<ConcertPlayer>();
            if (player.altFunctionUse == 2)
            {
                if (bp.manualStartBand && bp.customMode)
                {
                    bp.musicRerolled = false;
                }
            }
            else
            {
                if (bp.prismriverBand)
                {
                    bp.customMode = !bp.customMode;
                }
                if (!bp.customMode)
                {
                    CustomMusicManager.Stop();
                }
                else
                {
                    bp.musicRerolled = false;
                }
            }
            return true;
        }
    }
}
