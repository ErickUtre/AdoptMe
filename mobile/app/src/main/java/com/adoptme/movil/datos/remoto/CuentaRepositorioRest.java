package com.adoptme.movil.datos.remoto;

import com.adoptme.movil.comun.AlTerminar;
import com.adoptme.movil.datos.remoto.api.ApisRest.AccesoApi;
import com.adoptme.movil.datos.remoto.api.ApisRest.Credenciales;
import com.adoptme.movil.datos.remoto.api.ApisRest.RegistroUsuario;
import com.adoptme.movil.datos.remoto.api.ApisRest.UsuarioApi;
import com.adoptme.movil.datos.repositorios.CuentaRepositorio;
import com.adoptme.movil.dominio.Modelos.Perfil;
import com.adoptme.movil.dominio.Modelos.Sesion;
import com.adoptme.movil.dominio.Ubicacion;

import java.util.Collections;

public final class CuentaRepositorioRest implements CuentaRepositorio {

    private final AccesoApi acceso;
    private final UsuarioApi usuarios;

    public CuentaRepositorioRest(ClienteRest cliente) {
        acceso = cliente.crear(AccesoApi.class);
        usuarios = cliente.crear(UsuarioApi.class);
    }

    @Override
    public void iniciarSesion(String correo, String contrasena, AlTerminar<Sesion> alTerminar) {
        LlamadasApi.ejecutar(acceso.iniciarSesion(new Credenciales(correo, contrasena)), alTerminar);
    }

    @Override
    public void registrar(String nombre, String telefono, String correo, String contrasena, Ubicacion ubicacion, AlTerminar<Void> alTerminar) {
        RegistroUsuario registro = new RegistroUsuario(nombre, telefono, ubicacion, new Credenciales(correo, contrasena));
        LlamadasApi.ejecutar(usuarios.registrar(registro), alTerminar);
    }

    @Override
    public void actualizarNombre(String nombre, AlTerminar<Perfil> alTerminar) {
        LlamadasApi.ejecutar(usuarios.actualizarPerfil(Collections.singletonMap("Nombre", nombre)), alTerminar);
    }

    @Override
    public void actualizarTelefono(String telefono, AlTerminar<Perfil> alTerminar) {
        LlamadasApi.ejecutar(usuarios.actualizarPerfil(Collections.singletonMap("Telefono", telefono)), alTerminar);
    }

    @Override
    public void actualizarCorreo(String correo, AlTerminar<Void> alTerminar) {
        LlamadasApi.ejecutar(acceso.actualizar(Collections.singletonMap("Correo", correo)), alTerminar);
    }

    @Override
    public void actualizarUbicacion(Ubicacion ubicacion, AlTerminar<Ubicacion> alTerminar) {
        LlamadasApi.ejecutar(usuarios.actualizarUbicacion(ubicacion), alTerminar);
    }
}
