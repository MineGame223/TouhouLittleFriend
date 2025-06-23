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
    public class Sunny : BasicTouhouPet
    {
        private enum States
        {
            Idle,
            Blink,
            Reflecting,
            AfterReflecting,
            RainWet,
            RainWetBlink,
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
        private bool IsWetState => CurrentState >= States.RainWet && CurrentState <= States.RainWetBlink;
        private bool IsRainWet => Main.raining &&
            (Owner.ZoneOverworldHeight || Owner.ZoneSkyHeight);
        private bool UnderSunShine => Main.cloudAlpha <= 0 && Main.dayTime &&
            (Owner.ZoneOverworldHeight || Owner.ZoneSkyHeight);
        public override void SetStaticDefaults()
        {
            Main.projFrames[Type] = 13;
            Main.projPet[Type] = true;
            ProjectileID.Sets.LightPet[Type] = true;
        }
        public override TouhouPetID UniqueID => TouhouPetID.Sunny;

        private int blinkFrame, blinkFrameCounter;
        private int wingsFrame, wingsFrameCounter;
        private int clothFrame, clothFrameCounter;
        private int hairFrame, hairFrameCounter;
        private float extraX, extraY, phantomTime;

        private DrawPetConfig drawConfig = new(2);
        private readonly Texture2D clothTex = AltVanillaFunction.GetExtraTexture("Sunny_Cloth");
        public override bool DrawPetSelf(ref Color lightColor)
        {
            DrawSunny(lightColor);

            if (phantomTime >= 0)
            {
                for (int i = -1; i <= 1; i++)
                {
                    if (i != 0)
                    {
                        Vector2 dist = Owner.Center - Projectile.Center;
                        Vector2 drift = new Vector2(dist.X * i * 2, dist.Y * 2).RotatedBy(Main.GlobalTimeWrappedHourly);
                        Color clr = lightColor * 0.4f * phantomTime;
                        DrawSunny(clr, drift);
                        DrawSunny(clr, -drift);
                    }
                }
            }

            return false;
        }
        private void DrawSunny(Color lightColor, Vector2? posOffset = default)
        {
            Vector2 extraPos = new Vector2(extraX, extraY);
            Vector2 offset = posOffset ?? Vector2.Zero;

            DrawPetConfig config = drawConfig with
            {
                PositionOffset = extraPos + offset,
            };
            DrawPetConfig config2 = drawConfig with
            {
                ShouldUseEntitySpriteDraw = true,
            };

            Projectile.DrawPet(wingsFrame, lightColor * 0.8f, config, 1);

            Projectile.DrawPet(hairFrame, lightColor, config, 1);

            Projectile.DrawPet(Projectile.frame, lightColor,
                drawConfig with
                {
                    PositionOffset = offset,
                });

            if (CurrentState == States.Blink || CurrentState == States.RainWetBlink)
                Projectile.DrawPet(blinkFrame, lightColor, config);

            Projectile.DrawPet(Projectile.frame, lightColor,
                config2 with
                {
                    PositionOffset = offset,
                    AltTexture = clothTex,
                });
            Projectile.DrawPet(clothFrame, lightColor,
                config with
                {
                    ShouldUseEntitySpriteDraw = true,
                }, 1);
            Projectile.ResetDrawStateForPet();

            if (Projectile.frame == 4)
            {
                Projectile.DrawPet(5, lightColor,
                drawConfig with
                {
                    PositionOffset = offset,
                });
                Projectile.DrawPet(5, lightColor,
                config2 with
                {
                    PositionOffset = offset,
                    AltTexture = clothTex,
                });
            }
        }
        public override ChatSettingConfig ChatSettingConfig => new ChatSettingConfig() with
        {
            TextColor = new Color(240, 196, 48),
        };
        public override void RegisterChat(ref string name, ref Vector2 indexRange)
        {
            name = "Sunny";
            indexRange = new Vector2(1, 15);
        }
        public override void SetRegularDialog(ref int timePerDialog, ref int chance, ref bool whenShouldStop)
        {
            timePerDialog = 640;//640
            chance = 6;//6
            whenShouldStop = !IsIdleState;
        }
        public override WeightedRandom<LocalizedText> RegularDialogText()
        {
            WeightedRandom<LocalizedText> chat = new();
            {
                if (IsRainWet)
                {
                    chat.Add(ChatDictionary[6]);
                    chat.Add(ChatDictionary[7]);
                    chat.Add(ChatDictionary[8]);
                }
                else
                {
                    chat.Add(ChatDictionary[1]);
                    chat.Add(ChatDictionary[2]);
                    chat.Add(ChatDictionary[3]);
                    if (UnderSunShine)
                    {
                        chat.Add(ChatDictionary[4]);
                        chat.Add(ChatDictionary[5]);

                        if (chatCD <= 0)
                            chat.Add(ChatDictionary[9]);
                    }
                    if (!Owner.HasBuff<ReimuBuff>())
                    chat.Add(ChatDictionary[12], 3);
                }
            }
            return chat;
        }
        public override List<List<ChatRoomInfo>> RegisterChatRoom()
        {
            return new()
            {
                Chatting1(),
                Chatting2()
            };
        }
        private List<ChatRoomInfo> Chatting1()
        {
            TouhouPetID sunny = TouhouPetID.Sunny;

            List<ChatRoomInfo> list =
            [
                new ChatRoomInfo(sunny,ChatDictionary[9], -1),//桑尼：日光妖精~ 洁白身体~
                new ChatRoomInfo(sunny,ChatDictionary[10], 0),//桑尼：日光妖精~ 碧蓝双眸~
                new ChatRoomInfo(sunny,ChatDictionary[11], 1),//桑尼：日光妖精—— 桑尼！米尔克！
            ];

            return list;
        }
        private List<ChatRoomInfo> Chatting2()
        {
            TouhouPetID sunny = TouhouPetID.Sunny;
            TouhouPetID luna = TouhouPetID.Luna;
            TouhouPetID star = TouhouPetID.Star;

            List<ChatRoomInfo> list =
            [
                new ChatRoomInfo(sunny, ChatDictionary[12], -1),//桑尼：下一次该去哪里恶作剧呢？
                new ChatRoomInfo(star, GetChatText("Star",6), 0),//斯塔：要不去偷那个黑白魔法使的蘑菇吧？
                new ChatRoomInfo(luna, GetChatText("Luna",9), 1),//露娜：且不说被发现了会怎么样...咱们去小偷家里偷东西？
                new ChatRoomInfo(sunny,ChatDictionary[13],2),//桑尼：没事的啦！露娜你只管殿后就好啦。
                new ChatRoomInfo(luna, GetChatText("Luna",10), 3),//露娜：每次都是我收拾残局欸？！这次要去你们俩去吧，人家才不去呢！
                new ChatRoomInfo(sunny, ChatDictionary[14], 4),//桑尼：呜哇！偷东西的时候你的能力超重要的好吗？
                new ChatRoomInfo(star, GetChatText("Star",7), 5),//斯塔：好啦好啦，那要不咱们去偷那个红白巫女的赛钱箱吧？
                new ChatRoomInfo(sunny,ChatDictionary[15], 6),//桑尼 & 露娜：不可以！！！
                new ChatRoomInfo(luna,GetChatText("Luna",11), 6),
            ];

            return list;
        }
        public override void VisualEffectForPreview()
        {
            UpdateMiscFrame();
        }
        public override void SetPetLight(ref Vector2 position, ref Vector3 rgb, ref bool inactive)
        {
            float brightness = IsRainWet ? 0.5f : 1f;
            rgb = new Vector3(2.40f, 1.96f, 0.84f) * brightness;
        }
        public override void AI()
        {
            Projectile.SetPetActive(Owner, BuffType<SunnyBuff>());
            Projectile.SetPetActive(Owner, BuffType<TheThreeFairiesBuff>());

            ControlMovement(Owner);

            if (ShouldExtraVFXActive)
                GenDust();

            switch (CurrentState)
            {
                case States.Blink:
                    Blink();
                    break;

                case States.Reflecting:
                    shouldNotTalking = true;
                    Reflecting();
                    break;

                case States.AfterReflecting:
                    shouldNotTalking = true;
                    AfterReflecting();
                    break;

                case States.RainWet:
                    shouldNotTalking = true;
                    RainWet();
                    break;

                case States.RainWetBlink:
                    shouldNotTalking = true;
                    RainWetBlink();
                    break;

                default:
                    Idle();
                    break;
            }
            if (IsIdleState && ActionCD > 0)
            {
                ActionCD--;
            }

            UpdateExtraPos();
        }
        private void GenDust()
        {
            if (IsRainWet)
            {
                if (Main.rand.NextBool(6) && !Owner.behindBackWall)
                {
                    Dust.NewDustPerfect(Projectile.position + new Vector2(Main.rand.Next(10, Projectile.width - 10), Main.rand.Next(10, Projectile.height - 10)),
                            MyDustId.BlueThin, new Vector2(0, 0.1f), 100, Color.White).scale = Main.rand.NextFloat(0.5f, 1.2f);
                }
                return;
            }

            int dustID = MyDustId.YellowGoldenFire;
            if (Main.rand.NextBool(10))
            {
                Dust d = Dust.NewDustPerfect(Projectile.Center
                + new Vector2(Main.rand.NextFloat(-30f, -10f) * Projectile.spriteDirection, Main.rand.NextFloat(-5f, 25f)), dustID
                , new Vector2(Main.rand.NextFloat(-0.2f, 0.2f), Main.rand.NextFloat(0.8f, 1.2f)), 100, default
                , Main.rand.NextFloat(1f, 2f));
                d.noGravity = true;
                //d.shader = GameShaders.Armor.GetSecondaryShader(Owner.cLight, Owner);
            }
            if (Main.rand.NextBool(10))
            {
                Dust d = Dust.NewDustPerfect(Projectile.Center
                    + new Vector2(Main.rand.NextFloat(20f, 22f) * Projectile.spriteDirection, Main.rand.NextFloat(-5f, 25f)), dustID
                    , new Vector2(Main.rand.NextFloat(-0.2f, 0.2f), Main.rand.NextFloat(0.8f, 1.2f)), 100, default
                    , Main.rand.NextFloat(1f, 2f));
                d.noGravity = true;
                //d.shader = GameShaders.Armor.GetSecondaryShader(Owner.cLight, Owner);
            }
        }
        private void ControlMovement(Player player)
        {
            Projectile.tileCollide = false;
            Projectile.rotation = Projectile.velocity.X * 0.02f;

            ChangeDir(200);

            Vector2 point = new Vector2(60 * player.direction, -40 + player.gfxOffY);
            if (player.HasBuff<TheThreeFairiesBuff>())
            {
                point = new Vector2(60 * player.direction, -70 + player.gfxOffY);
                point += new Vector2(0, -40).RotatedBy(MathHelper.ToRadians(360 / 3 * 0) + Main.GlobalTimeWrappedHourly);
            }
            MoveToPoint(point, 8.5f);
        }
        private void UpdateExtraPos()
        {
            extraX = 0;
            extraY = 0;
            if (Projectile.frame >= 1 && Projectile.frame <= 3)
            {
                extraY = -2;
            }
            if (Projectile.frame == 4)
            {
                extraY = 2;
            }
            if (CurrentState == States.Reflecting)
            {
                phantomTime += 0.1f;
            }
            else
            {
                phantomTime -= 0.1f;
            }
            phantomTime = MathHelper.Clamp(phantomTime, 0, 1);
        }
        private void Idle()
        {
            Projectile.frame = 0;
            if (OwnerIsMyPlayer)
            {
                if (IsRainWet)
                {
                    CurrentState = States.RainWet;
                    return;
                }
                if (mainTimer % 270 == 0)
                {
                    CurrentState = States.Blink;
                }
                if (mainTimer > 0 && mainTimer % 650 == 0 && UnderSunShine
                    && currentChatRoom == null && ActionCD <= 0)
                {
                    if (Main.rand.NextBool(6))
                    {
                        RandomCount = Main.rand.Next(6, 12);
                        CurrentState = States.Reflecting;
                    }
                }
            }
        }
        private void Blink()
        {
            Projectile.frame = 0;
            int startFrame = 10;
            if (blinkFrame < startFrame)
            {
                blinkFrame = startFrame;
            }
            if (++blinkFrameCounter > 3)
            {
                blinkFrameCounter = 0;
                blinkFrame++;
            }
            if (blinkFrame > 12)
            {
                blinkFrame = startFrame;
                CurrentState = States.Idle;
            }
        }
        private void Reflecting()
        {
            if (IsRainWet)
            {
                Timer = 0;
                CurrentState = States.RainWet;
                return;
            }

            if (++Projectile.frameCounter > 8)
            {
                Projectile.frameCounter = 0;
                Projectile.frame++;
            }
            if (Projectile.frame > 3)
            {
                Projectile.frame = 2;
                Timer++;
            }
            if (OwnerIsMyPlayer && Timer > RandomCount)
            {
                Timer = 0;
                CurrentState = States.AfterReflecting;
            }
        }
        private void AfterReflecting()
        {
            if (++Projectile.frameCounter > 8)
            {
                Projectile.frameCounter = 0;
                Projectile.frame++;
            }
            if (Projectile.frame > 3)
            {
                Projectile.frame = 0;
                if (OwnerIsMyPlayer)
                {
                    ActionCD = 3600;
                    CurrentState = States.Idle;
                }
            }
        }
        private void RainWet()
        {
            Projectile.frame = 4;
            if (OwnerIsMyPlayer)
            {
                if (!IsRainWet)
                {
                    CurrentState = States.Idle;
                    return;
                }
                if (mainTimer % 270 == 0)
                {
                    CurrentState = States.RainWetBlink;
                }
            }
        }
        private void RainWetBlink()
        {
            Projectile.frame = 4;
            int startFrame = 11;
            if (blinkFrame < startFrame)
            {
                blinkFrame = startFrame;
            }
            if (++blinkFrameCounter > 3)
            {
                blinkFrameCounter = 0;
                blinkFrame++;
            }
            if (blinkFrame > 12)
            {
                blinkFrame = startFrame;
                CurrentState = States.RainWet;
            }
        }
        private void UpdateMiscFrame()
        {
            if (wingsFrame < 9)
            {
                wingsFrame = 9;
            }
            if (++wingsFrameCounter > 3)
            {
                wingsFrameCounter = 0;
                wingsFrame++;
            }
            if (wingsFrame > 12)
            {
                wingsFrame = 9;
            }

            if (IsWetState)
            {
                hairFrame = 8;
                clothFrame = 0;
            }
            else
            {
                if (hairFrame < 4)
                {
                    hairFrame = 4;
                }
                if (++hairFrameCounter > 7)
                {
                    hairFrameCounter = 0;
                    hairFrame++;
                }
                if (hairFrame > 7)
                {
                    hairFrame = 4;
                }
                if (++clothFrameCounter > 6)
                {
                    clothFrameCounter = 0;
                    clothFrame++;
                }
                if (clothFrame > 3)
                {
                    clothFrame = 0;
                }
            }
        }
    }
}


