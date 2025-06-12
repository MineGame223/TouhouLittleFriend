using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.DataStructures;
using Terraria.Localization;
using Terraria.Utilities;
using TouhouPets.Content.Buffs.PetBuffs;

namespace TouhouPets.Content.Projectiles.Pets
{
    public class Koakuma : BasicTouhouPet
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
        private bool EarActive
        {
            get => Projectile.ai[2] == 0;
            set => Projectile.ai[2] = value ? 0 : 1;
        }

        private int wingFrame, wingFrameCounter;
        private int blinkFrame, blinkFrameCounter;
        private int earFrame, earFrameCounter;
        private int hairFrame, hairFrameCounter;

        private DrawPetConfig drawConfig = new(2);
        private readonly Texture2D clothTex = AltVanillaFunction.GetExtraTexture("Koakuma_Cloth");
        public override void SetStaticDefaults()
        {
            Main.projFrames[Type] = 10;
            Main.projPet[Type] = true;
        }
        public override TouhouPetID UniqueID => TouhouPetID.Koakuma;
        public override void OnSpawn(IEntitySource source)
        {
            TouhouPetPlayer lp = Main.LocalPlayer.GetModPlayer<TouhouPetPlayer>();
            lp.koakumaNumber = Main.rand.Next(1, 301);
            Projectile.Name = Language.GetTextValue("Mods.TouhouPets.Projectiles.Koakuma.DisplayName", NumberToCNCharacter.GetNumberText(lp.koakumaNumber));
            ChatDictionary[1] = ModUtils.GetChatText("Koakuma", "1", NumberToCNCharacter.GetNumberText(lp.koakumaNumber));

            base.OnSpawn(source);
        }
        public override bool DrawPetSelf(ref Color lightColor)
        {
            Projectile.DrawPet(hairFrame, lightColor, drawConfig);

            Projectile.DrawPet(wingFrame, lightColor, drawConfig, 1);

            Projectile.DrawPet(Projectile.frame, lightColor, drawConfig);

            Projectile.DrawPet(earFrame, lightColor, drawConfig, 1);

            if (CurrentState == States.Blink)
                Projectile.DrawPet(blinkFrame, lightColor, drawConfig);

            Projectile.DrawPet(Projectile.frame, lightColor,
                drawConfig with
                {
                    AltTexture = clothTex,
                    ShouldUseEntitySpriteDraw = true,
                });
            return false;
        }
        public override ChatSettingConfig ChatSettingConfig => new ChatSettingConfig() with
        {
            TextColor = new Color(224, 78, 78),
        };
        public override void RegisterChat(ref string name, ref Vector2 indexRange)
        {
            name = "Koakuma";
            indexRange = new Vector2(2, 7);
        }
        public override void SetRegularDialog(ref int timePerDialog, ref int chance, ref bool whenShouldStop)
        {
            timePerDialog = 666;
            chance = 6;
            whenShouldStop = false;
        }
        public override WeightedRandom<string> RegularDialogText()
        {
            WeightedRandom<string> chat = new WeightedRandom<string>();
            {
                chat.Add(ChatDictionary[1]);
                chat.Add(ChatDictionary[2]);
                chat.Add(ChatDictionary[3]);
                if (FindPet(ProjectileType<Patchouli>()))
                {
                    chat.Add(ChatDictionary[4], 4);
                }
                if (FindPet(ProjectileType<Patchouli>(), true, 2, 4))
                {
                    chat.Add(ChatDictionary[6], 4);
                }
            }
            return chat;
        }
        public override void VisualEffectForPreview()
        {
            UpdateWingFrame();
            UpdateHairFrame();
            UpdateEarsFrame();
        }
        private void UpdateTalking()
        {
            if (FindChatIndex(4, 7))
            {
                Chatting1(currentChatRoom ?? Projectile.CreateChatRoomDirect(), chatIndex);
            }
        }
        private void Chatting1(PetChatRoom chatRoom, int index)
        {
            int type = ProjectileType<Patchouli>();
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
            Projectile koakuma = chatRoom.initiator;
            Projectile patchouli = chatRoom.member[0];
            int turn = chatRoom.chatTurn;
            if (index >= 4 && index <= 5)
            {
                if (turn == -1)
                {
                    //小恶魔：帕秋莉大人！该锻炼身体啦！
                    patchouli.CloseCurrentDialog();

                    if (koakuma.CurrentDialogFinished())
                        chatRoom.chatTurn++;
                }
                else if (turn == 0)
                {
                    //帕秋莉：不要！会累死人的...
                    patchouli.SetChat(ChatSettingConfig, 16, 20);

                    if (patchouli.CurrentDialogFinished())
                        chatRoom.chatTurn++;
                }
                else if (turn == 1)
                {
                    //小恶魔：为了您的健康着想，这很必要的哦！
                    koakuma.SetChat(ChatSettingConfig, 5, 20);

                    if (koakuma.CurrentDialogFinished())
                        chatRoom.chatTurn++;
                }
                else if (turn == 2)
                {
                    //帕秋莉：一点都不必要，我现在挺好的...咳咳！咳！
                    patchouli.SetChat(ChatSettingConfig, 17, 20);

                    if (patchouli.CurrentDialogFinished())
                        chatRoom.chatTurn++;
                }
                else if (turn == 3)
                {
                    //帕秋莉：...我真的很好！
                    patchouli.SetChat(ChatSettingConfig, 18, 20);

                    if (patchouli.CurrentDialogFinished())
                        chatRoom.chatTurn++;
                }
                else
                {
                    chatRoom.CloseChatRoom();
                }
            }
            else if (index == 6 || index == 7)
            {
                if (turn == -1)
                {
                    //小恶魔：帕秋莉大人在看什么？
                    patchouli.CloseCurrentDialog();

                    if (koakuma.CurrentDialogFinished())
                        chatRoom.chatTurn++;
                }
                else if (turn == 0)
                {
                    //帕秋莉：是关于xxxx的书...
                    patchouli.SetChat(ChatSettingConfig, Main.rand.Next(19, 36), 20);

                    if (patchouli.CurrentDialogFinished())
                        chatRoom.chatTurn++;
                }
                else if (turn == 1)
                {
                    //小恶魔：好像很有趣啊！
                    koakuma.SetChat(ChatSettingConfig, 7, 20);

                    if (koakuma.CurrentDialogFinished())
                        chatRoom.chatTurn++;
                }
                else
                {
                    chatRoom.CloseChatRoom();
                }
            }
        }
        public override void AI()
        {
            Projectile.SetPetActive(Owner, BuffType<KoakumaBuff>());
            Projectile.SetPetActive(Owner, BuffType<ScarletBuff>());

            UpdateTalking();

            ControlMovement(Owner);

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
        private void ControlMovement(Player player)
        {
            Projectile.tileCollide = false;
            Projectile.rotation = Projectile.velocity.X * 0.022f;

            ChangeDir();

            Vector2 point;
            Vector2 center = default;
            float speed = 9f;
            if (FindPet(ProjectileType<Patchouli>(), false) && player.HasBuff<ScarletBuff>())
            {
                point = new Vector2(-50 * player.direction, player.gfxOffY - 120);
                Projectile.spriteDirection = player.direction;
                speed = 4.5f;
            }
            else
            {
                point = new Vector2(-50 * player.direction, -30 + player.gfxOffY);
            }

            MoveToPoint(point, speed, center);
        }
        private void Idle()
        {
            Projectile.frame = 0;
            if (OwnerIsMyPlayer && mainTimer % 270 == 0)
            {
                if (Main.rand.NextBool(4))
                {
                    EarActive = true;
                    Projectile.netUpdate = true;
                }
                CurrentState = States.Blink;
            }
        }
        private void Blink()
        {
            Projectile.frame = 0;
            if (blinkFrame < 3)
            {
                blinkFrame = 3;
            }
            if (++blinkFrameCounter > 3)
            {
                blinkFrameCounter = 0;
                blinkFrame++;
            }
            if (blinkFrame > 5)
            {
                blinkFrame = 3;
                CurrentState = States.Idle;
            }
        }
        private void UpdateWingFrame()
        {
            if (wingFrame < 4)
            {
                wingFrame = 4;
            }
            if (++wingFrameCounter > 5)
            {
                wingFrameCounter = 0;
                wingFrame++;
            }
            if (wingFrame > 9)
            {
                wingFrame = 4;
            }
        }
        private void UpdateHairFrame()
        {
            if (hairFrame < 6)
            {
                hairFrame = 6;
            }
            if (++hairFrameCounter > 7)
            {
                hairFrameCounter = 0;
                hairFrame++;
            }
            if (hairFrame > 9)
            {
                hairFrame = 6;
            }
        }
        private void UpdateEarsFrame()
        {
            if (++earFrameCounter > 5 && EarActive)
            {
                earFrameCounter = 0;
                earFrame++;
            }
            if (earFrame > 3)
            {
                earFrame = 0;
                EarActive = false;
            }
        }
    }
}


