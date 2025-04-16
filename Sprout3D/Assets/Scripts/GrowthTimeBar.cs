using UnityEngine;

public class GrowthTimeBar : MonoBehaviour
{
    public float barLength = 0.8f;
    private float barProgress = 1f;

    public Transform progressBar;
    public Transform progressBarBackground;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void SetBarProgress(float progress)
    {
        float xScale = Mathf.Lerp(barLength, 0, progress);
        float xPos = Mathf.Lerp(0, barLength, progress) * -0.5f;

        progressBar.transform.localScale = new Vector3(xScale, progressBar.transform.localScale.y, progressBar.transform.localScale.z);
        progressBar.transform.localPosition = new Vector3(xPos, progressBar.transform.localPosition.y, progressBar.transform.localPosition.z);
    }

    public void Overcharge()
    {
        barLength = barLength * 1.5f;
        progressBarBackground.transform.localScale = new Vector3(progressBarBackground.transform.localScale.x * 1.55f, progressBarBackground.transform.localScale.y, progressBarBackground.transform.localScale.z);
        progressBar.transform.GetComponent<SpriteRenderer>().color = Color.yellow;
    }

    public void RevertOvercharge()
    {
        barLength = barLength / 1.5f;
        progressBarBackground.transform.localScale = new Vector3(progressBarBackground.transform.localScale.x / 1.55f, progressBarBackground.transform.localScale.y, progressBarBackground.transform.localScale.z);
        progressBar.transform.GetComponent<SpriteRenderer>().color = Color.green;
    }
}
