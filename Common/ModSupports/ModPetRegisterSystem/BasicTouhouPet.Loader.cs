using Terraria;
using TouhouPets.Common.ModSupports.ModPetRegisterSystem;

namespace TouhouPets;

partial class BasicTouhouPet
{
    void SetTouhouPetType()
    {
        if (ModTouhouPetLoader.ReadyToUse)
            TouhouPetType = ModTouhouPetLoader.FromProjID[Type].TouhouPetType; // 从模板实例中复制值
    }

    void RegisterToModPetLoader()
    {
        // 注册
        TouhouPetType = ModTouhouPetLoader.Add(this);

        // 对于TouhouPets自身的宠物，和枚举对齐
        if (TouhouPetType < (int)TouhouPetID.Count)
            TouhouPetType = (int)UniqueID;
    }

    /// <summary>
    /// 对于<see cref="TouhouPets"/>内的类，这个值就是<see cref="UniqueID">对应的整数值
    /// <br>而新添加的类则</br>
    /// </summary>
    public int TouhouPetType { get; private set; }
}
