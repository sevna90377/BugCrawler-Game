using UnityEngine;
using TMPro;

public class TileWithTextBehaviour : MonoBehaviour
{
    [SerializeField]
    private TMP_Text text;


    // Start is called before the first frame update
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void setText(string t)
    {
        if (text != null)
        {
            text.text = t;
        }
    }
}
