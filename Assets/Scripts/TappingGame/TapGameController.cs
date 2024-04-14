using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using System;

public class TapGameController : MonoBehaviour
{

    [SerializeField] GameObject tileObjectPrefab;
    [SerializeField] Transform startPoint;
    //[SerializeField] Collider2D gameBounds;
    //[SerializeField] int tilesNb = 0;

    /// <summary>
    /// param bool : is tile long
    /// </summary>
    private Action<bool> tileTouchedEvent;

    private Coroutine currentCoroutine;

    private List<Transform> tilesList = new List<Transform>();

    [SerializeField] int bpm = 60;
    [SerializeField] int addedSpeed = 20;
    [SerializeField] float startDelayInSec = 1;
    /// <summary>
    /// beat per second
    /// </summary>
    private float bps = 0;

    private bool start = false;

    [SerializeField] KeyCode TappingKey;

    [SerializeField] Difficultylvl currentDifficulty = Difficultylvl.Easy;

    public enum Difficultylvl { Easy, Medium, Hard }

    [SerializeField] private bool useRegularInstanciation = false;
    [SerializeField] private float regularWaitBetweenInstanceInSec = 2f;




    private bool tileInCollision = false;
    private bool tileTapped = false;

    private bool waitingForLongTile = false;
    private bool longTileInProgress = false;

    public List<Tile> currentCollisionTiles = new List<Tile>();

    public List<Tile> garbageTiles = new List<Tile>();


    private bool mustCollectGarbage = false;


    private void Update()
    {

        if (start)
        {

            #region MOVE

            foreach (var tileDyn in tilesList)
            {
                if (tileDyn != null)
                    tileDyn.Translate(Vector3.right * bps * addedSpeed * Time.deltaTime);
            }
            #endregion

            if (!waitingForLongTile)
            {
                if (Input.GetKeyDown(TappingKey))
                {
                    if (tileInCollision)
                    {
                        tileTapped = true;
                        TileTouched();
                    }
                    else
                        currentCollisionTiles.Clear();
                }
            }
            else
            {
                if (Input.GetKeyDown(TappingKey))
                {
                    if (tileInCollision)
                    {
                        tileTapped = true;
                        longTileInProgress = true;
                        TileLongTouch(true);
                    }
                    else
                        currentCollisionTiles.Clear();
                }
                if (Input.GetKeyUp(TappingKey))
                {
                    if (tileInCollision && longTileInProgress)
                    {
                        tileTapped = true;
                        waitingForLongTile = false;
                        TileLongTouch(false);
                    }
                    else
                        currentCollisionTiles.Clear(); //Did not go up at the right moment

                    longTileInProgress = false;
                }
            }



        }


        if (mustCollectGarbage)
            CollectGarbage();

    }

    public void Init()
    {
        bps = bpm / 60;
        CollectGarbage();

        foreach (Transform trans in tilesList)
        {
            Destroy(trans.gameObject);
        }
        tilesList.Clear();

        foreach (Tile tile in currentCollisionTiles)
        {
            Destroy(tile.parentObject);
        }
        currentCollisionTiles.Clear();

        Difficultylvl currentDifficulty = Difficultylvl.Easy;

        start = false;
        tileInCollision = false;
        tileTapped = false;
        waitingForLongTile = false;
        longTileInProgress = false;
    }

    public void StartGame()
    {
        Debug.Log("Start game");
        start = true;
        if (tileObjectPrefab)
            StartCoroutine(InstantiateTilesCoroutines());

        StartCoroutine(GarbageCountdownCoroutine());

    }

    public void StopGame()
    {
        start = false;
    }

    private IEnumerator InstantiateTilesCoroutines()
    {

        if (useRegularInstanciation)
        {
            #region regular instanciation

            WaitForSeconds delay = new WaitForSeconds(regularWaitBetweenInstanceInSec);
            yield return new WaitForSeconds(startDelayInSec);

            while (start)
            {
                InstantiateTile();
                yield return delay;
            }

            #endregion
        }
        else
        {
            #region dynamic instanciation
            int chanceForDouble = 60;

            yield return new WaitForSeconds(startDelayInSec);
            yield return new WaitForSeconds(UnityEngine.Random.Range(2, 7));
            Debug.Log("Start coroutine");
            while (start)
            {
                if (UnityEngine.Random.Range(1, 100) <= chanceForDouble)
                {
                    Debug.Log("Instantiate long");
                    InstantiateTile(isStartLong: true);
                    yield return new WaitForSeconds(UnityEngine.Random.Range(2, 6));
                    InstantiateTile(isEndLong: true);
                }
                else
                {
                    InstantiateTile();
                }



                yield return new WaitForSeconds(UnityEngine.Random.Range(0.5f, 5));
            }
            #endregion
        }


    }

