using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;
using Terraria;
using Terraria.ID;
using Terraria.Utilities;
using TouhouPets.Content.Buffs.PetBuffs;

namespace TouhouPets.Content.Projectiles.Pets
{
    public class Marisa : BasicTouhouPet
    {
        private enum States
        {
            Idle,
            Blink,
            Happy,
            AfterHappy,
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

        private DrawPetConfig drawConfig = new(2);
        private readonly Texture2D clothTex = AltVanillaFunction.GetExtraTexture("Marisa_Cloth");

        private int blinkFrame, blinkFrameCounter;
        private int broomFrame, broomFrameCounter;
        private int lightFrame, lightFrameCounter;
        public override void SetStaticDefaults()
        {
            Main.projFrames[Type] = 15;
            Main.projPet[Type] = true;
            ProjectileID.Sets.LightPet[Type] = true;
        }
        public override TouhouPetID UniqueID => TouhouPetID.Marisa;
        public override bool DrawPetSelf(ref Color lightColor)
        {
            DrawPetConfig config = drawConfig with
            {
                ShouldUseEntitySpriteDraw = true,
            };

            Projectile.DrawPet(broomFrame, lightColor, config, 1);
            Projectile.ResetDrawStateForPet();

            Projectile.DrawPet(Projectile.frame, lightColor, drawConfig);

            if (CurrentState == States.Blink)
                Projectile.DrawPet(blinkFrame, lightColor, drawConfig);

            Projectile.DrawPet(Projectile.frame, lightColor,
                config with
                {
                    AltTexture = clothTex,
                });
            Projectile.ResetDrawStateForPet();

            Projectile.DrawPet(lightFrame, Color.White, drawConfig, 1);
            return false;
        }
        public override ChatSettingConfig ChatSettingConfig => new ChatSettingConfig() with
        {
            TextColor = new(255, 249, 137),
        };
        private const int PresetMaxChat = 50;
        public override void RegisterChat(ref string name, ref Vector2 indexRange)
        {
            name = "Marisa";
            indexRange = new Vector2(1, PresetMaxChat);
        }
        public override void PostRegisterChat()
        {
            this.RegisterComment_Vanilla();
            this.RegisterComment_Coralite();
            this.RegisterComment_Thorium();
            this.RegisterComment_HJ();
            this.RegisterComment_ByMod();
        }
        public override void SetRegularDialog(ref int timePerDialog, ref int chance, ref bool whenShouldStop)
        {
            timePerDialog = 720;//720
            chance = 5;//5
            whenShouldStop = !IsIdleState;
        }
        public override WeightedRandom<string> RegularDialogText()
        {
            WeightedRandom<string> chat = new();
            {
                chat.Add(ChatDictionary[1]);
                chat.Add(ChatDictionary[2]);
                chat.Add(ChatDictionary[3]);
                if (Owner.ZoneForest && Main.cloudAlpha == 0 && !Main.bloodMoon)
                {
                    if (Main.dayTime)
                        chat.Add(ChatDictionary[4]);
                    else
                        chat.Add(ChatDictionary[5]);
                }
                if (Main.bloodMoon || Main.eclipse)
                {
                    chat.Add(ChatDictionary[6]);
                }
                if (FindPet(ProjectileType<Reimu>()))
                {
                    chat.Add(ChatDictionary[7]);
                }
            }
            return chat;
        }
        public override void OnFindBoss(NPC boss)
        {
            Projectile.BossChat_ByMod(ChatSettingConfig, boss);

            Projectile.BossChat_Vanilla(ChatSettingConfig, boss);
            Projectile.BossChat_Coralite(ChatSettingConfig, boss);
            Projectile.BossChat_Thorium(ChatSettingConfig, boss);
            Projectile.BossChat_HomewardHourney(ChatSettingConfig, boss);
            Projectile.BossChat_Gensokyo(ChatSettingConfig, boss);
        }
        public override List<List<ChatRoomInfo>> RegisterChatRoom()
        {
            return new()
            {
                Chatting1(),
            };
        }
        private static List<ChatRoomInfo> Chatting1()
        {
            TouhouPetID marisa = TouhouPetID.Marisa;
            TouhouPetID reimu = TouhouPetID.Reimu;

            List<ChatRoomInfo> list =
            [
                new ChatRoomInfo(marisa, 7, -1), //魔理沙：嘿，嘿，灵梦！
                new ChatRoomInfo(reimu, 5, 0),//灵梦：咋了？
                new ChatRoomInfo(marisa, 8, 1), //魔理沙：我今天找到了一个很有意思的东西！
                new ChatRoomInfo(reimu, 6, 2),//灵梦：是什么？不会又是从图书馆偷来的书吧...
                new ChatRoomInfo(marisa, 9, 3), //魔理沙：不是啦...是一颗落星！货真价实的星星欸！
                new ChatRoomInfo(reimu, 7, 4),//灵梦：...那东西不是一到夜晚满地都是么？
                new ChatRoomInfo(marisa, 10, 5), //魔理沙：但这颗星星要更闪亮一些啊，你不觉得吗？
                new ChatRoomInfo(reimu, 8, 6),//灵梦：不觉得，一定是你又闲得慌了...
                new ChatRoomInfo(marisa, 11, 7), //魔理沙：欸嘿嘿嘿...
            ];

            return list;
        }
        public override void VisualEffectForPreview()
        {
            UpdateMiscFrame();
        }
        public override void SetPetLight(ref Vector2 position, ref Vector3 rgb, ref bool inactive)
        {
            rgb = new Vector3(1.95f, 1.90f, 1.03f);
        }
        public override void AI()
        {
            Projectile.SetPetActive(Owner, BuffType<MarisaBuff>());

            ControlMovement();

            switch (CurrentState)
            {
                case States.Blink:
                    Blink();
                    break;

                case States.Happy:
                    shouldNotTalking = true;
                    Happy();
                    break;

                case States.AfterHappy:
                    shouldNotTalking = true;
                    AfterHappy();
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
            Projectile.rotation = Projectile.velocity.X * 0.012f;

            ChangeDir();

            Vector2 point = new Vector2(50 * Owner.direction, -30 + Owner.gfxOffY);
            MoveToPoint(point, 12.5f);
        }
        private void Idle()
        {
            if (OwnerIsMyPlayer)
            {
                if (mainTimer % 270 == 0)
                {
                    CurrentState = States.Blink;
                }
                if (mainTimer > 0 && mainTimer % 120 == 0 && currentChatRoom == null && ActionCD <= 0)
                {
                    if (Main.rand.NextBool(9) && !FindBoss)
                    {
                        RandomCount = Main.rand.Next(10, 20);
                        CurrentState = States.Happy;
                    }
                }
            }
        }
        private void Blink()
        {
            if (blinkFrame < 12)
            {
                blinkFrame = 12;
            }
            if (++blinkFrameCounter > 3)
            {
                blinkFrameCounter = 0;
                blinkFrame++;
            }
            if (blinkFrame > 14)
            {
                blinkFrame = 12;
                CurrentState = States.Idle;
            }
        }
        private void Happy()
        {
            if (++Projectile.frameCounter > 6)
            {
                Projectile.frameCounter = 0;
                Projectile.frame++;
            }
            if (Projectile.frame > 11)
            {
                Gore.NewGoreDirect(Projectile.GetSource_FromAI(), Projectile.Center + new Vector2(0, -27), new Vector2(Main.rand.Next(-5, 5), Main.rand.Next(-6, -3)) * 0.1f, Main.rand.Next(570, 573), Main.rand.NextFloat(0.9f, 1.1f));
                Projectile.frame = 4;
                Timer++;
            }
            if (OwnerIsMyPlayer && Timer > RandomCount || FindBoss)
            {
                Timer = 0;
                CurrentState = States.AfterHappy;
            }
        }
        private void AfterHappy()
        {
            if (++Projectile.frameCounter > 6)
            {
                Projectile.frameCounter = 0;
                Projectile.frame++;
            }
            if (Projectile.frame > 11)
            {
                Projectile.frame = 0;
                if (OwnerIsMyPlayer)
                {
                    ActionCD = 3600;
                    CurrentState = States.Idle;
                }
            }
        }
        private void IdleAnimation()
        {
            if (++Projectile.frameCounter > 6)
            {
                Projectile.frameCounter = 0;
                Projectile.frame++;
            }
            if (Projectile.frame > 3)
            {
                Projectile.frame = 0;
            }
        }
        private void UpdateMiscFrame()
        {
            if (broomFrame < 4)
            {
                broomFrame = 4;
            }
            if (++broomFrameCounter > 6)
            {
                broomFrameCounter = 0;
                broomFrame++;
            }
            if (broomFrame > 7)
            {
                broomFrame = 4;
            }

            if (++lightFrameCounter > 10)
            {
                lightFrameCounter = 0;
                lightFrame++;
            }
            if (lightFrame > 3)
            {
                lightFrame = 0;
            }
        }
    }
}


