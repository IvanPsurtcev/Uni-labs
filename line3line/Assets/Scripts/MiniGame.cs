using System.Collections;
using TMPro;
using UnityEngine;

/// <summary> Мини-игра.</summary>
/// <remarks> Родительский класс для всех мини-игр.
/// Мини-игра представляет собой мини задание, которое необходимо периодически
/// выполнять магистрам.</remarks>
public class MiniGame : MonoBehaviour
{
    #region Private Fields
    /// <summary> Таймер на решение задачи.</summary>
    [SerializeField] private int solutionTimerValue;

    /// <summary> Иконка мини-игры.</summary>
    private MiniGameIcon miniGameIcon;

    /// <summary> Чёрный экран.</summary>
    /// <remarks> Родительский узел для окна мини-игры.</remarks>
    private GameObject blackBg;
    #endregion

    #region Protected Fields
    /// <summary> Текстовое поле таймера.</summary>
    [SerializeField] protected TMP_Text timerText;

    /// <summary> Панель с правилами игры.</summary>
    [SerializeField] protected GameObject rulesPanel;

    /// <summary> Время на ознакомление с правилами.</summary>
    protected const int RulesTimerValue = 3;

    /// <summary> Задержка между появлением панелей.</summary>
    protected const float DelayBetweenPanels = 0.5f;
    #endregion

    #region Properties
    /// <summary> Игра открыта? True (False) - да (нет).</summary>
    public bool IsGameOpened => blackBg.activeSelf;

    /// <summary> Игра запущена? True (False) - да (нет).</summary>
    private bool IsGameRunning { get; set; } = false;
    #endregion

    #region Private Methods
    /// <summary> При старте инициализируем игру.</summary>
    private void Start()
    {
        Initialize();
    }

    /// <summary> Уничтожить мини-игру.</summary>
    private void Destroy()
    {
        Destroy(gameObject);
        if (miniGameIcon != null) Destroy(miniGameIcon.gameObject);
    }
    #endregion

    #region Protected Methods
    /// <summary> Инициализация мини-игры.</summary>
    protected virtual void Initialize()
    {
        blackBg = transform.GetChild(0).gameObject;
        //blackBg.SetActive(false);
        //timerText.gameObject.SetActive(false);
        //rulesPanel.SetActive(true);
    }

    /// <summary> Запустить мини-игру.</summary>
    private void Run()
    {
        if (!IsGameRunning)
        {
            IsGameRunning = true;
            StartCoroutine(RulesPanelTimer());
        }
    }

    /// <summary> Отсчёт таймера на ознакомление с правилами.</summary>
    /// <returns> Возвращает значение таймера с задержкой.</returns>
    private IEnumerator RulesPanelTimer()
    {
        int timerValue = RulesTimerValue;
        while (timerValue-- > 0)
        {
            yield return new WaitForSeconds(1);
        }
        //rulesPanel.SetActive(false);
        yield return new WaitForSeconds(DelayBetweenPanels);

        LaunchGame();
    }

    /// <summary> Запустить игру.</summary>
    protected virtual void LaunchGame()
    {

    }

    /// <summary> Отсчёт таймера на решение задачи.</summary>
    protected virtual IEnumerator SolutionTimer()
    {
        int timerValue = solutionTimerValue;
        //timerText.text = solutionTimerValue.ToString();
        while (timerValue-- > 0)
        {
            //timerText.text = timerValue.ToString();
            yield return new WaitForSeconds(1);
        }
        Lose();
    }

    /// <summary> Победа игрока.</summary>
    protected virtual void Win()
    {
        Invoke(nameof(Destroy), 2f);
        //Challenge.IncreaseSealsCount();
        StopAllCoroutines();
    }

    /// <summary> Поражение игрока.</summary>
    protected virtual void Lose()
    {
        Invoke(nameof(Destroy), 2f);
    }
    #endregion

    #region Public Methods
    /// <summary> Установить информацию о классе.</summary>
    /// <param name="miniGameIcon"> Иконка мини-игры.</param>
    public void SetInfo(MiniGameIcon miniGameIcon)
    {
        this.miniGameIcon = miniGameIcon;
    }

    /// <summary> Открыть мини-игру.</summary>
    public void Open()
    {
        gameObject.SetActive(true);
        //blackBg.SetActive(true);  
        Run();
    }
    #endregion
}