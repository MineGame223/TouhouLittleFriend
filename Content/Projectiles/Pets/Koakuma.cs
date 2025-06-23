using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;
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
            ChatDictionary[1] = GetChatText("Koakuma", 1, NumberToCNCharacter.GetNumberText(lp.koakumaNumber));

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
            timePerDialog = 666;//666
            chance = 6;//6
            whenShouldStop = false;
        }
        public override WeightedRandom<LocalizedText> RegularDialogText()
        {
            WeightedRandom<LocalizedText> chat = new ();
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
        public override List<List<ChatRoomInfo>> RegisterChatRoom()
        {
            return new()
            {
                Chatting1(),
                Chatting2(),
            };
        }
        private List<ChatRoomInfo> Chatting1()
        {
            TouhouPetID koakuma = TouhouPetID.Koakuma;
            TouhouPetID patchouli = TouhouPetID.Patchouli;

            List<ChatRoomInfo> list =
            [
                new ChatRoomInfo(koakuma, ChatDictionary[4], -1), //小恶魔：帕秋莉大人！该锻炼身体啦！
                new ChatRoomInfo(patchouli, GetChatText("Patchouli",16), 0),//帕秋莉：不要！会累死人的...
                new ChatRoomInfo(koakuma, ChatDictionary[5], 1), //小恶魔：为了您的健康着想，这很必要的哦！
                new ChatRoomInfo(patchouli, GetChatText("Patchouli",17), 2),//帕秋莉：一点都不必要，我现在挺好的...咳咳！咳！
                new ChatRoomInfo(patchouli, ChatDictionary[18], 3),//帕秋莉：...我真的很好！
            ];

            return list;
        }
        private List<ChatRoomInfo> Chatting2()
        {
            TouhouPetID koakuma = TouhouPetID.Koakuma;
            TouhouPetID patchouli = TouhouPetID.Patchouli;

            List<ChatRoomInfo> list =
            [
                new ChatRoomInfo(koakuma, ChatDictionary[6], -1), //小恶魔：帕秋莉大人在看什么？
                new ChatRoomInfo(patchouli, GetChatText("Patchouli",Main.rand.Next(19, 36)), 0),//帕秋莉：是关于xxxx的书...
                new ChatRoomInfo(koakuma, ChatDictionary[7], 1), //小恶魔：好像很有趣啊！
            ];

            return list;
        }
        public override void AI()
        {
            Projectile.SetPetActive(Owner, BuffType<KoakumaBuff>());
            Projectile.SetPetActive(Owner, BuffType<ScarletBuff>());

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


