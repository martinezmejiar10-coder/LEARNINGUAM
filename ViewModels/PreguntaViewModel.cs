//using System.Collections.Generic;
using LEARNINGUAM.Web.Models;

public class PreguntaViewModel
{
    //public int IdCuestionario { get; set; }

    public string TextoPregunta { get; set; }

    public string Tipo { get; set; }

    public List<OpcionViewModel> Opciones { get; set; }

    public List<Pregunta> PreguntasExistentes { get; set; }
}