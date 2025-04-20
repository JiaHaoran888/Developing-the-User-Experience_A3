using UnityEngine;
using System.Collections;
using DG.Tweening;
using UnityEngine.SceneManagement;
//<summary>
//Game object, that creates maze and instantiates it in scene
//</summary>
public class MazeSpawner : MonoBehaviour
{
    public enum MazeGenerationAlgorithm
    {
        PureRecursive,
        RecursiveTree,
        RandomTree,
        OldestTree,
        RecursiveDivision,
    }

    public MazeGenerationAlgorithm Algorithm = MazeGenerationAlgorithm.PureRecursive;
    public bool FullRandom = false;
    public int RandomSeed = 12345;
    public GameObject Floor = null;
    public GameObject Wall = null;
    public GameObject Pillar = null;
    public int Rows;
    public int Columns;
    public float CellWidth = 5;
    public float CellHeight = 5;
    public bool AddGaps = true;
    public GameObject GoalPrefab = null;
    public GameObject BossPrefab = null; // Boss prefab
    private GameObject boss;
    private BasicMazeGenerator mMazeGenerator = null;

    public float safeDistance = 2f;
    public GameObject Player; // 

    public GameObject arrow;
    public Camera ViewCamera; //
    public float duration = 1f; // 

    //viewCamera
    public Transform target; // 
    public float smoothSpeed = 0.125f; // 
    public float distance = 5f; // 
    public float height = 2f; // 
    public float lookAtSpeed = 2f; // 
    private Vector3 velocity = Vector3.zero; // 
    void Awake()
    {
        GameMgr.instance.canmove = false;
        Player = GameObject.FindGameObjectWithTag("Player");
        Rows = GameMgr.instance.row;
        Columns = GameMgr.instance.col;
        if (!FullRandom)
        {
            Random.seed = RandomSeed;
        }
        switch (Algorithm)
        {
            case MazeGenerationAlgorithm.PureRecursive:
                mMazeGenerator = new RecursiveMazeGenerator(Rows, Columns);
                break;
            case MazeGenerationAlgorithm.RecursiveTree:
                mMazeGenerator = new RecursiveTreeMazeGenerator(Rows, Columns);
                break;
            case MazeGenerationAlgorithm.RandomTree:
                mMazeGenerator = new RandomTreeMazeGenerator(Rows, Columns);
                break;
            case MazeGenerationAlgorithm.OldestTree:
                mMazeGenerator = new OldestTreeMazeGenerator(Rows, Columns);
                break;
            case MazeGenerationAlgorithm.RecursiveDivision:
                mMazeGenerator = new DivisionMazeGenerator(Rows, Columns);
                break;
        }

        mMazeGenerator.GenerateMaze();
        for (int row = 0; row < Rows; row++)
        {
            for (int column = 0; column < Columns; column++)
            {
                float x = column * (CellWidth + (AddGaps ? .2f : 0));
                float z = row * (CellHeight + (AddGaps ? .2f : 0));
                MazeCell cell = mMazeGenerator.GetMazeCell(row, column);
                GameObject tmp;
                tmp = Instantiate(Floor, new Vector3(x, 0, z), Quaternion.Euler(0, 0, 0)) as GameObject;
                tmp.transform.parent = transform;
                if (cell.WallRight)
                {
                    tmp = Instantiate(Wall, new Vector3(x + CellWidth / 2, 0, z) + Wall.transform.position, Quaternion.Euler(0, 90, 0)) as GameObject;// right
                    tmp.transform.parent = transform;
                }
                if (cell.WallFront)
                {
                    tmp = Instantiate(Wall, new Vector3(x, 0, z + CellHeight / 2) + Wall.transform.position, Quaternion.Euler(0, 0, 0)) as GameObject;// front
                    tmp.transform.parent = transform;
                }
                if (cell.WallLeft)
                {
                    tmp = Instantiate(Wall, new Vector3(x - CellWidth / 2, 0, z) + Wall.transform.position, Quaternion.Euler(0, 270, 0)) as GameObject;// left
                    tmp.transform.parent = transform;
                }
                if (cell.WallBack)
                {
                    tmp = Instantiate(Wall, new Vector3(x, 0, z - CellHeight / 2) + Wall.transform.position, Quaternion.Euler(0, 180, 0)) as GameObject;// back
                    tmp.transform.parent = transform;
                }
                if (cell.IsGoal && GoalPrefab != null)
                {
                    tmp = Instantiate(GoalPrefab, new Vector3(x, 1, z), Quaternion.Euler(0, 0, 0)) as GameObject;
                    tmp.transform.parent = transform;
                }
            }
        }
        if (Pillar != null)
        {
            for (int row = 0; row < Rows + 1; row++)
            {
                for (int column = 0; column < Columns + 1; column++)
                {
                    float x = column * (CellWidth + (AddGaps ? .2f : 0));
                    float z = row * (CellHeight + (AddGaps ? .2f : 0));
                    GameObject tmp = Instantiate(Pillar, new Vector3(x - CellWidth / 2, 0, z - CellHeight / 2), Quaternion.identity) as GameObject;
                    tmp.transform.parent = transform;
                }
            }
        }
        SpawnBoss();
    }

