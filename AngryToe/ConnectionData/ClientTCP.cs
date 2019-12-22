using System;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace AngryToe.ConnectionData
{
    public static class ClientTCP
    {
        //private const int port = 8888;
        //public static string IpAdressToConnect { get; set; }
        public static int ClientStep { get; set; }
        public static string ClientAnswer { get; set; }
        public static TaskCompletionSource<bool> tcsClient { get; set; }
        public static TcpClient client { get; set; }
        public static string ClientUserName { get; set; }
        public static int ClientUserScore { get; set; }

        static ClientTCP()
        {
            tcsClient = null;
        }

        public static async Task WriteLabelBestAsync(string server, int port, TextBlock WhomStepTB, bool Requested3SizeField, int[,] WorkingField,
            RadioButton FiveFieldRB, RadioButton ThreeFieldRB, string contrPlayerName, TextBlock Player1Name)
        {
            string responseStep = "";
            if (client != null) client.Close();
            try
            {
                IPAddress ipAddress = null;
                IPHostEntry ipHostInfo = Dns.GetHostEntry(server);
                for (int i = 0; i < ipHostInfo.AddressList.Length; ++i)
                {
                    if (ipHostInfo.AddressList[i].AddressFamily == AddressFamily.InterNetwork)
                    {
                        ipAddress = ipHostInfo.AddressList[i];
                        break;
                    }
                }
                if (ipAddress == null)
                    throw new Exception("Unable to find an IPv4 address for server");

                client = new TcpClient();

                bool connected = false; int numberOfTimes = 0;
                while (!connected)
                {
                    try
                    {
                        await client.ConnectAsync(ipAddress, port); // connect to the server
                        connected = true;
                    }
                    catch (SocketException retryConnectException)
                    {
                        //Optional - add some wait time may be 5 seconds i.e. "trying again in 5 seconds"
                        System.Threading.Thread.Sleep(1000);
                        //Here check the number of attempts and if exceeded:
                        if (numberOfTimes == 10)
                        {
                            break;
                        }
                        else
                        {
                            numberOfTimes++;
                            continue;
                        }
                    }
                }

                NetworkStream networkStream = client.GetStream();
                var reader = new StreamReader(networkStream);
                string request = await reader.ReadLineAsync();

                if (request != "")
                {
                    string[] dataSetArr = new string[4]; int clicked_i_Index = 0; int clicked_j_Index = 0;
                    dataSetArr = ServerTCP.DataParsing(request);
                    if (dataSetArr[0] == "S2")
                    { WhomStepTB.Text = "Step for Player: " + ClientUserName; contrPlayerName = dataSetArr[3];
                        Player1Name.Text = dataSetArr[3];
                    } //"Step for Player 2"
                    if (dataSetArr[1] == "F3") Requested3SizeField = true;
                    if (dataSetArr[1] == "F5") Requested3SizeField = false;
                    if (Requested3SizeField == true && FiveFieldRB.IsChecked == true)
                    {
                        client.Close();
                        return;
                    }
                    if (Requested3SizeField == false && ThreeFieldRB.IsChecked == true)
                    {
                        client.Close(); return;
                    }
                    if (dataSetArr[2] == "STRT") return;
                    if (dataSetArr[2].IndexOf("I") == 0 && dataSetArr[2].IndexOf("J") == 2)
                    {
                        string temp = dataSetArr[2].Remove(0, 1);
                        int index = temp.IndexOf("J");
                        if (index > 0)
                            clicked_i_Index = Convert.ToInt32(temp.Substring(0, index));
                        clicked_j_Index = Convert.ToInt32(dataSetArr[2].Substring(dataSetArr[2].IndexOf("J") + 1));

                        WorkingField[clicked_i_Index, clicked_j_Index] = 1;
                    }


                }
            }
            catch (Exception ex)
            {
                // return ex.Message;
            }
        } // SendRequest

       
        public static async Task RequestToContinueAsync(string str)
        {
            NetworkStream networkStream = client.GetStream();
            var writer = new StreamWriter(networkStream);
            writer.AutoFlush = true;
            await writer.WriteLineAsync(str);

        }

        public static async Task<string> ReceiveDataClientBest()
        {
            string responseStep = "";
            try
            {
                NetworkStream networkStream = client.GetStream();
                var reader = new StreamReader(networkStream);

                string request = await reader.ReadLineAsync();
                if (request != null)
                {
                    return request;
                }

                //client.Close();
                return null;
            }
            catch (Exception ex)
            {
                return ex.Message;
            }
        } // SendData


        public static async Task SendDataClientBest(string str)
        {
            string responseStep = "";
            try
            {
                NetworkStream networkStream = client.GetStream();
                var writer = new StreamWriter(networkStream);
                writer.AutoFlush = true;

                await writer.WriteLineAsync(str);

                //client.Close();
            }
            catch (Exception ex)
            {
                //return ex.Message;
            }
        } // SendData

    }
}
