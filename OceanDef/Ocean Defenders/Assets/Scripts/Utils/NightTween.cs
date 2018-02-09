using UnityEngine;
using UnityEngine.UI;
using System.Collections;

/// <summary>
/// Instance used to control a single object.
/// </summary>
public class NightTweenInstance : MonoBehaviour
{
    /// <summary>
    /// GameObject being animated.
    /// </summary>
    [SerializeField]
    private GameObject _targetObject;
    /// <summary>
    /// Duration of a single loop.
    /// </summary>
    [SerializeField]
    private float _duration;
    /// <summary>
    /// Number of loops completed.
    /// </summary>
    [SerializeField]
    private int _loopsComplete = 0;
    /// <summary>
    /// Parameters acquired during creation.
    /// </summary>
    [SerializeField]
    private NightTweenParams.StoredParams _parameters;
    /// <summary>
    /// Time born.
    /// </summary>
    private float _born = 0f;
    /// <summary>
    /// Time started.
    /// </summary>
    private float _started = 0f;

    /// <summary>
    /// Use regular Update.
    /// </summary>
    private bool useUpdate = false;
    /// <summary>
    /// Use LateUpdate.
    /// </summary>
    private bool useLateUpdate = false;
    /// <summary>
    /// Use FixedUpdate.
    /// </summary>
    private bool useFixedUpdate = false;
    /// <summary>
    /// Use regular Update with real world time.
    /// </summary>
    private bool useTimescaleInd = false;

    /// <summary>
    /// Stores if the delay was completed.
    /// </summary>
    private bool pastDelay = false;

    /// <summary>
    /// Starting values to lerp.
    /// </summary>
    private object[] startingValue;
    /// <summary>
    /// End values to lerp.
    /// </summary>
    private object[] endValue;


    /// <summary>
    /// Setups the NightTweenInstance.
    /// </summary>
    /// <param name="targetObject">GameObject being animated.</param>
    /// <param name="duration">Duration of a single loop.</param>
    /// <param name="parameters">Parameters acquired during creation.</param>
    public void Setup(GameObject targetObject, float duration, NightTweenParams parameters)
    {
        _targetObject = targetObject;
        _duration = duration;
        _parameters = parameters.storedParams;

        if (_parameters.updateMode == NightTween.UpdateType.Update) useUpdate = true;
        else if (_parameters.updateMode == NightTween.UpdateType.LateUpdate) useLateUpdate = true;
        else if (_parameters.updateMode == NightTween.UpdateType.FixedUpdate) useFixedUpdate = true;
        else if (_parameters.updateMode == NightTween.UpdateType.TimeScaleIndependentUpdate) useTimescaleInd = true;

        _born = TimeNow();
        _started = _born + _parameters.delay;

        endValue = _parameters.propertyValue;
        startingValue = new object[_parameters.property.Length];
        for (int x = 0; x < startingValue.Length; x++)
        {
            startingValue[x] = GetValue(_parameters.property[x]);
        }

        if (_parameters.startCallback != null)
        {
            for (int x = 0; x < _parameters.startCallback.Length; x++)
            {
                if (_parameters.startCallback[x] != null)
                {
                    _parameters.startCallback[x]();
                }
            }
        }
    }

    /// <summary>
    /// Returns current time.
    /// </summary>
    private float TimeNow()
    {
        if (useTimescaleInd) return Time.realtimeSinceStartup;
        else return Time.time;
    }

    /// <summary>
    /// Update happens every frame.
    /// </summary>
    private void Update()
    {
        if (useUpdate || useTimescaleInd)
        {
            ProcessFrame();
        }
    }

    /// <summary>
    /// LateUpdate happens every frame after Update.
    /// </summary>
    private void LateUpdate()
    {
        if (useLateUpdate)
        {
            ProcessFrame();
        }
    }

    /// <summary>
    /// FixelUpdate happens every physics frame.
    /// </summary>
    private void FixedUpdate()
    {
        if (useFixedUpdate)
        {
            ProcessFrame();
        }
    }

