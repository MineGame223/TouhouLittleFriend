using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;
using Terraria;
using Terraria.GameContent;
using Terraria.GameInput;
using Terraria.ID;
using Terraria.Localization;
using Terraria.UI;
using TouhouPets.Content.Projectiles.Pets;

namespace TouhouPets
{
    public class SolutionSpraySystem : ModSystem
    {
        private static int sprayMode;
        private static Projectile yuka;
        private static Item solution;
        private static readonly Dictionary<int, SprayInfo> sprayInfo = new()
        {
            { ItemID.GreenSolution, new SprayInfo(ProjectileID.PureSpray, MyDustId.GreenBubble) },
            { ItemID.PurpleSolution, new SprayInfo(ProjectileID.CorruptSpray, MyDustId.PinkBubble) },
            { ItemID.RedSolution, new SprayInfo(ProjectileID.CrimsonSpray, MyDustId.PinkYellowBubble) },
            { ItemID.BlueSolution, new SprayInfo(ProjectileID.HallowSpray, MyDustId.CyanBubble) },
            { ItemID.DarkBlueSolution, new SprayInfo(ProjectileID.MushroomSpray, MyDustId.BlueIce) },
            { ItemID.DirtSolution, new SprayInfo(ProjectileID.DirtSpray, MyDustId.BrownBubble) },
            { ItemID.SandSolution, new SprayInfo(ProjectileID.SandSpray, MyDustId.YellowBubble) },
            { ItemID.SnowSolution, new SprayInfo(ProjectileID.SnowSpray, MyDustId.WhiteBubble) },
        };

        public const int Phase_Spray_Mode1 = 3;
        public const int Phase_Spray_Mode2 = 4;
        public const int Phase_StopSpray = 5;
        public static Item Sprayer => new(ItemID.Clentaminator2);
        public static bool IsSpraying => PetState >= Phase_Spray_Mode1 && PetState <= Phase_Spray_Mode2;
        public static Item Solution { get => solution; set => solution = value; }
        private static float PetState
        {
            get
            {
                if (yuka == null)
                    return 0;

                return yuka.ai[1];
            }
            set
            {
                if (yuka == null)
                    return;

                yuka.ai[1] = (int)value;
            }
        }

