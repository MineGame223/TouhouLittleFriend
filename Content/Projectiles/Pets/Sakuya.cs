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
    public class Sakuya : BasicTouhouPet
    {
        private enum States
        {
            Idle,
            Blink,
            Serve,
            TeaServe,
            ThrowingKnife,
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
        private bool IsIdleState => CurrentState <= States.Blink;
        private bool IsSevring => CurrentState >= States.Serve && CurrentState <= States.TeaServe;

        private int blinkFrame, blinkFrameCounter;
        private int clothFrame, clothFrameCounter;

        private DrawPetConfig drawConfig = new(1);
        private readonly Texture2D clothTex = AltVanillaFunction.GetExtraTexture("Sakuya_Cloth");
        public override void SetStaticDefaults()
        {
            Main.projFrames[Type] = 18;
            Main.projPet[Type] = true;

            ProjectileID.Sets.CharacterPreviewAnimations[Type] =
                ProjectileID.Sets.SimpleLoop(0, 1)
                .WhenSelected(1, 2, 9);
        }
        public override TouhouPetID UniqueID => TouhouPetID.Sakuya;
        public override bool DrawPetSelf(ref Color lightColor)
        {
            DrawPetConfig config = drawConfig with
            {
                ShouldUseEntitySpriteDraw = true,
            };

            Projectile.DrawPet(Projectile.frame, lightColor, drawConfig);

            if (CurrentState == States.Blink)
                Projectile.DrawPet(blinkFrame, lightColor, drawConfig);

            Projectile.DrawPet(Projectile.frame, lightColor,
                config with
                {
                    AltTexture = clothTex,
                });
            Projectile.DrawPet(clothFrame, lightColor, config);
            Projectile.ResetDrawStateForPet();

            if (Projectile.frame == 9)
            {
                DrawUmbrella(lightColor);
            }
            return false;
        }
        private void DrawUmbrella(Color lightColor)
        {
            int type = ItemID.TragicUmbrella;
            Texture2D tex = AltVanillaFunction.ItemTexture(type);
            Vector2 pos = Projectile.Center - Main.screenPosition + new Vector2(13 * Projectile.spriteDirection, -20) + new Vector2(0, 7f * Main.essScale);
            Color clr = Projectile.GetAlpha(lightColor);
            Vector2 orig = tex.Size() / 2;
            SpriteEffects effect = Projectile.spriteDirection == 1 ? SpriteEffects.None : SpriteEffects.FlipHorizontally;
            Main.EntitySpriteDraw(tex, pos, null, clr, Projectile.rotation, orig, Projectile.scale, effect, 0);
        }
        public override ChatSettingConfig ChatSettingConfig => new ChatSettingConfig() with
        {
            TextColor = new Color(114, 106, 255),
        };
        public override void RegisterChat(ref string name, ref Vector2 indexRange)
        {
            name = "Sakuya";
            indexRange = new Vector2(1, 10);
        }
        public override void SetRegularDialog(ref int timePerDialog, ref int chance, ref bool whenShouldStop)
        {
            timePerDialog = 960;
            chance = Owner.HasBuff<ScarletBuff>() ? 30 : 12;
            whenShouldStop = !IsIdleState;
        }
        public override WeightedRandom<string> RegularDialogText()
        {
            WeightedRandom<string> chat = new WeightedRandom<string>();
            {
                chat.Add(ChatDictionary[1]);
                chat.Add(ChatDictionary[3]);
                if (FindPet(ProjectileType<Meirin>()))
                {
                    chat.Add(ChatDictionary[4]);
                }
                chat.Add(ChatDictionary[10]);
            }
            return chat;
        }
        public override void VisualEffectForPreview()
        {
            UpdateClothFrame();
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
        private static List<ChatRoomInfo> Chatting1()
        {
            TouhouPetID sakuya = TouhouPetID.Sakuya;
            TouhouPetID remilia = TouhouPetID.Remilia;

            List<ChatRoomInfo> list =
            [
                new ChatRoomInfo(sakuya, 1, -1), //咲夜：过去已为过去，如今只要侍奉大小姐便是。
                new ChatRoomInfo(remilia, 14, 0),//蕾米：咲夜还记得你过去的日子吗？
                new ChatRoomInfo(sakuya, 2, 1),//咲夜：从遇到您的那一刻起我的人生就重新开始了，没有所谓过去了。
            ];

            return list;
        }
        private static List<ChatRoomInfo> Chatting2()
        {
            TouhouPetID sakuya = TouhouPetID.Sakuya;
            TouhouPetID remilia = TouhouPetID.Remilia;

            List<ChatRoomInfo> list =
            [
                new ChatRoomInfo(sakuya, 5, -1), //咲夜：大小姐能安好，我就安好。
                new ChatRoomInfo(remilia, 13, 0),//蕾米：咲夜偶尔也得为自己考虑一下嘛。
            ];

            return list;
        }
        private static List<ChatRoomInfo> Chatting3()
        {
            TouhouPetID sakuya = TouhouPetID.Sakuya;
            TouhouPetID meirin = TouhouPetID.Meirin;

            List<ChatRoomInfo> list =
            [
                new ChatRoomInfo(sakuya, 5, -1), //咲夜：美铃那家伙，是不是又在偷懒了...
                new ChatRoomInfo(meirin, 9, 0),//美铃：我才没有呐！
            ];

            return list;
        }
        public override void AI()
        {
            Projectile.SetPetActive(Owner, BuffType<SakuyaBuff>());
            Projectile.SetPetActive(Owner, BuffType<ScarletBuff>());

            ControlMovement(Owner);


            if (OwnerIsMyPlayer)
            {
                if (FindPet(out Projectile master, ProjectileType<Remilia>(), -1, 0, false)
                    && CurrentState != States.Serve)
                {
                    if (ShouldDefense(Projectile))
                    {
                        Teleport(master.Center + new Vector2(-20 * master.spriteDirection, Owner.gfxOffY));
                        CurrentState = States.Serve;
                        return;
                    }
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

                case States.TeaServe:
                    shouldNotTalking = true;
                    TeaServe();
                    break;

                case States.ThrowingKnife:
                    shouldNotTalking = true;
                    ThrowingKnife();
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
            if (IsIdleState)
                Projectile.rotation = Projectile.velocity.X * 0.01f;
            else
                Projectile.rotation = Projectile.velocity.X * 0.001f;

            ChangeDir(120);

            Vector2 point = new Vector2((player.HasBuff<ScarletBuff>() ? 100 : -50) * player.direction, -30 + player.gfxOffY);
            Vector2 center = default;
            float speed = 16f;
            if (FindPet(out Projectile master, ProjectileType<Remilia>(), -1, 0, false))
            {
                Projectile.spriteDirection = master.spriteDirection;
                if (IsSevring)
                {
                    int xOffset = CurrentState == States.TeaServe ? 20 : 40;
                    point = new Vector2(xOffset * master.spriteDirection, player.gfxOffY - 20);
                    speed = 19f;
                }
            }
            MoveToPoint(point, speed, center);
        }
        private void Teleport(Vector2 targetPos)
        {
            for (int i = 0; i < 20; i++)
            {
                Dust.NewDustDirect(Projectile.position, Projectile.width, Projectile.height, MyDustId.BlueMagic, 0, 0
                    , 100, default, Main.rand.NextFloat(1.5f, 2.2f)).noGravity = true;
            }
            Projectile.Center = targetPos;
            for (int i = 0; i < 20; i++)
            {
                Dust.NewDustDirect(Projectile.position, Projectile.width, Projectile.height, MyDustId.BlueMagic, 0, 0
                    , 100, default, Main.rand.NextFloat(1.5f, 2.2f)).noGravity = true;
            }
        }
        private void Idle()
        {
            Projectile.frame = 0;
            if (OwnerIsMyPlayer)
            {
                if (FindPet(out Projectile master, ProjectileType<Remilia>(), 3, 4, false))
                {
                    Teleport(master.Center + new Vector2(-40 * master.spriteDirection, Owner.gfxOffY));
                    CurrentState = States.TeaServe;
                }
                if (FindPet(ProjectileType<Meirin>(), false, 6))
                {
                    if (mainTimer % 320 == 0 && Main.rand.NextBool(3))
                    {
                        CurrentState = States.ThrowingKnife;
                    }
                }
                if (mainTimer % 270 == 0)
                {
                    CurrentState = States.Blink;
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
        private void TeaServe()
        {
            if (++Projectile.frameCounter > 9)
            {
                Projectile.frameCounter = 0;
                Projectile.frame++;
                if (OwnerIsMyPlayer)
                {
                    if (Main.rand.NextBool(15) && Timer == 0)
                    {
                        Projectile.SetChat(ChatSettingConfig, 5);
                        Timer++;
                    }
                }
            }
            if (Projectile.frame > 2)
            {
                Projectile.frame = 1;
            }
            if (!FindPet(ProjectileType<Remilia>(), false, 3, 4))
            {
                if (OwnerIsMyPlayer)
                {
                    Timer = 0;
                    CurrentState = States.Idle;
                }
            }
        }
        private void Serve()
        {
            Projectile.frame = 9;
            if (!FindPet(ProjectileType<Remilia>(), false) || !ShouldDefense(Projectile))
            {
                if (OwnerIsMyPlayer)
                {
                    CurrentState = States.Idle;
                }
            }
        }
        private void ThrowingKnife()
        {
            if (Projectile.frame < 3)
            {
                Projectile.frame = 3;
            }
            if (++Projectile.frameCounter > 9)
            {
                Projectile.frameCounter = 0;
                Projectile.frame++;
            }
            if (FindPet(out Projectile meirin, ProjectileType<Meirin>(), -1, 0, false))
            {
                Projectile.spriteDirection = Projectile.position.X > meirin.position.X ? -1 : 1;
                if (Projectile.frameCounter == 2 && Projectile.frame == 6)
                {
                    Vector2 vel = Vector2.Normalize(meirin.Center - Projectile.Center) * 12f;
                    Projectile.NewProjectileDirect(Projectile.GetSource_FromAI(), Projectile.Center, vel, ProjectileType<SakuyaKnife>()
                        , 0, 0, Projectile.owner);
                }
            }
            if (Projectile.frame > 8)
            {
                Projectile.frame = 0;
                if (OwnerIsMyPlayer)
                {
                    CurrentState = States.Idle;
                }
            }
        }
        private void UpdateClothFrame()
        {
            if (clothFrame < 13)
            {
                clothFrame = 13;
            }
            int count = 4;
            if (++clothFrameCounter > count)
            {
                clothFrameCounter = 0;
                clothFrame++;
            }
            if (clothFrame > 16)
            {
                clothFrame = 13;
            }
        }
    }
}


