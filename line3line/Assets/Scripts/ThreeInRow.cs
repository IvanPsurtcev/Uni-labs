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
    //Переменные
    #region Настройка игры
    /// <summary> Использовать вывод таймера в текст с оставшимися элементами </summary>
    public bool ShowOwnTimer = true;
    /// <summary> Кол-во элементов в ряду </summary>
    public int fieldSizeW = 6;
    /// <summary> Кол-во элементов в столбце </summary>
    public int fieldSizeH = 6;
    /// <summary> (deprecated) Путь к папке со стартовыми позициями </summary>
    public string cfgPath = "/StartPositions/";
    /// <summary> Кол-во элементов каждого цвета, которое нужно сломать для победы </summary>
    public int brokenElemsToWin = 9;
    /// <summary> Кол-во линий для случайной генерации </summary>
    public int countOfRandomLines = 100;
    /// <summary> Максимальное кол-во ходов </summary>
    public int maxMoves = 12;
    /// <summary> Начальное время для решения мини-игры </summary>
    public float baseSolutionTime = 60f;
    /// <summary> Дополнительное время для подстройки времени анимации </summary>
    [Obsolete("Более не используется, время выполнения анимации вычисляется точно, оставить 0.0f")]
    private const float animDelay = 0.0f;
    #endregion
    #region Ссылки на внешние объекты
    /// <summary> Образцовый элемент - префаб </summary>
    public Element elem;
    /// <summary> Текст для вывода </summary>
    public TextMeshProUGUI countOfBrokenText;
    #endregion
    #region Технические объекты
    /// <summary> Выбранный элемент </summary>
    Element selectElem;
    /// <summary> Ширина поля </summary>
    float fieldWidth;
    /// <summary> Высота поля </summary>
    float fieldHeight;
    /// <summary> Ширина элемента </summary>
    float elem_w;
    /// <summary> Высота элемента </summary>
    float elem_h;
    /// <summary> Названия файлов со стартовыми конфигурациями </summary>
    List<string> files;
    /// <summary> Названия цветов </summary>
    List<string> colorNames = new List<string>();
    /// <summary> Представление поля в памяти (скрытые элементы наверху включены) </summary>
    List<List<Element>> grid = new List<List<Element>>();
    /// <summary> Кол-во уже сломанных элементов каждого цвета </summary>
    int[] countOfBrokenByColor;
    /// <summary> Оставшееся кол-во ходов </summary>
    int currentMovesLeft;
    /// <summary> Текущее оставшееся время для решения мини-игры </summary>
    public float timeLeft;
    /// <summary> Событие завершения игры </summary>
    public UnityEvent<bool> gameFinished;
    /// <summary> Время для выполнения анимации, необходимо для точной остановки </summary>
    public float animationExitTime;
    /// <summary> Флаг завершения корутины </summary>
    bool coroutineFinished = true;
    #endregion

    //Функции
    #region Override функции
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
    #region Функции настройки стартового поля
    /// <summary>
    /// Поиск и считывание всех файлов начальной конфигурации
    /// </summary>
    /// <exception cref="System.Exception">Не найдены файлы стартовых конфигураций</exception>
    [Obsolete("Устаревший способ загрузки поля")]
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
    /// Считывает конфигурацию из случайного файла
    /// </summary>
    /// <exception cref="Exception">Ошибка при считывании файла конфигурации</exception>
    [Obsolete("Устаревший способ загрузки поля, может работать некорректно в связи с отсутствием последних изменений")]
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
                    clone.y = i - countOfLines + fieldSizeH; //ставим реальное положение на поле, то есть 0 - первая видимая линия, все отрицательные значения на поле не будут видны
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
            throw new Exception($"Не удалось считать конфигурацию из файла. Более подробная информация: {e.Message}");
        }
        //DrawField();
    }

    /// <summary>
    /// Сгенерировать случайную возможную конфигурацию поля
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
                    clone.y = i - countOfRandomLines + fieldSizeH; //ставим реальное положение на поле, то есть 0 - первая видимая линия, все отрицательные значения на поле не будут видны
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
    #region Основные функции игры

    /// <summary>
    /// Проверка, есть ли у данного элемента где-то хотя бы три в ряду
    /// </summary>
    /// <param name="e">Элемент для проверки</param>
    /// <returns>Первый элемент в данном ряду, первый для горизонтали, второй для вертикали</returns>
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
    /// Проверяем, не набрали ли мы уже достаточно сломанных элементов для победы и заканчиваем, если да
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
    /// Убирает все элементы с поля и ставит его в null
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
    /// Обновление текста в соответствии с текущим счетом
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
    /// Вызов SwapAndBreak как корутины
    /// </summary>
    /// <param name="e">Элемент, для которого будет вызываться swap</param>
    public void StartSwapCoroutine(Element e)
    {
        if (coroutineFinished)
        {
            StartCoroutine(SwapAndBreak(e));
        }
    }

    /// <summary>
    /// Основная функция, которая меняет между собой два элемента и ломает элементы, если необходимо
    /// </summary>
    /// <param name="e">Элемент, который выбрали</param>
    /// <returns></returns>
    public IEnumerator SwapAndBreak(Element e)
    {
        //если до этого никакого элемента не было выбрано, то делаем текущий элемент выбранным
        if (selectElem == null)
        {
            selectElem = e;
            yield break;
        }

        if (Math.Abs(e.x - selectElem.x) + Math.Abs(e.y - selectElem.y) == 1) //если текущий элемент соседний с выбранным до этого
        {
            coroutineFinished = false;
            ShowAnimSwap(e, selectElem);
            yield return new WaitForSeconds(animationExitTime);
            MakeTypeSwap(selectElem, e);//меняем их местами
            Tuple<Element, Element> res1 = CheckForThree(e);//проверяем, не получилась ли тройка у текущего элемента после обмена
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
            Tuple<Element, Element> res2 = CheckForThree(selectElem);//проверяем, не получилась ли тройка у выбранного до этого элемента после обмена
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
            if (res1.Item1 == null && res1.Item2 == null && res2.Item1 == null && res2.Item2 == null)//если никакой тройки не вышло, то меняем обратно
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
            yield return new WaitForSeconds(animationExitTime);//небольшое ожидание перед дальнейшими проверками, чтобы была пауза и было видно, что ломается
            bool needGoMore = false;
            do
            {
                needGoMore = false;
                for (int i = grid.Count - fieldSizeH; i < grid.Count; i++) //проходимся по видимому полю (видны последние fieldSize элементов)
                {
                    for (int j = 0; j < fieldSizeW; j++)
                    {
                        if (grid[i][j] == null)
                        {
                            continue;
                        }
                        res1 = CheckForThree(grid[i][j]); //проверяем, не получили ли троек где-то еще
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
            while (needGoMore == true); //пока никаких поломок не останется
            currentMovesLeft--;
            UpdateBrokenElemsText();
        }

        selectElem = null;

        CheckForWin();
        coroutineFinished = true;
    }

    /// <summary>
    /// Поломка троек камней (или больше, если имеется)
    /// </summary>
    /// <param name="firstElem">Первый элемент, от которого идет отсчет тройки или больше</param>
    /// <param name="horizontal">По горизонтали, если true, иначе false</param>
    /// <returns></returns>
    private IEnumerator BreakElems(Element firstElem, bool horizontal)
    {
        int x = firstElem.x;
        int y = GetFieldVertOffset(firstElem.y);
        if (horizontal)
        {
            for (int i = x; i < fieldSizeW && grid[y][i] != null && grid[y][i].type == firstElem.type; i++) //ломаем, пока по горизонтали идут элементы одного типа
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
            while (y + maxDown + 1 < grid.Count && grid[y + maxDown + 1][x].type == firstElem.type) //вычисляем, как глубоко вниз идут наши элементы
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
    /// Функция отрисовки видимой части поля
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
    /// Получаем положение как элемент располагается на поле относительно верхней границы
    /// </summary>
    /// <param name="y">Положение на видимом поле</param>
    /// <returns></returns>
    private int GetFieldVertOffset(int y)
    {
        return y + grid.Count - fieldSizeH;
    }

    /// <summary>
    /// Функция опускает вниз все элементы выше поломанного
    /// </summary>
    /// <param name="x">x элемента в grid</param>
    /// <param name="y">y элемента в grid (см. GetFieldVertOffset)</param>
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
    /// Запуск анимаций для элементов
    /// </summary>
    /// <param name="e1">Элемент 1</param>
    /// <param name="e2">Элемент 2</param>
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
    /// Функция обменивает типами между собой (только внутренне) два элемента
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
    /// Функция меняет местами спрайты у уже поменянных по внутреннему типу элементов
    /// </summary>
    /// <param name="e1"></param>
    /// <param name="e2"></param>
    private void MakeImageSwap(Element e1, Element e2)
    {
        e1.SetType(e1.type);
        e2.SetType(e2.type);
    }

    /// <summary>
    /// Нужен для таймера
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
