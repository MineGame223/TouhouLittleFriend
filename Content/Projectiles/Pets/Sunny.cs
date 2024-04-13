using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ID;
using Terraria.Utilities;
using TouhouPets.Content.Buffs.PetBuffs;

namespace TouhouPets.Content.Projectiles.Pets
{
    public class Sunny : BasicTouhouPetNeo
    {
        private bool RainWet => Main.raining &&
            (Owner.ZoneOverworldHeight || Owner.ZoneSkyHeight);

        private bool UnderSunShine => Main.cloudAlpha <= 0 && Main.dayTime &&
            (Owner.ZoneOverworldHeight || Owner.ZoneSkyHeight);
        public override void SetStaticDefaults()
        {
            Main.projFrames[Type] = 13;
            Main.projPet[Type] = true;
            ProjectileID.Sets.LightPet[Type] = true;
        }
        DrawPetConfig drawConfig = new(2);
        readonly Texture2D clothTex = AltVanillaFunction.GetExtraTexture("Sunny_Cloth");
        public override bool PreDraw(ref Color lightColor)
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

            Projectile.DrawPet(wingsFrame, lightColor * 0.7f, config, 1);

            Projectile.DrawPet(hairFrame, lightColor, config, 1);

            Projectile.DrawPet(Projectile.frame, lightColor,
                drawConfig with
                {
                    PositionOffset = offset,
                });

