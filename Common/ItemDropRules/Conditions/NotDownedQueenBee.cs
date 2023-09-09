using Terraria;
using Terraria.GameContent.ItemDropRules;

namespace TouhouPets
{
    public class NotDownedQueenBee : IItemDropRuleCondition
    {
        public bool CanDrop(DropAttemptInfo info)
        {
            if (info.IsInSimulation)
            {
                return false;
            }
            return !NPC.downedQueenBee;
        }
        public bool CanShowItemDropInUI()
        {
            return false;
        }
        public string GetConditionDescription()
        {
            return "";
        }
    }
}