    /// <summary>
    /// Processes the current frame.
    /// </summary>
    private void ProcessFrame()
    {
        if (_targetObject == null)
        {
            Destroy(gameObject);
            return;
        }

        if (!pastDelay)
        {
            float delayDelta = TimeNow() - _born;
            if (delayDelta >= _parameters.delay)
            {
                pastDelay = true;
            }
            return;
        }

        float delta = TimeNow() - _started;
        float lerp = delta;
        if (_duration <= 0)
        {
            lerp = 1f;
        }
        else
        {
            lerp = lerp / _duration;
            lerp = lerp > 1f ? 1f : lerp;
        }

        int length = _parameters.property.Length;
        for (int x = 0; x < length; x++)
        {
            SetValue(x, NightTween.Ease(_parameters.easeType, lerp));
        }

        if (lerp >= 1f)
        {
            _loopsComplete++;

            if (_parameters.loops < _loopsComplete)
            {
                if (_parameters.finishCallback != null)
                {
                    for (int x = 0; x < _parameters.finishCallback.Length; x++)
                    {
                        if (_parameters.finishCallback[x] != null)
                        {
                            _parameters.finishCallback[x]();
                        }
                    }
                }

                Destroy(gameObject);
                return;
            }

            _started += _duration;

            if (_parameters.loopType == NightTween.LoopType.Yoyo)
            {
                object[] temp = startingValue;
                startingValue = endValue;
                endValue = temp;
            }

            _parameters.easeType = (NightTween.EaseType)((int)_parameters.easeType + 1);

            if (_parameters.endCycleCallback != null)
            {
                for (int x = 0; x < _parameters.endCycleCallback.Length; x++)
                {
                    if (_parameters.endCycleCallback[x] != null)
                    {
                        _parameters.endCycleCallback[x]();
                    }
                }
            }
        }

        if (_parameters.updateCallback != null)
        {
            for (int x = 0; x < _parameters.updateCallback.Length; x++)
            {
                if (_parameters.updateCallback[x] != null)
                {
                    _parameters.updateCallback[x]();
                }
            }
        }
    }

    /// <summary>
    /// Gets a value according to NTPropType.
    /// </summary>
    private object GetValue(NTPropType pType)
    {
        if (pType == NTPropType.transformPosition)
            return _targetObject.transform.position;
        else if (pType == NTPropType.transformLocalPosition)
            return _targetObject.transform.localPosition;
        else if (pType == NTPropType.transformRotation)
            return _targetObject.transform.rotation;
        else if (pType == NTPropType.transformLocalScale)
            return _targetObject.transform.localScale;
        else if (pType == NTPropType.transformEulerAngles)
            return _targetObject.transform.eulerAngles;
        else if (pType == NTPropType.spriteRendererColor)
            return _targetObject.GetComponent<SpriteRenderer>().color;
        else if (pType == NTPropType.uiTextColor)
            return _targetObject.GetComponent<Text>().color;
        else if (pType == NTPropType.rectTransformAnchoredPosition)
            return _targetObject.GetComponent<RectTransform>().anchoredPosition;
        else if (pType == NTPropType.rectTransformSizeDelta)
            return _targetObject.GetComponent<RectTransform>().sizeDelta;
        else if (pType == NTPropType.canvasGroupAlpha)
            return _targetObject.GetComponent<CanvasGroup>().alpha;

        return null;
    }

    /// <summary>
    /// Sets a value according to NTPropType.
    /// </summary>
    private void SetValue(int index, float mult)
    {
        if (_parameters.property[index] == NTPropType.transformPosition)
        {
            Vector3 start = (Vector3)startingValue[index];
            Vector3 end = (Vector3)endValue[index];
            _targetObject.transform.position = start + (end - start) * mult;
        }
        else if (_parameters.property[index] == NTPropType.transformLocalPosition)
        {
            Vector3 start = (Vector3)startingValue[index];
            Vector3 end = (Vector3)endValue[index];
            _targetObject.transform.localPosition = start + (end - start) * mult;
        }
        else if (_parameters.property[index] == NTPropType.transformRotation)
        {
            Quaternion start = (Quaternion)startingValue[index];
            Quaternion end = (Quaternion)endValue[index];
            _targetObject.transform.rotation = Quaternion.Lerp(start, end, mult);
        }
        else if (_parameters.property[index] == NTPropType.transformLocalScale)
        {
            Vector3 start = (Vector3)startingValue[index];
            Vector3 end = (Vector3)endValue[index];
            _targetObject.transform.localScale = start + (end - start) * mult;
        }
        else if (_parameters.property[index] == NTPropType.transformEulerAngles)
        {
            Vector3 start = (Vector3)startingValue[index];
            Vector3 end = (Vector3)endValue[index];
            _targetObject.transform.rotation = Quaternion.Lerp(Quaternion.Euler(start), Quaternion.Euler(end), mult);
        }
        else if (_parameters.property[index] == NTPropType.spriteRendererColor)
        {
            Color start = (Color)startingValue[index];
            Color end = (Color)endValue[index];
            _targetObject.GetComponent<SpriteRenderer>().color = Color.Lerp(start, end, mult);
        }
        else if (_parameters.property[index] == NTPropType.uiTextColor)
        {
            Color start = (Color)startingValue[index];
            Color end = (Color)endValue[index];
            _targetObject.GetComponent<Text>().color = Color.Lerp(start, end, mult);
        }
        else if (_parameters.property[index] == NTPropType.rectTransformAnchoredPosition)
        {
            Vector2 start = (Vector2)startingValue[index];
            Vector2 end = (Vector2)endValue[index];
            _targetObject.GetComponent<RectTransform>().anchoredPosition = start + (end - start) * mult;
        }
        else if (_parameters.property[index] == NTPropType.rectTransformSizeDelta)
        {
            Vector2 start = (Vector2)startingValue[index];
            Vector2 end = (Vector2)endValue[index];
            _targetObject.GetComponent<RectTransform>().sizeDelta = start + (end - start) * mult;
        }
        else if(_parameters.property[index] == NTPropType.canvasGroupAlpha)
        {
            float start = (float)startingValue[index];
            float end = (float)endValue[index];
            _targetObject.GetComponent<CanvasGroup>().alpha = start + (end - start) * mult;
        }
    }
}


