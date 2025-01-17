using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace SimpleCity.AI
{
    public class AiDirector : MonoBehaviour
    {
        public PlacementManager placementManager;
        public GameObject carPrefab;
        public GameObject electricCarPrefab;

        AdjacencyGraph carGraph = new AdjacencyGraph();
        List<Vector3> carPath = new List<Vector3>();
        List<GameObject> spawnedCars = new List<GameObject>();
        public static int completedJourneys = 0;
        public static int evJourneys = 0;
        public static int normalJourneys = 0;
        public static int chargeUps = 0;

        private float trafficIntensity = 0.8f;
        private float isElectricThreshold = 0.5f;
        private float needsChargeThreshold = 0.2f;

        public void SetSimulationParams(float trafficIntensity, float isElectricThreshold, float needsChargeThreshold)
        {
            this.trafficIntensity = trafficIntensity;
            this.isElectricThreshold = isElectricThreshold;
            this.needsChargeThreshold = needsChargeThreshold;
        }

        public void RemoveCarFromList(GameObject car)
        {
            spawnedCars.Remove(car);
        }

        public void SpawnACar()
        {
            foreach (var house in placementManager.GetAllHouses())
            {
                bool spawn = UnityEngine.Random.value < trafficIntensity;
                if (!spawn)
                    continue;

                bool isElectric = UnityEngine.Random.value < isElectricThreshold;
                if (isElectric)
                {
                    bool needsCharge = UnityEngine.Random.value < needsChargeThreshold;
                    if (needsCharge)
                    {
                        TrySpawninACar(house, placementManager.GetRandomChargerStructure(), true, true);
                    }
                    else 
                    {
                        TrySpawninACar(house, placementManager.GetRandomSpecialStrucutre(), true, false);
                    }
                }
                else
                {
                    TrySpawninACar(house, placementManager.GetRandomSpecialStrucutre(), false, false);
                }   
            }
        }

        private void TrySpawninACar(StructureModel startStructure, StructureModel endStructure, bool isElectric = false, bool needsCharge = false)
        {
            Debug.Log($"Trying to spawn a car: isElectric={isElectric}, needsCharge={needsCharge}");
            if (startStructure != null && endStructure != null)
            {
                var startRoadPosition = ((INeedingRoad)startStructure).RoadPosition;
                var endRoadPosition = ((INeedingRoad)endStructure).RoadPosition;

                var path = placementManager.GetPathBetween(startRoadPosition, endRoadPosition, true);
                path.Reverse();

                if (path.Count == 0 || path.Count <= 2)
                    return;

                var startMarkerPosition = placementManager.GetStructureAt(startRoadPosition).GetCarSpawnMarker(path[1]);
                var endMarkerPosition = placementManager.GetStructureAt(endRoadPosition).GetCarEndMarker(path[path.Count - 2]);

                carPath = GetCarPath(path, startMarkerPosition.Position, endMarkerPosition.Position);

                if (carPath.Count > 0)
                {
                    var carPrefabToUse = isElectric ? electricCarPrefab : carPrefab;
                    var car = Instantiate(carPrefabToUse, startMarkerPosition.Position, Quaternion.identity);
                    var carAI = car.GetComponent<CarAI>();
                    carAI.SetPath(carPath);
                    carAI.OnCarDestroyed += RemoveCarFromList;
                    carAI.SetEVParams(isElectric, needsCharge, endStructure);
                    if (needsCharge)
                    {
                        carAI.OnChargingComplete += HandleChargingComplete;
                    }
                    spawnedCars.Add(car);
                    if (isElectric) {
                        evJourneys++;
                    }
                    else
                    {
                        normalJourneys++;
                    }
                    completedJourneys++;
                }
            }
        }

        private void HandleChargingComplete(GameObject car)
        {
            var carAI = car.GetComponent<CarAI>();
            var startStructure = carAI.charger;

            if (CarAI.chargerCarCount.ContainsKey(startStructure)){
                CarAI.chargerCarCount[startStructure]--;
            }

            if (CarAI.chargerCarCount[startStructure] <= 0)
            {
                GameObject[] cubes = GameObject.FindGameObjectsWithTag("Cube");
                foreach (GameObject cube in cubes)
                {
                    if (cube.transform.IsChildOf(startStructure.transform))
                    {
                        Debug.Log("Charging the car");
                        MeshRenderer cubeRenderer = cube.GetComponent<MeshRenderer>();
                        if (cubeRenderer != null)
                        {
                            Debug.Log("Changing the color of the cube");
                            cubeRenderer.material.color = Color.gray;
                        }
                        break;
                    }
                }
            }
            chargeUps++;

            var endStructure = placementManager.GetRandomSpecialStrucutre();
            Destroy(car);
            TrySpawninACar(startStructure, endStructure, true, false);
        }

        public int GetActiveCarsCount()
        {
            return spawnedCars.Count;
        }

        private List<Vector3> GetCarPath(List<Vector3Int> path, Vector3 startPosition, Vector3 endPosition)
        {
            carGraph.ClearGraph();
            CreatACarGraph(path);
            Debug.Log(carGraph);
            return AdjacencyGraph.AStarSearch(carGraph, startPosition, endPosition);
        }

        private void CreatACarGraph(List<Vector3Int> path)
        {
            Dictionary<Marker, Vector3> tempDictionary = new Dictionary<Marker, Vector3>();
            for (int i = 0; i < path.Count; i++)
            {
                var currentPosition = path[i];
                var roadStructure = placementManager.GetStructureAt(currentPosition);
                var markersList = roadStructure.GetCarMarkers();
                var limitDistance = markersList.Count > 3;
                tempDictionary.Clear();

                foreach (var marker in markersList)
                {
                    carGraph.AddVertex(marker.Position);
                    foreach (var markerNeighbour in marker.adjacentMarkers)
                    {
                        carGraph.AddEdge(marker.Position, markerNeighbour.Position);
                    }
                    if(marker.OpenForconnections && i + 1 < path.Count)
                    {
                        var nextRoadPosition = placementManager.GetStructureAt(path[i + 1]);
                        if (limitDistance)
                        {
                            tempDictionary.Add(marker, nextRoadPosition.GetNearestCarMarkerTo(marker.Position));
                        }
                        else
                        {
                            carGraph.AddEdge(marker.Position, nextRoadPosition.GetNearestCarMarkerTo(marker.Position));
                        }
                    }
                }
                if (limitDistance && tempDictionary.Count > 2)
                {
                    var distanceSortedMarkers = tempDictionary.OrderBy(x => Vector3.Distance(x.Key.Position, x.Value)).ToList();
                    foreach (var item in distanceSortedMarkers)
                    {
                        Debug.Log(Vector3.Distance(item.Key.Position, item.Value));
                    }
                    for (int j = 0; j < 2; j++)
                    {
                        carGraph.AddEdge(distanceSortedMarkers[j].Key.Position, distanceSortedMarkers[j].Value);
                    }
                }
            }
        }
    }
}