            if (PetState == 1 || PetState == 4)
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
            Projectile.DrawStateNormalizeForPet();

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
        private void Blink(bool alt = false)
        {
            int startFrame = alt ? 11 : 10;
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
                PetState = alt ? 3 : 0;
            }
        }
        int blinkFrame, blinkFrameCounter;
        int wingsFrame, wingsFrameCounter;
        int clothFrame, clothFrameCounter;
        int hairFrame, hairFrameCounter;
        float extraX, extraY, phantomTime;
        private void Happy()
        {
            if (++Projectile.frameCounter > 8)
            {
                Projectile.frameCounter = 0;
                Projectile.frame++;
            }
            if (extraAI[0] == 0)
            {
                if (Projectile.frame > 3)
                {
                    Projectile.frame = 2;
                    extraAI[1]++;
                }

                if (OwnerIsMyPlayer)
                {
                    if (extraAI[1] > Main.rand.Next(6, 12))
                    {
                        extraAI[1] = 0;
                        extraAI[0] = 1;
                        Projectile.netUpdate = true;
                    }
                }
            }
            else
            {
                if (Projectile.frame > 3)
                {
                    Projectile.frame = 0;
                    extraAI[0] = 3600;
                    PetState = 0;
                }
            }
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
            if (PetState == 2)
            {
                phantomTime += 0.1f;
            }
            else
            {
                phantomTime -= 0.1f;
            }
            phantomTime = MathHelper.Clamp(phantomTime, 0, 1);
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

            if (PetState >= 3)
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
        public override Color ChatTextColor => new Color(240, 196, 48);
        public override void RegisterChat(ref string name, ref Vector2 indexRange)
        {
            name = "Sunny";
            indexRange = new Vector2(1, 15);
        }
        public override void SetRegularDialog(ref int timePerDialog, ref int chance, ref bool whenShouldStop)
        {
            timePerDialog = 640;
            chance = 6;
            whenShouldStop = PetState == 2;
        }
        public override string GetRegularDialogText()
        {
            WeightedRandom<string> chat = new WeightedRandom<string>();
            {
                if (RainWet)
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
                        chat.Add(ChatDictionary[9], 10);
                    }
                    if (!Owner.HasBuff<ReimuBuff>())
                        chat.Add(ChatDictionary[12], 10);
                }
            }
            return chat;
        }
        public override void VisualEffectForPreview()
        {
            UpdateMiscFrame();
            if (PetState < 2)
            {
                Projectile.frame = 0;
            }
        }
        private void UpdateTalking()
        {
            if (FindChatIndex(9, 11))
            {
                Chatting1(currentChatRoom ?? Projectile.CreateChatRoomDirect());
            }
            if (FindChatIndex(12, 15))
            {
                Chatting2(currentChatRoom ?? Projectile.CreateChatRoomDirect());
            }
        }
        private void Chatting1(PetChatRoom chatRoom)
        {
            int turn = chatRoom.chatTurn;
            if (turn == -1)
            {
                //桑尼：日光妖精~ 洁白身体~
                if (Projectile.CurrentDialogFinished())
                    chatRoom.chatTurn++;
            }
            else if (turn == 0)
            {
                //桑尼：日光妖精~ 碧蓝双眸~
                Projectile.SetChat(ChatSettingConfig, 10, 20);

                if (Projectile.CurrentDialogFinished())
                    chatRoom.chatTurn++;
            }
            else if (turn == 1)
            {
                //桑尼：日光妖精—— 桑尼！米尔克！
                Projectile.SetChat(ChatSettingConfig, 11, 20);

                if (Projectile.CurrentDialogFinished())
                    chatRoom.chatTurn++;
            }
            else
            {
                chatRoom.CloseChatRoom();
            }
        }
        private void Chatting2(PetChatRoom chatRoom)
        {
            int type = ProjectileType<Luna>();
            int type2 = ProjectileType<Star>();
            if (FindPet(out Projectile member, type) && FindPet(out Projectile member2, type2))
            {
                chatRoom.member[0] = member;
                member.ToPetClass().currentChatRoom = chatRoom;

                chatRoom.member[1] = member2;
                member2.ToPetClass().currentChatRoom = chatRoom;
            }
            else
            {
                chatRoom.CloseChatRoom();
                return;
            }

            Projectile sunny = chatRoom.initiator;
            Projectile luna = chatRoom.member[0];
            Projectile star = chatRoom.member[1];
            int turn = chatRoom.chatTurn;
            if (turn == -1)
            {
                //桑尼：下一次该去哪里恶作剧呢？
                if (sunny.CurrentDialogFinished())
                    chatRoom.chatTurn++;
            }
            else if (turn == 0)
            {
                //斯塔：要不去偷那个黑白魔法使的蘑菇吧？
                star.SetChat(ChatSettingConfig, 6, 20);

                if (star.CurrentDialogFinished())
                    chatRoom.chatTurn++;
            }
            else if (turn == 1)
            {
                //露娜：且不说被发现了会怎么样...咱们去小偷家里偷东西？
                luna.SetChat(ChatSettingConfig, 9, 20);

                if (luna.CurrentDialogFinished())
                    chatRoom.chatTurn++;
            }
            else if (turn == 2)
            {
                //桑尼：没事的啦！露娜你只管殿后就好啦。
                sunny.SetChat(ChatSettingConfig, 13, 20);

                if (sunny.CurrentDialogFinished())
                    chatRoom.chatTurn++;
            }
            else if (turn == 3)
            {
                //露娜：每次都是我收拾残局欸？！这次要去你们俩去吧，人家才不去呢！
                luna.SetChat(ChatSettingConfig, 10, 20);

                if (luna.CurrentDialogFinished())
                    chatRoom.chatTurn++;
            }
            else if (turn == 4)
            {
                //桑尼：呜哇！偷东西的时候你的能力超重要的好吗？
                sunny.SetChat(ChatSettingConfig, 14, 20);

                if (sunny.CurrentDialogFinished())
                    chatRoom.chatTurn++;
            }
            else if (turn == 5)
            {
                //斯塔：好啦好啦，那要不咱们去偷那个红白巫女的赛钱箱吧？
                star.SetChat(ChatSettingConfig, 7, 20);

                if (star.CurrentDialogFinished())
                    chatRoom.chatTurn++;
            }
            else if (turn == 6)
            {
                //桑尼&露娜：不可以！！！
                sunny.SetChat(ChatSettingConfig, 15, 20);
                luna.SetChat(ChatSettingConfig, 11, 20);

                if (luna.CurrentDialogFinished())
                    chatRoom.chatTurn++;
            }
            else
            {
                chatRoom.CloseChatRoom();
            }
        }
        private void GenDust()
        {
            if (RainWet)
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
                Dust.NewDustPerfect(Projectile.Center
                + new Vector2(Main.rand.NextFloat(-30f, -10f) * Projectile.spriteDirection, Main.rand.NextFloat(-5f, 25f)), dustID
                , new Vector2(Main.rand.NextFloat(-0.2f, 0.2f), Main.rand.NextFloat(0.8f, 1.2f)), 100, default
                , Main.rand.NextFloat(1f, 2f)).noGravity = true;
            }
            if (Main.rand.NextBool(10))
            {
                Dust.NewDustPerfect(Projectile.Center
                    + new Vector2(Main.rand.NextFloat(20f, 22f) * Projectile.spriteDirection, Main.rand.NextFloat(-5f, 25f)), dustID
                    , new Vector2(Main.rand.NextFloat(-0.2f, 0.2f), Main.rand.NextFloat(0.8f, 1.2f)), 100, default
                    , Main.rand.NextFloat(1f, 2f)).noGravity = true;
            }
        }
        private void ControlMovement(Player player)
        {
            Projectile.tileCollide = false;
            Projectile.rotation = Projectile.velocity.X * 0.02f;

            ChangeDir(true, 200);

            Vector2 point = new Vector2(60 * player.direction, -40 + player.gfxOffY);
            if (player.HasBuff<TheThreeFairiesBuff>())
            {
                point = new Vector2(60 * player.direction, -70 + player.gfxOffY);
                point += new Vector2(0, -40).RotatedBy(MathHelper.ToRadians(360 / 3 * 0) + Main.GlobalTimeWrappedHourly);
            }
            MoveToPoint(point, 8.5f);
        }
        public override void AI()
        {
            Lighting.AddLight(Projectile.Center, 2.40f, 1.96f, 0.84f);
            Player player = Owner;
            Projectile.SetPetActive(player, BuffType<SunnyBuff>());
            Projectile.SetPetActive(player, BuffType<TheThreeFairiesBuff>());

            UpdateTalking();
            ControlMovement(player);
            GenDust();

            if (OwnerIsMyPlayer)
            {
                if (mainTimer % 270 == 0)
                {
                    if (PetState == 3)
                    {
                        PetState = 4;
                    }
                    else if (PetState == 0)
                    {
                        PetState = 1;
                    }
                    Projectile.netUpdate = true;
                }
                if (RainWet && PetState < 3)
                {
                    PetState = 3;
                    Projectile.netUpdate = true;
                }
                else if (mainTimer >= 600 && mainTimer < 3600 && PetState == 0)
                {
                    if (mainTimer % 600 == 0 && Main.rand.NextBool(6) && extraAI[0] <= 0 && UnderSunShine && chatTimeLeft <= 0)
                    {
                        PetState = 2;
                        Projectile.netUpdate = true;
                    }
                }
            }
            if (PetState == 0)
            {
                if (extraAI[0] >= 1)
                {
                    extraAI[0]--;
                }
            }
            else if (PetState == 1)
            {
                Blink();
            }
            else if (PetState == 2)
            {
                Happy();
            }
            else if (PetState == 3 || PetState == 4)
            {
                Projectile.frame = 4;
                if (PetState == 4)
                    Blink(true);
                if (!RainWet)
                {
                    PetState = 0;
                }
            }
            UpdateExtraPos();
        }
    }
}


