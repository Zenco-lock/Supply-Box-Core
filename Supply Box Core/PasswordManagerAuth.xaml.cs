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
        // Flag to track Windows Hello availability
        private bool isHelloAvailable = false;

        public PasswordManagerAuth()
        {
            InitializeComponent();

            // On startup, check if Windows Hello is available
            CheckWindowsHelloAvailability();
        }

        // Checks Windows Hello availability and handles the flow accordingly
        private async void CheckWindowsHelloAvailability()
        {
            // Query Windows Hello availability
            var availability = await UserConsentVerifier.CheckAvailabilityAsync();

            if (availability == UserConsentVerifierAvailability.Available)
            {
                isHelloAvailable = true;
                // If available, start biometric authentication automatically
                AuthenticateWithWindowsHello();
            }
            else
            {
                isHelloAvailable = false;
                // Show recommendation to enable Windows Hello
                AuthMessageText.Text =
                    "Windows Hello is not enabled on this device.\n" +
                    "To enhance security, enable Windows Hello or set a password " +
                    "in Settings > Accounts > Sign-in options.\n\n" +
                    "Continuing without Windows Hello is possible, however, enabling it later is highly recommended.";
                // Display Continue button even without Windows Hello
                LoginButton.Content = "Continue";
                LoginButton.Visibility = Visibility.Visible;
            }
        }

        // Windows Hello authentication flow
        private async void AuthenticateWithWindowsHello()
        {
            try
            {
                // Update UI and hide button during authentication attempt
                AuthMessageText.Text = "Authenticating with Windows Hello...";
                LoginButton.Visibility = Visibility.Collapsed;

                // Request biometric verification from the user
                var result = await UserConsentVerifier.RequestVerificationAsync(
                    "Please verify your identity to proceed.");

                if (result == UserConsentVerificationResult.Verified)
                {
                    // If verified, open main window and close the current one
                    var mainWindow = new PasswordManagerMainWindow();
                    mainWindow.Show();
                    Window.GetWindow(this)?.Close();
                }
                else
                {
                    // On failure, show message and allow retry
                    AuthMessageText.Text = "Authentication failed. Please try again.";
                    LoginButton.Content = "Try Again";
                    LoginButton.Visibility = Visibility.Visible;
                }
            }
            catch (Exception ex)
            {
                // On unexpected error, display exception message and allow retry
                AuthMessageText.Text = "Error: " + ex.Message;
                LoginButton.Content = "Try Again";
                LoginButton.Visibility = Visibility.Visible;
            }
        }

        // Click event for the login/continue button
        private void LoginButton_Click(object sender, RoutedEventArgs e)
        {
            if (!isHelloAvailable)
            {
                // If Hello is not available and user clicked Continue, open main window
                var mainWindow = new PasswordManagerMainWindow();
                mainWindow.Show();
                Window.GetWindow(this)?.Close();
            }
            else
            {
                // If Hello is available, hide button and retry authentication
                LoginButton.Visibility = Visibility.Collapsed;
                AuthenticateWithWindowsHello();
            }
        }
    }
}