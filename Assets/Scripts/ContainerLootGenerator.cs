using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Runtime.CompilerServices;
using Unity.VisualScripting;
using UnityEngine;


[System.Serializable]
public class WeightedGroup
{
    public float weight;         // Probability weight for this group
    public string[] items;       // The items that belong to this group

    public WeightedGroup(float w, string[] i)
    {
        weight = w;
        items = i;
    }
}



public class ContainerLootGenerator : MonoBehaviour
{
    [SerializeField] private GameObject[] containers;   //Containers
    private Dictionary<string, GameObject> containerPrefabDict;
    Dictionary<string, WeightedGroup[]> containerLootGroups;


    private void BuildContainerPrefabDict()
    {
        containerPrefabDict = new Dictionary<string, GameObject>();

        foreach (GameObject prefab in containers)
        {
            if (prefab == null) continue;

            // Remove ".vox" if needed, otherwise just store the full name
            string cleanName = prefab.name.Replace(".vox", "");

            containerPrefabDict[cleanName] = prefab;
        }
    }


    public GameObject GenerateAContainer(int floor) 
    {
        Dictionary<string, float> containerProbabilities = new Dictionary<string, float>();

        // Define probabilities for different floors
        if (floor >= 1 && floor <= 4)
        {
            // Floors 1–4 (weaker containers)
            containerProbabilities.Add("Money-Belt-Tan", 0.50f);
            containerProbabilities.Add("Bag-Small-Tan", 0.30f);
            containerProbabilities.Add("Bag-Large-Tan", 0.20f);
        }
        else if (floor >= 5 && floor <= 8)
        {
            // Floors 5–8 (mid-tier containers)
            containerProbabilities.Add("Money-Belt-Orange", 0.25f);
            containerProbabilities.Add("Bag-Small-Orange", 0.25f);
            containerProbabilities.Add("Bag-Large-Orange", 0.20f);
            containerProbabilities.Add("Box-Tan", 0.10f);
            containerProbabilities.Add("Pack-Tan", 0.10f);
            containerProbabilities.Add("Pack-Orange", 0.10f);
        }
        else if (floor >= 9)
        {
            // Floors 9+ (best containers)
            containerProbabilities.Add("Money-Belt-Blue", 0.20f);
            containerProbabilities.Add("Bag-Small-Blue", 0.15f);
            containerProbabilities.Add("Bag-Large-Blue", 0.15f);
            containerProbabilities.Add("Box-Orange", 0.10f);
            containerProbabilities.Add("Pack-Blue", 0.10f);
            containerProbabilities.Add("Chest-Tan", 0.10f);
            containerProbabilities.Add("Chest-Orange", 0.10f);
            containerProbabilities.Add("Chest-Blue", 0.10f);
        }

        // If no containers available, return null
        if (containerProbabilities.Count == 0) return null;

        string selectedContainer = GetWeightedRandomContainer(containerProbabilities);
        
        if (selectedContainer != null && containerPrefabDict.TryGetValue(selectedContainer, out GameObject prefab))
        {
            return prefab; // Return the actual prefab
        }

        Debug.LogWarning($"No prefab found for container: {selectedContainer}");
        return null;
    }


    private string GetWeightedRandomContainer(Dictionary<string, float> containerProbabilities)
    {
        float totalWeight = 0f;

        // Sum all weights
        foreach (var entry in containerProbabilities)
            totalWeight += entry.Value;

        // Pick a random value within the total weight range
        float randomValue = UnityEngine.Random.value * totalWeight;
        float cumulativeWeight = 0f;

        // Select the container based on weighted probability
        foreach (var entry in containerProbabilities)
        {
            cumulativeWeight += entry.Value;
            if (randomValue <= cumulativeWeight)
            {
                return entry.Key; // Return the selected container name
            }
        }

        // Fallback (should never happen)
        return null;
    }


    void Awake()
    {
        InitializeLootGroups();
        BuildContainerPrefabDict();
    }


