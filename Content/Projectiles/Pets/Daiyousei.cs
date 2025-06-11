using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.Utilities;
using TouhouPets.Content.Buffs.PetBuffs;

namespace TouhouPets.Content.Projectiles.Pets
{
    public class Daiyousei : BasicTouhouPet
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

        private int wingFrame, wingFrameCounter;

        private DrawPetConfig drawConfig = new(1);
        private readonly Texture2D clothTex = AltVanillaFunction.GetExtraTexture("Daiyousei_Cloth");
        public override void SetStaticDefaults()
        {
            Main.projFrames[Type] = 7;
            Main.projPet[Type] = true;
        }
        public override TouhouPetID UniqueID => TouhouPetID.Daiyousei;
        public override bool DrawPetSelf(ref Color lightColor)
        {
            Projectile.DrawPet(wingFrame, lightColor * 0.7f, drawConfig);
            Projectile.DrawPet(Projectile.frame, lightColor, drawConfig);
            Projectile.DrawPet(Projectile.frame, lightColor,
                drawConfig with
                {
                    AltTexture = clothTex,
                    ShouldUseEntitySpriteDraw = true,
                });
            return false;
        }
        public override Color ChatTextColor => new Color(71, 228, 63);
        public override void RegisterChat(ref string name, ref Vector2 indexRange)
        {
            name = "Daiyousei";
            indexRange = new Vector2(1, 8);
        }
        public override void SetRegularDialog(ref int timePerDialog, ref int chance, ref bool whenShouldStop)
        {
            timePerDialog = 960;
            chance = 9;
            whenShouldStop = false;
        }
        public override WeightedRandom<string> RegularDialogText()
        {
            WeightedRandom<string> chat = new WeightedRandom<string>();
            {
                if (Owner.ZoneGraveyard)
                    chat.Add(ChatDictionary[5]);
                else
                {
                    chat.Add(ChatDictionary[1]);
                    chat.Add(ChatDictionary[2]);

                    if (FindPet(ProjectileType<Cirno>()))
                    {
                        chat.Add(ChatDictionary[3]);
                        chat.Add(ChatDictionary[4]);
                    }
                }
            }
            return chat;
        }
        public override void VisualEffectForPreview()
        {
            UpdateWingFrame();
        }
        private void UpdateTalking()
        {
            if (FindChatIndex(4, 6))
            {
                Chatting1(currentChatRoom ?? Projectile.CreateChatRoomDirect(), chatIndex);
            }
        }
        private void Chatting1(PetChatRoom chatRoom, int index)
        {
            int type = ProjectileType<Cirno>();
            if (FindPet(out Projectile member, type))
            {
                chatRoom.member[0] = member;
                member.ToPetClass().currentChatRoom = chatRoom;
            }
            else
            {
                chatRoom.CloseChatRoom();
                return;
            }
            Projectile daiyousei = chatRoom.initiator;
            Projectile cirno = chatRoom.member[0];
            int turn = chatRoom.chatTurn;
            if (index == 4)
            {
                if (turn == -1)
                {
                    //大妖精：琪露诺酱，今天去哪里玩？
                    cirno.CloseCurrentDialog();

                    if (daiyousei.CurrentDialogFinished())
                        chatRoom.chatTurn++;
                }
                else if (turn == 0)
                {
                    //琪露诺：大酱去哪儿我就去哪儿！
                    cirno.SetChat(ChatSettingConfig, 10, 20);

                    if (cirno.CurrentDialogFinished())
                        chatRoom.chatTurn++;
                }
                else
                {
                    chatRoom.CloseChatRoom();
                }
            }
            else if (index == 5 || index == 6)
            {
                if (turn == -1)
                {
                    //大妖精：好...好可怕的地方！
                    cirno.CloseCurrentDialog();

                    if (daiyousei.CurrentDialogFinished())
                        chatRoom.chatTurn++;
                }
                else if (turn == 0)
                {
                    //琪露诺：大酱别怕，有我在！
                    cirno.SetChat(ChatSettingConfig, 9, 20);

                    if (cirno.CurrentDialogFinished())
                        chatRoom.chatTurn++;
                }
                else if (turn == 1)
                {
                    //大妖精：嗯...我，我不怕！
                    daiyousei.SetChat(ChatSettingConfig, 6, 20);

                    if (daiyousei.CurrentDialogFinished())
                        chatRoom.chatTurn++;
                }
                else
                {
                    chatRoom.CloseChatRoom();
                }
            }
        }
        public override void AI()
        {
            Player player = Main.player[Projectile.owner];
            Projectile.SetPetActive(player, BuffType<DaiyouseiBuff>());

            UpdateTalking();

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
            Projectile.rotation = Projectile.velocity.X * 0.032f;

            ChangeDir();

            Vector2 point = new Vector2(-40 * Owner.direction, -30 + Owner.gfxOffY);
            if (FindPet(ProjectileType<Cirno>(), false))
            {
                point = new Vector2(80 * Owner.direction, -30 + Owner.gfxOffY);
            }
            MoveToPoint(point, 9f);
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
            if (++Projectile.frameCounter > 3)
            {
                Projectile.frameCounter = 0;
                Projectile.frame++;
            }
            if (Projectile.frame > 2)
            {
                Projectile.frame = 0;
                CurrentState = States.Idle;
            }
        }
        private void UpdateWingFrame()
        {
            if (wingFrame < 3)
            {
                wingFrame = 3;
            }
            if (++wingFrameCounter > 4)
            {
                wingFrameCounter = 0;
                wingFrame++;
            }
            if (wingFrame > 6)
            {
                wingFrame = 3;
            }
        }
    }
}


