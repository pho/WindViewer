using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using WWActorEdit.Kazari;
using WWActorEdit.Kazari.DZB;
using WWActorEdit.Kazari.J3Dx;

namespace WWActorEdit.Source
{
    /// <summary>
    /// The .rarc / .arc format is a propietary Nintendo format that appears to
    /// be reused across multiple games. Not all games use all parts of the format
    /// (ie: Four Swords Adventures doesn't use filename hashes) so this 
    /// implementation is only enough for at least Wind Waker. YMMV when trying to
    /// use this code for archives in other 
    /// </summary>
    public class ZArchive
    {
        //This holds the actual data in memory for now
        public RARC Archive;

        //These are simply references for quickly getting to the data we want.
        //They will be NULL if the archive does not have the data.
        public DZSFormat RoomData; /* Room Entity Data */
        public DZSFormat StageData; /* Stage Entity Data */
        public DZB Collision;

        public List<J3Dx> Models;



        public ZArchive()
        {
            Archive = null;

        }
    }
}