/// <summary>
/// Property type to be used.
/// </summary>
public enum NTPropType
{
    /// <summary>
    /// Transform.position.
    /// </summary>
    transformPosition,
    /// <summary>
    /// Transform.rotation.
    /// </summary>
    transformRotation,
    /// <summary>
    /// Transform.localScale.
    /// </summary>
    transformLocalScale,
    /// <summary>
    /// UI.Text.color.
    /// </summary>
    uiTextColor,
    /// <summary>
    /// Transform.eulerAngles.
    /// </summary>
    transformEulerAngles,
    /// <summary>
    /// RectTransform.anchoredPosition.
    /// </summary>
    rectTransformAnchoredPosition,
    /// <summary>
    /// RectTransform.sizeDelta.
    /// </summary>
    rectTransformSizeDelta,
    /// <summary>
    /// Transform.localPosition.
    /// </summary>
    transformLocalPosition,
    /// <summary>
    /// canvasGroup.alpha
    /// </summary>
    canvasGroupAlpha,
    /// <summary>
    /// SpriteRenderer.color
    /// </summary>
    spriteRendererColor
}


/// <summary>
/// Main tween class.
/// </summary>
public static class NightTween
{
    /// <summary>
    /// Container for all created NightTweenInstances.
    /// </summary>
    private static GameObject container;

    /// <summary>
    /// Used to create a tween object.
    /// </summary>
    /// <param name="targetObject">GameObject to be targeted.</param>
    /// <param name="duration">The duration of a single run of the tween.</param>
    /// <param name="parameters">Tween parameters.</param>
    public static GameObject Create(GameObject targetObject, float duration, NightTweenParams parameters)
    {
        if (targetObject == null) return null;
        if (parameters == null) return null;
        if (container == null) container = new GameObject("NightTween");

        GameObject o = new GameObject(parameters.storedParams.id);
        o.transform.SetParent(container.transform);

        NightTweenInstance t = o.AddComponent<NightTweenInstance>();
        t.Setup(targetObject, duration, parameters);

        return o;
    }

    /// <summary>
    /// Loop type.
    /// </summary>
    public enum LoopType
    {
        /// <summary>
        /// Restarts at the initial state.
        /// </summary>
        Restart = 0,
        /// <summary>
        /// Swaps start and end state after a cycle has been completed.
        /// </summary>
        Yoyo = 1
    }

    /// <summary>
    /// Update type.
    /// </summary>
    public enum UpdateType
    {
        /// <summary>
        /// Use regular Update.
        /// </summary>
        Update = 0,
        /// <summary>
        /// Use LateUpdate.
        /// </summary>
        LateUpdate = 1,
        /// <summary>
        /// Use FixedUpdate.
        /// </summary>
        FixedUpdate = 2,
        /// <summary>
        /// Use regular Update with real world time.
        /// </summary>
        TimeScaleIndependentUpdate = 3,
    }

