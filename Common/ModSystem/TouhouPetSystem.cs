

namespace TouhouPets
{
    public class TouhouPetSystem:ModSystem
    {
        public override void PreUpdateProjectiles()
        {
            DanmakuFightHelper.UpdateDanmakuRingScale();
        }
    }
}
