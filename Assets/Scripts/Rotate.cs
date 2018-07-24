using UnityEngine;

public class Rotate : MonoBehaviour {
    public bool rotate;
    
    private void Update()
    {
        if (rotate) transform.Rotate(Vector3.up * 100 * Time.deltaTime);
    }
}
