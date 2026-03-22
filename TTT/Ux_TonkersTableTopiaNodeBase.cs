using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(RectTransform))]
public abstract class Ux_TonkersTableTopiaNodeBase : MonoBehaviour
{
    protected RectTransform _cachedRect;
    protected Ux_TonkersTableTopiaLayout _cachedTable;
    protected Image _cachedImage;

    protected virtual void Awake()
    {
        CacheRectTransform();
        CacheImage();
    }

    protected virtual void OnTransformParentChanged()
    {
        _cachedTable = null;
    }

    protected RectTransform CacheRectTransform()
    {
        if (_cachedRect == null)
        {
            _cachedRect = GetComponent<RectTransform>();
        }

        return _cachedRect;
    }

    protected void CacheImage()
    {
        RectTransform rect = CacheRectTransform();
        if (rect == null || _cachedImage != null)
        {
            return;
        }

        rect.TryGetComponent(out _cachedImage);
    }

    protected RectTransform GetCachedRectTransform()
    {
        return CacheRectTransform();
    }

    protected Ux_TonkersTableTopiaLayout GetCachedTable()
    {
        if (_cachedTable == null)
        {
            _cachedTable = GetComponentInParent<Ux_TonkersTableTopiaLayout>(true);
        }

        return _cachedTable;
    }

    internal void SetCachedBackgroundVisual(bool needImage, Sprite sprite, Color tint, bool useSliced)
    {
        RectTransform rect = CacheRectTransform();
        if (rect == null)
        {
            return;
        }

        CacheImage();

        if (!needImage)
        {
            if (_cachedImage != null)
            {
                _cachedImage.enabled = false;
                _cachedImage.sprite = null;
                _cachedImage.color = Color.white;
                _cachedImage.raycastTarget = false;
            }

            return;
        }

        if (_cachedImage == null)
        {
            _cachedImage = Ux_TonkersTableTopiaObjectUtility.GetOrAddComponent<Image>(rect.gameObject);
        }

        if (_cachedImage == null)
        {
            return;
        }

        _cachedImage.enabled = true;
        _cachedImage.sprite = sprite;
        _cachedImage.color = tint;
        _cachedImage.type = sprite != null && useSliced ? Image.Type.Sliced : Image.Type.Simple;
        _cachedImage.raycastTarget = false;
    }
}