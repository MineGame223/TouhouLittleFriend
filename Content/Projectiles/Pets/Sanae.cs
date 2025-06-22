using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using Terraria;
using Terraria.Graphics.Shaders;
using Terraria.ID;
using Terraria.Utilities;
using TouhouPets.Content.Buffs.PetBuffs;

namespace TouhouPets.Content.Projectiles.Pets
{
    public class Sanae : BasicTouhouPet
    {
        private enum States
        {
            Idle,
            Blink,
            Pray,
            AfterPray,
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
        private bool IsPraying => CurrentState == States.Pray;
        private bool IsIdleState => CurrentState <= States.Blink;

        private int blinkFrame, blinkFrameCounter;
        private int clothFrame, clothFrameCounter;
        private int itemFrame, itemFrameCounter;
        private int hairFrame, hairFrameCounter;
        private float auraScale;
        private int extraAdjY;
        private int flyTimeleft = 0;

        private DrawPetConfig drawConfig = new(2);
        private readonly Texture2D clothTex = AltVanillaFunction.GetExtraTexture("Sanae_Cloth");
        public override void SetStaticDefaults()
        {
            Main.projFrames[Type] = 15;
            Main.projPet[Type] = true;
            ProjectileID.Sets.LightPet[Type] = true;
        }
        public override TouhouPetID UniqueID => TouhouPetID.Sanae;
        public override bool DrawPetSelf(ref Color lightColor)
        {
            if (auraScale > 0)
            {
                float time = Main.GlobalTimeWrappedHourly * 6f;
                Color clr = Color.SeaGreen * mouseOpacity;
                clr.A *= 0;
                for (int o = 0; o < 8; o++)
                {
                    for (int i = -1; i <= 1; i++)
                    {
                        Vector2 auraPos = new Vector2(2.5f * auraScale * (float)Math.Sin(time), 0);
                        DrawSanaeAura(clr * 0.3f, auraPos.RotatedBy(MathHelper.Pi * i));

                        auraPos = new Vector2(0, 2.5f * auraScale * (float)Math.Sin(time));
                        DrawSanaeAura(clr * 0.3f, auraPos.RotatedBy(MathHelper.Pi * i));
                    }
                }
            }
            Projectile.ResetDrawStateForPet();

            DrawSanae(lightColor);
            return false;
        }
        private void DrawSanae(Color lightColor)
        {
            DrawPetConfig config = drawConfig with
            {
                ShouldUseEntitySpriteDraw = true,
            };

            if (CurrentState < States.Flying)
                Projectile.DrawPet(hairFrame, lightColor,
                    drawConfig with
                    {
                        PositionOffset = new Vector2(0, extraAdjY),
                    }, 1);

            Projectile.DrawPet(Projectile.frame, lightColor, drawConfig);

            if (CurrentState == States.Blink || CurrentState == States.FlyingBlink)
                Projectile.DrawPet(blinkFrame, lightColor, drawConfig);

            Projectile.DrawPet(Projectile.frame, lightColor,
                config with
                {
                    AltTexture = clothTex,
                });
            Projectile.ResetDrawStateForPet();

            if (Projectile.frame < 5)
            {
                Projectile.DrawPet(itemFrame, lightColor, drawConfig, 1);
                Projectile.DrawPet(clothFrame, lightColor,
                    config with
                    {
                        PositionOffset = new Vector2(0, extraAdjY),
                    });
            }
        }
        private void DrawSanaeAura(Color lightColor, Vector2? posOffset = default)
        {
            Vector2 offset = posOffset ?? Vector2.Zero;
            DrawPetConfig config = drawConfig with
            {
                ShouldUseEntitySpriteDraw = false,
            };
            DrawPetConfig config2 = config with
            {
                PositionOffset = new Vector2(0, extraAdjY) + offset,
            };
            Projectile.DrawPet(hairFrame, lightColor, config2, 1);
            Projectile.DrawPet(Projectile.frame, lightColor,
                config with
                {
                    PositionOffset = offset,
                });
            Projectile.DrawPet(clothFrame, lightColor, config2);
        }
        public override ChatSettingConfig ChatSettingConfig => new ChatSettingConfig() with
        {
            TextColor = new Color(83, 241, 146),
        };
        public override void RegisterChat(ref string name, ref Vector2 indexRange)
        {
            name = "Sanae";
            indexRange = new Vector2(1, 5);
        }
        public override void SetRegularDialog(ref int timePerDialog, ref int chance, ref bool whenShouldStop)
        {
            timePerDialog = 960;//960
            chance = 7;//7
            whenShouldStop = !IsIdleState;
        }
        public override WeightedRandom<string> RegularDialogText()
        {
            WeightedRandom<string> chat = new WeightedRandom<string>();
            {
                chat.Add(ChatDictionary[1]);
                chat.Add(ChatDictionary[2]);
                chat.Add(ChatDictionary[3]);
                chat.Add(ChatDictionary[4]);
                if (Main.IsItAHappyWindyDay)
                {
                    chat.Add(ChatDictionary[5]);
                }
            }
            return chat;
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
            TouhouPetID sanae = TouhouPetID.Sanae;
            TouhouPetID reimu = TouhouPetID.Reimu;

            List<ChatRoomInfo> list =
            [
                new ChatRoomInfo(sanae, 4, -1), //早苗：加入守矢神社，信仰伟大的乾神和坤神吧！
                new ChatRoomInfo(reimu, 9, 0),//灵梦：给我适可而止啊喂！
            ];

            return list;
        }
        public override void VisualEffectForPreview()
        {
            UpdateClothFrame();
            UpdateHairFrame();
            UpdateItemFrame();
        }
        public override void SetPetLight(ref Vector2 position, ref Vector3 rgb, ref bool inactive)
        {
            float lightPlus = 1 + auraScale * Main.essScale;
            rgb = new Vector3(0.55f, 2.14f, 1.53f) * lightPlus;
        }
        public override void AI()
        {
            Projectile.SetPetActive(Owner, BuffType<SanaeBuff>());

            ControlMovement();

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

                case States.Pray:
                    shouldNotTalking = true;
                    Pray();
                    break;

                case States.AfterPray:
                    shouldNotTalking = true;
                    AfterPray();
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

            UpdateMiscData();
        }
        private void UpdateMiscData()
        {
            if (flyTimeleft > 0)
            {
                flyTimeleft--;
            }
            if (CurrentState == States.Pray)
            {
                if (ShouldExtraVFXActive)
                    PrayEffect();

                auraScale = MathHelper.Clamp(auraScale += 0.02f, 0, 2);
            }
            else
            {
                auraScale = MathHelper.Clamp(auraScale -= 0.01f, 0, 1);
            }
            extraAdjY = 0;
            if (Projectile.frame >= 2 && Projectile.frame <= 3)
            {
                extraAdjY = -2;
            }
        }
        private void PrayEffect()
        {
            var dustType = Main.rand.Next(4) switch
            {
                1 => MyDustId.TrailingGreen1,
                _ => MyDustId.GreenTrans,
            };
            if (Main.rand.NextBool(3))
            {
                Dust d = Dust.NewDustDirect(Projectile.position, Projectile.width, Projectile.height, dustType,
                    0, Main.rand.Next(-3, -1), 100, default, auraScale * 0.45f);
                d.noGravity = true;
                //d.shader = GameShaders.Armor.GetSecondaryShader(Owner.cLight, Owner);
            }
        }
        private void ControlMovement()
        {
            Projectile.tileCollide = false;
            Projectile.rotation = Projectile.velocity.X * 0.012f;

            if (CurrentState != States.Pray)
            {
                ChangeDir();
            }

            Vector2 point = new Vector2(50 * Owner.direction, -30 + Owner.gfxOffY);
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
                if (mainTimer > 0 && mainTimer % 480 == 0 && currentChatRoom == null && ActionCD <= 0)
                {
                    if (Main.rand.NextBool(5))
                    {
                        RandomCount = Main.rand.Next(120, 240);
                        CurrentState = States.Pray;
                    }
                }
            }
        }
        private void Blink()
        {
            Projectile.frame = 0;
            if (blinkFrame < 7)
            {
                blinkFrame = 7;
            }
            if (++blinkFrameCounter > 3)
            {
                blinkFrameCounter = 0;
                blinkFrame++;
            }
            if (blinkFrame > 9)
            {
                blinkFrame = 7;
                CurrentState = States.Idle;
            }
        }
        private void Pray()
        {
            if (Projectile.frame < 1)
            {
                Projectile.frame = 1;
            }
            int count = (Projectile.frame >= 2 && Projectile.frame <= 3) ? 3 : 6;
            if (++Projectile.frameCounter > count)
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
                CurrentState = States.AfterPray;
            }
        }
        private void AfterPray()
        {
            if (++Projectile.frameCounter > 6)
            {
                Projectile.frameCounter = 0;
                Projectile.frame++;
            }
            if (Projectile.frame > 4)
            {
                Projectile.frame = 0;
                if (OwnerIsMyPlayer)
                {
                    ActionCD = 4800;
                    CurrentState = States.Idle;
                }
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
            if (Projectile.frame < 5)
            {
                Projectile.frame = 5;
            }
            if (++Projectile.frameCounter > 3)
            {
                Projectile.frameCounter = 0;
                Projectile.frame++;
            }
            if (Projectile.frame > 6)
            {
                Projectile.frame = 5;
            }
        }
        private void FlyingBlink()
        {
            if (blinkFrame < 7)
            {
                blinkFrame = 7;
            }
            if (++blinkFrameCounter > 3)
            {
                blinkFrameCounter = 0;
                blinkFrame++;
            }
            if (blinkFrame > 9)
            {
                blinkFrame = 7;
                CurrentState = States.Flying;
            }
        }
        private void UpdateClothFrame()
        {
            if (clothFrame < 10)
            {
                clothFrame = 10;
            }
            int count = IsPraying ? 3 : 5;
            if (++clothFrameCounter > count)
            {
                clothFrameCounter = 0;
                clothFrame++;
            }
            if (clothFrame > 13)
            {
                clothFrame = 10;
            }
        }
        private void UpdateHairFrame()
        {
            if (hairFrame < 8)
            {
                hairFrame = 8;
            }
            int count = IsPraying ? 3 : 6;
            if (++hairFrameCounter > count)
            {
                hairFrameCounter = 0;
                hairFrame++;
            }
            if (hairFrame > 11)
            {
                hairFrame = 8;
            }
        }
        private void UpdateItemFrame()
        {
            int count = IsPraying ? 3 : 7;
            if (++itemFrameCounter > count)
            {
                itemFrameCounter = 0;
                itemFrame++;
            }
            if (IsPraying)
            {
                if (Projectile.frame > 0)
                {
                    itemFrame = Projectile.frame + 3;
                }
                return;
            }
            if (itemFrame > 3)
            {
                itemFrame = 0;
            }
        }
    }
}


