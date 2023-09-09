using Terraria;
using Terraria.GameContent.ItemDropRules;

namespace TouhouPets
{
    public class DownedDeerclops : IItemDropRuleCondition
    {
        public bool CanDrop(DropAttemptInfo info)
        {
            if (info.IsInSimulation)
            {
                return false;
            }
            return NPC.downedDeerclops;
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
