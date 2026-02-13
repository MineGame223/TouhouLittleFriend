using Microsoft.Xna.Framework;
using Terraria;
using Terraria.Graphics.CameraModifiers;
using TouhouPets.Content.Projectiles.Pets;
using static TouhouPets.SolutionSpraySystem;

namespace TouhouPets
{
    public class YukaCameraModifier(Projectile pet, string uniqueIdentity = null) : ICameraModifier
    {
        public Vector2 targetPosition = pet.Center - new Vector2(Main.screenWidth / 2, Main.screenHeight / 2);
        public string UniqueIdentity { get; private set; } = uniqueIdentity;
        public bool Finished { get; private set; }
        public void Update(ref CameraInfo cameraInfo)
        {
            if (pet == null || !pet.active ||
                pet.owner != Main.myPlayer ||
                pet.type != ProjectileType<Yuka>())
            {
                Finished = true;
                return;
            }
            int state = (int)pet.ai[1];

            cameraInfo.CameraPosition = Vector2.SmoothStep(cameraInfo.CameraPosition, targetPosition, pet.localAI[2]);
            if (state == Phase_Spray_ManualMode)
            {
                if (pet.localAI[2] < 1)
                    pet.localAI[2] += 0.01f;
            }
            if (state == Phase_StopSpray)
            {
                if (pet.localAI[2] >= 1)
                    pet.localAI[2] -= 0.01f;
                else
                    Finished = true;
            }
        }
    }
}