    private void InitializeLootGroups()
    {
        containerLootGroups = new Dictionary<string, WeightedGroup[]>();

        //////////////////////
        /// MONEY BELT
        //////////////////////
        // MoneyBelt-Tan
        containerLootGroups["MoneyBelt-Tan"] = new WeightedGroup[]
        {
            // BIG probability group
            new WeightedGroup(0.60f, new string[] {
                "Coins-Grey", "Necklace-Grey", "Ingot-Grey"
            }),
            // SMALL probability group
            new WeightedGroup(0.30f, new string[] {
                "Key-Tan", "Lamp-Grey", "Chalice-Grey"
            }),
            // VERY SMALL probability group
            new WeightedGroup(0.10f, new string[] {
                "Crown-Grey"
            })
        };

        // MoneyBelt-Orange
        containerLootGroups["MoneyBelt-Orange"] = new WeightedGroup[]
        {
            // BIG
            new WeightedGroup(0.50f, new string[] {
                "Coins-Grey", "Necklace-Grey", "Ingot-Grey", "Lamp-Grey"
            }),
            // SMALL
            new WeightedGroup(0.35f, new string[] {
                "Key-Tan", "Chalice-Grey", "Crown-Grey",
                "Coins-Yellow", "Necklace-Yellow", "Ingot-Yellow"
            }),
            // VERY SMALL
            new WeightedGroup(0.15f, new string[] {
                "Potion-Small-Blue"
            })
        };

        // MoneyBelt-Blue
        containerLootGroups["MoneyBelt-Blue"] = new WeightedGroup[]
        {
            // BIG
            new WeightedGroup(0.50f, new string[] {
                "Ingot-Grey", "Lamp-Grey", "Chalice-Grey", "Crown-Grey",
                "Coins-Yellow", "Necklace-Yellow", "Ingot-Yellow"
            }),
            // SMALL
            new WeightedGroup(0.35f, new string[] {
                "Potion-Small-Blue", "Key-Tan", "Key-Orange",
                "Chalice-Yellow", "Crown-Yellow", "Coins-White", "Necklace-White"
            }),
            // VERY SMALL
            new WeightedGroup(0.15f, new string[] {
                "Book-War-Blue", "Book-Spiritual-Blue"
            })
        };

        //////////////////////
        /// SMALL BAG
        //////////////////////
        // SmallBag-Tan
        containerLootGroups["SmallBag-Tan"] = new WeightedGroup[]
        {
            // BIG
            new WeightedGroup(0.60f, new string[] {
                "Coins-Grey", "Necklace-Grey", "Ingot-Grey", "Lamp-Grey", "Chalice-Grey"
            }),
            // SMALL
            new WeightedGroup(0.30f, new string[] {
                "Key-Tan", "Coins-Yellow", "Necklace-Yellow", "Ingot-Yellow", "Lamp-Yellow"
            }),
            // VERY SMALL
            new WeightedGroup(0.10f, new string[] {
                "Chalice-Yellow", "Crown-Grey"
            })
        };

        // SmallBag-Orange
        containerLootGroups["SmallBag-Orange"] = new WeightedGroup[]
        {
            // BIG
            new WeightedGroup(0.50f, new string[] {
                "Key-Tan", "Coins-Yellow", "Necklace-Yellow", "Ingot-Yellow", "Lamp-Yellow"
            }),
            // SMALL
            new WeightedGroup(0.35f, new string[] {
                "Chalice-Yellow", "Crown-Grey", "Potion-Small-Blue"
            }),
            // VERY SMALL
            new WeightedGroup(0.15f, new string[] {
                "Key-Orange", "Chalice-Yellow", "Lamp-Yellow", "Chalice-White", "Crown-Yellow",
                "Book-War-Blue", "Book-Spiritual-Blue"
            })
        };

        // SmallBag-Blue
        containerLootGroups["SmallBag-Blue"] = new WeightedGroup[]
        {
            // BIG
            new WeightedGroup(0.50f, new string[] {
                "Key-Tan", "Crown-Grey", "Coins-Yellow", "Necklace-Yellow",
                "Ingot-Yellow", "Lamp-Yellow", "Potion-Small-Blue"
            }),
            // SMALL
            new WeightedGroup(0.35f, new string[] {
                "Key-Orange", "Coins-White", "Necklace-White", "Ingot-White",
                "Chalice-Yellow", "Lamp-Yellow"
            }),
            // VERY SMALL
            new WeightedGroup(0.15f, new string[] {
                "Key-Blue", "Chalice-Yellow", "Lamp-White", "Crown-Yellow",
                "Book-War-Blue", "Book-Spiritual-Blue"
            })
        };

        //////////////////////
        /// LARGE BAG
        //////////////////////
        containerLootGroups["LargeBag-Tan"] = new WeightedGroup[]
        {
            new WeightedGroup(0.60f, new string[] {
                "Coins-Grey", "Necklace-Grey", "Ingot-Grey", "Lamp-Grey",
                "Chalice-Grey", "Crown-Grey"
            }),
            new WeightedGroup(0.30f, new string[] {
                "Key-Tan", "Coins-Yellow", "Necklace-Yellow", "Ingot-Yellow", "Lamp-Yellow"
            }),
            new WeightedGroup(0.10f, new string[] {
                "Potion-Small-Blue", "Potion-Small-Pink"
            })
        };

        containerLootGroups["LargeBag-Orange"] = new WeightedGroup[]
        {
            new WeightedGroup(0.50f, new string[] {
                "Key-Tan", "Coins-Yellow", "Necklace-Yellow", "Ingot-Yellow", "Lamp-Yellow"
            }),
            new WeightedGroup(0.35f, new string[] {
                "Chalice-Yellow", "Crown-Grey", "Potion-Small-Blue", "Potion-Small-Pink"
            }),
            new WeightedGroup(0.15f, new string[] {
                "Key-Orange", "Chalice-White", "Crown-Yellow",
                "Book-War-Pink", "Book-Spiritual-Pink", "Potion-Small-Purple"
            })
        };

        containerLootGroups["LargeBag-Blue"] = new WeightedGroup[]
        {
            new WeightedGroup(0.50f, new string[] {
                "Key-Orange", "Coins-White", "Necklace-White", "Ingot-White", "Lamp-White"
            }),
            new WeightedGroup(0.35f, new string[] {
                "Chalice-Yellow", "Crown-Yellow", "Potion-Small-Purple"
            }),
            new WeightedGroup(0.15f, new string[] {
                "Key-Blue", "Book-War-Purple", "Book-Spiritual-Purple"
            })
        };

        //////////////////////
        /// BOX
        //////////////////////
        containerLootGroups["Box-Tan"] = new WeightedGroup[]
        {
            new WeightedGroup(0.60f, new string[] {
                "Coins-Yellow", "Necklace-Yellow", "Ingot-Yellow", "Lamp-Yellow", "Chalice-Yellow"
            }),
            new WeightedGroup(0.30f, new string[] {
                "Crown-Grey", "Potion-Small-Blue", "Potion-Large-Blue"
            }),
            new WeightedGroup(0.10f, new string[] {
                "Book-War-Blue", "Book-Spiritual-Blue"
            })
        };

        containerLootGroups["Box-Orange"] = new WeightedGroup[]
        {
            new WeightedGroup(0.60f, new string[] {
                "Coins-White", "Necklace-White", "Ingot-White", "Lamp-White", "Chalice-White"
            }),
            new WeightedGroup(0.30f, new string[] {
                "Crown-Yellow", "Potion-Large-Pink"
            }),
            new WeightedGroup(0.10f, new string[] {
                "Book-War-Pink", "Book-Spiritual-Pink"
            })
        };

        containerLootGroups["Box-Blue"] = new WeightedGroup[]
        {
            new WeightedGroup(0.60f, new string[] {
                "Coins-White", "Necklace-White", "Ingot-White", "Lamp-White", "Chalice-White"
            }),
            new WeightedGroup(0.30f, new string[] {
                "Crown-White", "Potion-Large-Purple"
            }),
            new WeightedGroup(0.10f, new string[] {
                "Book-War-Purple", "Book-Spiritual-Purple"
            })
        };

        //////////////////////
        /// PACK
        //////////////////////
        containerLootGroups["Pack-Tan"] = new WeightedGroup[]
        {
            new WeightedGroup(0.60f, new string[] {
                "Coins-White", "Necklace-White", "Ingot-White", "Lamp-White", "Chalice-White"
            }),
            new WeightedGroup(0.30f, new string[] {
                "Crown-White", "Potion-Large-Pink"
            }),
            new WeightedGroup(0.10f, new string[] {
                "Book-War-Pink"
            })
        };

        containerLootGroups["Pack-Orange"] = new WeightedGroup[]
        {
            new WeightedGroup(0.60f, new string[] {
                "Coins-White", "Necklace-White", "Ingot-White", "Lamp-White", "Chalice-White"
            }),
            new WeightedGroup(0.30f, new string[] {
                "Crown-White", "Potion-Large-Purple"
            }),
            new WeightedGroup(0.10f, new string[] {
                "Book-War-Purple"
            })
        };

        containerLootGroups["Pack-Blue"] = new WeightedGroup[]
        {
            new WeightedGroup(0.60f, new string[] {
                "Coins-White", "Necklace-White", "Ingot-White", "Lamp-White", "Chalice-White"
            }),
            new WeightedGroup(0.30f, new string[] {
                "Crown-White", "Potion-Large-Purple"
            }),
            new WeightedGroup(0.10f, new string[] {
                "Book-Special-Blue"
            })
        };

        //////////////////////
        /// CHEST
        //////////////////////
        containerLootGroups["Chest-Tan"] = new WeightedGroup[]
        {
            new WeightedGroup(0.60f, new string[] {
                "Coins-White", "Necklace-White", "Ingot-White", "Lamp-White", "Chalice-White"
            }),
            new WeightedGroup(0.30f, new string[] {
                "Crown-White", "Potion-Large-Purple"
            }),
            new WeightedGroup(0.10f, new string[] {
                "Book-Special-Pink"
            })
        };

        containerLootGroups["Chest-Orange"] = new WeightedGroup[]
        {
            new WeightedGroup(0.60f, new string[] {
                "Coins-White", "Necklace-White", "Ingot-White", "Lamp-White", "Chalice-White"
            }),
            new WeightedGroup(0.30f, new string[] {
                "Crown-White", "Potion-Large-Purple"
            }),
            new WeightedGroup(0.10f, new string[] {
                "Book-Special-Purple"
            })
        };

        containerLootGroups["Chest-Blue"] = new WeightedGroup[]
        {
            new WeightedGroup(0.50f, new string[] {
                "Coins-White", "Necklace-White", "Ingot-White", "Lamp-White", "Chalice-White"
            }),
            new WeightedGroup(0.30f, new string[] {
                "Crown-White", "Potion-Large-Purple", "Key-Blue"
            }),
            new WeightedGroup(0.20f, new string[] {
                "Book-Special-Blue"
            })
        };
    }

