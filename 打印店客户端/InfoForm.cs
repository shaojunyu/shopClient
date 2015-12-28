using System;
using System.Windows.Forms;
using cn.bmob.io;
using cn.bmob.json;
using 打印店客户端.Model;
using Newtonsoft.Json;
using System.IO;
using Aliyun.OSS;
using Aliyun.OSS.Common;
using System.Web.Script.Serialization;
using System.Collections.Generic;
using Newtonsoft.Json.Linq;
using System.Linq;
using System.Runtime.Serialization.Json;
using System.Text;
using System.Threading;
using System.Security.Cryptography;
using rcsdk_test;
using System.Media;
using Word = Microsoft.Office.Interop.Word;

namespace 打印店客户端
{
    public partial class InfoForm : BmobBaseForm
    {
        private BmobShopObject shopObject = new BmobShopObject("Shop");
        private BmobOrderObject orderObject = new BmobOrderObject("Order");
        private OssClient ossClient;
        private string shopName;
        private string shopId;
        private string RCToken;//融云推送的token

        const string bucket = "99dayin";
        const string accessKeyId = "GtzMAvDTnxg72R04";
        const string accessKeySecret = "VhD2czcwLVAaE7DReDG4uEVSgtaSYK";
        const string endpoint = "oss-cn-hangzhou.aliyuncs.com";

        public InfoForm()
        {
            InitializeComponent();
        }

        private void InfoForm_Load(object sender, EventArgs e)
        {
            this.ossClient = new OssClient(endpoint, accessKeyId, accessKeySecret);
            this.shopName = this.Text;
            this.getShopInfo();
            this.initRongCloud();
            this.updateOrderlist();
        }

        private void getShopInfo()
        {
            this.shopName = this.Text;
            var query = new BmobQuery();
            query.WhereContainedIn<string>("shopName",this.shopName);
            var future = Bmob.FindTaskAsync<BmobShopObject>("Shop",query);
            this.RCToken = future.Result.results[0].RCToken;
            this.shopId = future.Result.results[0].objectId;
        }

        //初始化融云推送服务
        private void initRongCloud()
        {
            string path = System.Environment.CurrentDirectory;
            rcsdk.InitClient("lmxuhwagx93bd","99dayin","devideId"+this.shopId,path,path);
            ConnectAckListenerEventHandler connect_callback = new ConnectAckListenerEventHandler(connect_ack_callback_delegate.connect_call_back);
            MessageListenerEventHandler msg_callback = new MessageListenerEventHandler(message_listener_call_back);
            set_message_listener_callback_delegate.updateOrder += Set_message_listener_callback_delegate_updateOrder;
            //注册文本消息
            rcsdk.RegisterMessageType("RC:TxtMsg", 2);
            //设置消息监听
            rcsdk.SetMessageListener(msg_callback);
            rcsdk.Connect(this.RCToken,connect_callback);
        }

        private static void message_listener_call_back(string json)
        {
            string path = System.Environment.CurrentDirectory;
            SoundPlayer p = new SoundPlayer(path + "\\new.wav");
            p.Play();
            //MessageBox.Show("有新订单！","消息提示");
            byte[] buffer1 = Encoding.Default.GetBytes(json);
            byte[] buffer2 = Encoding.Convert(Encoding.UTF8, Encoding.Unicode, buffer1, 0, buffer1.Length);
            string strBuffer = Encoding.Unicode.GetString(buffer2, 0, buffer2.Length);
            JObject jobj = JObject.Parse(strBuffer);
            string msg_json_str = (string)jobj["m_Message"];
            JObject msg_jobj = JObject.Parse(msg_json_str);
            string content_str = (string)msg_jobj["content"];
            Console.WriteLine("收到消息" + content_str);
        }

        private void Set_message_listener_callback_delegate_updateOrder()
        {
            Control.CheckForIllegalCrossThreadCalls = false;
            //this.label1.Visible = true;
            //this.updateOrderlist();
        }

        private void InfoForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            System.Environment.Exit(0);
        }

        private void OrderListView_SelectedIndexChanged(object sender, EventArgs e)
        {

            //Console.WriteLine(this.OrderListView.FocusedItem.SubItems[0].Text);
        }

