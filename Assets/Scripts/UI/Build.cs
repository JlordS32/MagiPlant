using UnityEngine;
using UnityEngine.UI;

public class Build : MonoBehaviour
{
    [SerializeField] GameObject _defensePrefab;

    void Start()
    {
        GetComponent<Button>().onClick.AddListener(OnClick);
    }

    void OnClick()
    {
        FindFirstObjectByType<BuildManager>().SelectPrefab(_defensePrefab);
    }
}