    /// <summary>
    /// Easing functions.
    /// </summary>
    public enum EaseType
    {
        Linear,
        ExpoEaseOut,
        ExpoEaseIn,
        ExpoEaseInOut,
        ExpoEaseOutIn,
        CircEaseOut,
        CircEaseIn,
        CircEaseInOut,
        CircEaseOutIn,
        QuadEaseOut,
        QuadEaseIn,
        QuadEaseInOut,
        QuadEaseOutIn,
        SineEaseOut,
        SineEaseIn,
        SineEaseInOut,
        SineEaseOutIn,
        CubicEaseOut,
        CubicEaseIn,
        CubicEaseInOut,
        CubicEaseOutIn,
        QuartEaseOut,
        QuartEaseIn,
        QuartEaseInOut,
        QuartEaseOutIn,
        QuintEaseOut,
        QuintEaseIn,
        QuintEaseInOut,
        QuintEaseOutIn,
        ElasticEaseOut,
        ElasticEaseIn,
        ElasticEaseInOut,
        ElasticEaseOutIn,
        BounceEaseOut,
        BounceEaseIn,
        BounceEaseInOut,
        BounceEaseOutIn,
        BackEaseOut,
        BackEaseIn,
        BackEaseInOut,
        BackEaseOutIn
    }

    /// <summary>
    /// Apply Ease function.
    /// </summary>
    /// <param name="type">Ease Type.</param>
    /// <param name="lerp">Value to lerp from 0 to 1.</param>
    public static float Ease(EaseType type, float lerp)
    {
        switch (type)
        {
            case EaseType.Linear:
                return Linear(lerp);
            case EaseType.ExpoEaseOut:
                return ExpoEaseOut(lerp);
            case EaseType.ExpoEaseIn:
                return ExpoEaseIn(lerp);
            case EaseType.ExpoEaseInOut:
                return ExpoEaseInOut(lerp);
            case EaseType.ExpoEaseOutIn:
                return ExpoEaseOutIn(lerp);
            case EaseType.CircEaseOut:
                return CircEaseOut(lerp);
            case EaseType.CircEaseIn:
                return CircEaseIn(lerp);
            case EaseType.CircEaseInOut:
                return CircEaseInOut(lerp);
            case EaseType.CircEaseOutIn:
                return CircEaseOutIn(lerp);
            case EaseType.QuadEaseOut:
                return QuadEaseOut(lerp);
            case EaseType.QuadEaseIn:
                return QuadEaseIn(lerp);
            case EaseType.QuadEaseInOut:
                return QuadEaseInOut(lerp);
            case EaseType.QuadEaseOutIn:
                return QuadEaseOutIn(lerp);
            case EaseType.SineEaseOut:
                return SineEaseOut(lerp);
            case EaseType.SineEaseIn:
                return SineEaseIn(lerp);
            case EaseType.SineEaseInOut:
                return SineEaseInOut(lerp);
            case EaseType.SineEaseOutIn:
                return SineEaseOutIn(lerp);
            case EaseType.CubicEaseOut:
                return CubicEaseOut(lerp);
            case EaseType.CubicEaseIn:
                return CubicEaseIn(lerp);
            case EaseType.CubicEaseInOut:
                return CubicEaseInOut(lerp);
            case EaseType.CubicEaseOutIn:
                return CubicEaseOutIn(lerp);
            case EaseType.QuartEaseOut:
                return QuartEaseOut(lerp);
            case EaseType.QuartEaseIn:
                return QuartEaseIn(lerp);
            case EaseType.QuartEaseInOut:
                return QuartEaseInOut(lerp);
            case EaseType.QuartEaseOutIn:
                return QuartEaseOutIn(lerp);
            case EaseType.QuintEaseOut:
                return QuintEaseOut(lerp);
            case EaseType.QuintEaseIn:
                return QuintEaseIn(lerp);
            case EaseType.QuintEaseInOut:
                return QuintEaseInOut(lerp);
            case EaseType.QuintEaseOutIn:
                return QuintEaseOutIn(lerp);
            case EaseType.ElasticEaseOut:
                return ElasticEaseOut(lerp);
            case EaseType.ElasticEaseIn:
                return ElasticEaseIn(lerp);
            case EaseType.ElasticEaseInOut:
                return ElasticEaseInOut(lerp);
            case EaseType.ElasticEaseOutIn:
                return ElasticEaseOutIn(lerp);
            case EaseType.BounceEaseOut:
                return BounceEaseOut(lerp);
            case EaseType.BounceEaseIn:
                return BounceEaseIn(lerp);
            case EaseType.BounceEaseInOut:
                return BounceEaseInOut(lerp);
            case EaseType.BounceEaseOutIn:
                return BounceEaseOutIn(lerp);
            case EaseType.BackEaseOut:
                return BackEaseOut(lerp);
            case EaseType.BackEaseIn:
                return BackEaseIn(lerp);
            case EaseType.BackEaseInOut:
                return BackEaseInOut(lerp);
            case EaseType.BackEaseOutIn:
                return BackEaseOutIn(lerp);
            default:
                break;
        }
        return lerp;
    }


