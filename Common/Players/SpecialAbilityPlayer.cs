using Terraria;
using TouhouPets.Content.Buffs.PetBuffs;

namespace TouhouPets
{
    public class SpecialAbilityPlayer : ModPlayer
    {
        public bool MurasasCurse => Player.HasBuff<MurasaBuff>() && Main.remixWorld;

        public bool treasureShine;
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
            if (Player.HasBuff(BuffType<HinaBuff>())
                && GetInstance<PetAbilitiesConfig>().SpecialAbility_Hina)
            {
                if (luck < 0)
                {
                    luck = 0;
                }
            }
            if (Player.HasBuff(BuffType<TenshiBuff>())
                && GetInstance<PetAbilitiesConfig>().SpecialAbility_Tenshi)
            {
                luck += 0.5f;
            }

            if (Player.HasBuff(BuffType<TewiBuff>())
                && GetInstance<PetAbilitiesConfig>().SpecialAbility_Tewi)
            {
                luck += 0.3f;
            }
        }
        public override void PostUpdateBuffs()
        {
            if (treasureShine && GetInstance<PetAbilitiesConfig>().SpecialAbility_Star)
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
            if (MurasasCurse && GetInstance<PetAbilitiesConfig>().SpecialAbility_Murasa)
            {
                Player.waterWalk = false;

                if (Player.breathCD > 0 && Player.breath > 0)
                    Player.breath = 0;
            }
        }
    }
}
