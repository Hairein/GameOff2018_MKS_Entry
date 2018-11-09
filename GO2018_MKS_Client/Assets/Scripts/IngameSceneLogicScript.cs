using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class IngameSceneLogicScript : MonoBehaviour
{
    private GameLogicScript gameLogicScriptComponent = null;

    public bool IsSelecting = false;
    public bool IsShowingSelectionRectangle = false;
    public Texture2D RectSelectionTexture;
    private Rect selectionRect = new Rect();
    private Color selectionRectColor = new Color(0.0f, 0.0f, 0.0f, 0.25f);
    private Color selectionBorderColor = new Color(1.0f, 1.0f, 1.0f, 1.0f);
    private Vector3 selectionStartPosition = Vector3.zero;
    private Vector3 selectionDragPosition = Vector3.zero;

    public int TeamNumber = 1;
    public GameObject[] SelectedUnits;

    public GameObject teamBreeder;
    public GameObject[] teamDrones;

    public GameObject team1SpawnParent;
    public GameObject team2SpawnParent;

    public bool EnableFoodDrain = true;
    public float FoodDrainPerSec = 5.0f;
    public float HungerDeathFoodLevel = -10.0f;

    public bool IgnorePointerInput = false;

    public Vector2 MapDimensions = new Vector2(48.0f, 48.0f);

    public RawImage BreederIcon;
    public RawImage[] DroneIcons;

    public Texture2D SelectedBreederIcon;
    public Texture2D UnselectedBreederIcon;
    public Texture2D SelectedDroneIcon;
    public Texture2D UnselectedDroneIcon;

    public bool CountdownRoundTime = false;
    public float RoundTimeInSeconds = 210.0f;

    public Text TimeText;
    public Text TimeTextShadow;

    // Upgrading 
    public int Team1UpgradeLevel = 0;
    public float Team1BreederMaxFoodResourceCount = 3000.0f;
    public float Team1BreederMaxTechResourceCount = 3000.0f;
    public float Team1DroneMaxFoodResourceCount = 300.0f;
    public float Team1DroneMaxTechResourceCount = 300.0f;
    public bool Team1HasUpgradedSpeed = false;
    public bool Team1HasUpgradedFoodDrain = false;
    public bool Team1HasUpgradedFoodCollect = false;
    public bool Team1HasUpgradedTechCollect = false;
    public bool Team1HasUpgradedBarricadeBuild = false;
    public bool Team1HasUpgradedBarricadeBreak = false;
    public bool Team1HasUpgradedFoodSteal = false;
    public bool Team1HasUpgradedTechSteal = false;

    public int Team2UpgradeLevel = 0;
    public float Team2BreederMaxFoodResourceCount = 3000.0f;
    public float Team2BreederMaxTechResourceCount = 3000.0f;
    public float Team2DroneMaxFoodResourceCount = 300.0f;
    public float Team2DroneMaxTechResourceCount = 300.0f;
    public bool Team2HasUpgradedSpeed = false;
    public bool Team2HasUpgradedFoodDrain = false;
    public bool Team2HasUpgradedFoodCollect = false;
    public bool Team2HasUpgradedTechCollect = false;
    public bool Team2HasUpgradedBarricadeBuild = false;
    public bool Team2HasUpgradedBarricadeBreak = false;
    public bool Team2HasUpgradedFoodSteal = false;
    public bool Team2HasUpgradedTechSteal = false;

    // ---
    public float[] FoodUpgradeCosts = { 1000.0f, 1500.0f, 2000.0f, 2500.0f };
    public float[] TechUpgradeCosts = { 1125.0f, 1125.0f, 1125.0f, 1125.0f };
    public bool ShowingTeamUpgradeChoice = false;
    public RawImage[] TeamUpgradeAChoices;
    public RawImage[] TeamUpgradeBChoices;
    public RawImage[] TeamUpgradeCChoices;
    public RawImage[] TeamUpgradeDChoices;
    public float TeamSpeedUpgradeFactor = 1.25f;

    // ~~~

    void Start()
    {
        GameObject gameLogic = GameObject.Find("GameLogic");
        if (gameLogic == null)
        {
            SceneManager.LoadScene("BootScene", LoadSceneMode.Single);
            return;
        }
        gameLogicScriptComponent = gameLogic.GetComponent<GameLogicScript>();

        BuildTeamMembers();
        UpdateUnitIcons();

        bool oldCountdownRoundTime = CountdownRoundTime;
        CountdownRoundTime = false;
        HandleRoundTime(0.0f);
        CountdownRoundTime = oldCountdownRoundTime;
    }

    void Update()
    {
        if (gameLogicScriptComponent == null)
        {
            return;
        }

        float deltaTime = Time.deltaTime;
        Vector3 mousePosition = Input.mousePosition;

        if (!IgnorePointerInput)
        {
            if (Input.GetMouseButtonDown(0))
            {
                if (!IsSelecting)
                {
                    IsSelecting = true;
                    selectionStartPosition = mousePosition;
                }
            }
            else if (Input.GetMouseButtonUp(0))
            {
                if (IsSelecting)
                {
                    FindTeamUnitsWithinSelectionRectangle();

                    IsSelecting = false;
                    IsShowingSelectionRectangle = false;
                }
            }

            if (Input.GetMouseButtonUp(1))
            {
                Vector3 clickHitPosition = Vector3.zero;
                if (GetScreenHitInWorld(mousePosition, out clickHitPosition))
                {
                    SetNavigationTarget(clickHitPosition);
                }
            }
        }

        // Check if need to show selection rectangle (4 px drag minimum)
        if(IsSelecting)
        {
            selectionDragPosition = mousePosition;

            Vector3 selectionVector = selectionDragPosition - selectionStartPosition;
            float selectionVectorLength = selectionVector.magnitude;
            IsShowingSelectionRectangle = selectionVectorLength > 4.0f;

            float minX = Mathf.Min(selectionStartPosition.x, selectionDragPosition.x);
            float minY = Mathf.Min(Screen.height -  selectionStartPosition.y, Screen.height - selectionDragPosition.y);
            float maxX = Mathf.Max(selectionStartPosition.x, selectionDragPosition.x);
            float maxY = Mathf.Max(Screen.height - selectionStartPosition.y, Screen.height - selectionDragPosition.y);

            selectionRect.Set(minX, minY, maxX - minX, maxY - minY);
        }

        HandleRoundTime(deltaTime);
        HandleUnits(deltaTime);

        HandleTeamUpgradingCapability();
    }

    private void OnGUI()
    {
        if (RectSelectionTexture == null)
        {
            return;
        }

        if (IsSelecting && IsShowingSelectionRectangle)
        {
            GUI.color = selectionRectColor;

            GUI.DrawTexture(selectionRect, RectSelectionTexture);

            GUI.color = selectionBorderColor;

            Rect topRect = new Rect(selectionRect.x, selectionRect.y, selectionRect.width, 1);
            GUI.DrawTexture(topRect, RectSelectionTexture);

            Rect bottomRect = new Rect(selectionRect.x, selectionRect.y + selectionRect.height, selectionRect.width, 1);
            GUI.DrawTexture(bottomRect, RectSelectionTexture);

            Rect rightRect = new Rect(selectionRect.x + selectionRect.width, selectionRect.y, 1, selectionRect.height);
            GUI.DrawTexture(rightRect, RectSelectionTexture);

            Rect leftRect = new Rect(selectionRect.x, selectionRect.y, 1, selectionRect.height);
            GUI.DrawTexture(leftRect, RectSelectionTexture);

            GUI.color = Color.white;
        }
    }

    private bool GetScreenHitInWorld(Vector3 screenPosition, out Vector3 hitPosition)
    {
        hitPosition = Vector3.zero;

        Ray ray = Camera.main.ScreenPointToRay(screenPosition);
        RaycastHit hit;
        if (!Physics.Raycast(ray, out hit, 1000.0f))
        {
            return false;
        }

        hitPosition = hit.point;
        return true;
    }

    private Vector3 CalculateWorldPositionFromMapOffsets(Vector2 offset)
    {
        float xOffset = (offset.x - 0.5f) * MapDimensions.x;
        float zOffset = (offset.y - 0.5f) * MapDimensions.y;

        Vector3 position = new Vector3(xOffset, 0.0f, zOffset);
        return position;
    }

    private void SetNavigationTarget(Vector3 target)
    {
        if(SelectedUnits.Length == 0)
        {
            return;
        }

        foreach (GameObject navGameObject in SelectedUnits)
        {
            NavMeshAgent agent = navGameObject.GetComponent<NavMeshAgent>();
            if (agent != null)
            {
                agent.destination = target;
                agent.isStopped = false;
            }
        }
    }

    public void HandleMapNavigation(Vector2 offset)
    {
        Vector3 worldOffset = CalculateWorldPositionFromMapOffsets(offset);
        Vector3 newTargetPosition = new Vector3(worldOffset.x, 0.0f, worldOffset.z);
        SetNavigationTarget(newTargetPosition);
    }

    public Vector3 GetCameraStartPosition()
    {
        GameObject targetGameObject;
        if (TeamNumber == 1)
        {
            targetGameObject = GameObject.Find("Team1SpawnSetup");
        }
        else
        {
            targetGameObject = GameObject.Find("Team2SpawnSetup");
        }

        if (targetGameObject == null)
        {
            return new Vector3(0.0f, 0.0f, 0.0f);
        }

        Vector3 position = targetGameObject.transform.position;
        position.y = 0.0f;
        return position;
    }

    public static Bounds GetViewportBounds(Vector3 screenPosition1, Vector3 screenPosition2)
    {
        var v1 = Camera.main.ScreenToViewportPoint(screenPosition1);
        var v2 = Camera.main.ScreenToViewportPoint(screenPosition2);
        var min = Vector3.Min(v1, v2);
        var max = Vector3.Max(v1, v2);
        min.z = Camera.main.nearClipPlane;
        max.z = Camera.main.farClipPlane;

        var bounds = new Bounds();
        bounds.SetMinMax(min, max);
        return bounds;
    }

    public void FindTeamUnitsWithinSelectionRectangle()
    {
        List<GameObject> listOfSelectedUnits = new List<GameObject>();

        Vector3 bound1 = new Vector3(selectionRect.x, Screen.height - selectionRect.y);
        Vector3 bound2 = new Vector3(selectionRect.x + selectionRect.width, Screen.height - (selectionRect.y + selectionRect.height));
        var viewportBounds = GetViewportBounds(bound1, bound2);

        GameObject[] teamUnits;
        if (TeamNumber == 1)
        {
            teamUnits = GameObject.FindGameObjectsWithTag("Team1Member");
        }
        else
        {
            teamUnits = GameObject.FindGameObjectsWithTag("Team2Member");
        }

        if(teamUnits.Length != 0)
        {
            foreach (GameObject unit in teamUnits)
            {
                UnitLogic unitLogic = unit.GetComponent<UnitLogic>();
                if(unitLogic != null)
                {
                    unitLogic.SetSelection(false);
                }

                if(viewportBounds.Contains(Camera.main.WorldToViewportPoint(unit.transform.position)))
                {
                    if (unitLogic != null)
                    {
                        unitLogic.SetSelection(true);
                    }

                    listOfSelectedUnits.Add(unit);
                }
            }
        }

        SelectedUnits = listOfSelectedUnits.ToArray();

        UpdateUnitIcons();
    }

    public void UpdateUnitIcons()
    {
        UnitLogic breederUnitLogic = teamBreeder.GetComponent<UnitLogic>();
        if(breederUnitLogic != null && breederUnitLogic.gameObject.activeSelf)
        {
            if(breederUnitLogic.IsSelected)
            {
                BreederIcon.texture = SelectedBreederIcon;
            }
            else
            {
                BreederIcon.texture = UnselectedBreederIcon;
            }
        }

        for (int index = 0; index < DroneIcons.Length; index++)
        {
            if (index < teamDrones.Length && teamDrones[index].activeSelf)
            {
                DroneIcons[index].gameObject.SetActive(true);

                DroneIcons[index].texture = UnselectedDroneIcon;
                GameObject indexedDroneUnit = teamDrones[index];
                foreach(GameObject selectedUnit in SelectedUnits)
                {
                    if (indexedDroneUnit == selectedUnit)
                    {
                        DroneIcons[index].texture = SelectedDroneIcon;
                        break;
                    }
                }
            }
            else
            {
                DroneIcons[index].gameObject.SetActive(false);
            }
        }
    }

    private void BuildTeamMembers()
    {
        GameObject[] teamMembers;
        if (TeamNumber == 1)
        {
            teamMembers = GameObject.FindGameObjectsWithTag("Team1Member");
        }
        else
        {
            teamMembers = GameObject.FindGameObjectsWithTag("Team2Member");
        }

        if(teamMembers.Length == 0)
        {
            return;
        }

        List<GameObject> drones = new List<GameObject>();
        foreach (GameObject unit in teamMembers)
        {
            BreederLogic breederLogic = unit.GetComponent<BreederLogic>();
            if (breederLogic != null && breederLogic.gameObject.activeSelf)
            {
                teamBreeder = unit;
            }
            else
            {
                DroneLogic droneLogic = unit.GetComponent<DroneLogic>();
                if (droneLogic != null && droneLogic.gameObject.activeSelf)
                {
                    drones.Add(unit);
                }
            }
        }
        teamDrones = drones.ToArray();
    }

    private void HandleRoundTime(float deltaTime)
    {
        if (CountdownRoundTime == true)
        {
            RoundTimeInSeconds -= deltaTime;
        }
        if(RoundTimeInSeconds < 0.0f)
        {
            RoundTimeInSeconds = 0.0f;
        }

        int minutes = (int)(RoundTimeInSeconds / 60.0);
        string minutesText = minutes.ToString("D2");

        int seconds = (int)(RoundTimeInSeconds % 60.0);
        string secondsText = seconds.ToString("D2");

        string timeText = string.Format(minutesText + ":" + secondsText);
        TimeText.text = timeText;
        TimeTextShadow.text = timeText;
    }

    private void HandleUnits(float deltaTime)
    {

        // Food drain teams
        HandleTeam("Team1Member", deltaTime);
        HandleTeam("Team2Member", deltaTime);
    }

    private void HandleTeam(string teamName, float deltaTime)
    {
        bool unitDidDie = false;

        // Handle food drain

        float foodDrainPerFrame = FoodDrainPerSec * deltaTime;

        GameObject[] teamMembers = GameObject.FindGameObjectsWithTag(teamName);
        foreach (GameObject unit in teamMembers)
        {
            UnitLogic unitLogic = unit.GetComponent<UnitLogic>();

            if (unitLogic.FoodResourceCount < HungerDeathFoodLevel)
            {
                // Die...
                RemoveDeadUnitFromWorld(unitLogic.gameObject);

                unitDidDie |= true;
                continue;
            }

            if (unitLogic == null)
            {
                continue;
            }

            if (EnableFoodDrain)
            {
                unitLogic.FoodResourceCount -= foodDrainPerFrame;
            }
        }

        if(unitDidDie)
        {
            BuildTeamMembers();
            UpdateUnitIcons();
        }
    }


    private void RemoveDeadUnitFromWorld(GameObject deadMember)
    {
        List<GameObject> listoFSelectedGameObjects = new List<GameObject>();

        foreach(GameObject member in SelectedUnits)
        {
            if(member != deadMember)
            {
                listoFSelectedGameObjects.Add(member);
            }
        }

        SelectedUnits = listoFSelectedGameObjects.ToArray();

        deadMember.SetActive(false);
        Destroy(deadMember);

    }

    public float GetBreederMaxFoodResource(int teamNumber)
    {
        if(teamNumber == 1)
        {
            return Team1BreederMaxFoodResourceCount;
        }
        else
        {
            return Team2BreederMaxFoodResourceCount;
        }
    }

    public float GetBreederMaxTechResource(int teamNumber)
    {
        if (teamNumber == 1)
        {
            return Team1BreederMaxFoodResourceCount;
        }
        else
        {
            return Team2BreederMaxFoodResourceCount;
        }
    }

    public float GetDroneMaxFoodResource(int teamNumber)
    {
        if (teamNumber == 1)
        {
            return Team1DroneMaxFoodResourceCount;
        }
        else
        {
            return Team2DroneMaxFoodResourceCount;
        }
    }

    public float GetDroneMaxTechResource(int teamNumber)
    {
        if (teamNumber == 1)
        {
            return Team1DroneMaxFoodResourceCount;
        }
        else
        {
            return Team2DroneMaxFoodResourceCount;
        }
    }

    private void HandleTeamUpgradingCapability()
    {
        if(Team1UpgradeLevel > 3)
        {
            return;
        }

        if (!ShowingTeamUpgradeChoice)
        {
            float nextFoodUpgradeCost = float.MaxValue;
            float nextTechUpgradeCost = float.MaxValue;
            int nextUpgradeLevel = 0;

            if (TeamNumber == 1)
            {
                nextFoodUpgradeCost = FoodUpgradeCosts[Team1UpgradeLevel];
                nextTechUpgradeCost = TechUpgradeCosts[Team1UpgradeLevel];

                nextUpgradeLevel = Team1UpgradeLevel + 1;
            }
            else
            {
                nextFoodUpgradeCost = FoodUpgradeCosts[Team2UpgradeLevel];
                nextTechUpgradeCost = TechUpgradeCosts[Team2UpgradeLevel];

                nextUpgradeLevel = Team2UpgradeLevel + 1;
            }

            UnitLogic breederUnitLogic = teamBreeder.GetComponent<UnitLogic>();
            if(breederUnitLogic != null)
            {
                if(breederUnitLogic.FoodResourceCount >= nextFoodUpgradeCost && breederUnitLogic.TechResourceCount >= nextTechUpgradeCost)
                {
                    ShowingTeamUpgradeChoice = true;
                    ShowUpgradeChoices(nextUpgradeLevel);
                }
            }
        }
    }

    private void ShowUpgradeChoices(int level)
    {
        switch(level)
        {
            case 1:
                TeamUpgradeAChoices[0].gameObject.SetActive(true);
                TeamUpgradeAChoices[1].gameObject.SetActive(true);

                TeamUpgradeBChoices[0].gameObject.SetActive(false);
                TeamUpgradeBChoices[1].gameObject.SetActive(false);
                TeamUpgradeCChoices[0].gameObject.SetActive(false);
                TeamUpgradeCChoices[1].gameObject.SetActive(false);
                TeamUpgradeDChoices[0].gameObject.SetActive(false);
                TeamUpgradeDChoices[1].gameObject.SetActive(false);
                break;
            case 2:
                TeamUpgradeAChoices[0].gameObject.SetActive(false);
                TeamUpgradeAChoices[1].gameObject.SetActive(false);

                TeamUpgradeBChoices[0].gameObject.SetActive(true);
                TeamUpgradeBChoices[1].gameObject.SetActive(true);

                TeamUpgradeCChoices[0].gameObject.SetActive(false);
                TeamUpgradeCChoices[1].gameObject.SetActive(false);
                TeamUpgradeDChoices[0].gameObject.SetActive(false);
                TeamUpgradeDChoices[1].gameObject.SetActive(false);
                break;
            case 3:
                TeamUpgradeAChoices[0].gameObject.SetActive(false);
                TeamUpgradeAChoices[1].gameObject.SetActive(false);
                TeamUpgradeBChoices[0].gameObject.SetActive(false);
                TeamUpgradeBChoices[1].gameObject.SetActive(false);

                TeamUpgradeCChoices[0].gameObject.SetActive(true);
                TeamUpgradeCChoices[1].gameObject.SetActive(true);

                TeamUpgradeDChoices[0].gameObject.SetActive(false);
                TeamUpgradeDChoices[1].gameObject.SetActive(false);
                break;
            case 4:
                TeamUpgradeAChoices[0].gameObject.SetActive(false);
                TeamUpgradeAChoices[1].gameObject.SetActive(false);
                TeamUpgradeBChoices[0].gameObject.SetActive(false);
                TeamUpgradeBChoices[1].gameObject.SetActive(false);
                TeamUpgradeCChoices[0].gameObject.SetActive(false);
                TeamUpgradeCChoices[1].gameObject.SetActive(false);

                TeamUpgradeDChoices[0].gameObject.SetActive(true);
                TeamUpgradeDChoices[1].gameObject.SetActive(true);
               break;
            default:
                TeamUpgradeAChoices[0].gameObject.SetActive(false);
                TeamUpgradeAChoices[1].gameObject.SetActive(false);
                TeamUpgradeBChoices[0].gameObject.SetActive(false);
                TeamUpgradeBChoices[1].gameObject.SetActive(false);
                TeamUpgradeCChoices[0].gameObject.SetActive(false);
                TeamUpgradeCChoices[1].gameObject.SetActive(false);
                TeamUpgradeDChoices[0].gameObject.SetActive(false);
                TeamUpgradeDChoices[1].gameObject.SetActive(false);
                break;
        }
    }

    public void MoveTeamToNextUpgradeLevel()
    {
        UnitLogic breederUnitLogic = teamBreeder.GetComponent<UnitLogic>();
        if (breederUnitLogic != null)
        {
            // Set team to next upgrade level
            if (TeamNumber == 1)
            {
                breederUnitLogic.FoodResourceCount -= FoodUpgradeCosts[Team1UpgradeLevel];
                breederUnitLogic.TechResourceCount -= TechUpgradeCosts[Team1UpgradeLevel];

                Team1UpgradeLevel++;
            }
            else
            {
                breederUnitLogic.FoodResourceCount -= FoodUpgradeCosts[Team2UpgradeLevel];
                breederUnitLogic.TechResourceCount -= TechUpgradeCosts[Team2UpgradeLevel];

                Team2UpgradeLevel++;
            }
        }

        // Hide upgrade icons
        ShowingTeamUpgradeChoice = false;
        ShowUpgradeChoices(0);
    }

    public void UpgradeSpeedPlus()
    {
        if (TeamNumber == 1)
        {
            Team1HasUpgradedSpeed = true;
        }
        else
        {
            Team2HasUpgradedSpeed = true;
        }

        UnitLogic breederUnitLogic = teamBreeder.GetComponent<UnitLogic>();
        if (breederUnitLogic != null)
        {
            breederUnitLogic.UpgradeSpeed(TeamSpeedUpgradeFactor);
        }

        foreach (GameObject droneUnit in teamDrones)
        {
            UnitLogic droneUnitLogic = droneUnit.GetComponent<UnitLogic>();
            if (droneUnitLogic != null)
            {
                droneUnitLogic.UpgradeSpeed(TeamSpeedUpgradeFactor);
            }
        }

        MoveTeamToNextUpgradeLevel();
    }

    public void UpgradeFoodDrainMinus()
    {
        MoveTeamToNextUpgradeLevel();
    }

    public void UpgradeFoodCollectPlus()
    {
        MoveTeamToNextUpgradeLevel();
    }

    public void UpgradeTechCollectPlus()
    {
        MoveTeamToNextUpgradeLevel();
    }

    public void UpgradeBarricadeBuild()
    {
        MoveTeamToNextUpgradeLevel();
    }

    public void UpgradeBarricadeBreak()
    {
        MoveTeamToNextUpgradeLevel();
    }

    public void UpgradeFoodSteal()
    {
        MoveTeamToNextUpgradeLevel();
    }

    public void UpgradeTechSteal()
    {
        MoveTeamToNextUpgradeLevel();
    }
}
