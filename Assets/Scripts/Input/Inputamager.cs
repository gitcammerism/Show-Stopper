using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Inputamager : MonoBehaviour
{
	private TouchControls _touchControls;
    // Singleton instance
    public static Inputamager instance;

	private void Awake()
	{
		_touchControls = new TouchControls();

        // check if instance exists
        if(instance != null && instance != this)
        {
            Destroy(gameObject);
            return;
        }
        instance = this;
        DontDestroyOnLoad(gameObject);
	}

    private void OnEnable()
    {
        _touchControls.Enable();
    }

    private void OnDisable()
    {
        _touchControls?.Disable();
    }

    private void Start()
    {
        
    }

}
