public class CuestionarioBuilderViewModel
{
    public int IdCuestionario { get; set; }

    public int IdCurso { get; set; }

    // Pregunta que se está escribiendo
    public string TextoPregunta { get; set; }
    public string Tipo { get; set; }
    public List<OpcionViewModel> Opciones { get; set; } = new();

    // Lista de preguntas agregadas (en memoria)
    public List<PreguntaViewModel> Preguntas { get; set; } = new();
}