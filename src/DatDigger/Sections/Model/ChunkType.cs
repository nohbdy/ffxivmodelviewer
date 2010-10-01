namespace DatDigger.Sections.Model
{
    public enum ChunkType : int
    {
        WRB  = 0x00425257,
        MDLC = 0x434C444D,
        MDL  = 0x004C444D,
        NAME = 0x454D414E,
        HEAD = 0x44414548,
        MESH = 0x4853454D,
        STR  = 0x00525453,
        RSID = 0x44495352,
        RSTP = 0x50545352,
        PRID = 0x44495250,
        PRTP = 0x50545250,
        STMS = 0x534D5453,
        ENVD = 0x44564E45,
        AABB = 0x42424141,
        COMP = 0x504D4F43,
        LTCD = 0x4443544C,
        MICT = 0x5443494D,
        MINS = 0x534E494D,
        PGRP = 0x50524750,
        SHAP = 0x50414853
    }
}
