using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ID;
using Terraria.Utilities;
using TouhouPets.Content.Buffs.PetBuffs;

namespace TouhouPets.Content.Projectiles.Pets
{
    public class Shinki : BasicTouhouPet
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
        public override void SetStaticDefaults()
        {
            Main.projFrames[Type] = 12;
            Main.projPet[Type] = true;
            ProjectileID.Sets.LightPet[Type] = true;
        }

        private int wingFrame, wingFrameCounter;
        private int clothFrame, clothFrameCounter;
        private int hairFrame, hairFrameCounter;
        private int blinkFrame, blinkFrameCounter;

        private DrawPetConfig drawConfig = new(2);
        private readonly Texture2D clothTex = AltVanillaFunction.GetExtraTexture("Shinki_Cloth");

        public override bool DrawPetSelf(ref Color lightColor)
        {
            Main.spriteBatch.QuickEndAndBegin(true, Projectile.isAPreviewDummy, BlendState.Additive);
            for (int i = 0; i < 4; i++)
            {
                DrawShinki(Color.Purple * 0.8f, new Vector2(6 * Main.essScale, 0)
                    .RotatedBy(MathHelper.ToRadians(90 * i) + Main.GlobalTimeWrappedHourly));
            }
            Projectile.ResetDrawStateForPet();

            DrawShinki(lightColor, Vector2.Zero);
            return false;
        }

