using System;
using System.Collections.Generic;

namespace CloudLand
{
    public class ByteListStream : List<ArraySegment<byte>>
    {

        public byte[] getBytes(int len)
        {
            if (available() < len) return null;
            if (Count <= 0) return null;
            byte[] data = new byte[len];
            int offset = 0;
            while(offset < data.Length)
            {
                ArraySegment<byte> seg = this[0];
                if(offset + seg.Count > len)
                {
                    Buffer.BlockCopy(seg.Array, seg.Offset, data, offset, len - offset);
                    updateOffset(seg.Offset + (len - offset));
                    return data;
                } else
                {
                    Buffer.BlockCopy(seg.Array, seg.Offset, data, offset, seg.Count);
                    offset += seg.Count;
                    this.RemoveAt(0);
                }
            }
            // UnityEngine.Debug.Log("EXIT LOOP????!??!?!?!?? ");
            return data;
        }

        public int available()
        {
            int total = 0;
            foreach(ArraySegment<byte> seg in this) {
                total += seg.Count;
            }
            return total;
        }

        public void updateOffset(int offset)
        {
            ArraySegment<byte> seg = this[0];
            this.RemoveAt(0);
            ArraySegment<byte> item = new ArraySegment<byte>(seg.Array, offset, seg.Count - (offset - seg.Offset));
            this.Insert(0, item);
        }

    }
}
