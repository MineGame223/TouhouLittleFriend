using Microsoft.Xna.Framework;
using Terraria;
using Terraria.DataStructures;
using TouhouPets.Content.Items;

namespace TouhouPets.Common
{
    public class FinishingPlayer : ModPlayer
    {
        public override void CatchFish(FishingAttempt attempt, ref int itemDrop, ref int npcSpawn, ref AdvancedPopupRequest sonar, ref Vector2 sonarPosition)
        {
            if (Main.rand.Next(50) > attempt.fishingLevel && attempt.waterTilesCount < attempt.waterNeededToFish
                || attempt.veryrare)
            {
                if (Main.rand.NextBool(7))
                    itemDrop = ItemType<UselessBook>();
            }
        }
    }
}
