using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using System;

public class TapGameController : MonoBehaviour
{

    [SerializeField] GameObject tileObjectPrefab;
    [SerializeField] Transform startPoint;

    public MidiGameController midiController;

    /// <summary>
    /// param bool : is tile long
    /// </summary>
    private Action<bool> tileTouchedEvent;

    //Tiles collection
    [SerializeField] private List<Transform> tilesList = new List<Transform>();
    private List<Tile> currentCollisionTiles = new List<Tile>();
    private List<Tile> garbageTiles = new List<Tile>();


    [Header("Instance controls")]
    public string PathToPartition = "";

    [SerializeField] KeyCode TappingKey;



    [SerializeField] private bool useRegularInstanciation = false;
    [SerializeField] private float regularWaitBetweenInstanceInSec = 2f;

    private bool start = false;

    private bool isShowing = false;

    [SerializeField] float addedSpeed = 20;
    /// <summary>
    /// beat per second
    /// </summary>
    [SerializeField] int bpm = 60;

    private float bps = 0;

    private float laneDistance = 0;

    private int noteTotalNumber = -1;


    private bool tileInCollision = false;
    private bool tileTapped = false;

    //Double tiles

    /// <summary>
    /// Should wait for long tile
    /// </summary>
    private bool waitingForLongTileOrder = false;
    /// <summary>
    /// Key is down on long tile
    /// </summary>
    private bool longTileKeyDown = false;

    private bool nextIsLongTile = false; //Used for instanciation
    [SerializeField] private int chanceForDouble = 60;


    private bool mustCollectGarbage = false;


    //Difficulty

    [Header("Difficulty")]
    [SerializeField] Difficultylvl currentDifficulty = Difficultylvl.Easy;

    [SerializeField, Range(0, 100)] private int veryEasyRate = 20;
    [SerializeField, Range(0, 100)] private int easyRate = 30;
    [SerializeField, Range(0, 100)] private int mediumRate = 50;
    [SerializeField, Range(0, 100)] private int hardRate = 70;

    public enum Difficultylvl { VeryEasy = 0, Easy, Medium, Hard, Hell }

    //Midi calculations

    int currentNoteIdx = 0;

    List<float> noteWaintingTime = new List<float>();

    //Calculated with math
    float distance = 709.09f;

    float timeDecalage;

    [SerializeField] float speed =  0f;

    float firstWaitingTime = 0f;

    #region Test vars

    [Header("Test vars")]
    [SerializeField] private bool useRandomInstanciation = true;
    [SerializeField] float startDelayInSec = 1;

    public string midiName;

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

