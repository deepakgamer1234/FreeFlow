using System.Collections.Generic;
using TMPro;
using UnityEngine;


public class GameplayManager : MonoBehaviour
{
    public static GameplayManager Instance;
    [HideInInspector] public bool hasGameFinished;
    [SerializeField] private TMP_Text _titleText;
    [SerializeField] private GameObject _winText;
    [SerializeField] private SpriteRenderer _boardPrefab, _bgCellPrefab;
    private LevelData CurrentLevelData;
    [SerializeField] private Node _nodePrefab;
    private List<Node> _nodes;
    public Dictionary<Vector2Int, Node> _nodeGrid;
    public List<Color> NodeColors;
    private Node startNode;
    [SerializeField] private SpriteRenderer _clickHighlight;


    private void Awake()
    {
        Instance = this;
        hasGameFinished = false;
        _winText.SetActive(false);
        CurrentLevelData = GameManager.Instance.GetLevel();
        print("CurrentLevelData :" + CurrentLevelData);
        SpawnBoard();
        SpawnNodes();
    }

    private void SpawnBoard()
    {
        int currentLevelSize = 4;
        for (int i = currentLevelSize - 1; i >= 0; i--)
        {
            for (int j = currentLevelSize - 1; j >= 0; j--)
            {
                Instantiate(_bgCellPrefab, new Vector3(i + 0.5f, j + 0.5f, 0f), Quaternion.identity);
            }
        }
        _clickHighlight.size = new Vector2(currentLevelSize / 4f, currentLevelSize / 4f);
        _clickHighlight.transform.position = Vector3.zero;
        _clickHighlight.gameObject.SetActive(false);

    }

    private void SpawnNodes()
    {
        _nodes = new List<Node>();
        _nodeGrid = new Dictionary<Vector2Int, Node>();
        int currentLevelSize = 4;
        Node spawnedNode;
        Vector3 spawnPos;

        for (int j = currentLevelSize - 1; j >= 0; j--)
        {
            for (int i = currentLevelSize - 1; i >= 0; i--)
            {
                spawnPos = new Vector3(i + 0.5f, j + 0.5f, 0f);
                spawnedNode = Instantiate(_nodePrefab, spawnPos, Quaternion.identity);
                spawnedNode.Init();

                int colorIdForSpawnedNode = GetColorId(i, j);

                if (colorIdForSpawnedNode != -1)
                {
                    spawnedNode.SetColorForPoint(colorIdForSpawnedNode);
                }

                _nodes.Add(spawnedNode);
                _nodeGrid.Add(new Vector2Int(i, j), spawnedNode);
                spawnedNode.gameObject.name = i.ToString() + j.ToString();
                spawnedNode.Pos2D = new Vector2Int(i, j);
            }
        }

        List<Vector2Int> offsetPos = new List<Vector2Int>()
            {Vector2Int.up,Vector2Int.down,Vector2Int.left,Vector2Int.right };

        foreach (var item in _nodeGrid)
        {
            foreach (var offset in offsetPos)
            {
                var checkPos = item.Key + offset;
                if (_nodeGrid.ContainsKey(checkPos))
                {
                    item.Value.SetEdge(offset, _nodeGrid[checkPos]);
                }
            }
        }
    }

    public int GetColorId(int i, int j)
    {
        List<Edge> edges = CurrentLevelData.Edges;
        Vector2Int point = new Vector2Int(i, j);

        for (int colorId = 0; colorId < edges.Count; colorId++)
        {
            if (edges[colorId].StartPoint == point ||
                edges[colorId].EndPoint == point)
            {
                return colorId;
            }
        }

        return -1;
    }

    public Color GetHighLightColor(int colorID)
    {
        Color result = NodeColors[colorID % NodeColors.Count];
        result.a = 0.4f;
        return result;
    }

    private void Update()
    {
        if (hasGameFinished) return;

        if (Input.GetMouseButtonDown(0))
        {
            startNode = null;
            return;
        }

        if (Input.GetMouseButton(0))
        {
            Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            Vector2 mousePos2D = new Vector2(mousePos.x, mousePos.y);
            RaycastHit2D hit = Physics2D.Raycast(mousePos, Vector2.zero);

            if (startNode == null)
            {
                if (hit && hit.collider.gameObject.TryGetComponent(out Node tNode)
                    && tNode.IsClickable)
                {
                    startNode = tNode;
                    _clickHighlight.gameObject.SetActive(true);
                    _clickHighlight.gameObject.transform.position = (Vector3)mousePos2D;
                    _clickHighlight.color = GetHighLightColor(tNode.colorId);

                }
                return;
            }
            _clickHighlight.gameObject.transform.position = (Vector3)mousePos2D;

            if (hit && hit.collider.gameObject.TryGetComponent(out Node tempNode)
                    && startNode != tempNode)
            {
                if (startNode.colorId != tempNode.colorId && tempNode.IsEndNode)
                {
                    return;
                }

                startNode.UpdateInput(tempNode);
                CheckWin();
                startNode = null;
            }

            return;
        }

        if (Input.GetMouseButtonUp(0))
        {
            startNode = null;
            _clickHighlight.gameObject.SetActive(false);

        }
    }

    private void CheckWin()
    {
        foreach (var item in _nodes)
        {
            item.SolveHighlight();
        }

        if (Node.colorHighlightCount.Count == GameManager.Instance.CurrentLevel + 1)
        {
            _winText.gameObject.SetActive(true);
            _clickHighlight.gameObject.SetActive(false);

            hasGameFinished = true;
        }
    }

    public void ClickedBack()
    {
        GameManager.Instance.GoToMainMenu();
        Node.colorHighlightCount.Clear();
    }

    public void ClickedRestart()
    {
        Node.colorHighlightCount.Clear();
        GameManager.Instance.GoToGameplay();
    }

    public void ClickedNextLevel()
    {
        if (!hasGameFinished) return;
        Node.colorHighlightCount.Clear();
        GameManager.Instance.CurrentLevel++;
        GameManager.Instance.GoToGameplay();
    }
}


