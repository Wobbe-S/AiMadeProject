using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class ScreenFlash : MonoBehaviour
{
    public Image flashImage;

    public float flashDuration = 0.5f;

    public IEnumerator Flash()
    {
        Debug.Log("FLASH!");
        // Show red flash
        flashImage.color = new Color(1, 0, 0, 0.5f);

        yield return new WaitForSeconds(flashDuration);

        // Hide flash
        flashImage.color = new Color(1, 0, 0, 0);
    }
}
