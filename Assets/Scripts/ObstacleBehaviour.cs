using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObstacleBehaviour : MonoBehaviour {
	
	[SerializeField] private float moveSpeed;
	[SerializeField] private Transform center;
    [SerializeField] private Transform top;
    [SerializeField] private Transform bottom;
    [SerializeField] private Transform pipes;
    [Header("Difficulty")]
    [SerializeField] private float easyThreadhold = 0.5f;
    [SerializeField] private float offsetEasy = 1.5f;
    [SerializeField] private float difficultyThreadhold = 3f;
    [SerializeField] private float offsetDifficult = 5f;

	void Update () {
		if(GameManager.Instance.GameState()){
			// Continuosly move the obstacles to the left if the game hasn't ended
			transform.position = new Vector2(transform.position.x - Time.deltaTime * moveSpeed, transform.position.y);
		}
	}

	public void SetAltitude(Vector3 lastAltitude, bool isHard)
	{
        Vector3 altitudeOffet = Vector3.zero;
		if(!isHard)
		{
            float offsetAmount = Random.Range(easyThreadhold, offsetEasy);
			altitudeOffet = ((Random.value >= 0.5f) ? Vector3.up : Vector3.down) * offsetAmount;
        }
		else
		{
            float offsetAmount = Random.Range(difficultyThreadhold, offsetDifficult);
            altitudeOffet = (lastAltitude.y > center.localPosition.y) ? Vector3.down * offsetAmount : Vector3.up * offsetAmount;
        }

        Vector3 altitude = lastAltitude + altitudeOffet;
        altitude.y = Mathf.Clamp(altitude.y, bottom.localPosition.y, top.localPosition.y);
        pipes.localPosition = altitude;
    }

	public Vector3 GetAltitude()
	{
        return pipes.localPosition;
    }
}
