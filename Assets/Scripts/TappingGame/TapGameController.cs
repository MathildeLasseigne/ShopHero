using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using System;

public class TapGameController : MonoBehaviour
{

    [SerializeField] GameObject tileObjectPrefab;
    [SerializeField] Transform startPoint;

    /// <summary>
    /// param bool : is tile long
    /// </summary>
    private Action<bool> tileTouchedEvent;

    private Coroutine currentCoroutine;

    //Tiles collection
    private List<Transform> tilesList = new List<Transform>();
    private List<Tile> currentCollisionTiles = new List<Tile>();
    private List<Tile> garbageTiles = new List<Tile>();


    [Header("Instance controls")]
    public string PathToPartition = "";

    [SerializeField] KeyCode TappingKey;

    [SerializeField] Difficultylvl currentDifficulty = Difficultylvl.Easy;

    public enum Difficultylvl { Easy, Medium, Hard }


    [SerializeField] private bool useRegularInstanciation = false;
    [SerializeField] private float regularWaitBetweenInstanceInSec = 2f;

    private bool start = false;

    [SerializeField] int addedSpeed = 20;
    /// <summary>
    /// beat per second
    /// </summary>
    [SerializeField] int bpm = 60;

    private float bps = 0;


    private bool tileInCollision = false;
    private bool tileTapped = false;

    private bool waitingForLongTile = false;
    private bool longTileInProgress = false;

    


    private bool mustCollectGarbage = false;

    int skipNotesTo = 5;

    //Midi calculations

    int currentNoteIdx = 0;

    List<float> noteWaintingTime = new List<float>();

    //Calculated with math
    float distance = 709.09f;

    float timeDecalage;

    float speed =  0f;

    float firstWaitingTime = 0f;

    #region Test vars

    [Header("Test vars")]
    [SerializeField] private bool useRandomInstanciation = true;
    [SerializeField] float startDelayInSec = 1;


    #endregion


    private void Update()
    {

        if (start)
        {

            #region MOVE

            foreach (var tileDyn in tilesList)
            {
                if (tileDyn != null)
                    tileDyn.Translate(Vector3.right * speed * Time.deltaTime);
            }
            #endregion

            #region Collision

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

            #endregion

        }


        if (mustCollectGarbage)
            CollectGarbage();
    }

    public void Init()
    {
        bps = bpm / 60;
        CollectGarbage();

        CleanAllTiles();

        Difficultylvl currentDifficulty = Difficultylvl.Easy;

        start = false;
        tileInCollision = false;
        tileTapped = false;
        waitingForLongTile = false;
        longTileInProgress = false;

        Debug.Log("Parent" + startPoint.position);

        this.noteWaintingTime = getTileTimeFromMidi();
        speed = bps * addedSpeed;
        timeDecalage = distance / speed;
        for (int i = 0; i < this.skipNotesTo; i++)
        {
            currentNoteIdx = i;
            firstWaitingTime += noteWaintingTime[i];
        }
        firstWaitingTime -= timeDecalage;

    }

    public void StartGame()
    {
        start = true;
        if (tileObjectPrefab)
            StartCoroutine(InstantiateTilesCoroutines());

        StartGarbageCollectionRoutine();

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


            
            //yield return new WaitForSeconds(firstWaitingTime);

            Debug.Log("Start coroutine");

            #region Random Instanciation for tests : if useRandomInstanciation = true

            if (useRandomInstanciation) // For tests, else set to false
            {
                int chanceForDouble = 60;

                yield return new WaitForSeconds(startDelayInSec);

                yield return new WaitForSeconds(UnityEngine.Random.Range(2, 7));


                while (start && currentNoteIdx < noteWaintingTime.Count)
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

                    yield return new WaitForSeconds(UnityEngine.Random.Range(2, 7));
                }
            }
            #endregion
            else
            {
                // TODO : Real instanciation
            }

            /*yield return new WaitForSeconds(2);
            InstantiateTile(isStartLong: true);

            yield return new WaitForSeconds(3);
            InstantiateTile(isStartLong: true);

            yield return new WaitForSeconds(5);
            InstantiateTile(isStartLong: true);*/

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
                waitingForLongTile = false; //mark end of long tile in case of miss
                longTileInProgress = false;
            }
            currentCollisionTiles.Remove(tile);
        }

        tileTapped = false;
    }

    


    public void TileMissed()
    {
        //Debug.Log("Tile Missed !");
        //MainGameManager.Instance.AddToScore(-10);
    }

    public void TileTouched()
    {
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

    



    #region Instance cleanup

    public void RegisterToDestroy(Tile tile)
    {
        tile.parentObject.SetActive(false);
        tilesList.Remove(tile.transform);
        garbageTiles.Add(tile);
    }

    private void CleanGoodTiles()
    {
        foreach (Tile tile in currentCollisionTiles)
        {
            RegisterToDestroy(tile);
        }
        currentCollisionTiles.Clear();

    }

    private void CleanAllTiles()
    {
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
    }

    private void StartGarbageCollectionRoutine()
    {
        StartCoroutine(GarbageCountdownCoroutine());
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

    #endregion

    List<float> getTileTimeFromMidi()
    {
        List<float> ticks = new List<float>();
        List<float> values = new List<float>();
        var midiFile = new MidiFile(PathToPartition);
        int BPM = midiFile.TicksPerQuarterNote;
        List<float> diff = new List<float>();

        foreach (var track in midiFile.Tracks)
        {
            foreach (var midiEvent in track.MidiEvents)
            {

                if (midiEvent.MidiEventType == MidiEventType.NoteOn)
                {
                    var time = midiEvent.Time;

                        ticks.Add(time);
                    // Ici, créer une liste des c après boucle puis faire apparaître notes en fonction des c
                    Debug.Log("Lane " + TappingKey + " Time = " + midiEvent.Time);
                }

            }
            for(int i = 0; i < ticks.Count-1; i++)
            {
                diff.Add(ticks[i + 1] - ticks[i]);
            }
        }
        Debug.Log("Count = " + ticks.Count);

        return diff;
    }
}
