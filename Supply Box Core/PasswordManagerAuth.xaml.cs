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
using Windows.Security.Credentials.UI;

namespace Supply_Box_Core
{
    public partial class PasswordManagerAuth : Page
    {
        public PasswordManagerAuth()
        {
            InitializeComponent();
            AuthenticateWithWindowsHello(); // Automatically triggers Windows Hello on page load
        }

        private async void AuthenticateWithWindowsHello()
        {
            try
            {
                // Show message indicating authentication process
                AuthMessageText.Text = "Authenticating with Windows Hello...";

                // Trigger Windows Hello authentication process
                var result = await UserConsentVerifier.RequestVerificationAsync("Please verify your identity to proceed.");

                if (result == UserConsentVerificationResult.Verified)
                {
                    // Successful authentication, open next window
                    PasswordManagerMainWindow mainWindow = new PasswordManagerMainWindow();
                    mainWindow.Show();

                    // Close the current window
                    Window.GetWindow(this)?.Close();
                }
                else
                {
                    // Authentication failed, show retry button
                    AuthMessageText.Text = "Authentication failed. Please try again.";
                    LoginButton.Visibility = Visibility.Visible; // Show retry button
                }
            }
            catch (Exception ex)
            {
                // Handle error and show retry option
                AuthMessageText.Text = "Error: " + ex.Message;
                LoginButton.Visibility = Visibility.Visible; // Show retry button
            }
        }

        private void LoginButton_Click(object sender, RoutedEventArgs e)
        {
            // Hide button while retrying authentication
            LoginButton.Visibility = Visibility.Collapsed;
            AuthenticateWithWindowsHello(); // Retry authentication
        }
    }
}