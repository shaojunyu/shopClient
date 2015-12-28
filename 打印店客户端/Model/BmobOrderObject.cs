using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using cn.bmob.api;
using cn.bmob.io;

namespace 打印店客户端.Model
{
    class BmobOrderObject : BmobTable
    {
        private String fTable;

        //以下对应云端字段名称
        public String items { get; set; }
        public String state { get; set; }
        public BmobInt totalPrice { get; set; }
        public string username { get; set; }
        public string shop { get; set; }
        public string address { get; set; }
        public string phone { get; set; }
        //构造函数
        public BmobOrderObject() { }

        //构造函数
        public BmobOrderObject(String tableName)
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
            this.items = input.getString("items");
            this.state = input.getString("state");
            this.totalPrice = input.getInt("totalPrice");
            this.username = input.getString("username");
            this.shop = input.getString("shop");
            this.address = input.getString("address");
            this.phone = input.getString("phone");
        }

        //写字段信息
        public override void write(BmobOutput output, bool all)
        {
            base.write(output, all);

            output.Put("state",this.state);
            //output.Put("score", this.score);
            //output.Put("cheatMode", this.cheatMode);
            //output.Put("playerName", this.playerName);
        }
    }
}