    #region Interpolation Equations

    public static float Linear(float t)
    {
        return t;
    }

    public static float ExpoEaseOut(float t)
    {
        return (t == 1f) ? 1f : 1f * (-Mathf.Pow(2f, -10f * t) + 1f);
    }

    public static float ExpoEaseIn(float t)
    {
        return (t == 0f) ? 0f : 1f * Mathf.Pow(2f, 10f * (t - 1f)) + 0f;
    }

    public static float ExpoEaseInOut(float t)
    {
        if (t == 0f)
            return 0f;

        if (t == 1f)
            return 1f;

        if ((t /= 0.5f) < 1f)
            return 0.5f * Mathf.Pow(2f, 10f * (t - 1f));

        return 0.5f * (-Mathf.Pow(2f, -10f * --t) + 2f);
    }

    public static float ExpoEaseOutIn(float t)
    {
        if (t < 0.5f)
            return ExpoEaseOut(t * 2f) * 0.5f;

        return ExpoEaseIn((t - 0.5f) * 2f) * 0.5f + 0.5f;
    }

    public static float CircEaseOut(float t)
    {
        return 1f * Mathf.Sqrt(1f - (t = t - 1f) * t);
    }

    public static float CircEaseIn(float t)
    {
        return -1f * (Mathf.Sqrt(1f - (t /= 1f) * t) - 1f);
    }

    public static float CircEaseInOut(float t)
    {
        if ((t /= 0.5f) < 1f)
            return -0.5f * (Mathf.Sqrt(1f - t * t) - 1f);

        return 0.5f * (Mathf.Sqrt(1f - (t -= 2f) * t) + 1f);
    }

    public static float CircEaseOutIn(float t)
    {
        if (t < 0.5f)
            return CircEaseOut(t * 2f) * 0.5f;

        return CircEaseIn((t - 0.5f) * 2f) * 0.5f + 0.5f;
    }

    public static float QuadEaseOut(float t)
    {
        return -1f * (t /= 1f) * (t - 2f);
    }

    public static float QuadEaseIn(float t)
    {
        return 1f * (t /= 1f) * t;
    }

    public static float QuadEaseInOut(float t)
    {
        if ((t /= 0.5f) < 1f)
            return 0.5f * t * t;

        return -0.5f * ((--t) * (t - 2f) - 1f);
    }

    public static float QuadEaseOutIn(float t)
    {
        if (t < 0.5f)
            return QuadEaseOut(t * 2f) * 0.5f;

        return QuadEaseIn((t - 0.5f) * 2f) * 0.5f + 0.5f;
    }

    public static float SineEaseOut(float t)
    {
        return 1f * Mathf.Sin(t * (Mathf.PI * 0.5f));
    }

    public static float SineEaseIn(float t)
    {
        return -1f * Mathf.Cos(t * (Mathf.PI * 0.5f)) + 1f;
    }

    public static float SineEaseInOut(float t)
    {
		if (t < 0.5f)
			return SineEaseIn(t * 2f) * 0.5f;
		
		return SineEaseOut((t - 0.5f) * 2f) * 0.5f + 0.5f;
    }

    public static float SineEaseOutIn(float t)
    {
        if (t < 0.5f)
            return SineEaseOut(t * 2f) * 0.5f;

        return SineEaseIn((t - 0.5f) * 2f) * 0.5f + 0.5f;
    }

    public static float CubicEaseOut(float t)
    {
        return 1f * ((t = t - 1f) * t * t + 1f);
    }

    public static float CubicEaseIn(float t)
    {
        return 1f * (t /= 1f) * t * t;
    }

    public static float CubicEaseInOut(float t)
    {
        if ((t /= 0.5f) < 1f)
            return 0.5f * t * t * t;

        return 0.5f * ((t -= 2f) * t * t + 2f);
    }

    public static float CubicEaseOutIn(float t)
    {
        if (t < 0.5f)
            return CubicEaseOut(t * 2f) * 0.5f;

        return CubicEaseIn((t - 0.5f) * 2f) * 0.5f + 0.5f;
    }

    public static float QuartEaseOut(float t)
    {
        return -1f * ((t = t - 1f) * t * t * t - 1f);
    }

    public static float QuartEaseIn(float t)
    {
        return 1f * (t /= 1f) * t * t * t;
    }

