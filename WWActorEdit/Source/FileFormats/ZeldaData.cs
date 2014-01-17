using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using OpenTK;
using WWActorEdit.Kazari;
using WWActorEdit.Source;

namespace WWActorEdit
{
    /// <summary>
    /// Nicknamed "ZeldaData" because both "DZR" and "DZS" use the same format. DZR = "Zelda Room Data"
    /// while DZS = "Zelda Stage Data". 
    /// </summary>
    public class DZSFormat : BaseArchiveFile
    {
        public DZSHeader Header;
        public List<DZSChunkHeader> ChunkHeaders;

        //Data from file
        public byte[] Data;

        private List<IChunkType> _chunkList;

        public T GetSingleChunk<T>()
        {
            foreach (IChunkType chunk in _chunkList)
            {
                if (chunk is T)
                    return (T) chunk;
            }

            return default(T);
        }

        public List<T> GetAllChunks<T>()
        {
            List<T> returnList = new List<T>();
            foreach (IChunkType chunk in _chunkList)
            {
                if (chunk is T)
                    returnList.Add((T) chunk);
            }

            return returnList;
        }

        public override void Load(byte[] data)
        {
            int offset = 0;
            Header = new DZSHeader(data, ref offset);
            ChunkHeaders = new List<DZSChunkHeader>();
            Data = data;

            for (int i = 0; i < Header.ChunkCount; i++)
            {
                DZSChunkHeader chunkHeader = new DZSChunkHeader(data, ref offset);
                ChunkHeaders.Add(chunkHeader);

                for (int k = 0; k < chunkHeader.ElementCount; k++)
                {
                    IChunkType chunk;

                    switch (chunkHeader.Tag)
                    {
                        case "EnvR": chunk = new EnvRChunk(); break;
                        case "Colo": chunk = new ColoChunk(); break;
                        case "Pale": chunk = new PaleChunk(); break;
                        case "Virt": chunk = new VirtChunk(); break;
                        case "SCLS": chunk = new SclsChunk(); break;
                        case "PLYR": chunk = new PlyrChunk(); break;
                        default:
                            chunk = new DefaultChunk();
                            break;
                    }

                    chunk.LoadData(data, ref chunkHeader.ChunkOffset);
                    _chunkList.Add(chunk);
                }
            }
        }

        public override void Save(BinaryWriter stream)
        {
            
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

        public DZSChunkHeader(byte[] data, ref int srcOffset)
        {
            Tag = Helpers.ReadString(data, srcOffset, 4); //Tag is 4 bytes in length.
            ElementCount = (int) Helpers.Read32(data, srcOffset + 4);
            ChunkOffset = (int) Helpers.Read32(data, srcOffset + 8);

            srcOffset += 12; //Header is 0xC/12 bytes in length
        }
    }

    /// <summary>
    /// This empty interface is used so we can stick all of the chunks in a single list.
    /// </summary>
    public interface IChunkType
    {
        void WriteData(BinaryWriter stream);
        void LoadData(byte[] data, ref int srcOffset);
    }

    #region DZS Chunk File Formats

    /// <summary>
    /// For anything not supported yet!
    /// </summary>
    public class DefaultChunk : IChunkType
    {
        public void LoadData(byte[] data, ref int srcOffset) {}
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

        public EnvRChunk()
        {
            ClearColorIndexA = RainingColorIndexA = SnowingColorIndexA = UnknownColorIndexA = 0;
            ClearColorIndexB = RainingColorIndexB = SnowingColorIndexB = UnknownColorIndexB = 0;
        }

        public void LoadData(byte[] data, ref int srcOffset)
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

        public ColoChunk()
        {
            DawnIndex = MorningIndex = NoonIndex = AfternoonIndex = DuskIndex = NightIndex = 0;
        }

        public void LoadData(byte[] data, ref int srcOffset)
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
        public ByteColor UnknownColor1; //Unknown
        public ByteColor UnknownColor2; //Unknown
        public ByteColor DoorwayColor; //Tints the 'Light' mesh behind doors for entering/exiting to the exterior
        public ByteColor UnknownColor3; //Unknown
        public ByteColor FogColor;

        public byte VirtIndex; //Index of the Virt entry to use for Skybox Colors

        public ByteColorAlpha OceanFadeInto;
        public ByteColorAlpha ShoreFadeInto;

