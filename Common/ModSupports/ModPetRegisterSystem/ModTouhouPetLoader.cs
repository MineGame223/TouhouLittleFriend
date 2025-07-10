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

    internal static void RegisterAllPetsChat() 
    {
        // 这里遍历所有记录在案的宠物，进行它们的对话注册
        // 需要延后到这里来统一进行是因为那个数组的扩容发生在模组内容加载完
        // 也就是所有的模组宠物都已经添加到FromProjID后
        // 提前进行的话数组就越界啦
        // 每注册一个新宠物就重新扩一次容也不好
        // 所以延后到这里进行对话注册了
        foreach (var pet in FromProjID.Values)
            pet.RegisterChat_Full();
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
        TouhouPets.ResizeChatSetting(ModTouhouPetLoader.TotalCount);
        TouhouPets.ResizeCrossModList(ModTouhouPetLoader.TotalCount);
        ModTouhouPetLoader.RegisterAllPetsChat();
        ModTouhouPetLoader.SetReadyToUse();
    }
}