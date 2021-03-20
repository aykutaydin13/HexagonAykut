using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;

public class EdgeController : MonoBehaviour
{
    [SerializeField] private GameObject Edge;
    [SerializeField] private GameObject HexParent;
    [SerializeField] private GameObject HexPrefab;
    [SerializeField] private GameObject ClockWiseRotButton;
    [SerializeField] private GameObject CounterClockWiseRotButton;
    [SerializeField] private GameObject Bomb;
    [SerializeField] private GameObject BombCounterText;
    private HexGridController hexGridController;
    private int boardHeight;
    private int boardWidth;

    //İki komşu Hexagon arasındaki x ve y değerlerinin farkları
    private float HexXOffset;
    private float HexYOffset;

    private List<GameObject> HexList = new List<GameObject>();
    float firstZAngle = 0;
    float zAngle = 0;
    bool isRotateFinished = true;
    bool isRotateStarted = false;
    bool isClockWiseRotate = false;
    bool isCounterClockWiseRotate = false;
    Camera mainCam;

    private bool isFirstNeighborExist;
    private bool isSecondNeighborExist;
    private bool isThirdNeighborExist;
    private bool isFourthNeighborExist;
    private bool isFifthNeighborExist;
    private bool isSixthNeighborExist;

    private bool anyMatches = true;
    private bool anyMatchesForRotatingHexes = false;
    private bool shouldFillBoard = true;
    private bool isBombActive = false;
    private bool isGameOver = false;

    private float xOffset = 0.9f / Mathf.Sqrt(3);
    private float yOffset = 0.9f;

    private Vector3 currentHexPos;
    private GameObject tempHex;
    private GameObject currentHex;

    Vector3 mousePosition;

    private Dictionary<int, GameObject> NeigborHexes = new Dictionary<int, GameObject>();
    private Dictionary<int, GameObject> Selected3Hexes = new Dictionary<int, GameObject>();

    //Hexleri destroy ettikten sonra hangi coulmn'da kaç tane hex kaldığını tutmak için
    private Dictionary<int, int> ColumnAndElementCounts = new Dictionary<int, int>();


    // Start is called before the first frame update
    private void Start()
    {
        mainCam = Camera.main;
        hexGridController = GetComponent<HexGridController>();
        boardHeight = hexGridController.Height;
        boardWidth = hexGridController.Width;
        HexXOffset = Mathf.Sqrt(3) * 0.9f;
        HexYOffset = -1.8f;
        Edge.SetActive(false);
        Bomb.SetActive(false);
        BombCounterText.SetActive(false);
    }

    void Update()
    {
        ///Oyun Bittiğinde Sahne Değiştir.
        if (isGameOver)
        {
            SceneManager.LoadScene(1);
        }

        //Ekrana tıklandığında Edge' i aktifleştirip konumunu belirle
        if (Input.GetMouseButtonDown(0))
        {
            Edge.gameObject.SetActive(true);

            RaycastHit hit;
            Ray ray = mainCam.ScreenPointToRay(Input.mousePosition);

            if (Physics.Raycast(ray, out hit))
            {
                //Debug.Log(hit.collider.name);
                mousePosition = hit.point;

                CheckNeighbors(hit.transform.gameObject);

                currentHex = hit.transform.gameObject;

                tempHex = currentHex;
                currentHexPos = currentHex.transform.position;

                ChangeEdgePosition(mousePosition);
            }
        }

        //Edge aktif değilse rotate oklarını gizle
        if (Edge.activeSelf)
        {
            ClockWiseRotButton.SetActive(true);
            CounterClockWiseRotButton.SetActive(true);
        }
        else
        {
            ClockWiseRotButton.SetActive(false);
            CounterClockWiseRotButton.SetActive(false);
        }


        //Bomb Counter ı kontrol et 
        if (ScoreScript.scoreValue % 200 <5 && ScoreScript.scoreValue > 5)
        {
            isBombActive = true;
        }
        if (!isBombActive)
        {
            BombCounterSprite.bombCounter = 7;
        }
        if (BombCounterSprite.bombCounter == 0)
        {
            isGameOver = true;
        }

    }

