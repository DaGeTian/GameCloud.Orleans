// Copyright (c) Cragon. All rights reserved.

namespace GameCloud.Unity.Common
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Text;

    public abstract class EbData
    {
        //---------------------------------------------------------------------
        public int Id { get; internal set; }

        //---------------------------------------------------------------------
        public abstract void load(EbTableBuffer table_buf);
    }

    public class EbTableBuffer
    {
        //---------------------------------------------------------------------
        MemoryStream MemoryStream { get; set; }
        byte[] Buffer { get; set; }
        string mTableName;
        int mRecordCount;

        //---------------------------------------------------------------------
        public string TableName { get { return mTableName; } }

        //---------------------------------------------------------------------
        public EbTableBuffer(string tb_name)
        {
            MemoryStream = new MemoryStream();
            Buffer = new byte[1024];
            mTableName = tb_name;
            mRecordCount = 0;
        }

        //---------------------------------------------------------------------
        public EbTableBuffer(byte[] buf, string tb_name)
        {
            MemoryStream = new MemoryStream(buf);
            Buffer = new byte[1024];
            mTableName = tb_name;
            mRecordCount = 0;
        }

        //---------------------------------------------------------------------
        public void Close()
        {
            if (MemoryStream != null)
            {
                MemoryStream.Close();
                MemoryStream = null;
            }
            Buffer = null;
        }

        //---------------------------------------------------------------------
        public byte[] GetTableData()
        {
            return MemoryStream.ToArray();
        }

        //---------------------------------------------------------------------
        public void RecordCountIncrease()
        {
            mRecordCount++;
        }

        //---------------------------------------------------------------------
        public int GetRecordCount()
        {
            return mRecordCount;
        }

        //---------------------------------------------------------------------
        public void WriteInt(int value)
        {
            var data = BitConverter.GetBytes(value);
            MemoryStream.Write(data, 0, data.Length);
        }

        //---------------------------------------------------------------------
        public void WriteFloat(float value)
        {
            var data = BitConverter.GetBytes(value);
            MemoryStream.Write(data, 0, data.Length);
        }

        //---------------------------------------------------------------------
        public void WriteString(string value)
        {
            short str_len = 0;
            if (!string.IsNullOrEmpty(value))
            {
                str_len = (short)value.Length;
            }

            var data = BitConverter.GetBytes(str_len);
            MemoryStream.Write(data, 0, data.Length);

            if (str_len > 0)
            {
                byte[] data_str = System.Text.Encoding.UTF8.GetBytes(value);
                MemoryStream.Write(data_str, 0, data_str.Length);
            }
        }

        //---------------------------------------------------------------------
        public int ReadInt()
        {
            MemoryStream.Read(Buffer, 0, sizeof(int));
            return BitConverter.ToInt32(Buffer, 0);
        }

        //---------------------------------------------------------------------
        public float ReadFloat()
        {
            MemoryStream.Read(Buffer, 0, sizeof(float));
            return BitConverter.ToSingle(Buffer, 0);
        }

        //---------------------------------------------------------------------
        public string ReadString()
        {
            MemoryStream.Read(Buffer, 0, sizeof(short));
            short str_len = BitConverter.ToInt16(Buffer, 0);
            if (str_len > 0)
            {
                if (str_len > Buffer.Length)
                {
                    Buffer = new byte[str_len + 128];
                }

                MemoryStream.Read(Buffer, 0, str_len);

                return System.Text.Encoding.UTF8.GetString(Buffer, 0, (int)str_len);
            }
            else
            {
                return "";
            }
        }
    }
}
