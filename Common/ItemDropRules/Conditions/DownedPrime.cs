using Terraria;
using Terraria.GameContent.ItemDropRules;

namespace TouhouPets
{
    public class DownedPrime : IItemDropRuleCondition
    {
        public bool CanDrop(DropAttemptInfo info)
        {
            if (info.IsInSimulation)
            {
                return false;
            }
            return NPC.downedMechBoss3;
        }
        public bool CanShowItemDropInUI()
        {
            return true;
        }
        public string GetConditionDescription()
        {
            return "";
        }
    }
}