    public static float QuartEaseInOut(float t)
    {
        if ((t /= 0.5f) < 1f)
            return 0.5f * t * t * t * t;

        return -0.5f * ((t -= 2f) * t * t * t - 2f);
    }

    public static float QuartEaseOutIn(float t)
    {
        if (t < 0.5f)
            return QuartEaseOut(t * 2f) * 0.5f;

        return QuartEaseIn((t - 0.5f) * 2f) * 0.5f + 0.5f;
    }

    public static float QuintEaseOut(float t)
    {
        return 1f * ((t = t - 1f) * t * t * t * t + 1f);
    }

    public static float QuintEaseIn(float t)
    {
        return 1f * (t /= 1f) * t * t * t * t;
    }

    public static float QuintEaseInOut(float t)
    {
        if ((t /= 0.5f) < 1f)
            return 0.5f * t * t * t * t * t;
        return 0.5f * ((t -= 2f) * t * t * t * t + 2f);
    }

    public static float QuintEaseOutIn(float t)
    {
        if (t < 0.5f)
            return QuintEaseOut(t * 2f) * 0.5f;
        return QuintEaseIn((t - 0.5f) * 2f) * 0.5f + 0.5f;
    }

    public static float ElasticEaseOut(float t)
    {
        if ((t /= 1f) == 1f)
            return 1f;

        float p = 1f * .3f;
        float s = p / 4f;

        return (1f * Mathf.Pow(2f, -10f * t) * Mathf.Sin((t * 1f - s) * (2f * Mathf.PI) / p) + 1f + 0f);
    }

    public static float ElasticEaseIn(float t)
    {
        if ((t /= 1f) == 1f)
            return 1f;

        float p = 1f * .3f;
        float s = p / 4f;

        return -(1f * Mathf.Pow(2f, 10f * (t -= 1f)) * Mathf.Sin((t * 1f - s) * (2f * Mathf.PI) / p));
    }

    public static float ElasticEaseInOut(float t)
    {
        if ((t /= 0.5f) == 2f)
            return 1f;

        float p = 1f * (.3f * 1.5f);
        float s = p / 4f;

        if (t < 1f)
            return -.5f * (1f * Mathf.Pow(2f, 10f * (t -= 1f)) * Mathf.Sin((t * 1f - s) * (2f * Mathf.PI) / p));
        return 1f * Mathf.Pow(2f, -10f * (t -= 1f)) * Mathf.Sin((t * 1f - s) * (2f * Mathf.PI) / p) * .5f + 1f;
    }

    public static float ElasticEaseOutIn(float t)
    {
        if (t < 0.5f)
            return ElasticEaseOut(t * 2f) * 0.5f;
        return ElasticEaseIn((t - 0.5f) * 2f) * 0.5f + 0.5f;
    }

    public static float BounceEaseOut(float t)
    {
        if ((t /= 1f) < (1f / 2.75f))
            return 1f * (7.5625f * t * t);
        else if (t < (2f / 2.75f))
            return 1f * (7.5625f * (t -= (1.5f / 2.75f)) * t + .75f);
        else if (t < (2.5f / 2.75f))
            return 1f * (7.5625f * (t -= (2.25f / 2.75f)) * t + .9375f);
        else
            return 1f * (7.5625f * (t -= (2.625f / 2.75f)) * t + .984375f);
    }

    public static float BounceEaseIn(float t)
    {
        return 1f - BounceEaseOut(1f - t);
    }

    public static float BounceEaseInOut(float t)
    {
        if (t < 0.5f)
            return BounceEaseIn(t * 2f) * 0.5f;
        else
            return ((t - 0.5f) * 2f) * 0.5f + 0.5f;
    }

    public static float BounceEaseOutIn(float t)
    {
        if (t < 0.5f)
            return BounceEaseOut(t * 2f) * 0.5f;
        return ((t - 0.5f) * 2f) * 0.5f + 0.5f;
    }

    public static float BackEaseOut(float t)
    {
        return 1f * ((t = t - 1f) * t * ((1.70158f + 1f) * t + 1.70158f) + 1f);
    }

    public static float BackEaseIn(float t)
    {
        return 1f * (t /= 1f) * t * ((1.70158f + 1f) * t - 1.70158f);
    }

    public static float BackEaseInOut(float t)
    {
        float s = 1.70158f;
        if ((t /= 0.5f) < 1f)
            return 0.5f * (t * t * (((s *= (1.525f)) + 1f) * t - s));
        return 0.5f * ((t -= 2) * t * (((s *= (1.525f)) + 1f) * t + s) + 2f);
    }

