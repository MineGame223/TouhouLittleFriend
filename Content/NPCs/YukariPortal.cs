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
            NPCID.Sets.NPCBestiaryDrawModifiers drawModifiers = new(0);
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
            Main.LocalPlayer.currentShoppingSettings.HappinessReport = "";

            WeightedRandom<string> chat = new();
            {
                chat.Add(ModUtils.GetChatText("Portal", "1"));
                chat.Add(ModUtils.GetChatText("Portal", "2"));
                chat.Add(ModUtils.GetChatText("Portal", "3"));
                chat.Add(ModUtils.GetChatText("Portal", "4"));
            }
            return chat;
        }
        public override void SetChatButtons(ref string button, ref string button2)
        {
            button = Language.GetTextValue("LegacyInterface.28");
        }
        public override void OnChatButtonClicked(bool firstButton, ref string shopName)
        {
            if (firstButton)
            {
                shopName = "Shop";
            }
        }        
        private void AddShopItem_Legacy()
        {
            NPCShop shop = new(Type);
            shop.Add(ItemType<DaiyouseiBomb>());
            shop.Add(ItemType<LilyOneUp>());
            shop.Add(ItemType<KoakumaPower>());
            shop.Add(ItemType<KogasaUmbrella>(), Condition.DownedEyeOfCthulhu);
            shop.Add(ItemType<RumiaRibbon>(), Condition.DownedEyeOfCthulhu);
            shop.Add(ItemType<RemiliaRedTea>(), Condition.DownedEowOrBoc);
            shop.Add(ItemType<FlandrePudding>(), Condition.DownedEowOrBoc);
            shop.Add(ItemType<MystiaFeather>(), Condition.DownedEowOrBoc);
            shop.Add(ItemType<WriggleInAJar>(), Condition.DownedQueenBee);
            shop.Add(ItemType<WakasagihimeFishingRod>(), Condition.DownedQueenBee);
            shop.Add(ItemType<HinaDoll>(), Condition.DownedSkeletron);
            shop.Add(ItemType<AliceDoll>(), Condition.DownedSkeletron);
            shop.Add(ItemType<CirnoIceShard>(), Condition.DownedDeerclops);
            shop.Add(ItemType<RinSkull>(), Condition.Hardmode);
            shop.Add(ItemType<UtsuhoEye>(), Condition.Hardmode);
            shop.Add(ItemType<AyaCamera>(), Condition.Hardmode);
            shop.Add(ItemType<PatchouliMoon>(), Condition.DownedQueenSlime);
            shop.Add(ItemType<SatoriSlippers>(), Condition.DownedTwins);
            shop.Add(ItemType<KoishiTelephone>(), Condition.DownedTwins);
            shop.Add(ItemType<NitoriCucumber>(), Condition.DownedSkeletronPrime);
            shop.Add(ItemType<YukaSunflower>(), Condition.DownedPlantera);
            shop.Add(ItemType<YuyukoFan>(), Condition.DownedPlantera);
            shop.Add(ItemType<YoumuKatana>(), Condition.DownedPlantera);
            shop.Add(ItemType<SekibankiBow>(), Condition.DownedGolem);
            shop.Add(ItemType<IkuOarfish>(), Condition.DownedDukeFishron);
            shop.Add(ItemType<TenshiKeyStone>(), Condition.DownedEmpressOfLight);
            shop.Add(ItemType<ReimuYinyangOrb>(), Condition.DownedCultist);
            shop.Add(ItemType<MarisaHakkero>(), Condition.DownedCultist);
            shop.Add(ItemType<SanaeCoin>(), Condition.DownedCultist);
            shop.Add(ItemType<JunkoMooncake>(), Condition.DownedMoonLord);
            shop.Add(ItemType<HecatiaPlanet>(), Condition.DownedMoonLord);
            shop.Register();
        }
        private void AddShopItem()
        {
            NPCShop shop = new(Type);
            shop.Add(ItemType<DaiyouseiBomb>());
            shop.Add(ItemType<LilyOneUp>());
            shop.Add(ItemType<KoakumaPower>());
            shop.Add(ItemType<KogasaUmbrella>());
            shop.Add(ItemType<RumiaRibbon>());
            shop.Add(ItemType<RemiliaRedTea>());
            shop.Add(ItemType<FlandrePudding>());
            shop.Add(ItemType<MystiaFeather>());
            shop.Add(ItemType<WriggleInAJar>());
            shop.Add(ItemType<WakasagihimeFishingRod>());
            shop.Add(ItemType<HinaDoll>());
            shop.Add(ItemType<AliceDoll>());
            shop.Add(ItemType<CirnoIceShard>());
            shop.Add(ItemType<RinSkull>());
            shop.Add(ItemType<UtsuhoEye>());
            shop.Add(ItemType<AyaCamera>());
            shop.Add(ItemType<PatchouliMoon>());
            shop.Add(ItemType<SatoriSlippers>());
            shop.Add(ItemType<KoishiTelephone>());
            shop.Add(ItemType<NitoriCucumber>());
            shop.Add(ItemType<YukaSunflower>());
            shop.Add(ItemType<YuyukoFan>());
            shop.Add(ItemType<YoumuKatana>());
            shop.Add(ItemType<SekibankiBow>());
            shop.Add(ItemType<IkuOarfish>());
            shop.Add(ItemType<TenshiKeyStone>());
            shop.Add(ItemType<ReimuYinyangOrb>());
            shop.Add(ItemType<MarisaHakkero>());
            shop.Add(ItemType<SanaeCoin>());
            shop.Add(ItemType<JunkoMooncake>());
            shop.Add(ItemType<HecatiaPlanet>());
            shop.Register();
        }
        public override void AddShops()
        {
            if (GetInstance<PetObtainConfig>().PetSoldAtAnyTime)
            {
                AddShopItem();
            }
            else
            {
                AddShopItem_Legacy();
            }
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