    private void InstantiateTile(bool isStartLong = false, bool isEndLong = false)
    {
        GameObject tileobject = Instantiate(tileObjectPrefab, startPoint.transform.position, Quaternion.identity);
        tileobject.transform.parent = startPoint;
        tilesList.Add(tileobject.transform);
        Tile tile = tileobject.GetComponent<TileParent>().GetTile();
        tile.SetController(this);
        if (isStartLong)
            tile.SetIsLongStart();
        if (isEndLong)
            tile.SetIsLongEnd();
    }

    private IEnumerator GarbageCountdownCoroutine()
    {
        WaitForSeconds delay = new WaitForSeconds(5);
        while (start)
        {
            yield return delay;
            mustCollectGarbage = true;
        }
    }


    /// <summary>
    /// trigger in collision and handle missed tile case
    /// </summary>
    /// <param name="tile"></param>
    /// <param name="isInCollision"></param>
    /// <param name="wait"></param>
    public void TriggerTileInCollisionZone(Tile tile, bool isInCollision, bool wait = false)
    {
        tileInCollision = isInCollision;
        if (isInCollision)
        {
            waitingForLongTile = wait;
            currentCollisionTiles.Add(tile);
        }
        else if (!tileTapped)
        {
            TileMissed();
            if (tile.isLong && tile.isEnd)
            {
                Debug.Log("Missed long tile end");
                waitingForLongTile = false; //mark end of long tile in case of miss
                longTileInProgress = false;
            }
            currentCollisionTiles.Remove(tile);
        }

        tileTapped = false;
    }

    public void RegisterToDestroy(Tile tile)
    {
        tile.parentObject.SetActive(false);
        tilesList.Remove(tile.transform);
        garbageTiles.Add(tile);
    }


    public void TileMissed()
    {
        //Debug.Log("Tile Missed !");
        //MainGameManager.Instance.AddToScore(-10);
    }

    public void TileTouched()
    {
        Debug.Log("Tile Touched !");
        tileTouchedEvent.Invoke(false);
        MainGameManager.Instance.AddToScore(DataDetail.SCORE_INCREASE_SIMPLE);
        MainGameManager.Instance.SoundBoard.SourceTappingGame.PlayOneShot(MainGameManager.Instance.SoundBoard.ClipTileTaped);
        CleanGoodTiles();
    }

    public void TileLongTouch(bool isStart)
    {
        if (isStart)
        {
            tileTouchedEvent.Invoke(false);
            MainGameManager.Instance.AddToScore(DataDetail.SCORE_INCREASE_SIMPLE);
            currentCollisionTiles.Clear(); // Remove from current tile but keep moving to the right
        }
        else
        {
            tileTouchedEvent.Invoke(true);
            MainGameManager.Instance.AddToScore(DataDetail.SCORE_INCREASE_DOUBLE);
            CleanGoodTiles();
        }
        MainGameManager.Instance.SoundBoard.SourceTappingGame.PlayOneShot(MainGameManager.Instance.SoundBoard.ClipTileTaped);
    }

    public void SuscribeToTileTouchEvent(Action<bool> callback)
    {
        tileTouchedEvent += callback;
    }

    private void CleanGoodTiles()
    {
        foreach (Tile tile in currentCollisionTiles)
        {
            if (tile == null)
                Debug.Log("debug");
            RegisterToDestroy(tile);
        }
        currentCollisionTiles.Clear();

    }

    private void CollectGarbage()
    {
        /*for (int i = garbageTiles.Count - 1; i >= 0; i--)
        {
            Destroy(garbageTiles.)
            if (whatever) garbageTiles.RemoveAt(i);
        }*/
        foreach (Tile tile in garbageTiles)
        {
            Destroy(tile.parentObject);
        }
        garbageTiles.Clear();
        mustCollectGarbage = false;
    }

    /*List<float> getTileTimeFromMidi()
    {
        List<float> ticks = new List<float>();
        var midiFile = new MidiFile("path_to_midi_file.mid");
        int BPM = midiFile.TicksPerQuarterNote;
        foreach (var track in midiFile.Tracks)
        {
            var c = 0;
            foreach (var midiEvent in track.MidiEvents)
            {

                if (midiEvent.MidiEventType == MidiEventType.NoteOn)
                {
                    var time = midiEvent.Time;
                    c = c + time;
                    // Ici, créer une liste des c après boucle puis faire apparaître notes en fonction des c
                }

            }
        }
    }*/
}
