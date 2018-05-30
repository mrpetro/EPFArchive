using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace EPF
{

    internal class BitArray
    {
        private List<byte> m_Data;
        private int m_BitsNo;

        public int Length { get { return m_BitsNo; } }

        public BitArray()
        {
            m_BitsNo = 0;
            m_Data = new List<byte>();
        }

        internal void Add(byte bit)
        {
            if(m_BitsNo>=(m_Data.Count*8))
                m_Data.Add(0);

            int dataPos = m_BitsNo / 8;
            int dataBitPos = m_BitsNo % 8;

            if(bit==1)
                m_Data[dataPos] += (byte)(1 << (8-dataBitPos-1));

            m_BitsNo++;
        }

        public byte[] ToArray()
        {
            return m_Data.ToArray();
        }
    };

    public class LZWCompressor
    {
        private Int32 m_MaxBits;

        //private Trie m_Dict;
        private Dictionary<string,int> m_Dict;

        public LZWCompressor(Int32 maxBits)
        {
            m_MaxBits = maxBits;

            //m_Dict = new Trie();
            m_Dict = new Dictionary<string,int>();
        }

        private void ResetDictionary()
        {
           m_Dict.Clear();

           for( int x = 0 ; x < 256 ; x++ )
           {
               var byteKey = new string((char)x, 1);
               m_Dict.Add(byteKey, m_Dict.Count);
           }
        }

        private bool AddToDictionary(string Entry, ref Int32 usebits)
        {
            if (m_Dict.Count < (Math.Pow(2, m_MaxBits) - 2))
           {
               m_Dict.Add(Entry, m_Dict.Count);
              if (m_Dict.Count == (Math.Pow(2, usebits) - 1))
                 usebits = Math.Min(usebits+1, m_MaxBits);

              return true;
           }
           else
              return false;
        }

        private void PutCode(Int32 Code, Int32 usebits, BitArray bits)
        {
            for(int b=usebits-1 ; b>=0;b--)
            {
                Int32 theBit = (Code >> b) & 1;  
                bits.Add((byte)theBit);
            }
        }

        private Int32 GetDictCode(string word)
        {
            Int32 code = -1;
            m_Dict.TryGetValue(word, out code);
            return code;
        }

        public void Compress(Stream input, Stream output)
        {
           Int32 usebits = 9;

           BitArray bits = new BitArray();

           string match = string.Empty;

           ResetDictionary();

           while(input.Position!=input.Length)
           {
              byte nbyte = (byte)input.ReadByte();

              string nmatch = match;
              nmatch += (char)nbyte;

              if (m_Dict.ContainsKey(nmatch))
              {
                 match = nmatch;
              }
              else
              {

                  PutCode(GetDictCode(match), usebits, bits);

                 if(!AddToDictionary(nmatch, ref usebits))
                 {
                    ResetDictionary();
                    //Output reset code
                    PutCode((2<<(usebits-1))-2,usebits,bits);
                 }

                 match = new string((char)nbyte, 1);
              }
           }

           PutCode(GetDictCode(match), usebits, bits);
           //Output finish code
           PutCode((2 << (usebits - 1)) - 1, usebits, bits);

           var array = bits.ToArray();
           output.Write(array, 0, array.Length);
        }
    }
}
