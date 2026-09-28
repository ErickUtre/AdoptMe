using AdoptMe.Escritorio.Nucleo.VistasModelo;
using CommunityToolkit.Mvvm.ComponentModel;
using Microsoft.Extensions.DependencyInjection;

namespace AdoptMe.Escritorio.Nucleo.Presentacion;

public interface INavegador
{
    VistaModeloBase? Actual { get; }

    Task NavegarAAsync<TVistaModelo>(object? parametro = null)
        where TVistaModelo : VistaModeloBase;

    void Reiniciar();
}

public sealed partial class Navegador(IServiceProvider proveedor) : ObservableObject, INavegador
{
    [ObservableProperty]
    private VistaModeloBase? actual;

    public async Task NavegarAAsync<TVistaModelo>(object? parametro = null)
        where TVistaModelo : VistaModeloBase
    {
        if (parametro is null && Actual is TVistaModelo)
        {
            return;
        }
        (Actual as IAlSalir)?.AlSalir();
        var destino = proveedor.GetRequiredService<TVistaModelo>();
        Actual = destino;
        if (destino is IAlNavegar alNavegar)
        {
            await alNavegar.AlNavegarAsync(parametro).ConfigureAwait(true);
        }
    }

    public void Reiniciar()
    {
        (Actual as IAlSalir)?.AlSalir();
        Actual = null;
    }
}
