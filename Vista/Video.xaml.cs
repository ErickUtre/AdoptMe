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
using System.Windows.Threading;

namespace Cliente_AdoptMe.Vista
{
    /// <summary>
    /// Lógica de interacción para Video.xaml
    /// </summary>
    public partial class Video : Window
    {
        private DispatcherTimer timer;
        private bool sliderEnUso = false;

        public Video(string rutaVideo)
        {
            InitializeComponent();

            if (!string.IsNullOrEmpty(rutaVideo))
            {
                mediaPlayer.Source = new Uri(rutaVideo);
                mediaPlayer.Play();
            }

            // Inicializar timer
            timer = new DispatcherTimer();
            timer.Interval = TimeSpan.FromMilliseconds(500);
            timer.Tick += Timer_Tick;
        }

        private void mediaPlayer_MediaOpened(object sender, RoutedEventArgs e)
        {
            if (mediaPlayer.NaturalDuration.HasTimeSpan)
            {
                sliderProgreso.Maximum = mediaPlayer.NaturalDuration.TimeSpan.TotalSeconds;
                timer.Start();
            }
        }

        private void Timer_Tick(object sender, EventArgs e)
        {
            if (!sliderEnUso && mediaPlayer.NaturalDuration.HasTimeSpan)
            {
                sliderProgreso.Value = mediaPlayer.Position.TotalSeconds;
            }
        }

        private void Slider_PreviewMouseDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            sliderEnUso = true;
        }

        private void Slider_PreviewMouseUp(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            mediaPlayer.Position = TimeSpan.FromSeconds(sliderProgreso.Value);
            sliderEnUso = false;
        }

        private void BtnReproducir_Click(object sender, RoutedEventArgs e)
        {
            mediaPlayer.Play();
        }

        private void BtnPausar_Click(object sender, RoutedEventArgs e)
        {
            mediaPlayer.Pause();
        }

        private void BtnDetener_Click(object sender, RoutedEventArgs e)
        {
            mediaPlayer.Stop();
            sliderProgreso.Value = 0;
        }

        private void BtnCerrar_Click(object sender, RoutedEventArgs e)
        {
            timer.Stop();
            mediaPlayer.Stop();
            this.Close();
        }
    }

}

