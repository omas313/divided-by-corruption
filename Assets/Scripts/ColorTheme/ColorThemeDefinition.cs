using UnityEngine;

[CreateAssetMenu(fileName = "ColorThemeDefinition.asset", menuName = "Theme/Color Theme Definition")]
public class ColorThemeDefinition : ScriptableObject
{
    public Color Primary => _primary;
    public Color PrimaryLighter => _primaryLighter; 
    public Color Text => _text;
    public Color Contrast => _contrast;

    [SerializeField] Color _primary;
    [SerializeField] Color _primaryLighter; 
    [SerializeField] Color _text;
    [SerializeField] Color _contrast;
}
