using UnityEngine;
using System.Collections.Generic;

public enum ObjectCategory { Juguete, Naturaleza, Infraestructura, Indefinido }

[CreateAssetMenu(fileName = "New ObjectData", menuName = "CogniSim/Object Data")]
public class ObjectData : ScriptableObject
{
    [Tooltip("El nombre que se mostrará al jugador. Ej: 'Pelota de Fútbol'")]
    public string displayName = "Nuevo Objeto";

    [Tooltip("La categoría principal a la que pertenece el objeto.")]
    public ObjectCategory category = ObjectCategory.Indefinido;

    [Tooltip("Etiquetas descriptivas para desafíos más complejos. Ej: 'Deporte', 'Redondo', 'Para patear'")]
    public List<string> tags;
}
