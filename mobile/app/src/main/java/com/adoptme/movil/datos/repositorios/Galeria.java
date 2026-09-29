package com.adoptme.movil.datos.repositorios;

import android.graphics.Bitmap;

import com.adoptme.movil.comun.AlTerminar;

public interface Galeria {

    void guardarPng(Bitmap imagen, String nombre, AlTerminar<Void> alTerminar);
}
