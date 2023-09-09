using Terraria;
using Terraria.GameContent.ItemDropRules;

namespace TouhouPets
{
    public class NotDownedCultist : IItemDropRuleCondition
    {
        public bool CanDrop(DropAttemptInfo info)
        {
            if (info.IsInSimulation)
            {
                return false;
            }
            return !NPC.downedAncientCultist;
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
