using System;
using UnityEngine;
using UnityEngine.UIElements;

[UxmlElement]
public partial class RadialFillElement : VisualElement
{
    // scriptable object holding all tile visual info
    static TileTypeVisuals s_visuals;

    // variables used to create the visual element
    private readonly VisualElement _icon;
    private TileType _tileType;
    private float _fill = 1f;
    private float _lineWidth = 10f;
    private Color _trackColor = new Color(1, 1, 1, 0.15f);
    private Color _fillColor = Color.white;

    [UxmlAttribute("tile-type")]
    public TileType TileType
    {
        get => _tileType;
        set { _tileType = value; ApplyTileType(); }
    }

    [UxmlAttribute("fill")]
    public float Fill
    {
        get => _fill;
        set { _fill = Mathf.Clamp01(value); MarkDirtyRepaint(); }
    }

    [UxmlAttribute("line-width")]
    public float LineWidth
    {
        get => _lineWidth;
        set { _lineWidth = value; MarkDirtyRepaint(); }
    }

    [UxmlAttribute("track-color")]
    public Color TrackColor
    {
        get => _trackColor;
        set { _trackColor = value; MarkDirtyRepaint(); }
    }

    // constructor that initializes element
    public RadialFillElement()
    {
        // center align & justify
        style.alignItems = Align.Center;
        style.justifyContent = Justify.Center;

        // default size
        style.width = 100;
        style.height = 100;

        // creates element icon
        _icon = new VisualElement { pickingMode = PickingMode.Ignore };
        _icon.style.width = Length.Percent(50);
        _icon.style.height = Length.Percent(50);
        _icon.style.backgroundSize = new BackgroundSize(BackgroundSizeType.Contain);
        _icon.style.backgroundPositionX = new BackgroundPosition(BackgroundPositionKeyword.Center);
        _icon.style.backgroundPositionY = new BackgroundPosition(BackgroundPositionKeyword.Center);
        _icon.style.backgroundRepeat = new BackgroundRepeat(Repeat.NoRepeat, Repeat.NoRepeat);
        Add(_icon);

        // subscribe to event & apply tile visuals
        generateVisualContent += OnGenerateVisualContent;
        ApplyTileType();
    }

    // applies tile visuals to element
    private void ApplyTileType()
    {
        // if null, find scriptable object
        if (s_visuals == null)
        {
            s_visuals = Resources.Load<TileTypeVisuals>("TileTypeVisuals");
        }

        // if not null and you can get the given tile type
        if (s_visuals != null && s_visuals.TryGet(_tileType, out var entry))
        {
            // changes color & icon to entry color and icon (or no icon if none is set)
            _fillColor = entry.color;
            _icon.style.backgroundImage = entry.icon != null
                ? new StyleBackground(entry.icon)
                : new StyleBackground(StyleKeyword.None);
        }
        else
        {
            // if it doesn't exist, default values
            _fillColor = Color.white;
            _icon.style.backgroundImage = new StyleBackground(StyleKeyword.None);
        }

        // regenerate visuals after changes
        MarkDirtyRepaint();
    }

    // creates visual for element
    private void OnGenerateVisualContent(MeshGenerationContext ctx)
    {
        var rect = contentRect;
        float size = Mathf.Min(rect.width, rect.height);
        if (size <= 0f || _lineWidth <= 0f) return;

        var center = rect.center;
        float radius = (size - _lineWidth) * 0.5f;
        if (radius <= 0f) return;

        // create painter/mesh generator
        var p = ctx.painter2D;
        p.lineWidth = _lineWidth;
        p.lineCap = LineCap.Round;

        // background track (circle outline)
        p.strokeColor = _trackColor;
        p.BeginPath();
        p.Arc(center, radius, 0f, 360f);
        p.Stroke();

        // fill arc for element
        if (_fill > 0f)
        {
            p.strokeColor = _fillColor;
            p.BeginPath();
            p.Arc(center, radius, -90f, -90f + 360f * _fill);
            p.Stroke();
        }
    }
}