        private void button3_Click(object sender, EventArgs e)
        {
            this.OrderListView.Focus();
            var orderId = this.OrderListView.FocusedItem.SubItems[0].Text;
            string desktopDIR = Environment.GetFolderPath(Environment.SpecialFolder.DesktopDirectory);
            var localDIR = desktopDIR + "\\99打印订单\\订单" + orderId;
            //创建下载目录
            if (!Directory.Exists(localDIR))
            {
                Directory.CreateDirectory(localDIR);
            }

            DialogResult dr =  MessageBox.Show("确定下载订单:" + this.OrderListView.FocusedItem.SubItems[0].Text + "吗？", "下载提示",MessageBoxButtons.OKCancel);
            if (dr == DialogResult.OK)
            {
                //订单信息写入本地文档
                var query1 = new BmobQuery();
                query1.WhereContainedIn<string>("objectId",orderId);
                var future1 = Bmob.FindTaskAsync<BmobOrderObject>("Order",query1);
                var res1 = future1.Result.results[0];

                Word.Application App = new Word.Application();
                Word.Document DOC = new Word.Document();
                object nothing = System.Reflection.Missing.Value;
                DOC = App.Documents.Add(ref nothing, ref nothing, ref nothing, ref nothing);

                Word.Paragraph p1;
                p1 = DOC.Content.Paragraphs.Add(ref nothing);
                p1.Range.Text = "99打印订单";
                //p1.Range.Font.Size = 25;
                //p1.Range.ParagraphFormat.Alignment = Word.WdParagraphAlignment.wdAlignParagraphCenter;
                p1.Range.InsertParagraphAfter();

                Word.Paragraph p2;
                p2 = DOC.Paragraphs.Add(ref nothing);
                p2.Range.Text = "订单编号：" + orderId;
                p2.Range.InsertAfter("\n用户名：" + res1.username);
                p2.Range.InsertAfter("\n联系电话：" + res1.phone);
                p2.Range.InsertAfter("\n订单价格：" + res1.totalPrice.Get()/100.0 + "元");
                if (res1.address.Length == 0)
                {
                    p2.Range.InsertAfter("\n收获方式：到店自取");
                    p2.Range.InsertAfter("\n打印店：" + res1.shop);
                }
                else
                {
                    p2.Range.InsertAfter("\n收获方式：送货上门");
                    p2.Range.InsertAfter("\n收货地址：" + res1.address);
                }

                p2.Range.InsertAfter("\n\n订单内容及打印设置：\n");


                var jsonText = this.OrderListView.FocusedItem.SubItems[1].Text;
                JArray items = (JArray)JsonConvert.DeserializeObject(jsonText);
                Thread t = new Thread(new ParameterizedThreadStart(ShowMessage));
                t.Start("正在下载，请稍后");
                foreach (var item in items)
                {
                    String fileMD5 = (String)item["fileMD5"];
                    string filename = (string)item["filename"];
                    string fileType = (string)item["fileType"];

                    var query = new BmobQuery();
                    query.WhereContainedIn<string>("fileMD5",fileMD5);
                    var future = Bmob.FindTaskAsync<BmobFileInfoObject>("File_Info",query);
                    var path = future.Result.results[0].path;

                    var printSettings = item["printSettings"];
                    //打印设置写入文件

                    p2.Range.InsertAfter("\n文件名：" + filename);
                    p2.Range.InsertAfter("\n打印数量：" + printSettings["amount"]);
                    p2.Range.InsertAfter("\n纸张尺寸：" + printSettings["paperSize"]);
                    if ((Boolean)printSettings["isTwoSides"])
                    {
                        p2.Range.InsertAfter("\n双面打印");
                    }
                    if ((Boolean)printSettings["isColor"])
                    {
                        p2.Range.InsertAfter("\n彩色打印");
                    }
                    if (fileType.Equals("PPT"))
                    {
                        p2.Range.InsertAfter("\n每页PPT数量：" + printSettings["pptPerPage"]);
                    }

                    string direction = (string)printSettings["direction"];
                    if (direction.Equals("vertical"))
                    {
                        p2.Range.InsertAfter("\n打印方向：竖直");
                    }
                    else
                    {
                        p2.Range.InsertAfter("\n打印方向：横向");
                    }
                    p2.Range.InsertAfter("\n其他备注：" + printSettings["remark"]);
                    p2.Range.InsertAfter("\n");
                    //下载文件
                    this.downloadObject(bucket,path,localDIR  + "\\" +filename);
                }
                t.Abort();
                MessageBox.Show("下载完成","提醒消息");

                //退出word
                p2.Range.Font.Size = 10;
                p2.Range.ParagraphFormat.Alignment = Word.WdParagraphAlignment.wdAlignParagraphLeft;
                DOC.SaveAs2(localDIR + "\\"  + orderId +"订单详情.doc", Microsoft.Office.Interop.Word.WdSaveFormat.wdFormatDocument97);
                DOC.Close();
                App.Quit();

                string path1 = System.Environment.CurrentDirectory;
                SoundPlayer p = new SoundPlayer(path1 + "\\done.wav");
                p.Play();
            }
        }

