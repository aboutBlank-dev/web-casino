using System;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class UISpriteAnimation : MonoBehaviour
{
    [SerializeField] Image targetImage;
    public Sprite[] sprites;

    [Header("Control")]
    [SerializeField][Range(1, 60)] int framesPerSecond = 60;
    [SerializeField] bool playByDefault = true;
    [SerializeField] float waitSecondsAtEnd = 0.0f;
    [SerializeField] bool loop = true;
    [SerializeField] bool loopBack = false;

    private int currentFrame = 0;
    private int direction = 1;
    private float timer;
    private bool playing;
    private bool waiting;
    private bool waitedRecently;

    private Action onFinish;

    void Start()
    {
        if (targetImage != null && sprites.Length > 0)
            targetImage.sprite = sprites[0];

        if (playByDefault)
            playing = true;
    }

    public void PlayOnce(Action onFinish = null)
    {
        this.onFinish = onFinish;

        loop = false;
        playing = true;
        currentFrame = 0;
    }

    public void PlayLoop(bool loopBack = false)
    {
        this.loopBack = loopBack;
        loop = true;
        playing = true;
        currentFrame = 0;
    }

    public void Stop()
    {
        playing = false;
    }

    void Update()
    {
        if (targetImage == null || sprites.Length == 0)
            return;

        if (!playing) return;

        timer += Time.deltaTime;

        if (waiting)
        {
            if (timer >= waitSecondsAtEnd)
            {
                waiting = false;
                waitedRecently = true;
                timer = 0;
            }
            else return;
        }

        if (timer >= 1f / framesPerSecond)
        {
            timer = 0;
            currentFrame = (currentFrame + direction) % sprites.Length;
            targetImage.sprite = sprites[currentFrame];

            if (currentFrame == sprites.Length - 1)
            {
                if (!loop)
                {
                    playing = false;

                    if (onFinish != null)
                    {
                        onFinish();
                        onFinish = null;
                    }
                    return;
                }

                direction = loopBack ? -1 : 1;
                waiting = !waitedRecently && waitSecondsAtEnd > 0;
            }
            else if (currentFrame == 0)
            {
                direction = 1;
                waiting = !waitedRecently && waitSecondsAtEnd > 0;
            }

            waitedRecently = false;
        }
    }
}
