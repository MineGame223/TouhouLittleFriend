using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ModLoader.IO;
using TouhouPets.Content.Buffs.PetBuffs;
using TouhouPets.Content.Items.PetItems;
using TouhouPets.Content.NPCs;

namespace TouhouPets
{
    public class TouhouPetPlayer : ModPlayer
    {
        public int koakumaNumber;
        public bool yukariLicenseLeft;
        public bool yukariLicenseRight;

        public int purchaseValueCount;
        public int totalPurchaseValueCount;
        private void ChangePurchaseCount(int amount)
        {
            totalPurchaseValueCount += amount;
            purchaseValueCount += amount;
        }
        private void CommonResetUpdate()
        {
            yukariLicenseLeft = false;
            yukariLicenseRight = false;
        }
        public override void ResetEffects()
        {
            purchaseValueCount = (int)MathHelper.Clamp(purchaseValueCount, 0, int.MaxValue);
            totalPurchaseValueCount = (int)MathHelper.Clamp(totalPurchaseValueCount, 0, int.MaxValue);
            CommonResetUpdate();
        }
        public override void UpdateDead()
        {
            CommonResetUpdate();
        }
        public override bool CanBuyItem(NPC vendor, Item[] shopInventory, Item item)
        {
            if (vendor.type == NPCType<YukariPortal>())
            {
                if (!item.buyOnce)
                {
                    ChangePurchaseCount(item.value);
                }
            }
            return base.CanBuyItem(vendor, shopInventory, item);
        }
        public override void PostBuyItem(NPC vendor, Item[] shopInventory, Item item)
        {
            if (vendor.type == NPCType<YukariPortal>())
            {
                if (purchaseValueCount > Item.buyPrice(0, 50))
                {
                    purchaseValueCount = 0;
                    Player.QuickSpawnItemDirect(Player.GetSource_GiftOrReward(), ItemType<YukariTicketLeft>());
                    Player.QuickSpawnItemDirect(Player.GetSource_GiftOrReward(), ItemType<YukariTicketRight>());

                    if (Player.whoAmI == Main.myPlayer)
                        Main.npcChatText = ModUtils.GetChatText("Portal", "5");
                }
            }
        }
        public override void PostSellItem(NPC vendor, Item[] shopInventory, Item item)
        {
            if (vendor.type == NPCType<YukariPortal>())
            {
                if (item.buyOnce)
                {
                    ChangePurchaseCount(-item.value / 5);
                }
                else
                {
                    ChangePurchaseCount(-item.value);
                }
            }
        }
        public override void PreUpdateBuffs()
        {
            if (Player.miscEquips[0].type == ItemType<YukariTicketLeft>())
            {
                yukariLicenseLeft = true;
            }
            if (Player.miscEquips[1].type == ItemType<YukariTicketRight>())
            {
                yukariLicenseRight = true;
            }

        }
        public override void PostUpdateBuffs()
        {
            Player.buffImmune[BuffType<LightYukariBuff>()] = true;
            if (!yukariLicenseLeft || !yukariLicenseRight)
            {
                Player.buffImmune[BuffType<YukariBuff>()] = true;
            }
        }
        public override void SaveData(TagCompound tag)
        {
            tag["purchaseValueCount"] = purchaseValueCount;
            tag["totalPurchaseValueCount"] = totalPurchaseValueCount;
        }

        public override void LoadData(TagCompound tag)
        {
            purchaseValueCount = tag.GetInt("purchaseValueCount");
            totalPurchaseValueCount = tag.GetInt("totalPurchaseValueCount");
        }
    }
}
