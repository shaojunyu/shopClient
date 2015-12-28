using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using cn.bmob.api;
using cn.bmob.io;

namespace 打印店客户端.Model
{
        class BmobShopObject : BmobTable
        {
            private String fTable;
            //以下对应云端字段名称
            public String shopName { get; set; }
            public String address { get; set; }
            public String cellphone { get; set; }
            public String password { get; set; }
            public String RCToken { get; set; }
            public String username { get; set; }
            //构造函数
            public BmobShopObject() { }

            //构造函数
            public BmobShopObject(String tableName)
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

                this.shopName = input.getString("shopName");
                this.address = input.getString("address");
                this.cellphone = input.getString("cellphone");
                this.password = input.getString("password");
                this.RCToken = input.getString("RCToken");
            }

            //写字段信息
            public override void write(BmobOutput output, bool all)
            {
                base.write(output, all);

                //output.Put("score", this.score);
                //output.Put("cheatMode", this.cheatMode);
                //output.Put("playerName", this.playerName);
            }
    }
}
