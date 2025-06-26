using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;
using Terraria;
using Terraria.ID;
using Terraria.Localization;
using Terraria.Utilities;
using TouhouPets.Content.Buffs.PetBuffs;

namespace TouhouPets.Content.Projectiles.Pets
{
    public class Hina : BasicTouhouPet
    {
        private enum States
        {
            Idle,
            Blink,
            Turning,
            AfterTurning,
        }
        private States CurrentState
        {
            get => (States)PetState;
            set => PetState = (int)value;
        }
        private int ActionCD
        {
            get => (int)Projectile.localAI[0];
            set => Projectile.localAI[0] = value;
        }
        private int Timer
        {
            get => (int)Projectile.localAI[1];
            set => Projectile.localAI[1] = value;
        }
        private int RandomCount
        {
            get => (int)Projectile.localAI[2];
            set => Projectile.localAI[2] = value;
        }
        private bool IsIdleState => CurrentState <= States.Blink;

        private int blinkFrame, blinkFrameCounter;

        private DrawPetConfig drawConfig = new(1);
        private readonly Texture2D clothTex = AltVanillaFunction.GetExtraTexture("Hina_Cloth");
        public override void PetStaticDefaults()
        {
            Main.projFrames[Type] = 17;
            Main.projPet[Type] = true;

            ProjectileID.Sets.CharacterPreviewAnimations[Type] =
                ProjectileID.Sets.SimpleLoop(0, 6, 8)
                .WhenSelected(6, 7, 7);
        }
        public override TouhouPetID UniqueID => TouhouPetID.Hina;
        public override bool DrawPetSelf(ref Color lightColor)
        {
            Projectile.DrawPet(Projectile.frame, lightColor, drawConfig);
            if (CurrentState == States.Blink)
                Projectile.DrawPet(blinkFrame, lightColor, drawConfig);
            Projectile.DrawPet(Projectile.frame, lightColor,
                drawConfig with
                {
                    AltTexture = clothTex,
                    ShouldUseEntitySpriteDraw = true,
                });
            return false;
        }
        public override ChatSettingConfig ChatSettingConfig => new ChatSettingConfig() with
        {
            TextColor = new Color(70, 226, 164),
        };
        public override void RegisterChat(ref string name, ref Vector2 indexRange)
        {
            name = "Hina";
            indexRange = new Vector2(1, 8);
        }
        public override void SetRegularDialog(ref int timePerDialog, ref int chance, ref bool whenShouldStop)
        {
            timePerDialog = 720;//20
            chance = 7;//7
            whenShouldStop = !IsIdleState;
        }
        public override WeightedRandom<LocalizedText> RegularDialogText()
        {
            WeightedRandom<LocalizedText> chat = new ();
            {
                chat.Add(ChatDictionary[1]);
                chat.Add(ChatDictionary[2]);
                if (FindPet(ProjectileType<Nitori>()))
                {
                    chat.Add(ChatDictionary[4]);
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
        private List<ChatRoomInfo> Chatting1()
        {
            TouhouPetID hina = TouhouPetID.Hina;
            TouhouPetID nitori = TouhouPetID.Nitori;

            List<ChatRoomInfo> list =
            [
                new ChatRoomInfo(hina, ChatDictionary[4], -1), //转转：荷取，你知道吗？我一直有一个愿望。
                new ChatRoomInfo(nitori, GetChatText("Nitori",4), 0),//荷取：嗯？是什么愿望呢？
                new ChatRoomInfo(hina, ChatDictionary[5], 1), //转转：我希望，把这个世界彻底净化成没有厄运的世界。
                new ChatRoomInfo(nitori, GetChatText("Nitori",5), 2),//荷取：那不就是你的能力嘛，不过全世界的厄运即便对你而言也不太现实吧...
                new ChatRoomInfo(hina, ChatDictionary[6], 3), //转转：哈哈，所以说只是一个愿望啊。
                new ChatRoomInfo(nitori, GetChatText("Nitori",6), 4),//荷取：说不定哪一天真的可以实现哦！
            ];

            return list;
        }
        private List<ChatRoomInfo> Chatting2()
        {
            TouhouPetID hina = TouhouPetID.Hina;
            TouhouPetID nitori = TouhouPetID.Nitori;

            List<ChatRoomInfo> list =
            [
                new ChatRoomInfo(hina, ChatDictionary[7], -1), //转转：我们是...
                new ChatRoomInfo(nitori, GetChatText("Nitori",7), 0),//荷取：“旋转河童”组合！
                new ChatRoomInfo(hina, ChatDictionary[8], 1), //转转 & 荷取：哈哈哈哈！...
                new ChatRoomInfo(nitori, GetChatText("Nitori",8), 1),
            ];

            return list;
        }
        public override void AI()
        {
            Projectile.SetPetActive(Owner, BuffType<HinaBuff>());

            ControlMovement();

            switch (CurrentState)
            {
                case States.Blink:
                    Blink();
                    break;

                case States.Turning:
                    shouldNotTalking = true;
                    Turning();
                    break;

                case States.AfterTurning:
                    shouldNotTalking = true;
                    AfterTurning();
                    break;

                default:
                    Idle();
                    break;
            }
            if (IsIdleState)
            {
                IdleAnimation();
            }
            if (IsIdleState && ActionCD > 0)
            {
                ActionCD--;
            }
        }
        private void ControlMovement()
        {
            Projectile.tileCollide = false;
            Projectile.rotation = Projectile.velocity.X * 0.032f;

            if (IsIdleState)
                ChangeDir();

            Vector2 point = new Vector2(-40 * Owner.direction, -30 + Owner.gfxOffY);
            MoveToPoint(point, 13f);
        }
        private void Idle()
        {
            if (OwnerIsMyPlayer)
            {
                if (mainTimer % 270 == 0)
                {
                    CurrentState = States.Blink;
                }
                if (mainTimer > 0 && mainTimer % 320 == 0 && currentChatRoom == null && ActionCD <= 0)
                {
                    if (Main.rand.NextBool(5))
                    {
                        RandomCount = Main.rand.Next(10, 20);
                        CurrentState = States.Turning;

                        if (Main.rand.NextBool(2))
                            Projectile.SetChat(ChatDictionary[3], 20);
                    }
                }
            }
        }
        private void Blink()
        {
            if (blinkFrame < 14)
            {
                blinkFrame = 14;
            }
            if (++blinkFrameCounter > 3)
            {
                blinkFrameCounter = 0;
                blinkFrame++;
            }
            if (blinkFrame > 16)
            {
                blinkFrame = 14;
                CurrentState = States.Idle;
            }
        }
        private void Turning()
        {
            if (++Projectile.frameCounter > 7)
            {
                Projectile.frameCounter = 0;
                Projectile.frame++;
            }
            if (Projectile.frame > 13)
            {
                Projectile.frame = 6;
                Timer++;
            }
            if (OwnerIsMyPlayer && Timer > RandomCount)
            {
                Timer = 0;
                CurrentState = States.AfterTurning;
            }
        }
        private void AfterTurning()
        {
            if (++Projectile.frameCounter > 7)
            {
                Projectile.frameCounter = 0;
                Projectile.frame++;
            }
            if (Projectile.frame > 13)
            {
                Projectile.frame = 0;
                if (OwnerIsMyPlayer)
                {
                    ActionCD = 500;
                    CurrentState = States.Idle;
                }
            }
        }
        private void IdleAnimation()
        {
            if (++Projectile.frameCounter > 8)
            {
                Projectile.frameCounter = 0;
                Projectile.frame++;
            }
            if (Projectile.frame > 5)
            {
                Projectile.frame = 0;
            }
        }
    }
}


