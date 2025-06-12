using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria;
using Terraria.ID;
using Terraria.Utilities;
using TouhouPets.Content.Buffs.PetBuffs;

namespace TouhouPets.Content.Projectiles.Pets
{
    public class Reimu : BasicTouhouPet
    {
        private enum States
        {
            Idle,
            Blink,
            BeforeNap,
            Nap,
            WakeUp,
            WakeUp2,
            AfterWakeUp,
            Shining,
            Flying,
            FlyingBlink,
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
        private bool IsNapState => CurrentState >= States.BeforeNap && CurrentState <= States.AfterWakeUp;

        private DrawPetConfig drawConfig = new(2);
        private readonly Texture2D clothTex = AltVanillaFunction.GetExtraTexture("Reimu_Cloth");
        private readonly Texture2D newYearClothTex = AltVanillaFunction.GetExtraTexture("Reimu_Cloth_NewYear");

        private int blinkFrame, blinkFrameCounter;
        private int clothFrame, clothFrameCounter;
        private int itemFrame, itemFrameCounter;
        private int flyTimeleft = 0;
        private bool seeCoin = false;
        public override void SetStaticDefaults()
        {
            Main.projFrames[Type] = 14;
            Main.projPet[Type] = true;

            ProjectileID.Sets.CharacterPreviewAnimations[Type] =
                ProjectileID.Sets.SimpleLoop(0, 1)
                .WhenSelected(12, 2, 5);
        }
        public override TouhouPetID UniqueID => TouhouPetID.Reimu;
        public override bool OnMouseHover(ref bool dontInvis)
        {
            Item coin = Owner.inventory[Owner.selectedItem];
            if ((coin.type == ItemID.GoldCoin || coin.type == ItemID.PlatinumCoin)
                && coin.stack > 0)
            {
                seeCoin = true;
            }

            dontInvis = seeCoin;
            return false;
        }
        public override bool DrawPetSelf(ref Color lightColor)
        {
            bool isFebrary = DateTime.Now.Month == 2;
            Texture2D cloth = isFebrary ? newYearClothTex : clothTex;

            DrawPetConfig config = drawConfig with
            {
                ShouldUseEntitySpriteDraw = true,
                AltTexture = cloth,
            };

            Projectile.DrawPet(Projectile.frame, lightColor, drawConfig);

            if (CurrentState == States.Blink || CurrentState == States.FlyingBlink)
                Projectile.DrawPet(blinkFrame, lightColor, drawConfig, 1);

            Projectile.DrawPet(Projectile.frame, lightColor, config);
            Projectile.ResetDrawStateForPet();

            if (Projectile.frame >= 12)
                return false;

            if (CurrentState != States.Shining)
            {
                Projectile.DrawPet(itemFrame, lightColor,
                    config with
                    {
                        ShouldUseEntitySpriteDraw = false,
                    }, 1);
            }
            Projectile.DrawPet(clothFrame, lightColor, config, 1);

            return false;
        }
        public override ChatSettingConfig ChatSettingConfig => new ChatSettingConfig() with
        {
            TextColor = new(255, 120, 120),
        };
        private const int PresetMaxChat = 50;
        public override void RegisterChat(ref string name, ref Vector2 indexRange)
        {
            name = "Reimu";
            indexRange = new Vector2(1, PresetMaxChat);
        }
        public override void PostRegisterChat()
        {
            this.RegisterComments();
        }
        public override void SetRegularDialog(ref int timePerDialog, ref int chance, ref bool whenShouldStop)
        {
            timePerDialog = 840;//840
            chance = 6;//6
            whenShouldStop = !IsIdleState;
        }
        public override WeightedRandom<string> RegularDialogText()
        {
            WeightedRandom<string> chat = new();
            {
                if (Main.bloodMoon || Main.eclipse || Main.slimeRain)
                {
                    chat.Add(ChatDictionary[4]);
                    chat.Add(ChatDictionary[10]);
                }
                else
                {
                    this.Comment_TouhouLightPet(chat);

                    chat.Add(ChatDictionary[1]);
                    chat.Add(ChatDictionary[2]);
                    if (FindPet(ProjectileType<Marisa>()))
                    {
                        chat.Add(ChatDictionary[13]);
                    }
                }
            }
            return chat;
        }
        public override void OnFindBoss(NPC boss)
        {
            Projectile.SetChat(ChatSettingConfig, 3);
        }
        public override void VisualEffectForPreview()
        {
            UpdateMiscFrame();
        }
        private void UpdateTalking()
        {
            if (FindChatIndex(13, 16))
            {
                Chatting1(currentChatRoom ?? Projectile.CreateChatRoomDirect());
            }
        }
        private void Chatting1(PetChatRoom chatRoom)
        {
            int type = ProjectileType<Marisa>();
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
            Projectile reimu = chatRoom.initiator;
            Projectile marisa = chatRoom.member[0];
            int turn = chatRoom.chatTurn;
            if (turn == -1)
            {
                //灵梦：喂，魔理沙？
                marisa.CloseCurrentDialog();

                if (reimu.CurrentDialogFinished())
                    chatRoom.chatTurn++;
            }
            else if (turn == 0)
            {
                //魔理沙：怎么了灵梦？
                marisa.SetChat(ChatSettingConfig, 15, 20);

                if (marisa.CurrentDialogFinished())
                    chatRoom.chatTurn++;
            }
            else if (turn == 1)
            {
                //灵梦：你说，如果我们其实是什么人被制造出来的、并且存在的目的是为了哪个世界的延续，你会怎么想？
                reimu.SetChat(ChatSettingConfig, 14, 20);

                if (reimu.CurrentDialogFinished())
                    chatRoom.chatTurn++;
            }
            else if (turn == 2)
            {
                //魔理沙：呃啊...怎么突然说这个？感觉怪怪的...
                marisa.SetChat(ChatSettingConfig, 16, 20);

                if (marisa.CurrentDialogFinished())
                    chatRoom.chatTurn++;
            }
            else if (turn == 3)
            {
                //灵梦：你就说你会怎么想嘛！
                reimu.SetChat(ChatSettingConfig, 15, 20);

                if (reimu.CurrentDialogFinished())
                    chatRoom.chatTurn++;
            }
            else if (turn == 4)
            {
                //魔理沙：嗯...感觉、挺好的？这说明我们肩负着伟大的使命daze！
                marisa.SetChat(ChatSettingConfig, 17, 20);

                if (marisa.CurrentDialogFinished())
                    chatRoom.chatTurn++;
            }
            else if (turn == 5)
            {
                //灵梦：也许吧...
                reimu.SetChat(ChatSettingConfig, 16, 20);

                if (reimu.CurrentDialogFinished())
                    chatRoom.chatTurn++;
            }
            else
            {
                chatRoom.CloseChatRoom();
            }
        }
        public override void AI()
        {
            Projectile.SetPetActive(Owner, BuffType<ReimuBuff>());

            UpdateTalking();

            ControlMovement();

            if (IsNapState && FindBoss)
            {
                CurrentState = States.Idle;
            }
            if (Owner.velocity.Length() > 15f)
            {
                flyTimeleft = 5;
                if (OwnerIsMyPlayer && CurrentState < States.Flying)
                {
                    Timer = 0;
                    CurrentState = States.Flying;
                }
            }

            switch (CurrentState)
            {
                case States.Blink:
                    Blink();
                    break;

                case States.BeforeNap:
                    shouldNotTalking = true;
                    BeforeNap();
                    break;

                case States.Nap:
                    shouldNotTalking = true;
                    Nap();
                    break;

                case States.WakeUp:
                    shouldNotTalking = true;
                    WakeUp();
                    break;

                case States.WakeUp2:
                    shouldNotTalking = true;
                    WakeUp2();
                    break;

                case States.AfterWakeUp:
                    shouldNotTalking = true;
                    AfterWakeUp();
                    break;

                case States.Shining:
                    shouldNotTalking = true;
                    Shining();
                    break;

                case States.Flying:
                    Flying();
                    break;

                case States.FlyingBlink:
                    Flying();
                    FlyingBlink();
                    break;

                default:
                    Idle();
                    break;
            }

            if (IsIdleState && ActionCD > 0)
            {
                ActionCD--;
            }

            if (flyTimeleft > 0)
            {
                flyTimeleft--;
            }

            if (seeCoin && IsIdleState)
            {
                CurrentState = States.Shining;
            }
            seeCoin = false;
        }
        private void ControlMovement()
        {
            Projectile.tileCollide = false;
            if (CurrentState != States.Shining)
            {
                Projectile.rotation = Projectile.velocity.X * 0.012f;
            }

            if (CurrentState == States.Nap)
                return;

            ChangeDir();

            Vector2 point = new Vector2(-50 * Owner.direction, -30 + Owner.gfxOffY);
            MoveToPoint(point, 22f);
        }
        private void Idle()
        {
            Projectile.frame = 0;
            if (OwnerIsMyPlayer)
            {
                if (mainTimer % 270 == 0)
                {
                    CurrentState = States.Blink;
                }
                if (mainTimer > 0 && mainTimer % 240 == 0 && currentChatRoom == null && ActionCD <= 0
                     && Owner.velocity.Length() == 0 && !FindBoss)
                {
                    int chance = 11;
                    if (Main.bloodMoon || Main.eclipse || Main.slimeRain)
                    {
                        chance = 30;
                    }
                    else if (Main.dayTime || Main.raining)
                    {
                        chance = 6;
                    }
                    else if (Owner.sleeping.FullyFallenAsleep)
                    {
                        chance = 2;
                    }
                    if (Main.rand.NextBool(chance))
                    {
                        RandomCount = Main.rand.Next(320, 560);
                        CurrentState = States.BeforeNap;

                        if (Main.rand.NextBool(8) && chatTimeLeft <= 0)
                            Projectile.SetChat(ChatSettingConfig, 11);
                    }
                }
            }
        }
        private void Blink()
        {
            Projectile.frame = 0;
            if (++blinkFrameCounter > 3)
            {
                blinkFrameCounter = 0;
                blinkFrame++;
            }
            if (blinkFrame > 2)
            {
                blinkFrame = 0;
                CurrentState = States.Idle;
            }
        }
        private void BeforeNap()
        {
            Projectile.velocity *= 0.5f;
            Projectile.frame = 1;

            if (seeCoin)
            {
                CurrentState = States.Idle;
                return;
            }

            Timer++;
            if (OwnerIsMyPlayer && Timer > RandomCount)
            {
                Timer = 0;
                CurrentState = States.Nap;
            }
        }
        private void Nap()
        {
            if (++Projectile.frameCounter > 6)
            {
                Projectile.frameCounter = 0;
                Projectile.frame++;
            }
            Projectile.velocity *= 0.1f;
            if (Projectile.frame >= 3)
            {
                Projectile.frame = 3;
            }
            if (OwnerIsMyPlayer)
            {
                if (Projectile.Distance(Owner.position) > 300f || seeCoin)
                {
                    RandomCount = Main.rand.Next(2, 4);
                    CurrentState = States.WakeUp;
                    return;
                }
                if (mainTimer % 320 == 0 && Main.rand.NextBool(3) && !Owner.sleeping.FullyFallenAsleep)
                {
                    if (Main.rand.NextBool(2))
                    {
                        RandomCount = Main.rand.Next(2, 4);
                        CurrentState = States.WakeUp;
                    }
                    else
                    {
                        RandomCount = Main.rand.Next(320, 560);
                        CurrentState = States.BeforeNap;
                    }
                }
            }
        }
        private void WakeUp()
        {
            if (++Projectile.frameCounter > 6)
            {
                Projectile.frameCounter = 0;
                Projectile.frame++;
            }
            Projectile.velocity *= 0.5f;
            if (Projectile.frame == 5)
            {
                Projectile.frame = 6;
            }
            if (Projectile.frame > 6)
            {
                Projectile.frame = 4;
                Timer++;
            }
            if (OwnerIsMyPlayer && Timer > RandomCount)
            {
                Timer = 0;
                RandomCount = Main.rand.Next(40, 60);
                CurrentState = States.WakeUp2;
            }
        }
        private void WakeUp2()
        {
            Projectile.frame = 5;
            Projectile.velocity *= 0.75f;

            Timer++;
            if (OwnerIsMyPlayer && Timer > RandomCount)
            {
                Timer = 0;
                RandomCount = Main.rand.Next(6, 9);
                CurrentState = States.AfterWakeUp;
            }
        }
        private void AfterWakeUp()
        {
            if (++Projectile.frameCounter > 2)
            {
                Projectile.frameCounter = 0;
                Projectile.frame++;
            }
            if (Projectile.frame > 8)
            {
                Projectile.frame = 7;
                Timer++;
            }
            if (OwnerIsMyPlayer && Timer > RandomCount)
            {
                Timer = 0;
                ActionCD = 600;
                CurrentState = States.Idle;
            }
        }
        private void Shining()
        {
            if (++Projectile.frameCounter > 5)
            {
                Projectile.frameCounter = 0;
                Projectile.frame++;
            }
            if (Projectile.frame < 9)
            {
                Projectile.frame = 9;
            }
            if (Projectile.frame > 11)
            {
                Projectile.frame = 10;
            }
            if (OwnerIsMyPlayer && !seeCoin)
            {
                if (Projectile.frame >= 11)
                    CurrentState = States.Idle;
            }
        }
        private void Flying()
        {
            if (OwnerIsMyPlayer)
            {
                if (flyTimeleft <= 0)
                {
                    ActionCD = 600;
                    CurrentState = States.Idle;
                    return;
                }
                if (mainTimer % 270 == 0 && CurrentState == States.Flying)
                {
                    CurrentState = States.FlyingBlink;
                }
            }
            if (Projectile.frame < 12)
            {
                Projectile.frame = 12;
            }
            if (++Projectile.frameCounter > 3)
            {
                Projectile.frameCounter = 0;
                Projectile.frame++;
            }
            if (Projectile.frame > 13)
            {
                Projectile.frame = 12;
            }
        }
        private void FlyingBlink()
        {
            if (++blinkFrameCounter > 3)
            {
                blinkFrameCounter = 0;
                blinkFrame++;
            }
            if (blinkFrame > 2)
            {
                blinkFrame = 0;
                CurrentState = States.Flying;
            }
        }
        private void UpdateMiscFrame()
        {
            if (clothFrame < 3)
            {
                clothFrame = 3;
            }
            int count = 5;
            if (++clothFrameCounter > count)
            {
                clothFrameCounter = 0;
                clothFrame++;
            }
            if (clothFrame > 6)
            {
                clothFrame = 3;
            }

            if (itemFrame < 7)
            {
                itemFrame = 7;
            }
            count = 8;
            if (++itemFrameCounter > count)
            {
                itemFrameCounter = 0;
                itemFrame++;
            }
            if (itemFrame > 10)
            {
                itemFrame = 7;
            }
        }
    }
}


