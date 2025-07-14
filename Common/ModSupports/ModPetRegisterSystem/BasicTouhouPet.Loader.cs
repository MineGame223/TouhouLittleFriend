using TouhouPets.Common.ModSupports.ModPetRegisterSystem;

namespace TouhouPets;

partial class BasicTouhouPet
{
    /// <summary>
    /// 为当前弹幕实例设置注册时的拓展独特ID
    /// <br></br>
    /// <br>在<see cref="SetDefaults"/>中被调用</br>
    /// <br></br>
    /// <br>用于保证处于世界时新生成的宠物拥有和注册时一致的ID</br>
    /// </summary>
    void SetExtendedUniqueID()
    {
        if (ModTouhouPetLoader.ReadyToUse)
            UniqueIDExtended = ModTouhouPetLoader.FromProjID[Type].UniqueIDExtended; // 从模板实例中复制值
    }

    /// <summary>
    /// 将当前加载期宠物实例注册到<see cref="ModTouhouPetLoader"/>
    /// </summary>
    void RegisterToModPetLoader()
    {
        // 注册
        UniqueIDExtended = ModTouhouPetLoader.Add(this);

        // 对于TouhouPets自身的宠物，和枚举对齐
        if (UniqueIDExtended <= (int)TouhouPetID.Count)
            UniqueIDExtended = (int)UniqueID;
    }

    /// <summary>
    /// 对于<see cref="TouhouPets"/>内的类，这个值就是<see cref="UniqueID"/>对应的整数值
    /// <br></br>
    /// <br>而新添加的类则由它们注册时的值决定，这个值一定会比<see cref="TouhouPetID.Count"/>大</br>
    /// <br></br>
    /// <br>基本替代了<see cref="UniqueID"/>的功能，枚举值ID现仅用作对东方小伙伴内部宠物进行注册排序</br>
    /// <br></br>
    /// <br>对于东方小伙伴内部的宠物，添加注册对话时可以依旧采用<see cref="TouhouPetID"/></br>
    /// <br></br>
    /// <br>对于附属模组添加的宠物，添加注册对话时可以用<see cref="ModTouhouPetLoader.UniqueID{T}"/>获取这个值</br>
    /// </summary>
    public int UniqueIDExtended { get; private set; }
}
