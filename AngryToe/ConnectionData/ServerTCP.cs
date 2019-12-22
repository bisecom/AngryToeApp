using System;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace AngryToe.ConnectionData
{
    public static class ServerTCP
    {

        public static IPAddress ipAddress { get; set; }
        public static int port { get; set; }
        public static int ServerStep { get; set; }
        public static NetworkStream networkStream { get; set; }
        public static TcpClient tcpClient { get; set; }
        public static string MeesageToSend { get; set; }
        public static string ServUserName { get; set; }
        public static int ServUserScore { get; set; }

        static ServerTCP()
        { }

        public static void InitializeServer(string IpAddress, int port_)
        {
            ipAddress = IPAddress.Parse(IpAddress); ;
            port = port_;
            //tcs = null;
        }


        public static async Task<string> RunBest(TextBlock WhomStepTB, bool Requested3SizeField, int[,] WorkingField,
            RadioButton FiveFieldRB, RadioButton ThreeFieldRB, string ServerPlayerName, string contrPlayerName, TextBlock Player2Name, string whomStep)
        {
            if (tcpClient != null) tcpClient.Close();
            string fieldSizeOrCases = ""; int[] tempArr = new int[2];
            int clicked_i_Index = 0; int clicked_j_Index = 0;
            string[] dataSetArr = new string[4];
            var listener = new TcpListener(ipAddress, port);
            listener.Start();
        
            tcpClient = await listener.AcceptTcpClientAsync();
            networkStream = tcpClient.GetStream();

            var reader = new StreamReader(networkStream);
            var writer = new StreamWriter(networkStream);
            writer.AutoFlush = true;

            if (FiveFieldRB.IsChecked == true)
                fieldSizeOrCases = "F5";
            if (ThreeFieldRB.IsChecked == true)
                fieldSizeOrCases = "F3";

            await writer.WriteLineAsync("S2"+ fieldSizeOrCases + "STRT"+"&"+ ServerPlayerName);

            string request = await reader.ReadLineAsync();
            if (request != "")
            {

                dataSetArr = ServerTCP.DataParsing(request);
                if (dataSetArr[0] == "S1")
                { WhomStepTB.Text = "Step for Player: " + ServUserName; whomStep = "S1"; contrPlayerName = dataSetArr[3]; Player2Name.Text = dataSetArr[3]; }
                if (dataSetArr[1] == "F3") Requested3SizeField = true;
                if (dataSetArr[1] == "F5") Requested3SizeField = false;
                if (dataSetArr[2].IndexOf("I") == 0 && dataSetArr[2].IndexOf("J") == 2)
                {
                    string temp = dataSetArr[2].Remove(0, 1);
                    int index = temp.IndexOf("J");
                    if (index > 0)
                        clicked_i_Index = Convert.ToInt32(temp.Substring(0, index));
                    clicked_j_Index = Convert.ToInt32(dataSetArr[2].Substring(dataSetArr[2].IndexOf("J") + 1));

                    WorkingField[clicked_i_Index, clicked_j_Index] = 2;
                    //MessageBox.Show(WhomStepTB.Text + " " + Requested3SizeField.ToString() + " " + clicked_i_Index.ToString() + " " + clicked_j_Index.ToString());
                }
            }
            return dataSetArr[2];
        } // Start


        public static async Task<string> FirstTimeReceiveAsync(TextBlock WhomStepTB, bool Requested3SizeField, int[,] WorkingField)
        {
            int clicked_i_Index = 0; int clicked_j_Index = 0;
            string[] dataSetArr = new string[3];
            
            var reader = new StreamReader(networkStream);
            string request = await reader.ReadLineAsync();
            if (request != "")
            {

                dataSetArr = ServerTCP.DataParsing(request);
                if (dataSetArr[0] == "S1") WhomStepTB.Text = "Step for Player 1";
                if (dataSetArr[1] == "F3") Requested3SizeField = true;
                if (dataSetArr[1] == "F5") Requested3SizeField = false;
                if (dataSetArr[2].IndexOf("I") == 0 && dataSetArr[2].IndexOf("J") == 2)
                {
                    string temp = dataSetArr[2].Remove(0, 1);
                    int index = temp.IndexOf("J");
                    if (index > 0)
                        clicked_i_Index = Convert.ToInt32(temp.Substring(0, index));
                    clicked_j_Index = Convert.ToInt32(dataSetArr[2].Substring(dataSetArr[2].IndexOf("J") + 1));

                    WorkingField[clicked_i_Index, clicked_j_Index] = 2;
                    //MessageBox.Show(WhomStepTB.Text + " " + Requested3SizeField.ToString() + " " + clicked_i_Index.ToString() + " " + clicked_j_Index.ToString());
                }
            }

            return dataSetArr[2];
        } 


        public static async Task<string> SendDataAsync(/*TextBlock myTextTB, StackPanel myIconTB*/ string str)
        {
            var reader = new StreamReader(networkStream);
            var writer = new StreamWriter(networkStream);
            writer.AutoFlush = true;
            await writer.WriteLineAsync(str);

            string request = await reader.ReadLineAsync();
            if (request != null)
            {
                return request;
            }
            return null;
        }

       
        public static async Task RequestToContinueAsync(string str, int[,] WorkingField, TextBlock WhomStepTB, TextBlock Player1Name)
        {
            string[] dataSetArr = new string[4]; 
            var writer = new StreamWriter(networkStream);
            var reader = new StreamReader(networkStream);
            writer.AutoFlush = true;
            await writer.WriteLineAsync(str);
            
            tcpClient.Close();
        }


        public static string GetLocalIPAddress()
        {
            var host = Dns.GetHostEntry(Dns.GetHostName());
            foreach (var ip in host.AddressList)
            {
                if (ip.AddressFamily == AddressFamily.InterNetwork)
                {
                    return ip.ToString();
                }
            }
            throw new Exception("No network adapters with an IPv4 address in the system!");
        }

        public static string[] DataParsing(string str)
        {
            //S1F3I2J3&ENDXXXXXXX
            string[] dataSetArr = new string[4];
            string whomStep = ""; //S2 - Client, S1 - Server
            string fieldSize = ""; //F3 - 3x3, F5 - 5x5, WN - Winner
            string playerStepAtFieldOrCases = ""; // like I2J3 or STRT or CONT or YESX or NOXX
            string playerNameOrCases = ""; //Players name
            whomStep = str.Substring(0, 2);

            if (str.Substring(0, 4).IndexOf('F') > 0)
                fieldSize = (str.Substring(0, 4)).Substring((str.Substring(0, 4)).IndexOf('F'));
            if (str.Substring(0, 4).IndexOf('W') > 0)
                fieldSize = (str.Substring(0, 4)).Substring((str.Substring(0, 4)).IndexOf('W'));

            playerStepAtFieldOrCases = str.Substring(4, 4);

            if (str.IndexOf("END") < 0)
                playerNameOrCases = str.Substring(str.LastIndexOf('&') + 1);
            if (str.IndexOf("END") > 0)
                playerNameOrCases = "END";

            dataSetArr[0] = whomStep;
            dataSetArr[1] = fieldSize;
            dataSetArr[2] = playerStepAtFieldOrCases;
            dataSetArr[3] = playerNameOrCases;
            return dataSetArr;
        }


        public static T FindChild<T>(DependencyObject parent, string childName)
   where T : DependencyObject
        {
            // Confirm parent and childName are valid. 
            if (parent == null) return null;

            T foundChild = null;

            int childrenCount = VisualTreeHelper.GetChildrenCount(parent);
            for (int i = 0; i < childrenCount; i++)
            {
                var child = VisualTreeHelper.GetChild(parent, i);
                // If the child is not of the request child type child
                T childType = child as T;
                if (childType == null)
                {
                    // recursively drill down the tree
                    foundChild = FindChild<T>(child, childName);

                    // If the child is found, break so we do not overwrite the found child. 
                    if (foundChild != null) break;
                }
                else if (!string.IsNullOrEmpty(childName))
                {
                    var frameworkElement = child as FrameworkElement;
                    // If the child's name is set for search
                    if (frameworkElement != null && frameworkElement.Name == childName)
                    {
                        // if the child's name is of the request name
                        foundChild = (T)child;
                        break;
                    }
                }
                else
                {
                    // child element found.
                    foundChild = (T)child;
                    break;
                }
            }

            return foundChild;
        }

    }
}