    public static float BackEaseOutIn(float t)
    {
        if (t < 0.5f)
            return (t * 2f) * 0.5f;
        return BackEaseIn((t - 0.5f) * 2f) * 0.5f + 0.5f;
    }

    #endregion


}


/// <summary>
/// NightTween parameters.
/// </summary>
public class NightTweenParams
{
    /// <summary>
    /// Constructor. You can also use NightTweenParams.New for the same effect.
    /// </summary>
    public NightTweenParams()
    {
        storedParams = new StoredParams(0);
    }

    /// <summary>
    /// Simple tween callback.
    /// </summary>
    public delegate void TweenCallback();
    /// <summary>
    /// Parameters stored.
    /// </summary>
    public StoredParams storedParams;
    /// <summary>
    /// Unique ID number counter.
    /// </summary>
    private static int numberCount = 0;

    /// <summary>
    /// Returns a new NightTweenParams. Same as using the constructor.
    /// </summary>
    public static NightTweenParams New
    {
        get
        {
            return new NightTweenParams();
        }
    }

    /// <summary>
    /// Returns a unique ID.
    /// </summary>
    private static string NextNumber()
    {
        numberCount++;
        return numberCount.ToString();
    }

    /// <summary>
    /// Used to add a delay before starting the tween.
    /// </summary>
    /// <param name="p_delay">Delay in seconds.</param>
    public NightTweenParams Delay(float p_delay)
    {
        storedParams.delay = p_delay;
        return this;
    }
    /// <summary>
    /// Easing function to use.
    /// </summary>
    /// <param name="p_easeType">NightTween.EaseType.</param>
    public NightTweenParams Ease(NightTween.EaseType p_easeType)
    {
        storedParams.easeType = p_easeType;
        return this;
    }
    /// <summary>
    /// Custom ID for identification.
    /// </summary>
    /// <param name="p_id">Unique name.</param>
    public NightTweenParams Id(string p_id)
    {
        storedParams.id = "NightTween Object " + p_id;
        return this;
    }
    /// <summary>
    /// Custom ID for identification.
    /// </summary>
    /// <param name="p_intId">Unique number.</param>
    public NightTweenParams Id(int p_intId)
    {
        storedParams.id = "NightTween Object " + p_intId.ToString();
        return this;
    }
    /// <summary>
    /// Number of loops.
    /// </summary>
    /// <param name="p_loops">Default is 0 (no loop). Negative numbers equals int.MaxValue.</param>
    public NightTweenParams Loops(int p_loops)
    {
        storedParams.loops = p_loops < 0 ? int.MaxValue : p_loops;
        return this;
    }
    /// <summary>
    /// Number of loops.
    /// </summary>
    /// <param name="p_loops">Default is 0 (no loop). Negative numbers equals int.MaxValue.</param>
    /// <param name="p_loopType">NightTween.LoopType to be used.</param>
    public NightTweenParams Loops(int p_loops, NightTween.LoopType p_loopType)
    {
        storedParams.loops = p_loops < 0 ? int.MaxValue : p_loops;
        storedParams.loopType = p_loopType;
        return this;
    }
    /// <summary>
    /// Callback to be called on the end of each cycle.
    /// </summary>
    /// <param name="p_function">Callback method.</param>
    public NightTweenParams OnEndCycle(TweenCallback p_function)
    {
        if (storedParams.endCycleCallback == null)
            storedParams.endCycleCallback = new TweenCallback[1];
        else
        {
            TweenCallback[] newCallbacks = new TweenCallback[storedParams.endCycleCallback.Length + 1];
            for (int x = 0; x < storedParams.endCycleCallback.Length; x++)
            {
                newCallbacks[x] = storedParams.endCycleCallback[x];
            }
            storedParams.endCycleCallback = newCallbacks;
        }
        storedParams.endCycleCallback[storedParams.endCycleCallback.Length - 1] = p_function;
        return this;
    }
    /// <summary>
    /// Callback to be called on start.
    /// </summary>
    /// <param name="p_function">Callback method.</param>
    public NightTweenParams OnStart(TweenCallback p_function)
    {
        if (storedParams.startCallback == null)
            storedParams.startCallback = new TweenCallback[1];
        else
        {
            TweenCallback[] newCallbacks = new TweenCallback[storedParams.startCallback.Length + 1];
            for (int x = 0; x < storedParams.startCallback.Length; x++)
            {
                newCallbacks[x] = storedParams.startCallback[x];
            }
            storedParams.startCallback = newCallbacks;
        }
        storedParams.startCallback[storedParams.startCallback.Length - 1] = p_function;
        return this;
    }
    /// <summary>
    /// Callback to be called on update.
    /// </summary>
    /// <param name="p_function">Callback method.</param>
    public NightTweenParams OnUpdate(TweenCallback p_function)
    {
        if (storedParams.updateCallback == null)
            storedParams.updateCallback = new TweenCallback[1];
        else
        {
            TweenCallback[] newCallbacks = new TweenCallback[storedParams.updateCallback.Length + 1];
            for (int x = 0; x < storedParams.updateCallback.Length; x++)
            {
                newCallbacks[x] = storedParams.updateCallback[x];
            }
            storedParams.updateCallback = newCallbacks;
        }
        storedParams.updateCallback[storedParams.updateCallback.Length - 1] = p_function;
        return this;
    }
    /// <summary>
    /// Callback to be called on finish.
    /// </summary>
    /// <param name="p_function">Callback method.</param>
    public NightTweenParams OnFinish(TweenCallback p_function)
    {
        if (storedParams.finishCallback == null)
            storedParams.finishCallback = new TweenCallback[1];
        else
        {
            TweenCallback[] newCallbacks = new TweenCallback[storedParams.finishCallback.Length + 1];
            for (int x = 0; x < storedParams.finishCallback.Length; x++)
            {
                newCallbacks[x] = storedParams.finishCallback[x];
            }
            storedParams.finishCallback = newCallbacks;
        }
        storedParams.finishCallback[storedParams.finishCallback.Length - 1] = p_function;
        return this;
    }
    /// <summary>
    /// Property to be changed. You can use more than one property by calling this again.
    /// </summary>
    /// <param name="p_propName">Property type.</param>
    /// <param name="p_endVal">Property value.</param>
    public NightTweenParams Property(NTPropType p_propName, object p_endVal)
    {
        if (storedParams.property == null)
        {
            storedParams.property = new NTPropType[1];
        }
        else
        {
            NTPropType[] newParams = new NTPropType[storedParams.property.Length + 1];
            for (int x = 0; x < storedParams.property.Length; x++)
            {
                newParams[x] = storedParams.property[x];
            }
            storedParams.property = newParams;
        }
        storedParams.property[storedParams.property.Length - 1] = p_propName;

        if (storedParams.propertyValue == null)
            storedParams.propertyValue = new object[1];
        else
        {
            object[] newValues = new object[storedParams.propertyValue.Length + 1];
            for (int x = 0; x < storedParams.propertyValue.Length; x++)
            {
                newValues[x] = storedParams.propertyValue[x];
            }
            storedParams.propertyValue = newValues;
        }
        storedParams.propertyValue[storedParams.propertyValue.Length - 1] = p_endVal;

        return this;
    }
    /// <summary>
    /// Update mode used.
    /// </summary>
    /// <param name="p_updateType">NightTween.UpdateType. Default is NightTween.UpdateType.Update.</param>
    public NightTweenParams UpdateMode(NightTween.UpdateType p_updateType)
    {
        storedParams.updateMode = p_updateType;
        return this;
    }

