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
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace MathEquation
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private void calculate_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                string expr = mathExpression.Text;
                string valuable = mathValuable.Text;
                Parsing parser = new Parsing();
                double result = parser.Parse(expr, valuable);
                resultbx.Text = "";
                resultbx.Text += result.ToString();
            }
            catch(Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void clear_Click(object sender, RoutedEventArgs e)
        {
            mathExpression.Text = "";
            mathValuable.Text = "";
            resultbx.Text = "";
        }
    }
}
