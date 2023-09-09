using Terraria;
using Terraria.GameContent.ItemDropRules;

namespace TouhouPets
{
    public class DownedEoC : IItemDropRuleCondition
    {
        public bool CanDrop(DropAttemptInfo info)
        {
            if (info.IsInSimulation)
            {
                return false;
            }
            return NPC.downedBoss1;
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
