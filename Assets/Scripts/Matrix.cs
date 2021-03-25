using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[System.Serializable]
public class Matrix : MonoBehaviour
{
    [SerializeField] private InputField[,] cells = new InputField[0, 0];
    [SerializeField] private InputField minInput = default;
    [SerializeField] private InputField maxInput = default;
    [SerializeField] private InputField weightInput = default;
    [SerializeField] private Dropdown target = default;

    [SerializeField] private RectTransform root = default;

    [SerializeField] GameObject prefab = default;

    public System.Action<float> OnWeightChanged = delegate { };

    int min, max;

    private void Start()
    {
        weightInput.onValueChanged.AddListener(value =>
        {
            float t= 0f;
            if(float.TryParse(value, out t))
            WeightChanged(value);
        });
    }

    public void CreateMatrix(int w, int h)
    {
        foreach (var l in cells) Destroy(l.gameObject);

        cells = new InputField[h, w];

        for(int i = 0; i < cells.GetLength(0); i++)
            for(int j = 0; j < cells.GetLength(1); j++)
            {
                cells[i, j] = Instantiate(prefab, root).GetComponent<InputField>();
                cells[i, j].placeholder.GetComponent<Text>().text = string.Format("{0}|{1}", i+1, j+1);
                cells[i, j].GetComponent<RectTransform>().anchoredPosition = new Vector2(41f + 82f * j, -41f - 82f * i);
            }
    }

    public void RandomFill()
    {
        min = int.Parse(minInput.text);
        max = int.Parse(maxInput.text);

        for (int i = 0; i < cells.GetLength(0); i++)
        {
            for (int j = 0; j < cells.GetLength(1); j++)
            {
                cells[i, j].text = Random.Range(min, max).ToString();
            }
        }
    }

    public int[,] GetValues()
    {
        int[,] temp = new int[cells.GetLength(0), cells.GetLength(1)];
        for (int i = 0; i < cells.GetLength(0); i++)
        {
            for (int j = 0; j < cells.GetLength(1); j++)
            {
                temp[i, j] = int.Parse(cells[i, j].text);
            }
        }
        return temp;
    }

    public float WEIGHT
    {
        get
        {
            return Mathf.Clamp(float.Parse(weightInput.text), 0f, 1f);
        }
        set
        {
            weightInput.SetTextWithoutNotify(value.ToString());
        }
    }

    public bool IsMax
    {
        get
        {
            return target.value == 1;
        }
    }


    void WeightChanged(string value)
    {
        float val = Mathf.Clamp(float.Parse(value), 0f, 1f);
        OnWeightChanged?.Invoke(val);
    }

    public static string ShowMatrix(int [,] matr)
    {
        string result = "";
        for (int i = 0; i < matr.GetLength(0); i++)
        {
            for (int j = 0; j < matr.GetLength(1); j++)
                result += matr[i,j] + " ";
            result += '\n';
        }
        return result;
    }
}
