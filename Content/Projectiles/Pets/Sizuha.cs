using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ID;
using Terraria.Utilities;
using TouhouPets.Content.Buffs.PetBuffs;

namespace TouhouPets.Content.Projectiles.Pets
{
    public class Sizuha : BasicTouhouPet
    {
        private enum States
        {
            Idle,
            Blink,
            Cold,
            ColdBlink,
        }
        private States CurrentState
        {
            get => (States)PetState;
            set => PetState = (int)value;
        }
        private bool IsIdleState => PetState <= 1;

        private int blinkFrame, blinkFrameCounter;
        private int clothFrame, clothFrameCounter;

        private DrawPetConfig drawConfig = new(1);
        private readonly Texture2D clothTex = AltVanillaFunction.GetExtraTexture("Sizuha_Cloth");
        private readonly Texture2D glowTex = AltVanillaFunction.GetGlowTexture("Sizuha_Glow");
        public override void SetStaticDefaults()
        {
            Main.projFrames[Type] = 12;
            Main.projPet[Type] = true;
            ProjectileID.Sets.LightPet[Type] = true;
        }

        public override bool DrawPetSelf(ref Color lightColor)
        {
            DrawPetConfig config = drawConfig with
            {
                ShouldUseEntitySpriteDraw = true,
            };

            Projectile.DrawPet(Projectile.frame, lightColor, drawConfig);

            if (CurrentState == States.Blink || CurrentState == States.ColdBlink)
                Projectile.DrawPet(blinkFrame, lightColor, drawConfig);

            Projectile.DrawPet(clothFrame, lightColor, config);
            Projectile.DrawPet(Projectile.frame, lightColor,
                config with
                {
                    AltTexture = clothTex,
                });
            Projectile.ResetDrawStateForPet();

            Projectile.DrawPet(Projectile.frame, Color.White * 0.7f,
                drawConfig with
                {
                    AltTexture = glowTex,
                });
            return false;
        }
        public override Color ChatTextColor => new Color(244, 150, 91);
        public override void RegisterChat(ref string name, ref Vector2 indexRange)
        {
            name = "Sizuha";
            indexRange = new Vector2(1, 9);
        }
        public override void SetRegularDialog(ref int timePerDialog, ref int chance, ref bool whenShouldStop)
        {
            timePerDialog = 720;
            chance = 8;
            whenShouldStop = false;
        }
        public override string GetRegularDialogText()
        {
            WeightedRandom<string> chat = new WeightedRandom<string>();
            {
                if (!IsIdleState)
                {
                    chat.Add(ChatDictionary[9]);
                }
                else
                {
                    chat.Add(ChatDictionary[1]);
                    chat.Add(ChatDictionary[2]);
                    chat.Add(ChatDictionary[3]);
                    chat.Add(ChatDictionary[4]);
                    chat.Add(ChatDictionary[5]);
                    chat.Add(ChatDictionary[6]);
                }
            }
            return chat;
        }
        private void UpdateTalking()
        {
            if (FindChatIndex(6, 8))
            {
                Chatting1(currentChatRoom ?? Projectile.CreateChatRoomDirect());
            }
        }
        private void Chatting1(PetChatRoom chatRoom)
        {
            int type = ProjectileType<Minoriko>();
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
            Projectile sizuha = chatRoom.initiator;
            Projectile minoriko = chatRoom.member[0];
            int turn = chatRoom.chatTurn;
            if (turn == -1)
            {
                //静叶：明明我才是姐姐，为什么人气会赶不上妹妹呢...
                minoriko.CloseCurrentDialog();

                if (sizuha.CurrentDialogFinished())
                    chatRoom.chatTurn++;
            }
            else if (turn == 0)
            {
                //穰子：毕竟对于人类来说吃饱饭才是第一位吧...不过也有很多人喜欢秋天的落叶呢！
                minoriko.SetChat(ChatSettingConfig, 6, 20);

                if (minoriko.CurrentDialogFinished())
                    chatRoom.chatTurn++;
            }
            else if (turn == 1)
            {
                //静叶：...那、那是当然了，落叶好歹也是组成秋天的重要部分啦！
                sizuha.SetChat(ChatSettingConfig, 7, 20);

                if (sizuha.CurrentDialogFinished())
                    chatRoom.chatTurn++;
            }
            else if (turn == 2)
            {
                //穰子：那姐姐要吃烤红薯吗？
                minoriko.SetChat(ChatSettingConfig, 7, 20);

                if (minoriko.CurrentDialogFinished())
                    chatRoom.chatTurn++;
            }
            else if (turn == 3)
            {
                //静叶：好欸！...我是说，好啊。
                sizuha.SetChat(ChatSettingConfig, 8, 20);

                if (sizuha.CurrentDialogFinished())
                    chatRoom.chatTurn++;
            }
            else
            {
                chatRoom.CloseChatRoom();
            }
        }
        public override void VisualEffectForPreview()
        {
            UpdateClothFrame();
            if (IsIdleState)
            {
                IdleAnimation();
            }
        }
        public override void SetPetLight(ref Vector2 position, ref Vector3 rgb, ref bool inactive)
        {
            rgb = new Vector3(1.61f, 0.98f, 0.58f);
        }
        public override void AI()
        {
            Projectile.SetPetActive(Owner, BuffType<SizuhaBuff>());

            UpdateTalking();

            ControlMovement();

            SpawnFallingLeaves();

            switch (CurrentState)
            {
                case States.Blink:
                    Blink();
                    break;

                case States.Cold:
                    shouldNotTalking = true;
                    Cold();
                    break;

                case States.ColdBlink:
                    shouldNotTalking = true;
                    ColdBlink();
                    break;

                default:
                    Idle();
                    break;
            }
        }
        private void SpawnFallingLeaves()
        {
            if (!OwnerIsMyPlayer || !IsIdleState)
                return;

            if (Main.rand.NextBool(16) && Projectile.velocity.Length() > 3f)
            {
                Projectile.NewProjectile(Projectile.GetSource_FromThis(), Projectile.Center + new Vector2(Main.rand.Next(-20, 20), Main.rand.Next(0, 50))
                            , new Vector2(0, Main.rand.NextFloat(0.3f, 0.4f)), ProjectileType<SizuhaLeaf>(), 0, 0, Main.myPlayer, Main.rand.Next(0, 3));
            }
        }
        private void ControlMovement()
        {
            Projectile.tileCollide = false;
            Projectile.rotation = Projectile.velocity.X * 0.02f;

            ChangeDir();

            Vector2 point = new Vector2(50 * Owner.direction, -30 + Owner.gfxOffY);
            MoveToPoint(point, 10f);
        }
        private void Idle()
        {
            if (OwnerIsMyPlayer)
            {
                if (Owner.ZoneSnow)
                {
                    CurrentState = States.Cold;
                }
                if (mainTimer % 270 == 0)
                {
                    CurrentState = States.Blink;
                }
            }
        }
        private void Blink()
        {
            int startFrame = 8;
            if (blinkFrame < startFrame)
            {
                blinkFrame = startFrame;
            }
            if (++blinkFrameCounter > 3)
            {
                blinkFrameCounter = 0;
                blinkFrame++;
            }
            if (blinkFrame > 10)
            {
                blinkFrame = startFrame;
                CurrentState = States.Idle;
            }
        }
        private void Cold()
        {
            Projectile.frame = 11;
            if (OwnerIsMyPlayer)
            {
                if (!Owner.ZoneSnow)
                {
                    CurrentState = States.Idle;
                }
                if (mainTimer % 270 == 0)
                {
                    CurrentState = States.ColdBlink;
                }
            }
        }
        private void ColdBlink()
        {
            Projectile.frame = 11;
            int startFrame = 9;
            if (blinkFrame < startFrame)
            {
                blinkFrame = startFrame;
            }
            if (++blinkFrameCounter > 3)
            {
                blinkFrameCounter = 0;
                blinkFrame++;
            }
            if (blinkFrame > 10)
            {
                blinkFrame = startFrame;
                CurrentState = States.Cold;
            }
        }
        private void IdleAnimation()
        {
            if (++Projectile.frameCounter > 8)
            {
                Projectile.frameCounter = 0;
                Projectile.frame++;
            }
            if (Projectile.frame > 3)
            {
                Projectile.frame = 0;
            }
        }
        private void UpdateClothFrame()
        {
            if (clothFrame < 4)
            {
                clothFrame = 4;
            }
            if (++clothFrameCounter > 6)
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