    /// <summary>
    /// Returns ONE random item from the container’s WeightedGroups,
    /// picking which group based on group.weight, then picking
    /// a random item from that group’s array.
    /// </summary>
    public string GetRandomItem(string containerType)
    {
        if (!containerLootGroups.ContainsKey(containerType))
        {
            Debug.LogWarning($"No loot table for container type: {containerType}");
            return null;
        }

        WeightedGroup[] groups = containerLootGroups[containerType];

        // 1) Sum all group weights
        float totalWeight = 0f;
        foreach (var g in groups)
            totalWeight += g.weight;

        // 2) Pick a random point
        float r = UnityEngine.Random.value * totalWeight;

        // 3) Find which group we fall into
        float cumulative = 0f;
        foreach (var g in groups)
        {
            cumulative += g.weight;
            if (r <= cumulative)
            {
                // Pick a random item from this group
                if (g.items == null || g.items.Length == 0)
                {
                    Debug.LogWarning($"WeightedGroup for {containerType} has no items!");
                    return null;
                }
                int index = UnityEngine.Random.Range(0, g.items.Length);
                return g.items[index];
            }
        }

        // Fallback (should never happen if weights are set)
        return null;
    }
}

    //////   https://docs.google.com/spreadsheets/d/1jqqI34sGs0kx1J_uOj5Jc2wu2Sv4D0Um0ZMVFC0thjk/edit?gid=0#gid=0
    ///////////////
    /////MONEY BELT
    ///////////////
    /// if (Money Belt Tan) {
    ///     BIG PROBABILITY
    //      Coins gret, Necklace grey, Ingot grey, 
    //      
    //      SMALL PROBABILITY     
    //      Key tan, Lamp grey, Chalice grey
    //
    //      VERY SMALL PROBABILITY
    //      Crown grey
    //
    /// }
    /// if (Money Belt Orange) {
    ///     BIG PROBABILITY
    ///     Coins Gray, Necklace Gray , Ingot Gray , Lamp Gray, 
    /// 
    ///     SMALL PROBABILITY  
    ///     Key tan, Chalice Gray, Crown Gray, Coins Yellow, Neckalce Yellow, Ingot Yellow
    ///     
    ///     VERY SMALL PROBABILITY
    ///     Small blue potion
    /// }
    /// if (Money Belt Blue) {
    ///     BIG PROBABILITY
    ///     Ingot Gray , Lamp Gray, Chalice Gray, Crown Gray, Coins Yellow, Neckalce Yellow, Ingot Yellow
    /// 
    ///     SMALL PROBABILITY  
    ///     Small blue potion, Key tan, Key orange, Chalice Yellow, Crown Yellow, Coins white, Necklace white
    ///     
    ///     VERY SMALL PROBABILITY
    ///     War book blue, Spiritual book blue
    /// }
    /// /////////////
    /// SMALL BAG ///
    /// /////////////
    /// if (Small Bag Tan) {
    ///     BIG PROBABILITY
    //      Coins grey, Necklace grey, Ingot grey, Lamp grey, Chalice grey, 
    //      
    //      SMALL PROBABILITY     
    //      Key tan, Coins Yellow, Neckalce Yellow, Ingot Yellow, Lamp Yellow,
    //
    //      VERY SMALL PROBABILITY
    //       Challice Yellow, Crown Grey
    //
    /// }
    /// if (Small Bag Orange) {
    ///     BIG PROBABILITY
    ///     Key tan, Coins Yellow, Neckalce Yellow, Ingot Yellow, Lamp Yellow,
    /// 
    ///     SMALL PROBABILITY  
    ///     Challice Yellow, Crown Grey, Small blue potion
    ///     
    ///     VERY SMALL PROBABILITY
    ///     Key Orange, Challice yellow, Lamp Yellow, Chalice White, Crown yellow,  War book blue, Spiritual book blue,
    ///
    /// }
    /// if (Small Bag Blue) {
    ///     BIG PROBABILITY
    ///     Key Tan, Crown Grey, Coins Yellow, Neckalce Yellow, Ingot Yellow, Lamp Yellow, Small blue potion
    /// 
    ///     SMALL PROBABILITY  
    ///     Key Orange, Coins White, Neckalce White, Ingot White, Challice yellow, Lamp Yellow,
    ///     
    ///     VERY SMALL PROBABILITY
    ///     Key Blue, Challice yellow, Lamp White, Crown Yellow, War book blue, Spiritual book blue,
    /// }
