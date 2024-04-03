using Microsoft.Xna.Framework;
using Terraria;
using Terraria.GameContent.Drawing;
using Terraria.ID;
using TouhouPets.Content.Buffs.PetBuffs;
using TouhouPets.Content.Items.PetItems;

namespace TouhouPets
{
    public class TouhouPetGlobalItem : GlobalItem
    {
        public override bool InstancePerEntity => true;
        public override bool CanUseItem(Item item, Player player)
        {
            if (player.HasBuff<MinorikoBuff>())
            {
                if (ItemID.Sets.IsFood[item.type] && item.buffType > 0)
                {
                    int defaultTime = item.buffTime;
                    item.buffTime += (int)(defaultTime * 0.1f);
                }
            }
            return base.CanUseItem(item, player);
        }
        public override bool? UseItem(Item item, Player player)
        {
            return base.UseItem(item, player);
        }
        public override void Update(Item item, ref float gravity, ref float maxFallSpeed)
        {
            if (item.type == ItemID.Sapphire && !item.shimmered)
            {
                foreach (Item i in Main.item)
                {
                    if (i != null && i.active
                        && i.type == ItemID.FallenStar && i.getRect().Intersects(item.getRect()) && !i.shimmered)
                    {
                        i.stack--;
                        item.stack--;
                        if (i.stack <= 0)
                        {
                            i.TurnToAir();
                        }
                        if (item.stack <= 0)
                        {
                            item.TurnToAir();
                        }
                        ParticleOrchestraSettings settings;
                        for (int z = 0; z < 8; z++)
                        {
                            settings = new ParticleOrchestraSettings
                            {
                                PositionInWorld = item.Center + new Vector2(0, Main.rand.Next(0, 50)).RotatedByRandom(MathHelper.TwoPi),
                                MovementVector = Vector2.Zero,
                            };
                            ParticleOrchestrator.SpawnParticlesDirect(ParticleOrchestraType.StardustPunch, settings);
                        }
                        settings = new ParticleOrchestraSettings
                        {
                            PositionInWorld = item.Center,
                            MovementVector = Vector2.Zero,
                        };
                        ParticleOrchestrator.SpawnParticlesDirect(ParticleOrchestraType.ShimmerTownNPC, settings);
                        Item.NewItem(item.GetSource_FromThis(), item.getRect(), ItemType<StarSapphire>());
                        break;
                    }
                }
            }
        }
    }
}
