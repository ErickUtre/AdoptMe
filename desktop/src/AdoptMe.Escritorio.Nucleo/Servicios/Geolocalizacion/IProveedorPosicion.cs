using AdoptMe.Escritorio.Nucleo.Comun;
using AdoptMe.Escritorio.Nucleo.Modelos;

namespace AdoptMe.Escritorio.Nucleo.Servicios.Geolocalizacion;

public interface IProveedorPosicion
{
    Task<Resultado<Coordenadas>> ObtenerPosicionAsync(CancellationToken cancelacion = default);
}
