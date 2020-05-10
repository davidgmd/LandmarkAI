using LandmarkAI.Classes;
using Microsoft.Win32;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
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
using static LandmarkAI.Classes.CustomView;

namespace LandmarkAI
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog dialog = new OpenFileDialog();
            //En filter primero indicamos el texto y separado por | indicamos las extensiones estas si se aplican
            dialog.Filter = "Image Files (*.png; *.jpg)|*.png;*.jpg;*.jpeg";
            dialog.InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.MyPictures);

            if (dialog.ShowDialog() == true)
            {
                string filename = dialog.FileName;
                selectedImage.Source = new BitmapImage(new Uri(filename));

                MakePredictionAsync(filename);
            }
        }

        private async void MakePredictionAsync(string filename)
        {
            string url = "https://westeurope.api.cognitive.microsoft.com/customvision/v3.0/Prediction/5a09f57d-6c7d-4383-a1d2-6b8611c1d9ac/classify/iterations/Iteration1/image";
            string predictionKey = "3fe71b3a24124dff8f7c30b6cf72c78f";
            string contentType = "application/octet-stream";
            var file = File.ReadAllBytes(filename);

            using (HttpClient client = new HttpClient())
            {
                client.DefaultRequestHeaders.Add("Prediction-Key", predictionKey);
                
                using(var content = new ByteArrayContent(file))
                {
                    content.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue(contentType);
                    var response = await client.PostAsync(url, content);

                    var responseString = await response.Content.ReadAsStringAsync();

                    List<Prediction> predicciones = (JsonConvert.DeserializeObject<CustomView>(responseString)).predictions;
                    predictionsListView.ItemsSource = predicciones;
                }
            }
        }
    }
}