        public override void PostUpdateProjectiles()
        {
            if (Main.netMode == NetmodeID.Server || !SpecialAbility_Yuka)
                return;

            if (sprayMode > 1 || sprayMode < 0)
            {
                sprayMode = 0;
            }
            yuka = null;
            foreach (Projectile p in Main.ActiveProjectiles)
            {
                if (p.owner == Main.myPlayer && p.type == ProjectileType<Yuka>())
                {
                    yuka = p;
                }
            }
        }
        public override void ModifyInterfaceLayers(List<GameInterfaceLayer> layers)
        {
            int emoteIndex = layers.FindIndex(layer => layer.Name.Equals("Vanilla: Emote Bubbles"));
            if (emoteIndex != -1)
            {
                layers.Insert(emoteIndex, new LegacyGameInterfaceLayer(
                    "TouhouPets : YukaRequest",
                    delegate
                    {
                        SetSpray();
                        return true;
                    },
                    InterfaceScaleType.Game)
                );
            }
        }
        public static SprayInfo GetSprayInfo(int key)
        {
            if (!sprayInfo.TryGetValue(key, out SprayInfo value)
                && !TouhouPets.CrossModSprayInfo.TryGetValue(key, out value))
                return new SprayInfo(Solution.shoot, MyDustId.GreenBubble);

            return value;
        }
        private static void SetSpray()
        {
            if (yuka == null)
                return;

            if (yuka.isAPreviewDummy || PetState == Phase_StopSpray)
                return;

            if (!IsSpraying)
                solution = new Item();

            bool request = false;
            Player player = Main.player[yuka.owner];
            if (!PlayerInput.IgnoreMouseInterface)
            {
                Vector2 yukaPos = yuka.position - Main.screenPosition;
                Rectangle yukaRect = new((int)yukaPos.X, (int)yukaPos.Y, yuka.width, yuka.height);
                if (yukaRect.Contains(new Point(Main.mouseX, Main.mouseY)))
                {
                    player.mouseInterface = true;
                    request = true;
                    solution = player.ChooseAmmo(Sprayer);
                    if (yukaRect.Contains(new Point(Main.mouseX, Main.mouseY)) && solution != null && !solution.IsAir)
                    {
                        if (Main.mouseRight && Main.mouseRightRelease)
                        {
                            int targetMode = sprayMode == 0 ? Phase_Spray_Mode1 : Phase_Spray_Mode2;
                            if (PetState != targetMode)
                            {
                                PetState = targetMode;
                                yuka.netUpdate = true;
                                AltVanillaFunction.PlaySound(SoundID.MenuOpen, yuka.position);
                            }
                            else
                            {
                                PetState = Phase_StopSpray;
                                yuka.netUpdate = true;
                                AltVanillaFunction.PlaySound(SoundID.MenuClose, yuka.position);
                            }
                        }
                        if (Main.mouseLeft && Main.mouseLeftRelease)
                        {
                            if (!IsSpraying)
                            {
                                AltVanillaFunction.PlaySound(SoundID.MenuTick, yuka.position);
                                sprayMode++;
                            }
                        }
                    }
                }
            }
            DrawLeftSolution(request);
        }
        private static void DrawLeftSolution(bool drawRequestText)
        {
            if (solution == null || solution.IsAir)
            {
                return;
            }
            Main.instance.LoadItem(solution.type);
            Texture2D t = AltVanillaFunction.ItemTexture(solution.type);
            Vector2 pos = yuka.Center - Main.screenPosition + new Vector2(0, 7f * Main.essScale)
                + new Vector2(-20, -50);
            Rectangle rect = new(0, 0, t.Width, t.Height);
            Vector2 orig = rect.Size() / 2;
            SpriteEffects effect = SpriteEffects.None;
            if (solution.ammo != AmmoID.None)
            {
                Main.spriteBatch.MyDraw(t, pos, rect, yuka.GetAlpha(Color.White), 0, orig, 1.3f, effect, 0f);
                Utils.DrawBorderStringFourWay(Main.spriteBatch, FontAssets.MouseText.Value
                                , ": " + solution.stack.ToString(), pos.X + 20
                                , pos.Y - 4
                                , Color.White, Color.Black
                                , orig, 1f);
            }
            string modeText = IsSpraying ? "Stop" : "Request";
            if (drawRequestText)
            {
                if (!IsSpraying && PetState != Phase_StopSpray)
                    DrawSprayModeSign(sprayMode == 0 ? 9 : 8);

                Utils.DrawBorderStringFourWay(Main.spriteBatch, FontAssets.MouseText.Value
                                , Language.GetTextValue($"Mods.TouhouPets.Yuka{modeText}"), Main.MouseScreen.X + TextureAssets.Cursors[2].Width()
                                , Main.MouseScreen.Y + TextureAssets.Cursors[2].Height()
                                , Color.White, Color.Black
                                , orig, 1f);
            }
        }
        private static void DrawSprayModeSign(int frame)
        {
            Texture2D t = AltVanillaFunction.ProjectileTexture(yuka.type);
            int width = t.Width / 2;
            int height = t.Height / Main.projFrames[yuka.type];
            Vector2 pos = yuka.Center + new Vector2(-50, -50)
                - Main.screenPosition + new Vector2(0, 7f * Main.essScale);
            Rectangle rect = new(width, frame * height, 32, 32);
            Vector2 orig = rect.Size() / 2;
            SpriteEffects effect = SpriteEffects.None;
            Main.spriteBatch.MyDraw(t, pos, rect, yuka.GetAlpha(Color.White), 0, orig, yuka.scale, effect, 0f);
        }
    }
}
