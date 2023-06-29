using System.Collections;
using TMPro;
using UnityEngine;

/// <summary> ������ ����-����.</summary>
/// <remarks> ������ ������� � ������� ����� ����-����. 
/// ��� ������������ ��������� �� �����.</remarks>
public class MiniGameIcon : MonoBehaviour
{
    #region Private Fields
    /// <summary> ����� ������� �����.</summary>
    [SerializeField] private TMP_Text lifeTimeText;

    /// <summary> ����-����.</summary>
    private MiniGame miniGame;

    /// <summary> ����� ����� ����-����.</summary>
    private const int LifeTimeValue = 20;
    #endregion

    #region Private Methods
    /// <summary> ��� ������ ��������� ������.</summary>
    private void Start()
    {
        StartCoroutine(LifeTimeTimer());
    }

    /// <summary> ������ ������� �� ����� ����� �������.</summary>
    /// <returns> ���������� �������� �� ������� � 1 �������.</returns>
    private IEnumerator LifeTimeTimer()
    {
        int timerValue = LifeTimeValue;
        lifeTimeText.text = timerValue.ToString();
        while (timerValue-- > 0)
        {
            lifeTimeText.text = timerValue.ToString();
            yield return new WaitForSeconds(1);
        }
        Destroy();
    }

    /// <summary> ���������� ������ ����-����.</summary>
    private void Destroy()
    {
        Destroy(gameObject);
        if (!miniGame.IsGameOpened) Destroy(miniGame.gameObject);
    }
    #endregion

    #region Public Methods
    /// <summary> ���������� ���������� � ������.</summary>
    /// <param name="miniGame"> ����-����.</param>
    public void SetInfo(MiniGame miniGame)
    {
        this.miniGame = miniGame;
    }

    /// <summary> ������� �� ������� ������.</summary>
    public void OnClick()
    {
        miniGame.Open();
    }
    #endregion
}