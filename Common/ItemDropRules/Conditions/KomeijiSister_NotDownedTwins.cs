using Terraria;
using Terraria.GameContent.ItemDropRules;
using Terraria.ID;

namespace TouhouPets
{
    public class KomeijiSister_NotDownedTwins : IItemDropRuleCondition
    {
        public bool CanDrop(DropAttemptInfo info)
        {
            if (info.IsInSimulation)
            {
                return false;
            }
            int type = NPCID.Retinazer;
            if (info.npc.type == NPCID.Retinazer)
                type = NPCID.Spazmatism;
            return !NPC.downedMechBoss2 && !NPC.AnyNPCs(type);
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
