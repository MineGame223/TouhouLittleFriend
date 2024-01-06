using Microsoft.Xna.Framework;
using Terraria;
using Terraria.DataStructures;
using TouhouPets.Content.Items;
using TouhouPets.Content.Items.PetItems;

namespace TouhouPets.Common
{
    public class FinishingPlayer : ModPlayer
    {
        public override void CatchFish(FishingAttempt attempt, ref int itemDrop, ref int npcSpawn, ref AdvancedPopupRequest sonar, ref Vector2 sonarPosition)
        {
            if (attempt.inLava || attempt.inHoney)
                return;

            if (Main.rand.Next(50) > attempt.fishingLevel && attempt.waterTilesCount < attempt.waterNeededToFish
                || attempt.rare)
            {
                if (Main.rand.NextBool(7))
                    itemDrop = ItemType<UselessBook>();
            }

            if (!GetInstance<PetObtainConfig>().ObtainPetByFishing)
                return;

            if (attempt.rare)
            {
                if (Main.rand.NextBool(5))
                    itemDrop = ItemType<WakasagihimeFishingRod>();

                if (Main.rand.NextBool(5))
                    itemDrop = ItemType<HinaDoll>();
            }
            if (attempt.legendary)
            {
                if (Main.rand.NextBool(3))
                    itemDrop = ItemType<IkuOarfish>();
            }
        }
    }
}
