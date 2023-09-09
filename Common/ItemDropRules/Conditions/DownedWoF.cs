using Terraria;
using Terraria.GameContent.ItemDropRules;

namespace TouhouPets
{
    public class DownedWoF : IItemDropRuleCondition
    {
        public bool CanDrop(DropAttemptInfo info)
        {
            if (info.IsInSimulation)
            {
                return false;
            }
            return Main.hardMode;
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
