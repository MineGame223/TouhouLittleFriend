using Terraria;
using Terraria.GameContent.ItemDropRules;

namespace TouhouPets
{
    public class NotDownedEoL : IItemDropRuleCondition
    {
        public bool CanDrop(DropAttemptInfo info)
        {
            if (info.IsInSimulation)
            {
                return false;
            }
            return !NPC.downedEmpressOfLight;
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
