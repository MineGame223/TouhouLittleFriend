using Terraria.GameContent.ItemDropRules;

namespace TouhouPets
{
    public static class ItemDropHelper
    {
        /// <summary>
        /// 添加掉落：Common
        /// </summary>
        /// <param name="loot"></param>
        /// <param name="itemID"></param>
        /// <param name="dropRate"></param>
        /// <param name="minQuantity"></param>
        /// <param name="maxQuantity"></param>
        /// <returns></returns>
        public static IItemDropRule Add(this ILoot loot, int itemID, int dropRate = 1, int minQuantity = 1, int maxQuantity = 1)
        {
            if (minQuantity > maxQuantity)
            {
                maxQuantity = minQuantity;
            }
            return loot.Add(ItemDropRule.Common(itemID, dropRate, minQuantity, maxQuantity));
        }
        /// <summary>
        /// 添加掉落：ByCondition
        /// </summary>
        /// <param name="loot"></param>
        /// <param name="condition"></param>
        /// <param name="itemID"></param>
        /// <param name="dropRate"></param>
        /// <param name="minQuantity"></param>
        /// <param name="maxQuantity"></param>
        /// <returns></returns>
        public static IItemDropRule Add(this ILoot loot, IItemDropRuleCondition condition, int itemID, int dropRate = 1, int minQuantity = 1, int maxQuantity = 1)
        {
            if (minQuantity > maxQuantity)
            {
                maxQuantity = minQuantity;
            }
            return loot.Add(ItemDropRule.ByCondition(condition, itemID, dropRate, minQuantity, maxQuantity));
        }
        /// <summary>
        /// 添加掉落：东方宠物专用
        /// </summary>
        /// <param name="loot"></param>
        /// <param name="condition1"></param>
        /// <param name="condition2"></param>
        /// <param name="itemID"></param>
        /// <param name="dropRate"></param>
        public static void Add(this ILoot loot, IItemDropRuleCondition condition1, IItemDropRuleCondition condition2, int itemID, int dropRate = 1)
        {
            loot.Add(ItemDropRule.ByCondition(condition1, itemID, 1));
            loot.Add(ItemDropRule.ByCondition(condition2, itemID, dropRate));
        }
        /// <summary>
        /// 添加掉落：东方宠物专用 + OneFromOptions
        /// </summary>
        /// <param name="loot"></param>
        /// <param name="condition1"></param>
        /// <param name="condition2"></param>
        /// <param name="itemID"></param>
        /// <param name="dropRate"></param>
        public static void Add(this ILoot loot, IItemDropRuleCondition condition1, IItemDropRuleCondition condition2, int dropRate = 1, params int[] itemID)
        {
            IItemDropRule rule1 = loot.Add(new LeadingConditionRule(condition1));
            IItemDropRule rule2 = loot.Add(new LeadingConditionRule(condition2));
            foreach (int i in itemID)
            {
                rule1.OnSuccess(ItemDropRule.Common(i));
            }
            rule2.OnSuccess(ItemDropRule.OneFromOptions(dropRate, itemID));
        }
    }
}
