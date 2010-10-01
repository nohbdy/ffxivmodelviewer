namespace DatDigger.Sections
{
    public enum SectionMagicNumber : long
    {
        Texture = 0x0062787442444553, // SEDBtxb\0
        Resource = 0x2053455242444553, // 'SEDBRES '
        Sound = 0x4643535342444553, // SEDBSSCF
        Vins = 0x736E697642444553, // SEDBvins
        Vtex = 0x7865747642444553, // SEDBvtex
        Vmdl = 0x6C646D7642444553, // SEDBvmdl
        Veff = 0x6666657642444553, // SEDBveff
        Leaf = 0x6661656C42444553, // SEDBleaf
        Phb = 0x0042485042444553, // SEDBPHB\0
        Mtb = 0x0062746D42444553, // SEDBmtb\0
        Unknown = -1
    }
}
