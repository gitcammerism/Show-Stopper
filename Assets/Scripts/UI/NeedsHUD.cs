using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.UIElements;
using UnityEngine.UIElements.Experimental;

// This script handles the animal needs HUD logic for when the radial fill elements are clicked by the player

public class NeedsHUD : MonoBehaviour
{
    // animal setup for script
    private AnimalData animalData;
    private AnimalManager.AnimalType animal;
    public AnimalManager.AnimalType Animal => animal;

    // custom struct holding each ring's data
    public struct RingData
    {
        public RadialFillElement element;
        public float current;
        public float max;
    }

    // variables for HUD/UI logic
    private UIDocument document;
    private readonly Dictionary<TileType, RingData> _rings = new();
    private Label _valueLabel;
    TileType _selected = TileType.None;


    // on enable, loop through each ring and have them subscribe to click event
    private void OnEnable()
    {
        // ensures null ref can't get through
        if (animal == AnimalManager.AnimalType.none) return;

        // get corresponding animal's current need values
        animalData = DataPersistenceManager.instance.GetAnimalData(animal);

        // get document ref from parent gameobject 
        document = GetComponentInParent<UIDocument>();
        if (document == null) return;

        var root = document.rootVisualElement;

        _valueLabel = root.Q<Label>("value-label");
        _valueLabel.style.display = DisplayStyle.None;

        // finds every ringle in UXML document
        root.Query<RadialFillElement>().ForEach(ring =>
        {
            _rings[ring.TileType] = new RingData
            {
                element = ring,
                current = ring.TileType switch
                {
                    TileType.Comfort => animalData.comfort,
                    TileType.Food => animalData.food,
                    TileType.Toy => animalData.toys,
                    TileType.Treat => animalData.treats,
                    TileType.Water => animalData.water,
                    _ => ring.Fill * 100f // default if no values match
                },
                max = 100f
            };
            ring.RegisterCallback<ClickEvent>(OnRingClicked);
        });
    }

    // unsubscribe all rings from event and clear dictionary
    private void OnDisable()
    {
        foreach (var data in _rings.Values)
        {
            data.element.UnregisterCallback<ClickEvent>(OnRingClicked);
        }
        _rings.Clear();
    }

    // event that triggers on clicking each ring to show, hide, and get tile values
    private void OnRingClicked(ClickEvent evt)
    {
        var ring = (RadialFillElement)evt.currentTarget;

        // clicking the ring again hides the label
        if (_selected == ring.TileType && _valueLabel.style.display == DisplayStyle.Flex)
        {
            _valueLabel.style.display = DisplayStyle.None;
            return;
        }

        _selected = ring.TileType;
        RefreshLabel();
        _valueLabel.style.display = DisplayStyle.Flex;
    }

    // changes label text
    private void RefreshLabel()
    {
        if (!_rings.TryGetValue(_selected, out var data)) return;
        _valueLabel.text = $"{_selected}: {data.current:0} / {data.max:0}";
    }

    // method that lets you set the animal when enabled
    public void SetAnimal(AnimalManager.AnimalType animalClicked) => animal = animalClicked;

    // public method that lets you change a given ring value
    public void SetValue(TileType type, float current, float max)
    {
        if (!_rings.TryGetValue(type, out var data)) return;

        data.current = current;
        data.max = max;
        _rings[type] = data;

        // sets fill of ring based on amount
        data.element.Fill = max > 0f ? current / max : 0f;

        // if the given label is open, update values
        if (type == _selected) RefreshLabel();
    }
}
