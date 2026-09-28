using System.Diagnostics.CodeAnalysis;
using AdoptMe.Escritorio.Nucleo.Comun;
using AdoptMe.Escritorio.Nucleo.Presentacion;
using CommunityToolkit.Mvvm.ComponentModel;

namespace AdoptMe.Escritorio.Nucleo.VistasModelo;

public abstract partial class VistaModeloBase : ObservableObject
{
    [ObservableProperty]
    private bool estaOcupado;

    protected async Task<T> MientrasOcupadoAsync<T>(Func<Task<T>> operacion)
    {
        ArgumentNullException.ThrowIfNull(operacion);
        EstaOcupado = true;
        try
        {
            return await operacion().ConfigureAwait(true);
        }
        finally
        {
            EstaOcupado = false;
        }
    }

    protected static bool Informar(IDialogos dialogos, Resultado resultado, string? mensajeExito = null)
    {
        ArgumentNullException.ThrowIfNull(dialogos);
        ArgumentNullException.ThrowIfNull(resultado);
        if (!resultado.Exito)
        {
            dialogos.Mostrar(resultado.Error.Mensaje, TipoMensaje.Error, Textos.Error);
            return false;
        }
        if (mensajeExito is not null)
        {
            dialogos.Mostrar(mensajeExito, TipoMensaje.Informacion, Textos.Exito);
        }
        return true;
    }

    protected static bool Informar<T>(IDialogos dialogos, Resultado<T> resultado, [NotNullWhen(true)] out T? valor, string? mensajeExito = null)
    {
        ArgumentNullException.ThrowIfNull(resultado);
        valor = resultado.Valor;
        return Informar(dialogos, (Resultado)resultado, mensajeExito) && valor is not null;
    }
}

public abstract partial class VistaModeloDialogo<TResultado> : VistaModeloBase
{
    public event EventHandler<bool>? CierreSolicitado;

    public TResultado? Resultado { get; private set; }

    public virtual void Inicializar(object? parametro)
    {
    }

    protected void Aceptar(TResultado resultado)
    {
        Resultado = resultado;
        CierreSolicitado?.Invoke(this, true);
    }

    protected void Cancelar() => CierreSolicitado?.Invoke(this, false);
}
