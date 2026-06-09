using DemoExz22.DataBase;
using DemoExz22.Helpers;
using DemoExz22.Statics;
using System.Linq;
using System.Windows;

namespace DemoExz22
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private ShoeShopDBEntities db = new ShoeShopDBEntities();
        private MessageHelper messageHelper = new MessageHelper();
        public MainWindow()
        {
            InitializeComponent();
        }

        private void LoginButton_Click(object sender, RoutedEventArgs e)
        {
            string login = LoginTextBox.Text;
            string password = PasswordBox.Password;

            var user = db.Users.Where(u => u.Login == login && u.Password == password).FirstOrDefault();

            if (user == null)
            {
                messageHelper.ShowError("Неверный логин или пароль.");
                return;
            }
            else
            {
                CurrentSession.CurrentUser = user;
                new ProductWindow().Show();
                Close();
            }
        }

        private void TextBlock_MouseDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            new ProductWindow().Show();
            Close();
        }
    }
}
