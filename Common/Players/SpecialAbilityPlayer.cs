using Terraria;
using TouhouPets.Content.Buffs.PetBuffs;

namespace TouhouPets
{
    public class SpecialAbilityPlayer : ModPlayer
    {
        public bool MurasasCurse => Player.HasBuff<MurasaBuff>() && Main.remixWorld;

        public bool treasureShine;
        public override bool IsLoadingEnabled(Mod mod)
        {
            return GetInstance<PetAbilitiesConfig>().SpecialAbility;
        }
        private void CommonResetUpdate()
        {
            treasureShine = false;
        }
        public override void ResetEffects()
        {
            CommonResetUpdate();
        }
        public override void UpdateDead()
        {
            CommonResetUpdate();
        }
        public override void ModifyLuck(ref float luck)
        {
            if (Player.HasBuff(BuffType<HinaBuff>()))
            {
                if (luck < 0)
                {
                    luck = 0;
                }
            }
            if (Player.HasBuff(BuffType<TenshiBuff>()))
            {
                luck += 0.5f;
            }
        }
        public override void PostUpdateBuffs()
        {
            if (treasureShine)
            {
                Player.spelunkerTimer++;
                if (Player.spelunkerTimer >= 10)
                {
                    Player.spelunkerTimer = 0;
                    Main.instance.SpelunkerProjectileHelper.AddSpotToCheck(Player.Center);
                }
            }
        }
        public override void UpdateEquips()
        {
            if (MurasasCurse)
            {
                Player.waterWalk = false;

                if (Player.breathCD > 0 && Player.breath > 0)
                    Player.breath = 0;
            }
        }
    }
}
