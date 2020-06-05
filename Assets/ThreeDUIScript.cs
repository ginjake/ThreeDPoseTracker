using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ThreeDUIScript : MonoBehaviour
{
    public UIScript uiScript;
    public PowerPointViewer powerPointViewer;

    public GameObject Input;

    public enum EventType
    {
        Next,
        Prev,
        Avator,
        Input,
    }
    public EventType eventType = EventType.Next;

    private Vector3 defaultPosition;
    private bool isTrigger = false;
    private bool isMoving = false;
    private float startT = 0f;
    private Vector3 _velocity = new Vector3(0, 2, 0);
    private Vector3 defaultInputPosition;
    private bool defaultInputPositionF = false;
    // Start is called before the first frame update
    void Start()
    {
        defaultPosition = this.transform.position;
        defaultInputPosition = Input.transform.position;
    }

    // Update is called once per frame
    void Update()
    {
        if(isTrigger)
        {
            if(!isMoving)
            {
                isMoving = true;
                startT = 0f;

                switch(eventType)
                {
                    case EventType.Next:
                        powerPointViewer.NextPage();
                        break;
                    case EventType.Prev:
                        powerPointViewer.PrevPage();
                        break;
                    case EventType.Input:
                        defaultInputPositionF =! defaultInputPositionF;
                        if (defaultInputPositionF)
                            Input.transform.position = new Vector3(-0.5f, 0.5f, -0.5f);
                        else
                            Input.transform.position = defaultInputPosition;

                        break;
                }
            }

            startT = startT + Time.deltaTime;
            if (startT < 0.15f)
            {
                transform.position = transform.position - (_velocity * Time.deltaTime);
            }
            else if (startT < 0.3f)
            {
                transform.position = transform.position + (_velocity * Time.deltaTime);
            }
            else if (startT < 1.0f)
            {
                transform.position = defaultPosition;
            }
            else
            { 
                isTrigger = false;
                isMoving = false;
            }

        }
    }

    // 重なり瞬間判定
    void OnTriggerEnter(Collider other)
    {
        Debug.Log(other.name + "Enter");
        if (!isTrigger)
        {
            isTrigger = true;
            isMoving = false;
        }
    }

    // 重なり中の判定
    void OnTriggerStay(Collider other)
    {
    }

    // 重なり離脱の判定
    void OnTriggerExit(Collider other)
    {
    }
}