    private void CheckNeighbors(GameObject currentHex)
    {
        NeigborHexes.Clear();

        isFirstNeighborExist = false;
        isSecondNeighborExist = false;
        isThirdNeighborExist = false;
        isFourthNeighborExist = false;
        isFifthNeighborExist = false;
        isSixthNeighborExist = false;

        RaycastHit hit;
        Vector3 direction1 = Vector3.up;
        Vector3 direction2 = (Vector3.right + Vector3.up).normalized;
        Vector3 direction3 = (Vector3.right - Vector3.up).normalized;
        Vector3 direction4 = -Vector3.up;
        Vector3 direction5 = (-Vector3.right - Vector3.up).normalized;
        Vector3 direction6 = (-Vector3.right + Vector3.up).normalized;
        Ray ray1 = new Ray(currentHex.transform.position, direction1);
        Ray ray2 = new Ray(currentHex.transform.position, direction2);
        Ray ray3 = new Ray(currentHex.transform.position, direction3);
        Ray ray4 = new Ray(currentHex.transform.position, direction4);
        Ray ray5 = new Ray(currentHex.transform.position, direction5);
        Ray ray6 = new Ray(currentHex.transform.position, direction6);

        if (Physics.Raycast(ray1, out hit))
        {
            //Debug.Log("First Neighbor: " + hit.collider.name);
            isFirstNeighborExist = true;
            NeigborHexes.Add(1, hit.transform.gameObject);
        }
        if (Physics.Raycast(ray2, out hit))
        {
            //Debug.Log("Second Neighbor: " + hit.collider.name);
            isSecondNeighborExist = true;
            NeigborHexes.Add(2, hit.transform.gameObject);
        }
        if (Physics.Raycast(ray3, out hit))
        {
            //Debug.Log("Third Neighbor: " + hit.collider.name);
            isThirdNeighborExist = true;
            NeigborHexes.Add(3, hit.transform.gameObject);
        }
        if (Physics.Raycast(ray4, out hit))
        {
            //Debug.Log("Fourth Neighbor: " + hit.collider.name);
            isFourthNeighborExist = true;
            NeigborHexes.Add(4, hit.transform.gameObject);
        }
        if (Physics.Raycast(ray5, out hit))
        {
            //Debug.Log("Fifth Neighbor: " + hit.collider.name);
            isFifthNeighborExist = true;
            NeigborHexes.Add(5, hit.transform.gameObject);
        }
        if (Physics.Raycast(ray6, out hit))
        {
            //Debug.Log("Sixth Neighbor: " + hit.collider.name);
            isSixthNeighborExist = true;
            NeigborHexes.Add(6, hit.transform.gameObject);
        }

    }

