using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ID;
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
        public override void SetStaticDefaults()
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
            timePerDialog = 720;
            chance = 7;
            whenShouldStop = !IsIdleState;
        }
        public override WeightedRandom<string> RegularDialogText()
        {
            WeightedRandom<string> chat = new WeightedRandom<string>();
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
        private void UpdateTalking()
        {
            if (FindChatIndex(4, 8))
            {
                Chatting1(currentChatRoom ?? Projectile.CreateChatRoomDirect(), chatIndex);
            }
        }
        private void Chatting1(PetChatRoom chatRoom, int index)
        {
            int type = ProjectileType<Nitori>();
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
            Projectile hina = chatRoom.initiator;
            Projectile nitori = chatRoom.member[0];
            int turn = chatRoom.chatTurn;
            if (index >= 4 && index <= 6)
            {
                if (turn == -1)
                {
                    //转转：荷取，你知道吗？我一直有一个愿望。
                    nitori.CloseCurrentDialog();

                    if (hina.CurrentDialogFinished())
                        chatRoom.chatTurn++;
                }
                else if (turn == 0)
                {
                    //荷取：嗯？是什么愿望呢？
                    nitori.SetChat(ChatSettingConfig, 4, 20);

                    if (nitori.CurrentDialogFinished())
                        chatRoom.chatTurn++;
                }
                else if (turn == 1)
                {
                    //转转：我希望，把这个世界彻底净化成没有厄运的世界。
                    hina.SetChat(ChatSettingConfig, 5, 20);

                    if (hina.CurrentDialogFinished())
                        chatRoom.chatTurn++;
                }
                else if (turn == 2)
                {
                    //荷取：那不就是你的能力嘛，不过全世界的厄运即便对你而言也不太现实吧...
                    nitori.SetChat(ChatSettingConfig, 5, 20);

                    if (nitori.CurrentDialogFinished())
                        chatRoom.chatTurn++;
                }
                else if (turn == 3)
                {
                    //转转：哈哈，所以说只是一个愿望啊。
                    hina.SetChat(ChatSettingConfig, 6, 20);

                    if (hina.CurrentDialogFinished())
                        chatRoom.chatTurn++;
                }
                else if (turn == 4)
                {
                    //荷取：说不定哪一天真的可以实现哦！
                    nitori.SetChat(ChatSettingConfig, 6, 20);

                    if (nitori.CurrentDialogFinished())
                        chatRoom.chatTurn++;
                }
                else
                {
                    chatRoom.CloseChatRoom();
                }
            }
            else if (index == 7 || index == 8)
            {
                if (turn == -1)
                {
                    //转转：我们是...
                    nitori.CloseCurrentDialog();

                    if (hina.CurrentDialogFinished())
                        chatRoom.chatTurn++;
                }
                else if (turn == 0)
                {
                    //荷取：“旋转河童”组合！
                    nitori.SetChat(ChatSettingConfig, 7, 20);

                    if (nitori.CurrentDialogFinished())
                        chatRoom.chatTurn++;
                }
                else if (turn == 1)
                {
                    //转转&荷取：哈哈哈哈！...
                    nitori.SetChat(ChatSettingConfig, 8, 20);
                    hina.SetChat(ChatSettingConfig, 8, 20);

                    if (hina.CurrentDialogFinished() || nitori.CurrentDialogFinished())
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
            Projectile.SetPetActive(Owner, BuffType<HinaBuff>());

            UpdateTalking();

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
                    if (Main.rand.NextBool(3))
                    {
                        RandomCount = Main.rand.Next(10, 20);
                        CurrentState = States.Turning;

                        if (Main.rand.NextBool(2) && chatTimeLeft <= 0)
                            Projectile.SetChat(ChatSettingConfig, 3, 20);
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


