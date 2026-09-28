using System.Collections.ObjectModel;
using AdoptMe.Escritorio.Nucleo.Modelos;
using AdoptMe.Escritorio.Nucleo.Presentacion;
using AdoptMe.Escritorio.Nucleo.Servicios.Api;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace AdoptMe.Escritorio.Nucleo.VistasModelo.Chat;

public sealed partial class ConversacionesVistaModelo(IServicioChat chat, INavegador navegador, IDialogos dialogos)
    : VistaModeloBase, IAlNavegar
{
    [ObservableProperty]
    private bool sinConversaciones;

    public ObservableCollection<Conversacion> Conversaciones { get; } = [];

    public async Task AlNavegarAsync(object? parametro)
    {
        var resultado = await MientrasOcupadoAsync(chat.ListarConversacionesAsync).ConfigureAwait(true);
        if (!Informar(dialogos, resultado, out var lista))
        {
            return;
        }
        Conversaciones.Clear();
        foreach (var conversacion in lista)
        {
            Conversaciones.Add(conversacion);
        }
        SinConversaciones = Conversaciones.Count == 0;
    }

    [RelayCommand]
    private Task AbrirAsync(Conversacion conversacion) => navegador.NavegarAAsync<ChatVistaModelo>(conversacion.UsuarioID);
}
