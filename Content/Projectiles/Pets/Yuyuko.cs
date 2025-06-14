using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Linq;
using System.Collections.Generic;
using System.IO;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader.IO;
using Terraria.Utilities;
using TouhouPets.Content.Buffs.PetBuffs;
using Terraria.Localization;

namespace TouhouPets.Content.Projectiles.Pets
{
    public class Yuyuko : BasicTouhouPet
    {
        private enum States
        {
            Idle,
            Blink,
            SwingFan,
            AfterSwingFan,
            BeforeEatting,
            Eatting,
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
        private bool IsIdleState => PetState <= 1;
        private bool IsEattingState => PetState >= (int)States.BeforeEatting && PetState <= (int)States.Eatting;

        private int hatFrame, hatFrameCounter;
        private int blinkFrame, blinkFrameCounter;
        private int clothFrame, clothFrameCounter;
        private int extraAdjX, extraAdjY;
        private Item food = new();
        private List<Item> foodList = [];
        private int hungerPoint;
        private bool feeded;

        private DrawPetConfig drawConfig = new(2);
        private readonly Texture2D clothTex = AltVanillaFunction.GetExtraTexture("Yuyuko_Cloth");
        public override void SetStaticDefaults()
        {
            Main.projFrames[Type] = 20;
            Main.projPet[Type] = true;

            ProjectileID.Sets.CharacterPreviewAnimations[Type] =
                ProjectileID.Sets.SimpleLoop(0, 1)
                .WhenSelected(11, 5, 12);
        }
        public override TouhouPetID UniqueID => TouhouPetID.Yuyuko;
        public override bool OnMouseHover(ref bool dontInvis)
        {
            Item food = Owner.inventory[Owner.selectedItem];
            if (food.stack > 0 && ItemID.Sets.IsFood[food.type])
            {
                if (!IsEattingState && GetInstance<PetAbilitiesConfig>().SpecialAbility_Yuyuko)
                {
                    Owner.cursorItemIconEnabled = true;
                    Owner.cursorItemIconText = Language.GetTextValue($"Mods.TouhouPets.FeedYuyuko");
                    dontInvis = true;
                    return true;
                }
            }
            dontInvis = IsEattingState;
            return false;
        }
        public override void OnMouseClick(bool leftMouse, bool rightMouse)
        {
            if (!rightMouse)
                return;

            Item selectedItem = Owner.inventory[Owner.selectedItem];
            if (selectedItem.type == ItemID.JojaCola)//请不要喂垃圾
            {
                Projectile.SetChat(ChatSettingConfig, 26, 60);
                return;
            }

            food = new Item(selectedItem.type);
            Main.instance.LoadItem(food.type);
            if (selectedItem.consumable)
            {
                selectedItem.stack--;
                if (selectedItem.stack < 0)
                {
                    selectedItem.TurnToAir();
                }
            }

            feeded = true;
            hungerPoint += 10800;
            UpdateEattingText(food);
        }
        public override bool DrawPetSelf(ref Color lightColor)
        {
            DrawPetConfig config = drawConfig with
            {
                ShouldUseEntitySpriteDraw = true,
            };

            Projectile.DrawPet(Projectile.frame, lightColor, drawConfig);

            Projectile.DrawPet(hatFrame, lightColor, config, 1);
            Projectile.ResetDrawStateForPet();

            if (CurrentState == States.Blink)
                Projectile.DrawPet(blinkFrame, lightColor, drawConfig, 1);

            Projectile.DrawPet(Projectile.frame, lightColor,
                config with
                {
                    AltTexture = clothTex,
                });
            Projectile.DrawPet(clothFrame, lightColor, config, 1);
            Projectile.ResetDrawStateForPet();

            if (!Main.gameMenu)//避免有魂灵Mod冲突
            {
                if (Projectile.frame >= 2 && Projectile.frame <= 4)
                {
                    DrawFood(lightColor);
                }
            }
            return false;
        }
        private void DrawFood(Color lightColor)
        {
            if (food.IsAir)
                return;

            Texture2D t = AltVanillaFunction.ItemTexture(food.type);
            Vector2 pos = Projectile.Center + new Vector2(10 * Projectile.spriteDirection, -3) + new Vector2(extraAdjX, extraAdjY) - Main.screenPosition + new Vector2(0, 7f * Main.essScale);
            int height = t.Height / 3;
            Rectangle rect = new Rectangle(0, height * (food.type == ItemID.Ale ? 2 : 1), t.Width, height);
            Vector2 orig = rect.Size() / 2;
            Color clr = Projectile.GetAlpha(lightColor);
            SpriteEffects effect = Projectile.spriteDirection == -1 ? SpriteEffects.None : SpriteEffects.FlipHorizontally;
            Main.spriteBatch.MyDraw(t, pos, rect, clr, Projectile.rotation, orig, 1f, effect, 0f);
        }
        public override ChatSettingConfig ChatSettingConfig => new ChatSettingConfig() with
        {
            TextColor = new Color(255, 112, 214),
        };
        public override void RegisterChat(ref string name, ref Vector2 indexRange)
        {
            name = "Yuyuko";
            indexRange = new Vector2(1, 27);
        }
        public override void SetRegularDialog(ref int timePerDialog, ref int chance, ref bool whenShouldStop)
        {
            timePerDialog = 970;//970
            chance = 9;//9
            whenShouldStop = !IsIdleState;
        }
        public override WeightedRandom<string> RegularDialogText()
        {
            WeightedRandom<string> chat = new ();
            {
                chat.Add(ChatDictionary[1]);
                chat.Add(ChatDictionary[2]);
                chat.Add(ChatDictionary[3]);
                chat.Add(ChatDictionary[4]);
                if (FindPet(ProjectileType<Youmu>()))
                {
                    chat.Add(ChatDictionary[11]);
                    chat.Add(ChatDictionary[12]);
                }
            }
            return chat;
        }
        public override void VisualEffectForPreview()
        {
            UpdateHatFrame();
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
            TouhouPetID yuyuko = TouhouPetID.Yuyuko;
            TouhouPetID youmu = TouhouPetID.Youmu;

            List<ChatRoomInfo> list =
            [
                new ChatRoomInfo(yuyuko, 12, -1), //幽幽子：妖梦酱有为未来做过打算嘛？
                new ChatRoomInfo(youmu, 6, 0),//妖梦：欸？只要伺候幽幽子大人就行了吧...
                new ChatRoomInfo(yuyuko, 13, 1), //幽幽子：妖梦酱果然还是太单纯了呀...以后再聊吧。
                new ChatRoomInfo(youmu, 10, 2),//妖梦：只要能待在幽幽子大人身旁，我就很知足了。
            ];

            return list;
        }
        private static List<ChatRoomInfo> Chatting2()
        {
            TouhouPetID yuyuko = TouhouPetID.Yuyuko;
            TouhouPetID youmu = TouhouPetID.Youmu;

            List<ChatRoomInfo> list =
            [
                new ChatRoomInfo(yuyuko, 11, -1), //幽幽子：妖梦酱，今天晚上吃什么？
                new ChatRoomInfo(youmu, 8, 0),//妖梦：幽幽子大人您五分钟之前刚吃过饭。
            ];

            return list;
        }
        private static List<ChatRoomInfo> Chatting3()
        {
            TouhouPetID yuyuko = TouhouPetID.Yuyuko;
            TouhouPetID youmu = TouhouPetID.Youmu;

            List<ChatRoomInfo> list =
            [
                new ChatRoomInfo(yuyuko, 1, -1), //幽幽子：生亦好、死也罢，不过都是场轮回。可惜与我无关...
                new ChatRoomInfo(youmu, 9, 0),//妖梦：可是幽幽子大人您已经死了啊？也不会复生。
            ];

            return list;
        }
        public override void AI()
        {
            Projectile.SetPetActive(Owner, BuffType<YuyukoBuff>());

            ControlMovement();

            if (ShouldExtraVFXActive)
            {
                SpawnButterfly();
            }

            if (food.IsAir && IsEattingState)
            {
                Projectile.frame = 0;
                CurrentState = States.Idle;
                return;
            }

            switch (CurrentState)
            {
                case States.Blink:
                    Blink();
                    break;

                case States.SwingFan:
                    shouldNotTalking = true;
                    SwingFan();
                    break;

                case States.AfterSwingFan:
                    shouldNotTalking = true;
                    AfterSwingFan();
                    break;

                case States.BeforeEatting:
                    shouldNotTalking = true;
                    BeforeEatting();
                    break;

                case States.Eatting:
                    shouldNotTalking = true;
                    Eatting();
                    break;

                default:
                    Idle();
                    break;
            }

            if (IsIdleState && ActionCD > 0)
            {
                ActionCD--;
            }
            if (OwnerIsMyPlayer && hungerPoint > 0)
            {
                hungerPoint--;
            }

            if (feeded)
            {
                CurrentState = States.BeforeEatting;
            }
            feeded = false;

            UpdatePositionOffset();
        }
        public override void ReceiveExtraAI(BinaryReader reader)
        {
            base.ReceiveExtraAI(reader);
            ItemIO.Receive(food, reader);
        }
        public override void SendExtraAI(BinaryWriter writer)
        {
            base.SendExtraAI(writer);
            ItemIO.Send(food, writer);
        }
        private void ControlMovement()
        {
            Projectile.tileCollide = false;
            Projectile.rotation = Projectile.velocity.X * 0.003f;

            ChangeDir();

            Vector2 point = new Vector2(-40 * Owner.direction, -50 + Owner.gfxOffY);
            if (Owner.ownedProjectileCounts[ProjectileType<Youmu>()] > 0)
            {
                point = new Vector2(40 * Owner.direction, -50 + Owner.gfxOffY);
            }
            MoveToPoint(point, 16f);
        }
        private void SpawnButterfly()
        {
            if (!OwnerIsMyPlayer)
                return;

            if (mainTimer % 20 == 0 && mouseOpacity >= 1f)
            {
                Projectile.NewProjectile(Projectile.GetSource_FromThis(), Projectile.Center + new Vector2(Main.rand.Next(-40, 40), Main.rand.Next(-20, 50))
                            , new Vector2(0, Main.rand.NextFloat(-0.3f, -0.7f)), ProjectileType<YuyukoButterfly>(), 0, 0, Main.myPlayer);
            }
        }
        private void UpdatePositionOffset()
        {
            extraAdjX = 0;
            extraAdjY = 0;
            switch (Projectile.frame)
            {
                case 2:
                    extraAdjY = 2;
                    break;
                case 4:
                    extraAdjX = -4 * Projectile.spriteDirection;
                    extraAdjY = -4;
                    break;
                default:
                    break;
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
                    if (Main.rand.NextBool(6))
                    {
                        RandomCount = Main.rand.Next(10, 30);
                        CurrentState = States.SwingFan;
                    }
                    else if (Main.rand.NextBool(3) && GetInstance<PetAbilitiesConfig>().SpecialAbility_Yuyuko
                        && hungerPoint <= 0)
                    {
                        FoodSelect(Owner);
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
        private void SwingFan()
        {
            if (Projectile.frame < 7)
                Projectile.frame = 7;

            if (++Projectile.frameCounter > 12)
            {
                Projectile.frameCounter = 0;
                Projectile.frame++;
            }
            if (Projectile.frame > 15)
            {
                Projectile.frame = 11;
                Timer++;
            }
            if (OwnerIsMyPlayer && Timer > RandomCount)
            {
                Timer = 0;
                CurrentState = States.AfterSwingFan;
            }
        }
        private void AfterSwingFan()
        {
            if (++Projectile.frameCounter > 6)
            {
                Projectile.frameCounter = 0;
                Projectile.frame++;
            }
            if (Projectile.frame > 19)
            {
                Projectile.frame = 0;
                if (OwnerIsMyPlayer)
                {
                    ActionCD = 600;
                    CurrentState = States.Idle;
                }
            }
        }
        private void BeforeEatting()
        {
            if (++Projectile.frameCounter > 12)
            {
                Projectile.frameCounter = 0;
                Projectile.frame++;
            }
            if (Projectile.frame >= 3)
            {
                Projectile.frame = 3;
                Timer++;
            }
            if (OwnerIsMyPlayer && Timer > 180)
            {
                Timer = 0;
                CurrentState = States.Eatting;
            }
        }
        private void Eatting()
        {
            if (++Projectile.frameCounter > 12)
            {
                Projectile.frameCounter = 0;
                Projectile.frame++;
            }
            if (Projectile.frame == 5)
            {
                EmitFoodParticles(food);
                if (Projectile.frameCounter == 1 && food.UseSound != null)
                    AltVanillaFunction.PlaySound((SoundStyle)food.UseSound, Projectile.Center);
            }
            if (Projectile.frame > 6)
            {
                Projectile.frame = 0;
                if (OwnerIsMyPlayer)
                {
                    CurrentState = States.Idle;
                }
            }
        }
        private void EmitFoodParticles(Item sItem)
        {
            Color[] array = ItemID.Sets.FoodParticleColors[sItem.type];
            if (array != null && array.Length != 0 && Main.rand.NextBool(2))
            {
                Vector2? mouthPosition = Projectile.Center + new Vector2(10 * Projectile.spriteDirection, -3)
                    + new Vector2(extraAdjX, extraAdjY) + new Vector2(0, 7f * Main.essScale);
                if (mouthPosition.HasValue)
                {
                    Vector2 vector = mouthPosition.Value + Main.rand.NextVector2Square(-4f, 4f);
                    Vector2 spinningpoint = new Vector2(Projectile.spriteDirection, 0);
                    Dust.NewDustPerfect(vector, 284, 1.3f * spinningpoint.RotatedBy((float)Math.PI / 5f * Main.rand.NextFloatDirection()), 0, array[Main.rand.Next(array.Length)], 0.8f + 0.2f * Main.rand.NextFloat()).fadeIn = 0f;
                }
            }

            Color[] array2 = ItemID.Sets.DrinkParticleColors[sItem.type];
            if (array2 != null && array2.Length != 0)
            {
                Vector2? mouthPosition = Projectile.Center + new Vector2(10 * Projectile.spriteDirection, -3)
                    + new Vector2(extraAdjX, extraAdjY) + new Vector2(0, 7f * Main.essScale);
                if (mouthPosition.HasValue)
                {
                    Vector2 vector = mouthPosition.Value + Main.rand.NextVector2Square(-4f, 4f);
                    Vector2 spinningpoint = new Vector2(Projectile.spriteDirection * 0.1f, 0);
                    Dust.NewDustPerfect(vector, 284, 1.3f * spinningpoint.RotatedBy(-(float)Math.PI / 5f * Main.rand.NextFloatDirection()), 0, array2[Main.rand.Next(array2.Length)] * 0.7f, 0.8f + 0.2f * Main.rand.NextFloat()).fadeIn = 0f;
                }
            }
        }
        private void UpdateHatFrame()
        {
            if (hatFrame < 3)
            {
                hatFrame = 3;
            }
            int count = 7;
            if (++hatFrameCounter > count)
            {
                hatFrameCounter = 0;
                hatFrame++;
            }
            if (hatFrame > 6)
            {
                hatFrame = 3;
            }
        }
        private void UpdateClothFrame()
        {
            if (clothFrame < 7)
            {
                clothFrame = 7;
            }
            int count = 5;
            if (++clothFrameCounter > count)
            {
                clothFrameCounter = 0;
                clothFrame++;
            }
            if (clothFrame > 10)
            {
                clothFrame = 7;
            }
        }
        private void FoodListUpdate(Player player)
        {
            foodList.Clear();
            for (int j = 0; j < player.inventory.Length; j++)
            {
                Item fd = player.inventory[j];
                if (fd != null && !fd.IsAir && ItemID.Sets.IsFood[fd.type]
                    && fd != player.inventory[player.selectedItem]
                    && !fd.favorited && fd.type != ItemID.JojaCola)
                {
                    foodList.Add(fd);
                }
                if (j < player.bank4.item.Length)
                {
                    if (player.IsVoidVaultEnabled)
                    {
                        Item fd2 = player.bank4.item[j];
                        if (fd2 != null && !fd2.IsAir && ItemID.Sets.IsFood[fd2.type]
                            && !fd2.favorited && fd.type != ItemID.JojaCola)
                        {
                            foodList.Add(fd2);
                        }
                    }
                }
            }
        }
        private void FoodSelect(Player player)
        {
            FoodListUpdate(player);

            if (foodList.Count > 0)
            {
                Item fd;
                if (ModLoader.TryGetMod("CyberNewYear", out Mod result) && result.TryFind("Dumpling", out ModItem dumpling)
                                && player.HasItemInInventoryOrOpenVoidBag(dumpling.Type))
                {
                    fd = foodList.First(food => food.type == dumpling.Type);
                }
                else
                    fd = Main.rand.NextFromCollection(foodList);

                food = new Item(fd.type);
                Main.instance.LoadItem(food.type);

                if (fd.consumable)
                {
                    fd.stack--;
                    if (fd.stack <= 0)
                    {
                        fd.TurnToAir(true);
                    }
                }
                UpdateEattingText(food);
                CurrentState = States.BeforeEatting;
            }
            else
            {
                UpdateRegularFoodText(false);
            }
        }
        private void UpdateEattingText(Item food)
        {
            Projectile.CloseCurrentDialog();
            switch (food.type)
            {
                case ItemID.Ale:
                case ItemID.Sake:
                    //人生得意须尽欢，莫使金樽空对月。干了！
                    Projectile.SetChat(14, 60);
                    break;

                case ItemID.GrubSoup:
                    //奇特的丛林美食，富含蛋白质！
                    Projectile.SetChat(15, 60);
                    break;

                case ItemID.Sashimi:
                    //是家乡的味道呢...但是冥界并没有海吧？
                    Projectile.SetChat(16, 60);
                    break;

                case ItemID.Burger:
                    //向传奇商业食物致敬！
                    Projectile.SetChat(17, 60);
                    break;

                case ItemID.Fries:
                    //没有番茄酱或者炸鱼的薯条是没有灵魂的...
                    Projectile.SetChat(18, 60);
                    break;

                case ItemID.GoldenDelight:
                    //谢谢你这么大方，请我吃这个！
                    Projectile.SetChat(19, 60);
                    break;

                case ItemID.ShuckedOyster:
                    //壳什么的一起吃掉就好啦！
                    Projectile.SetChat(20, 60);
                    break;

                case ItemID.Apple:
                    //一天一个苹果，医生...欸我需要医生吗？
                    Projectile.SetChat(21, 60);
                    break;

                case ItemID.Cherry:
                    //这不会爆炸，对吧？
                    Projectile.SetChat(22, 60);
                    break;

                case ItemID.Pizza:
                    //我已经把菠萝都藏起来了...
                    Projectile.SetChat(23, 60);
                    break;

                case ItemID.Escargot:
                    //能不能做成派呢？
                    Projectile.SetChat(24, 60);
                    break;

                case ItemID.ChickenNugget:
                    //没有碎骨更好吃！
                    Projectile.SetChat(25, 60);
                    break;

                default:
                    UpdateRegularFoodText(true);
                    break;
            };
        }
        private void UpdateRegularFoodText(bool hasFood)
        {
            if (chatTimeLeft <= 0)
            {
                if (hasFood)
                {
                    if (feeded)
                    {
                        Projectile.SetChat(27, 60);
                    }
                    else
                    {
                        Projectile.SetChat(Main.rand.Next(5, 8), 60);
                    }
                }
                else
                {
                    Projectile.SetChat(Main.rand.Next(8, 11), 60);
                }
            }
        }
    }
}


