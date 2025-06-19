using Terraria;
using Terraria.ID;
using Terraria.Localization;
using Terraria.Utilities;
using Terraria.GameContent.Bestiary;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using TouhouPets.Content.Items.PetItems;
using Terraria.DataStructures;

namespace TouhouPets.Content.NPCs
{
    public class YukariPortal : ModNPC
    {
        public override void SetStaticDefaults()
        {
            Main.npcFrameCount[Type] = 15;

            NPCID.Sets.ActsLikeTownNPC[Type] = true;
            NPCID.Sets.SpawnsWithCustomName[Type] = false;

            NPCID.Sets.TownNPCBestiaryPriority.Add(Type);
            NPCID.Sets.NPCBestiaryDrawModifiers drawModifiers = new();
            NPCID.Sets.NPCBestiaryDrawOffset.Add(Type, drawModifiers);
        }
        public override void SetBestiary(BestiaryDatabase database, BestiaryEntry bestiaryEntry)
        {
            bestiaryEntry.Info.AddRange(new IBestiaryInfoElement[] {
                BestiaryDatabaseNPCsPopulator.CommonTags.SpawnConditions.Times.NightTime,
                BestiaryDatabaseNPCsPopulator.CommonTags.SpawnConditions.Biomes.Surface,
                new FlavorTextBestiaryInfoElement(Language.GetTextValue("Mods.TouhouPets.Bestiary.YukariPortal"))
            });
        }
        public override void SetDefaults()
        {
            NPC.friendly = true;
            NPC.width = 16;
            NPC.height = 40;
            NPC.aiStyle = 7;
            NPC.damage = 0;
            NPC.defense = 0;
            NPC.lifeMax = 1;
            NPC.HitSound = SoundID.NPCHit1;
            NPC.DeathSound = SoundID.NPCDeath1;
            NPC.knockBackResist = 0f;
            NPC.rarity = 1;
            NPC.dontTakeDamage = true;
            NPC.dontTakeDamageFromHostiles = true;
            NPC.immortal = true;
            NPC.lavaImmune = true;
            NPC.noGravity = true;
        }
        public override void AI()
        {
            NPC.velocity *= 0f;
            if (Main.dayTime)
            {
                NPC.ai[3] = -1;
            }
        }
        public override float SpawnChance(NPCSpawnInfo spawnInfo)
        {
            if (!GetInstance<PetObtainConfig>().AllowGapToSpawn)
            {
                return 0f;
            }
            bool zoneOverworld = Main.remixWorld ?
                spawnInfo.Player.ZoneUnderworldHeight : spawnInfo.Player.ZoneOverworldHeight;
            if (!Main.dayTime && zoneOverworld && !NPC.AnyNPCs(Type)
                && !Main.bloodMoon && !Main.eclipse)
            {
                return 0.09f;
            }
            return 0f;
        }
        public override void OnSpawn(IEntitySource source)
        {
            NPC.position.Y -= 30;
        }
        public override bool CanGoToStatue(bool toKingStatue)
        {
            return false;
        }
        public override bool CanChat()
        {
            return true;
        }
        public override string GetChat()
        {
            InitializeTopic();
            Main.LocalPlayer.currentShoppingSettings.HappinessReport = "";

            WeightedRandom<string> chat = new();
            {
                chat.Add(ModUtils.GetChatTextValue("Portal", "1"));
                chat.Add(ModUtils.GetChatTextValue("Portal", "2"));
                chat.Add(ModUtils.GetChatTextValue("Portal", "3"));
                chat.Add(ModUtils.GetChatTextValue("Portal", "4"));
            }
            return chat;
        }
        private void InitializeTopic()
        {
            buttonText = Language.GetTextValue("Mods.TouhouPets.PetShop");
            topic = 0;
        }
        string buttonText;
        int topic;
        public override void SetChatButtons(ref string button, ref string button2)
        {
            button = buttonText;
            button2 = Language.GetTextValue("Mods.TouhouPets.SwitchTopic");
        }
        public override void OnChatButtonClicked(bool firstButton, ref string shopName)
        {
            if (firstButton)
            {
                switch (topic)
                {
                    case 1:
                        shopName = "Shop2";
                        break;
                    case 2:
                        Chat_ConsumpCount();
                        break;
                    case 3:
                        Chat_QQGroup();
                        break;
                    case 4:
                        Chat_GoodsShop();
                        break;
                    default:
                        shopName = "Shop";
                        break;
                }
                return;
            }
            if (topic == 0)
            {
                buttonText = Language.GetTextValue("Mods.TouhouPets.LightPetShop");
                topic++;
            }
            else if (topic == 1)
            {
                buttonText = Language.GetTextValue("Mods.TouhouPets.YukariWealthCount");
                topic++;               
            }
            else if (topic == 2)
            {
                if (ModUtils.IsSpecificLanguage(GameCulture.CultureName.Chinese))
                {
                    buttonText = Language.GetTextValue("Mods.TouhouPets.QQGroup");
                    topic++;
                }
                else
                {
                    buttonText = Language.GetTextValue("Mods.TouhouPets.PetShop");
                    topic = 0;
                }               
            }
            else if (topic == 3)
            {
                buttonText = Language.GetTextValue("Mods.TouhouPets.GoodsShop");
                topic++;
            }
            else if (topic == 4)
            {
                buttonText = Language.GetTextValue("Mods.TouhouPets.PetShop");
                topic = 0;
            }
        }
        private static void Chat_ConsumpCount()
        {
            TouhouPetPlayer mp = Main.LocalPlayer.GetModPlayer<TouhouPetPlayer>();
            if (mp.totalPurchaseValueCount <= 0)
                Main.npcChatText = ModUtils.GetChatTextValue("Portal", "7");
            else
                Main.npcChatText = ModUtils.GetChatTextValue("Portal", "6", ModUtils.CoinValue(mp.totalPurchaseValueCount));
        }
        private static void Chat_QQGroup()
        {
            Main.npcChatText = ModUtils.GetChatTextValue("Portal", "8");
        }
        private static void Chat_GoodsShop()
        {
            Main.npcChatText = ModUtils.GetChatTextValue("Portal", "9");
        }
        private void AddShopItem()
        {
            NPCShop shop = new(Type);
            shop.Add(ItemType<DaiyouseiBomb>());
            shop.Add(ItemType<KoakumaPower>());
            shop.Add(ItemType<KogasaUmbrella>());
            shop.Add(ItemType<RumiaRibbon>());
            shop.Add(ItemType<KaguyaBranch>());
            shop.Add(ItemType<KeineLeaf>());
            shop.Add(ItemType<RemiliaRedTea>());
            shop.Add(ItemType<SakuyaWatch>());
            shop.Add(ItemType<MystiaFeather>());
            shop.Add(ItemType<HinaDoll>());
            shop.Add(ItemType<AliceDoll>());
            shop.Add(ItemType<RinSkull>());
            shop.Add(ItemType<RaikoDrum>());
            shop.Add(ItemType<AyaCamera>());
            shop.Add(ItemType<KoishiTelephone>());            
            shop.Add(ItemType<YukaSunflower>());
            shop.Add(ItemType<YuyukoFan>());
            shop.Add(ItemType<SekibankiBow>());
            shop.Add(ItemType<TenshiKeyStone>());
            shop.Add(ItemType<ReimuYinyangOrb>());
            shop.Add(ItemType<HecatiaPlanet>());
            shop.Add(ItemType<ReisenGun>());
            shop.Add(ItemType<MinorikoSweetPotato>());
            shop.Add(ItemType<MurasaBailer>());
            shop.Add(ItemType<SuikaGourd>());
            shop.Add(ItemType<RukotoRemote>());
            shop.Add(ItemType<TewiCarrot>());
            shop.Add(ItemType<MomoyoPickaxe>());
            shop.Add(ItemType<LettyGlobe>());
            shop.Register();

            shop = new(Type, "Shop2");
            shop.Add(ItemType<LilyOneUp>());
            shop.Add(ItemType<FlandrePudding>());
            shop.Add(ItemType<MeirinPanda>());
            shop.Add(ItemType<WriggleInAJar>());
            shop.Add(ItemType<WakasagihimeFishingRod>());
            shop.Add(ItemType<CirnoIceShard>());
            shop.Add(ItemType<UtsuhoEye>());
            shop.Add(ItemType<MokuMatch>());
            shop.Add(ItemType<PatchouliMoon>());
            shop.Add(ItemType<SatoriSlippers>());
            shop.Add(ItemType<NitoriCucumber>());
            shop.Add(ItemType<YoumuKatana>());
            shop.Add(ItemType<IkuOarfish>());
            shop.Add(ItemType<MarisaHakkero>());
            shop.Add(ItemType<SanaeCoin>());
            shop.Add(ItemType<JunkoMooncake>());
            shop.Add(ItemType<SunnyMilk>());
            shop.Add(ItemType<LunaMoon>());
            shop.Add(ItemType<StarSapphire>());
            shop.Add(ItemType<SizuhaBrush>());
            shop.Add(ItemType<EirinBow>());
            shop.Add(ItemType<ShinkiHeart>());
            shop.Add(ItemType<KokoroMask>());
            shop.Add(ItemType<DoremyPillow>());
            shop.Add(ItemType<LunasaViolin>());
            shop.Add(ItemType<MerlinTrumpet>());
            shop.Add(ItemType<LyricaKeyboard>());
            shop.Register();
        }
        public override void AddShops()
        {
            AddShopItem();
        }
        public override void ModifyActiveShop(string shopName, Item[] items)
        {
            Main.LocalPlayer.currentShoppingSettings.PriceAdjustment = 1;
        }
        public override void FindFrame(int frameHeight)
        {
            NPC.frame.Width = 32;
            int frame = NPC.frame.Y / frameHeight;
            NPC.frameCounter++;
            if (NPC.frameCounter >= 5)
            {
                NPC.frameCounter = 0;
                frame++;
            }
            if (NPC.ai[3] < 0)
            {
                if (frame > 37)
                {
                    NPC.active = false;
                    NPC.netUpdate = true;
                }
            }
            else
            {
                if (frame > 29)
                {
                    frame = 0;
                }
            }
            NPC.frame.Y = frame * frameHeight;
        }
        public override bool PreDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
        {
            Color purple = NPC.GetAlpha(Color.Purple * 0.5f);
            if (NPC.IsABestiaryIconDummy)
            {
                purple = Color.Purple * 0.5f;
            }
            for (int i = 0; i < 4; i++)
            {
                DrawPortal(spriteBatch, screenPos, purple, new Vector2(0, -2 * Main.essScale).RotatedBy(MathHelper.ToRadians(90 * i)));
            }
            DrawPortal(spriteBatch, screenPos, NPC.GetAlpha(Color.White * 0.9f * Main.essScale));
            DrawPortal(spriteBatch, screenPos, drawColor, default, default, AltVanillaFunction.GetExtraTexture("YukariPortal_Cover"));
            return false;
        }
        private void DrawPortal(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor, Vector2 pos = default, float scale = default, Texture2D tex = null)
        {
            Texture2D npcTex = tex ?? AltVanillaFunction.NPCTexture(Type);
            Vector2 drawPos = NPC.Center + new Vector2(0, 6 * Main.essScale) - screenPos + (pos == default ? Vector2.Zero : pos);
            int frameCount = Main.npcFrameCount[Type];
            int frameY = NPC.frame.Y / NPC.frame.Height;
            Rectangle npcRect = npcTex.Frame(3, 15, frameY / frameCount, frameY % frameCount);
            Vector2 npcOrig = npcRect.Size() / 2;
            spriteBatch.Draw(npcTex, drawPos + pos, new Rectangle?(npcRect), drawColor, NPC.rotation, npcOrig, NPC.scale * (scale == default ? 1 : scale), SpriteEffects.None, 0);
        }
    }
}
