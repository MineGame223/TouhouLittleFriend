using Terraria;
using Terraria.Localization;
using TouhouPets.Content.Projectiles.Pets;

namespace TouhouPets.Content.Buffs.PetBuffs
{
    public class KoakumaBuff : BasicPetBuff
    {
        public override int PetType => ProjectileType<Koakuma>();
        public override bool LightPet => false;
        public override void ModifyBuffText(ref string buffName, ref string tip, ref int rare)
        {
            TouhouPetPlayer lp = Main.LocalPlayer.GetModPlayer<TouhouPetPlayer>();
            tip = Language.GetTextValue("Mods.TouhouPets.Buffs.KoakumaBuff.Description", NumberToCNCharacter.GetNumberText(lp.koakumaNumber));
        }
        public override void OnSummonPet(Player player)
        {
            if (player.whoAmI == Main.myPlayer)
                player.GetModPlayer<TouhouPetPlayer>().koakumaNumber = Main.rand.Next(1, 301);
        }
    }
}
