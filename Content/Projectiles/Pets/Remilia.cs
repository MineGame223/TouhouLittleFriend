using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ID;
using Terraria.Utilities;
using TouhouPets.Content.Buffs.PetBuffs;

namespace TouhouPets.Content.Projectiles.Pets
{
    public class Remilia : BasicTouhouPetNeo
    {
        public override void SetStaticDefaults()
        {
            Main.projFrames[Type] = 25;
            Main.projPet[Type] = true;
            ProjectileID.Sets.LightPet[Type] = false;
        }
        DrawPetConfig drawConfig = new(1);
        readonly Texture2D clothTex = AltVanillaFunction.GetExtraTexture("Remilia_Cloth");
        public override bool PreDraw(ref Color lightColor)
        {
            Vector2 extraOffset = new Vector2(extraAdjX, extraAdjY);
            Vector2 shake = new Vector2(Main.rand.Next(-1, 1), Main.rand.Next(-1, 1));
            DrawPetConfig config = drawConfig with
            {
                ShouldUseEntitySpriteDraw = true,
            };

            Projectile.DrawPet(wingFrame, lightColor,
                drawConfig with
                {
                    PositionOffset = extraOffset,
                });

            Projectile.DrawPet(Projectile.frame, lightColor, drawConfig);

            if (PetState == 1)
                Projectile.DrawPet(blinkFrame, lightColor, drawConfig);

            Projectile.DrawPet(Projectile.frame, lightColor,
                config with
                {
                    AltTexture = clothTex,
                    PositionOffset = HateSunlight(Projectile) ? shake : Vector2.Zero,
                });

            if (!HateSunlight(Projectile))
                Projectile.DrawPet(clothFrame, lightColor,
                config with
                {
                    PositionOffset = extraOffset,
                });
            return false;
        }
        public static bool HateSunlight(Projectile projectile)
        {
            Player player = Main.player[projectile.owner];
            bool sunlight = Main.dayTime && (player.ZoneOverworldHeight || player.ZoneSkyHeight) && !player.behindBackWall;
            bool rain = Main.raining && (player.ZoneOverworldHeight || player.ZoneSkyHeight);
            if (sunlight || rain)
            {
                if (projectile.type == ProjectileType<Remilia>() && player.ownedProjectileCounts[ProjectileType<Sakuya>()] > 0
                    || projectile.type == ProjectileType<Flandre>() && player.ownedProjectileCounts[ProjectileType<Meirin>()] > 0)
                    return false;
                else
                    return true;
            }
            return false;
        }
        private void Blink()
        {
            if (blinkFrame < 22)
            {
                blinkFrame = 22;
            }
            if (++blinkFrameCounter > 3)
            {
                blinkFrameCounter = 0;
                blinkFrame++;
            }
            if (blinkFrame > 24)
            {
                blinkFrame = 22;
                PetState = 0;
            }
        }
        int wingFrame, wingFrameCounter;
        int blinkFrame, blinkFrameCounter;
        int clothFrame, clothFrameCounter;
        int extraAdjX, extraAdjY;
        private void DrinkingTea()
        {
            if (Projectile.frame < 1)
            {
                Projectile.frame = 1;
            }
            if (++Projectile.frameCounter > 9)
            {
                Projectile.frameCounter = 0;
                Projectile.frame++;
            }
            if (extraAI[0] == 0)
            {
                if (Projectile.frame >= 5)
                {
                    Projectile.frame = 5;
                    extraAI[1]++;
                }
                if (Projectile.owner == Main.myPlayer)
                {
                    if (extraAI[1] > Main.rand.Next(120, 360))
                    {
                        extraAI[1] = 0;
                        extraAI[0] = 1;
                        Projectile.netUpdate = true;
                    }
                }
            }
            if (extraAI[0] == 1)
            {
                if (Projectile.frame >= 6)
                {
                    Projectile.frame = 6;
                    extraAI[1]++;
                }
                if (Projectile.owner == Main.myPlayer)
                {
                    if (extraAI[1] > Main.rand.Next(120, 360))
                    {
                        extraAI[1] = 0;
                        extraAI[2]++;
                        if (extraAI[2] > Main.rand.Next(3, 9))
                        {
                            extraAI[2] = 0;
                            extraAI[0] = 2;
                        }
                        else
                        {
                            extraAI[0] = 0;
                            if (Main.rand.NextBool(3) && currentChatRoom == null && chatTimeLeft <= 0)
                            {
                                int chance = Main.rand.Next(2);
                                switch (chance)
                                {
                                    case 1:
                                        Projectile.SetChat(ChatSettingConfig, 4, 20);
                                        break;
                                    default:
                                        Projectile.SetChat(ChatSettingConfig, 3, 20);
                                        break;
                                }
                            }
                        }
                        Projectile.netUpdate = true;
                    }
                }
            }
            else
            {
                if (Projectile.frame > 10)
                {
                    Projectile.frame = 0;
                    extraAI[0] = 1800;
                    PetState = 0;
                }
            }
        }
        private void UpdateWingFrame()
        {
            if (wingFrame < 16)
            {
                wingFrame = 16;
            }
            int count = 6;
            if (++wingFrameCounter > count)
            {
                wingFrameCounter = 0;
                wingFrame++;
            }
            if (wingFrame > 21)
            {
                wingFrame = 16;
            }
        }
        private void UpdateClothFrame()
        {
            if (clothFrame < 12)
            {
                clothFrame = 12;
            }
            int count = 4;
            if (++clothFrameCounter > count)
            {
                clothFrameCounter = 0;
                clothFrame++;
            }
            if (clothFrame > 15)
            {
                clothFrame = 12;
            }
        }
        public override Color ChatTextColor => new Color(255, 10, 10);
        public override void RegisterChat(ref string name, ref Vector2 indexRange)
        {
            name = "Remilia";
            indexRange = new Vector2(1, 14);
        }
        public override void SetRegularDialog(ref int timePerDialog, ref int chance, ref bool whenShouldStop)
        {
            timePerDialog = 720;
            chance = 9;
            whenShouldStop = HateSunlight(Projectile) || PetState == 2;
        }
        public override string GetRegularDialogText()
        {
            WeightedRandom<string> chat = new WeightedRandom<string>();
            {
                chat.Add(ChatDictionary[1]);
                chat.Add(ChatDictionary[2]);
                if (Main.bloodMoon)
                {
                    chat.Add(ChatDictionary[5]);
                }
                if (FindPet(ProjectileType<Flandre>()))
                {
                    chat.Add(ChatDictionary[6]);
                }
            }
            return chat;
        }
        public override void VisualEffectForPreview()
        {
            UpdateWingFrame();
            UpdateClothFrame();
        }
        private void UpdateTalking()
        {
            if (FindChatIndex(6, 8))
            {
                Chatting1(currentChatRoom ?? Projectile.CreateChatRoomDirect());
            }
        }
        private void Chatting1(PetChatRoom chatRoom)
        {
            int type = ProjectileType<Flandre>();
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
            Projectile remilia = chatRoom.initiator;
            Projectile flandre = chatRoom.member[0];
            int turn = chatRoom.chatTurn;
            if (turn == -1)
            {
                //蕾米：我亲爱的芙兰哟...
                flandre.CloseCurrentDialog();

                if (remilia.CurrentDialogFinished())
                    chatRoom.chatTurn++;
            }
            else if (turn == 0)
            {
                //芙兰：姐姐？叫芙兰有什么事嘛？
                flandre.SetChat(ChatSettingConfig, 6, 20);

                if (flandre.CurrentDialogFinished())
                    chatRoom.chatTurn++;
            }
            else if (turn == 1)
            {
                //蕾米：没什么...只是想叫你一下。
                remilia.SetChat(ChatSettingConfig, 7, 20);

                if (remilia.CurrentDialogFinished())
                    chatRoom.chatTurn++;
            }
            else if (turn == 2)
            {
                //芙兰：...姐姐什么时候能和芙兰一起玩...
                flandre.SetChat(ChatSettingConfig, 7, 20);

                if (flandre.CurrentDialogFinished())
                    chatRoom.chatTurn++;
            }
            else if (turn == 3)
            {
                //蕾米：有空会陪你的啦~
                remilia.SetChat(ChatSettingConfig, 8, 20);

                if (remilia.CurrentDialogFinished())
                    chatRoom.chatTurn++;
            }
            else if (turn == 4)
            {
                //芙兰：姐姐老是这么说...
                flandre.SetChat(ChatSettingConfig, 8, 20);

                if (flandre.CurrentDialogFinished())
                    chatRoom.chatTurn++;
            }
            else
            {
                chatRoom.CloseChatRoom();
            }
        }
        private void ControlMovement(Player player)
        {
            Projectile.tileCollide = false;
            if (PetState != 2)
                Projectile.rotation = Projectile.velocity.X * 0.03f;
            else
                Projectile.rotation = Projectile.velocity.X * 0.005f;

            Vector2 point = new Vector2(-50 * player.direction, -50 + player.gfxOffY);
            bool hasFlandre = player.ownedProjectileCounts[ProjectileType<Flandre>()] > 0;
            if (hasFlandre)
            {
                point = new Vector2(50 * player.direction, -50 + player.gfxOffY);
            }
            if (player.HasBuff<ScarletBuff>())
            {
                point = new Vector2(60 * player.direction, -20 + player.gfxOffY);
            }

            ChangeDir();
            MoveToPoint(point, 19f);
        }
        public override void AI()
        {
            Player player = Main.player[Projectile.owner];
            Projectile.SetPetActive(player, BuffType<RemiliaBuff>());
            Projectile.SetPetActive(player, BuffType<ScarletBuff>());

            UpdateTalking();
            ControlMovement(player);

            if (HateSunlight(Projectile))
            {
                extraAI[0] = 0;
                extraAI[1] = 0;
                extraAI[2] = 0;
                Projectile.rotation = 0f;
                PetState = 0;
                Projectile.frame = 11;
                return;
            }
            if (Projectile.owner == Main.myPlayer)
            {
                if (mainTimer % 270 == 0 && PetState != 2)
                {
                    PetState = 1;
                    Projectile.netUpdate = true;
                }
                if (mainTimer >= 1200 && mainTimer < 3600 && PetState != 1)
                {
                    if (mainTimer % 900 == 0 && Main.rand.NextBool(3) && extraAI[0] <= 0 && player.velocity.Length() < 4f)
                    {
                        PetState = 2;
                        Projectile.netUpdate = true;
                    }
                }
            }
            if (PetState == 0)
            {
                Projectile.frame = 0;
                if (extraAI[0] >= 1)
                {
                    extraAI[0]--;
                }
            }
            else if (PetState == 1)
            {
                Projectile.frame = 0;
                Blink();
            }
            else if (PetState == 2)
            {
                DrinkingTea();
            }
            extraAdjX = 0;
            extraAdjY = 0;
            if (Projectile.frame >= 1 && Projectile.frame <= 10)
            {
                extraAdjY = -2;
                if (Projectile.frame != 10)
                {
                    extraAdjX = -2 * Projectile.spriteDirection;
                }
            }
        }
    }
}


