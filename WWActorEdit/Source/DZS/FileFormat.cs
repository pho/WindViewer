using System;
using System.Collections.Generic;
using System.Drawing;
using WWActorEdit.Kazari;

namespace WWActorEdit
{
    public class DZSFormat
    {
        public DZSHeader Header;
        public List<DZSChunkHeader> ChunkHeaders;

        //Data from file
        public byte[] Data;


        public DZSFormat(byte[] data, ref int srcOffset)
        {
            Header = new DZSHeader(data, ref srcOffset);
            ChunkHeaders = new List<DZSChunkHeader>();
            Data = data;

            for (int i = 0; i < Header.ChunkCount; i++)
            {
                DZSChunkHeader chunkHeader = new DZSChunkHeader(data, ref srcOffset);
                ChunkHeaders.Add(chunkHeader);

                for (int k = 0; k < chunkHeader.ElementCount; k++)
                {
                    IChunkType chunk;

                    switch (chunkHeader.Tag)
                    {
                        case "EnvR": chunk = new EnvRChunk(data, ref chunkHeader.ChunkOffset); break;
                        case "Colo": chunk = new ColoChunk(data, ref chunkHeader.ChunkOffset); break;
                        case "Pale": chunk = new PaleChunk(data, ref chunkHeader.ChunkOffset); break;
                        case "Virt": chunk = new VirtChunk(data, ref chunkHeader.ChunkOffset); break;
                        default:
                            Console.WriteLine("Unsupported Chunk Type: " + chunkHeader.Tag + ", ignoring...");
                            chunk = new DefaultChunk();
                            break;
                    }

                    chunkHeader.ChunkElements.Add(chunk);
                }
            }
        }
    }

    public class DZSHeader
    {
        public UInt32 ChunkCount;

        public DZSHeader(byte[] data, ref int srcOffset)
        {
            ChunkCount = Helpers.Read32(data, srcOffset);
            srcOffset += 4;
        }
    }

    public class DZSChunkHeader
    {
        public string Tag;              //ASCI Name for Chunk
        public int ElementCount;     //How many elements of this Chunk type
        public int ChunkOffset;      //Offset from beginning of file to first element

        [NonSerialized] public List<IChunkType> ChunkElements; 

        public DZSChunkHeader(byte[] data, ref int srcOffset)
        {
            Tag = Helpers.ReadString(data, srcOffset, 4); //Tag is 4 bytes in length.
            ElementCount = (int) Helpers.Read32(data, srcOffset + 4);
            ChunkOffset = (int) Helpers.Read32(data, srcOffset + 8);

            ChunkElements = new List<IChunkType>();
            srcOffset += 12; //Header is 0xC/12 bytes in length
        }
    }

    /// <summary>
    /// This empty interface is used so we can stick all of the chunks in a single list.
    /// </summary>
    public interface IChunkType
    {
        
    }

    #region DZS Chunk File Formats

    /// <summary>
    /// For anything not supported yet!
    /// </summary>
    public class DefaultChunk : IChunkType
    {
        
    }

    /// <summary>
    /// The EnvR (short for Environment) chunk contains indexes of different color pallets
    ///  to use in different weather situations. 
    /// </summary>
    public class EnvRChunk : IChunkType
    {
        public byte ClearColorIndexA; //Index of the Color entry to use for clear weather.
        public byte RainingColorIndexA; //There's two sets, A and B. B's usage is unknown but identical.
        public byte SnowingColorIndexA;
        public byte UnknownColorIndexA; //We don't know what weather this color is used for!

        public byte ClearColorIndexB;
        public byte RainingColorIndexB;
        public byte SnowingColorIndexB;
        public byte UnknownColorIndexB;

        public EnvRChunk(byte[] data, ref int srcOffset)
        {
            ClearColorIndexA = Helpers.Read8(data, srcOffset + 0);
            RainingColorIndexA = Helpers.Read8(data, srcOffset + 1);
            SnowingColorIndexA = Helpers.Read8(data, srcOffset + 2);
            UnknownColorIndexA = Helpers.Read8(data, srcOffset + 3);

            ClearColorIndexB = Helpers.Read8(data, srcOffset + 4);
            RainingColorIndexB = Helpers.Read8(data, srcOffset + 5);
            SnowingColorIndexB = Helpers.Read8(data, srcOffset + 6);
            UnknownColorIndexB = Helpers.Read8(data, srcOffset + 7);

            srcOffset += 8;
        }
    }