    private void ChangeEdgePosition(Vector3 mousePos)
    {
        Selected3Hexes.Clear();
        Selected3Hexes.Add(1, currentHex);
        Vector3 firstCornerPos = new Vector3(currentHexPos.x + xOffset, currentHexPos.y + yOffset, 0);
        Vector3 secondCornerPos = new Vector3(currentHexPos.x + 2 * xOffset, currentHexPos.y, 0);
        Vector3 thirdCornerPos = new Vector3(currentHexPos.x + xOffset, currentHexPos.y - yOffset, 0);
        Vector3 fourthCornerPos = new Vector3(currentHexPos.x - xOffset, currentHexPos.y - yOffset, 0);
        Vector3 fifthCornerPos = new Vector3(currentHexPos.x - 2 * xOffset, currentHexPos.y, 0);
        Vector3 sixthCornerPos = new Vector3(currentHexPos.x - xOffset, currentHexPos.y + yOffset, 0);

        float firstCornerDistance = Mathf.Abs(Vector3.Distance(mousePos, firstCornerPos));
        float secondCornerDistance = Mathf.Abs(Vector3.Distance(mousePos, secondCornerPos));
        float thirdCornerDistance = Mathf.Abs(Vector3.Distance(mousePos, thirdCornerPos));
        float fourthCornerDistance = Mathf.Abs(Vector3.Distance(mousePos, fourthCornerPos));
        float fifthCornerDistance = Mathf.Abs(Vector3.Distance(mousePos, fifthCornerPos));
        float sixthCornerDistance = Mathf.Abs(Vector3.Distance(mousePos, sixthCornerPos));

        List<float> cornerDistanceList = new List<float>();

        if (isFirstNeighborExist && isSecondNeighborExist)
        {
            cornerDistanceList.Add(firstCornerDistance);
        }
        if (isSecondNeighborExist && isThirdNeighborExist)
        {
            cornerDistanceList.Add(secondCornerDistance);
        }
        if (isThirdNeighborExist && isFourthNeighborExist)
        {
            cornerDistanceList.Add(thirdCornerDistance);
        }
        if (isFourthNeighborExist && isFifthNeighborExist)
        {
            cornerDistanceList.Add(fourthCornerDistance);
        }
        if (isFifthNeighborExist && isSixthNeighborExist)
        {
            cornerDistanceList.Add(fifthCornerDistance);
        }
        if (isSixthNeighborExist && isFirstNeighborExist)
        {
            cornerDistanceList.Add(sixthCornerDistance);
        }

        float min = cornerDistanceList.Min();


        if (min == firstCornerDistance)
        {
            Edge.transform.position = firstCornerPos;
            Edge.transform.eulerAngles = new Vector3(0, 0, 0);
            Selected3Hexes.Add(2, NeigborHexes[1]);
            Selected3Hexes.Add(3, NeigborHexes[2]);
        }
        else
       if (min == secondCornerDistance)
        {
            Edge.transform.position = secondCornerPos;
            Edge.transform.eulerAngles = new Vector3(0, 0, 60);
            Selected3Hexes.Add(2, NeigborHexes[2]);
            Selected3Hexes.Add(3, NeigborHexes[3]);
        }
        else
       if (min == thirdCornerDistance)
        {
            Edge.transform.position = thirdCornerPos;
            Edge.transform.eulerAngles = new Vector3(0, 0, 0);
            Selected3Hexes.Add(2, NeigborHexes[3]);
            Selected3Hexes.Add(3, NeigborHexes[4]);
        }
        else
       if (min == fourthCornerDistance)
        {
            Edge.transform.position = fourthCornerPos;
            Edge.transform.eulerAngles = new Vector3(0, 0, 60);
            Selected3Hexes.Add(2, NeigborHexes[4]);
            Selected3Hexes.Add(3, NeigborHexes[5]);
        }
        if (min == fifthCornerDistance)
        {
            Edge.transform.position = fifthCornerPos;
            Edge.transform.eulerAngles = new Vector3(0, 0, 0);
            Selected3Hexes.Add(2, NeigborHexes[5]);
            Selected3Hexes.Add(3, NeigborHexes[6]);
        }
        else
        if (min == sixthCornerDistance)
        {
            Edge.transform.position = sixthCornerPos;
            Edge.transform.eulerAngles = new Vector3(0, 0, 60);
            Selected3Hexes.Add(2, NeigborHexes[6]);
            Selected3Hexes.Add(3, NeigborHexes[1]);
        }

    }

