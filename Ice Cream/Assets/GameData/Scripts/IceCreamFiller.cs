using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IceCreamFiller : MonoBehaviour
{

    #region Public

    [Header("IceCream Material")]
    public GameObject iceCreamMachine;
    public GameObject[] IceCreamBars;
    public Transform centre;
    public Material[] flavors;

    public Transform iceCreamOutPoint;
    public Transform finalPoint;


    #endregion

    #region Private

    #region IceCream Filling Data

    bool fillingRecord;
    bool FirstfillingDone;
    Material currentMaterial;
    Animator animator;

    #endregion

    #region Rotate IceCream Machine

    float timeCounter;
    public float speed;
    public float rotateX;
    public float rotateZ;

    #endregion

    #region IceCreamScoops

    GameObject cylinder;
    class IceCreamsClass
    {
        public GameObject iceCreamScoop;
        public float increment;
        public Vector3 destinationPosition;
        public Vector3 destinationRotation;
    }
    
    List<IceCreamsClass> listOfIceCreamsClass = new List<IceCreamsClass>();

    int lastIndex;

    #endregion

    #endregion


    #region MonoBehaviour CallBacks

    // Start is called before the first frame update
    void Start()
    {
        animator = GetComponent<Animator>();
        timeCounter = 0;
    }

    #endregion

    IEnumerator Fill()
    {
        yield return new WaitForEndOfFrame();
        if (fillingRecord)
        {
            timeCounter += Time.deltaTime * speed;
            float x = Mathf.Sin(timeCounter) * rotateX;
            float y = 0;
            float z = Mathf.Cos(timeCounter) * rotateZ;
            iceCreamMachine.transform.position = new Vector3(x, y, z);

            finalPoint.transform.RotateAround(centre.position, new Vector3(0, 1, 0),1);

            // Runtime IceCream Generator

            cylinder = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
            Destroy(cylinder.GetComponent<CapsuleCollider>());
            cylinder.transform.position = iceCreamOutPoint.position;
            cylinder.transform.localScale = new Vector3(0.4f, 0.05f, 0.4f);
            cylinder.gameObject.GetComponent<MeshRenderer>().material = currentMaterial;
            
            
            // Adding Runtime IceCream into List
            
            IceCreamsClass data = new IceCreamsClass();
            data.iceCreamScoop = cylinder;
            data.increment = 0;
            data.destinationPosition = finalPoint.position;
            data.destinationRotation = finalPoint.eulerAngles;
            listOfIceCreamsClass.Add(data);

            // Lerping the IceCream value until it reach at cone

            for (int i = 0; i < listOfIceCreamsClass.Count; i ++)
            {
                if (listOfIceCreamsClass[i].increment <= 1)
                {
                    listOfIceCreamsClass[i].iceCreamScoop.transform.position = Vector3.Lerp(iceCreamOutPoint.position, listOfIceCreamsClass[i].destinationPosition, listOfIceCreamsClass[i].increment);
                    listOfIceCreamsClass[i].iceCreamScoop.transform.eulerAngles = Vector3.Lerp(iceCreamOutPoint.eulerAngles, listOfIceCreamsClass[i].destinationRotation, listOfIceCreamsClass[i].increment);
                    listOfIceCreamsClass[i].increment += 0.01f;
                }
            }
            Vector3 tempVector3 = finalPoint.transform.position;
            tempVector3.y += 0.001f;
            finalPoint.transform.position = tempVector3;
            StartCoroutine(Fill());
        }
    }

    // Continue lerping after the button is not pressing

    IEnumerator CompleteIceCreams()
    {
        yield return new WaitForEndOfFrame();
        for (int i = 0; i < listOfIceCreamsClass.Count; i++)
        {
            if (listOfIceCreamsClass[i].increment <= 1)
            {
                listOfIceCreamsClass[i].iceCreamScoop.transform.position = Vector3.Lerp(iceCreamOutPoint.position, listOfIceCreamsClass[i].destinationPosition, listOfIceCreamsClass[i].increment);
                listOfIceCreamsClass[i].iceCreamScoop.transform.eulerAngles = Vector3.Lerp(iceCreamOutPoint.eulerAngles, listOfIceCreamsClass[i].destinationRotation, listOfIceCreamsClass[i].increment);
                listOfIceCreamsClass[i].increment += 0.01f;
            }
        }
        if(fillingRecord)
        {
            StartCoroutine(Fill());
        }
        else if (FirstfillingDone)
        {
            StartCoroutine(CompleteIceCreams());
        }
    }

    // on button down

    public void StartFill(int buttonNumber)
    {
        fillingRecord = true;
        StartCoroutine(Fill());
        animator.SetBool("Push" + buttonNumber, true);
        currentMaterial = flavors[buttonNumber];
    }

    // on button up

    public void StopFill(int buttonNumber)
    {
        fillingRecord = false;
        FirstfillingDone = true;
        animator.SetBool("Push" + buttonNumber, false);
        StartCoroutine(CompleteIceCreams());
    }



}
