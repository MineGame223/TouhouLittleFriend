﻿

namespace TouhouPets
{
    public class TouhouPetGlobalProj : GlobalProjectile
    {
        public override bool InstancePerEntity => true;
        public bool isADanmaku = false;
        public bool isDanmakuDestorible = false;

        public bool belongsToPlayerA = false;
        public bool belongsToPlayerB = false;
    }
}
