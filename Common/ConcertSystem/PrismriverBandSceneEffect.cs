using Terraria;
using Terraria.ID;

namespace TouhouPets.Common
{
    public class PrismriverBandSceneEffect : ModSceneEffect
    {
        public override int Music => Main.LocalPlayer.GetModPlayer<ConcertPlayer>().BandMusicID;
        public override SceneEffectPriority Priority => SceneEffectPriority.Event;
        public override bool IsSceneEffectActive(Player player)
        {
            return player.GetModPlayer<ConcertPlayer>().ShouldBandPlaying;
        }
    }
}
