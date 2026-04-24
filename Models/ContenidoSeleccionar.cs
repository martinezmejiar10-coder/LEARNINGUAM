using System.Collections.Generic;

public class ContenidoSeleccionar
{
    public string texto { get; set; }
    public List<EspacioSeleccion> espacios { get; set; }
}

public class EspacioSeleccion
{
    public int id { get; set; }
    public List<OpcionSeleccion> opciones { get; set; }
}

public class OpcionSeleccion
{
    public string texto { get; set; }
    public bool correcta { get; set; }
}