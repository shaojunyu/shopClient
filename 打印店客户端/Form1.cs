using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using rcsdk_test;
using System.Media;
using Word = Microsoft.Office.Interop.Word;

namespace 打印店客户端
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            string path = System.Environment.CurrentDirectory;
            rcsdk.InitClient("lmxuhwagx93bd", "99dayin", "deviceid", path, path);
            Console.WriteLine(path + "\\new.wav");

            SoundPlayer p = new SoundPlayer(path + "\\new.wav");
            //p.Play();

            Word.Application App = new Word.Application();
            Word.Document DOC = new Word.Document();
            object nothing = System.Reflection.Missing.Value;
            DOC = App.Documents.Add(ref nothing, ref nothing, ref nothing, ref nothing);
            App.Visible = true;

            Word.Paragraph p1;
            p1 = DOC.Content.Paragraphs.Add(ref nothing);
            
            p1.Range.Text = "dadsdsdd";
            //p1.Range.InsertBreak(ref nothing);
            p1.Range.InsertAfter("\n22222");
            p1.Range.ParagraphFormat.Alignment = Word.WdParagraphAlignment.wdAlignParagraphCenter;
            //DOC.SaveAs2(path + "\\test.doc", Microsoft.Office.Interop.Word.WdSaveFormat.wdFormatDocument97);
            //DOC.Close();
            //App.Quit();
        }
    }
}
