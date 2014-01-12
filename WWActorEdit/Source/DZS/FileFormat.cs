using System;
using System.Collections.Generic;

namespace WWActorEdit
{
    public class DZSFormat
    {
        public DZSHeader Header;
        public List<DZSChunk> Chunks;
    }



    public struct DZSHeader
    {
        public UInt32 ChunkCount;
    }

    public struct DZSChunk
    {
        public string Tag;              //ASCI Name for Chunk
        public UInt32 ElementCount;     //How many elements of this Chunk type
        public UInt32 ChunkOffset;      //Offset from beginning of file to first element
    }
}