    private IEnumerator DestroyHexes()
    {
        List<Vector3> ShiftHexPositionList = new List<Vector3>();
        ShiftHexPositionList.Clear();
        Vector3 position;
        GameObject firstHex = Selected3Hexes[1];
        Color firstHexColor = firstHex.GetComponentInChildren<Renderer>().GetComponent<Renderer>().material.color;
        GameObject secondHex = Selected3Hexes[2];
        Color secondHexColor = secondHex.GetComponentInChildren<Renderer>().GetComponent<Renderer>().material.color;
        GameObject thirdHex = Selected3Hexes[3];
        Color thirdHexColor = thirdHex.GetComponentInChildren<Renderer>().GetComponent<Renderer>().material.color;

        CheckNeighbors(firstHex);

        for (int i = 1; i < 6; i++)
        {
            if (NeigborHexes.ContainsKey(i) && NeigborHexes.ContainsKey(i + 1))
            {
                if (firstHexColor == NeigborHexes[i].GetComponentInChildren<Renderer>().GetComponent<Renderer>().material.color && firstHexColor == NeigborHexes[i + 1].GetComponentInChildren<Renderer>().GetComponent<Renderer>().material.color)
                {
                    Edge.gameObject.SetActive(false);
                    GameObject[] DescendingOrderOfHexes = { firstHex, NeigborHexes[i], NeigborHexes[i + 1] };
                    DescendingOrderOfHexes = DescendingSortOfObjects(DescendingOrderOfHexes);

                    for (int k = 0; k < DescendingOrderOfHexes.Length; k++)
                    {
                        position = DescendingOrderOfHexes[k].transform.position;

                        Destroy(DescendingOrderOfHexes[k]);
                        anyMatchesForRotatingHexes = true;

                        ScoreScript.scoreValue += 5;

                        yield return new WaitForSeconds(0.01f);

                        ShiftHexPositionList.Add(position);
                        //yield return StartCoroutine(ShiftHexDown(position));
                    }

                    break;
                }
            }

        }
        if (NeigborHexes.ContainsKey(6) && NeigborHexes.ContainsKey(1) && firstHex != null)
        {
            if (firstHexColor == NeigborHexes[6].GetComponentInChildren<Renderer>().GetComponent<Renderer>().material.color && firstHexColor == NeigborHexes[1].GetComponentInChildren<Renderer>().GetComponent<Renderer>().material.color)
            {
                Edge.gameObject.SetActive(false);

                GameObject[] DescendingOrderOfHexes = { firstHex, NeigborHexes[6], NeigborHexes[1] };
                DescendingOrderOfHexes = DescendingSortOfObjects(DescendingOrderOfHexes);

                for (int k = 0; k < DescendingOrderOfHexes.Length; k++)
                {
                    position = DescendingOrderOfHexes[k].transform.position;

                    Destroy(DescendingOrderOfHexes[k]);
                    anyMatchesForRotatingHexes = true;
                    ScoreScript.scoreValue += 5;

                    yield return new WaitForSeconds(0.01f);
                    ShiftHexPositionList.Add(position);
                    //yield return StartCoroutine(ShiftHexDown(position));
                }

            }
        }

        if (secondHex != null)
        {
            CheckNeighbors(secondHex);
            for (int i = 1; i < 6; i++)
            {
                if (NeigborHexes.ContainsKey(i) && NeigborHexes.ContainsKey(i + 1))
                {
                    if (secondHexColor == NeigborHexes[i].GetComponentInChildren<Renderer>().GetComponent<Renderer>().material.color && secondHexColor == NeigborHexes[i + 1].GetComponentInChildren<Renderer>().GetComponent<Renderer>().material.color)
                    {
                        Edge.gameObject.SetActive(false);
                        GameObject[] DescendingOrderOfHexes = { secondHex, NeigborHexes[i], NeigborHexes[i + 1] };
                        DescendingOrderOfHexes = DescendingSortOfObjects(DescendingOrderOfHexes);

                        for (int k = 0; k < DescendingOrderOfHexes.Length; k++)
                        {
                            position = DescendingOrderOfHexes[k].transform.position;

                            Destroy(DescendingOrderOfHexes[k]);
                            anyMatchesForRotatingHexes = true;
                            ScoreScript.scoreValue += 5;

                            yield return new WaitForSeconds(0.01f);
                            ShiftHexPositionList.Add(position);
                            //yield return StartCoroutine(ShiftHexDown(position));
                        }
                        break;

                    }
                }
            }
            if (NeigborHexes.ContainsKey(6) && NeigborHexes.ContainsKey(1) && secondHex != null)
            {
                if (secondHexColor == NeigborHexes[6].GetComponentInChildren<Renderer>().GetComponent<Renderer>().material.color && secondHexColor == NeigborHexes[1].GetComponentInChildren<Renderer>().GetComponent<Renderer>().material.color)
                {
                    Edge.gameObject.SetActive(false);
                    GameObject[] DescendingOrderOfHexes = { secondHex, NeigborHexes[6], NeigborHexes[1] };
                    DescendingOrderOfHexes = DescendingSortOfObjects(DescendingOrderOfHexes);

                    for (int k = 0; k < DescendingOrderOfHexes.Length; k++)
                    {
                        position = DescendingOrderOfHexes[k].transform.position;

                        Destroy(DescendingOrderOfHexes[k]);
                        anyMatchesForRotatingHexes = true;
                        ScoreScript.scoreValue += 5;

                        yield return new WaitForSeconds(0.01f);
                        ShiftHexPositionList.Add(position);
                        //yield return StartCoroutine(ShiftHexDown(position));
                    }

                }
            }
        }

        if (thirdHex != null)
        {
            CheckNeighbors(thirdHex);
            for (int i = 1; i < 6; i++)
            {
                if (NeigborHexes.ContainsKey(i) && NeigborHexes.ContainsKey(i + 1))
                {
                    if (thirdHexColor == NeigborHexes[i].GetComponentInChildren<Renderer>().GetComponent<Renderer>().material.color && thirdHexColor == NeigborHexes[i + 1].GetComponentInChildren<Renderer>().GetComponent<Renderer>().material.color)
                    {
                        Edge.gameObject.SetActive(false);
                        GameObject[] DescendingOrderOfHexes = { thirdHex, NeigborHexes[i], NeigborHexes[i + 1] };
                        DescendingOrderOfHexes = DescendingSortOfObjects(DescendingOrderOfHexes);

                        for (int k = 0; k < DescendingOrderOfHexes.Length; k++)
                        {
                            position = DescendingOrderOfHexes[k].transform.position;

                            Destroy(DescendingOrderOfHexes[k]);
                            anyMatchesForRotatingHexes = true;
                            ScoreScript.scoreValue += 5;

                            yield return new WaitForSeconds(0.01f);
                            ShiftHexPositionList.Add(position);
                            //yield return StartCoroutine(ShiftHexDown(position));
                        }
                        break;
                    }
                }
            }
            if (NeigborHexes.ContainsKey(6) && NeigborHexes.ContainsKey(1) && thirdHex != null)
            {
                if (thirdHexColor == NeigborHexes[6].GetComponentInChildren<Renderer>().GetComponent<Renderer>().material.color && thirdHexColor == NeigborHexes[1].GetComponentInChildren<Renderer>().GetComponent<Renderer>().material.color)
                {
                    Edge.gameObject.SetActive(false);

                    GameObject[] DescendingOrderOfHexes = { thirdHex, NeigborHexes[6], NeigborHexes[1] };
                    DescendingOrderOfHexes = DescendingSortOfObjects(DescendingOrderOfHexes);

                    for (int k = 0; k < DescendingOrderOfHexes.Length; k++)
                    {
                        position = DescendingOrderOfHexes[k].transform.position;

                        Destroy(DescendingOrderOfHexes[k]);
                        anyMatchesForRotatingHexes = true;
                        ScoreScript.scoreValue += 5;

                        yield return new WaitForSeconds(0.01f);
                        ShiftHexPositionList.Add(position);
                        //yield return StartCoroutine(ShiftHexDown(position));
                    }
                }
            }
        }


        if (ShiftHexPositionList.Count != 0)
        {
            ShiftHexPositionList = DescendingSort(ShiftHexPositionList);
            foreach (var pos in ShiftHexPositionList)
            {
                yield return StartCoroutine(ShiftHexDown(pos));
            }
        }

        StartCoroutine(DestroyHexGroup());

    }

