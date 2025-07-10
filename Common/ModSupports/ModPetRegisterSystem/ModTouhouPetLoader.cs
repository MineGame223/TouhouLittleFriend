using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.ID;

namespace TouhouPets.Common.ModSupports.ModPetRegisterSystem;

public class ModTouhouPetLoader : ILoadable
{
    private static readonly List<BasicTouhouPet> _touhouPets = [];

    public static IReadOnlyList<BasicTouhouPet> TouhouPets => _touhouPets;

    private static readonly Dictionary<int, BasicTouhouPet> _fromProjID = [];

    public static IReadOnlyDictionary<int, BasicTouhouPet> FromProjID => _fromProjID;

    public static int TotalCount => _touhouPets.Count;

    internal static bool ReadyToUse { get; private set; }

    internal static int Add(BasicTouhouPet victoryPose)
    {
        int type = _touhouPets.Count;
        _touhouPets.Add(victoryPose);
        return type;
    }

    public void Load(Mod mod)
    {
        Add(new NonePet());
        Add(new CountPet());
    }

    public void Unload()
    {
    }

    internal static void CleanUpVanillaParts()
    {
        int count = (int)TouhouPetID.Count + 1;
        var vanillaPart = _touhouPets[0..count];
        for (int n = 0; n < count; n++)
        {
            var vanillaInstance = vanillaPart[n];
            _touhouPets[(int)vanillaInstance.UniqueID] = vanillaInstance;
        }

    }

    internal static void RegisterFromIDDictionary()
    {

        foreach (var pet in TouhouPets)
            if (pet.Projectile is Projectile proj) // 增加一个null检测，防止未加载的类炸程序
                _fromProjID.Add(proj.type, pet);
    }

    public static int TouhouPetType<T>() where T : BasicTouhouPet => GetInstance<T>()?.TouhouPetType ?? 0;

    public static void SetReadyToUse() => ReadyToUse = true;

}
internal class ModPetRegisterSystem : ModSystem
{
    public override void PostSetupContent()
    {
        ModTouhouPetLoader.CleanUpVanillaParts();
        ModTouhouPetLoader.RegisterFromIDDictionary();
        ModTouhouPetLoader.SetReadyToUse();
        TouhouPets.ResizeCrossModList(ModTouhouPetLoader.TotalCount);
        TouhouPets.ResizeChatSetting(ModTouhouPetLoader.TotalCount);
    }
}