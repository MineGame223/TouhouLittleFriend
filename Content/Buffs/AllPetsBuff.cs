using Terraria;
using TouhouPets.Content.Projectiles.Pets;

namespace TouhouPets.Content.Buffs
{
    public class AllPetsBuff : BasicPetBuff
    {
        public override bool LightPet => false;
        public override void Update(Player player, ref int buffIndex)
        {
            player.SpawnPetAndSetBuffTime(buffIndex, ProjectileType<Alice>());
            player.SpawnPetAndSetBuffTime(buffIndex, ProjectileType<Aya>());
            player.SpawnPetAndSetBuffTime(buffIndex, ProjectileType<Chen>());
            player.SpawnPetAndSetBuffTime(buffIndex, ProjectileType<Cirno>());
            player.SpawnPetAndSetBuffTime(buffIndex, ProjectileType<Daiyousei>());
            player.SpawnPetAndSetBuffTime(buffIndex, ProjectileType<Flandre>());
            player.SpawnPetAndSetBuffTime(buffIndex, ProjectileType<Hecatia>());
            player.SpawnPetAndSetBuffTime(buffIndex, ProjectileType<Hina>());
            player.SpawnPetAndSetBuffTime(buffIndex, ProjectileType<Iku>());
            player.SpawnPetAndSetBuffTime(buffIndex, ProjectileType<Junko>());
            player.SpawnPetAndSetBuffTime(buffIndex, ProjectileType<Kaguya>());
            player.SpawnPetAndSetBuffTime(buffIndex, ProjectileType<Keine>());
            player.SpawnPetAndSetBuffTime(buffIndex, ProjectileType<Koakuma>());
            player.SpawnPetAndSetBuffTime(buffIndex, ProjectileType<Kogasa>());
            player.SpawnPetAndSetBuffTime(buffIndex, ProjectileType<Lily>());
            player.SpawnPetAndSetBuffTime(buffIndex, ProjectileType<Luna>());
            player.SpawnPetAndSetBuffTime(buffIndex, ProjectileType<Marisa>());
            player.SpawnPetAndSetBuffTime(buffIndex, ProjectileType<Meirin>());
            player.SpawnPetAndSetBuffTime(buffIndex, ProjectileType<Minoriko>());
            player.SpawnPetAndSetBuffTime(buffIndex, ProjectileType<Moku>());
            player.SpawnPetAndSetBuffTime(buffIndex, ProjectileType<Mystia>());
            player.SpawnPetAndSetBuffTime(buffIndex, ProjectileType<Nitori>());
            player.SpawnPetAndSetBuffTime(buffIndex, ProjectileType<Patchouli>());
            player.SpawnPetAndSetBuffTime(buffIndex, ProjectileType<Piece>());
            player.SpawnPetAndSetBuffTime(buffIndex, ProjectileType<Raiko>());
            player.SpawnPetAndSetBuffTime(buffIndex, ProjectileType<Ran>());
            player.SpawnPetAndSetBuffTime(buffIndex, ProjectileType<Reimu>());
            player.SpawnPetAndSetBuffTime(buffIndex, ProjectileType<Reisen>());
            player.SpawnPetAndSetBuffTime(buffIndex, ProjectileType<Remilia>());
            player.SpawnPetAndSetBuffTime(buffIndex, ProjectileType<Rumia>());
            player.SpawnPetAndSetBuffTime(buffIndex, ProjectileType<Sakuya>());
            player.SpawnPetAndSetBuffTime(buffIndex, ProjectileType<Sanae>());
            player.SpawnPetAndSetBuffTime(buffIndex, ProjectileType<Sekibanki>());
            player.SpawnPetAndSetBuffTime(buffIndex, ProjectileType<Sizuha>());
            player.SpawnPetAndSetBuffTime(buffIndex, ProjectileType<Projectiles.Pets.Star>());
            player.SpawnPetAndSetBuffTime(buffIndex, ProjectileType<Sunny>());
            player.SpawnPetAndSetBuffTime(buffIndex, ProjectileType<Tenshi>());
            player.SpawnPetAndSetBuffTime(buffIndex, ProjectileType<Wakasagihime>());
            player.SpawnPetAndSetBuffTime(buffIndex, ProjectileType<Wriggle>());
            player.SpawnPetAndSetBuffTime(buffIndex, ProjectileType<Youmu>());
            player.SpawnPetAndSetBuffTime(buffIndex, ProjectileType<Yuka>());
            player.SpawnPetAndSetBuffTime(buffIndex, ProjectileType<Yukari>());
            player.SpawnPetAndSetBuffTime(buffIndex, ProjectileType<Yuyuko>());
            player.SpawnPetAndSetBuffTime(buffIndex, ProjectileType<Murasa>());
            player.SpawnPetAndSetBuffTime(buffIndex, ProjectileType<Satori>());
            player.SpawnPetAndSetBuffTime(buffIndex, ProjectileType<Koishi>());
            player.SpawnPetAndSetBuffTime(buffIndex, ProjectileType<Rin>());
            player.SpawnPetAndSetBuffTime(buffIndex, ProjectileType<Utsuho>());
        }
    }
}