    private IEnumerator ShiftHexDown(Vector3 currentHexPosition)
    {
        GameObject upNeighbor = GetUpNeighbor(currentHexPosition);
        if (upNeighbor != null)
        {
            Destroy(upNeighbor.GetComponent<FixedJoint>());

            upNeighbor.transform.position = new Vector3(upNeighbor.transform.position.x, upNeighbor.transform.position.y - 1.8f, 0);
            upNeighbor.AddComponent<FixedJoint>();

            StartCoroutine(ShiftHexDown(upNeighbor.transform.position));
        }
        yield return new WaitForSeconds(0.001f);
    }

    private GameObject GetUpNeighbor(Vector3 currentHexPosition)
    {
        RaycastHit hit;
        Vector3 direction = Vector3.up;
        Ray ray = new Ray(currentHexPosition, direction);
        if (Physics.Raycast(ray, out hit))
        {
            return hit.transform.gameObject;
        }
        else
        {
            return null;
        }
    }


    private void GetListOfHexes()
    {
        ColumnAndElementCounts.Clear();
        foreach (Transform child in HexParent.transform)
        {
            HexList.Add(child.gameObject);

            int columnNo = (int)Mathf.Round(child.gameObject.transform.position.x / HexXOffset);
            if (ColumnAndElementCounts.ContainsKey(columnNo))
            {
                ColumnAndElementCounts[columnNo]++;
            }
            else
            {
                ColumnAndElementCounts.Add(columnNo, 1);
            }
            //Debug.Log("child: " + child.name);
        }
    }

    //private void CheckBoardforMatches()
    //{
    //    GetListOfHexes();
    //    //foreach (KeyValuePair<int, int> veri in ColumnAndElementCounts)
    //    //{
    //    //    Debug.Log("ColumnNo: " + veri.Key + "ElementCount:  " + veri.Value);
    //    //}


    //    if (HexList.Count > 0)
    //    {
    //        foreach (GameObject hex in HexList)
    //        {
    //            if (anyMatches)
    //            {
    //                StartCoroutine(DestroyHexGroup(hex));

