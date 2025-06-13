using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;
using Terraria;
using Terraria.ID;
using Terraria.Utilities;
using TouhouPets.Content.Buffs.PetBuffs;
using static TouhouPets.Content.Projectiles.Pets.Remilia;

namespace TouhouPets.Content.Projectiles.Pets
{
    public class Flandre : BasicTouhouPet
    {
        private enum States
        {
            Idle,
            Blink,
            Defense,
            BeforeEating,
            Eating,
            AfterEating,
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

        private int wingFrame, wingFrameCounter;
        private int blinkFrame, blinkFrameCounter;
        private int clothFrame, clothFrameCounter;
        private int extraAdjX, extraAdjY;

        private DrawPetConfig drawConfig = new(1);
        private readonly Texture2D clothTex = AltVanillaFunction.GetExtraTexture("Flandre_Cloth");
        private readonly Texture2D glowTex = AltVanillaFunction.GetGlowTexture("Flandre_Glow");
        public override void SetStaticDefaults()
        {
            Main.projFrames[Type] = 23;
            Main.projPet[Type] = true;
            ProjectileID.Sets.LightPet[Type] = true;
        }
        public override TouhouPetID UniqueID => TouhouPetID.Flandre;
        public override bool DrawPetSelf(ref Color lightColor)
        {
            Vector2 extraOffset = new Vector2(extraAdjX, extraAdjY);
            DrawPetConfig config = drawConfig with
            {
                ShouldUseEntitySpriteDraw = true,
                AltTexture = clothTex,
            };

            DrawWing(lightColor);
            Projectile.ResetDrawStateForPet();

            Projectile.DrawPet(Projectile.frame, lightColor, drawConfig);

            if (CurrentState == States.Blink)
                Projectile.DrawPet(blinkFrame, lightColor, drawConfig);

            Projectile.DrawPet(Projectile.frame, lightColor, config);

            if (CurrentState != States.Defense)
            {
                Projectile.DrawPet(clothFrame, lightColor,
                    drawConfig with
                    {
                        ShouldUseEntitySpriteDraw = true,
                        PositionOffset = extraOffset,
                    });
            }
            return false;
        }
        private void DrawWing(Color lightColor)
        {
            DrawPetConfig config = drawConfig with
            {
                PositionOffset = new Vector2(extraAdjX, extraAdjY),
            };
            Projectile.DrawPet(wingFrame, lightColor, config);

            Projectile.DrawPet(wingFrame, Color.White * 0.6f,
                    config with
                    {
                        AltTexture = glowTex,
                    });
        }
        public override ChatSettingConfig ChatSettingConfig => new ChatSettingConfig() with
        {
            TextColor = new Color(255, 10, 10),
        };
        public override void RegisterChat(ref string name, ref Vector2 indexRange)
        {
            name = "Flandre";
            indexRange = new Vector2(1, 11);
        }
        public override void SetRegularDialog(ref int timePerDialog, ref int chance, ref bool whenShouldStop)
        {
            timePerDialog = 480;
            chance = 12;
            whenShouldStop = !IsIdleState;
        }
        public override WeightedRandom<string> RegularDialogText()
        {
            WeightedRandom<string> chat = new WeightedRandom<string>();
            {
                chat.Add(ChatDictionary[1]);
                chat.Add(ChatDictionary[2]);
                chat.Add(ChatDictionary[3]);
                if (FindPet(ProjectileType<Meirin>(), true, 4))
                {
                    chat.Add(ChatDictionary[10]);
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
            TouhouPetID flandre = TouhouPetID.Flandre;
            TouhouPetID meirin = TouhouPetID.Meirin;

            List<ChatRoomInfo> list =
            [
                new ChatRoomInfo(flandre, 10, -1), //芙兰：美铃在跳什么奇怪的舞蹈吗？
                new ChatRoomInfo(meirin, 7, 0),//美铃：这叫"太极"哦，二小姐。
                new ChatRoomInfo(flandre, 11, 1), //芙兰：好像很厉害...可以教教芙兰吗？
                new ChatRoomInfo(meirin, 8, 2),//美铃：唔...可能需要大小姐的同意吧？
            ];

            return list;
        }
        public override void VisualEffectForPreview()
        {
            UpdateWingFrame();
            UpdateClothFrame();
        }
        public override void SetPetLight(ref Vector2 position, ref Vector3 rgb, ref bool inactive)
        {
            float r = Main.DiscoR / 255f;
            float g = Main.DiscoG / 255f;
            float b = Main.DiscoB / 255f;
            float strength = 2f;
            r = (strength + r) / 2f;
            g = (strength + g) / 2f;
            b = (strength + b) / 2f;
            Lighting.AddLight(Projectile.Center, r, g, b);

            rgb = new Vector3(0.90f, 0.31f, 0.68f);
        }
        public override void AI()
        {
            Projectile.SetPetActive(Owner, BuffType<FlandreBuff>());
            Projectile.SetPetActive(Owner, BuffType<ScarletBuff>());

            ControlMovement(Owner);

            if (ShouldDefense(Projectile) && CurrentState != States.Defense)
            {
                if (OwnerIsMyPlayer)
                {
                    Timer = 0;
                    CurrentState = States.Defense;
                    return;
                }
            }

            switch (CurrentState)
            {
                case States.Blink:
                    Blink();
                    break;

                case States.Defense:
                    Defense();
                    break;

                case States.BeforeEating:
                    shouldNotTalking = true;
                    BeforeEating();
                    break;

                case States.Eating:
                    shouldNotTalking = true;
                    Eating();
                    break;

                case States.AfterEating:
                    shouldNotTalking = true;
                    AfterEating();
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
        private void ControlMovement(Player player)
        {
            Projectile.tileCollide = false;
            if (PetState != 2)
                Projectile.rotation = Projectile.velocity.X * 0.03f;
            else
                Projectile.rotation = Projectile.velocity.X * 0.005f;

            Vector2 point = new Vector2(50 * player.direction, -40 + player.gfxOffY);
            if (FindPet(ProjectileType<Remilia>(), false))
            {
                point = new Vector2(-50 * player.direction, -40 + player.gfxOffY);
            }
            if (player.HasBuff<ScarletBuff>())
            {
                point = new Vector2(-60 * player.direction, -20 + player.gfxOffY);
            }

            ChangeDir();
            MoveToPoint(point, 19);
        }
        private void UpdateMiscData()
        {
            extraAdjX = 0;
            extraAdjY = 0;
            if (Projectile.frame >= 1 && Projectile.frame <= 9)
            {
                extraAdjY = -2;
                if (Projectile.frame != 1 && Projectile.frame != 6 && Projectile.frame != 9)
                {
                    extraAdjX = -2 * Projectile.spriteDirection;
                }
            }
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
                if (mainTimer > 0 && mainTimer % 600 == 0 && currentChatRoom == null && ActionCD <= 0)
                {
                    if (Main.rand.NextBool(4))
                    {
                        RandomCount = Main.rand.Next(90, 120);
                        CurrentState = States.BeforeEating;
                    }
                }
            }
        }
        private void Blink()
        {
            Projectile.frame = 0;
            if (blinkFrame < 20)
            {
                blinkFrame = 20;
            }
            if (++blinkFrameCounter > 3)
            {
                blinkFrameCounter = 0;
                blinkFrame++;
            }
            if (blinkFrame > 22)
            {
                blinkFrame = 20;
                CurrentState = States.Idle;
            }
        }
        private void BeforeEating()
        {
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
                    if (Main.rand.NextBool(3) && Timer == 0)
                    {
                        int chance = Main.rand.Next(2);
                        switch (chance)
                        {
                            case 1:
                                Projectile.SetChat(ChatSettingConfig, 4, 30);
                                break;
                            default:
                                Projectile.SetChat(ChatSettingConfig, 5, 30);
                                break;
                        }
                    }
                    Timer++;
                    if (Timer > RandomCount)
                    {
                        Timer = 0;
                        RandomCount = Main.rand.Next(120, 360);
                        CurrentState = States.Eating;
                    }
                }
            }
        }
        private void Eating()
        {
            int count = 10;
            if (Projectile.frame == 6)
                count = 24;

            if (++Projectile.frameCounter > count)
            {
                Projectile.frameCounter = 0;
                Projectile.frame++;
            }
            if (Projectile.frame >= 7)
            {
                Projectile.frame = 7;
                Timer++;
                if (OwnerIsMyPlayer && Timer > RandomCount)
                {
                    Timer = 0;
                    CurrentState = States.AfterEating;
                }
            }
        }
        private void AfterEating()
        {
            if (++Projectile.frameCounter > 10)
            {
                Projectile.frameCounter = 0;
                Projectile.frame++;
            }
            if (Projectile.frame > 9)
            {
                Projectile.frame = 0;
                if (OwnerIsMyPlayer)
                {
                    ActionCD = 900;
                    CurrentState = States.Idle;
                }
            }
        }
        private void Defense()
        {
            Projectile.rotation = 0f;
            Projectile.frame = 10;
            if (!ShouldDefense(Projectile) && OwnerIsMyPlayer)
            {
                CurrentState = States.Idle;
            }
        }
        private void UpdateWingFrame()
        {
            if (wingFrame < 15)
            {
                wingFrame = 15;
            }
            int count = 6;
            if (++wingFrameCounter > count)
            {
                wingFrameCounter = 0;
                wingFrame++;
            }
            if (wingFrame > 19)
            {
                wingFrame = 15;
            }
        }
        private void UpdateClothFrame()
        {
            if (clothFrame < 11)
            {
                clothFrame = 11;
            }
            int count = 4;
            if (++clothFrameCounter > count)
            {
                clothFrameCounter = 0;
                clothFrame++;
            }
            if (clothFrame > 14)
            {
                clothFrame = 11;
            }
        }
    }
}


