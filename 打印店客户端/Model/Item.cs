using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace 打印店客户端.Model
{
    class Item
    {
        public String filename { set; get; }
        public String fileMD5 { set; get; }
        public String fileType { set; get; }
        public String pages { set; get; }
        public int subtotal { set; get; }
        public printSettings printSettings { set; get; }
    }

    class printSettings
    {
        public int amount { set; get; }
        public String paperSize { set; get; }
        public Boolean isTwoSides { set; get; }
        public Boolean isColor { set; get; }
        public int pptPerPage { set; get; }
        public String direction { set; get; }
        public String remark { set; get; }
    }
}
