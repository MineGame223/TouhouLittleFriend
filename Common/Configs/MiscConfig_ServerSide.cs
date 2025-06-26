using System.ComponentModel;
using Terraria.ModLoader.Config;

namespace TouhouPets
{
    public class MiscConfig_ServerSide : ModConfig
    {
        public override ConfigScope Mode => ConfigScope.ServerSide;

        [DefaultValue(false)]
        public bool CompatibilityMode;

        [DefaultValue(true)]
        [ReloadRequired]
        public bool AllowModCall_MarisasReaction;

        [DefaultValue(true)]
        [ReloadRequired]
        public bool AllowModCall_YuyukosReaction;

        [DefaultValue(true)]
        [ReloadRequired]
        public bool AllowModCall_PetDialog;

        [DefaultValue(true)]
        [ReloadRequired]
        public bool AllowModCall_PetChatRoom;
    }
}