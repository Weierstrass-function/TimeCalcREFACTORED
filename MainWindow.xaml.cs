using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace TimeCalcREFACTORED
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            ResultText.Text = int.MaxValue.ToString();
        }

        private void OnEnterClick(object sender, RoutedEventArgs e)
        {
            try
            {
                var tokens = Tokenizer.Tokenize(ResultText.Text);
                AST some = new(tokens);
                ResultText.Text = Evaluator.Evaluate(some.Root).ToString();
            }
            catch (Exception ex)
            {
                MessageBoxResult result = MessageBox.Show(
                $"Ошибка: {ex.Message}",
                "",
                MessageBoxButton.OK,
                MessageBoxImage.Error);
            }
        }

        private void ResultText_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                OnEnterClick(sender, e);
            }
        }

        private void OnKeyboardClick(object sender, RoutedEventArgs e)
        {
            char newChar = sender.ToString()[^1];
            int cursorPos = ResultText.SelectionStart;
            ResultText.Text = ResultText.Text.Insert(cursorPos, newChar.ToString());
            ResultText.SelectionStart = cursorPos + 1;
        }

        private void OnDelClick(object sender, RoutedEventArgs e)
        {
            if (!string.IsNullOrEmpty(ResultText.Text))
            {
                ResultText.Text = ResultText.Text[..^1];
            }
        }

        private void OnAllClearClick(object sender, RoutedEventArgs e)
        {
            ResultText.Text = "";
        }
    }
}