        public PaleChunk()
        {
            ActorAmbient = new ByteColor();
            ShadowColor = new ByteColor();
            RoomFillColor = new ByteColor();
            RoomAmbient = new ByteColor();
            WaveColor = new ByteColor();
            OceanColor = new ByteColor();
            UnknownColor1 = new ByteColor();
            UnknownColor2 = new ByteColor();
            DoorwayColor = new ByteColor();
            UnknownColor3 = new ByteColor();
            FogColor = new ByteColor();

            VirtIndex = 0;

            OceanFadeInto = new ByteColorAlpha();
            ShoreFadeInto = new ByteColorAlpha();
        }

        public void LoadData(byte[] data, ref int srcOffset)
        {
            ActorAmbient = new ByteColor(data, ref srcOffset);
            ShadowColor = new ByteColor(data, ref srcOffset);
            RoomFillColor = new ByteColor(data, ref srcOffset); 
            RoomAmbient = new ByteColor(data, ref srcOffset);
            WaveColor = new ByteColor(data, ref srcOffset);
            OceanColor = new ByteColor(data, ref srcOffset);
            UnknownColor1 = new ByteColor(data, ref srcOffset); //Unknown
            UnknownColor2 = new ByteColor(data, ref srcOffset); //Unknown
            DoorwayColor = new ByteColor(data, ref srcOffset);
            UnknownColor3 = new ByteColor(data, ref srcOffset); //Unknown
            FogColor = new ByteColor(data, ref srcOffset);

            VirtIndex = Helpers.Read8(data, srcOffset);
            srcOffset += 3; //Read8 + 2x Padding

            OceanFadeInto = new ByteColorAlpha(data, ref srcOffset);
            ShoreFadeInto = new ByteColorAlpha(data, ref srcOffset);
        }

        public void WriteData(BinaryWriter stream)
        {
            FSHelpers.WriteArray(stream, ActorAmbient.GetBytes());
            FSHelpers.WriteArray(stream, ShadowColor.GetBytes());
            FSHelpers.WriteArray(stream, RoomFillColor.GetBytes());
            FSHelpers.WriteArray(stream, RoomAmbient.GetBytes());
            FSHelpers.WriteArray(stream, WaveColor.GetBytes());
            FSHelpers.WriteArray(stream, OceanColor.GetBytes());
            FSHelpers.WriteArray(stream, UnknownColor1.GetBytes()); //Unknown
            FSHelpers.WriteArray(stream, UnknownColor2.GetBytes()); //Unknown
            FSHelpers.WriteArray(stream, DoorwayColor.GetBytes());
            FSHelpers.WriteArray(stream, UnknownColor3.GetBytes()); //Unknown

            FSHelpers.WriteArray(stream, FogColor.GetBytes());
            FSHelpers.Write8(stream, VirtIndex);
            FSHelpers.WriteArray(stream, FSHelpers.ToBytes(0x0000, 2));//Two bytes padding on Virt Index

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

        public VirtChunk() 
        {
            HorizonCloudColor = new ByteColorAlpha();
            CenterCloudColor = new ByteColorAlpha();
            CenterSkyColor = new ByteColor();
            HorizonColor = new ByteColor();
            SkyFadeTo = new ByteColor();
        }

        public void LoadData(byte[] data, ref int srcOffset)
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
            FSHelpers.WriteArray(stream, FSHelpers.ToBytes(0x80000000, 4));
            FSHelpers.WriteArray(stream, FSHelpers.ToBytes(0x80000000, 4));
            FSHelpers.WriteArray(stream, FSHelpers.ToBytes(0x80000000, 4));
            FSHelpers.WriteArray(stream, FSHelpers.ToBytes(0x80000000, 4));

            FSHelpers.WriteArray(stream, HorizonCloudColor.GetBytes());
            FSHelpers.WriteArray(stream, CenterCloudColor.GetBytes());
            FSHelpers.WriteArray(stream, CenterSkyColor.GetBytes());
            FSHelpers.WriteArray(stream, HorizonColor.GetBytes());
            FSHelpers.WriteArray(stream, SkyFadeTo.GetBytes());

            FSHelpers.WriteArray(stream, FSHelpers.ToBytes(0xFFFFFF, 3)); //3 Bytes Padding
        }
    }

