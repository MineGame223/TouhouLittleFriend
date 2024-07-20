using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.Graphics.Shaders;
using Terraria.ID;
using Terraria.Utilities;
using TouhouPets.Content.Buffs.PetBuffs;

namespace TouhouPets.Content.Projectiles.Pets
{
    public class Yukari : BasicTouhouPet
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

        private int gapFrame, gapFrameCounter;
        private int blinkFrame, blinkFrameCounter;
        private int clothFrame, clothFrameCounter;
        private int hairFrame, hairFrameCounter;

        private DrawPetConfig drawConfig = new(2);
        private readonly Texture2D clothTex = AltVanillaFunction.GetExtraTexture("Yukari_Cloth");
        public override void SetStaticDefaults()
        {
            Main.projFrames[Type] = 10;
            Main.projPet[Type] = true;
            ProjectileID.Sets.LightPet[Type] = true;
        }
        public override bool DrawPetSelf(ref Color lightColor)
        {
            DrawPetConfig config = drawConfig with
            {
                ShouldUseEntitySpriteDraw = true,
            };

            DrawGap(lightColor);
            Projectile.ResetDrawStateForPet();

            Projectile.DrawPet(hairFrame, lightColor, drawConfig);
            Projectile.DrawPet(Projectile.frame, lightColor, drawConfig);

            if (CurrentState == States.Blink)
                Projectile.DrawPet(blinkFrame, lightColor, drawConfig, 1);

            Projectile.DrawPet(Projectile.frame, lightColor,
                config with
                {
                    AltTexture = clothTex,
                });
            Projectile.DrawPet(clothFrame, lightColor, config, 1);
            return false;
        }
        private void DrawGap(Color lightColor)
        {
            DrawPetConfig config = drawConfig with
            {
                AltTexture = clothTex,
                ShouldUseEntitySpriteDraw = true,
            };

            for (int i = 0; i < 4; i++)
            {
                Vector2 gapPos = new Vector2(0, -2 * Main.essScale).RotatedBy(MathHelper.PiOver2 * i);
                Projectile.DrawPet(gapFrame, Color.Purple * 0.2f,
                    drawConfig with
                    {
                        PositionOffset = gapPos,
                    });
                Projectile.DrawPet(gapFrame, Color.Purple * 0.2f,
                   config with
                   {
                       PositionOffset = gapPos,
                   });
            }
            Projectile.ResetDrawStateForPet();

            Projectile.DrawPet(gapFrame, Color.White * 0.9f * Main.essScale, drawConfig);
            Projectile.DrawPet(gapFrame, lightColor, config);
        }
        public override Color ChatTextColor => new Color(156, 91, 250);
        public override void RegisterChat(ref string name, ref Vector2 indexRange)
        {
            name = "Yukari";
            indexRange = new Vector2(1, 6);
        }
        public override void SetRegularDialog(ref int timePerDialog, ref int chance, ref bool whenShouldStop)
        {
            timePerDialog = 940;
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
                chat.Add(ChatDictionary[5]);
            }
            return chat;
        }
        private void UpdateTalking()
        {
            if (FindChatIndex(5, 6))
            {
                Chatting1(currentChatRoom ?? Projectile.CreateChatRoomDirect());
            }
        }
        private void Chatting1(PetChatRoom chatRoom)
        {
            int type = ProjectileType<Ran>();
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
            Projectile yukari = chatRoom.initiator;
            Projectile ran = chatRoom.member[0];
            int turn = chatRoom.chatTurn;
            if (turn == -1)
            {
                //紫：不知那位旧友最近是否安好呢。
                ran.CloseCurrentDialog();

                if (yukari.CurrentDialogFinished())
                    chatRoom.chatTurn++;
            }
            else if (turn == 0)
            {
                //蓝：紫大人是指？
                ran.SetChat(ChatSettingConfig, 5, 20);

                if (ran.CurrentDialogFinished())
                    chatRoom.chatTurn++;
            }
            else if (turn == 1)
            {
                //紫：没什么，吃这种事应该用不着我替她操心。
                yukari.SetChat(ChatSettingConfig, 6, 20);

                if (yukari.CurrentDialogFinished())
                    chatRoom.chatTurn++;
            }
            else if (turn == 2)
            {
                //蓝：大概知道是哪位了啊...
                ran.SetChat(ChatSettingConfig, 6, 20);

                if (ran.CurrentDialogFinished())
                    chatRoom.chatTurn++;
            }
            else
            {
                chatRoom.CloseChatRoom();
            }
        }
        public override void VisualEffectForPreview()
        {
            UpdateGapFrame();
            UpdateMiscFrame();
        }
        public override void SetPetLight(ref Vector2 position, ref Vector3 rgb, ref bool inactive)
        {
            rgb = new Vector3(1.56f, 0.91f, 2.50f);
        }
        public override void AI()
        {
            Projectile.SetPetActive(Owner, BuffType<YukariBuff>());

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
            Projectile.rotation = Projectile.velocity.X * 0.002f;

            ChangeDir();

            Vector2 point = new Vector2(60 * Owner.direction, -30 + Owner.gfxOffY);
            MoveToPoint(point, 18f);
        }
        private void GenDust()
        {
            if (Main.rand.NextBool(7))
            {
                Dust d = Dust.NewDustPerfect(Projectile.position + new Vector2(Main.rand.Next(0, Projectile.width), Main.rand.Next(0, Projectile.height)), MyDustId.PurpleLight
                    , new Vector2(Main.rand.NextFloat(-1f, 1f), Main.rand.NextFloat(-1f, 1f)), 100, default
                    , Main.rand.NextFloat(1f, 2f));
                d.noGravity = true;
                d.shader = GameShaders.Armor.GetSecondaryShader(Owner.cLight, Owner);
            }
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
        private void UpdateGapFrame()
        {
            if (gapFrame < 8)
            {
                gapFrame = 8;
            }
            int count = 11;
            if (++gapFrameCounter > count)
            {
                gapFrameCounter = 0;
                gapFrame++;
            }
            if (gapFrame > 9)
            {
                gapFrame = 8;
            }
        }
        private void UpdateMiscFrame()
        {
            int count = 5;
            if (++clothFrameCounter > count)
            {
                clothFrameCounter = 0;
                clothFrame++;
            }
            if (clothFrame > 3)
            {
                clothFrame = 0;
            }

            if (hairFrame < 4)
            {
                hairFrame = 4;
            }
            if (++hairFrameCounter > count)
            {
                hairFrameCounter = 0;
                hairFrame++;
            }
            if (hairFrame > 7)
            {
                hairFrame = 4;
            }
        }
    }
}


