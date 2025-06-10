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
            if (!GetInstance<PetObtainConfig>().ObtainPetByFishing)
                return;

            bool junkCondition = Main.rand.Next(50) > attempt.fishingLevel && attempt.waterTilesCount < attempt.waterNeededToFish;
            bool enoughWater = attempt.waterTilesCount > 1000;

            if (attempt.inLava || attempt.inHoney)
                return;

            if (junkCondition || attempt.veryrare)
            {
                if (Main.rand.NextBool(100))
                    itemDrop = ItemType<UselessBook>();
            }
            if (attempt.legendary)
            {
                if (Player.ZoneForest)
                {
                    if (Main.rand.NextBool(8))
                    {
                        itemDrop = ItemType<WakasagihimeFishingRod>();
                        return;
                    }

                    if (Main.rand.NextBool(8))
                    {
                        itemDrop = ItemType<HinaDoll>();
                        return;
                    }
                }
                if (Player.ZoneBeach)
                {
                    if (Main.rand.NextBool(20) && enoughWater)
                    {
                        itemDrop = ItemType<IkuOarfish>();
                        return;
                    }
                }
            }
        }
    }
}
