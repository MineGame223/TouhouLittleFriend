namespace TouhouPets.Common.ModSupports.ModPetRegisterSystem;
/// <summary>
/// 不对应任何宠物，仅作排序上占位用
/// </summary>
internal class NonePet:BasicTouhouPet
{
    public override bool IsLoadingEnabled(Mod mod) => false;
    public override TouhouPetID UniqueID => TouhouPetID.None;
}
/// <summary>
/// 不对应任何宠物，仅作排序上占位用
/// </summary>
internal class CountPet : BasicTouhouPet
{
    public override bool IsLoadingEnabled(Mod mod) => false;
    public override TouhouPetID UniqueID => TouhouPetID.Count;
}
