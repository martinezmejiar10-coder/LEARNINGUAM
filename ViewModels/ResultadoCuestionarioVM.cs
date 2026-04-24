public class ResultadoCuestionarioVM
{
    public int Correctas { get; set; }
    public int Total { get; set; }
    public int Porcentaje { get; set; }

    public int IdCurso { get; set; }

    public List<PreguntaResultadoVM> Preguntas { get; set; }
}

public class PreguntaResultadoVM
{
    public string TextoPregunta { get; set; }

    public List<OpcionResultadoVM> Opciones { get; set; }

    public List<int> OpcionesSeleccionadas { get; set; }
}

public class OpcionResultadoVM
{
    public int IdOpcion { get; set; }
    public string Texto { get; set; }
    public bool EsCorrecta { get; set; }
}