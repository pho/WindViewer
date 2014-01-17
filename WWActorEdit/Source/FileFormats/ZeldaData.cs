using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.IO;
using System.Windows.Forms;
using OpenTK;
using WWActorEdit.Kazari;
using WWActorEdit.Source;

namespace WWActorEdit
{
    /// <summary>
    /// Nicknamed "ZeldaData" because both "DZR" and "DZS" use the same format. DZR = "Zelda Room Data"
    /// while DZS = "Zelda Stage Data". To solve this, I'm just calling it ZeldaData;
    /// </summary>
    public class ZeldaData : BaseArchiveFile
    {
        //The only thing we keep is a list of the Chunks as this is all we need to re-create the file.
        private List<IChunkType> _chunkList;

        public override void Load(byte[] data)
        {
            int offset = 0;
            DZSHeader header = new DZSHeader();
            header.Load(data, ref offset);


            _chunkList = new List<IChunkType>();

            for (int i = 0; i < header.ChunkCount; i++)
            {
                DZSChunkHeader chunkHeader = new DZSChunkHeader();
                chunkHeader.Load(data, ref offset);

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
                        case "RPAT":
                        case "PATH":
                            chunk = new RPATChunk(); break;
                        case "RPPN":
                        case "PPNT":
                            chunk = new RPATChunk(); break;

                        case "SOND": chunk = new SondChunk(); break;
                        case "FILI": chunk = new FiliChunk(); break;
                        case "MECO": chunk = new MecoChunk(); break;
                        case "MEMA": chunk = new MemaChunk(); break;
                        case "TRES": chunk = new TresChunk(); break;
                        case "SHIP" : chunk = new ShipChunk(); break;
                        case "MULT" : chunk = new MultChunk(); break;
                        case "LGHT":
                        case "LGTV":
                            chunk = new LghtChunk(); break;
                        case "RARO":
                        case "AROB":
                            chunk = new RaroChunk(); break;
                        case "Evnt": chunk = new EvntChunk(); break;
                        case "ACTR": chunk = new ActrChunk(); break;
                        case "STAG": chunk = new StagChunk(); break;
                        case "RCAM":
                        case "CAMR":
                            chunk = new RcamChunk();break;
                        case "FLOR": chunk = new FlorChunk(); break;
                        default:
                            Console.WriteLine("Unsupported Chunk Tag: " + chunkHeader.Tag +
                                              " making DefaultChunk() instead!");
                            chunk = new DefaultChunk();
                            break;
                    }

                    Console.WriteLine("Processing chunk: " + chunkHeader.Tag);

                    chunk.LoadData(data, ref chunkHeader.ChunkOffset);
                    _chunkList.Add(chunk);
                }
            }
        }

        public override void Save(BinaryWriter stream)
        {
            //This is a really weird/complicated implementation. We can fill out the DZSHeader pretty easily
            //but then we don't know the offsets until we write them... To fix this we're going to jump around
            //in the stream a little bit. It's kind of weird. There's probably a simpler implementation that I'm
            //missing, but oh well.

            //We need to sort out the unique chunks out of our list, as some chunks only have one entry,
            //and some will have multiple. This is kind of a weird implementation, oops.
            var dict = new Dictionary<Type, List<IChunkType>>();

            foreach (IChunkType chunkType in _chunkList)
            {
                //If the dictionary already contains the key, then the list exists and we can just add to it.
                if (dict.ContainsKey(chunkType.GetType()))
                {
                    dict[chunkType.GetType()].Add(chunkType);
                }
                else
                {
                    //The dictionary doesn't contain the key, so make the list and add it.
                    dict[chunkType.GetType()] = new List<IChunkType>();
                    dict[chunkType.GetType()].Add(chunkType);
                }
            }

            //Write the Header
            DZSHeader header = new DZSHeader();
            header.ChunkCount = dict.Count;
            header.Save(stream);

            int curOffset = (int)stream.BaseStream.Position;

            //Then allocate numChunkHeaders * chunkHeaderSize and then write the chunks, and then we'll go back...?
            stream.BaseStream.Position += dict.Count*12; //A chunk Header is 12 bytes in size.

            List<DZSChunkHeader> headerList = new List<DZSChunkHeader>();

            //Now write all the chunk headers
            foreach (KeyValuePair<Type, List<IChunkType>> pair in dict)
            {
                DZSChunkHeader chnkHeader = new DZSChunkHeader();
                chnkHeader.ChunkOffset = (int) stream.BaseStream.Position;
                chnkHeader.Tag = pair.Key.Name.Substring(0, 4);
                chnkHeader.ElementCount = pair.Value.Count;

                headerList.Add(chnkHeader);

                //Now write the actual chunks to the stream.
                foreach (IChunkType chunk in pair.Value)
                {
                    chunk.WriteData(stream);
                }
            }

            //Now that we've created the chunk headers, they have their offsets set, and the data is made, lets go back and actually write them to file.
            stream.BaseStream.Position = curOffset;
            for (int i = 0; i < headerList.Count; i++)
            {
                headerList[i].Save(stream);
            }

        }

        /// <summary>
        /// Gets the first Chunk of type T and returns it, null if the
        /// specified chunk doesn't exist.
        /// </summary>
        /// <typeparam name="T">A IChunkType derived chunk.</typeparam>
        /// <returns></returns>
        public T GetSingleChunk<T>()
        {
            foreach (IChunkType chunk in _chunkList)
            {
                if (chunk is T)
                    return (T)chunk;
            }

            return default(T);
        }
        
        /// <summary>
        /// Returns all Chunks of type T, or a list with a length of zero
        /// if the specified chunk doesn't exist.
        /// </summary>
        /// <typeparam name="T">A IChunkType derived chunk.</typeparam>
        /// <returns></returns>
        public List<T> GetAllChunks<T>()
        {
            List<T> returnList = new List<T>();
            foreach (IChunkType chunk in _chunkList)
            {
                if (chunk is T)
                    returnList.Add((T)chunk);
            }

            return returnList;
        }
    }

    public class DZSHeader
    {
        public int ChunkCount;

        public DZSHeader()
        {
            ChunkCount = 0;
        }

        public void Load(byte[] data, ref int srcOffset)
        {
            ChunkCount = (int)Helpers.Read32(data, srcOffset);
            srcOffset += 4;
        }

        public void Save(BinaryWriter stream)
        {
            FSHelpers.Write32(stream, ChunkCount);
        }
    }

    public class DZSChunkHeader
    {
        public string Tag;              //ASCI Name for Chunk
        public int ElementCount;     //How many elements of this Chunk type
        public int ChunkOffset;      //Offset from beginning of file to first element

        public DZSChunkHeader()
        {
            Tag = "OOPS"; //For chunks someone forgot to name
            ElementCount = 0;
            ChunkOffset = 0;
        }

        public void Load(byte[] data, ref int srcOffset)
        {
            Tag = Helpers.ReadString(data, srcOffset, 4); //Tag is 4 bytes in length.
            ElementCount = (int)Helpers.Read32(data, srcOffset + 4);
            ChunkOffset = (int)Helpers.Read32(data, srcOffset + 8);

            srcOffset += 12; //Header is 0xC/12 bytes in length
        }

        public void Save(BinaryWriter stream)
        {
            FSHelpers.WriteString(stream, Tag, 4);
            FSHelpers.Write32(stream, ElementCount);
            FSHelpers.Write32(stream, ChunkOffset);
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
            FSHelpers.Write16(stream, (ushort) Rotation.X);
            FSHelpers.Write16(stream, (ushort) Rotation.Y);
            FSHelpers.Write16(stream, (ushort) Rotation.Z);

            FSHelpers.WriteArray(stream, FSHelpers.ToBytes(0xFFFF, 2));
        }
    }


    ///<summary>
    ///RPAT and Path are two chunks that put RPPN and PPNT chunk entries into groups.
    ///RPAT and RPPN are found in DZR files, while Path and PPNT are found in DZS files.
    ///</summary>
    public class RPATChunk : IChunkType
    {
        public ushort NumPoints;
        public ushort Unknown1; //Probably padding
        public byte Unknown2; //More padding?
        public byte Unknown3; //Possibly not padding
        public ushort Padding; //ACTUAL PADDING!?
        public int FirstPointOffset; //Offset in the DZx file of the first waypoint in the group

        public RPATChunk()
        {
            NumPoints = 0;
            Unknown1 = BitConverter.ToUInt16(FSHelpers.ToBytes(0xFFFF, 2), 0);
            Unknown2 = 0xFF;
            Unknown3 = 0;
            Padding = 0;
            FirstPointOffset = 0;
        }

        public void LoadData(byte[] data, ref int srcOffset)
        {
            NumPoints = Helpers.Read16(data, srcOffset);
            Unknown1 = Helpers.Read16(data, srcOffset + 2);
            Unknown2 = Helpers.Read8(data, srcOffset + 4);
            Unknown3 = Helpers.Read8(data, srcOffset + 5);
            Padding = Helpers.Read16(data, srcOffset + 6);
            FirstPointOffset = (int)Helpers.Read32(data, srcOffset + 8);

            srcOffset += 12;
        }

        public void WriteData(BinaryWriter stream)
        {
            FSHelpers.Write16(stream, NumPoints);
            FSHelpers.Write16(stream, Unknown1);
            FSHelpers.Write8(stream, Unknown2);
            FSHelpers.Write8(stream, Unknown3);
            FSHelpers.Write16(stream, Padding);
            FSHelpers.Write32(stream, FirstPointOffset);
        }
    }

    public class SondChunk : IChunkType
    {
        public string Name; //Seems to always be "sndpath"
        public Vector3 SourcePos; //Position the sound plays from
        public byte Unknown1; //Typically 00, one example had 08.
        public byte Padding;
        public byte Unknown2; //Typically FF, but Outset's entries have the room number (0x2C) here
        public byte SoundId;
        public byte SoundRadius;
        public byte Padding2;
        public byte Padding3;
        public byte Padding4;

        public void LoadData(byte[] data, ref int srcOffset)
        {
            Name = Helpers.ReadString(data, srcOffset, 8);
            SourcePos.X = Helpers.ConvertIEEE754Float(Helpers.Read32(data, srcOffset + 8));
            SourcePos.Y = Helpers.ConvertIEEE754Float(Helpers.Read32(data, srcOffset + 12));
            SourcePos.Z = Helpers.ConvertIEEE754Float(Helpers.Read32(data, srcOffset + 16));
            Unknown1 = Helpers.Read8(data, srcOffset + 20);
            Padding = Helpers.Read8(data, srcOffset + 21);
            Unknown2 = Helpers.Read8(data, srcOffset + 22);
            SoundId = Helpers.Read8(data, srcOffset + 23);
            SoundRadius = Helpers.Read8(data, srcOffset + 24);
            Padding2 = Helpers.Read8(data, srcOffset + 25);
            Padding3 = Helpers.Read8(data, srcOffset + 26);
            Padding4 = Helpers.Read8(data, srcOffset + 27);

            srcOffset += 28;
        }

        public void WriteData(BinaryWriter stream)
        {
            FSHelpers.WriteString(stream, Name, 8);
            FSHelpers.WriteFloat(stream, SourcePos.X);
            FSHelpers.WriteFloat(stream, SourcePos.Y);
            FSHelpers.WriteFloat(stream, SourcePos.Z);
            FSHelpers.Write8(stream, Unknown1);
            FSHelpers.Write8(stream, Padding);
            FSHelpers.Write8(stream, Unknown2);
            FSHelpers.Write8(stream, SoundId);
            FSHelpers.Write8(stream, SoundRadius);
            FSHelpers.Write8(stream, Padding2);
            FSHelpers.Write8(stream, Padding3);
            FSHelpers.Write8(stream, Padding4);
        }   
    }

    public class FlorChunk : IChunkType
    {
        public float LowerBoundaryYCoord; //Y value of the lower boundary of a floor. When link crosses the coord, the map switches him to being on that floor.
        public byte FloorId; //????
        public byte[] IncludedRooms;

        public void LoadData(byte[] data, ref int srcOffset)
        {
            LowerBoundaryYCoord = Helpers.ConvertIEEE754Float(Helpers.Read32(data, srcOffset));
            FloorId = Helpers.Read8(data, srcOffset + 4);
            IncludedRooms = new byte[15];
            for (int i = 0; i < 15; i++)
                IncludedRooms[i] = Helpers.Read8(data, srcOffset + 5 + i);

            srcOffset += 20;
        }

        public void WriteData(BinaryWriter stream)
        {
            FSHelpers.WriteFloat(stream, LowerBoundaryYCoord);
            FSHelpers.Write8(stream, FloorId);

            for (int i = 0; i < 15; i++)
                FSHelpers.Write8(stream, IncludedRooms[i]);
        }
    }

    public class FiliChunk : IChunkType
    {
        public byte TimePassage;
        public byte WindSettings;
        public byte Unknown1;
        public byte LightingType; //04 is normal, 05 is shadowed.
        public float Unknown2; 

        public void LoadData(byte[] data, ref int srcOffset)
        {
            TimePassage = Helpers.Read8(data, srcOffset + 0);
            WindSettings = Helpers.Read8(data, srcOffset + 1);
            Unknown1 = Helpers.Read8(data, srcOffset + 2);
            LightingType = Helpers.Read8(data, srcOffset + 3);

            Unknown2 = Helpers.ConvertIEEE754Float(Helpers.Read32(data, srcOffset + 4));
            srcOffset += 8;
        }

        public void WriteData(BinaryWriter stream)
        {
            FSHelpers.Write8(stream, TimePassage);
            FSHelpers.Write8(stream, WindSettings);
            FSHelpers.Write8(stream, Unknown1);
            FSHelpers.Write8(stream, LightingType);
            FSHelpers.WriteFloat(stream, Unknown2);
        }
    }

    public class RcamChunk : IChunkType
    {
        public string CameraType;
        public int Padding;

        public void LoadData(byte[] data, ref int srcOffset)
        {
            CameraType = Helpers.ReadString(data, srcOffset, 16);
            Padding = (int) Helpers.Read32(data, srcOffset + 16);

            srcOffset += 20;
        }


        public void WriteData(BinaryWriter stream)
        {
            FSHelpers.WriteString(stream, CameraType, 16);
            FSHelpers.Write32(stream, Padding);
        } 
    }

    public class MecoChunk : IChunkType
    {
        public byte RoomNumber; //Which room number this applies to
        public byte MemaIndex;  //Which index in the Mema array to use.

        public void LoadData(byte[] data, ref int srcOffset)
        {
            RoomNumber = Helpers.Read8(data, srcOffset);
            MemaIndex = Helpers.Read8(data, srcOffset + 1);

            srcOffset += 2;
        }

        public void WriteData(BinaryWriter stream)
        {
            FSHelpers.Write8(stream, RoomNumber);
            FSHelpers.Write8(stream, MemaIndex);
        }

    }

    public class MemaChunk : IChunkType
    {
        public uint MemSize; //Amount of memory to allocate for a room.

        public void LoadData(byte[] data, ref int srcOffset)
        {
            MemSize = Helpers.Read32(data, srcOffset);
            srcOffset += 4;
        }

        public void WriteData(BinaryWriter stream)
        {
            FSHelpers.Write32(stream, (int)MemSize);
        }
    }

    public class TresChunk : IChunkType
    {
        public string Name; //Usually Takara, 8 bytes + null terminator.
        public ushort ChestType; //Big Key, Common Wooden, etc.
        public Vector3 Position;
        public ushort Unknown;
        public ushort YRotation; //Rotation on the Y axis
        public byte ChestContents; //Rupees, Hookshot, etc.

        public void LoadData(byte[] data, ref int srcOffset)
        {
            Name = Helpers.ReadString(data, srcOffset, 8);
            ChestType = Helpers.Read16(data, srcOffset + 9);
            Position.X = Helpers.ConvertIEEE754Float(Helpers.Read32(data, srcOffset + 11));
            Position.Y = Helpers.ConvertIEEE754Float(Helpers.Read32(data, srcOffset + 15));
            Position.Z = Helpers.ConvertIEEE754Float(Helpers.Read32(data, srcOffset + 19));
            Unknown = Helpers.Read16(data, srcOffset + 23);
            YRotation = Helpers.Read16(data, srcOffset + 25);
            ChestContents = Helpers.Read8(data, srcOffset + 27);

            srcOffset += 28;
        }

        public void WriteData(BinaryWriter stream)
        {
            FSHelpers.WriteString(stream, Name, 8);
            FSHelpers.Write8(stream, 0xFF);
            FSHelpers.Write16(stream, ChestType);
            FSHelpers.WriteFloat(stream, Position.X);
            FSHelpers.WriteFloat(stream, Position.Y);
            FSHelpers.WriteFloat(stream, Position.Z);
            FSHelpers.Write16(stream, Unknown);
            FSHelpers.Write16(stream, YRotation);
            FSHelpers.Write8(stream, ChestContents);
        }
    }

    public class ShipChunk : IChunkType
    {
        public Vector3 Position;
        public uint Unknown;

        public void LoadData(byte[] data, ref int srcOffset)
        {
            Position.X = Helpers.ConvertIEEE754Float(Helpers.Read32(data, srcOffset + 0));
            Position.Y = Helpers.ConvertIEEE754Float(Helpers.Read32(data, srcOffset + 4));
            Position.Z = Helpers.ConvertIEEE754Float(Helpers.Read32(data, srcOffset + 8));

            Unknown = Helpers.Read32(data, srcOffset + 12);

            srcOffset += 12;
        }

        public void WriteData(BinaryWriter stream)
        {
            FSHelpers.WriteFloat(stream, Position.X);
            FSHelpers.WriteFloat(stream, Position.Y);
            FSHelpers.WriteFloat(stream, Position.Z);

            FSHelpers.Write32(stream, (int)Unknown);
        }
    }


    public class RppnChunk : IChunkType
    {
        public uint Unknown;
        public Vector3 Position;

        public void LoadData(byte[] data, ref int srcOffset)
        {
            Unknown = Helpers.Read32(data, srcOffset);
            Position.X = Helpers.ConvertIEEE754Float(Helpers.Read32(data, srcOffset + 4));
            Position.Y = Helpers.ConvertIEEE754Float(Helpers.Read32(data, srcOffset + 8));
            Position.Z = Helpers.ConvertIEEE754Float(Helpers.Read32(data, srcOffset + 12));

            srcOffset += 16;
        }

        public void WriteData(BinaryWriter stream)
        {
            FSHelpers.Write32(stream, (int)Unknown);

            FSHelpers.WriteFloat(stream, Position.X);
            FSHelpers.WriteFloat(stream, Position.Y);
            FSHelpers.WriteFloat(stream, Position.Z);
        }
    }

    public class MultChunk : IChunkType
    {
        public float TranslationX;
        public float TranslationY;
        public ushort YRotation;
        public byte RoomNumber;
        public byte Unknown;

        public void LoadData(byte[] data, ref int srcOffset)
        {
            TranslationX = Helpers.ConvertIEEE754Float(Helpers.Read32(data, srcOffset + 0));
            TranslationY = Helpers.ConvertIEEE754Float(Helpers.Read32(data, srcOffset + 4));
            YRotation = Helpers.Read16(data, srcOffset + 8);
            RoomNumber = Helpers.Read8(data, srcOffset + 10);
            Unknown = Helpers.Read8(data, srcOffset + 11);


            srcOffset += 12;
        }

        public void WriteData(BinaryWriter stream)
        {
            FSHelpers.WriteFloat(stream, TranslationX);
            FSHelpers.WriteFloat(stream, TranslationY);
            FSHelpers.Write16(stream, YRotation);
            FSHelpers.Write8(stream, RoomNumber);
            FSHelpers.Write8(stream, Unknown);
        }
    }

    public class LghtChunk : IChunkType
    {
        public Vector3 Position;
        public Vector3 Scale; //Or Intensity
        public ByteColorAlpha Color;

        public void LoadData(byte[] data, ref int srcOffset)
        {
            Position.X = Helpers.ConvertIEEE754Float(Helpers.Read32(data, srcOffset + 0));
            Position.Y = Helpers.ConvertIEEE754Float(Helpers.Read32(data, srcOffset + 4));
            Position.Z = Helpers.ConvertIEEE754Float(Helpers.Read32(data, srcOffset + 8));

            Scale.X = Helpers.ConvertIEEE754Float(Helpers.Read32(data, srcOffset + 12));
            Scale.Y = Helpers.ConvertIEEE754Float(Helpers.Read32(data, srcOffset + 16));
            Scale.Z = Helpers.ConvertIEEE754Float(Helpers.Read32(data, srcOffset + 20));

            srcOffset += 24;
            Color = new ByteColorAlpha(data, ref srcOffset);
        }

        public void WriteData(BinaryWriter stream)
        {
            FSHelpers.WriteFloat(stream, Position.X);
            FSHelpers.WriteFloat(stream, Position.Y);
            FSHelpers.WriteFloat(stream, Position.Z);

            FSHelpers.WriteFloat(stream, Scale.X);
            FSHelpers.WriteFloat(stream, Scale.Y);
            FSHelpers.WriteFloat(stream, Scale.Z);

            FSHelpers.WriteArray(stream, Color.GetBytes());
        }
    }

    public class RaroChunk : IChunkType
    {
        public Vector3 Position;
        public byte[] Unknown; //Always seems to be 00 00 80 00 00 00 FF FF

        public void LoadData(byte[] data, ref int srcOffset)
        {
            Position.X = Helpers.ConvertIEEE754Float(Helpers.Read32(data, srcOffset + 0));
            Position.Y = Helpers.ConvertIEEE754Float(Helpers.Read32(data, srcOffset + 4));
            Position.Z = Helpers.ConvertIEEE754Float(Helpers.Read32(data, srcOffset + 8));

            Unknown = new byte[8];
            for (int i = 0; i < 8; i++)
                Unknown[i] = Helpers.Read8(data, srcOffset + 12 + i);

            srcOffset += 20;
        }

        public void WriteData(BinaryWriter stream)
        {
            FSHelpers.WriteFloat(stream, Position.X);
            FSHelpers.WriteFloat(stream, Position.Y);
            FSHelpers.WriteFloat(stream, Position.Z);

            for (int i = 0; i < 8; i++)
                FSHelpers.Write8(stream, Unknown[i]);
        }
    }

    public class EvntChunk : IChunkType
    {
        public byte Unknown;
        public String EventName;
        public byte Unknown2;
        public byte Unknown3;
        public byte Unknown4;
        public byte Unknown5;
        public byte RoomNumber;
        public byte Unknown6;
        public byte Unknown7;
        public byte Unknown8;

        public void LoadData(byte[] data, ref int srcOffset)
        {
            Unknown = Helpers.Read8(data, srcOffset);
            EventName = Helpers.ReadString(data, srcOffset + 1, 15);
            Unknown2 = Helpers.Read8(data, srcOffset + 16);
            Unknown3 = Helpers.Read8(data, srcOffset + 17);
            Unknown4 = Helpers.Read8(data, srcOffset + 18);
            Unknown5 = Helpers.Read8(data, srcOffset + 19);
            RoomNumber = Helpers.Read8(data, srcOffset + 20);
            Unknown6 = Helpers.Read8(data, srcOffset + 21);
            Unknown7 = Helpers.Read8(data, srcOffset + 22);
            Unknown8 = Helpers.Read8(data, srcOffset + 23);

            srcOffset += 24;
        }

        public void WriteData(BinaryWriter stream)
        {
            FSHelpers.Write8(stream, Unknown);
            FSHelpers.WriteString(stream, EventName, 15);
            FSHelpers.Write8(stream, Unknown2);
            FSHelpers.Write8(stream, Unknown3);
            FSHelpers.Write8(stream, Unknown4);
            FSHelpers.Write8(stream, Unknown5);

            FSHelpers.Write8(stream, RoomNumber);

            FSHelpers.Write8(stream, Unknown6);
            FSHelpers.Write8(stream, Unknown7);
            FSHelpers.Write8(stream, Unknown8);
        }
    }

    public class ActrChunk : IChunkType
    {
        public string Name;
        public byte Unknown1;
        public byte RpatIndex;
        public byte Unknown2;
        public byte BehaviorType;
        public Vector3 Position;
        public HalfRotation Rotation;

        public ushort EnemyNumber;
            //Unknown purpose. Enemies are given a number here based on their position in the actor list.

        public void LoadData(byte[] data, ref int srcOffset)
        {
            Name = Helpers.ReadString(data, srcOffset, 8);
            Unknown1 = Helpers.Read8(data, srcOffset + 8);
            RpatIndex = Helpers.Read8(data, srcOffset + 9);
            Unknown2 = Helpers.Read8(data, srcOffset + 10);
            BehaviorType = Helpers.Read8(data, srcOffset + 11);

            Position.X = Helpers.ConvertIEEE754Float(Helpers.Read32(data, srcOffset + 12));
            Position.Y = Helpers.ConvertIEEE754Float(Helpers.Read32(data, srcOffset + 16));
            Position.Z = Helpers.ConvertIEEE754Float(Helpers.Read32(data, srcOffset + 20));

            srcOffset += 24;
            Rotation = new HalfRotation(data, ref srcOffset);

            EnemyNumber = Helpers.Read16(data, srcOffset + 30);

            srcOffset += 2; //Already got +24 from earlier, then +6 from HalfRotation.
        }

        public void WriteData(BinaryWriter stream)
        {
            FSHelpers.WriteString(stream, Name, 8);
            FSHelpers.Write8(stream, Unknown1);
            FSHelpers.Write8(stream, RpatIndex);
            FSHelpers.Write8(stream, Unknown2);
            FSHelpers.Write8(stream, BehaviorType);
            FSHelpers.WriteFloat(stream, Position.X);
            FSHelpers.WriteFloat(stream, Position.Y);
            FSHelpers.WriteFloat(stream, Position.Z);

            FSHelpers.Write16(stream, (ushort)Rotation.X);
            FSHelpers.Write16(stream, (ushort)Rotation.Y);
            FSHelpers.Write16(stream, (ushort)Rotation.Z);

            FSHelpers.Write16(stream, EnemyNumber);
        }
    }

    public class StagChunk : IChunkType
    {
        public float MinDepth;
        public float MaxDepth;
        public ushort KeyCounterDisplay; //Seems to be a multi-use field?
        public ushort LoadedParticleBank; //Particle Bank to load for the worldspace. Unclear how this works exactly.
        public ushort ItemUsageAndMinimap; //Items link can use and what color the minimap backgorund is
        public byte Padding;
        public byte Unknown1;
        public byte Unknown2;
        public byte Unknown3;
        public ushort DrawDistance;

        public void LoadData(byte[] data, ref int srcOffset)
        {
            MinDepth = Helpers.ConvertIEEE754Float(Helpers.Read32(data, srcOffset));
            MaxDepth = Helpers.ConvertIEEE754Float(Helpers.Read32(data, srcOffset + 4));
            KeyCounterDisplay = Helpers.Read16(data, srcOffset + 8);
            LoadedParticleBank = Helpers.Read16(data, srcOffset + 10);
            ItemUsageAndMinimap = Helpers.Read16(data, srcOffset + 12);
            Padding = Helpers.Read8(data, srcOffset + 14);
            Unknown1 = Helpers.Read8(data, srcOffset + 15);
            Unknown2 = Helpers.Read8(data, srcOffset + 16);
            Unknown3 = Helpers.Read8(data, srcOffset + 17);
            DrawDistance = Helpers.Read16(data, srcOffset + 18);

            srcOffset += 20;
        }

        public void WriteData(BinaryWriter stream)
        {
            FSHelpers.WriteFloat(stream, MinDepth);
            FSHelpers.WriteFloat(stream, MaxDepth);
            FSHelpers.Write16(stream, KeyCounterDisplay);
            FSHelpers.Write16(stream, LoadedParticleBank);
            FSHelpers.Write16(stream, ItemUsageAndMinimap);
            FSHelpers.Write8(stream, Padding);
            FSHelpers.Write8(stream, Unknown1);
            FSHelpers.Write8(stream, Unknown2);
            FSHelpers.Write8(stream, Unknown3);
            FSHelpers.Write16(stream, DrawDistance);
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
