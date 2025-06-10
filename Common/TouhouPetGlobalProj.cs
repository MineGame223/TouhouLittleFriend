

namespace TouhouPets
{
    public class TouhouPetGlobalProj : GlobalProjectile
    {
        public override bool InstancePerEntity => true;

        public bool isADanmaku;
        public bool isDanmakuDestorible;

        public bool belongsToPlayerA;
        public bool belongsToPlayerB;
    }
}
