using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Expense
{
    public int Count;
    public int CostPerCount;
}

public class ExpenseWatcher : MonoBehaviour
{
    protected float StartTime;
    public static Dictionary<string, Expense> ChargeCounts = new Dictionary<string, Expense>();
    void Start() {
        StartTime = Time.time;
        for (int i = Random.Range(5, 30); i > 0; i--)
        {
            ReportCharge("Villager relocation fee", 24575);
        }
        for (int i = Random.Range(1, 4); i > 0; i--)
        {
            ReportCharge("Side quest surcharge", 75399);
        }
        ReportCharge("Uniform dry cleaning", 8294);
        ReportCharge("Interdimensional transit fee", 34);
        ReportCharge("Timeline alteration tax", 31);
        ReportCharge("Deposit", -100000);

        PlayerController.OnRespawn += WatchRespawn;
        DestroyOnHit.OnDestroy += WatchDestroy;
        Stat.OnLevelUp += WatchLevelUp;
    }

    protected void ReportCharge(string Charge, int Cost, int Count)
    {
        if (!ChargeCounts.ContainsKey(Charge))
        {
            ChargeCounts[Charge] = new Expense(){CostPerCount = Cost, Count = Count};
        }
        else
        {
            ChargeCounts[Charge].Count += Count;
        }
    }

    protected void ReportCharge(string Charge, int Cost)
    {
        if (!ChargeCounts.ContainsKey(Charge))
        {
            ChargeCounts[Charge] = new Expense(){CostPerCount = Cost, Count = 1};
        }
        else
        {
            ChargeCounts[Charge].Count += 1;
        }
    }

    protected void WatchRespawn(ColliderType Killer)
    {
        ReportCharge("Agent trauma", 15840);
        ReportCharge("Medical surcharge", 2256);
        ReportCharge("Armor polishing", 2175);

        switch (Killer)
        {
            case ColliderType.Arrow:
            {
                for (int i = Random.Range(5, 30); i > 0; i--)
                {
                    ReportCharge("Stitches", 650);
                }
                for (int i = Random.Range(0, 6); i > 0; i--)
                {
                    ReportCharge("Splinter removal", 230);
                }
                ReportCharge("Arrow removal", 350);
                break;
            }
            case ColliderType.Monster:
            {
                for (int i = Random.Range(1, 4); i > 0; i--)
                {
                    ReportCharge("Limb Regrowth", 306322);
                }
                ReportCharge("Burn treatment", 30781);
                break;
            }
            case ColliderType.SpeedPit:
            {
                ReportCharge("Acid neutralization", 21);
                ReportCharge("Ammortized mutation insurance", 600431);
                for (int i = Random.Range(7, 25); i > 0; i--)
                {
                    ReportCharge("Wart removal", 6244);
                }
                break;
            }
            case ColliderType.Spike:
            {
                ReportCharge("Generalized head trauma", 37122);
                if (Random.Range(0f, 1f) > 0.3f)
                {
                    ReportCharge("Suicide counseling", 2741);
                }
                break;
            }
            case ColliderType.Wall:
            {
                ReportCharge("Whole body reconstruction", 47122384);
                ReportCharge("Wall cleaning", 6197);
                ReportCharge("Paint matching", 251);
                break;
            }
        }
    }

    protected void WatchDestroy(string tag)
    {
        switch (tag)
        {
            case "Arrow":
            {
                ReportCharge("Arrow disposal", 4273);
                break;
            }
            case "Monster":
            {
                ReportCharge("Laser creater infill", 293);
                ReportCharge("Paint matching", 251);
                ReportCharge("Wall cleaning", 6197);
                break;
            }
            case "Boss":
            {
                ReportCharge("Boss carcass removal", 74291);
                ReportCharge("Reagent reimbursement", -71);
                
                int seconds = Mathf.CeilToInt(Time.time - StartTime);
                ChargeCounts["1 second hero labor"] = new Expense(){CostPerCount = 34, Count = seconds};

                SceneManager.LoadScene(2);
                break;
            }
        }
    }

    protected void WatchLevelUp(string StatName)
    {
        ReportCharge("XP distibution fee", 50166);
        ReportCharge("XP acquisition tax", 31, Random.Range(150, 3000));
        ReportCharge($"{StatName} level", Random.Range(22570, 600000));
    }
}