    /// <summary>
    /// The SCLS Chunk defines information about exits on a map. It is pointed to by
    /// the maps collision data (which supplies the actual positions)
    /// </summary>
    public class SclsChunk : IChunkType
    {
        public string DestinationName;
        public byte SpawnNumber;
        public byte DestinationRoomNumber;
        public byte ExitType;
        public byte UnknownPadding;

        public void LoadData(byte[] data, ref int srcOffset)
        {
            DestinationName = Helpers.ReadString(data, srcOffset, 8);
            SpawnNumber = Helpers.Read8(data, srcOffset + 8);
            DestinationRoomNumber = Helpers.Read8(data, srcOffset + 9);
            ExitType = Helpers.Read8(data, srcOffset + 10);
            UnknownPadding = Helpers.Read8(data, srcOffset + 11);

            srcOffset += 12;
        }


        public void WriteData(BinaryWriter stream)
        {
            FSHelpers.WriteString(stream, DestinationName, 8);
            FSHelpers.Write8(stream, SpawnNumber);
            FSHelpers.Write8(stream, DestinationRoomNumber);
            FSHelpers.Write8(stream, ExitType);
            FSHelpers.Write8(stream, UnknownPadding);
        }
    }

    /// <summary>
    /// The Plyr (Player) chunk defines spawn points for Link.
    /// </summary>
    public class PlyrChunk : IChunkType
    {
        public string Name; //"Link"
        public byte EventIndex; //Spcifies an event from the DZS file to play upon spawn. FF = no event.
        public byte Unknown1; //Padding?
        public byte SpawnType; //How Link enters the room.
        public byte RoomNumber; //Room number the spawn is in.
        public Vector3 Position;
        public HalfRotation Rotation;

        public void LoadData(byte[] data, ref int srcOffset)
        {
            Name = Helpers.ReadString(data, srcOffset, 8);
            EventIndex = Helpers.Read8(data, srcOffset + 8);
            Unknown1 = Helpers.Read8(data, srcOffset + 9);
            SpawnType = Helpers.Read8(data, srcOffset + 10);
            RoomNumber = Helpers.Read8(data, srcOffset + 11);

            Position.X = Helpers.ConvertIEEE754Float(Helpers.Read32(data, srcOffset + 12));
            Position.Y = Helpers.ConvertIEEE754Float(Helpers.Read32(data, srcOffset + 16));
            Position.Z = Helpers.ConvertIEEE754Float(Helpers.Read32(data, srcOffset + 20));

            srcOffset += 24;
            Rotation = new HalfRotation(data, ref srcOffset);
         
            srcOffset += 2; //Two bytes Padding
        }

        public void WriteData(BinaryWriter stream)
        {
            FSHelpers.WriteString(stream, Name, 8);
            FSHelpers.Write8(stream, EventIndex);
            FSHelpers.Write8(stream, Unknown1);
            FSHelpers.Write8(stream, SpawnType);
            FSHelpers.Write8(stream, RoomNumber);
            FSHelpers.WriteFloat(stream, Position.X);
            FSHelpers.WriteFloat(stream, Position.Y);
            FSHelpers.WriteFloat(stream, Position.Z);
            FSHelpers.Write16(stream, (ushort)Rotation.X);
            FSHelpers.Write16(stream, (ushort) Rotation.Y);
            FSHelpers.Write16(stream, (ushort) Rotation.Z);

            FSHelpers.WriteArray(stream, FSHelpers.ToBytes(0xFFFF, 2));
        }

    }
    #endregion

    public class HalfRotation
    {
        public short X, Y, Z;

        public HalfRotation()
        {
            X = Y = Z = 0;
        }

        public HalfRotation(byte[] data, ref int srcOffset)
        {
            X = (short) Helpers.Read16(data, srcOffset);
            Y = (short) Helpers.Read16(data, srcOffset);
            Z = (short) Helpers.Read16(data, srcOffset);

            srcOffset += 6;
        }

        public Vector3 ToDegrees()
        {
            Vector3 rot = new Vector3();
            rot.X = X/182.04444444444f;
            rot.Y = Y/182.04444444444f;
            rot.Z = Z/182.04444444444f;

            return rot;
        }

        public void SetDegrees(Vector3 rot)
        {
            X = (short) (rot.X*182.04444444444f);
            Y = (short) (rot.Y*182.04444444444f);
            Z = (short) (rot.Z*182.04444444444f);
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
    
}
