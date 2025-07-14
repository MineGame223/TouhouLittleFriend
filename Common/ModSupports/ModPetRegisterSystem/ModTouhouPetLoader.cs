using System.Collections.Generic;
using Terraria;

namespace TouhouPets.Common.ModSupports.ModPetRegisterSystem;

/// <summary>
/// 用于管理东方小伙伴宠物和其附属模组新增的宠物
/// </summary>
public class ModTouhouPetLoader : ILoadable
{
    /// <summary>
    /// 加载时注册的弹幕实例列表
    /// </summary>
    private static readonly List<BasicTouhouPet> _touhouPets = [];

    /// <summary>
    /// 加载时注册的弹幕实例列表，以只读列表的形式对外暴露
    /// <br></br>
    /// <br>用<see cref="BasicTouhouPet.UniqueIDExtended"/>作为下标即可获得对应的实例</br>
    /// </summary>
    public static IReadOnlyList<BasicTouhouPet> TouhouPets => _touhouPets;

    /// <summary>
    /// 加载时注册的弹幕实例字典
    /// </summary>
    private static readonly Dictionary<int, BasicTouhouPet> _fromProjID = [];

    /// <summary>
    /// 加载时注册的弹幕实例字典，以只读字典的形式对外暴露
    /// <br></br>
    /// <br>用<see cref="ModProjectile.Type"/>作为下标即可获得对应的实例</br>
    /// </summary>
    public static IReadOnlyDictionary<int, BasicTouhouPet> FromProjID => _fromProjID;

    /// <summary>
    /// 目前的宠物的总数
    /// </summary>
    public static int TotalCount => _touhouPets.Count;

    /// <summary>
    /// 内部使用，用于判定对话字典数组是否已经重新扩容而可以用于写入值
    /// </summary>
    internal static bool ReadyToUse { get; private set; }

    /// <summary>
    /// 注册一个新宠物时调用的函数
    /// <br >完整注册见<see cref="BasicTouhouPet.RegisterToModPetLoader"/></br>
    /// </summary>
    /// <param name="pet">此次注册的宠物</param>
    /// <returns></returns>
    internal static int Add(BasicTouhouPet pet)
    {
        int type = _touhouPets.Count;
        _touhouPets.Add(pet);
        return type;
    }

    /// <summary>
    /// 加载时执行的函数，这里手动添加了两个用于占位的不加载的宠物
    /// <br>分别对应<see cref="TouhouPetID.None"/> 和 <see cref="TouhouPetID.Count"/></br>
    /// </summary>
    /// <param name="mod"></param>
    public void Load(Mod mod)
    {
        Add(new NonePet());
        Add(new CountPet());
    }

    public void Unload()
    {
    }

    /// <summary>
    /// 这个函数用于将<see cref="BasicTouhouPet.UniqueIDExtended"/>与<see cref="BasicTouhouPet.UniqueID"/>对齐
    /// <br></br>
    /// <br>也就是对于东方小伙伴内部的宠物，依旧完全可以采用原先的方式编写对话</br>
    /// </summary>
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

    /// <summary>
    /// 这个函数用于填充<see cref="_fromProjID"/>
    /// 
    /// <br>因为<see cref="NonePet"/>和<see cref="CountPet"/>是不会加载的，这两个宠物不会被加入字典</br>
    /// </summary>
    internal static void RegisterFromIDDictionary()
    {
        foreach (var pet in TouhouPets)
            if (pet.Projectile is Projectile proj) // 增加一个null检测，防止未加载的类炸程序
                _fromProjID.Add(proj.type, pet);
    }

    /// <summary>
    /// 注册包括拓展模组宠物在内的全部宠物的对话
    /// </summary>
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

    /// <summary>
    /// 通过类型查找对应的整数唯一ID
    /// <br></br>
    /// <br>使用方式和<see cref="ProjectileType{T}"/>完全一致</br>
    /// <br></br>
    /// <br>唯一的区别就是获取的不是弹幕ID而是宠物唯一ID</br>
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <returns></returns>
    public static int UniqueID<T>() where T : BasicTouhouPet => GetInstance<T>()?.UniqueIDExtended ?? 0;


    /// <summary>
    /// 标记准备工作完毕
    /// <br></br>
    /// <br>在 <see cref="ModPetRegisterSystem.PostSetupContent"/> 中调用</br>
    /// </summary>
    internal static void SetReadyToUse() => ReadyToUse = true;

}
internal class ModPetRegisterSystem : ModSystem
{
    /// <summary>
    /// 在这个重写函数完成如下工作
    /// <br></br>
    /// <br><see cref="ModTouhouPetLoader.CleanUpVanillaParts"/> : 根据内部宠物的UniqueID重新对齐UniqueIDExtended</br>
    /// <br></br>
    /// <br><see cref="ModTouhouPetLoader.RegisterFromIDDictionary"/> : 根据所有已加载宠物的Type将自身实例添加到字典</br>
    /// <br></br>
    /// <br><see cref="TouhouPets.ResizeChatSetting(int)"/> : 因为加载了模组宠物，这里需要对原先数组扩容</br>
    /// <br></br>
    /// <br><see cref="TouhouPets.ResizeCrossModList(int)"/> : 同样是对原先数组进行扩容</br>
    /// <br></br>
    /// <br><see cref="ModTouhouPetLoader.RegisterAllPetsChat"/> : 因为数组需要扩容，统一的对话注册只能放到扩容后进行</br>
    /// <br></br>
    /// <br><see cref="ModTouhouPetLoader.SetReadyToUse"/> : 标记准备工作已完成</br>
    /// </summary>
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

