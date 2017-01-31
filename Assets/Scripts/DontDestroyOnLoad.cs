using UnityEngine;

public class DontDestroyOnLoad : MonoBehaviour {
	// Use this for initialization
	void Start () {
        GameObject.DontDestroyOnLoad(this);
	}
}