            if (!waitingForLongTileOrder)
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
                        longTileKeyDown = true;
                        TileLongTouch(true);
                    }
                    else
                        currentCollisionTiles.Clear();
                }
                if (Input.GetKeyUp(TappingKey))
                {
                    if (tileInCollision && longTileKeyDown)
                    {
                        tileTapped = true;
                        waitingForLongTileOrder = false;
                        TileLongTouch(false);
                    }
                    else
                        currentCollisionTiles.Clear(); //Did not go up at the right moment

                    longTileKeyDown = false;
                }
            }

            #endregion

        }


        if (mustCollectGarbage)
            CollectGarbage();
    }

    #region Init

    public TapGameController Init()
    {

        CleanAllTiles();

        Difficultylvl currentDifficulty = Difficultylvl.Easy;

        start = false;
        tileInCollision = false;
        tileTapped = false;
        waitingForLongTileOrder = false;
        longTileKeyDown = false;
        nextIsLongTile = false;

        Debug.Log("Parent" + startPoint.position);

        //this.noteWaintingTime = getTileTimeFromMidi();
        //speed = bps * addedSpeed;
        /*timeDecalage = distance / speed;
        for (int i = 0; i < this.skipNotesTo; i++)
        {
            currentNoteIdx = i;
            firstWaitingTime += noteWaintingTime[i];
        }
        firstWaitingTime -= timeDecalage;*/

        

        //LoadGame("");

        return this;
    }

    public void SetLaneEndPoint(Transform laneEndPoint)
    {
        laneDistance = Mathf.Abs(laneEndPoint.position.x - startPoint.position.x); 
    }

    public void SetSpeedFromTempo(double tempoMidi)
    {
        speed = (float)tempoMidi * addedSpeed;
    }


    /// <summary>
    /// Set the difficulty.
    /// </summary>
    /// <param name="difficulty"> range from 0 to 3</param>
    public void SetDifficulty(int difficulty)
    {
        currentDifficulty = (Difficultylvl) difficulty;
    }

    public void SetKeyCode(KeyCode keyCode)
    {
        this.TappingKey = keyCode;
    }

    public void SetMidiController(MidiGameController controller)
    {
        this.midiController = controller;
    }

    #endregion

    #region Game start / end

    public void LoadGame(string midiName/*params*/)
    {
        bool shouldUseMidi = !useRandomInstanciation && !useRegularInstanciation;

        if (shouldUseMidi)
        {
            if(midiController)
            {/*
                //midiController.LoadMidiFile(midiName);           //Load the correct file. Uncomment to set the midi file from code

                midiController.SetMidiTempo(Data.mainInstance.mainConfig.midiCorrectedTempo);
                //double bpsMidi = midiController.GetMidiBPM() / 60;


                double bpsMidi = Data.mainInstance.mainConfig.midiCorrectedTempo / 60;
                Debug.Log("Midi bpm  = " + midiController.GetMidiBPM() + " & bps = " + bpsMidi);
                speed = (float) bpsMidi * addedSpeed;
                midiController.SetStartJump(midiController.CalculateDelayFromDistance(laneDistance, speed));

                noteTotalNumber = midiController.GetNoteNumber();
*/
            }
            else
            {
                Debug.LogWarning("[TapGameController] Midi controller for lane " + TappingKey + " missing !!");
            }
        } else
        {
            bps = bpm / 60;
            speed = bps * addedSpeed;
        }
    }

    public void StartGame()
    {
        start = true;
        if (tileObjectPrefab && (useRegularInstanciation || useRandomInstanciation))
        { //StartCoroutine(InstantiateTilesCoroutines());
        }
        else
        {
            if (midiController)
            {
                midiController.SuscribeToNotePlayedEvent(InstantiateNewTile);
                midiController.PlayMidi();
            }
        }
        StartGarbageCollectionRoutine();
    }

    public TapGameController StopGame()
    {
        start = false;
        midiController?.Stop();
        midiController?.UnsuscribeToNotePlayedEvent(InstantiateNewTile);
        StopAllCoroutines();
        CleanAllTiles();
        return this;
    }

    public bool IsOngoing()
    {
        return start;
    }

    public void Show()
    {
        isShowing = true;
    }

    public void Hide()
    {
        isShowing = false;
    }

    public bool IsShowing()
    {
        return isShowing;
    }

    #endregion

    #region Tiles

    /// <summary>
    /// Decide if the tile is instanciated depending on the difficulty.
    /// The easier the difficulty is, the less probable it is for tiles to be instanciated.
    /// If the last tile is a long one, it will always be instanciated
    /// </summary>
    /// <param name="isLastNote"></param>
    /// <returns></returns>
    private bool IsTileInstanciatedFromDifficulty(bool isLastNote)
    {
        if (currentDifficulty == Difficultylvl.Hell)
            return true;
        if (isLastNote && nextIsLongTile)
            return true;
        else
        {
            int randomValue = UnityEngine.Random.Range(0, 100);
            switch(currentDifficulty)
            {
                case Difficultylvl.VeryEasy:
                    return randomValue <= veryEasyRate;
                case Difficultylvl.Easy: 
                    return randomValue <= easyRate;
                case Difficultylvl.Medium: 
                    return randomValue <= mediumRate;
                    case Difficultylvl.Hard: 
                    return randomValue <= hardRate;
                default: return true;
            }
        }
    }

    public void InstantiateNewTile(bool isLastNote)
    {
        if (IsTileInstanciatedFromDifficulty(isLastNote))
        {
            if (isLastNote)
            {
                if (nextIsLongTile) //Always finish long tile
                {
                    nextIsLongTile = false;
                    InstantiateTile(isEndLong: true);
                }
            }
            else // if difficulty allow
            {
                if (nextIsLongTile)
                {
                    nextIsLongTile = false;
                    InstantiateTile(isEndLong: true);
                }
                else if (UnityEngine.Random.Range(1, 100) <= chanceForDouble)
                {
                    //Debug.Log("Instantiate long");
                    nextIsLongTile = true;
                    InstantiateTile(isStartLong: true);
                }
                else
                {
                    InstantiateTile();
                }
            }
        }
    }


    public void InstantiateTileFromType(RythmeInterpreter.TileType tileType)
    {
        nextIsLongTile = tileType.isLongStart; // == false if is endLong or single tile
        InstantiateTile(isStartLong: tileType.isLongStart, isEndLong: tileType.isLongEnd);
    }

    /// <summary>
    /// Used for lanes that don t depends on midi files. 2 cases : regular instanciation for the marchand, 
    /// and random instanciation for tests
    /// </summary>
    /// <returns></returns>
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
        else if(useRandomInstanciation)
        {
            #region dynamic instanciation


            
            //yield return new WaitForSeconds(firstWaitingTime);

            Debug.Log("Start random instanciation coroutine");

            #region Random Instanciation for tests : if useRandomInstanciation = true

            if (useRandomInstanciation) // For tests, else set to false
            {

                yield return new WaitForSeconds(startDelayInSec);

                yield return new WaitForSeconds(UnityEngine.Random.Range(2, 7));


                while (start /* && currentNoteIdx < noteTotalNumber*/)
                {
                    InstantiateNewTile(false);

                    yield return new WaitForSeconds(UnityEngine.Random.Range(2, 7));
                }
            }
            #endregion

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
    /// <param name="wait"> Used when a long tile begin </param>
    public void TriggerTileInCollisionZone(Tile tile, bool isInCollision, bool wait = false)
    {
        tileInCollision = isInCollision;
        if (isInCollision)
        {
            waitingForLongTileOrder = wait;
            currentCollisionTiles.Add(tile);
        }
        else if (!tileTapped)
        {
            TileMissed();
            if (tile.isLong && tile.isEnd)
            {
                waitingForLongTileOrder = false; //mark end of long tile in case of miss
                longTileKeyDown = false;
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

    #endregion

    public void SuscribeToTileTouchEvent(Action<bool> callback)
    {
        tileTouchedEvent += callback;
    }


    #region GetInfo


    public float GetLaneDistance()
    {
        return laneDistance;
    }

    public bool GetIsNextTileLong()
    {
        return nextIsLongTile;
    }

    public float GetSpeed()
    {
        return speed;
    }

    public bool GetDoNotUseMidi()
    {
        return useRegularInstanciation || useRandomInstanciation;
    }

    #endregion



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
        CollectGarbage();

        foreach (Transform trans in tilesList)
        {
            if(trans != null)
                Destroy(trans.gameObject);
        }
        tilesList.Clear();

        foreach (Tile tile in currentCollisionTiles)
        {
            if(tile != null)
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

}