    //            }
    //        }

    //    }

    //    StartCoroutine(DelayToFillBoard());

    //}

    private IEnumerator DestroyHexGroup()
    {
        
        List<Vector3> ShiftHexPositionList = new List<Vector3>();
        ShiftHexPositionList.Clear();
        anyMatches = false;
        GetListOfHexes();
        foreach (GameObject hex in HexList)
        {
            currentHex = hex;
            if (currentHex != null)
            {
                Vector3 position;
                Color hexColor = currentHex.GetComponentInChildren<Renderer>().GetComponent<Renderer>().material.color;
                CheckNeighbors(currentHex);

                for (int i = 1; i < 6; i++)
                {
                    if (NeigborHexes.ContainsKey(i) && NeigborHexes.ContainsKey(i + 1))
                    {
                        if (hexColor == NeigborHexes[i].GetComponentInChildren<Renderer>().GetComponent<Renderer>().material.color && hexColor == NeigborHexes[i + 1].GetComponentInChildren<Renderer>().GetComponent<Renderer>().material.color)
                        {
                            GameObject[] DescendingOrderOfHexes = { currentHex, NeigborHexes[i], NeigborHexes[i + 1] };
                            DescendingOrderOfHexes = DescendingSortOfObjects(DescendingOrderOfHexes);

                            for (int k = 0; k < DescendingOrderOfHexes.Length; k++)
                            {
                                position = DescendingOrderOfHexes[k].transform.position;

                                Destroy(DescendingOrderOfHexes[k]);
                                anyMatches = true;
                                ScoreScript.scoreValue += 5;

                                yield return new WaitForSeconds(0.01f);
                                ShiftHexPositionList.Add(position);
                                yield return StartCoroutine(ShiftHexDown(position));
                            }
                             
                            break;
                        }
                    }

                }

                if (NeigborHexes.ContainsKey(6) && NeigborHexes.ContainsKey(1) && currentHex != null)
                {
                    if (hexColor == NeigborHexes[6].GetComponentInChildren<Renderer>().GetComponent<Renderer>().material.color && hexColor == NeigborHexes[1].GetComponentInChildren<Renderer>().GetComponent<Renderer>().material.color)
                    {

                        GameObject[] DescendingOrderOfHexes = { currentHex, NeigborHexes[6], NeigborHexes[1] };
                        DescendingOrderOfHexes = DescendingSortOfObjects(DescendingOrderOfHexes);

                        for (int k = 0; k < DescendingOrderOfHexes.Length; k++)
                        {
                            position = DescendingOrderOfHexes[k].transform.position;

                            Destroy(DescendingOrderOfHexes[k]);
                            anyMatches = true;

                            yield return new WaitForSeconds(0.01f);
                            ShiftHexPositionList.Add(position);
                            yield return StartCoroutine(ShiftHexDown(position));
                        }


                    }
                }

            }
            //if (ShiftHexPositionList.Count != 0)
            //{
            //    ShiftHexPositionList = DescendingSort(ShiftHexPositionList);

            //    foreach (var pos in ShiftHexPositionList)
            //    {
            //        yield return StartCoroutine(ShiftHexDown(pos));
            //    }

            //}
        }

        if (anyMatches)
        {
            yield return StartCoroutine(DestroyHexGroup());
        }
        else
        {
            yield return StartCoroutine(DelayToFillBoard());

        }

    }

    private void FillBoard()
    {
        shouldFillBoard = false;
        GetListOfHexes();

        foreach (var element in ColumnAndElementCounts)
        {
            if (element.Value != 9)
            {
                shouldFillBoard = true;
            }
        }
        if (shouldFillBoard)
        {
            GameObject hexObject;
            int diff;
            for (int i = 0; i < boardWidth; i++)
            {
                if (ColumnAndElementCounts[i] < boardHeight)
                {
                    diff = boardHeight - ColumnAndElementCounts[i];

                    for (int j = 0; j < diff; j++)
                    {
                        Color randomColor = hexGridController.CreateRandomColor(null);
                        if (i % 2 == 0)
                        {
                            hexObject = (GameObject)Instantiate(HexPrefab, new Vector3(i * HexXOffset, j * HexYOffset, 0), Quaternion.identity);
                        }
                        else
                        {
                            hexObject = (GameObject)Instantiate(HexPrefab, new Vector3(i * HexXOffset, j * HexYOffset - 0.9f, 0), Quaternion.identity);

                        }

                        hexObject.GetComponentInChildren<Renderer>().material.color = randomColor;
                        hexObject.transform.parent = HexParent.transform;
                        //if (isBombActive)
                        //{
                        //    Bomb.SetActive(true);
                        //    BombCounterText.SetActive(true);
                        //    Bomb.transform.parent = hexObject.transform;
                        //}
                        ColumnAndElementCounts[i]++;

                    }

                }

            }

            StartCoroutine(DestroyHexGroup());
        }


    }

