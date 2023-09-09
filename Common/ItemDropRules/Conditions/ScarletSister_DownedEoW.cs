using Terraria;
using Terraria.GameContent.ItemDropRules;

namespace TouhouPets
{
    public class ScarletSister_DownedEoW : IItemDropRuleCondition
    {
        public bool CanDrop(DropAttemptInfo info)
        {
            if (info.IsInSimulation)
            {
                return false;
            }
            return NPC.downedBoss2 && info.npc.boss;
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
