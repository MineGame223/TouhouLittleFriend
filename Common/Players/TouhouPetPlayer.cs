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
        public bool MurasasCurse => Player.HasBuff<MurasaBuff>() && Main.remixWorld;

        public int koakumaNumber;
        public int purchaseValueCount;
        public int totalPurchaseValueCount;
        public bool treasureShine;
        private void ChangePurchaseCount(int amount)
        {
            totalPurchaseValueCount += amount;
            purchaseValueCount += amount;
        }
        private void CommonResetUpdate()
        {
            treasureShine = false;
        }
        public override void ResetEffects()
        {
            purchaseValueCount = (int)MathHelper.Clamp(purchaseValueCount, 0, int.MaxValue - 1);
            totalPurchaseValueCount = (int)MathHelper.Clamp(totalPurchaseValueCount, 0, int.MaxValue - 1);
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
                    Player.QuickSpawnItemDirect(Player.GetSource_GiftOrReward(), ItemType<YukarisItem>());

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
        public override void PostUpdateBuffs()
        {
            if (treasureShine)
            {
                Player.spelunkerTimer++;
                if (Player.spelunkerTimer >= 10)
                {
                    Player.spelunkerTimer = 0;
                    Main.instance.SpelunkerProjectileHelper.AddSpotToCheck(Player.Center);
                }
            }
        }
        public override void UpdateEquips()
        {
            if (MurasasCurse)
            {
                Player.waterWalk = false;
                Player.accMerman = false;

                if (Player.breathCD > 0 && Player.breath > 0)
                    Player.breath -= 10;

                Player.breathCD += Player.breathCDMax / 2;
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
