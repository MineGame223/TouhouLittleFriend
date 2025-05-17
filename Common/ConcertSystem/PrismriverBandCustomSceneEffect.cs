using Terraria;

namespace TouhouPets.Common
{
    public class PrismriverBandCustomSceneEffect : ModSceneEffect
    {
        public override int Music => 0;
        public override SceneEffectPriority Priority => SceneEffectPriority.BossHigh;
        public override bool IsSceneEffectActive(Player player)
        {
            ConcertPlayer bp = player.GetModPlayer<ConcertPlayer>();
            return bp.ShouldBandPlaying && bp.CustomModeOn;
        }
    }
}
