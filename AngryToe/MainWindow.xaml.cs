using AngryToe.ConnectionData;
using AngryToe.GameLogic;
using NAudio.Wave;
using System;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace AngryToe
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public bool ConnectionIsServer { get; set; }
        public string IpAddressData { get; set; }
        public int port { get; set; }
        public bool columnColapsed;
        public int ServerWorkingField { get; set; } // 3 or 5
        public int[,] WorkingField { get; set; }
        public bool firstCycle { get; set; }
        public bool firstCycleServer { get; set; }
        public bool soundTurnedOn { get; set; }
        public bool Server1Winner { get; set; }
        public bool Client2Winner { get; set; }
        public bool Requested3SizeField { get; set; }
        public string whomStep { get; set; }
        public string contrPlayerName { get; set; }
        public int Player1Scores { get; set; }
        public int Player2Scores { get; set; }
        private Grid InternalField_3x3_Grid { get; set; }

        private IWavePlayer waveOut;
        private Mp3FileReader mp3FileReader;


        public MainWindow()
        {
            InitializeComponent();
            port = 50001;
            columnColapsed = false;
            firstCycleServer = true;
            firstCycle = true;
            soundTurnedOn = false;
            InternalField_3x3_Grid = InternalField_3x3_Grid_Create(MainGrid);
        }

        public void radioButton_Checked(object sender, RoutedEventArgs e)
        {
            var radioButton = (RadioButton)sender; // checked RadioButton
            if (radioButton.Name == "ConnectRB")
            {
                IPData myTpData = new IPData();
                if (myTpData.ShowDialog() == true)
                {
                    try
                    {
                        //MessageBox.Show(ServerTCP.GetLocalIPAddress());
                        if (ServerTCP.GetLocalIPAddress() == myTpData.GettingIpTbx.Text)
                        {
                            ConnectionIsServer = true;
                            ServerTCP.InitializeServer(myTpData.GettingIpTbx.Text, port);
                        }
                        else
                        {
                            ConnectionIsServer = false;
                        }
                        IpAddressData = myTpData.GettingIpTbx.Text;
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show(ex.Message);
                    }
                }
            }

        }



        public void AddingButtonsToField(int number, Grid obj)
        {
            int count = 1;
            var st = Application.Current.Resources["CustomButtonStyle"] as Style;
            for (int i = 0; i < number; i++)
            {
                for (int j = 0; j < number; j++)
                {
                    Button MyControl1 = new Button();
                    MyControl1.Name = "I" + i.ToString() + "J" + j.ToString();
                    MyControl1.Style = st;
                   
                    MyControl1.Click += DynamicButtonClick;
                    Grid.SetColumn(MyControl1, j);
                    Grid.SetRow(MyControl1, i);
                    obj.Children.Add(MyControl1);
                    count++;
                }
            }
        }

        public Grid InternalField_3x3_Grid_Create(Grid parrent)
        {
            Grid DynamicGrid = new Grid();
            // Create Columns
            ColumnDefinition gridCol1 = new ColumnDefinition();
            ColumnDefinition gridCol2 = new ColumnDefinition();
            ColumnDefinition gridCol3 = new ColumnDefinition();
            DynamicGrid.ColumnDefinitions.Add(gridCol1);
            DynamicGrid.ColumnDefinitions.Add(gridCol2);
            DynamicGrid.ColumnDefinitions.Add(gridCol3);

            // Create Rows
            RowDefinition gridRow1 = new RowDefinition();
            RowDefinition gridRow2 = new RowDefinition();
            RowDefinition gridRow3 = new RowDefinition();
            DynamicGrid.RowDefinitions.Add(gridRow1);
            DynamicGrid.RowDefinitions.Add(gridRow2);
            DynamicGrid.RowDefinitions.Add(gridRow3);

            Grid.SetColumn(DynamicGrid, 1);
            Grid.SetRow(DynamicGrid, 1);
            Grid.SetColumnSpan(DynamicGrid, 4);
            Grid.SetRowSpan(DynamicGrid, 10);
            parrent.Children.Add(DynamicGrid);

            return DynamicGrid;
        }


        public async void StartButtonClick(object sender, RoutedEventArgs e)
        {
            
            await StartButtonRunAsync();

        }


        public async Task StartButtonRunAsync()
        {
            int threeCells = 3; int fiveCells = 5; firstCycle = true; firstCycleServer = true;
               if (InternalField_3x3_Grid.Children.Count > 0)
                    InternalField_3x3_Grid.Children.Clear();
               if (InternalField_5x5_Grid.Children.Count > 0)
                    InternalField_5x5_Grid.Children.Clear();
            Server1Winner = false; Client2Winner = false; whomStep = "";
            if (ThreeFieldRB.IsChecked == true)
            {
                //MainGrid.Children.Remove(InternalField_5x5_Grid);
                InternalField_3x3_Grid = InternalField_3x3_Grid_Create(MainGrid);
                AddingButtonsToField(threeCells, InternalField_3x3_Grid);
                WorkingField = new int[3, 3];

                for (int i = 0; i < threeCells; i++)
                {
                    for (int j = 0; j < threeCells; j++)
                    {
                        WorkingField[i, j] = 0;
                    }
                }

            }
            if (FiveFieldRB.IsChecked == true)
            {
                //MainGrid.Children.Remove(InternalField_3x3_Grid);
                AddingButtonsToField(fiveCells, InternalField_5x5_Grid);
                WorkingField = new int[5, 5];

                for (int i = 0; i < fiveCells; i++)
                {
                    for (int j = 0; j < fiveCells; j++)
                    {
                        WorkingField[i, j] = 0;
                    }
                }
            }

            if (NewGameRB.IsChecked == true)//server
            {

            }
            if (ConnectRB.IsChecked == true)//client
            {

            }

            //MessageBox.Show(ServerTCP.GetLocalIPAddress());
            if (ConnectionIsServer == false)
            {
                if (UserNameTB.Text != "")
                { ClientTCP.ClientUserName = UserNameTB.Text; Player2Name.Text = UserNameTB.Text; ScoresPlayer2.Text = "0"; }
                if (UserNameTB.Text == "")
                { MessageBox.Show("Please, enter Name!"); return; }

                try
                {
                    await ClientTCP.WriteLabelBestAsync(IpAddressData, port, WhomStepTB, Requested3SizeField, WorkingField, FiveFieldRB, ThreeFieldRB, ServerTCP.ServUserName, Player1Name);
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
                }
            }
            if (ConnectionIsServer == true)
            {
                if (UserNameTB.Text != "")
                { ServerTCP.ServUserName = UserNameTB.Text; Player1Name.Text = UserNameTB.Text; ScoresPlayer1.Text = "0"; }
                if (UserNameTB.Text == "")
                { MessageBox.Show("Please, enter Name!"); return; }

                if (ThreeFieldRB.IsChecked == true)
                    ServerWorkingField = 3;
                if (FiveFieldRB.IsChecked == true)
                    ServerWorkingField = 5; // To pass to Client and set correct field size

                try
                {

                    WhomStepTB.Text = "Step for Player: 2";
                    var serverRun = await ServerTCP.RunBest(WhomStepTB, Requested3SizeField, WorkingField, FiveFieldRB, ThreeFieldRB, ServerTCP.ServUserName, ClientTCP.ClientUserName, Player2Name, whomStep);
                    
                    Button myDynamicButton = new Button();
                    if (ThreeFieldRB.IsChecked == true)
                        myDynamicButton = ServerTCP.FindChild<Button>(InternalField_3x3_Grid, serverRun);
                    if (FiveFieldRB.IsChecked == true)
                        myDynamicButton = ServerTCP.FindChild<Button>(InternalField_5x5_Grid, serverRun);

                    if (AngryRB.IsChecked == true)
                    {
                        myDynamicButton.Content = new Image
                        {
                            Source = new BitmapImage(new Uri("pack://application:,,,/AngryToe;component/Resources/Icon_red_brd_X.png")),
                            VerticalAlignment = VerticalAlignment.Center,
                            Height = 50
                        };
                    }

                    if (PeppaRB.IsChecked == true)
                    {
                        myDynamicButton.Content = new Image
                        {
                            Source = new BitmapImage(new Uri("pack://application:,,,/AngryToe;component/Resources/Icon_peppa_X.png")),
                            VerticalAlignment = VerticalAlignment.Center,
                            Height = 50
                        };
                    }

                    if (ClassicRB.IsChecked == true)
                    {
                        myDynamicButton.Content = new Image
                        {
                            Source = new BitmapImage(new Uri("pack://application:,,,/AngryToe;component/Resources/Icon_X.png")),
                            VerticalAlignment = VerticalAlignment.Center,
                            Height = 50
                        };
                    }

                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
                }

                firstCycleServer = false;
            }
        }




        public async void DynamicButtonClick(object sender, RoutedEventArgs e)
        {
            Button clicked = (Button)sender;
            int clicked_i_Index = 0;
            int clicked_j_Index = 0;
            string temp = "";
            string playerStepAtFieldOrCases;
            string fieldSizeOrCases = "";
            string[] dataSetArr = new string[4];

            temp = clicked.Name.Remove(0, 1);
            int index = temp.IndexOf("J");
            if (index > 0)
                clicked_i_Index = Convert.ToInt32(temp.Substring(0, index));
            clicked_j_Index = Convert.ToInt32(clicked.Name.Substring(clicked.Name.IndexOf("J") + 1));

            if (ConnectionIsServer == true)
            {
                if (whomStep == "S2" || WhomStepTB.Text == "Step for Player: 2") return;
                if (WorkingField[clicked_i_Index, clicked_j_Index] == 0)
                {

                    if (AngryRB.IsChecked == true)
                    {
                        clicked.Content = new Image
                        {
                            Source = new BitmapImage(new Uri("pack://application:,,,/AngryToe;component/Resources/Icon_blue_brd_0.png")),
                            VerticalAlignment = VerticalAlignment.Center,
                            Height = 50
                        };
                    }

                    if (PeppaRB.IsChecked == true)
                    {
                        clicked.Content = new Image
                        {
                            Source = new BitmapImage(new Uri("pack://application:,,,/AngryToe;component/Resources/Icon_blue_Jorge_0.png")),
                            VerticalAlignment = VerticalAlignment.Center,
                            Height = 50
                        };
                    }

                    if (ClassicRB.IsChecked == true)
                    {
                        clicked.Content = new Image
                        {
                            Source = new BitmapImage(new Uri("pack://application:,,,/AngryToe;component/Resources/Icon_0.png")),
                            VerticalAlignment = VerticalAlignment.Center,
                            Height = 50
                        };
                    }

                    WorkingField[clicked_i_Index, clicked_j_Index] = 1;
                   

                    // Winner Check//-------------------------------------------------------------------------------------------
                    if (ThreeFieldRB.IsChecked == true)
                    {
                        if (WinnersCheck.Check_all_rows(WorkingField) == 1 || WinnersCheck.Check_all_diagonals3(WorkingField) == 1)
                        {
                            Server1Winner = true;
                            MessageBox.Show("Congratulation, Winner is Player #1!");
                        }
                        if (WinnersCheck.Check_all_rows(WorkingField) == 2 || WinnersCheck.Check_all_diagonals3(WorkingField) == 2)
                        {
                            Client2Winner = true;
                            MessageBox.Show("Congratulation, Winner is Player #2!");
                        }
                    }
                    if (FiveFieldRB.IsChecked == true)
                    {
                        if (WinnersCheck.DglRightToLeft5(WorkingField, 5) == 1 || WinnersCheck.DglLeftToRight5(WorkingField) == 1 || WinnersCheck.Check_all_rows5(WorkingField) == 1)
                        {
                            Server1Winner = true;
                            MessageBox.Show("Congratulation, Winner is Player #1!");
                        }
                        if (WinnersCheck.DglRightToLeft5(WorkingField, 5) == 2 || WinnersCheck.DglLeftToRight5(WorkingField) == 2 || WinnersCheck.Check_all_rows5(WorkingField) == 2)
                        {
                            Client2Winner = true;
                            MessageBox.Show("Congratulation, Winner is Player #2!");
                        }
                    }

                    if (Server1Winner == true)
                    {
                        fieldSizeOrCases = "WN"; Player1Scores++; ScoresPlayer1.Text = Player1Scores.ToString();
                    }
                    if (Server1Winner == false && Client2Winner == false)
                    {
                        playerStepAtFieldOrCases = clicked.Name;
                    }
                    if (FiveFieldRB.IsChecked == true && Server1Winner == false)
                    {
                        fieldSizeOrCases = "F5";
                    }
                    if (ThreeFieldRB.IsChecked == true && Server1Winner == false)
                    {
                        fieldSizeOrCases = "F3";
                    }

                    whomStep = "S2";
                    if (whomStep == "S2" && firstCycleServer == true) WhomStepTB.Text = "Step for Player: 2";
                    if (whomStep == "S2" && firstCycleServer == false) WhomStepTB.Text = "Step for Player:  " + Player2Name.Text;

                    string temp1 = await ServerTCP.SendDataAsync(whomStep + fieldSizeOrCases + clicked.Name);

                    if (temp1 != "")
                    {
                        dataSetArr = ServerTCP.DataParsing(temp1);
                        if (dataSetArr[0] == "S1")
                        {
                            WhomStepTB.Text = "Step for Player: " + Player1Name.Text;
                            whomStep = "S1";
                        }
                        if (dataSetArr[1] == "F3") Requested3SizeField = true;
                        if (dataSetArr[1] == "F5") Requested3SizeField = false;
                        if (dataSetArr[2].IndexOf("I") == 0 && dataSetArr[2].IndexOf("J") == 2)
                        {
                            temp = dataSetArr[2].Remove(0, 1);
                            index = temp.IndexOf("J");
                            if (index > 0)
                                clicked_i_Index = Convert.ToInt32(temp.Substring(0, index));
                            clicked_j_Index = Convert.ToInt32(dataSetArr[2].Substring(dataSetArr[2].IndexOf("J") + 1));

                            WorkingField[clicked_i_Index, clicked_j_Index] = 2;
                            Button myDynamicButton = new Button();
                            if (ThreeFieldRB.IsChecked == true)
                                myDynamicButton = ServerTCP.FindChild<Button>(InternalField_3x3_Grid, dataSetArr[2]);
                            if (FiveFieldRB.IsChecked == true)
                                myDynamicButton = ServerTCP.FindChild<Button>(InternalField_5x5_Grid, dataSetArr[2]);

                            if (AngryRB.IsChecked == true)
                            {
                                myDynamicButton.Content = new Image
                                {
                                    Source = new BitmapImage(new Uri("pack://application:,,,/AngryToe;component/Resources/Icon_red_brd_X.png")),
                                    VerticalAlignment = VerticalAlignment.Center,
                                    Height = 50
                                };
                            }

                            if (PeppaRB.IsChecked == true)
                            {
                                myDynamicButton.Content = new Image
                                {
                                    Source = new BitmapImage(new Uri("pack://application:,,,/AngryToe;component/Resources/Icon_peppa_X.png")),
                                    VerticalAlignment = VerticalAlignment.Center,
                                    Height = 50
                                };
                            }

                            if (ClassicRB.IsChecked == true)
                            {
                                myDynamicButton.Content = new Image
                                {
                                    Source = new BitmapImage(new Uri("pack://application:,,,/AngryToe;component/Resources/Icon_X.png")),
                                    VerticalAlignment = VerticalAlignment.Center,
                                    Height = 50
                                };
                            }
                        }

                        if (dataSetArr[1] == "WN")
                        {
                            Player2Scores++; ScoresPlayer2.Text = Player2Scores.ToString();
                            ContinueRequest myContinue = new ContinueRequest();
                            if (myContinue.ShowDialog() == true)
                            {
                                try
                                {
                                    WhomStepTB.Text = "Step for Player: " + Player2Name.Text;
                                    whomStep = "S2";
                                    await ServerTCP.RequestToContinueAsync(whomStep + fieldSizeOrCases + "YESX", WorkingField, WhomStepTB, Player1Name);
                        
                                    InternalField_3x3_Grid.Children.Clear();
                                    InternalField_5x5_Grid.Children.Clear();
                                    await StartButtonRunAsync();

                                }
                                catch (Exception ex)
                                {
                                    MessageBox.Show(ex.Message);
                                }

                            }
                            else
                            {
                                try
                                {
                                    string temp2 = await ServerTCP.SendDataAsync(whomStep + fieldSizeOrCases + "NOXX");
                                    ServerTCP.tcpClient.Close();
                                }
                                catch (Exception ex)
                                {
                                    MessageBox.Show(ex.Message);
                                }
                            }

                        }

                        if (dataSetArr[2] == "YESX")
                        {
                            ServerTCP.tcpClient.Close();
                            await StartButtonRunAsync();
                        }

                        if (dataSetArr[2] == "NOXX")
                        {
                            ClientTCP.client.Close();
                        }

                    }
                }

                else
                    MessageBox.Show("Not correct step!");
            }

            if (ConnectionIsServer == false /*&& ClientTB.Text == "Step for X"*/)
            {
                if (whomStep == "S1" || WhomStepTB.Text == "Step for Player: 1") return;

                if (WorkingField[clicked_i_Index, clicked_j_Index] == 0)
                {

                    if (AngryRB.IsChecked == true)
                    {
                        clicked.Content = new Image
                        {
                            Source = new BitmapImage(new Uri("pack://application:,,,/AngryToe;component/Resources/Icon_red_brd_X.png")),
                            VerticalAlignment = VerticalAlignment.Center,
                            Height = 50
                        };
                    }

                    if (PeppaRB.IsChecked == true)
                    {
                        clicked.Content = new Image
                        {
                            Source = new BitmapImage(new Uri("pack://application:,,,/AngryToe;component/Resources/Icon_peppa_X.png")),
                            VerticalAlignment = VerticalAlignment.Center,
                            Height = 50
                        };
                    }

                    if (ClassicRB.IsChecked == true)
                    {
                        clicked.Content = new Image
                        {
                            Source = new BitmapImage(new Uri("pack://application:,,,/AngryToe;component/Resources/Icon_X.png")),
                            VerticalAlignment = VerticalAlignment.Center,
                            Height = 50
                        };
                    }

                    WorkingField[clicked_i_Index, clicked_j_Index] = 2;

                    // Winner Check//-------------------------------------------------------------------------------------------
                    if (ThreeFieldRB.IsChecked == true)
                    {
                        if (WinnersCheck.Check_all_rows(WorkingField) == 1 || WinnersCheck.Check_all_diagonals3(WorkingField) == 1)
                        {
                            Server1Winner = true;
                            MessageBox.Show("Congratulation, Winner is Player #1!");
                        }
                        if (WinnersCheck.Check_all_rows(WorkingField) == 2 || WinnersCheck.Check_all_diagonals3(WorkingField) == 2)
                        {
                            Client2Winner = true;
                            MessageBox.Show("Congratulation, Winner is Player #2!");
                        }
                    }
                    if (FiveFieldRB.IsChecked == true)
                    {
                        if (WinnersCheck.DglRightToLeft5(WorkingField, 5) == 1 || WinnersCheck.DglLeftToRight5(WorkingField) == 1 || WinnersCheck.Check_all_rows5(WorkingField) == 1)
                        {
                            Server1Winner = true;
                            MessageBox.Show("Congratulation, Winner is Player #1!");
                        }
                        if (WinnersCheck.DglRightToLeft5(WorkingField, 5) == 2 || WinnersCheck.DglLeftToRight5(WorkingField) == 2 || WinnersCheck.Check_all_rows5(WorkingField) == 2)
                        {
                            Client2Winner = true;
                            MessageBox.Show("Congratulation, Winner is Player #2!");
                        }
                    }

                    if (Client2Winner == true)
                    {
                        fieldSizeOrCases = "WN"; Player2Scores++; ScoresPlayer2.Text = Player2Scores.ToString();
                    }
                    if (Server1Winner == false && Client2Winner == false)
                    {
                        playerStepAtFieldOrCases = clicked.Name;
                    }
                    if (FiveFieldRB.IsChecked == true && Client2Winner == false)
                    {
                        fieldSizeOrCases = "F5";
                    }
                    if (ThreeFieldRB.IsChecked == true && Client2Winner == false)
                    {
                        fieldSizeOrCases = "F3";
                    }
                    whomStep = "S1"; if (whomStep == "S1") WhomStepTB.Text = "Step for Player: " + Player1Name.Text;
                    if (firstCycle == true)
                    {
                        await ClientTCP.SendDataClientBest(whomStep + fieldSizeOrCases + clicked.Name + "&" + Player2Name.Text);
                    }
                    if (firstCycle == false)
                    {
                        await ClientTCP.SendDataClientBest(whomStep + fieldSizeOrCases + clicked.Name);
                    }

                    string temp1 = await ClientTCP.ReceiveDataClientBest();

                    if (temp1 != "")
                    {
                        dataSetArr = ServerTCP.DataParsing(temp1);
                        if (dataSetArr[0] == "S2")
                        {
                            WhomStepTB.Text = "Step for Player: " + Player2Name.Text;
                            whomStep = "S2";
                        }
                        if (dataSetArr[1] == "F3") Requested3SizeField = true;
                        if (dataSetArr[1] == "F5") Requested3SizeField = false;
                        if (dataSetArr[2].IndexOf("I") == 0 && dataSetArr[2].IndexOf("J") == 2)
                        {
                            temp = dataSetArr[2].Remove(0, 1);
                            index = temp.IndexOf("J");
                            if (index > 0)
                                clicked_i_Index = Convert.ToInt32(temp.Substring(0, index));
                            clicked_j_Index = Convert.ToInt32(dataSetArr[2].Substring(dataSetArr[2].IndexOf("J") + 1));

                            WorkingField[clicked_i_Index, clicked_j_Index] = 1;
                            //MessageBox.Show(WhomStepTB.Text + " " + Requested3SizeField.ToString() + " " + clicked_i_Index.ToString() + " " + clicked_j_Index.ToString());
                            firstCycle = false; // needed to pass name first time
                            Button myDynamicButton = new Button();
                            if (ThreeFieldRB.IsChecked == true)
                                myDynamicButton = ServerTCP.FindChild<Button>(InternalField_3x3_Grid, dataSetArr[2]);
                            if (FiveFieldRB.IsChecked == true)
                                myDynamicButton = ServerTCP.FindChild<Button>(InternalField_5x5_Grid, dataSetArr[2]);

                            if (AngryRB.IsChecked == true)
                            {
                                myDynamicButton.Content = new Image
                                {
                                    Source = new BitmapImage(new Uri("pack://application:,,,/AngryToe;component/Resources/Icon_blue_brd_0.png")),
                                    VerticalAlignment = VerticalAlignment.Center,
                                    Height = 50
                                };
                            }

                            if (PeppaRB.IsChecked == true)
                            {
                                myDynamicButton.Content = new Image
                                {
                                    Source = new BitmapImage(new Uri("pack://application:,,,/AngryToe;component/Resources/Icon_blue_Jorge_0.png")),
                                    VerticalAlignment = VerticalAlignment.Center,
                                    Height = 50
                                };
                            }

                            if (ClassicRB.IsChecked == true)
                            {
                                myDynamicButton.Content = new Image
                                {
                                    Source = new BitmapImage(new Uri("pack://application:,,,/AngryToe;component/Resources/Icon_0.png")),
                                    VerticalAlignment = VerticalAlignment.Center,
                                    Height = 50
                                };
                            }

                        }

                        if (dataSetArr[1] == "WN")
                        {
                            Player1Scores++; ScoresPlayer1.Text = Player1Scores.ToString();
                            ContinueRequest myContinue = new ContinueRequest();
                            if (myContinue.ShowDialog() == true)
                            {
                                try
                                {
                                    await ClientTCP.RequestToContinueAsync(whomStep + fieldSizeOrCases + "YESX");
                                    int threeCells = 3; int fiveCells = 5;
                                    InternalField_3x3_Grid.Children.Clear();
                                    InternalField_5x5_Grid.Children.Clear();
                                    Server1Winner = false; Client2Winner = false;
                                    if (ThreeFieldRB.IsChecked == true)
                                    {
                                        AddingButtonsToField(threeCells, InternalField_3x3_Grid);
                                        WorkingField = new int[3, 3];

                                        for (int i = 0; i < threeCells; i++)
                                        {
                                            for (int j = 0; j < threeCells; j++)
                                            {
                                                WorkingField[i, j] = 0;
                                            }
                                        }

                                    }
                                    if (FiveFieldRB.IsChecked == true)
                                    {
                                        AddingButtonsToField(fiveCells, InternalField_5x5_Grid);
                                        WorkingField = new int[5, 5];

                                        for (int i = 0; i < fiveCells; i++)
                                        {
                                            for (int j = 0; j < fiveCells; j++)
                                            {
                                                WorkingField[i, j] = 0;
                                            }
                                        }
                                    }
                                }
                                catch (Exception ex)
                                {
                                    MessageBox.Show(ex.Message);
                                }

                            }
                            else
                            {
                                try
                                {
                                    await ClientTCP.RequestToContinueAsync(whomStep + fieldSizeOrCases + "NOXX");
                                    ClientTCP.client.Close();
                                }
                                catch (Exception ex)
                                {
                                    MessageBox.Show(ex.Message);
                                }
                            }

                        }

                        if (dataSetArr[2] == "YESX")
                        {
                            InternalField_3x3_Grid.Children.Clear();
                            InternalField_5x5_Grid.Children.Clear();
                            Server1Winner = false; Client2Winner = false; firstCycle = true;
                            ClientTCP.client.Close();
                            ScoresPlayer1.Text = Player1Scores.ToString();
                            ScoresPlayer2.Text = Player2Scores.ToString();
                            //Thread.Sleep(2000);
                            await StartButtonRunAsync();

                            ScoresPlayer1.Text = Player1Scores.ToString();
                            ScoresPlayer2.Text = Player2Scores.ToString();
                           
                        }

                        if (dataSetArr[2] == "NOXX")
                        {
                            ClientTCP.client.Close();
                        }

                    }

                }
                else
                    MessageBox.Show("Not correct step!");
            }

        }


        private void ColapsButtonClick(object sender, RoutedEventArgs e)
        {

            if (columnColapsed == false)
            {
                ColapsColumn.Width = new GridLength(0, GridUnitType.Pixel);
                columnColapsed = true;
                myToe.Width = 440;
                ColapseBtn.Content = "Extend";
            }
            else
            {
                ColapsColumn.Width = new GridLength(210, GridUnitType.Pixel);
                columnColapsed = false;
                myToe.Width = 630;
                ColapseBtn.Content = "Colapse";
            }
        }

        private void SoundButtonClick(object sender, EventArgs e)
        {
            if (soundTurnedOn == false)
            {
                PlayMp3();
                SoundBtn.Content = "Tune Off";
                soundTurnedOn = true;
                return;
            }
            if (soundTurnedOn == true)
            {
                SoundBtn.Content = "Tune On";
                soundTurnedOn = false;
                this.waveOut.Dispose();
                this.mp3FileReader.Dispose();
            }

        }

        private void GameTypeRB_Checked(object sender, RoutedEventArgs e)
        {
            RadioButton checkedRB = sender as RadioButton;
            if (checkedRB.Name == "AngryRB")
            {
                var uri = new Uri("AngryTheme.xaml", UriKind.Relative);
                // загружаем словарь ресурсов
                ResourceDictionary resourceDict = Application.LoadComponent(uri) as ResourceDictionary;
                // очищаем коллекцию ресурсов приложения
                Application.Current.Resources.Clear();
                // добавляем загруженный словарь ресурсов
                Application.Current.Resources.MergedDictionaries.Add(resourceDict);

            }

            if (checkedRB.Name == "PeppaRB")
            {
                var uri = new Uri("PeppaTheme.xaml", UriKind.Relative);
                // загружаем словарь ресурсов
                ResourceDictionary resourceDict = Application.LoadComponent(uri) as ResourceDictionary;
                // очищаем коллекцию ресурсов приложения
                Application.Current.Resources.Clear();
                // добавляем загруженный словарь ресурсов
                Application.Current.Resources.MergedDictionaries.Add(resourceDict);
            }

            if (checkedRB.Name == "ClassicRB")
            {
                var uri = new Uri("ClassicTheme.xaml", UriKind.Relative);
                // загружаем словарь ресурсов
                ResourceDictionary resourceDict = Application.LoadComponent(uri) as ResourceDictionary;
                // очищаем коллекцию ресурсов приложения
                Application.Current.Resources.Clear();
                // добавляем загруженный словарь ресурсов
                Application.Current.Resources.MergedDictionaries.Add(resourceDict);
            }

        }


        private void GameFieldSizeTypeRB_Checked(object sender, RoutedEventArgs e)
        {
            RadioButton checkedRB = sender as RadioButton;

            if (ClassicRB.IsChecked == true && checkedRB.Name == "ThreeFieldRB")
            {
                ImageBrush myBrush = new ImageBrush();
                var img = new Image
                {
                    Source = new BitmapImage(new Uri("pack://application:,,,/AngryToe;component/Resources/Classic3x3Filed.png")),
                    VerticalAlignment = VerticalAlignment.Center
                };
                myBrush.ImageSource = img.Source;
                InternalField_5x5_Grid.Background = myBrush;
            }

            if (ClassicRB.IsChecked == true && checkedRB.Name == "FiveFieldRB")
            {
                ImageBrush myBrush = new ImageBrush();
                var img = new Image
                {
                    Source = new BitmapImage(new Uri("pack://application:,,,/AngryToe;component/Resources/Classic5x5Filed.png")),
                    VerticalAlignment = VerticalAlignment.Center
                };
                myBrush.ImageSource = img.Source;
                InternalField_5x5_Grid.Background = myBrush;
            }

            if (AngryRB.IsChecked == true && checkedRB.Name == "ThreeFieldRB")
            {
                ImageBrush myBrush = new ImageBrush();
                var img = new Image
                {
                    Source = new BitmapImage(new Uri("pack://application:,,,/AngryToe;component/Resources/Angry3x3Filed.png")),
                    VerticalAlignment = VerticalAlignment.Center
                };
                myBrush.ImageSource = img.Source;
                InternalField_5x5_Grid.Background = myBrush;
            }

            if (AngryRB.IsChecked == true && checkedRB.Name == "FiveFieldRB")
            {
                ImageBrush myBrush = new ImageBrush();
                var img = new Image
                {
                    Source = new BitmapImage(new Uri("pack://application:,,,/AngryToe;component/Resources/Angry5x5Filed.png")),
                    VerticalAlignment = VerticalAlignment.Center
                };
                myBrush.ImageSource = img.Source;
                InternalField_5x5_Grid.Background = myBrush;
            }
            if (PeppaRB.IsChecked == true && checkedRB.Name == "ThreeFieldRB")
            {
                ImageBrush myBrush = new ImageBrush();
                var img = new Image
                {
                    Source = new BitmapImage(new Uri("pack://application:,,,/AngryToe;component/Resources/Peppa3x3Filed.png")),
                    VerticalAlignment = VerticalAlignment.Center
                };
                myBrush.ImageSource = img.Source;
                InternalField_5x5_Grid.Background = myBrush;
            }

            if (PeppaRB.IsChecked == true && checkedRB.Name == "FiveFieldRB")
            {
                ImageBrush myBrush = new ImageBrush();
                var img = new Image
                {
                    Source = new BitmapImage(new Uri("pack://application:,,,/AngryToe;component/Resources/Peppa5x5Filed.png")),
                    VerticalAlignment = VerticalAlignment.Center
                };
                myBrush.ImageSource = img.Source;
                InternalField_5x5_Grid.Background = myBrush;
            }

        }

        private void PlayMp3()
        {
            this.waveOut = new WaveOut(); // or new WaveOutEvent() 
            this.mp3FileReader = new Mp3FileReader("../../Monkey-Island-Band.mp3");
            this.waveOut.Init(mp3FileReader);
            this.waveOut.Play();
            this.waveOut.PlaybackStopped += SoundButtonClick;

        }


    }
}
