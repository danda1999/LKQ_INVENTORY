using System;
using System.Net;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Management.Automation;
using System.Reflection;
using System.ServiceProcess;
using System.Text;
using System.Threading.Tasks;
using System.Timers;
using System.Net.Sockets;
using System.Xml.Linq;

namespace Inventory
{
    public partial class Service1 : ServiceBase
    {



        Timer timer = new Timer();
        public Service1()
        {
            InitializeComponent();
        }

        protected override void OnStart(string[] args)
        {
            using (StreamWriter sw = new StreamWriter(Constanty.PATH_FILE_LOGS, true))
            {
                sw.WriteLine("Start service: " + DateTime.Now.ToString("MM.dd.yyyy HH:mm"));
            }
            CheckInventory();
            timer.Elapsed += new ElapsedEventHandler(OnElapsedTime);
            timer.Interval = 360000; //number in milisecinds  
            timer.Enabled = true;
        }

        protected override void OnStop()
        {
            using (StreamWriter sw = new StreamWriter(Constanty.PATH_FILE_LOGS, true))
            {
                sw.WriteLine("End service: " + DateTime.Now.ToString("MM.dd.yyyy HH:mm"));
            }
        }

        private void OnElapsedTime(object source, ElapsedEventArgs e)
        {
            using (StreamWriter sw = new StreamWriter(Constanty.PATH_FILE_LOGS, true))
            {
                sw.WriteLine("Replace service: " + DateTime.Now.ToString("MM.dd.yyyy HH:mm"));
            }
            CheckInventory();
        }

        private void CheckInventory()
        {

            IPAddress ipAddr = IPAddress.Parse(Constanty.IPHOSTNAME);
            IPEndPoint localEndPoint = new IPEndPoint(ipAddr, Constanty.PORT);

            Socket sender = new Socket(ipAddr.AddressFamily, SocketType.Stream, ProtocolType.Tcp);

            if (File.Exists(Constanty.PATH_FILE_CHECK_TIME))
            {
                int substract = DateTime.Now.Subtract(File.GetLastWriteTime(Constanty.PATH_FILE_CHECK_TIME)).Days;

                if (substract >= 0)
                {
                    PowerShell ps = PowerShell.Create();

                    string scriptPath = Constanty.PATH_SCRIPT;

                    ps.AddScript(File.ReadAllText(scriptPath));

                    ps.Invoke();

                    using (StreamWriter sw = new StreamWriter(Constanty.PATH_FILE_CHECK_TIME))
                    {
                        sw.WriteLine(DateTime.Now.ToString());
                    }

                    NetworkCommunication(sender, localEndPoint);

                    File.Delete(Constanty.PATH_SHARE_FILE);

                } 
                
            }
            else if(!File.Exists(Constanty.PATH_FILE_CHECK_TIME))
            {

                PowerShell ps = PowerShell.Create();

                string scriptPath = Constanty.PATH_SCRIPT;

                ps.AddScript(File.ReadAllText(scriptPath));

                ps.Invoke();

                using (StreamWriter sw = new StreamWriter(Constanty.PATH_FILE_CHECK_TIME))
                {
                    sw.WriteLine(DateTime.Now.ToString());
                }

                NetworkCommunication(sender, localEndPoint);

                File.Delete(Constanty.PATH_SHARE_FILE);
            }

        }

        private bool NetworkCommunication(Socket sender, IPEndPoint localEndPoint)
        {
            bool connection = false;
            try
            {
                sender.Connect(localEndPoint);
                connection = true;

                using (StreamWriter sw = new StreamWriter(Constanty.PATH_FILE_LOGS, true))
                {
                    sw.WriteLine("Services connection: " + DateTime.Now.ToString("MM.dd.yyyy HH:mm"));
                }

                if (File.Exists(Constanty.PATH_CONNECTION_TIME) && connection == true)
                {
                    int byteRecv = 0;

                    int substract = DateTime.Now.Subtract(File.GetLastWriteTime(Constanty.PATH_CONNECTION_TIME)).Days;

                    if (substract >= 1)
                    {

                        byte[] messageReceived = new byte[4049];

                        byteRecv = sender.Receive(messageReceived);

                        string Hello_messege = Encoding.ASCII.GetString(messageReceived, 0, byteRecv);


                        if (Hello_messege.Equals("LKQ2023I"))
                        {

                            string name = Environment.MachineName;
                            byte[] name_array = Encoding.ASCII.GetBytes(name);
                            sender.Send(name_array);


                            byte[] file_array = File.ReadAllBytes(Constanty.PATH_SHARE_FILE);
                            sender.Send(file_array);


                            messageReceived = new byte[4049];

                            byteRecv = sender.Receive(messageReceived);

                            if (byteRecv == 0)
                            {
                                using (StreamWriter sw = new StreamWriter(Constanty.PATH_CONNECTION_TIME))
                                {
                                    sw.WriteLine(DateTime.Now.ToString());
                                }
                            }

                        }
                    } else
                    {
                        byte[] messageReceived = new byte[4049];

                        byteRecv = sender.Receive(messageReceived);

                        string Hello_messege = Encoding.ASCII.GetString(messageReceived, 0, byteRecv);


                        if (Hello_messege.Equals("LKQ2023I"))
                        {
                            string name = Environment.MachineName;
                            byte[] name_array = Encoding.ASCII.GetBytes(name);
                            sender.Send(name_array);

                            System.Threading.Thread.Sleep(60000);
                            byte[] quite_message = Encoding.ASCII.GetBytes("quit");
                            sender.Send(quite_message);
                        }
                    }

                } else if(!File.Exists(Constanty.PATH_CONNECTION_TIME) && connection == true){
                    int byteRecv = 0;

                    byte[] messageReceived = new byte[4049];

                    byteRecv = sender.Receive(messageReceived);

                    string Hello_messege = Encoding.ASCII.GetString(messageReceived, 0, byteRecv);

                    

                    if (Hello_messege.Equals("LKQ2023I"))
                    {

                        string name = Environment.MachineName;
                        byte[] name_array = Encoding.ASCII.GetBytes(name);
                        sender.Send(name_array);

                        byte[] file_array = File.ReadAllBytes(Constanty.PATH_SHARE_FILE);
                        sender.Send(file_array);

                        messageReceived = new byte[4049];

                        byteRecv = sender.Receive(messageReceived);

                        if(byteRecv == 0) {
                            using (StreamWriter sw = new StreamWriter(Constanty.PATH_CONNECTION_TIME))
                            {
                                sw.WriteLine(DateTime.Now.ToString());
                            }
                        }

                    }

                }                

                using (StreamWriter sw = new StreamWriter(Constanty.PATH_FILE_LOGS, true))
                {
                    sw.WriteLine("Connection close: " + DateTime.Now.ToString("MM.dd.yyyy HH:mm"));
                }

            } catch(Exception)
            {
                using (StreamWriter sw = new StreamWriter(Constanty.PATH_FILE_LOGS, true))
                {
                    sw.WriteLine("Nelze se připojit k serveru pro zpracování json file: " + DateTime.Now.ToString("MM.dd.yyyy HH:mm"));
                }
                
            }

            return connection;
        }
    }
}
