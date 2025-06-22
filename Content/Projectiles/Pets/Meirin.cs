using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;
using Terraria;
using Terraria.ID;
using Terraria.Localization;
using Terraria.Utilities;
using TouhouPets.Content.Buffs.PetBuffs;
using static TouhouPets.Content.Projectiles.Pets.Remilia;

namespace TouhouPets.Content.Projectiles.Pets
{
    public class Meirin : BasicTouhouPet
    {
        private enum States
        {
            Idle,
            Blink,
            Serve,
            ServeBlink,
            DoingKongfu,
            BeforeSleeping,
            Sleeping,
            GetHurt,
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
        private bool IsSevring => CurrentState >= States.Serve && CurrentState <= States.ServeBlink;
        private bool IsBlinking => CurrentState == States.Blink || CurrentState == States.ServeBlink;

        private int blinkFrame, blinkFrameCounter;
        private int clothFrame, clothFrameCounter;
        private int hairFrame, hairFrameCounter;
        private int auraFrame, auraFrameCounter;
        private Vector2 clothPosOffset;
        private Vector2 hairPosOffset;

        private DrawPetConfig drawConfig = new(2);
        private readonly Texture2D clothTex = AltVanillaFunction.GetExtraTexture("Meirin_Cloth");
        public override void SetStaticDefaults()
        {
            Main.projFrames[Type] = 24;
            Main.projPet[Type] = true;
            ProjectileID.Sets.LightPet[Type] = true;
        }
        public override TouhouPetID UniqueID => TouhouPetID.Meirin;
        public override bool DrawPetSelf(ref Color lightColor)
        {
            DrawPetConfig config = drawConfig with
            {
                ShouldUseEntitySpriteDraw = true,
            };

            Projectile.DrawPet(auraFrame, Color.White, config, 1);
            Projectile.ResetDrawStateForPet();

            Projectile.DrawPet(hairFrame, lightColor,
                drawConfig with
                {
                    PositionOffset = hairPosOffset,
                }, 1);

            Projectile.DrawPet(Projectile.frame, lightColor, drawConfig);

            if (IsBlinking)
                Projectile.DrawPet(blinkFrame, lightColor, drawConfig, 1);

            Projectile.DrawPet(Projectile.frame, lightColor,
                config with
                {
                    AltTexture = clothTex,
                });
            Projectile.DrawPet(clothFrame, lightColor,
                drawConfig with
                {
                    PositionOffset = clothPosOffset,
                }, 1);

            if (Projectile.frame == 23)
            {
                DrawUmbrella(lightColor);
            }
            return false;
        }
        private void DrawUmbrella(Color lightColor)
        {
            int type = ItemID.Umbrella;
            //Main.instance.LoadItem(type);
            Texture2D tex = AltVanillaFunction.ItemTexture(type);
            Vector2 pos = Projectile.Center - Main.screenPosition + new Vector2(17 * Projectile.spriteDirection, -20) + new Vector2(0, 7f * Main.essScale);
            Color clr = Projectile.GetAlpha(lightColor);
            Vector2 orig = tex.Size() / 2;
            Main.EntitySpriteDraw(tex, pos, null, clr, Projectile.rotation, orig, Projectile.scale, SpriteEffects.None, 0);
        }
        public override ChatSettingConfig ChatSettingConfig => new ChatSettingConfig() with
        {
            TextColor = new Color(255, 81, 81),
        };
        public override void RegisterChat(ref string name, ref Vector2 indexRange)
        {
            name = "Meirin";
            indexRange = new Vector2(1, 15);
        }
        public override void SetRegularDialog(ref int timePerDialog, ref int chance, ref bool whenShouldStop)
        {
            timePerDialog = 960;//960
            chance = 5;//5
            whenShouldStop = !IsIdleState;
        }
        public override WeightedRandom<LocalizedText> RegularDialogText()
        {
            WeightedRandom<LocalizedText> chat = new ();
            {
                chat.Add(ChatDictionary[1]);
                chat.Add(ChatDictionary[2]);
                chat.Add(ChatDictionary[3]);
                chat.Add(ChatDictionary[4]);
                if (FindPet(ProjectileType<Sakuya>()))
                {
                    chat.Add(ChatDictionary[10]);
                    chat.Add(ChatDictionary[13]);
                }
            }
            return chat;
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
            TouhouPetID meirin = TouhouPetID.Meirin;
            TouhouPetID sakuya = TouhouPetID.Sakuya;

            List<ChatRoomInfo> list =
            [
                new ChatRoomInfo(meirin, ChatDictionary[10], -1), //美铃：咲夜小姐每天那么忙，有过休假的时候吗？
                new ChatRoomInfo(sakuya, ChatDictionary[6], 0),//咲夜：和大小姐在一起的每一天都是休假，你不也没有什么“假期”么？
                new ChatRoomInfo(meirin, ChatDictionary[11], 1), //美铃：这个工作和休假没啥区别啊...
                new ChatRoomInfo(sakuya, ChatDictionary[7], 2),//咲夜：什么？
                new ChatRoomInfo(meirin, ChatDictionary[12], 3), //美铃：没！没什么...
            ];

            return list;
        }
        private List<ChatRoomInfo> Chatting2()
        {
            TouhouPetID meirin = TouhouPetID.Meirin;
            TouhouPetID sakuya = TouhouPetID.Sakuya;

            List<ChatRoomInfo> list =
            [
                new ChatRoomInfo(meirin, ChatDictionary[13], -1), //美铃：咲夜小姐，我最近发现了一本讲保安和女仆谈恋爱的漫画欸！
                new ChatRoomInfo(sakuya, ChatDictionary[8], 0),//咲夜：站岗期间你看漫画？
                new ChatRoomInfo(meirin, ChatDictionary[14], 1), //美铃：啊，糟了！偷懒的事暴露了...
                new ChatRoomInfo(sakuya, ChatDictionary[9], 2),//咲夜：什么？
                new ChatRoomInfo(meirin, ChatDictionary[15], 3), //美铃：呜呜呜...欸？
            ];

            return list;
        }
        private List<ChatRoomInfo> Chatting3()
        {
            TouhouPetID meirin = TouhouPetID.Meirin;
            TouhouPetID flandre = TouhouPetID.Flandre;

            List<ChatRoomInfo> list =
            [
                new ChatRoomInfo(meirin, ChatDictionary[5], -1), //美铃：易有太极，是生两仪...
                new ChatRoomInfo(flandre, ChatDictionary[9], 0),//芙兰：美铃在说什么？
                new ChatRoomInfo(meirin, ChatDictionary[6], 1), //美铃：是我的家乡话哦，二小姐。
            ];

            return list;
        }
        public override void VisualEffectForPreview()
        {
            UpdateMiscFrame();
        }
        public override void SetPetLight(ref Vector2 position, ref Vector3 rgb, ref bool inactive)
        {
            float r = Main.DiscoR / 255f;
            float g = Main.DiscoG / 255f;
            float b = Main.DiscoB / 255f;
            float strength = 2f;
            r = (strength + r) / 1.5f;
            g = (strength + g) / 1.5f;
            b = (strength + b) / 1.5f;
            Lighting.AddLight(Projectile.Center, r, g, b);

            rgb = new Vector3(0.40f, 0.31f, 0.48f);
        }
        public override void AI()
        {
            Projectile.SetPetActive(Owner, BuffType<MeirinBuff>());
            Projectile.SetPetActive(Owner, BuffType<ScarletBuff>());

            ControlMovement(Owner);

            if (OwnerIsMyPlayer)
            {
                if (ShouldDefense(Projectile) && FindPet(ProjectileType<Flandre>(), false) && !IsSevring)
                {
                    Timer = 0;
                    CurrentState = States.Serve;
                    return;
                }
            }

            switch (CurrentState)
            {
                case States.Blink:
                    Blink();
                    break;

                case States.Serve:
                    shouldNotTalking = true;
                    Serve();
                    break;

                case States.ServeBlink:
                    shouldNotTalking = true;
                    Serve();
                    ServeBlink();
                    break;

                case States.DoingKongfu:
                    DoingKongfu();
                    break;

                case States.BeforeSleeping:
                    shouldNotTalking = true;
                    BeforeSleeping();
                    break;

                case States.Sleeping:
                    shouldNotTalking = true;
                    Sleeping();
                    break;

                case States.GetHurt:
                    shouldNotTalking = true;
                    GetHurt();
                    break;

                default:
                    Idle();
                    break;
            }

            if (IsIdleState && ActionCD > 0)
            {
                ActionCD--;
            }

            UpdateMiscData();
        }
        private void UpdateMiscData()
        {
            clothPosOffset = Projectile.frame switch
            {
                7 => new Vector2(2, 0),
                8 => new Vector2(2, 0),
                9 => new Vector2(2, -2),
                10 => new Vector2(2, 0),
                11 => new Vector2(2, 0),
                13 => new Vector2(0, 2),
                14 => new Vector2(0, 2),
                15 => new Vector2(0, 2),
                16 => new Vector2(2, 2),
                17 => new Vector2(4, 2),
                18 => new Vector2(2, 2),
                19 => new Vector2(0, 2),
                21 => new Vector2(0, -2),
                _ => Vector2.Zero,
            };
            clothPosOffset.X *= -Projectile.spriteDirection;
            hairPosOffset = Projectile.frame switch
            {
                2 => new Vector2(0, 2),
                3 => new Vector2(0, 4),
                7 => new Vector2(2, 0),
                8 => new Vector2(2, 0),
                9 => new Vector2(2, -2),
                10 => new Vector2(2, 0),
                11 => new Vector2(2, 0),
                13 => new Vector2(0, 2),
                14 => new Vector2(0, 2),
                15 => new Vector2(0, 2),
                16 => new Vector2(2, 2),
                17 => new Vector2(4, 2),
                18 => new Vector2(2, 2),
                19 => new Vector2(0, 2),
                21 => new Vector2(0, -2),
                _ => Vector2.Zero,
            };
            hairPosOffset.X *= -Projectile.spriteDirection;
        }
        private void ControlMovement(Player player)
        {
            Projectile.tileCollide = false;
            Projectile.rotation = Projectile.velocity.X * 0.003f;

            ChangeDir(120);

            Vector2 point = new Vector2((player.HasBuff<ScarletBuff>() ? -100 : 50) * player.direction, -30 + player.gfxOffY);
            Vector2 center = default;
            float speed = 15f;
            if (FindPet(out Projectile master, ProjectileType<Flandre>(), -1, 0, false))
            {
                Projectile.spriteDirection = master.spriteDirection;
                if (IsSevring)
                {
                    point = new Vector2((-25 - 60) * master.spriteDirection, player.gfxOffY - 20);
                    speed = 19f;
                }
            }
            MoveToPoint(point, speed, center);
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
                if (mainTimer > 0 && mainTimer % 480 == 0 && currentChatRoom == null && ActionCD <= 0
                     && Owner.velocity.Length() <= 5f)
                {
                    if (Main.rand.NextBool(9))
                    {
                        if (!Owner.HasBuff<ScarletBuff>())
                        {
                            RandomCount = Main.rand.Next(120, 540);
                            CurrentState = States.BeforeSleeping;
                        }
                        else
                        {
                            CurrentState = States.DoingKongfu;
                        }
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
        private void DoingKongfu()
        {
            if (Projectile.frame < 6)
            {
                Projectile.frame = 6;
            }

            var count = Projectile.frame switch
            {
                9 => 120,
                13 => 120,
                17 => 120,
                19 => 15,
                20 => 20,
                21 => 20,
                22 => 180,
                _ => 10,
            };
            if (++Projectile.frameCounter > count)
            {
                Projectile.frameCounter = 0;
                Projectile.frame++;
                if (OwnerIsMyPlayer)
                {
                    if (Main.rand.NextBool(25) && Timer == 0)
                    {
                        Projectile.SetChat(ChatDictionary[5]);
                        Timer++;
                    }
                }
            }
            if (Projectile.frame > 22)
            {
                Projectile.frame = 0;
                if (OwnerIsMyPlayer)
                {
                    ActionCD = 1200;
                    Timer = 0;
                    CurrentState = States.Idle;
                }
            }
        }
        private void Serve()
        {
            Projectile.frame = 23;
            if (OwnerIsMyPlayer && mainTimer % 270 == 0)
            {
                CurrentState = States.ServeBlink;
            }
            if (!FindPet(ProjectileType<Flandre>(), false) || !ShouldDefense(Projectile))
            {
                if (OwnerIsMyPlayer)
                {
                    CurrentState = States.Idle;
                }
            }
        }
        private void ServeBlink()
        {
            if (++blinkFrameCounter > 3)
            {
                blinkFrameCounter = 0;
                blinkFrame++;
            }
            if (blinkFrame > 2)
            {
                blinkFrame = 0;
                CurrentState = States.Serve;
            }
        }
        private void BeforeSleeping()
        {
            Projectile.velocity *= 0.5f;
            Projectile.frame = 1;

            Timer++;
            if (OwnerIsMyPlayer && Timer > RandomCount)
            {
                Timer = 0;
                CurrentState = States.Sleeping;
            }
        }
        private void Sleeping()
        {
            Projectile.velocity *= 0.2f;
            Projectile.frame = 2;
            if (OwnerIsMyPlayer && Owner.velocity.Length() > 6f)
            {
                ActionCD = 1200;
                CurrentState = States.Idle;
            }
        }
        private void GetHurt()
        {
            if (Projectile.frame < 3)
            {
                Projectile.frame = 3;
            }
            var count = Projectile.frame switch
            {
                3 => 60,
                _ => 10,
            };
            if (++Projectile.frameCounter >= count)
            {
                Projectile.frameCounter = 0;
                Projectile.frame++;
                Timer++;
            }
            if (Projectile.frame > 5)
            {
                Projectile.frame = 4;
            }
            if (OwnerIsMyPlayer && Timer > 10)
            {
                Timer = 0;
                ActionCD = 1200;
                CurrentState = States.Idle;
            }
        }
        private void UpdateMiscFrame()
        {
            if (clothFrame < 3)
            {
                clothFrame = 3;
            }
            if (++clothFrameCounter > 6)
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
            if (++hairFrameCounter > 8)
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
            if (++auraFrameCounter > 7)
            {
                auraFrameCounter = 0;
                auraFrame++;
            }
            if (auraFrame > 18)
            {
                auraFrame = 11;
            }
        }
    }
}