    private IEnumerator DelayToFillBoard()
    {
        yield return new WaitForSeconds(0.5f);
        FillBoard();
        //CheckBoardforMatches();
    }

    private GameObject[] DescendingSortOfObjects(GameObject[] HexList)
    {

        for (int i = 0; i < HexList.Length; i++)
        {
            int max = i;
            for (int j = i + 1; j < HexList.Length; j++)
            {
                if (HexList[j].transform.position.y > HexList[max].transform.position.y)
                {
                    max = j;
                }
            }
            GameObject temp = HexList[i];
            HexList[i] = HexList[max];
            HexList[max] = temp;
        }
        GameObject[] DescendingOrderOfObjects = HexList;

        return DescendingOrderOfObjects;
    }

    private List<Vector3> DescendingSort(List<Vector3> list)
    {
        for (int i = 0; i < list.Count; i++)
        {
            int max = i;
            for (int j = i + 1; j < list.Count; j++)
            {
                if (list[j].y > list[max].y)
                {
                    max = j;
                }
            }
            Vector3 temp = list[i];
            list[i] = list[max];
            list[max] = temp;
        }

        return list;
    }

    public void ClockWiseRotate()
    {
        isClockWiseRotate = true;
        isCounterClockWiseRotate = false;
        StartCoroutine(RotateEdge());
    }
    public void CounterClockWiseRotate()
    {
        isClockWiseRotate = false;
        isCounterClockWiseRotate = true;
        StartCoroutine(RotateEdge());
    }
    private IEnumerator RotateEdge()
    {

        Selected3Hexes[1].GetComponent<FixedJoint>().connectedBody = Edge.GetComponent<Rigidbody>();
        Selected3Hexes[2].GetComponent<FixedJoint>().connectedBody = Edge.GetComponent<Rigidbody>();
        Selected3Hexes[3].GetComponent<FixedJoint>().connectedBody = Edge.GetComponent<Rigidbody>();

        if (isRotateFinished)
        {

            for (int i = 1; i <= 360; i++)
            {

                if (isClockWiseRotate)
                {
                    Edge.transform.Rotate(0, 0, -1f);

                }
                else if (isCounterClockWiseRotate)
                {
                    Edge.transform.Rotate(0, 0, 1f);

                }

                yield return new WaitForSeconds(0.001f);
                isRotateFinished = false;

                //Her 120 derece dönüşte renkleri eşleşen hexler var mı diye kontrol ediyoruz. 
                //Yoksa 360 derece dönerek başlangıç pozisyonuna dönmüş oluyor
                if (i % 120 == 0)
                {

                    Selected3Hexes[1].GetComponent<FixedJoint>().connectedBody = null;
                    Selected3Hexes[2].GetComponent<FixedJoint>().connectedBody = null;
                    Selected3Hexes[3].GetComponent<FixedJoint>().connectedBody = null;
                    anyMatchesForRotatingHexes = false;

                    StartCoroutine(DestroyHexes());
                    if (anyMatchesForRotatingHexes)
                    {
                        isRotateFinished = true;
                        break;
                    }
                    else
                    {
                        Selected3Hexes[1].GetComponent<FixedJoint>().connectedBody = Edge.GetComponent<Rigidbody>();
                        Selected3Hexes[2].GetComponent<FixedJoint>().connectedBody = Edge.GetComponent<Rigidbody>();
                        Selected3Hexes[3].GetComponent<FixedJoint>().connectedBody = Edge.GetComponent<Rigidbody>();
                    }
                }

            }

            isRotateFinished = true;

            Selected3Hexes[1].GetComponent<FixedJoint>().connectedBody = null;
            Selected3Hexes[2].GetComponent<FixedJoint>().connectedBody = null;
            Selected3Hexes[3].GetComponent<FixedJoint>().connectedBody = null;

        }

    }
}
