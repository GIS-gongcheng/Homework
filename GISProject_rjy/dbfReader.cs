using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace GISProject_rjy
{
    // .dbf 文件的文件头信息类
    internal class DBFHeader
    {
        public const int DBFHeaderSize = 32;
        /* 版本标志
                 0x02    FoxBASE  
                0x03    FoxBASE+/dBASE III PLUS，无备注  
                0x30    Visual FoxPro  
                0x43    dBASE IV SQL 表文件，无备注  
                0x63    dBASE IV SQL 系统文件，无备注  
                0x83    FoxBASE+/dBASE III PLUS，有备注  
                0x8B    dBASE IV 有备注  
                0xCB    dBASE IV SQL 表文件，有备注  
                0xF5    FoxPro 2.x（或更早版本）有备注  
                0xFB    FoxBASE  */
        public sbyte Version;
        public byte LastModifyYear; //最后更新年
        public byte LastModifyMonth; //最后更新月
        public byte LastModifyDay; //最后更新日
        public uint RecordCount; //文件包含总记录数
        public ushort HeaderLength; //第一条记录的偏移值 这个值也可以表示文件头长度
        public ushort RecordLength; //记录长度 包括删除标志
        public byte[] Reserved = new byte[16]; //保留
        /* 表的标志
                0x01具有 .cdx 结构的文件
                0x02文件包含备注
                0x04文件是数据库（.dbc） 
                标志可OR */
        public sbyte TableFlag;
        public sbyte CodePageFlag; //代码页标志
        public byte[] Reserved2 = new byte[2]; //保留
    }

    // 字段类型
    internal class DBFField
    {
        public const int DBFFieldSize = 32;
        public byte[] Name = new byte[11]; //字段名称
        /* 字段类型 C - 字符型  
                Y - 货币型  
                N - 数值型  
                F - 浮点型  
                D - 日期型  
                T - 日期时间型  
                B - 双精度型  
                I - 整型  
                L - 逻辑型 
                M - 备注型  
                G - 通用型  
                C - 字符型（二进制） 
                M - 备注型（二进制） 
                P - 图片型  */
        public sbyte Type;
        public uint Offset; //字段偏移量
        public byte Length; //字段长度
        public byte Precision; //浮点数小数部分长度
        public byte[] Reserved = new byte[2]; //保留
        public sbyte DbaseivID; //dBASE IV work area id 
        public byte[] Reserved2 = new byte[10];
        public sbyte ProductionIndex;
    }

    public class dbfReader : IDisposable
    {
        #region 字段
        private const string MSG_OPEN_FILE_FAIL = "不能打开文件{0}";
        private string _fileName = string.Empty; //文件名
        private bool _isFileOpened; //文件是否打开
        private DBFField[] _dbfFields; //文件字段
        private DBFHeader _dbfHeader = null; //文件头
        private System.IO.FileStream _fileStream = null;
        private System.IO.BinaryReader _binaryReader = null;
        private byte[] _recordBuffer;
        private string _tableName = string.Empty; //表格名称
        private uint _fieldCount = 0; //字段数目
        private int _recordIndex = -1;
        private uint _recordCount = 0;

        #endregion

        #region 属性

        /// <summary>
        /// 获取是否记录指针已经移动到记录最前面
        /// </summary>
        public bool IsBOF
        {
            get
            {
                return (-1 == this._recordIndex);
            }
        }

        /// <summary>
        /// 获取是否记录指针已经移动到记录最后面
        /// </summary>
        public bool IsEOF
        {
            get
            {
                return ((uint)this._recordIndex == this._recordCount);
            }
        }

        #endregion

        //构造函数
        public dbfReader() { }

        /// <summary>
        /// 清理所有正在使用的资源。
        /// </summary>
        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                this._recordBuffer = null;
                this._dbfHeader = null;
                this._dbfFields = null;

                if (this._isFileOpened && null != this._fileStream)
                {
                    this._fileStream.Close();
                    this._binaryReader.Close();
                }
                this._fileStream = null;
                this._binaryReader = null;

                this._isFileOpened = false;
                this._fieldCount = 0;
                this._recordCount = 0;
                this._recordIndex = -1;
            }
        }

        #region 方法

        /// <summary>
        /// 打开dbf文件
        /// </summary>
        /// <param name="fileName"></param>
        /// <returns></returns>
        public bool Open(string fileName)
        {
            if (null != fileName)
                this._fileName = fileName;

            bool ret = false;
            try
            {
                if (!this.OpenFile()) // 不能打开dbf文件，抛出不能打开文件异常
                    ret = false;
                //throw new Exception(string.Format(MSG_OPEN_FILE_FAIL, this._fileName));

                ret = this.ReadFileHeader(); // 读取文件头信息
                if (ret) //读取所有字段信息
                    ret = this.ReadFields();
                // 分配记录缓冲区
                if (ret && null == this._recordBuffer)
                {
                    this._recordBuffer = new byte[this._dbfHeader.RecordLength];
                    if (null == this._recordBuffer)
                        ret = false;
                }
                // 如果打开文件或读取信息不成功，关闭dbf文件
                if (!ret)
                    this.Close();
            }
            catch (Exception e)
            {
                throw e;
            }
            this._recordIndex = -1; // 设置当前记录索引为
            return ret; // 返回打开文件并且读取信息的成功状态
        }

        /// <summary>
        /// 获取dbf表文件对应的DataSet
        /// </summary>
        /// <returns></returns>
        public System.Data.DataTable GetDataTable()
        {
            // 确保文件已经打开
            if (!this._isFileOpened || (this.IsBOF && this.IsEOF))
                return null;

            System.Data.DataTable dt = new System.Data.DataTable(this._tableName);
            try
            {
                // 添加表格列
                for (uint i = 0; i < this._fieldCount; i++)
                {
                    System.Data.DataColumn col = new System.Data.DataColumn();
                    string colText = string.Empty;
                    if (this.GetFieldName(i, ref colText)) // 获取并设置列标题
                    {
                        col.ColumnName = colText;
                        col.Caption = colText;
                    }
                    col.DataType = FieldTypeToColumnType(this._dbfFields[i].Type); // 设置列类型
                    dt.Columns.Add(col); // 添加列信息
                }
                // 添加所有的记录信息
                this.MoveFirst();
                while (!this.IsEOF)
                {
                    System.Data.DataRow row = dt.NewRow(); // 创建新记录行
                                                           // 循环获取所有字段信息，添加到新的记录行内
                    for (uint i = 0; i < this._fieldCount; i++)
                    {
                        string temp = string.Empty;
                        // 获取字段值成功后才添加到记录行中
                        if (this.GetFieldValue(i, ref temp))
                        {
                            // 如果获取的字段值为空，设置DataTable里字段值为DBNull
                            if (string.Empty != temp)
                                row[(int)i] = temp;
                            else
                                row[(int)i] = System.DBNull.Value;
                        }
                    }
                    dt.Rows.Add(row); // 添加记录行
                    this.MoveNext(); // 后移记录
                }
            }
            catch (Exception e)
            {
                throw e;
            }
            return dt;
        }

        /// <summary>
        /// 获取相应索引序号处的字段名称
        /// </summary>
        /// <param name="fieldIndex"></param>
        /// <param name="fieldName"></param>
        /// <returns></returns>
        public bool GetFieldName(uint fieldIndex, ref string fieldName)
        {
            // 确保文件已经打开
            if (!this._isFileOpened)
                return false;

            // 索引边界检查
            if (fieldIndex >= this._fieldCount)
            {
                fieldName = string.Empty;
                return false;
            }

            try
            {
                // 反解码
                fieldName = System.Text.Encoding.Default.GetString(this._dbfFields[fieldIndex].Name);
                //去掉末尾的空字符标志
                int i = fieldName.IndexOf('\0');
                if (i > 0)
                {
                    fieldName = fieldName.Substring(0, i);
                }
                return true;
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        /// <summary>
        /// 获取相应索引序号处的字段文本值
        /// </summary>
        /// <param name="fieldIndex"></param>
        /// <param name="fieldValue"></param>
        /// <returns></returns>
        public bool GetFieldValue(uint fieldIndex, ref string fieldValue)
        {
            // 安全性检查
            if (!this._isFileOpened || this.IsBOF || this.IsEOF || null == this._recordBuffer)
                return false;

            // 字段索引超过最大值
            if (fieldIndex >= this._fieldCount)
            {
                fieldValue = string.Empty;
                return false;
            }

            try
            {
                // 从记录缓冲区中获取对应字段的byte[]
                //uint offset = this._dbfFields[fieldIndex].Offset;
                uint offset = 0;
                if (offset == 0)
                {
                    for (int i = 0; i < fieldIndex; i++)
                    {
                        offset += this._dbfFields[i].Length;
                    }
                }

                byte[] tmp = GetSubBytes(this._recordBuffer, offset, this._dbfFields[fieldIndex].Length);

                //
                // 开始byte数组的反解码过程
                //
                if (((sbyte)'I') == this._dbfFields[fieldIndex].Type)
                {
                    // 整形字段的反解码过程
                    int num1 = Byte2Int32(tmp);
                    fieldValue = num1.ToString();
                }
                else if (((sbyte)'B') == this._dbfFields[fieldIndex].Type)
                {
                    // 双精度型字段的反解码过程
                    double num1 = Byte2Double(tmp);
                    fieldValue = num1.ToString();
                }
                else
                {
                    // 其他字段值与字符存储方式类似，直接反解码成字符串就可以
                    fieldValue = System.Text.Encoding.Default.GetString(tmp);
                }

                // 消除字段数值的首尾空格
                fieldValue = fieldValue.Trim();

                // 如果本子段类型是数值相关型，进一步处理字段值
                if (((sbyte)'N') == this._dbfFields[fieldIndex].Type ||    // N - 数值型
                    ((sbyte)'F') == this._dbfFields[fieldIndex].Type)    // F - 浮点型
                {
                    if (0 == fieldValue.Length)
                        // 字段值为空，设置为0
                        fieldValue = "0";
                    else if ("." == fieldValue)
                        // 字段值为"."，也设置为0
                        fieldValue = "0";
                    else
                    {
                        // 将字段值先转化为Decimal类型然后再转化为字符串型，消除类似“.000”的内容
                        // 如果不能转化则为0
                        try
                        {
                            fieldValue = System.Convert.ToDecimal(fieldValue).ToString();
                        }
                        catch (Exception)
                        {
                            fieldValue = "0";
                        }
                    }
                }
                return true;
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        /// <summary>
        /// 关闭dbf文件
        /// </summary>
        public void Close()
        {
            this.Dispose(true);
        }

        public void Dispose()
        {
            throw new NotImplementedException();
        }

        #endregion

        #region 私有函数

        /// <summary>
        /// 将字段类型转换为系统数据类型
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        private Type FieldTypeToColumnType(sbyte type)
        {
            switch (type)
            {
                // C - 字符型、字符型（二进制）
                case (sbyte)'C':
                    return typeof(System.String);

                // Y - 货币型
                case (sbyte)'Y':
                    return typeof(System.Decimal);    // 虽然dbf中'Y'长度为64位，但是Double的精度不够，所以指定Decimal

                // N - 数值型
                case (sbyte)'N':
                    return typeof(System.Decimal);    // dbf中'N'的精度可以达到19，所以用Decimal

                // F - 浮点型
                case (sbyte)'F':
                    return typeof(System.Decimal);    // dbf中'F'的精度可以达到19，所以用Decimal

                // D - 日期型
                case (sbyte)'D':
                    return typeof(System.DateTime);

                // T - 日期时间型
                case (sbyte)'T':
                    return typeof(System.DateTime);

                // B - 双精度型
                case (sbyte)'B':
                    return typeof(System.Double);

                // I - 整型
                case (sbyte)'I':
                    return typeof(System.Int32);

                // L - 逻辑型
                case (sbyte)'L':
                    return typeof(System.Boolean);

                // M - 备注型、备注型（二进制）
                case (sbyte)'M':
                    return typeof(System.String);

                // G - 通用型
                case (sbyte)'G':
                    return typeof(System.String);

                // P - 图片型
                case (sbyte)'P':
                    return typeof(System.String);

                // 缺省字符串型
                default:
                    return typeof(System.String);

            }
        }

        /// <summary>
        /// 将纪录指针移动到第一条记录
        /// </summary>
        private void MoveFirst()
        {
            // 确认目标文件已经打开
            if (!this._isFileOpened)
                return;

            if (this.IsBOF && this.IsEOF)
                return;

            // 重新设置当前记录的索引
            this._recordIndex = 0;

            try
            {
                // 读取当前记录信息
                ReadCurrentRecord();
            }
            catch (Exception e)
            {
                throw e;
            }

            return;
        }

        /// <summary>
        /// 将记录指针后移一个记录
        /// </summary>
        private void MoveNext()
        {
            // 确认目标文件已经打开
            if (!this._isFileOpened)
                return;

            if (this.IsEOF)
                return;

            // 重新设置当前记录的索引
            this._recordIndex += 1;

            try
            {
                // 读取当前记录信息
                ReadCurrentRecord();
            }
            catch (Exception e)
            {
                throw e;
            }

            return;
        }

        /// <summary>
        /// 读取当前记录信息
        /// </summary>
        private void ReadCurrentRecord()
        {
            // 确认目标文件已经打开
            if (!this._isFileOpened)
                return;
            if (this.IsBOF && this.IsEOF)
                return;

            try
            {
                this._fileStream.Seek(this._dbfHeader.HeaderLength + this._dbfHeader.RecordLength * this._recordIndex + 1, SeekOrigin.Begin);
                this._recordBuffer = this._binaryReader.ReadBytes(this._dbfHeader.RecordLength);
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        /// <summary>
        /// 获取buf的子数组
        /// </summary>
        /// <param name="buf"></param>
        /// <param name="startIndex"></param>
        /// <param name="length"></param>
        /// <returns></returns>
        private static byte[] GetSubBytes(byte[] buf, uint startIndex, long length)
        {
            // 参数检查
            if (null == buf)
            {
                throw new ArgumentNullException("buf");
            }
            if (startIndex >= buf.Length)
            {
                throw new ArgumentOutOfRangeException("startIndex");
            }
            if (0 == length)
            {
                throw new ArgumentOutOfRangeException("length", "参数length必须大于0");
            }
            if (length > buf.Length - startIndex)
            {
                // 子数组的长度超过从startIndex起到buf末尾的长度时，修正为剩余长度
                length = buf.Length - startIndex;
            }

            byte[] target = new byte[length];

            // 逐位复制
            for (uint i = 0; i < length; i++)
            {
                target[i] = buf[startIndex + i];
            }

            // 返回buf的子数组
            return target;
        }

        /// <summary>
        /// byte数组存储的数值转换为int32类型
        /// </summary>
        /// <param name="buf"></param>
        /// <returns></returns>
        private static int Byte2Int32(byte[] buf)
        {
            // 参数检查
            if (null == buf)
            {
                // 参数为空
                throw new ArgumentNullException("buf");
            }
            if (4 != buf.Length)
            {
                // 如果参数buf的长度不为4，抛出参数异常
                throw new ArgumentException("函数Byte2Int32(byte[])的参数必须是长度为4的有效byte数组", "buf");
            }

            // byte[] 解码成 int
            return (int)((((buf[0] & 0xff) | (buf[1] << 8)) | (buf[2] << 0x10)) | (buf[3] << 0x18));
        }

        /// <summary>
        /// byte数组存储的数值转换为int64类型
        /// </summary>
        /// <param name="buf"></param>
        /// <returns></returns>
        private static long Byte2Int64(byte[] buf)
        {
            // 参数检查
            if (null == buf)
            {
                // 参数为空
                throw new ArgumentNullException("buf");
            }
            if (8 != buf.Length)
            {
                // 如果参数buf的长度不为4，抛出参数异常
                throw new ArgumentException("函数Byte2Int64(byte[])的参数必须是长度为8的有效byte数组", "buf");
            }

            // byte[] 解码成 long
            uint num1 = (uint)(((buf[0] | (buf[1] << 8)) | (buf[2] << 0x10)) | (buf[3] << 0x18));
            uint num2 = (uint)(((buf[4] | (buf[5] << 8)) | (buf[6] << 0x10)) | (buf[7] << 0x18));

            return (long)(((ulong)num2 << 0x20) | num1);
        }

        /// <summary>
        /// byte数组存储的数值转换为double类型
        /// </summary>
        /// <param name="buf"></param>
        /// <returns></returns>
        private static double Byte2Double(byte[] buf)
        {
            // 参数检查
            if (null == buf)
            {
                // 参数为空
                throw new ArgumentNullException("buf");
            }
            if (8 != buf.Length)
            {
                // 如果参数buf的长度不为8，抛出参数异常
                throw new ArgumentException("函数Byte2Double(byte[])的参数必须是长度为8的有效byte数组", "buf");
            }

            double num1 = 0;

            return num1;
        }

        /// <summary>
        /// 打开dbf文件
        /// </summary>
        /// <returns></returns>
        private bool OpenFile()
        {
            // 如果文件已经打开，则先关闭然后重新打开
            if (this._isFileOpened)
                this.Close();
            // 校验文件名
            if (null == this._fileName || 0 == this._fileName.Length)
                return false;

            this._isFileOpened = false;
            try
            {
                // 打开dbf文件，获取文件流对象
                this._fileStream = File.Open(this._fileName, FileMode.Open, FileAccess.Read,
                    FileShare.Read);
                // 使用获取的文件流对象构造二进制读取器对象
                this._binaryReader = new BinaryReader(this._fileStream,
                    System.Text.Encoding.Default);
                this._isFileOpened = true;
                this._tableName = System.IO.Path.GetFileNameWithoutExtension(this._fileName);
            }
            catch (Exception e)
            {
                throw e;
            }
            return this._isFileOpened;
        }

        /// <summary>
        /// 从dbf文件中读取文件头信息
        /// </summary>
        /// <returns></returns>
        private bool ReadFileHeader()
        {
            if (!this._isFileOpened) // 确认目标文件已经打开
                return false;
            if (null == this._dbfHeader) // 尝试构造新的dbf文件头对象
                this._dbfHeader = new DBFHeader();
            try
            {
                this._dbfHeader.Version = this._binaryReader.ReadSByte();//第1字节
                this._dbfHeader.LastModifyYear = this._binaryReader.ReadByte();//第2字节
                this._dbfHeader.LastModifyMonth = this._binaryReader.ReadByte();//第3字节
                this._dbfHeader.LastModifyDay = this._binaryReader.ReadByte();//第4字节
                this._dbfHeader.RecordCount = this._binaryReader.ReadUInt32();//第5-8字节
                this._dbfHeader.HeaderLength = this._binaryReader.ReadUInt16();//第9-10字节
                this._dbfHeader.RecordLength = this._binaryReader.ReadUInt16();//第11-12字节
                this._dbfHeader.Reserved = this._binaryReader.ReadBytes(16);//第13-14字节
                this._dbfHeader.TableFlag = this._binaryReader.ReadSByte();//第15字节
                this._dbfHeader.CodePageFlag = this._binaryReader.ReadSByte();//第16字节
                this._dbfHeader.Reserved2 = this._binaryReader.ReadBytes(2);////第17-18字节
            }
            catch (Exception e)
            {
                throw e;
            }

            // 设置记录数目
            this._recordCount = this._dbfHeader.RecordCount;
            uint fieldCount = (uint)((this._dbfHeader.HeaderLength - DBFHeader.DBFHeaderSize - 1) / DBFField.DBFFieldSize);
            this._fieldCount = 0;

            // 由于有些dbf文件的文件头最后有附加区段，但是有些文件没有，在此使用笨方法计算字段数目
            // 就是测试每一个存储字段结构区域的第一个字节的值，如果不为0x0D，表示存在一个字段
            // 否则从此处开始不再存在字段信息
            try
            {
                for (uint i = 0; i < fieldCount; i++)
                {
                    // 定位到每个字段结构区，获取第一个字节的值
                    this._fileStream.Seek(DBFHeader.DBFHeaderSize + i * DBFField.DBFFieldSize,
                        SeekOrigin.Begin);
                    byte flag = this._binaryReader.ReadByte();
                    // 如果获取到的标志不为0x0D，则表示该字段存在；否则从此处开始后面再没有字段信息
                    if (0x0D != flag)
                        this._fieldCount++;
                    else
                        break;
                }
            }
            catch (Exception e)
            {
                throw e;
            }

            return true;
        }

        /// <summary>
        /// 从dbf文件中读取所有字段信息
        /// </summary>
        /// <returns></returns>
        private bool ReadFields()
        {
            if (!this._isFileOpened) // 确认目标文件已经打开
                return false;
            if (null == this._dbfHeader) // 必须存在文件头对象信息
                return false;
            if (null == this._dbfFields) // 尝试构造字段信息对象数组
                this._dbfFields = new DBFField[this._fieldCount];
            try
            {
                // 定位字段信息结构区起点
                this._fileStream.Seek(DBFHeader.DBFHeaderSize, SeekOrigin.Begin);
                // 读取所有字段信息
                for (int i = 0; i < this._fieldCount; i++)
                {
                    this._dbfFields[i] = new DBFField();
                    this._dbfFields[i].Name = this._binaryReader.ReadBytes(11);
                    this._dbfFields[i].Type = this._binaryReader.ReadSByte();
                    this._dbfFields[i].Offset = this._binaryReader.ReadUInt32();
                    this._dbfFields[i].Length = this._binaryReader.ReadByte();
                    this._dbfFields[i].Precision = this._binaryReader.ReadByte();
                    this._dbfFields[i].Reserved = this._binaryReader.ReadBytes(2);
                    this._dbfFields[i].DbaseivID = this._binaryReader.ReadSByte();
                    this._dbfFields[i].Reserved2 = this._binaryReader.ReadBytes(10);
                    this._dbfFields[i].ProductionIndex = this._binaryReader.ReadSByte();
                }
            }
            catch (Exception e)
            {
                throw e;
            }

            return true;
        }

        #endregion
    }

}