        private void DrawShinki(Color lightColor, Vector2 extraPos)
        {
            DrawPetConfig config = drawConfig with
            {
                PositionOffset = extraPos,
            };
            DrawPetConfig config2 = config with
            {
                ShouldUseEntitySpriteDraw = true,
            };

            Projectile.DrawPet(wingFrame, lightColor, config);
            Projectile.DrawPet(hairFrame, lightColor, config, 1);

            Projectile.DrawPet(Projectile.frame, lightColor, config);

            if (CurrentState == States.Blink)
                Projectile.DrawPet(blinkFrame, lightColor, config, 1);

            Projectile.DrawPet(Projectile.frame, lightColor,
                config2 with
                {
                    AltTexture = clothTex,
                });
            Projectile.DrawPet(clothFrame, lightColor, config2, 1);
        }
        public override Color ChatTextColor => new Color(255, 110, 110);
        public override void RegisterChat(ref string name, ref Vector2 indexRange)
        {
            name = "Shinki";
            indexRange = new Vector2(1, 10);
        }
        public override void SetRegularDialog(ref int timePerDialog, ref int chance, ref bool whenShouldStop)
        {
            timePerDialog = 720;
            chance = 9;
            whenShouldStop = false;
        }
        public override string GetRegularDialogText()
        {
            WeightedRandom<string> chat = new WeightedRandom<string>();
            {
                chat.Add(ChatDictionary[1]);
                chat.Add(ChatDictionary[2]);
                chat.Add(ChatDictionary[3]);
                chat.Add(ChatDictionary[4]);
                if (!Owner.HasBuff<AliceBuff>())
                {
                    chat.Add(ChatDictionary[5]);
                }
                if (FindPet(ProjectileType<Alice>()))
                {
                    chat.Add(ChatDictionary[6]);
                }
            }
            return chat;
        }
        private void UpdateTalking()
        {
            if (FindChatIndex(1))
            {
                Chatting1(currentChatRoom ?? Projectile.CreateChatRoomDirect());
            }
            if (FindChatIndex(6, 10))
            {
                Chatting2(currentChatRoom ?? Projectile.CreateChatRoomDirect());
            }
        }
        private void Chatting1(PetChatRoom chatRoom)
        {
            int type = ProjectileType<Reimu>();
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
            Projectile shinki = chatRoom.initiator;
            Projectile reimu = chatRoom.member[0];
            int turn = chatRoom.chatTurn;
            if (turn == -1)
            {
                //神绮：所谓“巫女”，不过是神明的狗罢了~
                reimu.CloseCurrentDialog();

                if (shinki.CurrentDialogFinished())
                    chatRoom.chatTurn++;
            }
            else if (turn == 0)
            {
                //灵梦：*优美的博丽脏话
                reimu.SetChat(ChatSettingConfig, 12, 20);

                if (reimu.CurrentDialogFinished())
                    chatRoom.chatTurn++;
            }
            else
            {
                chatRoom.CloseChatRoom();
            }
        }
        private void Chatting2(PetChatRoom chatRoom)
        {
            int type = ProjectileType<Alice>();
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
            Projectile shinki = chatRoom.initiator;
            Projectile alice = chatRoom.member[0];
            int turn = chatRoom.chatTurn;
            if (turn == -1)
            {
                //神绮：爱~丽~丝~酱~~
                alice.CloseCurrentDialog();

                if (shinki.CurrentDialogFinished())
                    chatRoom.chatTurn++;
            }
            else if (turn == 0)
            {
                //爱丽丝：别那么肉麻的叫我！...咱们、咱们认识吗？
                alice.SetChat(ChatSettingConfig, 11, 20);

                if (alice.CurrentDialogFinished())
                    chatRoom.chatTurn++;
            }
            else if (turn == 1)
            {
                //神绮：别这样嘛！你难道连你亲爱的妈妈都不认识了？
                shinki.SetChat(ChatSettingConfig, 7, 20);

                if (shinki.CurrentDialogFinished())
                    chatRoom.chatTurn++;
            }
            else if (turn == 2)
            {
                //爱丽丝：不熟，真的不熟...
                alice.SetChat(ChatSettingConfig, 12, 20);

                if (alice.CurrentDialogFinished())
                    chatRoom.chatTurn++;
            }
            else if (turn == 3)
            {
                //神绮：呜呜呜...被女儿冷落了...
                shinki.SetChat(ChatSettingConfig, 8, 20);

                if (shinki.CurrentDialogFinished())
                    chatRoom.chatTurn++;
            }
            else if (turn == 4)
            {
                //爱丽丝：...好啦好啦，真受不了，我叫你一声母...神绮小姐。
                alice.SetChat(ChatSettingConfig, 13, 20);

                if (alice.CurrentDialogFinished())
                    chatRoom.chatTurn++;
            }
            else if (turn == 5)
            {
                //神绮：你刚刚是不是说“母亲”了？哇！妈妈好感动！
                shinki.SetChat(ChatSettingConfig, 9, 20);

                if (shinki.CurrentDialogFinished())
                    chatRoom.chatTurn++;
            }
            else if (turn == 6)
            {
                //爱丽丝：（真是说不清道不明的关系啊...）
                alice.SetChat(ChatSettingConfig, 14, 20);

                if (alice.CurrentDialogFinished())
                    chatRoom.chatTurn++;
            }
            else if (turn == 7)
            {
                //神绮：再叫一声好不好？
                shinki.SetChat(ChatSettingConfig, 10, 20);

                if (shinki.CurrentDialogFinished())
                    chatRoom.chatTurn++;
            }
            else if (turn == 8)
            {
                //爱丽丝：不、不行！
                alice.SetChat(ChatSettingConfig, 15, 20);

                if (alice.CurrentDialogFinished())
                    chatRoom.chatTurn++;
            }
            else
            {
                chatRoom.CloseChatRoom();
            }
        }
        public override void VisualEffectForPreview()
        {
            IdleAnimation();
            UpdateMiscFrame();
        }
        public override void SetPetLight(ref Vector2 position, ref Vector3 rgb, ref bool inactive)
        {
            rgb = new Vector3(1.69f, 1.03f, 2.46f) * 1.5f;
        }
        public override void AI()
        {
            Projectile.SetPetActive(Owner, BuffType<ShinkiBuff>());

            UpdateTalking();

            ControlMovement();

            GenDust();

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
            Projectile.rotation = Projectile.velocity.X * 0.012f;

            ChangeDir();

            Vector2 point = new Vector2(60 * Owner.direction, -50 + Owner.gfxOffY);
            MoveToPoint(point, 18f);
        }
        private void GenDust()
        {
            int dustID = MyDustId.PurpleShortFx;
            if (Main.rand.NextBool(15))
            {
                Dust d = Dust.NewDustPerfect(Projectile.Center
                + new Vector2(Main.rand.NextFloat(-30f, -10f) * Projectile.spriteDirection, Main.rand.NextFloat(-5f, 25f)), dustID
                , new Vector2(Main.rand.NextFloat(-0.2f, 0.2f), Main.rand.NextFloat(0.8f, 1.2f)), 100, default
                , Main.rand.NextFloat(1f, 1.2f));
                d.noGravity = true;
            }
            if (Main.rand.NextBool(15))
            {
                Dust d = Dust.NewDustPerfect(Projectile.Center
                    + new Vector2(Main.rand.NextFloat(20f, 22f) * Projectile.spriteDirection, Main.rand.NextFloat(-5f, 25f)), dustID
                    , new Vector2(Main.rand.NextFloat(-0.2f, 0.2f), Main.rand.NextFloat(0.8f, 1.2f)), 100, default
                    , Main.rand.NextFloat(1f, 1.2f));
                d.noGravity = true;
            }
        }
        private void Idle()
        {
            if (OwnerIsMyPlayer && mainTimer % 270 == 0)
            {
                CurrentState = States.Blink;
            }
        }
        private void Blink()
        {
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
        private void IdleAnimation()
        {
            Projectile.frame = 0;
        }
        private void UpdateMiscFrame()
        {
            if (wingFrame < 6)
            {
                wingFrame = 6;
            }
            if (++wingFrameCounter > 6)
            {
                wingFrameCounter = 0;
                wingFrame++;
            }
            if (wingFrame > 11)
            {
                wingFrame = 6;
            }

            if (hairFrame < 7)
            {
                hairFrame = 7;
            }
            if (++hairFrameCounter > 7)
            {
                hairFrameCounter = 0;
                hairFrame++;
            }
            if (hairFrame > 10)
            {
                hairFrame = 7;
            }

            if (clothFrame < 3)
            {
                clothFrame = 3;
            }
            if (++clothFrameCounter > 10)
            {
                clothFrameCounter = 0;
                clothFrame++;
            }
            if (clothFrame > 6)
            {
                clothFrame = 3;
            }
        }
    }
}


