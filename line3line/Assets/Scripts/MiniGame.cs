using System.Collections;
using TMPro;
using UnityEngine;

/// <summary> ����-����.</summary>
/// <remarks> ������������ ����� ��� ���� ����-���.
/// ����-���� ������������ ����� ���� �������, ������� ���������� ������������
/// ��������� ���������.</remarks>
public class MiniGame : MonoBehaviour
{
    #region Private Fields
    /// <summary> ������ �� ������� ������.</summary>
    [SerializeField] private int solutionTimerValue;

    /// <summary> ������ ����-����.</summary>
    private MiniGameIcon miniGameIcon;

    /// <summary> ׸���� �����.</summary>
    /// <remarks> ������������ ���� ��� ���� ����-����.</remarks>
    private GameObject blackBg;
    #endregion

    #region Protected Fields
    /// <summary> ��������� ���� �������.</summary>
    [SerializeField] protected TMP_Text timerText;

    /// <summary> ������ � ��������� ����.</summary>
    [SerializeField] protected GameObject rulesPanel;

    /// <summary> ����� �� ������������ � ���������.</summary>
    protected const int RulesTimerValue = 3;

    /// <summary> �������� ����� ���������� �������.</summary>
    protected const float DelayBetweenPanels = 0.5f;
    #endregion

    #region Properties
    /// <summary> ���� �������? True (False) - �� (���).</summary>
    public bool IsGameOpened => blackBg.activeSelf;

    /// <summary> ���� ��������? True (False) - �� (���).</summary>
    private bool IsGameRunning { get; set; } = false;
    #endregion

    #region Private Methods
    /// <summary> ��� ������ �������������� ����.</summary>
    private void Start()
    {
        Initialize();
    }

    /// <summary> ���������� ����-����.</summary>
    private void Destroy()
    {
        Destroy(gameObject);
        if (miniGameIcon != null) Destroy(miniGameIcon.gameObject);
    }
    #endregion

    #region Protected Methods
    /// <summary> ������������� ����-����.</summary>
    protected virtual void Initialize()
    {
        blackBg = transform.GetChild(0).gameObject;
        //blackBg.SetActive(false);
        //timerText.gameObject.SetActive(false);
        //rulesPanel.SetActive(true);
    }

    /// <summary> ��������� ����-����.</summary>
    private void Run()
    {
        if (!IsGameRunning)
        {
            IsGameRunning = true;
            StartCoroutine(RulesPanelTimer());
        }
    }

    /// <summary> ������ ������� �� ������������ � ���������.</summary>
    /// <returns> ���������� �������� ������� � ���������.</returns>
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

    /// <summary> ��������� ����.</summary>
    protected virtual void LaunchGame()
    {

    }

    /// <summary> ������ ������� �� ������� ������.</summary>
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

    /// <summary> ������ ������.</summary>
    protected virtual void Win()
    {
        Invoke(nameof(Destroy), 2f);
        //Challenge.IncreaseSealsCount();
        StopAllCoroutines();
    }

    /// <summary> ��������� ������.</summary>
    protected virtual void Lose()
    {
        Invoke(nameof(Destroy), 2f);
    }
    #endregion

    #region Public Methods
    /// <summary> ���������� ���������� � ������.</summary>
    /// <param name="miniGameIcon"> ������ ����-����.</param>
    public void SetInfo(MiniGameIcon miniGameIcon)
    {
        this.miniGameIcon = miniGameIcon;
    }

    /// <summary> ������� ����-����.</summary>
    public void Open()
    {
        gameObject.SetActive(true);
        //blackBg.SetActive(true);  
        Run();
    }
    #endregion
}