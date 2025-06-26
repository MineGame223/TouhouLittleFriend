using System;

namespace TouhouPets
{
    public struct SprayInfo(int projectileID, int dustID, Func<int> advancedDust = null)
    {
        public int SprayType = projectileID;
        public int SprayDust = dustID;
        public Func<int> SprayDustAdvanced = advancedDust;
    }
}
