using Terraria;

namespace TouhouPets.Common
{
    public class PrismriverBandSceneEffect : ModSceneEffect
    {
        public override int Music => Main.LocalPlayer.GetModPlayer<ConcertPlayer>().BandMusicID;
        public override SceneEffectPriority Priority => SceneEffectPriority.BossHigh;
        public override bool IsSceneEffectActive(Player player)
        {
            ConcertPlayer bp = player.GetModPlayer<ConcertPlayer>();
            return bp.ShouldBandPlaying && !bp.CustomModeOn;
        }
    }
}
