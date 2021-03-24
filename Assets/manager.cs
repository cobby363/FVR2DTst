using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class manager : MonoBehaviour
{
    [SerializeField] float growSpeed = 0.05f;
    public Image ting;
    public IEnumerator AnimateText() //typewriter effect
    {
        for (int i = 0; i < (1000); i++)
        {
            ting.gameObject.transform.localScale += new Vector3(1.92f,1.08f,0f);

            yield return new WaitForSeconds(growSpeed);
        }
    }

    private void Start()
    {
        StartCoroutine(AnimateText());
    }
}
