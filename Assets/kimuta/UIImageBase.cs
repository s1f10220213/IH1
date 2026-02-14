using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class UIImageBase : MonoBehaviour
{
    public enum EasingType
    {
        Linear,
        EaseIn,
        EaseOut
    }

    private Image image;
    private RectTransform rect;

    //　画像のパラメータ
    private Color imageColor;
    private float alpha = 1;

    [Header("BlinkSettings")]
    [SerializeField] bool isBlink;
    [SerializeField] float blinkDuration;
    
    [Header("BouceScaleSettings")]
    [SerializeField] bool isBounceScale;
    [SerializeField] float bounceScaleDuration;
    [SerializeField] float bounceScaleMin;
    [SerializeField] float bounceScaleMax;

    private void Start()
    {
        image = this.gameObject.GetComponent<Image>();
        rect = this.gameObject.GetComponent<RectTransform>();

        imageColor = image.color;
    }

    private void Update()
    {
        if (isBlink)
        {
            alpha = (Mathf.Sin(Time.time * (2.0f * Mathf.PI / blinkDuration)) + 1.0f) * 0.5f;

            imageColor.a = alpha;
            image.color = imageColor;
        }

        if (isBounceScale)
        {
            float scale = bounceScaleMin + Mathf.Abs(Mathf.Sin(Time.time * (Mathf.PI / bounceScaleDuration))) * (bounceScaleMax-bounceScaleMin);
            rect.localScale = new Vector3(scale, scale, 1f);
        }
    }

    private void SetPosition(Vector2 pos)
    {
        rect.anchoredPosition = pos;
    }

    public void SetSize(Vector2 size)
    {
        rect.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, size.x);
        rect.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, size.y);
    }

    public void SetScale(Vector2 scale)
    {
        rect.localScale = scale;
    }

    public void SetColor(Color color)
    {
        imageColor = new Color(color.r, color.g, color.b, imageColor.a);
        image.color = imageColor;
    }

    public void SetSprite(Sprite s)
    {
        image.sprite = s;
    }

    ///////////////////////////////////////////////////////////////////////
    //　演出呼び出し系
    ///////////////////////////////////////////////////////////////////////
    
    //　振動
    public void ShakeImage(float radius, float limitTime)
    {
        StartCoroutine(Shake(radius, limitTime));
    }

    //　点滅
    public void BlinkImageStart(float duration)
    {
        isBlink = true;
        blinkDuration = duration;
    }
    public void BlinkImageStop(bool isVisible)
    {
        isBlink = false;
        if (isVisible)
        {
            imageColor.a = 1;
            image.color = imageColor;
        }
        else
        {
            imageColor.a = 0;
            image.color = imageColor;
        }
    }

    //　フェードイン・アウト
    public void FadeInImage(float duration)
    {
        StartCoroutine(FadeIn(duration));
    }
    public void FadeOutImage(float duration)
    {
        StartCoroutine(FadeOut(duration));
    }

    //　拡大縮小
    public void BounceScaleImageStart(float duration, float minScale, float maxScale)
    {
        isBounceScale = true;
        bounceScaleDuration = duration;
        bounceScaleMin = minScale;
        bounceScaleMax = maxScale;
    }
    public void BounceScaleImageStop()
    {
        isBounceScale = false;
        rect.localScale = Vector3.one;
    }
    public void ScaleToScaleImage(Vector2 fromScale, Vector2 toScale, float duration)
    {
        StartCoroutine(ScaleToScale(fromScale, toScale, duration));
    }

    public void MoveFromTo(Vector2 fromPos, Vector2 toPos, float duration, EasingType easeType)
    {
        StartCoroutine(MoveWithEasing(fromPos, toPos, duration, easeType));
    }

    public void MoveDirection(Vector2 dir, float range, float duration, EasingType easeType)
    {
        StartCoroutine(MoveWithEasing(rect.anchoredPosition, rect.anchoredPosition+dir.normalized*range, duration, easeType));
    }

    ///////////////////////////////////////////////////////////////////////
    //　演出系
    ///////////////////////////////////////////////////////////////////////
    
    //　振動
    private IEnumerator Shake(float radius, float limitTime)
    {
        float time = 0;
        float rate = 1/limitTime;
        Vector2 basePos = rect.anchoredPosition;
        
        while(true)
        {
            if(time >= limitTime)
            {
                rect.anchoredPosition = basePos;
                yield break;
            }
            
            float angle = Random.Range(-180, 180);
            rect.anchoredPosition = basePos + new Vector2(Mathf.Cos(Mathf.Deg2Rad*angle), Mathf.Sin(Mathf.Deg2Rad*angle)) * radius * (1-time*rate);

            time += 0.05f;
            yield return new WaitForSeconds(0.05f);
        }
    }

    //　フェードイン
    private IEnumerator FadeIn(float duration)
    {
        float time = 0;
        float rate = 1/duration;

        while(true)
        {
            if (time >= duration)
            {
                imageColor.a = 1;
                image.color = imageColor;
                yield break;
            }

            imageColor.a = time * rate;
            image.color = imageColor;

            time += Time.deltaTime;
            yield return null;
        }
    }
    //　フェードアウト
    private IEnumerator FadeOut(float duration)
    {
        float time = 0;
        float rate = 1/duration;

        while(true)
        {
            if (time >= duration)
            {
                imageColor.a = 0;
                image.color = imageColor;
                yield break;
            }

            imageColor.a = 1 - (time * rate);
            image.color = imageColor;
            
            time += Time.deltaTime;
            yield return null;
        }
    }

    //　移動
    private IEnumerator MoveWithEasing(Vector2 startPos, Vector2 endPos, float duration, EasingType easing)
    {
        float time = 0;
        float rate = 1 / duration;

        while (true)
        {
            if (time >= duration)
            {
                rect.anchoredPosition = endPos;
                yield break;
            }

            time += Time.deltaTime;
            float t = time * rate;

            float easedT = ApplyEasing(t, easing);
            rect.anchoredPosition = Vector2.Lerp(startPos, endPos, easedT);

            yield return null;
        }
    }

    //　拡大縮小
    private IEnumerator ScaleToScale(Vector2 fromScale, Vector2 toScale, float duration)
    {
        float time = 0;
        float rate = 1 / duration;
        rect.localScale = fromScale;

        while (true)
        {
            if (time >= duration)
            {
                rect.localScale = toScale;
                yield break;
            }

            rect.localScale = Vector2.Lerp(fromScale, toScale, time*rate);
            time += Time.deltaTime;
            yield return null;
        }
    }

    ///////////////////////////////////////////////////////////////////////
    //　イージング処理
    ///////////////////////////////////////////////////////////////////////
    
    private float ApplyEasing(float t, EasingType type)
    {
        float result = 0;

        if (type == EasingType.Linear)
        {
            result = t; //　等速
        }
        else if (type == EasingType.EaseIn)
        {
            result = 1 - (1 - t) * (1 - t); //　減速
        }
        else if (type == EasingType.EaseOut)
        {
            result = t * t * t; //　加速
        }

        return result;
    }
}
