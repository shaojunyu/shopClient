using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using cn.bmob.api;
using cn.bmob.io;
using cn.bmob.tools;
using System.Diagnostics;
using cn.bmob.json;
using 打印店客户端.Model;
using System.Runtime.Serialization;

namespace 打印店客户端
{
    public partial class LoginForm : BmobBaseForm
    {
        //对应要操作的数据表
        public const String TABLE_NAME = "Shop";
        //接下来要操作的数据的数据
        private BmobShopObject shopObject = new BmobShopObject(TABLE_NAME);

        public LoginForm() : base()
        {
            InitializeComponent();
        }

        private void label1_Click(object sender, EventArgs e)
        {

        }

        private void LoginButton_Click(object sender, EventArgs e)
        {
            var query = new BmobQuery();
            query.WhereContainedIn<string>("cellphone",usernameTextBox.Text);
            query.WhereContainedIn<string>("password", passwordTextBox.Text);
            var future = Bmob.FindTaskAsync<BmobShopObject>(TABLE_NAME,query);
            //var r = future.Result;
            if (future.Result.results.Count == 1)
            {
                Console.WriteLine("登录成功");
                infoLable.Text = "登录成功";
                //显示详情窗口
                var infoForm = new InfoForm();
                infoForm.Text = future.Result.results[0].shopName;
                infoForm.Show();
                this.Hide();

                //数据保存到本地
                var shop = future.Result.results[0];
                String shopname = shop.shopName;

            }
            else {
                infoLable.Text = "用户名或密码错误！";
            }
            //Console.WriteLine(r);
            //string res = JsonAdapter.JSON.ToDebugJsonString(future.Result);
            //Console.WriteLine(res);
        }

        private void passwordTextBox_TextChanged(object sender, EventArgs e)
        {
            passwordTextBox.UseSystemPasswordChar = true;
        }

        private void LoginForm_Load(object sender, EventArgs e)
        {

        }
    }
}
