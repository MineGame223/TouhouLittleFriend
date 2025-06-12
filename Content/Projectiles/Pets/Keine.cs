using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.Enums;
using Terraria.Utilities;
using TouhouPets.Content.Buffs.PetBuffs;

namespace TouhouPets.Content.Projectiles.Pets
{
    public class Keine : BasicTouhouPet
    {
        private enum States
        {
            Idle,
            Blink,
            AltForm,
            AltFormBlink,
        }
        private States CurrentState
        {
            get => (States)PetState;
            set => PetState = (int)value;
        }
        internal static bool UseAlternateForm => Main.bloodMoon || (Main.GetMoonPhase() == MoonPhase.Full && !Main.dayTime);
        private bool IsAltForm => CurrentState >= States.AltForm && CurrentState <= States.AltFormBlink;

        private int hairFrame, hairFrameCounter;
        private int clothFrame, clothFrameCounter;
        private int auraFrame, auraFrameCounter;

        private DrawPetConfig drawConfig = new(2);
        private readonly Texture2D clothTex = AltVanillaFunction.GetExtraTexture("Keine_Cloth");
        public override void SetStaticDefaults()
        {
            Main.projFrames[Type] = 16;
            Main.projPet[Type] = true;
        }
        public override TouhouPetID UniqueID => TouhouPetID.Keine;
        public override bool DrawPetSelf(ref Color lightColor)
        {
            DrawPetConfig config = drawConfig with
            {
                ShouldUseEntitySpriteDraw = true,
            };
            int currentRow = IsAltForm ? 1 : 0;
            Projectile.DrawPet(auraFrame, Color.White, config, currentRow);
            Projectile.ResetDrawStateForPet();

            Projectile.DrawPet(hairFrame, lightColor, drawConfig, currentRow);
            Projectile.DrawPet(Projectile.frame, lightColor, drawConfig, currentRow);

            Projectile.DrawPet(Projectile.frame, lightColor,
                config with
                {
                    AltTexture = clothTex,
                }, currentRow);
            Projectile.DrawPet(clothFrame, lightColor, config, currentRow);
            return false;
        }
        public override ChatSettingConfig ChatSettingConfig => new ChatSettingConfig() with
        {
            TextColor = IsAltForm ? new Color(69, 172, 105) : new Color(97, 103, 255),
        };
        public override void RegisterChat(ref string name, ref Vector2 indexRange)
        {
            name = "Keine";
            indexRange = new Vector2(1, 9);
        }
        public override void SetRegularDialog(ref int timePerDialog, ref int chance, ref bool whenShouldStop)
        {
            timePerDialog = 720;//720
            chance = 7;//7
            whenShouldStop = false;
        }
        public override WeightedRandom<string> RegularDialogText()
        {
            WeightedRandom<string> chat = new WeightedRandom<string>();
            {
                if (UseAlternateForm)
                {
                    chat.Add(ChatDictionary[4]);
                    chat.Add(ChatDictionary[5]);
                    chat.Add(ChatDictionary[6]);
                }
                else
                {
                    chat.Add(ChatDictionary[1]);
                    chat.Add(ChatDictionary[2]);
                    chat.Add(ChatDictionary[3]);
                }
                if (FindPet(ProjectileType<Moku>()))
                {
                    chat.Add(ChatDictionary[7]);
                }
            }
            return chat;
        }
        private void UpdateTalking()
        {
            if (FindChatIndex(6))
            {
                Chatting2(currentChatRoom ?? Projectile.CreateChatRoomDirect());
            }
            if (FindChatIndex(7, 8))
            {
                Chatting1(currentChatRoom ?? Projectile.CreateChatRoomDirect(), chatIndex);
            }
        }
        private void Chatting1(PetChatRoom chatRoom, int index)
        {
            int type = ProjectileType<Moku>();
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
            Projectile keine = chatRoom.initiator;
            Projectile moku = chatRoom.member[0];
            int turn = chatRoom.chatTurn;
            if (index >= 7 && index <= 8)
            {
                if (turn == -1)
                {
                    //慧音：最近你怎么样？
                    moku.CloseCurrentDialog();

                    if (keine.CurrentDialogFinished())
                        chatRoom.chatTurn++;
                }
                else if (turn == 0)
                {
                    //妹红：还好吧，还是和以前一样罢了。
                    moku.SetChat(ChatSettingConfig, 7, 20);

                    if (moku.CurrentDialogFinished())
                        chatRoom.chatTurn++;
                }
                else if (turn == 1)
                {
                    //慧音：还在纠结过去的事情么？
                    keine.SetChat(ChatSettingConfig, 8, 20);

                    if (keine.CurrentDialogFinished())
                        chatRoom.chatTurn++;
                }
                else if (turn == 2)
                {
                    //妹红：不，我已经在试着忘掉那些了...
                    moku.SetChat(ChatSettingConfig, 8, 20);

                    if (moku.CurrentDialogFinished())
                        chatRoom.chatTurn++;
                }
                else
                {
                    chatRoom.CloseChatRoom();
                }
            }
        }
        private void Chatting2(PetChatRoom chatRoom)
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
            Projectile keine = chatRoom.initiator;
            Projectile cirno = chatRoom.member[0];
            int turn = chatRoom.chatTurn;
            if (turn == -1)
            {
                //慧音：让我看看谁没写作业...
                cirno.CloseCurrentDialog();

                if (keine.CurrentDialogFinished())
                    chatRoom.chatTurn++;
            }
            else if (turn == 0)
            {
                //琪露诺：（糟了！要被慧音老师发现了...）
                cirno.SetChat(ChatSettingConfig, 12, 20);

                if (cirno.CurrentDialogFinished())
                    chatRoom.chatTurn++;
            }
            else
            {
                chatRoom.CloseChatRoom();
            }
        }
        public override void VisualEffectForPreview()
        {
            UpdateMiscFrame();
        }
        public override void AI()
        {
            Projectile.SetPetActive(Owner, BuffType<KeineBuff>());

            UpdateTalking();

            ControlMovement();

            switch (CurrentState)
            {
                case States.Blink:
                    Blink();
                    break;

                case States.AltForm:
                    AltForm();
                    break;

                case States.AltFormBlink:
                    AltFormBlink();
                    break;

                default:
                    Idle();
                    break;
            }
        }
        private void ControlMovement()
        {
            Projectile.tileCollide = false;
            Projectile.rotation = Projectile.velocity.X * 0.007f;

            ChangeDir();

            Vector2 point = new Vector2(-50 * Owner.direction, -30 + Owner.gfxOffY);
            MoveToPoint(point, 12f);
        }
        private void Idle()
        {
            Projectile.frame = 0;
            if (OwnerIsMyPlayer && mainTimer % 270 == 0)
            {
                CurrentState = States.Blink;
            }
            if (UseAlternateForm)
            {
                for (int i = 0; i < 40; i++)
                {
                    Dust.NewDustDirect(Projectile.position, Projectile.width, Projectile.height, MyDustId.RedBubble, 0, 0
                        , 100, default, Main.rand.NextFloat(1.7f, 2.5f)).noGravity = true;
                }
                if (OwnerIsMyPlayer)
                {
                    CurrentState = States.AltForm;
                }
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
        private void AltForm()
        {
            Projectile.frame = 0;
            if (OwnerIsMyPlayer && mainTimer % 270 == 0)
            {
                CurrentState = States.AltFormBlink;
            }
            if (!UseAlternateForm)
            {
                for (int i = 0; i < 40; i++)
                {
                    Dust.NewDustDirect(Projectile.position, Projectile.width, Projectile.height, MyDustId.BlueMagic, 0, 0
                        , 100, default, Main.rand.NextFloat(1.7f, 2.5f)).noGravity = true;
                }
                if (OwnerIsMyPlayer)
                {
                    CurrentState = States.Idle;
                }
            }
        }
        private void AltFormBlink()
        {
            if (++Projectile.frameCounter > 3)
            {
                Projectile.frameCounter = 0;
                Projectile.frame++;
            }
            if (Projectile.frame > 2)
            {
                Projectile.frame = 0;
                CurrentState = States.AltForm;
            }
        }
        private void UpdateMiscFrame()
        {
            if (clothFrame < 3)
            {
                clothFrame = 3;
            }
            if (++clothFrameCounter > 4)
            {
                clothFrameCounter = 0;
                clothFrame++;
            }
            if (clothFrame > 6)
            {
                clothFrame = 3;
            }

            if (hairFrame < 7)
            {
                hairFrame = 7;
            }
            if (++hairFrameCounter > 4)
            {
                hairFrameCounter = 0;
                hairFrame++;
            }
            if (hairFrame > 10)
            {
                hairFrame = 7;
            }

            if (auraFrame < 11)
            {
                auraFrame = 11;
            }
            if (++auraFrameCounter > 4)
            {
                auraFrameCounter = 0;
                auraFrame++;
            }
            if (auraFrame > 14)
            {
                auraFrame = 11;
            }
        }
    }
}


