using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace WWActorEdit.Source.FileFormats
{
    /// <summary>
    /// This is a generic implementation for files that have not had their file formats
    /// mapped out yet. It simply saves the data as a byte[] and then writes the same
    /// array back out to disk.
    /// </summary>
    class GenericData : BaseArchiveFile
    {
        private byte[] _data;

        public override void Load(byte[] data)
        {
            _data = data;
        }

        public override void Save(BinaryWriter stream)
        {
            stream.Write(_data);
        }
    }
}
