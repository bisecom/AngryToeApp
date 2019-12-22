using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace AngryToe
{
    /// <summary>
    /// Логика взаимодействия для IPData.xaml
    /// </summary>
    public partial class IPData : Window
    {
        public IPData()
        {
            InitializeComponent();
            WindowDesign();
        }

        private void OkBtn_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = true;
        }

        private void CancelBtn_Click(object sender, RoutedEventArgs e)
        {
            this.Close();

        }
        private void WindowDesign()
        {
            var img = new Image { Width = 50, Height = 50 };
            var bitmapImage = new BitmapImage(new Uri(@"pack://application:,,,/AngryToe;component/Resources/Icon_blue_Jorge_0.png"));
            img.Source = bitmapImage;
            img.SetValue(Grid.RowProperty, 0);
            img.SetValue(Grid.ColumnProperty, 0);
            MainGrid.Children.Add(img);
        }

    }
}