    /// <summary>
    /// Stored parameters.
    /// </summary>
    [System.Serializable]
    public struct StoredParams
    {
        public float delay;
        public NightTween.EaseType easeType;
        public string id;
        public int loops;
        public NightTween.LoopType loopType;
        public TweenCallback[] endCycleCallback;
        public object[] endCycleCallbackPParam;
        public TweenCallback[] startCallback;
        public object[] startCallbackPParam;
        public TweenCallback[] finishCallback;
        public object[] finishCallbackPParam;
        public TweenCallback[] updateCallback;
        public object[] updateCallbackPParam;
        public NTPropType[] property;
        public object[] propertyValue;
        public NightTween.UpdateType updateMode;

        /// <summary>
        /// Create a new StoredParams with default values;
        /// </summary>
        /// <param name="mode">Any value.</param>
        public StoredParams(int mode)
        {
            delay = 0;
            easeType = NightTween.EaseType.Linear;
            id = "NightTween " + NightTweenParams.NextNumber();
            loops = 0;
            loopType = NightTween.LoopType.Restart;
            endCycleCallback = null;
            endCycleCallbackPParam = null;
            startCallback = null;
            startCallbackPParam = null;
            finishCallback = null;
            finishCallbackPParam = null;
            updateCallback = null;
            updateCallbackPParam = null;
            property = null;
            propertyValue = null;
            updateMode = NightTween.UpdateType.Update;
        }
    }
}

