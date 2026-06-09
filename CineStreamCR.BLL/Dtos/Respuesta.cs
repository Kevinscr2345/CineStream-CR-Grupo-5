namespace CineStreamCR.BLL.Dtos;

public sealed class Respuesta<T>
{
    public bool EsCorrecto { get; set; } = true;
    public string Mensaje { get; set; } = "Operación realizada correctamente.";
    public int Codigo { get; set; } = 200;
    public T? Dato { get; set; }

    public static Respuesta<T> Correcta(T? dato, string mensaje = "Operación realizada correctamente.", int codigo = 200) =>
        new() { Dato = dato, Mensaje = mensaje, Codigo = codigo };

    public static Respuesta<T> Error(string mensaje, int codigo = 400) =>
        new() { EsCorrecto = false, Mensaje = mensaje, Codigo = codigo };
}

