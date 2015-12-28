using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using cn.bmob.io;

namespace 打印店客户端.Model
{
    class BmobFileInfoObject : BmobTable
    {
        private String fTable;
        //以下对应云端字段名称
        public String fileMD5 { get; set; }
        public String filename { get; set; }
        public String path { get; set; }
        //构造函数
        public BmobFileInfoObject() {
            this.fTable = "File_Info";
        }

        //构造函数
        public BmobFileInfoObject(String tableName)
        {
            this.fTable = tableName;
        }

        public override string table
        {
            get
            {
                if (fTable != null)
                {
                    return fTable;
                }
                return base.table;
            }
        }

        //读字段信息
        public override void readFields(BmobInput input)
        {
            base.readFields(input);
            this.fileMD5 = input.getString("fileMD5");
            this.filename = input.getString("filename");
            this.path = input.getString("path");
        }

        //写字段信息
        public override void write(BmobOutput output, bool all)
        {
            base.write(output, all);
        }
    }
}
