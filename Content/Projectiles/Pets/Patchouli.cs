using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;
using Terraria;
using Terraria.ID;
using Terraria.Utilities;
using TouhouPets.Content.Buffs.PetBuffs;

namespace TouhouPets.Content.Projectiles.Pets
{
    public class Patchouli : BasicTouhouPet
    {
        private enum States
        {
            Idle,
            Blink,
            Reading,
            TurnPage,
            ReadingBlink,
            AfterReading,
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
        private bool IsBlinking => CurrentState == States.Blink || CurrentState == States.ReadingBlink;
        private bool IsReading => CurrentState >= States.Reading && CurrentState <= States.TurnPage;

        private int blinkFrame, blinkFrameCounter;
        private int clothFrame, clothFrameCounter;
        private int auraFrame, auraFrameCounter;

        private DrawPetConfig drawConfig = new(2);
        private readonly Texture2D clothTex = AltVanillaFunction.GetExtraTexture("Patchouli_Cloth");
        public override void SetStaticDefaults()
        {
            Main.projFrames[Type] = 16;
            Main.projPet[Type] = true;
            ProjectileID.Sets.LightPet[Type] = true;
        }
        public override TouhouPetID UniqueID => TouhouPetID.Patchouli;
        public override bool DrawPetSelf(ref Color lightColor)
        {
            DrawPetConfig config = drawConfig with
            {
                AltTexture = clothTex,
            };
            DrawAura();

            Projectile.DrawPet(clothFrame, lightColor,
                drawConfig with
                {
                    ShouldUseEntitySpriteDraw = true,
                });
            Projectile.ResetDrawStateForPet();

            Projectile.DrawPet(clothFrame, lightColor, drawConfig, 1);
            Projectile.DrawPet(clothFrame, lightColor, config);

            Projectile.DrawPet(Projectile.frame, lightColor, drawConfig);

            if (IsBlinking)
                Projectile.DrawPet(blinkFrame, lightColor, drawConfig);

            Projectile.DrawPet(Projectile.frame, lightColor,
                config with
                {
                    ShouldUseEntitySpriteDraw = true,
                });
            return false;
        }
        private void DrawAura()
        {
            DrawPetConfig config = drawConfig with
            {
                ShouldUseEntitySpriteDraw = true,
            };
            for (int i = 0; i < 3; i++)
            {
                Vector2 spinningpoint = new Vector2(0f, -1.4f);
                Color clr = (Color.White * 0.3f).ModifiedAlphaColor();
                Projectile.DrawPet(auraFrame, clr,
                    config with
                    {
                        PositionOffset = spinningpoint.RotatedBy(MathHelper.TwoPi * Main.GlobalTimeWrappedHourly
                        + MathHelper.TwoPi / 3 * i)
                    }
                    , 1);
            }
            Projectile.DrawPet(auraFrame, Color.White * 0.7f, config, 1);
        }
        public override ChatSettingConfig ChatSettingConfig => new ChatSettingConfig() with
        {
            TextColor = new Color(252, 197, 238),
        };
        public override void RegisterChat(ref string name, ref Vector2 indexRange)
        {
            name = "Patchouli";
            indexRange = new Vector2(1, 35);
        }
        public override void SetRegularDialog(ref int timePerDialog, ref int chance, ref bool whenShouldStop)
        {
            timePerDialog = 720;//720
            chance = 12;//12
            whenShouldStop = false;
        }
        public override WeightedRandom<string> RegularDialogText()
        {
            WeightedRandom<string> chat = new WeightedRandom<string>();
            {
                if (Projectile.velocity.Length() >= 4f)
                {
                    chat.Add(ChatDictionary[1]);
                }
                if (IsReading)
                {
                    chat.Add(ChatDictionary[2]);
                    chat.Add(ChatDictionary[3]);
                    chat.Add(ChatDictionary[4]);
                    chat.Add(ChatDictionary[5]);
                    if (FindPet(ProjectileType<Remilia>()) && !Main.dayTime)
                    {
                        chat.Add(ChatDictionary[8]);
                    }
                }
                else
                {
                    chat.Add(ChatDictionary[7]);
                }
                chat.Add(ChatDictionary[6]);
                if (FindPet(ProjectileType<Alice>()))
                {
                    chat.Add(ChatDictionary[12]);
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
            };
        }
        private static List<ChatRoomInfo> Chatting1()
        {
            TouhouPetID patchi = TouhouPetID.Patchouli;
            TouhouPetID remilia = TouhouPetID.Remilia;

            List<ChatRoomInfo> list =
            [
                new ChatRoomInfo(patchi, 8, -1), //帕秋莉：唔...蕾咪？
                new ChatRoomInfo(remilia, 10, 0),//蕾米：嗯？帕琪？有啥事么？
                new ChatRoomInfo(patchi, 9, 1), //帕秋莉：你身为吸血鬼，为什么不像书里说的一样怕十字架？
                new ChatRoomInfo(remilia, 11, 2),//蕾米：哈哈，那都是瞎扯，吸血鬼怕十字架不过是人类打不过吸血鬼而臆想出来的心理安慰。
                new ChatRoomInfo(patchi, 10, 3), //帕秋莉：好吧...看来书里说的不全是正确的。
                new ChatRoomInfo(remilia, 12, 4),//蕾米：当然了，帕琪你也要多出来走走嘛。
                new ChatRoomInfo(patchi, 11, 5), //帕秋莉：不要...
            ];

            return list;
        }
        private static List<ChatRoomInfo> Chatting2()
        {
            TouhouPetID patchi = TouhouPetID.Patchouli;
            TouhouPetID alice = TouhouPetID.Alice;

            List<ChatRoomInfo> list =
            [
                new ChatRoomInfo(patchi, 12, -1), //帕秋莉：最近魔理沙那家伙还安分么？
                new ChatRoomInfo(alice, 8, 0),//爱丽丝：别说了，上次刚顺走我一瓶魔药。
                new ChatRoomInfo(patchi, 13, 1), //帕秋莉：她偷走的那好几本书也一直没还...
                new ChatRoomInfo(alice, 9, 2),//帕秋莉 & 爱丽丝：...一定要找她算账！
                new ChatRoomInfo(patchi, 14, 2),
                new ChatRoomInfo(alice, 10, 3),//爱丽丝：...？还是我去找她吧，就不麻烦你了...
                new ChatRoomInfo(patchi, 15, 4), //帕秋莉：不不不，我去就行，我去就行...
            ];

            return list;
        }
        public override void VisualEffectForPreview()
        {
            UpdateClothFrame();
            UpdateAuraFrame();
        }
        public override void SetPetLight(ref Vector2 position, ref Vector3 rgb, ref bool inactive)
        {
            rgb = new Vector3(2.52f, 1.97f, 2.38f);
        }
        public override void AI()
        {
            Projectile.SetPetActive(Owner, BuffType<PatchouliBuff>());
            Projectile.SetPetActive(Owner, BuffType<ScarletBuff>());

            ControlMovement(Owner);

            if (IsReading && Projectile.velocity.Length() > 4.5f)
            {
                Timer = 0;
                CurrentState = States.AfterReading;
                return;
            }

            switch (CurrentState)
            {
                case States.Blink:
                    Blink();
                    break;

                case States.Reading:
                    Reading();
                    break;

                case States.ReadingBlink:
                    Reading();
                    ReadingBlink();
                    break;

                case States.TurnPage:
                    TurnPage();
                    break;

                case States.AfterReading:
                    AfterReading();
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
        private void ControlMovement(Player player)
        {
            Projectile.tileCollide = false;
            Projectile.rotation = Projectile.velocity.X * 0.012f;

            ChangeDir();

            Vector2 point = new Vector2(50 * player.direction, -20 + player.gfxOffY);
            if (player.HasBuff<ScarletBuff>())
            {
                point = new Vector2(0, -120 + player.gfxOffY);
            }

            MoveToPoint(point, 4.5f);
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
                if (mainTimer > 0 && mainTimer % 180 == 0 && currentChatRoom == null && ActionCD <= 0
                     && Projectile.velocity.Length() < 2f)
                {
                    if (Main.rand.NextBool(4))
                    {
                        RandomCount = Main.rand.Next(360, 540);
                        CurrentState = States.Reading;
                    }
                }
            }
        }
        private void Reading()
        {
            if (mainTimer % 270 == 0)
            {
                CurrentState = States.ReadingBlink;
            }
            if (++Projectile.frameCounter > 10)
            {
                Projectile.frameCounter = 0;
                Projectile.frame++;
            }
            if (Projectile.frame >= 3)
            {
                Projectile.frame = 3;
                if (OwnerIsMyPlayer)
                {
                    if (Timer < RandomCount)
                    {
                        Timer++;
                    }
                    else
                    {
                        Timer = 0;
                        CurrentState = States.TurnPage;
                    }
                }
            }
        }
        private void TurnPage()
        {
            if (++Projectile.frameCounter > 10)
            {
                Projectile.frameCounter = 0;
                Projectile.frame++;
            }
            if (Projectile.frame > 6)
            {
                Projectile.frame = 3;
                if (OwnerIsMyPlayer)
                {
                    RandomCount = Main.rand.Next(360, 540);
                    Timer = 0;
                    CurrentState = States.Reading;
                }
            }
        }
        private void AfterReading()
        {
            if (++Projectile.frameCounter > 10)
            {
                Projectile.frameCounter = 0;
                Projectile.frame++;
            }
            if (Projectile.frame < 7)
            {
                Projectile.frame = 7;
            }
            if (Projectile.frame > 8)
            {
                Projectile.frame = 0;
                if (OwnerIsMyPlayer)
                {
                    ActionCD = 480;
                    CurrentState = States.Idle;
                }
            }
        }
        private void Blink()
        {
            Projectile.frame = 0;
            if (++blinkFrameCounter > 6)
            {
                blinkFrameCounter = 0;
                blinkFrame++;
            }
            if (blinkFrame < 9)
            {
                blinkFrame = 9;
            }
            if (blinkFrame > 11)
            {
                blinkFrame = 9;
                CurrentState = States.Idle;
            }
        }
        private void ReadingBlink()
        {
            if (++blinkFrameCounter > 6)
            {
                blinkFrameCounter = 0;
                blinkFrame++;
            }
            if (blinkFrame < 10)
            {
                blinkFrame = 10;
            }
            if (blinkFrame > 11)
            {
                blinkFrame = 10;
                CurrentState = States.Reading;
            }
        }
        private void UpdateClothFrame()
        {
            if (clothFrame < 12)
            {
                clothFrame = 12;
            }
            if (++clothFrameCounter > 10)
            {
                clothFrameCounter = 0;
                clothFrame++;
            }
            if (clothFrame > 15)
            {
                clothFrame = 12;
            }
        }
        private void UpdateAuraFrame()
        {
            if (++auraFrameCounter > 3)
            {
                auraFrameCounter = 0;
                auraFrame++;
            }
            if (auraFrame > 4)
            {
                auraFrame = 0;
            }
        }
    }
}


