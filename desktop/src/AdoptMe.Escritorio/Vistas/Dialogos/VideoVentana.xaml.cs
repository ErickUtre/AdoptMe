using System.Windows;
using System.Windows.Controls.Primitives;
using System.Windows.Threading;

namespace AdoptMe.Escritorio.Vistas.Dialogos;

public partial class VideoVentana : Window
{
    private readonly DispatcherTimer temporizador = new() { Interval = TimeSpan.FromMilliseconds(500) };
    private bool arrastrando;

    public VideoVentana(Uri video)
    {
        InitializeComponent();
        temporizador.Tick += (_, _) => ActualizarProgreso();
        Reproductor.Source = video;
        Loaded += (_, _) => Reproductor.Play();
        Closed += (_, _) =>
        {
            temporizador.Stop();
            Reproductor.Close();
        };
    }

    private void AlAbrirMedio(object remitente, RoutedEventArgs argumentos)
    {
        if (Reproductor.NaturalDuration.HasTimeSpan)
        {
            Progreso.Maximum = Reproductor.NaturalDuration.TimeSpan.TotalSeconds;
            temporizador.Start();
        }
    }

    private void ActualizarProgreso()
    {
        if (!arrastrando)
        {
            Progreso.Value = Reproductor.Position.TotalSeconds;
        }
    }

    private void AlIniciarArrastre(object remitente, DragStartedEventArgs argumentos) => arrastrando = true;

    private void AlTerminarArrastre(object remitente, DragCompletedEventArgs argumentos)
    {
        Reproductor.Position = TimeSpan.FromSeconds(Progreso.Value);
        arrastrando = false;
    }

    private void Reproducir(object remitente, RoutedEventArgs argumentos) => Reproductor.Play();

    private void Pausar(object remitente, RoutedEventArgs argumentos) => Reproductor.Pause();

    private void Detener(object remitente, RoutedEventArgs argumentos)
    {
        Reproductor.Stop();
        Progreso.Value = 0;
    }
}
