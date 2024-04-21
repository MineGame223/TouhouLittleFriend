using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ID;
using Terraria.Utilities;
using TouhouPets.Content.Buffs.PetBuffs;

namespace TouhouPets.Content.Projectiles.Pets
{
    public class Flandre : BasicTouhouPetNeo
    {
        public override void SetStaticDefaults()
        {
            Main.projFrames[Type] = 23;
            Main.projPet[Type] = true;
            ProjectileID.Sets.LightPet[Type] = true;
        }
        DrawPetConfig drawConfig = new(1);
        readonly Texture2D clothTex = AltVanillaFunction.GetExtraTexture("Flandre_Cloth");
        readonly Texture2D glowTex = AltVanillaFunction.GetGlowTexture("Flandre_Glow");
        public override bool PreDraw(ref Color lightColor)
        {
            DrawPetConfig config = drawConfig with
            {
                ShouldUseEntitySpriteDraw = true,
                AltTexture = clothTex,
            };

            DrawWing(lightColor);
            Projectile.DrawStateNormalizeForPet();

            Projectile.DrawPet(Projectile.frame, lightColor, drawConfig);

            if (PetState == 1)
                Projectile.DrawPet(blinkFrame, lightColor, drawConfig);

            Projectile.DrawPet(Projectile.frame, lightColor, config);

            if (!Remilia.HateSunlight(Projectile))
            {
                Projectile.DrawPet(clothFrame, lightColor,
                    drawConfig with
                    {
                        ShouldUseEntitySpriteDraw = true
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

            Main.spriteBatch.QuickToggleAdditiveMode(true, Projectile.isAPreviewDummy);
            for (int i = -1; i <= 1; i++)
            {
                if (i == 0)
                    continue;

                Vector2 offset = new Vector2(extraAdjX, extraAdjY);
                DrawPetConfig config2 = config with
                {
                    AltTexture = glowTex,
                    Scale = 0.95f,
                    PositionOffset = offset,
                };
                Projectile.DrawPet(wingFrame, Color.White * 0.4f,
                    config2 with
                    {
                        PositionOffset = offset + new Vector2(0, i).RotatedBy(Main.GlobalTimeWrappedHourly),
                    });
                Projectile.DrawPet(wingFrame, Color.White * 0.4f,
                    config2 with
                    {
                        PositionOffset = offset + new Vector2(i, 0).RotatedBy(Main.GlobalTimeWrappedHourly),
                    });
            }
            Main.spriteBatch.QuickToggleAdditiveMode(false, Projectile.isAPreviewDummy);

            Projectile.DrawPet(wingFrame, Color.White * 0.6f,
                    config with
                    {
                        AltTexture = glowTex,
                    });
        }
        private void Blink()
        {
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
                PetState = 0;
            }
        }
        int wingFrame, wingFrameCounter;
        int blinkFrame, blinkFrameCounter;
        int clothFrame, clothFrameCounter;
        int extraAdjX, extraAdjY;
        private void Eatting()
        {
            if (Projectile.frame < 1)
            {
                Projectile.frame = 1;
            }
            int count = 10;
            if (Projectile.frame == 6)
                count = 24;

            if (++Projectile.frameCounter > count)
            {
                Projectile.frameCounter = 0;
                Projectile.frame++;
            }
            if (extraAI[0] == 0)
            {
                if (Projectile.frame >= 3)
                {
                    Projectile.frame = 3;
                    extraAI[1]++;
                    if (Projectile.owner == Main.myPlayer)
                    {
                        if (Main.rand.NextBool(3) && extraAI[2] == 0)
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
                            extraAI[2] = 1;
                        }
                    }
                }
                if (Projectile.owner == Main.myPlayer)
                {
                    if (extraAI[1] > Main.rand.Next(90, 120))
                    {
                        extraAI[1] = 0;
                        extraAI[0] = 1;
                        extraAI[2] = 0;
                        Projectile.netUpdate = true;
                    }
                }
            }
            else if (extraAI[0] == 1)
            {
                if (Projectile.frame >= 7)
                {
                    Projectile.frame = 7;
                    extraAI[1]++;
                }
                if (Projectile.owner == Main.myPlayer)
                {
                    if (extraAI[1] > Main.rand.Next(120, 360))
                    {
                        extraAI[1] = 0;
                        extraAI[0] = 2;
                        Projectile.netUpdate = true;
                    }
                }
            }
            else
            {
                Projectile.frameCounter += 1;
                if (Projectile.frame > 9)
                {
                    Projectile.frame = 0;
                    extraAI[0] = 1200;
                    PetState = 0;
                }
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
        public override Color ChatTextColor => new Color(255, 10, 10);
        public override void RegisterChat(ref string name, ref Vector2 indexRange)
        {
            name = "Flandre";
            indexRange = new Vector2(1, 11);
        }
        public override void SetRegularDialog(ref int timePerDialog, ref int chance, ref bool whenShouldStop)
        {
            timePerDialog = 480;
            chance = 12;
            whenShouldStop = false;
        }
        public override string GetRegularDialogText()
        {
            WeightedRandom<string> chat = new WeightedRandom<string>();
            {
                chat.Add(ChatDictionary[1]);
                chat.Add(ChatDictionary[2]);
                chat.Add(ChatDictionary[3]);
                if (FindPet(ProjectileType<Meirin>(), true, 2))
                {
                    chat.Add(ChatDictionary[10]);
                }
            }
            return chat;
        }
        private void UpdateTalking()
        {
            if (FindChatIndex(10, 11))
            {
                Chatting1(currentChatRoom ?? Projectile.CreateChatRoomDirect());
            }
        }
        private void Chatting1(PetChatRoom chatRoom)
        {
            int type = ProjectileType<Meirin>();
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
            Projectile flandre = chatRoom.initiator;
            Projectile meirin = chatRoom.member[0];
            int turn = chatRoom.chatTurn;
            if (turn == -1)
            {
                //芙兰：美铃在跳什么奇怪的舞蹈吗？
                meirin.CloseCurrentDialog();

                if (flandre.CurrentDialogFinished())
                    chatRoom.chatTurn++;
            }
            else if (turn == 0)
            {
                //美铃：这叫"太极"哦，二小姐。
                meirin.SetChat(ChatSettingConfig, 7, 20);

                if (meirin.CurrentDialogFinished())
                    chatRoom.chatTurn++;
            }
            else if (turn == 1)
            {
                //芙兰：好像很厉害...可以教教芙兰吗？
                flandre.SetChat(ChatSettingConfig, 11, 20);

                if (flandre.CurrentDialogFinished())
                    chatRoom.chatTurn++;
            }
            else if (turn == 2)
            {
                //美铃：唔...可能需要大小姐的同意吧？
                meirin.SetChat(ChatSettingConfig, 8, 20);

                if (meirin.CurrentDialogFinished())
                    chatRoom.chatTurn++;
            }
            else
            {
                chatRoom.CloseChatRoom();
            }
        }
        public override void VisualEffectForPreview()
        {
            UpdateWingFrame();
            UpdateClothFrame();
        }
        private void SetFlandreLight()
        {
            float r = Main.DiscoR / 255f;
            float g = Main.DiscoG / 255f;
            float b = Main.DiscoB / 255f;
            float strength = 2f;
            r = (strength + r) / 2f;
            g = (strength + g) / 2f;
            b = (strength + b) / 2f;
            Lighting.AddLight(Projectile.Center, r, g, b);
            Lighting.AddLight(Projectile.Center, 0.90f, 0.31f, 0.68f);
        }
        private void ControlMovement(Player player)
        {
            Projectile.tileCollide = false;
            if (PetState != 2)
                Projectile.rotation = Projectile.velocity.X * 0.03f;
            else
                Projectile.rotation = Projectile.velocity.X * 0.005f;

            Vector2 point = new Vector2(50 * player.direction, -40 + player.gfxOffY);
            bool hasRemilia = player.ownedProjectileCounts[ProjectileType<Remilia>()] > 0;
            if (hasRemilia)
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
        public override void AI()
        {
            SetFlandreLight();
            Player player = Main.player[Projectile.owner];
            Projectile.SetPetActive(player, BuffType<FlandreBuff>());
            Projectile.SetPetActive(player, BuffType<ScarletBuff>());

            UpdateTalking();
            ControlMovement(player);

            if (Remilia.HateSunlight(Projectile))
            {
                extraAI[0] = 0;
                extraAI[1] = 0;
                extraAI[2] = 0;
                Projectile.rotation = 0f;
                PetState = 0;
                Projectile.frame = 10;
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
                    if (mainTimer % 600 == 0 && Main.rand.NextBool(4) && extraAI[0] <= 0)
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
                Eatting();
            }
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
    }
}


