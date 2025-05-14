

namespace TouhouPets
{
    public class ConcertSystem : ModSystem
    {
        public override void PreSaveAndQuit()
        {
            CustomMusicManager.Stop();
        }
        public override void PostUpdateEverything()
        {
            CustomMusicManager.Update();
        }
    }
}
