using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class BlackholeSkillController : MonoBehaviour
{
    [SerializeField] private GameObject hotkeyPrefab;
    [SerializeField] private List<KeyCode> keyCodeList;

    public float maxSize;
    public float growSpeed;
    public bool canGrow;

    private List<Transform> targets = new List<Transform>();

    private void Update()
    {
        if (canGrow)
        {
            transform.localScale = Vector2.Lerp(transform.localScale, new Vector2(maxSize, maxSize), growSpeed * Time.deltaTime);
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.GetComponent<Enemy>() != null)
        {
            other.GetComponent<Enemy>().FreezeTime(true);
            CreateHotkey(other);
        }
    }

    private void CreateHotkey(Collider2D other)
    {
        if (keyCodeList.Count <= 0) return; // Safety measure in case we run out of key codes to spawn.

        GameObject newHotKey = Instantiate(hotkeyPrefab, other.transform.position + new Vector3(0, 2), Quaternion.identity);

        KeyCode chosenKey = keyCodeList[Random.Range(0, keyCodeList.Count)];
        keyCodeList.Remove(chosenKey);

        BlackholeHotkeyController newHotKeyScript = newHotKey.GetComponent<BlackholeHotkeyController>();

        newHotKeyScript.SetupHotkey(chosenKey, other.transform, this);
    }

    public void AddEnemyToList(Transform _enemyTransform) => targets.Add(_enemyTransform);

}
