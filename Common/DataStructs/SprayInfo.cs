namespace TouhouPets
{
    public struct SprayInfo(int projectileID, int dustID)
    {
        public int SprayType = projectileID;
        public int SprayDust = dustID;
    }
}
