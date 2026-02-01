using UnityEngine;
using UnityEngine.UI;

public class ColorButtonsSpawner : MonoBehaviour
{
    public Button buttonPrefab;
    public Transform buttonParent;
    public TargetColorPalette colorPalette;
    public GameManager gameManager;

    void Start()
    {
        if (buttonParent == null)
        {
            buttonParent = transform;
        }

        if (buttonPrefab == null)
        {
            Debug.LogWarning("[ColorButtonsSpawner] Missing buttonPrefab.");
            return;
        }

        if (colorPalette == null || colorPalette.colors == null || colorPalette.colors.Count == 0)
        {
            Debug.LogWarning("[ColorButtonsSpawner] Missing color palette or colors.");
            return;
        }

        ClearChildren();

        if (gameManager == null)
        {
            Debug.LogWarning("[ColorButtonsSpawner] Missing gameManager reference.");
        }

        for (int i = 0; i < colorPalette.colors.Count; i++)
        {
            Button instance = Instantiate(buttonPrefab, buttonParent);
            Color buttonColor = colorPalette.colors[i];
            SetButtonColor(instance, buttonColor);

            if (gameManager != null)
            {
                instance.onClick.AddListener(() => gameManager.RegisterColorInput(buttonColor));
            }
        }
    }

    void SetButtonColor(Button button, Color color)
    {
        if (button == null)
        {
            return;
        }

        Graphic graphic = button.targetGraphic;
        if (graphic == null)
        {
            graphic = button.GetComponent<Graphic>();
        }

        if (graphic == null)
        {
            Debug.LogWarning("[ColorButtonsSpawner] Button has no Graphic to color.");
            return;
        }

        graphic.color = color;
    }

    void ClearChildren()
    {
        for (int i = buttonParent.childCount - 1; i >= 0; i--)
        {
            Destroy(buttonParent.GetChild(i).gameObject);
        }
    }
}
