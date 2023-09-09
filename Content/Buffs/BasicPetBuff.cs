using Terraria;

namespace TouhouPets.Content.Buffs
{
    public abstract class BasicPetBuff : ModBuff
    {
        public virtual int PetType => -1;
        public virtual bool LightPet => false;
        public override void SetStaticDefaults()
        {
            Main.buffNoTimeDisplay[Type] = true;
            Main.vanityPet[Type] = !LightPet;
            Main.lightPet[Type] = LightPet;
        }
        public override void Update(Player player, ref int buffIndex)
        {
            player.buffTime[buffIndex] = 18000;
            bool flag = PetType != -1 && player.ownedProjectileCounts[PetType] <= 0;
            if (flag)
            {
                if (player.whoAmI == Main.myPlayer)
                    Projectile.NewProjectile(player.GetSource_Buff(buffIndex), player.position.X + player.width / 2, player.position.Y + player.height / 2, 0f, 0f, PetType, 0, 0f, player.whoAmI, 0f, 0f);
            }
        }
    }
}
