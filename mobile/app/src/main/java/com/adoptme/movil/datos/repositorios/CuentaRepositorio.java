package com.adoptme.movil.datos.repositorios;

import com.adoptme.movil.comun.AlTerminar;
import com.adoptme.movil.dominio.Modelos.Perfil;
import com.adoptme.movil.dominio.Modelos.Sesion;
import com.adoptme.movil.dominio.Ubicacion;

public interface CuentaRepositorio {

    void iniciarSesion(String correo, String contrasena, AlTerminar<Sesion> alTerminar);

    void registrar(String nombre, String telefono, String correo, String contrasena, Ubicacion ubicacion, AlTerminar<Void> alTerminar);

    void actualizarNombre(String nombre, AlTerminar<Perfil> alTerminar);

    void actualizarTelefono(String telefono, AlTerminar<Perfil> alTerminar);

    void actualizarCorreo(String correo, AlTerminar<Void> alTerminar);

    void actualizarUbicacion(Ubicacion ubicacion, AlTerminar<Ubicacion> alTerminar);
}
