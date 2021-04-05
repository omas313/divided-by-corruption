using System.Collections.Generic;
using UnityEngine;

public class Theme : MonoBehaviour
{
    public static Theme Instance { get; private set; }

    public Color PrimaryLighterColor => _colorThemeDefinition.PrimaryLighter;
    public Color PrimaryColor => _colorThemeDefinition.Primary;
    public Color TextColor => _colorThemeDefinition.Text;
    public Color ContrastColor => _colorThemeDefinition.Contrast;


    // todo: set the color theme from the "area definition" we are in
    [SerializeField] ColorThemeDefinition _colorThemeDefinition;
    public void SetColorTheme(ColorThemeDefinition colorThemeDefinition) => _colorThemeDefinition = colorThemeDefinition;

    void Awake()
    {
        if (Instance == null)
            Instance = this;
        else 
            Destroy(gameObject);
        
        DontDestroyOnLoad(gameObject);

        // todo: set the color theme from the "area definition" we are in
        SetColorTheme(_colorThemeDefinition);
    }
}
