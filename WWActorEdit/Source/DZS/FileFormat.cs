using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using WWActorEdit.Kazari;
using WWActorEdit.Source;

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
        void WriteData(BinaryWriter stream);
    }

    #region DZS Chunk File Formats

    /// <summary>
    /// For anything not supported yet!
    /// </summary>
    public class DefaultChunk : IChunkType
    {
        public void WriteData(BinaryWriter stream) {}
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

        public void WriteData(BinaryWriter stream)
        {
            FSHelpers.Write8(stream, ClearColorIndexA);
            FSHelpers.Write8(stream, RainingColorIndexA);
            FSHelpers.Write8(stream, SnowingColorIndexA);
            FSHelpers.Write8(stream, UnknownColorIndexA);

            FSHelpers.Write8(stream, ClearColorIndexB);
            FSHelpers.Write8(stream, RainingColorIndexB);
            FSHelpers.Write8(stream, SnowingColorIndexB);
            FSHelpers.Write8(stream, UnknownColorIndexB);
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

        public void WriteData(BinaryWriter stream)
        {
            FSHelpers.Write8(stream, DawnIndex);
            FSHelpers.Write8(stream, MorningIndex);
            FSHelpers.Write8(stream, NoonIndex);
            FSHelpers.Write8(stream, AfternoonIndex);
            FSHelpers.Write8(stream, DuskIndex);
            FSHelpers.Write8(stream, NightIndex);
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
        public ByteColor RoomFillColor;
        public ByteColor RoomAmbient;
        public ByteColor WaveColor;
        public ByteColor OceanColor;
        public ByteColor DoorwayColor; //Tints the 'Light' mesh behind doors for entering/exiting to the exterior
        public ByteColor FogColor;

        public byte VirtIndex; //Index of the Virt entry to use for Skybox Colors

        public ByteColorAlpha OceanFadeInto;
        public ByteColorAlpha ShoreFadeInto;

        public PaleChunk(byte[] data, ref int srcOffset)
        {
            ActorAmbient = new ByteColor(data, ref srcOffset);
            ShadowColor = new ByteColor(data, ref srcOffset);
            RoomFillColor = new ByteColor(data, ref srcOffset); 
            RoomAmbient = new ByteColor(data, ref srcOffset);
            WaveColor = new ByteColor(data, ref srcOffset);
            OceanColor = new ByteColor(data, ref srcOffset);
            srcOffset += 6; //More unused values
            DoorwayColor = new ByteColor(data, ref srcOffset);
            srcOffset += 3;
            FogColor = new ByteColor(data, ref srcOffset);

            VirtIndex = Helpers.Read8(data, srcOffset);
            srcOffset += 2; //More unused values

            OceanFadeInto = new ByteColorAlpha(data, ref srcOffset);
            ShoreFadeInto = new ByteColorAlpha(data, ref srcOffset);
            srcOffset += 1;
        }

        public void WriteData(BinaryWriter stream)
        {
            FSHelpers.WriteArray(stream, ActorAmbient.GetBytes());
            FSHelpers.WriteArray(stream, ShadowColor.GetBytes());
            FSHelpers.WriteArray(stream, RoomFillColor.GetBytes());
            FSHelpers.WriteArray(stream, RoomAmbient.GetBytes());
            FSHelpers.WriteArray(stream, WaveColor.GetBytes());
            FSHelpers.WriteArray(stream, OceanColor.GetBytes());
            
            //Write our 6 unknown bytes in as FF FF FF for now.
            FSHelpers.WriteArray(stream, BitConverter.GetBytes(0xFFFFFF));
            byte[] test = BitConverter.GetBytes(0xFFFFFF);
            FSHelpers.WriteArray(stream, BitConverter.GetBytes(0xFFFFFF));

            FSHelpers.WriteArray(stream, DoorwayColor.GetBytes());

            //Unknown 3
            FSHelpers.WriteArray(stream, BitConverter.GetBytes(0xFFFFFF));

            FSHelpers.WriteArray(stream, FogColor.GetBytes());
            FSHelpers.Write8(stream, VirtIndex);
            FSHelpers.WriteArray(stream, BitConverter.GetBytes(0xFFFF));//Two bytes padding on Virt Index

            FSHelpers.WriteArray(stream, OceanFadeInto.GetBytes());
            FSHelpers.WriteArray(stream, ShoreFadeInto.GetBytes());
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

        public void WriteData(BinaryWriter stream)
        {
            //Fixed values that doesn't seem to change.
            FSHelpers.WriteArray(stream, BitConverter.GetBytes(0x80000000));
            FSHelpers.WriteArray(stream, BitConverter.GetBytes(0x80000000));
            FSHelpers.WriteArray(stream, BitConverter.GetBytes(0x80000000));
            FSHelpers.WriteArray(stream, BitConverter.GetBytes(0x80000000));

            FSHelpers.WriteArray(stream, HorizonCloudColor.GetBytes());
            FSHelpers.WriteArray(stream, CenterCloudColor.GetBytes());
            FSHelpers.WriteArray(stream, CenterSkyColor.GetBytes());
            FSHelpers.WriteArray(stream, HorizonColor.GetBytes());
            FSHelpers.WriteArray(stream, SkyFadeTo.GetBytes());

            FSHelpers.WriteArray(stream, BitConverter.GetBytes(0xFFFFFF)); //Padding
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

        public byte[] GetBytes()
        {
            byte[] bytes = new byte[3];
            bytes[0] = R;
            bytes[1] = G;
            bytes[2] = B;

            return bytes;
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

        public byte[] GetBytes()
        {
            byte[] bytes = new byte[4];
            bytes[0] = R;
            bytes[1] = G;
            bytes[2] = B;
            bytes[3] = A;

            return bytes;
        }
    }
    #endregion
}
