using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;
using Terraria;
using Terraria.ID;
using Terraria.Utilities;
using TouhouPets.Content.Buffs.PetBuffs;

namespace TouhouPets.Content.Projectiles.Pets
{
    public class Ran : BasicTouhouPet
    {
        private enum States
        {
            Idle,
            Blink,
        }
        private States CurrentState
        {
            get => (States)PetState;
            set => PetState = (int)value;
        }

        private int tailFrame, tailFrameCounter;
        private int blinkFrame, blinkFrameCounter;
        private int clothFrame, clothFrameCounter;

        private DrawPetConfig drawConfig = new(2);
        private readonly Texture2D clothTex = AltVanillaFunction.GetExtraTexture("Ran_Cloth");
        public override void SetStaticDefaults()
        {
            Main.projFrames[Type] = 8;
            Main.projPet[Type] = true;
            ProjectileID.Sets.LightPet[Type] = false;
        }
        public override TouhouPetID UniqueID => TouhouPetID.Ran;
        public override bool DrawPetSelf(ref Color lightColor)
        {
            DrawPetConfig config = drawConfig with
            {
                ShouldUseEntitySpriteDraw = true,
            };
            Projectile.DrawPet(tailFrame, lightColor, drawConfig, 1);

            Projectile.DrawPet(Projectile.frame, lightColor, drawConfig);

            if (CurrentState == States.Blink)
                Projectile.DrawPet(blinkFrame, lightColor, drawConfig);

            Projectile.DrawPet(Projectile.frame, lightColor,
                config with
                {
                    AltTexture = clothTex,
                });
            Projectile.DrawPet(clothFrame, lightColor, config, 1);
            return false;
        }
        public override ChatSettingConfig ChatSettingConfig => new ChatSettingConfig() with
        {
            TextColor = new Color(254, 216, 82),
        };
        public override void RegisterChat(ref string name, ref Vector2 indexRange)
        {
            name = "Ran";
            indexRange = new Vector2(1, 7);
        }
        public override void SetRegularDialog(ref int timePerDialog, ref int chance, ref bool whenShouldStop)
        {
            timePerDialog = 750;
            chance = 8;
            whenShouldStop = false;
        }
        public override WeightedRandom<string> RegularDialogText()
        {
            WeightedRandom<string> chat = new WeightedRandom<string>();
            {
                chat.Add(ChatDictionary[1]);
                chat.Add(ChatDictionary[2]);
                chat.Add(ChatDictionary[3]);
                if (FindPet(ProjectileType<Chen>()))
                {
                    chat.Add(ChatDictionary[7]);
                }
            }
            return chat;
        }
        public override List<List<ChatRoomInfo>> RegisterChatRoom()
        {
            return new()
            {
                Chatting1(),
                Chatting2(),
            };
        }
        private static List<ChatRoomInfo> Chatting1()
        {
            TouhouPetID ran = TouhouPetID.Ran;
            TouhouPetID yukari = TouhouPetID.Yukari;

            List<ChatRoomInfo> list =
            [
                new ChatRoomInfo(ran, 3, -1), //蓝：饕餮那家伙，依旧坚持在畜牲界混吗...
                new ChatRoomInfo(yukari, 4, 0), //紫：若是叨念的话，我允许你临时请假哦。
                new ChatRoomInfo(ran, 4, 1), //蓝：才、才没有啊紫大人！
            ];

            return list;
        }
        private static List<ChatRoomInfo> Chatting2()
        {
            TouhouPetID ran = TouhouPetID.Daiyousei;
            TouhouPetID chen = TouhouPetID.Chen;

            List<ChatRoomInfo> list =
            [
                new ChatRoomInfo(ran, 7, -1), //蓝：橙，不要乱跑哦！
                new ChatRoomInfo(chen, 3, 0), //橙：知道啦蓝大人，橙可没有乱跑！
            ];

            return list;
        }
        public override void VisualEffectForPreview()
        {
            UpdateTailFrame();
            UpdateClothFrame();
        }
        public override void AI()
        {
            Projectile.SetPetActive(Owner, BuffType<YukariBuff>());

            ControlMovement();

            switch (CurrentState)
            {
                case States.Blink:
                    Blink();
                    break;

                default:
                    Idle();
                    break;
            }
        }
        private void ControlMovement()
        {
            Projectile.tileCollide = false;
            Projectile.rotation = Projectile.velocity.X * 0.015f;

            ChangeDir();

            Vector2 point = new Vector2(-60 * Owner.direction, -30 + Owner.gfxOffY);
            MoveToPoint(point, 18f);
        }
        private void Idle()
        {
            Projectile.frame = 0;
            if (OwnerIsMyPlayer && mainTimer % 270 == 0)
            {
                CurrentState = States.Blink;
            }
        }
        private void Blink()
        {
            Projectile.frame = 0;
            if (blinkFrame < 4)
            {
                blinkFrame = 4;
            }
            if (++blinkFrameCounter > 3)
            {
                blinkFrameCounter = 0;
                blinkFrame++;
            }
            if (blinkFrame > 6)
            {
                blinkFrame = 4;
                CurrentState = States.Idle;
            }
        }
        private void UpdateTailFrame()
        {
            int count = 6;
            if (++tailFrameCounter > count)
            {
                tailFrameCounter = 0;
                tailFrame++;
            }
            if (tailFrame > 3)
            {
                tailFrame = 0;
            }
        }
        private void UpdateClothFrame()
        {
            if (clothFrame < 4)
            {
                clothFrame = 4;
            }
            int count = 5;
            if (++clothFrameCounter > count)
            {
                clothFrameCounter = 0;
                clothFrame++;
            }
            if (clothFrame > 7)
            {
                clothFrame = 4;
            }
        }
    }
}


