using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class TapGameController : MonoBehaviour
{

    [SerializeField] GameObject tileObjectRegularPrefab;
    [SerializeField] GameObject tileObjectDynamicPrefab;
    [SerializeField] Transform startPointReg;
    [SerializeField] Transform startPointDyn;
    //[SerializeField] Collider2D gameBounds;
    //[SerializeField] int tilesNb = 0;

    private Coroutine currentCoroutine;

    private List<Transform> tilesRegular = new List<Transform>();
    private List<Transform> tilesDynamic = new List<Transform>();

    [SerializeField] int bpm = 60;
    [SerializeField] int addedSpeed = 20;
    [SerializeField] float startDelayInSec = 1;
    /// <summary>
    /// beat per second
    /// </summary>
    private float bps = 0;

    private bool start = false;

    [SerializeField] KeyCode TappingKey;

    [SerializeField] Difficulty difficulties = new Difficulty();
    [SerializeField] Difficultylvl currentDifficulty = Difficultylvl.Easy;

    private Motifs currentMotif = Motifs.Noire;



    public enum Motifs {Noire, Croche, Double, Triple, Maintien}
    public enum Difficultylvl { Easy, Medium, Hard }



    private bool tileInCollision = false;
    private bool currentlyLongTile = false;
    private bool tileTapped = false;


    private void Update()
    {

        if (start)
        {

    #region MOVE
            foreach (var tile in tilesRegular)
            {
                if (tile != null)
                    tile.Translate(Vector3.right * bps * addedSpeed * Time.deltaTime);
            }

            foreach (var tileDyn in tilesDynamic)
            {
                if (tileDyn != null)
                    tileDyn.Translate(Vector3.right * bps * addedSpeed * Time.deltaTime);
            }
            #endregion

            if (Input.GetKeyDown(TappingKey))
            {
                if (tileInCollision)
                {
                    tileTapped = true;
                    TileTouched();
                }
            }


        }

    }

    public void Init()
    {
        bps = bpm / 60;

        if (currentDifficulty == Difficultylvl.Easy)
            currentMotif = difficulties.Lvl_1;
        else if (currentDifficulty == Difficultylvl.Medium)
            currentMotif = difficulties.Lvl_2;
        else
            currentMotif = difficulties.Lvl_3;
    }

    public void StartGame()
    {
        Debug.Log("Start game");
        start = true;
        if(tileObjectRegularPrefab)
            StartCoroutine(InstantiateTilesRegularCoroutines());
        if (tileObjectDynamicPrefab)
            StartCoroutine(InstantiateTilesDynamicCoroutines());

    }

    void StopGame()
    {
        start = false;
    }

    private IEnumerator InstantiateTilesRegularCoroutines()
    {
        yield return new WaitForSeconds(startDelayInSec);
        WaitForSeconds delay = new WaitForSeconds(bps);

        Debug.Log("Start coroutine reg");
        while (start)
        {
            GameObject tile = Instantiate(tileObjectRegularPrefab, startPointReg.transform.position, Quaternion.identity);
            tile.transform.parent = startPointReg;
            tilesRegular.Add(tile.transform);
            tile.GetComponent<TileParent>().GetTile().SetController(this);
            yield return delay;
        }
    }

    private IEnumerator InstantiateTilesDynamicCoroutines()
    {

        yield return new WaitForSeconds(startDelayInSec);
        yield return new WaitForSeconds(Random.Range(2, 7));
        Debug.Log("Start coroutine dyn");
        while (start)
        {
            GameObject tile = Instantiate(tileObjectDynamicPrefab, startPointReg.transform.position, Quaternion.identity);
            tile.transform.parent = startPointDyn;
            tilesDynamic.Add(tile.transform);
            tile.GetComponent<TileParent>().GetTile().SetController(this);
            yield return new WaitForSeconds(Random.Range(0.5f, 5));
        }
    }


    public void TriggerTileInCollisionZone(bool isInCollision, bool wait = false, bool isStart = true)
    {
        tileInCollision = isInCollision;
        if (isInCollision)
            currentlyLongTile = wait;
        else if(! tileTapped)
        {
            TileMissed();
        }

        tileTapped = false;
    }


    public void TileMissed()
    {
        Debug.Log("Tile Missed !");
        MainGameManager.Instance.AddToScore(-10);
    }

    public void TileTouched()
    {
        Debug.Log("Tile Touched !");
        MainGameManager.Instance.AddToScore(10);
    }





    [System.Serializable]
    class Difficulty
    {
        public Motifs Lvl_1;
        public Motifs Lvl_2;
        public Motifs Lvl_3;
    }

}