        public static void ShowMessage(object s)
        {
            //messageBoxForm m = new messageBoxForm();
            //m.Show();
            MessageBox.Show((string)s,"提醒消息");
            //this.Invoke(new MessageBoxShow(MessageBoxShow_F), new object[] { msg });
        }

        public void downloadObject(string bucketName, string key, string fileToDownload)
        {
            try
            {
                var ossObject = ossClient.GetObject(bucketName, key);

                //将从OSS读取到的文件写到本地
                using (var requestStream = ossObject.Content)
                {
                    byte[] buf = new byte[1024];
                    using (var fs = File.Open(fileToDownload, FileMode.OpenOrCreate))
                    {
                        var len = 0;
                        while ((len = requestStream.Read(buf, 0, 1024)) != 0)
                        {

                            fs.Write(buf, 0, len);
                        }
                    }
                }
            }
            catch (Exception es)
            {
                Console.WriteLine("Get object failed, {0}", es.Message);
            }
        }

        private void button4_Click(object sender, EventArgs e)
        {
            this.OrderListView.Focus();
            DialogResult dr = MessageBox.Show("确认订单" + this.OrderListView.FocusedItem.SubItems[0].Text + "已打印完成吗？" ,"完成提示" ,MessageBoxButtons.OKCancel);
            if (dr == DialogResult.OK)
            {
                this.orderObject.objectId = this.OrderListView.FocusedItem.SubItems[0].Text;
                this.orderObject.state = "PRINTED";
                var future = Bmob.UpdateTaskAsync<BmobOrderObject>(this.orderObject);
                this.updateOrderlist();
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            this.downloadButton.Enabled = false;
            this.button4.Enabled = false;
            this.updateDoneOrderList();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            this.downloadButton.Enabled = true;
            this.button4.Enabled = true;
            this.OrderListView.Show();
            this.updateOrderlist();
            //this.label1.Visible = false;
        }

        private void updateOrderlist()
        {
            this.OrderListView.Items.Clear();
            var query = new BmobQuery();
            query.WhereContainedIn<string>("state", "PAID");
            query.WhereContainedIn<string>("shop", this.shopName);
            var future = Bmob.FindTaskAsync<BmobOrderObject>("Order", query);
            var res = future.Result;
            foreach (var one in future.Result.results)
            {
                ListViewItem lvi = new ListViewItem();
                lvi.Text = one.objectId;
                lvi.SubItems.Add(one.items);
                int total = one.totalPrice.Get();

                lvi.SubItems.Add((one.totalPrice.Get() / 100.0).ToString() + "元");
                this.OrderListView.Items.Add(lvi);
            }
        }

        private void updateDoneOrderList()
        {
            this.OrderListView.Items.Clear();
            var query = new BmobQuery();
            query.WhereContainedIn<string>("state", "PRINTED");
            query.WhereContainedIn<string>("shop", this.shopName);
            var future = Bmob.FindTaskAsync<BmobOrderObject>("Order", query);
            var res = future.Result;
            foreach (var one in future.Result.results)
            {
                ListViewItem lvi = new ListViewItem();
                lvi.Text = one.objectId;
                lvi.SubItems.Add(one.items);
                int total = one.totalPrice.Get();

                lvi.SubItems.Add((one.totalPrice.Get() / 100.0).ToString() + "元");
                this.OrderListView.Items.Add(lvi);
                JsonReader reader = new JsonTextReader(new StringReader(one.items));
            }
        }

        private void progressBar1_Click(object sender, EventArgs e)
        {

        }

        private void label1_Click(object sender, EventArgs e)
        {

        }
    }
}
