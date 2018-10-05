using Microsoft.Graph;
using Microsoft.Graph.Helpers;
using Microsoft.Identity.Client.Helpers;
using System;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media.Imaging;

namespace WpfAppCallingGraph
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        const string clientId = "145cec56-05b2-4764-a41c-b77466387462";
        SingleUserPublicClientGraphApplication graphServiceClient;

        public MainWindow()
        {
            InitializeComponent();

            // We want the user to control when to have interactions. The user will be notified when s/he needs
            // to sign-in, or consent for more
        }

        private async void Window_Initialized(object sender, EventArgs e)
        {
            graphServiceClient = new SingleUserPublicClientGraphApplication(clientId,
                                                new Authority(Audience.AcountsInAnyAzureAdDirectory));
            graphServiceClient.InteractionRequired += GraphServiceClient_InteractionRequired;

            await UpdateUI();

            // We don't await here. the picture will arrive when it arrives, that's fine
            await GetPictureAsync();
        }

        private async Task UpdateUI()
        {
            var account = await graphServiceClient.GetUserAsync();
            if (account == null)
            {
                this.user.Content = "No user is signed-in";
                signInSignOut.Content = "Get Picture";
            }
            else
            {
                this.user.Content = account.Username;
                signInSignOut.Content = "Sign-Out";
            }
        }

        private void GraphServiceClient_InteractionRequired(object sender, System.EventArgs e)
        {
            this.credentialRequired.Visibility = Visibility.Visible;
        }

        private async void SignInSignOut_Click(object sender, RoutedEventArgs e)
        {
            string label = signInSignOut.Content as string;
            if (label == "Sign-Out")
            {
                await graphServiceClient.SignOut();
                signInSignOut.Content = "Get Picture";
                image.Source = null;
            }
            else
            {
                await GetPictureAsync();
            }
            await UpdateUI();
        }

        private async Task GetPictureAsync()
        {
            graphServiceClient.Scopes.Add("User.Read");
            try
            {
                System.IO.Stream photoStream = await graphServiceClient.Me.Photo.Content.Request().GetAsync();
                this.image.Source = BitmapFrame.Create(photoStream);
                this.credentialRequired.Visibility = Visibility.Collapsed;
            }
            catch (Exception exception)
            {
                // this is expected in the case where an interaction is required and the application
                // as subscribed to the InteractionRequiredEvent
            }
            await UpdateUI();
        }

        private async void CredentialRequired_Click(object sender, RoutedEventArgs e)
        {
            graphServiceClient.AcceptInteraction = AcceptInteraction.Once;
            await GetPictureAsync();
        }


    }
}
