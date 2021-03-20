using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class HexGridController : MonoBehaviour
{
    [SerializeField] private GameObject Hexagon;
    [SerializeField] private GameObject hexParent;
    public int Height;
    public int Width;
    [SerializeField] private List<Color> ColorList;
    Camera cam;
    [SerializeField] private LayerMask movementMask;
    private Dictionary<int, GameObject> CurrentHexes = new Dictionary<int, GameObject>();
    private GameObject hexObject;
    private float xOffset = Mathf.Sqrt(3) * 0.9f;
    private float yOffset = -1.8f;
    private float xPos;
    private float yPos;
    private int orderNum = 1;

    // Start is called before the first frame update
    void Start()
    {
        CreateHexagons();
    }
    // Update is called once per frame
    void Update()
    {
        //if (Input.GetMouseButtonDown(0))
        //{
        //    Ray ray = cam.ScreenPointToRay(Input.mousePosition);
        //    RaycastHit hit;
        //    if (Physics.Raycast(ray, out hit, 100, movementMask))
        //    {

        //        Debug.Log("We hit: " + hit.collider.name + "  " + hit.transform.position);

        //    }
        //}

    }

    private void CreateHexagons()
    {
        Color randomColor = CreateRandomColor(null);
        Color tempColor = randomColor; // Bir önceki hex in rengini tutup, sonraki hexin aynı renkte olmayacağından emin olmak için
        for (int x = 0; x < Width; x++)
        {
            xPos = x * xOffset;

            for (int y = 0; y < Height; y++)
            {
                randomColor = CreateRandomColor(tempColor);
                yPos = y * yOffset;
                if (x % 2 != 0)
                {
                    yPos -= 0.9f;
                }
                hexObject = (GameObject)Instantiate(Hexagon, new Vector3(xPos, yPos, 0), Quaternion.identity);
                hexObject.layer = 8;
                hexObject.GetComponentInChildren<Renderer>().material.color = randomColor;
                hexObject.name = (x + 1) + "_" + (y + 1);
                hexObject.transform.parent = hexParent.transform;
                CurrentHexes.Add(orderNum, hexObject);
                orderNum++;
                tempColor = randomColor;
            }
        }
    }

    public Color CreateRandomColor(Color? color)
    {
        int randomNum = Random.Range(0, ColorList.Count);
        Color randomColor = ColorList[randomNum];
        while (randomColor == color)
        {
            randomNum = Random.Range(0, ColorList.Count);
            randomColor = ColorList[randomNum];
        }
        return randomColor;
    }

    public void RestartGame()
    {
        SceneManager.LoadScene(1);
    }
}
