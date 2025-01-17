using System.Collections;
using SimpleCity.AI;
using TMPro;
using UnityEngine;

public class SimulationManager : MonoBehaviour
{
    [SerializeField] AiDirector aiDirector;
    [SerializeField] PlacementManager placementManager;
    [SerializeField] UIController uiController;
    [SerializeField] InputManager inputManager;
    [SerializeField] GameObject panel;
    [SerializeField] GameObject endPanel;
    [SerializeField] GameObject startButton;

    [SerializeField] TMP_Text completedJourneysText;
    [SerializeField] TMP_Text evJourneysText;
    [SerializeField] TMP_Text normalJourneysText;
    [SerializeField] TMP_Text chargeUpsText;

    [SerializeField] GameObject inputPanel;
    [SerializeField] TMP_InputField trafficIntensityInput;
    [SerializeField] TMP_InputField isElectricThresholdInput;
    [SerializeField] TMP_InputField needsChargeThresholdInput;

    public void ShowInput()
    {
        panel.SetActive(false);
        startButton.SetActive(false);
        inputPanel.SetActive(true);
        inputManager.SetEditMode(false);
    }

    public void StartSimulation()
    {
        float trafficIntensity = float.Parse(trafficIntensityInput.text) / 100;
        float isElectricThreshold = float.Parse(isElectricThresholdInput.text) / 100;
        float needsChargeThreshold = float.Parse(needsChargeThresholdInput.text) / 100;

        Debug.Log(trafficIntensity + " " + isElectricThreshold + " " + needsChargeThreshold);

        aiDirector.SetSimulationParams(trafficIntensity, isElectricThreshold, needsChargeThreshold);
        StartCoroutine(SimulationCoroutine());
        inputPanel.SetActive(false);
    }

    private IEnumerator SimulationCoroutine()
    {
        float simulationDuration = 30f;
        float elapsedTime = 0f;

        while (elapsedTime < simulationDuration)
        {
            if (aiDirector.GetActiveCarsCount() < 15)
            {
                aiDirector.SpawnACar();
            }
            yield return new WaitForSeconds(8f);
            elapsedTime += 8f;
            Debug.Log("Simulation time: " + elapsedTime);
            Debug.Log("Active cars: " + aiDirector.GetActiveCarsCount());
        }

        while (aiDirector.GetActiveCarsCount() > 0)
        {
            yield return new WaitForSeconds(1f);
        }

        Debug.Log("Simulation ended successfully");
        // edit text on endPanel that will show statistics from AiDirector
        completedJourneysText.text = "Completed Journeys: " + AiDirector.completedJourneys;
        evJourneysText.text = "EV Journeys: " + AiDirector.evJourneys;
        normalJourneysText.text = "Normal Journeys: " + AiDirector.normalJourneys;
        chargeUpsText.text = "Charge Ups: " + AiDirector.chargeUps;

        endPanel.SetActive(true);
    }

    public void OnReset()
    {
        panel.SetActive(true);
        startButton.SetActive(true);
        endPanel.SetActive(false);
        placementManager.ResetAllStructures();
        uiController.ResetButtonColor();
        inputManager.SetEditMode(true);
    }
}