///////////////
/// LARGE BAG
///////////////
// if (Large Bag Tan) {
//     BIG PROBABILITY
//         Key Tan, Coins Grey, Necklace Grey, Ingot Grey, Lamp Grey, Chalice Grey, Crown Grey

//     SMALL PROBABILITY     
//         Key Orange, Coins Yellow, Necklace Yellow, Ingot Yellow, Lamp Yellow, Chalice Yellow

//     VERY SMALL PROBABILITY
//         Key Orange, Key BlueCrown Yellow, Small Blue Potion
// }

// if (Large Bag Orange) {
//     BIG PROBABILITY
//         Key Tan, Coins Yellow, Necklace Yellow, Ingot Yellow, Lamp Yellow, Chalice Yellow, Crown Yellow

//     SMALL PROBABILITY  
//         Key Orange, Coins White, Necklace White, Ingot White, Chalice White, Crown Grey, War Book Blue, Spiritual Book Blue

//     VERY SMALL PROBABILITY
//         Key Blue, Lamp White, Chalice White, Crown White, Small Pink Potion
// }

// if (Large Bag Blue) {
//     BIG PROBABILITY
//         Key Orange, Coins White, Necklace White, Ingot White, Lamp White, Chalice White, Crown White, Small Blue Potion

//     SMALL PROBABILITY  
//         Key Blue, War Book Blue, Spiritual Book Blue, Large Blue Potion

