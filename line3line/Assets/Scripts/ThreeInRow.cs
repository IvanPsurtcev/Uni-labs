using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class ThreeInRow : MiniGame
{
    //����������
    #region ��������� ����
    /// <summary> ������������ ����� ������� � ����� � ����������� ���������� </summary>
    public bool ShowOwnTimer = true;
    /// <summary> ���-�� ��������� � ���� </summary>
    public int fieldSizeW = 6;
    /// <summary> ���-�� ��������� � ������� </summary>
    public int fieldSizeH = 6;
    /// <summary> (deprecated) ���� � ����� �� ���������� ��������� </summary>
    public string cfgPath = "/StartPositions/";
    /// <summary> ���-�� ��������� ������� �����, ������� ����� ������� ��� ������ </summary>
    public int brokenElemsToWin = 9;
    /// <summary> ���-�� ����� ��� ��������� ��������� </summary>
    public int countOfRandomLines = 100;
    /// <summary> ������������ ���-�� ����� </summary>
    public int maxMoves = 12;
    /// <summary> ��������� ����� ��� ������� ����-���� </summary>
    public float baseSolutionTime = 60f;
    /// <summary> �������������� ����� ��� ���������� ������� �������� </summary>
    [Obsolete("����� �� ������������, ����� ���������� �������� ����������� �����, �������� 0.0f")]
    private const float animDelay = 0.0f;
    #endregion
    #region ������ �� ������� �������
    /// <summary> ���������� ������� - ������ </summary>
    public Element elem;
    /// <summary> ����� ��� ������ </summary>
    public TextMeshProUGUI countOfBrokenText;
    #endregion
    #region ����������� �������
    /// <summary> ��������� ������� </summary>
    Element selectElem;
    /// <summary> ������ ���� </summary>
    float fieldWidth;
    /// <summary> ������ ���� </summary>
    float fieldHeight;
    /// <summary> ������ �������� </summary>
    float elem_w;
    /// <summary> ������ �������� </summary>
    float elem_h;
    /// <summary> �������� ������ �� ���������� �������������� </summary>
    List<string> files;
    /// <summary> �������� ������ </summary>
    List<string> colorNames = new List<string>();
    /// <summary> ������������� ���� � ������ (������� �������� ������� ��������) </summary>
    List<List<Element>> grid = new List<List<Element>>();
    /// <summary> ���-�� ��� ��������� ��������� ������� ����� </summary>
    int[] countOfBrokenByColor;
    /// <summary> ���������� ���-�� ����� </summary>
    int currentMovesLeft;
    /// <summary> ������� ���������� ����� ��� ������� ����-���� </summary>
    public float timeLeft;
    /// <summary> ������� ���������� ���� </summary>
    public UnityEvent<bool> gameFinished;
    /// <summary> ����� ��� ���������� ��������, ���������� ��� ������ ��������� </summary>
    public float animationExitTime;
    /// <summary> ���� ���������� �������� </summary>
    bool coroutineFinished = true;
    #endregion

    //�������
    #region Override �������
    protected override void Initialize()
    {
        base.Initialize();
        fieldWidth = this.GetComponent<RectTransform>().rect.width;
        fieldHeight = this.GetComponent<RectTransform>().rect.height;
        elem_w = elem.GetComponent<RectTransform>().rect.width;
        elem_h = elem.GetComponent<RectTransform>().rect.height;

        if (elem_w != fieldWidth / fieldSizeW)
        {
            elem_w = fieldWidth / fieldSizeW;
            elem.GetComponent<RectTransform>().SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, elem_w);
            elem.image.GetComponent<RectTransform>().SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, elem_w);
        }
        if (elem_h != fieldHeight / fieldSizeH)
        {
            elem_h = fieldHeight / fieldSizeH;
            elem.GetComponent<RectTransform>().SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, elem_h);
            elem.image.GetComponent<RectTransform>().SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, elem_h);
        }

        for (int i = 0; i < Element.sprites.Length; i++)
        {
            colorNames.Add(Element.sprites[i].name);
        }

        countOfBrokenByColor = new int[colorNames.Count];
        currentMovesLeft = maxMoves;
        timeLeft = baseSolutionTime;
        animationExitTime = elem_w / 100;
        coroutineFinished = true;

        //ReadAnyCfg();
        RandomStartCfg();
    }
    protected override void LaunchGame()
    {
        base.LaunchGame();
        DrawField();
        countOfBrokenText.gameObject.SetActive(true);
    }
    protected override void Win()
    {
        base.Win();

        DestroyField();
        countOfBrokenText.text = "You won!";
        gameFinished.Invoke(true);
    }
    protected override void Lose()
    {
        base.Lose();

        DestroyField();
        countOfBrokenText.text = "You lost!";
        gameFinished.Invoke(false);
    }
    #endregion
    #region ������� ��������� ���������� ����
    /// <summary>
    /// ����� � ���������� ���� ������ ��������� ������������
    /// </summary>
    /// <exception cref="System.Exception">�� ������� ����� ��������� ������������</exception>
    [Obsolete("���������� ������ �������� ����")]
    private void GetCfg()
    {
        string path = Application.dataPath + cfgPath;
        //Debug.Log(path);
        if (!Directory.Exists(path))
        {
            throw new System.Exception("Start positions directory was not found");
        }
        string[] filesArray = Directory.GetFiles(path);
        files = filesArray.ToList().Where(x => !x.EndsWith("meta")).ToList();
        if (files.Count == 0)
        {
            throw new System.Exception("No start position files images were found");
        }
    }

    /// <summary>
    /// ��������� ������������ �� ���������� �����
    /// </summary>
    /// <exception cref="Exception">������ ��� ���������� ����� ������������</exception>
    [Obsolete("���������� ������ �������� ����, ����� �������� ����������� � ����� � ����������� ��������� ���������")]
    private void ReadAnyCfg()
    {
        GetCfg();
        string randPath = files[UnityEngine.Random.Range(0, files.Count)];
        try
        {
            int countOfLines = File.ReadLines(randPath).Count();
            FileStream cfg = File.Open(randPath, FileMode.Open);
            for (int i = 0; i < countOfLines; i++)
            {
                List<Element> row = new List<Element>();
                for (int j = 0; j < fieldSizeW; j++)
                {
                    int type = cfg.ReadByte() - 48; //48 - ASCII code for 0
                    if (type == -38 || type == -35) //10 - ASCII code for \n, 10-48 = -38, 13 - \r, 13-48 = -35
                    {
                        j--;
                        continue;
                    }
                    Element clone = Instantiate<Element>(elem);
                    clone.game = this;
                    clone.y = i - countOfLines + fieldSizeH; //������ �������� ��������� �� ����, �� ���� 0 - ������ ������� �����, ��� ������������� �������� �� ���� �� ����� �����
                    clone.x = j;
                    clone.SetType(type);
                    clone.transform.SetParent(this.transform, false);
                    clone.gameObject.SetActive(false);
                    row.Add(clone);
                }
                grid.Add(row);
            }
        } catch(Exception e) 
        {
            throw new Exception($"�� ������� ������� ������������ �� �����. ����� ��������� ����������: {e.Message}");
        }
        //DrawField();
    }

    /// <summary>
    /// ������������� ��������� ��������� ������������ ����
    /// </summary>
    private void RandomStartCfg()
    {
        grid = new List<List<Element>>();
        for (int i = 0; i < countOfRandomLines; i++)
        {
            List<Element> row = Enumerable.Repeat<Element>(null, fieldSizeW).ToList();
            grid.Add(row);
        }
        for (int i = 0; i < countOfRandomLines; i++)
        {
            for (int j = 0; j < fieldSizeW; j++)
            {
                Element clone = null;
                do
                {
                    if (clone != null)
                    {
                        Destroy(clone.gameObject);
                        grid[i][j] = null;
                        clone = null;
                    }
                    int type = UnityEngine.Random.Range(0, colorNames.Count);
                    clone = Instantiate<Element>(elem);
                    clone.game = this;
                    clone.y = i - countOfRandomLines + fieldSizeH; //������ �������� ��������� �� ����, �� ���� 0 - ������ ������� �����, ��� ������������� �������� �� ���� �� ����� �����
                    clone.x = j;
                    clone.SetType(type);
                    clone.transform.SetParent(this.transform, false);
                    clone.gameObject.SetActive(false);
                    grid[i][j] = clone;

                } while (CheckForThree(clone).Item1 != null || CheckForThree(clone).Item2 != null);
            }
        }
        //DrawField();
    }
    #endregion
    #region �������� ������� ����

    /// <summary>
    /// ��������, ���� �� � ������� �������� ���-�� ���� �� ��� � ����
    /// </summary>
    /// <param name="e">������� ��� ��������</param>
    /// <returns>������ ������� � ������ ����, ������ ��� �����������, ������ ��� ���������</returns>
    private Tuple<Element, Element> CheckForThree(Element e)
    {
        Element resHor = null;
        Element resVert = null;
        for (int x = e.x - 2; x <= e.x; x++)
        {
            if (x >= 0 && x + 2 < fieldSizeW)
            {
                int posY = GetFieldVertOffset(e.y);
                if (grid[posY][x] != null && grid[posY][x + 1] != null && grid[posY][x + 2] != null)
                {
                    if (grid[posY][x].type == grid[posY][x + 1].type && grid[posY][x + 1].type == grid[posY][x + 2].type)
                    {
                        resHor = grid[posY][x];
                        break;
                    }
                }
            }
        }
        for (int y = GetFieldVertOffset(e.y) - 2; y <= GetFieldVertOffset(e.y); y++)
        {
            if (y >= grid.Count - fieldSizeH && y + 2 < grid.Count)
            {
                if (grid[y][e.x] != null && grid[y + 1][e.x] != null && grid[y + 2][e.x] != null)
                {
                    if (grid[y][e.x].type == grid[y + 1][e.x].type && grid[y + 1][e.x].type == grid[y + 2][e.x].type)
                    {
                        resVert = grid[y][e.x];
                        break;
                    }
                }
            }
        }
        return new Tuple<Element, Element>(resHor, resVert);
    }

    /// <summary>
    /// ���������, �� ������� �� �� ��� ���������� ��������� ��������� ��� ������ � �����������, ���� ��
    /// </summary>
    private void CheckForWin()
    {
        bool res = true;
        for (int i = 0; i < countOfBrokenByColor.Length; i++)
        {
            if (countOfBrokenByColor[i] < brokenElemsToWin)
            {
                res = false;
            }
        }
        if (res)
        {
            Win();
        }
        else if (currentMovesLeft <= 0)
        {
            Lose();
        }
    }

    /// <summary>
    /// ������� ��� �������� � ���� � ������ ��� � null
    /// </summary>
    private void DestroyField()
    {
        for (int i = 0; i < grid.Count; i++)
        {
            for (int j = 0; j < fieldSizeW; j++)
            {
                if (grid[i][j] != null)
                {
                    Destroy(grid[i][j].gameObject);
                }
            }
        }
        grid = null;
    }

    /// <summary>
    /// ���������� ������ � ������������ � ������� ������
    /// </summary>
    private void UpdateBrokenElemsText()
    {
        countOfBrokenText.text = "";
        for (int i = 0; i < colorNames.Count; i++)
        {
            countOfBrokenText.text += $"{colorNames[i]} left: " + (brokenElemsToWin - countOfBrokenByColor[i] > 0 ? brokenElemsToWin - countOfBrokenByColor[i] : 0) + "\n";
        }
        countOfBrokenText.text += "Moves left: " + currentMovesLeft + "\n";
        if (ShowOwnTimer)
        {
            countOfBrokenText.text += "Time left: " + Math.Round(timeLeft, 2);
        }
    }

    /// <summary>
    /// ����� SwapAndBreak ��� ��������
    /// </summary>
    /// <param name="e">�������, ��� �������� ����� ���������� swap</param>
    public void StartSwapCoroutine(Element e)
    {
        if (coroutineFinished)
        {
            StartCoroutine(SwapAndBreak(e));
        }
    }

    /// <summary>
    /// �������� �������, ������� ������ ����� ����� ��� �������� � ������ ��������, ���� ����������
    /// </summary>
    /// <param name="e">�������, ������� �������</param>
    /// <returns></returns>
    public IEnumerator SwapAndBreak(Element e)
    {
        //���� �� ����� �������� �������� �� ���� �������, �� ������ ������� ������� ���������
        if (selectElem == null)
        {
            selectElem = e;
            yield break;
        }

        if (Math.Abs(e.x - selectElem.x) + Math.Abs(e.y - selectElem.y) == 1) //���� ������� ������� �������� � ��������� �� �����
        {
            coroutineFinished = false;
            ShowAnimSwap(e, selectElem);
            yield return new WaitForSeconds(animationExitTime);
            MakeTypeSwap(selectElem, e);//������ �� �������
            Tuple<Element, Element> res1 = CheckForThree(e);//���������, �� ���������� �� ������ � �������� �������� ����� ������
            e.animator.SetBool("breakSituation", true);
            selectElem.animator.SetBool("breakSituation", true);
            if (res1.Item1 != null)
            {
                MakeImageSwap(selectElem, e);
                StartCoroutine(BreakElems(res1.Item1, true));
            }
            if (res1.Item2 != null)
            {
                MakeImageSwap(selectElem, e);
                StartCoroutine(BreakElems(res1.Item2, false));
            }
            Tuple<Element, Element> res2 = CheckForThree(selectElem);//���������, �� ���������� �� ������ � ���������� �� ����� �������� ����� ������
            if (res2.Item1 != null)
            {
                MakeImageSwap(selectElem, e);
                StartCoroutine(BreakElems(res2.Item1, true));
            }
            if (res2.Item2 != null)
            {
                MakeImageSwap(selectElem, e);
                StartCoroutine(BreakElems(res2.Item2, false));
            }
            if (res1.Item1 == null && res1.Item2 == null && res2.Item1 == null && res2.Item2 == null)//���� ������� ������ �� �����, �� ������ �������
            {
                e.animator.SetBool("breakSituation", false);
                selectElem.animator.SetBool("breakSituation", false);
                ShowAnimSwap(e, selectElem);
                yield return new WaitForSeconds(animationExitTime);
                MakeTypeSwap(selectElem, e);
                selectElem = null;
                coroutineFinished = true;
                yield break;
            }
            yield return new WaitForSeconds(animationExitTime);//��������� �������� ����� ����������� ����������, ����� ���� ����� � ���� �����, ��� ��������
            bool needGoMore = false;
            do
            {
                needGoMore = false;
                for (int i = grid.Count - fieldSizeH; i < grid.Count; i++) //���������� �� �������� ���� (����� ��������� fieldSize ���������)
                {
                    for (int j = 0; j < fieldSizeW; j++)
                    {
                        if (grid[i][j] == null)
                        {
                            continue;
                        }
                        res1 = CheckForThree(grid[i][j]); //���������, �� �������� �� ����� ���-�� ���
                        if (res1.Item1 != null)
                        {
                            yield return StartCoroutine(BreakElems(res1.Item1, true));
                            needGoMore = true;
                        }
                        if (res1.Item2 != null)
                        {
                            yield return StartCoroutine(BreakElems(res1.Item2, false));
                            needGoMore = true;
                        }
                        if (res1.Item1 != null || res1.Item2 != null)
                        {
                            //Debug.Log(res1.Item1 != null ? res1.Item1.x + " " + res1.Item1.y : res1.Item2.x + " " + res1.Item2.y);
                            yield return new WaitForSeconds(animationExitTime);
                        }
                    }
                }

            }
            while (needGoMore == true); //���� ������� ������� �� ���������
            currentMovesLeft--;
            UpdateBrokenElemsText();
        }

        selectElem = null;

        CheckForWin();
        coroutineFinished = true;
    }

    /// <summary>
    /// ������� ����� ������ (��� ������, ���� �������)
    /// </summary>
    /// <param name="firstElem">������ �������, �� �������� ���� ������ ������ ��� ������</param>
    /// <param name="horizontal">�� �����������, ���� true, ����� false</param>
    /// <returns></returns>
    private IEnumerator BreakElems(Element firstElem, bool horizontal)
    {
        int x = firstElem.x;
        int y = GetFieldVertOffset(firstElem.y);
        if (horizontal)
        {
            for (int i = x; i < fieldSizeW && grid[y][i] != null && grid[y][i].type == firstElem.type; i++) //������, ���� �� ����������� ���� �������� ������ ����
            {
                if (grid[y][i].gameObject.activeInHierarchy)
                {
                    grid[y][i].animator.Play("Remove", 0);
                }
                grid[y][i] = null;
                FallColumn(i, y);
                countOfBrokenByColor[firstElem.type]++;
            }
        }
        else
        {
            int maxDown = 1;
            while (y + maxDown + 1 < grid.Count && grid[y + maxDown + 1][x].type == firstElem.type) //���������, ��� ������� ���� ���� ���� ��������
            {
                maxDown++;
            }
            while (grid[y + maxDown][x] != null && grid[y + maxDown][x].type == firstElem.type)
            {
                if (grid[y + maxDown][x].gameObject.activeInHierarchy)
                {
                    grid[y + maxDown][x].animator.Play("Remove", 0);
                }
                grid[y + maxDown][x] = null;
                FallColumn(x, y + maxDown);
                countOfBrokenByColor[firstElem.type]++;
            }
        }
        yield return new WaitForSeconds(animationExitTime);
        DrawField();
        yield break;
    }

    /// <summary>
    /// ������� ��������� ������� ����� ����
    /// </summary>
    private void DrawField()
    {
        for (int i = grid.Count - fieldSizeH; i < grid.Count; i++)
        {
            for (int j = 0; j < fieldSizeW; j++)
            {
                Element clone = grid[i][j];
                if (clone == null)
                {
                    continue;
                }
                clone.gameObject.SetActive(true);
                clone.SetType(clone.type);
                clone.GetComponent<RectTransform>().anchoredPosition =
                    new Vector2(j * elem_w + elem_w / 2, -((i - grid.Count + fieldSizeH) * elem_h + elem_h / 2));
                clone.SetAnimatorSize(animationExitTime);
            }
        }
        UpdateBrokenElemsText();
    }
    /// <summary>
    /// �������� ��������� ��� ������� ������������� �� ���� ������������ ������� �������
    /// </summary>
    /// <param name="y">��������� �� ������� ����</param>
    /// <returns></returns>
    private int GetFieldVertOffset(int y)
    {
        return y + grid.Count - fieldSizeH;
    }

    /// <summary>
    /// ������� �������� ���� ��� �������� ���� �����������
    /// </summary>
    /// <param name="x">x �������� � grid</param>
    /// <param name="y">y �������� � grid (��. GetFieldVertOffset)</param>
    private void FallColumn(int x, int y)
    {
        for (int i = y; i > 0; i--)
        {
            Element temp = grid[i][x];
            grid[i][x] = grid[i - 1][x];
            grid[i - 1][x] = temp;
            if (grid[i][x] == null)
            {
                break;
            }
            grid[i][x].y++;
            if (i >= grid.Count - fieldSizeH)
            {
                //grid[i][x].gameObject.SetActive(true);
            }
        }
    }

    /// <summary>
    /// ������ �������� ��� ���������
    /// </summary>
    /// <param name="e1">������� 1</param>
    /// <param name="e2">������� 2</param>
    private void ShowAnimSwap(Element e1, Element e2)
    {
        int state = e1.x - e2.x == 0 ? e1.y - e2.y + 1 : e1.x - e2.x;
        switch (state)
        {
            case -1:
                e1.animator.Play("SwapRight", 0);
                e2.animator.Play("SwapLeft", 0);
                break;
            case 0:
                e1.animator.Play("SwapDown", 0);
                e2.animator.Play("SwapUp", 0);
                break;
            case 1:
                e1.animator.Play("SwapLeft", 0);
                e2.animator.Play("SwapRight", 0);
                break;
            case 2:
                e1.animator.Play("SwapUp", 0);
                e2.animator.Play("SwapDown", 0);
                break;
            default:
                break;
        }
    }

    /// <summary>
    /// ������� ���������� ������ ����� ����� (������ ���������) ��� ��������
    /// </summary>
    /// <param name="e1"></param>
    /// <param name="e2"></param>
    private void MakeTypeSwap(Element e1, Element e2)
    {
        int temp = e1.type;
        e1.type = e2.type;
        e2.type = temp;
        //e1.SetType(e2.type);
        //e2.SetType(temp);
    }
    
    /// <summary>
    /// ������� ������ ������� ������� � ��� ���������� �� ����������� ���� ���������
    /// </summary>
    /// <param name="e1"></param>
    /// <param name="e2"></param>
    private void MakeImageSwap(Element e1, Element e2)
    {
        e1.SetType(e1.type);
        e2.SetType(e2.type);
    }

    /// <summary>
    /// ����� ��� �������
    /// </summary>
    private void Update()
    {
        if (timeLeft > 0)
        {
            if (countOfBrokenText.gameObject.activeInHierarchy)
            {
                timeLeft -= Time.deltaTime;
            }
            if (grid != null)
            {
                UpdateBrokenElemsText();
            }
        }
        else
        {
            timeLeft = 0;
            if (grid != null && countOfBrokenByColor.Any(x => x > 0))
            {
                Lose();
            }
        }
    }
    #endregion
}
