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
using ООО_обувь;
using System.Linq;
using ООО_обувь.Models;

namespace ООО_обувь
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        AppDbContext AppDbContext = new AppDbContext(); //инициализация контекста базы данных

        public MainWindow()
        {
            InitializeComponent();
           
        }

        private void Login_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var userLogin = AppDbContext.Users.FirstOrDefault(x => x.Login == TextBoxLogin.Text && x.Password == TextBoxPassword.Password); // поиск пользователя в системе

                if (userLogin != null)
                {
                    switch (userLogin.RoleId)
                    {
                        case 1:
                            //открытие формы администратора

                            adminWindow adminWindow = new adminWindow(userLogin); 
                            adminWindow.Show();
                            break;

                        case 2:
                            managerWindow managerWindow = new managerWindow(userLogin);
                            managerWindow.Show();
                            break;

                        case 3:
                            UserWindow userWindow = new UserWindow(userLogin);
                            userWindow.Show();
                            break;

                        default:
                            MessageBox.Show("Ошибка");
                            return;

                    }
                    
                   
                }
                else
                {
                    MessageBox.Show("неверный пользователь или пароль");
                    return;
                }
            }
            catch
            {
                MessageBox.Show("Ошибка");
                return;
            }
            
        }

        private void Guest_Click(object sender, RoutedEventArgs e)
        {
            GuesForms guesForms = new GuesForms();
            guesForms.Show();
            this.Close();
        }
    }
}