//     VERY SMALL PROBABILITY
//         War Book Pink, Spiritual Book Pink, Special Book Blue
// }

// ///////////////
// /// BOX
// ///////////////
// if (Box Tan) {
//     BIG PROBABILITY
//         Coins Grey, Necklace Grey, Ingot Grey, Lamp Grey, Chalice Grey

//     SMALL PROBABILITY     
//         Crown Grey, Small Blue Potion, War Book Blue

//     VERY SMALL PROBABILITY
//         Large Blue Potion, Key Tan, Small Pink Potion
// }

// if (Box Orange) {
//     BIG PROBABILITY
//         Key Tan, Coins Yellow, Necklace Yellow, Ingot Yellow, Lamp Yellow, Chalice Yellow, Crown Grey

//     SMALL PROBABILITY  
//         Small Blue Potion, War Book Blue, Spiritual Book Blue, Bomb Weak

//     VERY SMALL PROBABILITY
//         Large Blue Potion, Key Orange, War Book Pink, Spiritual Book Pink, Bomb Mid
// }

// if (Box Blue) {
//     BIG PROBABILITY
//         Key Orange, Coins White, Necklace White, Ingot White, Lamp White, Chalice White, Crown White, Small Blue Potion

//     SMALL PROBABILITY  
//         Key Blue, War Book Blue, Spiritual Book Blue, Large Blue Potion, Bomb Mid

