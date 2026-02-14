using Microsoft.Xna.Framework;
using Terraria;
using Terraria.Graphics.CameraModifiers;

namespace TouhouPets
{
    public class MovingCameraModifier(Vector2 position, bool whenLock, string uniqueIdentity = null, int frames = 120) : ICameraModifier
    {
        private readonly int framesToLast = frames;
        public Vector2 targetPosition = position - new Vector2(Main.screenWidth / 2, Main.screenHeight / 2);
        public string UniqueIdentity { get; private set; } = uniqueIdentity;
        public bool Finished { get; private set; }
        public void Update(ref CameraInfo cameraInfo)
        {
            float progress = Utils.GetLerpValue(0, framesToLast, TouhouPets.CameraLerpValue);
            float lerpAmount = progress switch
            {
                < 0.5f => Utils.Remap(progress, 0, 0.5f, 0, 1),
                > 0.5f => Utils.Remap(progress, 0.5f, 1f, 1, 0),
                _ => 1,
            };

            cameraInfo.CameraPosition = Vector2.SmoothStep(cameraInfo.CameraPosition, targetPosition, lerpAmount);

            if (!Main.gameInactive && !Main.gamePaused)
            {
                if (lerpAmount < 1 || !whenLock)
                    TouhouPets.CameraLerpValue++;
            }
            if (TouhouPets.CameraLerpValue >= framesToLast)
            {
                Finished = true;
                TouhouPets.CameraLerpValue = 0;
            }
            //此处无限重置，以确保当没有再添加摄像机编辑器时它能归位
            whenLock = false;
        }
    }
}
