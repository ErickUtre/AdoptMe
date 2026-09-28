using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;

namespace AdoptMe.Escritorio.Controles;

public static class TextoSugerido
{
    public static readonly DependencyProperty TextoProperty = DependencyProperty.RegisterAttached(
        "Texto", typeof(string), typeof(TextoSugerido), new FrameworkPropertyMetadata(string.Empty));

    public static string GetTexto(DependencyObject elemento)
    {
        ArgumentNullException.ThrowIfNull(elemento);
        return (string)elemento.GetValue(TextoProperty);
    }

    public static void SetTexto(DependencyObject elemento, string valor)
    {
        ArgumentNullException.ThrowIfNull(elemento);
        elemento.SetValue(TextoProperty, valor);
    }
}

public static class ContrasenaEnlazable
{
    public static readonly DependencyProperty ContrasenaProperty = DependencyProperty.RegisterAttached(
        "Contrasena",
        typeof(string),
        typeof(ContrasenaEnlazable),
        new FrameworkPropertyMetadata(string.Empty, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault, AlCambiarContrasena));

    public static readonly DependencyProperty EstaVaciaProperty = DependencyProperty.RegisterAttached(
        "EstaVacia", typeof(bool), typeof(ContrasenaEnlazable), new FrameworkPropertyMetadata(true));

    static ContrasenaEnlazable()
    {
        EventManager.RegisterClassHandler(typeof(PasswordBox), PasswordBox.PasswordChangedEvent, new RoutedEventHandler(AlEscribir));
    }

    public static string GetContrasena(DependencyObject elemento)
    {
        ArgumentNullException.ThrowIfNull(elemento);
        return (string)elemento.GetValue(ContrasenaProperty);
    }

    public static void SetContrasena(DependencyObject elemento, string valor)
    {
        ArgumentNullException.ThrowIfNull(elemento);
        elemento.SetValue(ContrasenaProperty, valor);
    }

    public static bool GetEstaVacia(DependencyObject elemento)
    {
        ArgumentNullException.ThrowIfNull(elemento);
        return (bool)elemento.GetValue(EstaVaciaProperty);
    }

    public static void SetEstaVacia(DependencyObject elemento, bool valor)
    {
        ArgumentNullException.ThrowIfNull(elemento);
        elemento.SetValue(EstaVaciaProperty, valor);
    }

    private static void AlCambiarContrasena(DependencyObject elemento, DependencyPropertyChangedEventArgs argumentos)
    {
        if (elemento is PasswordBox caja && argumentos.NewValue is string nueva && caja.Password != nueva)
        {
            caja.Password = nueva;
        }
    }

    private static void AlEscribir(object remitente, RoutedEventArgs argumentos)
    {
        var caja = (PasswordBox)remitente;
        SetEstaVacia(caja, caja.Password.Length == 0);
        if (BindingOperations.IsDataBound(caja, ContrasenaProperty) && GetContrasena(caja) != caja.Password)
        {
            SetContrasena(caja, caja.Password);
        }
    }
}
