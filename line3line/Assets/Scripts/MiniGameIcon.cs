using System.Collections;
using TMPro;
using UnityEngine;

/// <summary> Иконка мини-игры.</summary>
/// <remarks> Иконка свзяана с классом самой мини-игры. 
/// Она периодически спавнится на ленте.</remarks>
public class MiniGameIcon : MonoBehaviour
{
    #region Private Fields
    /// <summary> Текст времени жизни.</summary>
    [SerializeField] private TMP_Text lifeTimeText;

    /// <summary> Мини-игра.</summary>
    private MiniGame miniGame;

    /// <summary> Время жизни мини-игры.</summary>
    private const int LifeTimeValue = 20;
    #endregion

    #region Private Methods
    /// <summary> При старте запускаем таймер.</summary>
    private void Start()
    {
        StartCoroutine(LifeTimeTimer());
    }

    /// <summary> Отсчёт таймера на время жизни объекта.</summary>
    /// <returns> Возвращает задержку по времени в 1 секунду.</returns>
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

    /// <summary> Уничтожить иконку мини-игры.</summary>
    private void Destroy()
    {
        Destroy(gameObject);
        if (!miniGame.IsGameOpened) Destroy(miniGame.gameObject);
    }
    #endregion

    #region Public Methods
    /// <summary> Установить информацию о классе.</summary>
    /// <param name="miniGame"> Мини-игра.</param>
    public void SetInfo(MiniGame miniGame)
    {
        this.miniGame = miniGame;
    }

    /// <summary> Событие на нажатие кнопки.</summary>
    public void OnClick()
    {
        miniGame.Open();
    }
    #endregion
}