using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityRawInput;

public class PanelDown : MonoBehaviour
{
    public bool WorkInBackground;
    public bool InterceptMessages;

    public Material[] _material;           // 割り当てるマテリアル.

    // Start is called before the first frame update
    void Start()
    {
        this.GetComponent<Renderer>().sharedMaterial = _material[0];
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKey(KeyCode.Space))
        {
            transform.position = new Vector3(0f, transform.position.y + 0.1f, 0f);
        }
    }

    private void OnEnable()
    {
        RawKeyInput.Start(WorkInBackground);
        RawKeyInput.OnKeyUp += LogKeyUp;
        RawKeyInput.OnKeyDown += LogKeyDown;

    }

    private void OnDisable()
    {
        RawKeyInput.Stop();
        RawKeyInput.OnKeyUp -= LogKeyUp;
        RawKeyInput.OnKeyDown -= LogKeyDown;
    }

    private void OnValidate()
    {
        RawKeyInput.InterceptMessages = InterceptMessages;
    }

    private void LogKeyUp(RawKey key)
    {
        if (key == RawKey.Down)
        {
            this.GetComponent<Renderer>().sharedMaterial = _material[0];
        }
    }

    private void LogKeyDown(RawKey key)
    {
        if (RawKeyInput.IsKeyDown(RawKey.Down))
        {
            this.GetComponent<Renderer>().sharedMaterial = _material[1];
        }
    }

}


