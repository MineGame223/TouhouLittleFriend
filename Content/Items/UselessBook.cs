using Terraria;
using Terraria.ID;
using TouhouPets.Content.Items.PetItems;

namespace TouhouPets.Content.Items
{
    public class UselessBook : ModItem
    {
        public override void SetStaticDefaults()
        {
            ItemID.Sets.ShimmerTransformToItem[Type] = ItemType<PatchouliMoon>();
        }
        public override void SetDefaults()
        {
            Item.width = 28;
            Item.height = 32;
            Item.rare = ItemRarityID.Gray;
            Item.value = 0;
        }
    }
}