    private void Start()
    {
        this.arrow.SetActive(false);
        
        StartCoroutine(ZoomToTarget(boss.transform.position + new Vector3(0, 3, 0), Player.transform.position + new Vector3(0, 3, 0)));

    }



    private IEnumerator ZoomToTarget(Vector3 targetPosition1, Vector3 targetPosition2)
    {
        // 
        Sequence sequence = DOTween.Sequence();


        // 
        Vector3 cameraPos1 = new Vector3(targetPosition1.x, targetPosition1.y + height, targetPosition1.z);
        sequence.Append(ViewCamera.transform.DOLookAt(targetPosition1, duration).SetEase(Ease.InOutQuad));
        sequence.AppendInterval(1f);
        Tween moveTween1 = ViewCamera.transform.DOMove(cameraPos1, duration).SetEase(Ease.InOutQuad);
        

       

        sequence.Append(moveTween1); //

        //
        sequence.Append(ViewCamera.transform.DOLookAt(targetPosition1, duration).SetEase(Ease.InOutQuad));

        //
        sequence.AppendInterval(1f);


        // 
        Vector3 cameraPos2 = new Vector3(targetPosition2.x - 3, targetPosition2.y + height, targetPosition2.z - 3);
        sequence.Append(ViewCamera.transform.DOLookAt(targetPosition2, duration).SetEase(Ease.InOutQuad));
        Tween moveTween2 = ViewCamera.transform.DOMove(cameraPos2, duration).SetEase(Ease.InOutQuad);
        sequence.Append(moveTween2); //


        sequence.Append(ViewCamera.transform.DOLookAt(targetPosition2, duration).SetEase(Ease.InOutQuad)).OnComplete(()=> { GameMgr.instance.canmove = true; });



        yield return sequence.WaitForCompletion();


       
        arrow.SetActive(true);
        ViewCamera.gameObject.SetActive(false);
    }
    private void Update()
    {
        if (boss.transform!=null)
        {
            Vector3 arrowdir = (boss.transform.position - Player.transform.position).normalized;
            arrowdir.y = 0;
            arrow.transform.right = -arrowdir;
        }
       
       
    }


    void SpawnBoss()
    {
        if (BossPrefab != null)
        {
            bool validPosition = false;
            Vector3 bossPosition = Vector3.zero;

            while (!validPosition)
            {

                int bossRow = Random.Range(3, Rows);
                int bossColumn = Random.Range(3, Columns);
                float x = bossColumn * (CellWidth + (AddGaps ? 2f : 0));
                float z = bossRow * (CellHeight + (AddGaps ? 2f : 0));
                bossPosition = new Vector3(x, 0f, z);


                if (Player != null)
                {
                    float distance = Vector3.Distance(bossPosition, Player.transform.position);
                    if (distance > safeDistance)
                    {
                        validPosition = true;
                    }
                }
                else
                {

                    validPosition = true;
                }
            }

            GameObject bossobj = Instantiate(BossPrefab, bossPosition, Quaternion.identity) as GameObject;
            bossobj.transform.parent = transform;
            boss = bossobj;
        }
    }
}
