using System;
using System.IO;
using System.Linq;

namespace EPF
{
    public class LZWDecompressor
    {
        #region Private Fields

        private readonly Int32 DICT_SIZE;
        private readonly Int32 STACK_SIZE;

        private readonly Int32[] _LZW_Char;
        private readonly Int32[] _LZW_Dict;
        private readonly byte[] _LZW_Stack;
        private readonly Int32 _maxBits;

        #endregion Private Fields

        #region Public Constructors

        public LZWDecompressor(Int32 maxBits)
        {
            _maxBits = maxBits;

            DICT_SIZE = 2 << _maxBits - 1;
            STACK_SIZE = 1024;

            _LZW_Char = new Int32[DICT_SIZE];
            _LZW_Dict = new Int32[DICT_SIZE];
            _LZW_Stack = new byte[STACK_SIZE];
        }

        #endregion Public Constructors

        #region Public Methods

        public void Decompress(Stream input, Stream output)
        {
            ResetDictionary();
            int curpos = 256;
            //Decompress input stream to output stream
            int bitlen = 9;
            //int p = 0;
            int c = 0;

            var oldOutputPos = output.Position;

            int change_bits = (2 << (bitlen - 1)) - 2;
            int finish_cw = (2 << (bitlen - 1)) - 1;

            int counter = 0;
            Int32 prevCode = 0;
            Int32 currCode = (Int32)ReadBits(input, counter, bitlen);
            counter += bitlen;

            c = GetCode(c);
            OutputLZW(output, _LZW_Dict[currCode], _LZW_Char[currCode]);

            Int32 current_size = (Int32)(output.Position - oldOutputPos);
            while (curpos != finish_cw && current_size < output.Length)
            {
                prevCode = currCode;

                currCode = (Int32)ReadBits(input, counter, bitlen);
                counter += bitlen;

                //Reset dictionary
                if (currCode == change_bits)
                {
                    ResetDictionary();
                    curpos = 256;
                }
                else if (curpos != finish_cw)
                {
                    //Check if codeword is in dictionary
                    if (ContainsCode(currCode))
                    {
                        c = GetCode(currCode);
                        OutputLZW(output, _LZW_Dict[currCode], _LZW_Char[currCode]);
                    }
                    else
                    {
                        c = GetCode(prevCode);
                        OutputLZW(output, prevCode, c);
                    }

                    if (curpos < DICT_SIZE)
                    {
                        if (prevCode != change_bits)
                        {
                            //m_Dict.Insert(Bytes(1,c),p);
                            _LZW_Dict[curpos] = prevCode;
                            _LZW_Char[curpos] = c;
                            curpos++;
                        }

                        //Incress bitlength
                        if (curpos == change_bits)
                        {
                            if (bitlen < 14)
                            {
                                bitlen++;
                                change_bits = (2 << (bitlen - 1)) - 2;
                                finish_cw = (2 << (bitlen - 1)) - 1;
                            }
                        }
                    }

                    current_size = (Int32)(output.Position - oldOutputPos);
                }
            }
        }

        #endregion Public Methods

        #region Private Methods

        private bool ContainsCode(Int32 codeword)
        {
            return _LZW_Char[codeword] != -1;
        }

        private Int32 GetCode(Int32 code)
        {
            int tries = 100;

            Int32 pc = code;
            Int32 c = code;
            while (_LZW_Dict[c] != -1)
            {
                pc = c;
                c = _LZW_Dict[c];

                if (pc == c)
                {
                    tries--;
                    if (tries == 0)
                        throw new InvalidOperationException("LZW data decompression error");
                }
                else
                    tries = 100;
            }
            return _LZW_Char[c];
        }

        private void OutputLZW(Stream stream, int dic, int ch)
        {
            int x = 1;
            int max = STACK_SIZE - 1;
            int sp = max;

            while (x != 0)
            {
                if (sp < 0)
                    throw new Exception("LZW: Stack Overflow!");

                _LZW_Stack[sp] = (byte)ch;
                sp--;

                if (dic != -1)
                {
                    ch = _LZW_Char[dic];
                    dic = _LZW_Dict[dic];
                }
                else
                    x = 0;
            }

            stream.Write(_LZW_Stack, sp + 1, max - sp);
        }

        //Valid only if bits_no <= 24
        private UInt32 ReadBits(Stream source, int bit_index, int bits_no)
        {
            var oldPos = source.Position;

            source.Position += bit_index / 8;

            byte[] value = new byte[4];
            source.Read(value, 0, 4);
            value = value.Reverse().ToArray();

            UInt32 res_bits = BitConverter.ToUInt32(value, 0);
            //res_bits = FlipEndian(res_bits);
            res_bits = res_bits << (bit_index % 8);
            res_bits = res_bits >> (32 - bits_no);

            source.Position = oldPos;

            return res_bits;
        }

        private void ResetDictionary()
        {
            //Fill dictionary with starting values
            for (int i = 0; i < DICT_SIZE; i++)
            {
                _LZW_Dict[i] = -1;
                if (i < 256)
                    _LZW_Char[i] = i;
                else
                    _LZW_Char[i] = -1;
            }

            //m_Dict.Clear();
            //for(int x = 0;x<256;x++)
            //{
            //    m_Dict.Insert(Bytes(1,x),m_Dict.Size());
            //}
        }

        #endregion Private Methods
    }
}