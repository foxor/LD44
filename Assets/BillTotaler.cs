using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using TMPro;

public class BillTotaler : MonoBehaviour
{
    public GameObject Bill1;
    public GameObject Bill2;
    public GameObject Bill3;
    public GameObject EntryPrefab;
    public TextMeshProUGUI Total;

    public void Start() {
        var ChargeKeys = ExpenseWatcher.ChargeCounts;
        var Entries = ChargeKeys.OrderBy(x => Random.Range(0f, 1f)).Select(x => {
            var PrefabInstance = GameObject.Instantiate<GameObject>(EntryPrefab);
            var Controller = PrefabInstance.GetComponent<BillEntryController>();
            Controller.Setup(x.Key, x.Value.Count, x.Value.CostPerCount);
            return PrefabInstance;
        }).ToArray();

        for (int i = 0; i < Entries.Length; i++) {
            var Parent = i < Entries.Length / 3 ? Bill1 : (i < Entries.Length * 0.6666f ? Bill2 : Bill3);
            Entries[i].transform.parent = Parent.transform;
        }

        long TotalCost = ExpenseWatcher.ChargeCounts.Values.Aggregate<Expense, long>(0L, (a, x) => a + ((long)x.Count) * x.CostPerCount);
        Total.text = $"Total Due: {TotalCost:C2}";
    }
}