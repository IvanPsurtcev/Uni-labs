using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Element : MonoBehaviour
{
    public ThreeInRow game;
    public Image image;
    public int x;
    public int y;
    public int type;
    public static Object[] sprites;
    public Animator animator;

    static Element()
    {
        sprites = Resources.LoadAll("Diamonds", typeof(Sprite));
        if (sprites.Length == 0)
        {
            throw new System.Exception("No sprites were found");
        }
    }

    public Element(Element e)
    {
        this.game = e.game;
        this.x = e.x;
        this.y = e.y;
        this.type = e.type;
    }

    public void SetAnimatorSize(float size)
    {
        animator.SetFloat("size", size);
    }

    //вызываем функцию свапа при нажатии на элемент
    public void CallSwap()
    {
        if (animator.GetCurrentAnimatorStateInfo(0).IsName("Idle"))
        {
            game.StartSwapCoroutine(this);
        }
    }

    public void SetType(int type)
    {
        this.type = type;
        if (image == null)
        {
            throw new System.Exception("No image component was found");
        }
        image.sprite = sprites[type] as Sprite;
    }

    public void Remove()
    {
        this.gameObject.SetActive(false);
    }
}
