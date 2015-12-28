using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Diagnostics;
using cn.bmob.api;
using cn.bmob.io;
using cn.bmob.json;
using cn.bmob.tools;

namespace 打印店客户端
{
    public partial class BmobBaseForm : Form
    {
        //创建Bmob实例
        private BmobWindows bmob;

        public BmobBaseForm() : base()
        {
            bmob = new BmobWindows();

            //初始化ApplicationId，这个ApplicationId需要更改为你自己的ApplicationId（ http://www.bmob.cn 上注册登录之后，创建应用可获取到ApplicationId）
            Bmob.initialize("4c69d6ed89b6591d050f809d70930b2a", "bce9f6f7da21f79dd7f3d77c3b84d335");

            //注册调试工具
            BmobDebug.Register(msg => { Debug.WriteLine(msg); });

            //InitializeComponent();
        }

        public BmobWindows Bmob
        {
            get { return bmob; }
        }

        //对返回结果进行显示处理
        public void FinishedCallback<T>(T data, TextBox text)
        {
            text.Text = JsonAdapter.JSON.ToDebugJsonString(data);
        }

    }
}
