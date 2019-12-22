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
    /// Логика взаимодействия для ContinueRequest.xaml
    /// </summary>
    public partial class ContinueRequest : Window
    {
        public ContinueRequest()
        {
            InitializeComponent();
            WindowDesign();
        }

        private void YesButtonClick(object sender, RoutedEventArgs e)
        {
            this.DialogResult = true;
        }

        private void NoButtonClick(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void WindowDesign()
        {
            var img = new Image { Width = 50, Height = 50 };
            var bitmapImage = new BitmapImage(new Uri(@"pack://application:,,,/AngryToe;component/Resources/Icon_X.png"));
            img.Source = bitmapImage;
            img.SetValue(Grid.RowProperty, 0);
            img.SetValue(Grid.RowSpanProperty, 2);
            img.SetValue(Grid.ColumnProperty, 0);
            MainGridContinue.Children.Add(img);
        }


    }
}
