using Terraria;
using Terraria.GameContent.ItemDropRules;

namespace TouhouPets
{
    public class NotDownedEvilBoss : IItemDropRuleCondition
    {
        public bool CanDrop(DropAttemptInfo info)
        {
            if (info.IsInSimulation)
            {
                return false;
            }
            return !NPC.downedBoss2;
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
