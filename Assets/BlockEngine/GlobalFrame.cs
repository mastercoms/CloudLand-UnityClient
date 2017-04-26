using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GlobalFrame : MonoBehaviour {

    public static int Frame
    {
        get
        {
            return _globalFrame;
        }
    }

    private static int _globalFrame;
    private static float _frameTime;
	
	// Update is called once per frame
	void Update () {
        _frameTime += Time.deltaTime;
        if(_frameTime > 1.2f)
        {
            _frameTime = 0f;

            _globalFrame++;
            if(_globalFrame > 15)
            {
                _globalFrame = 0;
            }
        }
	}
}
