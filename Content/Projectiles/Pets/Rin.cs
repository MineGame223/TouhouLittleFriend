using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;
using Terraria;
using Terraria.Graphics.Shaders;
using Terraria.ID;
using Terraria.Localization;
using Terraria.Utilities;
using TouhouPets.Content.Buffs.PetBuffs;

namespace TouhouPets.Content.Projectiles.Pets
{
    public class Rin : BasicTouhouPet
    {
        private enum States
        {
            Idle,
            Blink,
            Playing,
            AfterPlaying,
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
        private int swingFrame, swingFrameCounter;

        private DrawPetConfig drawConfig = new(3);
        private readonly Texture2D clothTex = AltVanillaFunction.GetExtraTexture("Rin_Cloth");
        public override void PetStaticDefaults()
        {
            Main.projFrames[Type] = 13;
            Main.projPet[Type] = true;

            ProjectileID.Sets.CharacterPreviewAnimations[Type] =
                ProjectileID.Sets.SimpleLoop(0, 1)
                .WhenSelected(3, 4, 4);
        }
        public override TouhouPetID UniqueID => TouhouPetID.Rin;
        public override bool DrawPetSelf(ref Color lightColor)
        {
            DrawPetConfig config = drawConfig with
            {
                ShouldUseEntitySpriteDraw = true,
                AltTexture = clothTex,
            };

            Projectile.DrawPet(swingFrame + 8, lightColor, drawConfig, 1);
            Projectile.DrawPet(swingFrame + 4, lightColor, drawConfig, 1);
            Projectile.DrawPet(Projectile.frame, lightColor, drawConfig);

            if (CurrentState == States.Blink)
                Projectile.DrawPet(blinkFrame, lightColor, drawConfig);

            Projectile.DrawPet(swingFrame, lightColor, drawConfig, 1);

            Projectile.DrawPet(swingFrame + 4, lightColor, config, 1);
            Projectile.DrawPet(Projectile.frame, lightColor, config);
            Projectile.DrawPet(swingFrame, lightColor, config, 1);
            Projectile.ResetDrawStateForPet();

            Projectile.DrawPet(Projectile.frame, Color.White * 0.8f, drawConfig, 2);
            return false;
        }
        public override ChatSettingConfig ChatSettingConfig => new ChatSettingConfig() with
        {
            TextColor = new Color(227, 59, 59),
        };
        public override void RegisterChat(ref string name, ref Vector2 indexRange)
        {
            name = "Rin";
            indexRange = new Vector2(1, 7);
        }
        public override void SetRegularDialog(ref int timePerDialog, ref int chance, ref bool whenShouldStop)
        {
            timePerDialog = 641;//641
            chance = 6;//6
            whenShouldStop = !IsIdleState;
        }
        public override WeightedRandom<LocalizedText> RegularDialogText()
        {
            WeightedRandom<LocalizedText> chat = new ();
            {
                chat.Add(ChatDictionary[1]);
                chat.Add(ChatDictionary[2]);
                chat.Add(ChatDictionary[3]);
                if (Owner.ZoneUnderworldHeight)
                {
                    chat.Add(ChatDictionary[4]);
                }
                if (FindPet(ProjectileType<Satori>()))
                {
                    chat.Add(ChatDictionary[5], 3);
                    chat.Add(ChatDictionary[6], 3);
                }
                if (FindPet(ProjectileType<Utsuho>()))
                {
                    chat.Add(ChatDictionary[7], 2);
                }
            }
            return chat;
        }
        public override void VisualEffectForPreview()
        {
            UpdateMiscFrame();
        }
        public override List<List<ChatRoomInfo>> RegisterChatRoom()
        {
            return new()
            {
                Chatting1(),
                Chatting2(),
                Chatting3(),
            };
        }
        private List<ChatRoomInfo> Chatting1()
        {
            TouhouPetID rin = TouhouPetID.Rin;
            TouhouPetID satori = TouhouPetID.Satori;

            List<ChatRoomInfo> list =
            [
                new ChatRoomInfo(rin, ChatDictionary[5], -1), //阿燐：觉大人最好了！
                new ChatRoomInfo(satori, GetChatText("Satori",5), 0),//觉：阿燐也是我最喜欢的猫咪哦。
            ];

            return list;
        }
        private List<ChatRoomInfo> Chatting2()
        {
            TouhouPetID rin = TouhouPetID.Rin;
            TouhouPetID satori = TouhouPetID.Satori;

            List<ChatRoomInfo> list =
            [
                new ChatRoomInfo(rin, ChatDictionary[6], -1), //阿燐：今天觉大人有好好吃饭吗？
                new ChatRoomInfo(satori, GetChatText("Satori",7), 0),//觉：别担心啦。
            ];

            return list;
        }
        private List<ChatRoomInfo> Chatting3()
        {
            TouhouPetID rin = TouhouPetID.Rin;
            TouhouPetID utsuho = TouhouPetID.Utsuho;

            List<ChatRoomInfo> list =
            [
                new ChatRoomInfo(rin, ChatDictionary[7], -1), //阿燐：阿空，今天也要好好干活啊！
                new ChatRoomInfo(utsuho, GetChatText("Utsuho",6), 0),//阿空：放心交给我吧！
            ];

            return list;
        }
        public override void AI()
        {
            Projectile.SetPetActive(Owner, BuffType<RinBuff>());
            Projectile.SetPetActive(Owner, BuffType<KomeijiBuff>());

            ControlMovement();

            if (ShouldExtraVFXActive)
                GenDust(Owner);

            switch (CurrentState)
            {
                case States.Blink:
                    Blink();
                    break;

                case States.Playing:
                    shouldNotTalking = true;
                    Playing();
                    break;

                case States.AfterPlaying:
                    shouldNotTalking = true;
                    AfterPlaying();
                    break;

                default:
                    Idle();
                    break;
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

            Vector2 point = new Vector2(-50 * Owner.direction, -30 + Owner.gfxOffY);
            if (Owner.HasBuff<KomeijiBuff>())
                point = new Vector2(-70 * Owner.direction, 0 + Owner.gfxOffY);
            MoveToPoint(point, 12.5f);
        }
        private void GenDust(Player player)
        {
            int dustID = MyDustId.CyanBubble;

            Dust d = Dust.NewDustPerfect(Projectile.Center + new Vector2(-28, 8), dustID
                , new Vector2(Main.rand.NextFloat(-0.5f, 0.5f), Main.rand.NextFloat(-2.5f, -1.2f)), 100, default
                , Main.rand.NextFloat(0.5f, 1.5f));
            d.noGravity = true;
            d.shader = GameShaders.Armor.GetSecondaryShader(player.cPet, player);

            d = Dust.NewDustPerfect(Projectile.Center + new Vector2(28, 8), dustID
                , new Vector2(Main.rand.NextFloat(-0.5f, 0.5f), Main.rand.NextFloat(-2.5f, -1.2f)), 100, default
                , Main.rand.NextFloat(0.5f, 1.5f));
            d.noGravity = true;
            d.shader = GameShaders.Armor.GetSecondaryShader(player.cPet, player);
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
                if (mainTimer > 0 && mainTimer % 240 == 0 && currentChatRoom == null && ActionCD <= 0)
                {
                    if (Main.rand.NextBool(6))
                    {
                        RandomCount = Main.rand.Next(20, 35);
                        CurrentState = States.Playing;
                    }
                }
            }
        }
        private void Blink()
        {
            Projectile.frame = 0;
            if (blinkFrame < 10)
            {
                blinkFrame = 10;
            }
            if (++blinkFrameCounter > 3)
            {
                blinkFrameCounter = 0;
                blinkFrame++;
            }
            if (blinkFrame > 12)
            {
                blinkFrame = 10;
                CurrentState = States.Idle;
            }
        }
        private void Playing()
        {
            if (++Projectile.frameCounter > 4)
            {
                Projectile.frameCounter = 0;
                Projectile.frame++;
            }
            if (Projectile.frame > 7)
            {
                Projectile.frame = 3;
                Timer++;
            }
            if (OwnerIsMyPlayer && Timer > RandomCount)
            {
                Timer = 0;
                CurrentState = States.AfterPlaying;
            }
        }
        private void AfterPlaying()
        {
            if (++Projectile.frameCounter > 4)
            {
                Projectile.frameCounter = 0;
                Projectile.frame++;
            }
            if (Projectile.frame > 9)
            {
                Projectile.frame = 0;
                if (OwnerIsMyPlayer)
                {
                    ActionCD = 3600;
                    CurrentState = States.Idle;
                }
            }
        }
        private void UpdateMiscFrame()
        {
            if (++swingFrameCounter > 6)
            {
                swingFrameCounter = 0;
                swingFrame++;
            }
            if (swingFrame > 3)
            {
                swingFrame = 0;
            }
        }
    }
}