//     VERY SMALL PROBABILITY
//         War Book Pink, Spiritual Book Pink, Special Book Blue, Bomb Strong
// }

// ///////////////
// /// PACK
// ///////////////
// if (Pack Tan) {
//     BIG PROBABILITY
//         Coins Grey, Necklace Grey, Ingot Grey, Lamp Grey, Chalice Grey, Crown Grey, Small Blue Potion

//     SMALL PROBABILITY     
//         Key Tan, War Book Blue, Spiritual Book Blue, Large Blue Potion, Bomb Weak

//     VERY SMALL PROBABILITY
//         Key Orange, Small Pink Potion, War Book Pink, Spiritual Book Pink, Bomb Mid
// }

// if (Pack Orange) {
//     BIG PROBABILITY
//         Key Tan, Coins Yellow, Necklace Yellow, Ingot Yellow, Lamp Yellow, Chalice Yellow, Crown Yellow

//     SMALL PROBABILITY  
//         Key Orange, Small Blue Potion, War Book Blue, Spiritual Book Blue, Large Blue Potion, Bomb Mid

//     VERY SMALL PROBABILITY
//         Key Blue, War Book Pink, Spiritual Book Pink, Special Book Blue, Bomb Strong
// }

// if (Pack Blue) {
//     BIG PROBABILITY
//         Key Orange, Coins White, Necklace White, Ingot White, Lamp White, Chalice White, Crown White, Small Blue Potion, Large Blue Potion

//     SMALL PROBABILITY  
//         Key Blue, War Book Blue, Spiritual Book Blue, Special Book Blue, Bomb Strong

//     VERY SMALL PROBABILITY
//         Special Book Pink, War Book Purple, Spiritual Book Purple
// }

// ///////////////
// /// CHEST
// ///////////////
// if (Chest Tan) {
//     BIG PROBABILITY
//         Coins Grey, Necklace Grey, Ingot Grey, Lamp Grey, Chalice Grey, Crown Grey, Small Blue Potion, Large Blue Potion

//     SMALL PROBABILITY     
//         Key Tan, War Book Blue, Spiritual Book Blue, Special Book Blue, Bomb Weak

//     VERY SMALL PROBABILITY
//         Key Orange, War Book Pink, Spiritual Book Pink, Bomb Mid
// }

// if (Chest Orange) {
//     BIG PROBABILITY
//         Key Tan, Coins Yellow, Necklace Yellow, Ingot Yellow, Lamp Yellow, Chalice Yellow, Crown Yellow, Small Blue Potion, Large Blue Potion

//     SMALL PROBABILITY  
//         Key Orange, War Book Blue, Spiritual Book Blue, Special Book Blue, Bomb Mid

//     VERY SMALL PROBABILITY
//         Key Blue, War Book Pink, Spiritual Book Pink, Special Book Pink, Bomb Strong
// }

// if (Chest Blue) {
//     BIG PROBABILITY
//         Key Orange, Coins White, Necklace White, Ingot White, Lamp White, Chalice White, Crown White, Small Blue Potion, Large Blue Potion, Bomb Strong

//     SMALL PROBABILITY  
//         Key Blue, War Book Blue, Spiritual Book Blue, Special Book Blue, War Book Pink, Spiritual Book Pink, Special Book Pink

//     VERY SMALL PROBABILITY
//         War Book Purple, Spiritual Book Purple, Special Book Purple
// }