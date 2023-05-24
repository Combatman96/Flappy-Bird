using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObstacleSpawner : MonoBehaviour {

	[SerializeField] private float waitTime;
	[SerializeField] private ObstacleBehaviour obstaclePrefabs;
	private float tempTime;

    private Vector3 oldAltitude = Vector3.zero;
    [SerializeField] private int difficultyCycle = 6;
    [SerializeField] private int numOfEasy = 3;
    private int cycle = 0;

    void Start(){
		tempTime = waitTime - Time.deltaTime;
		cycle = 0;
		oldAltitude = Vector3.zero;
	}

	void LateUpdate () {
		if(GameManager.Instance.GameState()){
			tempTime += Time.deltaTime;
			if(tempTime > waitTime){
				// Wait for some time, create an obstacle, then set wait time to 0 and start again
				tempTime = 0;
				var pipeClone = Instantiate(obstaclePrefabs, transform.position, transform.rotation);
                bool isHard = (cycle > numOfEasy);
                pipeClone.SetAltitude(oldAltitude, isHard);
                oldAltitude = pipeClone.GetAltitude();
                cycle++;
				if(cycle >= difficultyCycle) cycle = 0;
            }
		}
	}

	void OnTriggerEnter2D(Collider2D col){
		if(col.gameObject.transform.parent != null){
			Destroy(col.gameObject.transform.parent.gameObject);
		}else{
			Destroy(col.gameObject);
		}
	}

}
