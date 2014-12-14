using MediaInfoNET;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Xml;

namespace TGServer
{
    public partial class Form1 : Form
    {
        BAL.TGREQ _Rq = new BAL.TGREQ();
        string _AeProject = null;
        string _Composition = null;
        XmlDocument _XDoc = new XmlDocument();
        long _DurationSec = 0;
        string _FileNamePrefix = null;
        public Form1()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            timer1.Enabled = false;

            button1.ForeColor = Color.White;
            button1.Text = "Started";
            button1.BackColor = Color.Red;
            richTextBox1.Text = "";

          
            MainAct();

            button1.ForeColor = Color.White;
            button1.Text = "Start";
            button1.BackColor = Color.Navy;

           timer1.Enabled = true;
        }
        protected void MainAct()
        {
            _Rq = DAL.SelectJob();

            _Rq.DATETIME_START = DateTime.Now;

            DAL.Update(_Rq);

            LoadXml();

            GenerateScript();

            System.IO.File.Delete(Path.GetDirectoryName(Application.ExecutablePath) + "\\Files\\Title.avi");

            ApplyScript();

            RenderTitle();


            string CleanFile = ConfigurationSettings.AppSettings["CleanDirectory"].ToString().Trim() + "\\" + _Rq.Id.ToString() + ".mp4";
            MediaFile VideoFile = new MediaFile(CleanFile);
            _DurationSec = VideoFile.Video[0].DurationMillis / 1000;
            string TitleFile = Path.GetDirectoryName(Application.ExecutablePath) + "\\Files\\Title.avi";
            string TitledFile = Path.GetDirectoryName(Application.ExecutablePath) + "\\Files\\Titled.mp4";
            System.IO.File.Delete(Path.GetDirectoryName(Application.ExecutablePath) + "\\Files\\Titled.mp4");

            Overlay(TitleFile, CleanFile, _DurationSec - 20, TitledFile);


            string EndingFile = Path.GetDirectoryName(Application.ExecutablePath) + "\\Files\\" + _Rq.ENDINGKIND + ".mov";
            string DateTimeStr = string.Format("{0:00}", DateTime.Now.Hour) + "-" + string.Format("{0:00}", DateTime.Now.Minute) + "-" + string.Format("{0:00}", DateTime.Now.Second);
            string FinalFileName = _FileNamePrefix + "_" + _Rq.Title1.Replace("\r", "-").Replace("\n", "-").Replace("\r\n", "-").Replace(":", "") + "_" + _Rq.Title2.Replace("\r", "").Replace("\n", "").Replace("\r\n", "-").Replace(":", "") + "_" + DateTimeStr + ".mp4";
             string FinalFile = ConfigurationSettings.AppSettings["OutputPath"].ToString().Trim() + "\\"+FinalFileName;
            Overlay(EndingFile, TitledFile, _DurationSec - 5, FinalFile);


            DAL.UpdateIsDone(_Rq.Id, true, FinalFileName);


            try
            {
                if (System.IO.File.Exists(CleanFile))
                {
                    System.IO.File.Delete(CleanFile);
                }
            }
            catch (Exception Exp)
            {
                richTextBox1.Text += (Exp.Message) + " \n";
                richTextBox1.SelectionStart = richTextBox1.Text.Length;
                richTextBox1.ScrollToCaret();
            }

        }
        protected void LoadXml()
        {
            string XmlPath = Path.GetDirectoryName(Application.ExecutablePath) + "\\Xml\\" + _Rq.TEMPLATE + ".xml";
            _XDoc.Load(XmlPath);
            XmlNodeList NodeProject = _XDoc.GetElementsByTagName("Project");
            _AeProject = NodeProject[0].Attributes["AePath"].Value.ToString();
            _Composition = NodeProject[0].Attributes["Composition"].Value.ToString();
            _FileNamePrefix = NodeProject[0].Attributes["FileNamePrefix"].Value.ToString();
        }
        protected string GetLayerName(string TName)
        {
            XmlNodeList LayersLst = _XDoc.GetElementsByTagName("Layer");
            string Name = null;
            foreach (XmlNode Nd in LayersLst)
            {
                if (Nd.Attributes["id"].Value.ToString() == TName)
                {
                    Name = Nd.Attributes["name"].Value.ToString();
                }
            }
            return Name;
        }
        protected void GenerateScript()
        {
            StreamWriter Str = new StreamWriter(Path.GetDirectoryName(Application.ExecutablePath) + "//Files//Scr.jsx");

            StringBuilder Scrpt = new StringBuilder();
            Str.WriteLine("app.open(new File(\"" + _AeProject.Replace("\\", "\\\\") + "\"));  ");
            Str.WriteLine("function LayerText(tname,text)  ");
            Str.WriteLine("{  ");
            Str.WriteLine("for(var i = 1; i <= app.project.numItems; i++) {  ");
            Str.WriteLine("var B=app.project.item(i);  ");
             Str.WriteLine("for(var j=1; j <= B.numLayers;j++) {  ");
             Str.WriteLine("	var L=B.layer(j);  ");
             Str.WriteLine("	if(L.name==tname) {  ");
             Str.WriteLine("	L.sourceText.setValue(text);  ");
             Str.WriteLine("	break;  ");
             Str.WriteLine("}  ");
             Str.WriteLine("}  ");
             Str.WriteLine("}  ");
             Str.WriteLine("}  ");


            string LayerNameTitle1 = GetLayerName("TITLE1");
            if (LayerNameTitle1 != null)
            {
                Str.WriteLine(" LayerText (\"" + LayerNameTitle1 + "\",\"" + _Rq.Title1.Replace("\r\n","\\r") + "\");  ");
            }

            string LayerNameTitle2 = GetLayerName("TITLE2");
            if (LayerNameTitle2 != null)
            {
                Str.WriteLine(" LayerText (\"" + LayerNameTitle2 + "\",\"" + _Rq.Title2.Replace("\r\n", "\\r") + "\");  ");
            }

            for (int i = 1; i < 11; i++)
            {
                string LayerName = GetLayerName("T" + i);
                if (LayerName != null)
                {
                    switch (i)
                    {
                        case 1:
                             Str.WriteLine(" LayerText ('" + LayerName + "','" + _Rq.T1 + "');  ");
                            break;

                        case 2:
                             Str.WriteLine(" LayerText ('" + LayerName + "','" + _Rq.T2 + "');  ");
                            break;


                        case 3:
                             Str.WriteLine(" LayerText ('" + LayerName + "','" + _Rq.T3 + "');  ");
                            break;


                        case 4:
                             Str.WriteLine(" LayerText ('" + LayerName + "','" + _Rq.T4 + "');  ");
                            break;


                        case 5:
                             Str.WriteLine(" LayerText ('" + LayerName + "','" + _Rq.T5 + "');  ");
                            break;


                        case 6:
                             Str.WriteLine(" LayerText ('" + LayerName + "','" + _Rq.T6 + "');  ");
                            break;


                        case 7:
                             Str.WriteLine(" LayerText ('" + LayerName + "','" + _Rq.T7 + "');  ");
                            break;


                        case 8:
                             Str.WriteLine(" LayerText ('" + LayerName + "','" + _Rq.T8 + "');  ");
                            break;


                        case 9:
                             Str.WriteLine(" LayerText ('" + LayerName + "','" + _Rq.T9 + "');  ");
                            break;


                        case 10:
                             Str.WriteLine(" LayerText ('" + LayerName + "','" + _Rq.T10 + "');  ");
                            break;

                        default:
                            break;
                    }

                }
            }
            Str.WriteLine("app.project.save()");
            Str.WriteLine("app.quit();");
            Str.Close();


        }
        protected void RenderTitle()
        {
            Process proc = new Process();
            proc.StartInfo.FileName = "\"" + ConfigurationSettings.AppSettings["AeRenderPath"].ToString().Trim() +"aerender.exe"+ "\"";
            string OutFile = Path.GetDirectoryName(Application.ExecutablePath) + "\\Files\\Title.avi";
            proc.StartInfo.Arguments = " -project " + "\"" + _AeProject + "\"" + "   -comp   \"" + _Composition + "\" -output " + "\"" + OutFile + "\"";
            proc.StartInfo.RedirectStandardError = true;
            proc.StartInfo.UseShellExecute = false;
            proc.StartInfo.CreateNoWindow = true;
            proc.EnableRaisingEvents = true;
            proc.StartInfo.RedirectStandardOutput = true;
            proc.StartInfo.RedirectStandardError = true;

            proc.Start();
           

            proc.PriorityClass = ProcessPriorityClass.Normal;
            StreamReader reader = proc.StandardOutput;
            string line;
            while ((line = reader.ReadLine()) != null)
            {
                //if (richTextBox1.Lines.Length > 10)
                //{
                //    richTextBox1.Text = "";
                //}
                richTextBox1.Text += (line) + " \n";
                richTextBox1.SelectionStart = richTextBox1.Text.Length;
                richTextBox1.ScrollToCaret();
                Application.DoEvents();

            }
            proc.Close();
        }
        protected void Overlay(string OverlayFile, string BackFile, long StartOverlaySec, string OutFile)
        {
            Process proc = new Process(); if (Environment.Is64BitOperatingSystem)
            proc.StartInfo.FileName = Path.GetDirectoryName(Application.ExecutablePath) + "//files//ffmpeg";
            proc.StartInfo.Arguments = "-i " + "\"" + BackFile + "\"" + " -itsoffset " + StartOverlaySec +" -i "+ "\"" + OverlayFile + "\"" + " -filter_complex overlay " + "\"" + OutFile + "\"";

            proc.StartInfo.RedirectStandardError = true;
            proc.StartInfo.UseShellExecute = false;
            proc.StartInfo.CreateNoWindow = true;
            proc.EnableRaisingEvents = true;
            proc.Start();
            proc.PriorityClass = ProcessPriorityClass.Normal;
            StreamReader reader = proc.StandardError;
            string line;
            while ((line = reader.ReadLine()) != null)
            {
                //if (richTextBox1.Lines.Length > 15)
                //{
                //    richTextBox1.Text = "";
                //}

                richTextBox1.Text += (line) + " \n";
                richTextBox1.SelectionStart = richTextBox1.Text.Length;
                richTextBox1.ScrollToCaret();
                Application.DoEvents();
            }
            proc.Close();
        }
        protected void ApplyScript()
        {
            Process proc = new Process();
            proc.StartInfo.FileName = "\"" + ConfigurationSettings.AppSettings["AeRenderPath"].ToString().Trim()+"afterfx.com" + "\"";
            string OutFile = Path.GetDirectoryName(Application.ExecutablePath) + "\\Files\\Title.avi";
            string ScriptFile = Path.GetDirectoryName(Application.ExecutablePath) + "\\Files\\Scr.jsx";
            proc.StartInfo.Arguments = "  -r  " + "\"" + ScriptFile + "\"";
            proc.StartInfo.RedirectStandardError = true;
            proc.StartInfo.UseShellExecute = false;
            proc.StartInfo.CreateNoWindow = true;
            proc.EnableRaisingEvents = true;
            proc.StartInfo.RedirectStandardOutput = true;
            proc.StartInfo.RedirectStandardError = true;

            proc.Start();


            proc.PriorityClass = ProcessPriorityClass.Normal;
            StreamReader reader = proc.StandardOutput;
            string line;
            while ((line = reader.ReadLine()) != null)
            {
                //if (richTextBox1.Lines.Length > 10)
                //{
                //    richTextBox1.Text = "";
                //}
                richTextBox1.Text += (line) + " \n";
                richTextBox1.SelectionStart = richTextBox1.Text.Length;
                richTextBox1.ScrollToCaret();
                Application.DoEvents();

            }
            proc.Close();
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            if(DAL.SelectJob() !=null)
            {
                timer1.Enabled = false;
                button1_Click(new object(), new EventArgs());
            }
            

        }
    }
}
