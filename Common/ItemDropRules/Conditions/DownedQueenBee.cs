using Terraria;
using Terraria.GameContent.ItemDropRules;

namespace TouhouPets
{
    public class DownedQueenBee : IItemDropRuleCondition
    {
        public bool CanDrop(DropAttemptInfo info)
        {
            if (info.IsInSimulation)
            {
                return false;
            }
            return NPC.downedQueenBee;
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