    /// <summary>
    /// Colo (short for Color) contains indexes into the Pale section. Color specifies
    /// which color to use for the different times of day.
    /// </summary>
    public class ColoChunk : IChunkType
    {
        public byte DawnIndex; //Index of the Pale entry to use for Dawn
        public byte MorningIndex;
        public byte NoonIndex;
        public byte AfternoonIndex;
        public byte DuskIndex;
        public byte NightIndex;

        public ColoChunk(byte[] data, ref int srcOffset)
        {
            DawnIndex =     Helpers.Read8(data, srcOffset + 0);
            MorningIndex =  Helpers.Read8(data, srcOffset + 1);
            NoonIndex =     Helpers.Read8(data, srcOffset + 2);
            AfternoonIndex = Helpers.Read8(data, srcOffset + 3);
            DuskIndex =     Helpers.Read8(data, srcOffset + 4);
            NightIndex =    Helpers.Read8(data, srcOffset + 5);

            srcOffset += 6;
        }
    }

    /// <summary>
    /// The Pale (short for Palette) chunk contains the actual RGB colors for different
    /// types of lighting. 
    /// </summary>
    public class PaleChunk : IChunkType
    {
        public ByteColor ActorAmbient;
        public ByteColor ShadowColor;
        public ByteColor RoomAmbient;
        public ByteColor WaveColor;
        public ByteColor OceanColor;
        public ByteColor DoorwayColor; //Tints the 'Light' mesh behind doors for entering/exiting to the exterior
        public ByteColor FogColor;

        public byte VirtIndex; //Index of the Virt entry to use for Skybox Colors

        public ByteColor OceanFadeInto;

        public PaleChunk(byte[] data, ref int srcOffset)
        {
            ActorAmbient = new ByteColor(data, ref srcOffset);
            ShadowColor = new ByteColor(data, ref srcOffset);
            srcOffset += 3; //Unused Values apparently
            RoomAmbient = new ByteColor(data, ref srcOffset);
            WaveColor = new ByteColor(data, ref srcOffset);
            OceanColor = new ByteColor(data, ref srcOffset);
            srcOffset += 6; //More unused values
            DoorwayColor = new ByteColor(data, ref srcOffset);
            srcOffset += 3;
            FogColor = new ByteColor(data, ref srcOffset);

            VirtIndex = Helpers.Read8(data, srcOffset); srcOffset += 1;
            srcOffset += 2; //More unused values

            OceanFadeInto = new ByteColor(data, ref srcOffset);
            srcOffset += 5;
        }
    }

    /// <summary>
    /// The Virt (short for uh.. Virtual? I dunno) chunk contains color data for the skybox. Indexed by a Pale
    /// chunk.
    /// </summary>
    public class VirtChunk : IChunkType
    {
        public ByteColorAlpha HorizonCloudColor; //The Horizon
        public ByteColorAlpha CenterCloudColor;  //Directly above you
        public ByteColor CenterSkyColor;
        public ByteColor HorizonColor;
        public ByteColor SkyFadeTo; //Color to fade to from CenterSky. 

        public VirtChunk(byte[] data, ref int srcOffset)
        {
            //First 16 bytes are 80 00 00 00 (repeated 4 times). Unknown why.
            srcOffset += 16;

            HorizonCloudColor = new ByteColorAlpha(data, ref srcOffset);
            CenterCloudColor = new ByteColorAlpha(data, ref srcOffset);

            CenterSkyColor = new ByteColor(data, ref srcOffset);
            HorizonColor = new ByteColor(data, ref srcOffset);
            SkyFadeTo = new ByteColor(data, ref srcOffset);

            //More apparently unused bytes.
            srcOffset += 3;
        }
    }


    public class ByteColor
    {
        public byte R, G, B;

        public ByteColor()
        {
            R = B = G = 0;
        }

        public ByteColor(byte[] data, ref int srcOffset)
        {
            R = Helpers.Read8(data, srcOffset + 0);
            G = Helpers.Read8(data, srcOffset + 1);
            B = Helpers.Read8(data, srcOffset + 2);

            srcOffset += 3;
        }
    }

    public class ByteColorAlpha
    {
        public byte R, G, B, A;

        public ByteColorAlpha(byte[] data, ref int srcOffset)
        {
            R = Helpers.Read8(data, srcOffset + 0);
            G = Helpers.Read8(data, srcOffset + 1);
            B = Helpers.Read8(data, srcOffset + 2);
            A = Helpers.Read8(data, srcOffset + 3);

            srcOffset += 4;
        }

        public ByteColorAlpha()
        {
            R = G = B = A = 0;
        }

        public ByteColorAlpha(ByteColor color)
        {
            R = color.R;
            G = color.G;
            B = color.B;
            A = 0;
        }
    }
    #endregion
}
