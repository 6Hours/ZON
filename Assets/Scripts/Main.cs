using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class Main : MonoBehaviour
{
    [SerializeField] private InputField widthInput = default;
    [SerializeField] private InputField heightInput = default;
    [SerializeField] private Matrix First = default;
    [SerializeField] private Matrix Second = default;

    [SerializeField] private Text ResultText = default;
    [SerializeField] private GameObject ResultScreen = default;

    int width, height;

    private void Awake()
    {
        First.OnWeightChanged += value => { Second.WEIGHT = 1f - value; };
        Second.OnWeightChanged += value => { First.WEIGHT = 1f - value; };
    }

    public void CreateMatrix()
    {
        width = (int)Mathf.Clamp(float.Parse(widthInput.text), 1, float.Parse(widthInput.text));
        height = (int)Mathf.Clamp(float.Parse(heightInput.text), 1, float.Parse(heightInput.text));
        widthInput.SetTextWithoutNotify(width.ToString());
        heightInput.SetTextWithoutNotify(height.ToString());

        First.CreateMatrix(width, height);
        Second.CreateMatrix(width, height);
    }

    public void Solve()
    {
        ResultScreen.SetActive(true);
        ResultText.text += "ANSWER" + '\n';
        int[,] temp = new int[1,1];
        List<int[]> ans = Method(First.GetValues(), First.IsMax, out temp);
        ResultText.text += Matrix.ShowMatrix(temp);
        ResultText.text += "\n";
        for (int i = 0; i < ans.Count; i++)
            ResultText.text += string.Format("{0}|{1} ", ans[i][1], ans[i][0]);
        ResultText.text += "\n";
        ResultText.text +="_________"+"\n";
        /////
        ans = Method(Second.GetValues(), Second.IsMax, out temp);
        ResultText.text += Matrix.ShowMatrix(temp);
        ResultText.text += "\n";
        for (int i = 0; i < ans.Count; i++)
            ResultText.text += string.Format("{0}|{1} ", ans[i][1], ans[i][0]);
        ResultText.text += "\n";
        ResultText.text += "_________" + "\n";
        /////
        int[,] first = First.GetValues();
        int[,] second = Second.GetValues();
        for (int i = 0; i<temp.GetLength(0); i++)
            for(int j = 0; j < temp.GetLength(1); j++)
            {
                temp[i,j] = Mathf.FloorToInt(first[i, j] * First.WEIGHT * (First.IsMax ? 1 : -1) + second[i, j] * Second.WEIGHT * (Second.IsMax ? 1 : -1));
            }
        ans = Method(temp, true, out first);
        ResultText.text += Matrix.ShowMatrix(temp);
        ResultText.text += "\n";
        for (int i = 0; i < ans.Count; i++)
            ResultText.text += string.Format("{0}|{1} ", ans[i][1], ans[i][0]);
        ResultText.text += "\n";
        ResultText.text += "\n";
    }

    List<int[]> Method(int[,] Matrix, bool isMax, out int[,] exit)
    {
        if (isMax)
        {
            for (int i = 0; i < Matrix.GetLength(0); i++)
            {
                int max = Matrix[i, 0];
                for (int j = 0; j < Matrix.GetLength(1); j++) if (Matrix[i, j] > max) max = Matrix[i, j];

                for (int j = 0; j < Matrix.GetLength(1); j++)
                {
                    Matrix[i, j] -= max;
                    Matrix[i, j] *= -1;
                }
            }
        }

        List<int[]> check = new List<int[]>();
        for (int i = 0; i < Matrix.GetLength(0); i++) check.Add(new int[2]);

        int cycle = 0;
        while (cycle<100)
        {
            //редукция по строкам
            for (int i = 0; i < Matrix.GetLength(0); i++)
            {
                int min = Matrix[i, 0];
                for (int j = 0; j < Matrix.GetLength(1); j++) if (Matrix[i, j] < min) min = Matrix[i, j];

                for (int j = 0; j < Matrix.GetLength(1); j++) Matrix[i, j] -= min;
            }


            //поиск перекрестных нулей

            check.ForEach(el => el[0] = -1);

            for (int i = 0; i < check.Count; i++)
            {
                for (int j = 0; j < Matrix.GetLength(1); j++)
                {
                    if (Matrix[i, j] == 0)
                    {
                        for (int k = 0; k < check.Count; k++)
                        {
                            if (check[k][0] == j) break;
                            if (k == check.Count - 1)
                            {
                                check[i][0] = j;
                                check[i][1] = i;
                            }
                        }
                    }
                }
            }
            //подсчет
            int sum = 0;
            for (int i = 0; i < check.Count; i++)
                if (check[i][0] != -1) sum++;

            if (sum == Matrix.GetLength(0) || sum == Matrix.GetLength(1)) break;

            int[] v, h;
            v = new int[Matrix.GetLength(0)];
            h = new int[Matrix.GetLength(1)];
            for (int i = 0; i < Mathf.Max(v.Length, h.Length); i++)
            {
                if (i < v.Length) v[i] = 0;
                if (i < h.Length) h[i] = 0;
            }
            
            //пересечения
            for (int i = 0; i < v.Length; i++) 
                for (int j = 0; j < h.Length; j++)
                    if (Matrix[i, j] == 0)
                    {
                        int vsum, hsum;
                        vsum = 0; hsum = 0;
                        for (int ci = 0; ci < v.Length; ci++) if (Matrix[ci, j] == 0) vsum++;
                        for (int ci = 0; ci < h.Length; ci++) if (Matrix[i, ci] == 0) hsum++;

                        if (vsum > hsum) v[i] = 1;
                        else if(hsum > vsum) h[j] = 1;
                        else
                        {
                            if (v.Length > h.Length) h[j] = 1;
                            else v[i] = 1;
                        }
                    }

            int zeroMin = 0;
            for (int i = 0; i < Matrix.GetLength(0); i++)
                for (int j = 0; j < Matrix.GetLength(1); j++)
                    if (v[i] == 0 && h[j] == 0)
                    {
                        if (zeroMin == 0 || zeroMin > Matrix[i, j]) zeroMin = Matrix[i, j];
                    }

            for (int i = 0; i < Matrix.GetLength(0); i++)
                for (int j = 0; j < Matrix.GetLength(1); j++)
                    if (v[i] == 1 && h[j] == 1)
                    {
                        Matrix[i, j] += zeroMin;
                    }
                    else if (v[i] == 0 && h[j] == 0)
                    {
                        Matrix[i, j] -= zeroMin;
                    }

            //Проверка

            check.ForEach(el => el[0] = -1);

            for (int i = 0; i < check.Count; i++)
            {
                for (int j = 0; j < Matrix.GetLength(1); j++)
                {
                    if (Matrix[i, j] == 0)
                    {
                        for (int k = 0; k < check.Count; k++)
                        {
                            if (check[k][0] == j) break;
                            if (k == check.Count - 1)
                            {
                                check[i][0] = j;
                                check[i][1] = i;
                            }
                        }
                    }
                }
            }

            sum = 0;
            for (int i = 0; i < check.Count; i++)
                if (check[i][0] != -1) sum++;

            if (sum == Matrix.GetLength(0) || sum == Matrix.GetLength(1)) break;
            cycle++;
        }
        if(cycle==100)Debug.Log("Fail");
        exit = Matrix;
        return check.FindAll(el => el[0]!=-1);
    }
}
