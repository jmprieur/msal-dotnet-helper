using Microsoft.Graph;
using Microsoft.Graph.Helpers;
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
        SingleUserPublicClientGraphApplication graphServiceClient = new SingleUserPublicClientGraphApplication(clientId, 
             "https://login.microsoftonline.com/organizations");


        public MainWindow()
        {
            InitializeComponent();

            // We want the user to control when to have interactions. The user will be notified when s/he needs
            // to sign-in, or consent for more
            graphServiceClient.InteractionRequired += GraphServiceClient_InteractionRequired;
        }

        private async void Window_Initialized(object sender, EventArgs e)
        {
            UpdateUI();

            // We don't await here. the picture will arrive when it arrives, that's fine
            await GetPictureAsync();
        }

        private void UpdateUI()
        {
            if (graphServiceClient.User == null)
            {
                this.user.Content = "No user is signed-in";
                signInSignOut.Content = "Get Picture";
            }
            else
            {
                this.user.Content = graphServiceClient.User.DisplayableId;
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
                graphServiceClient.SignOut();
                signInSignOut.Content = "Get Picture";
                image.Source = null;
            }
            else
            {
                await GetPictureAsync();
            }
            UpdateUI();
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
            UpdateUI();
        }

        private async void CredentialRequired_Click(object sender, RoutedEventArgs e)
        {
            graphServiceClient.AcceptInteraction = AcceptInteraction.Once;
            await GetPictureAsync();
        }


    }
}
