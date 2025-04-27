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
    InventoryManager inventoryManager;
    GameManager gameManager;

    private void Start() {
        inventoryManager = GameObject.Find("GameManager").GetComponent<InventoryManager>();
        gameManager = GameObject.Find("GameManager").GetComponent<GameManager>();
    }

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
        // Money-Belt-Tan
        containerLootGroups["Money-Belt-Tan"] = new WeightedGroup[]
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
                "Crown-Grey", "Book-War-Blue", "Book-Spiritual-Blue", "Key-Orange"
            })
        };

        // Money-Belt-Orange
        containerLootGroups["Money-Belt-Orange"] = new WeightedGroup[]
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
                "Potion-Small-Blue", "Book-War-Blue", "Book-Spiritual-Blue", "Key-Orange"
            })
        };

        // Money-Belt-Blue
        containerLootGroups["Money-Belt-Blue"] = new WeightedGroup[]
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
        // Bag-Small-Tan
        containerLootGroups["Bag-Small-Tan"] = new WeightedGroup[]
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
                "Chalice-Yellow", "Crown-Grey", "Key-Orange", "Book-War-Blue", "Book-Spiritual-Blue"
            })
        };

        // Bag-Small-Orange
        containerLootGroups["Bag-Small-Orange"] = new WeightedGroup[]
        {
            // BIG
            new WeightedGroup(0.50f, new string[] {
                "Key-Tan", "Coins-Yellow", "Necklace-Yellow", "Ingot-Yellow", "Lamp-Yellow"
            }),
            // SMALL
            new WeightedGroup(0.35f, new string[] {
                "Chalice-Yellow", "Crown-Grey", "Potion-Small-Blue", "Key-Orange", "Crown-Yellow", "Chalice-White",
            }),
            // VERY SMALL
            new WeightedGroup(0.15f, new string[] {
                "Key-Blue", "Book-War-Blue", "Book-Spiritual-Blue"
            })
        };

        // Bag-Small-Blue
        containerLootGroups["Bag-Small-Blue"] = new WeightedGroup[]
        {
            // BIG
            new WeightedGroup(0.50f, new string[] {
                "Key-Tan", "Crown-Grey", "Coins-Yellow", "Necklace-Yellow",
                "Ingot-Yellow", "Lamp-Yellow", "Potion-Small-Blue"
            }),
            // SMALL
            new WeightedGroup(0.35f, new string[] {
                "Key-Orange", "Coins-White", "Necklace-White", "Ingot-White",
                "Chalice-Yellow", "Lamp-Yellow", "Book-War-Blue", "Book-Spiritual-Blue"
            }),
            // VERY SMALL
            new WeightedGroup(0.15f, new string[] {
                "Key-Blue", "Chalice-Yellow", "Lamp-White", "Crown-Yellow",
                "Book-War-Blue", "Book-Spiritual-Blue", "Bomb"
            })
        };

        //////////////////////
        /// LARGE BAG
        //////////////////////
        containerLootGroups["Bag-Large-Tan"] = new WeightedGroup[]
        {
            new WeightedGroup(0.60f, new string[] {
                "Coins-Grey", "Necklace-Grey", "Ingot-Grey", "Lamp-Grey",
                "Chalice-Grey", "Crown-Grey", "Key-Tan"
            }),
            new WeightedGroup(0.30f, new string[] {
                "Key-Orange", "Coins-Yellow", "Necklace-Yellow", "Ingot-Yellow", "Lamp-Yellow"
            }),
            new WeightedGroup(0.10f, new string[] {
                "Potion-Small-Blue", "Potion-Small-Pink",  "Book-War-Blue", "Book-Spiritual-Blue"
            })
        };

        containerLootGroups["Bag-Large-Orange"] = new WeightedGroup[]
        {
            new WeightedGroup(0.50f, new string[] {
                "Key-Orange", "Coins-Yellow", "Necklace-Yellow", "Ingot-Yellow", "Lamp-Yellow"
            }),
            new WeightedGroup(0.35f, new string[] {
                "Chalice-Yellow", "Crown-Grey", "Potion-Small-Blue", "Potion-Small-Pink",  "Book-War-Blue", "Book-Spiritual-Blue"
            }),
            new WeightedGroup(0.15f, new string[] {
                "Key-Orange", "Chalice-White", "Crown-Yellow",
                "Book-War-Pink", "Book-Spiritual-Pink", "Potion-Small-Purple", "Key-Blue"
            })
        };

        containerLootGroups["Bag-Large-Blue"] = new WeightedGroup[]
        {
            new WeightedGroup(0.50f, new string[] {
                "Key-Orange", "Coins-White", "Necklace-White", "Ingot-White", "Lamp-White", "Book-War-Pink", "Book-Spiritual-Pink"
            }),
            new WeightedGroup(0.35f, new string[] {
                "Chalice-Yellow", "Crown-Yellow", "Potion-Small-Purple", "Key-Blue", "Book-Spiritual-Blue"
            }),
            new WeightedGroup(0.15f, new string[] {
                "Key-Blue", "Book-War-Purple", "Book-Spiritual-Purple", "Bomb"
            })
        };

        //////////////////////
        /// LOCKED CONTAINERS 
        //  BOX
        //////////////////////
        containerLootGroups["Box-Tan"] = new WeightedGroup[]
        {
            new WeightedGroup(0.60f, new string[] {
                "Coins-Yellow", "Necklace-Yellow", "Ingot-Yellow", "Lamp-Yellow", "Chalice-Yellow", "Book-War-Blue", "Book-Spiritual-Blue"
            }),
            new WeightedGroup(0.30f, new string[] {
                "Crown-Grey", "Potion-Small-Blue", "Potion-Large-Blue", "Key-Orange", "Book-War-Pink", "Book-Spiritual-Pink"
            }),
            new WeightedGroup(0.10f, new string[] {
                "Book-War-Purple", "Book-Spiritual-Purple", "Bomb", "Key-Blue"
            })
        };

        containerLootGroups["Box-Orange"] = new WeightedGroup[]
        {
            new WeightedGroup(0.60f, new string[] {
                "Coins-White", "Necklace-White", "Ingot-White", "Lamp-White", "Chalice-White", "Book-War-Blue", "Book-Spiritual-Blue", "Potion-Large-Blue"
            }),
            new WeightedGroup(0.30f, new string[] {
                "Crown-Yellow", "Potion-Large-Pink", "Key-Blue", "Book-War-Pink", "Book-Spiritual-Pink", "Potion-Large-Pink"
            }),
            new WeightedGroup(0.10f, new string[] {
                "Book-War-Purple", "Book-Spiritual-Purple", "Bomb", "Potion-Large-Purple"
            })
        };

        containerLootGroups["Box-Blue"] = new WeightedGroup[]
        {
            new WeightedGroup(0.60f, new string[] {
                "Coins-White", "Necklace-White", "Ingot-White", "Lamp-White", "Chalice-White", "Book-Spiritual-Pink", "Potion-Large-Pink"
            }),
            new WeightedGroup(0.30f, new string[] {
                "Crown-White", "Potion-Large-Purple", "Book-War-Purple", "Book-Spiritual-Purple"
            }),
            new WeightedGroup(0.10f, new string[] {
                "Book-War-Purple", "Book-Spiritual-Purple", "Bomb"
            })
        };

        //////////////////////
        /// PACK
        //////////////////////
        containerLootGroups["Pack-Tan"] = new WeightedGroup[]
        {
            new WeightedGroup(0.60f, new string[] {
                "Coins-White", "Necklace-White", "Ingot-White", "Lamp-White", "Chalice-White", "Book-War-Pink", "Book-Spiritual-Pink"
            }),
            new WeightedGroup(0.30f, new string[] {
                "Crown-White", "Potion-Large-Pink", "Key-Orange", "Book-War-Purple", "Book-Spiritual-Purple", "Key-Blue"
            }),
            new WeightedGroup(0.10f, new string[] {
                "Book-Special-Blue", "Bomb"
            })
        };

        containerLootGroups["Pack-Orange"] = new WeightedGroup[]
        {
            new WeightedGroup(0.60f, new string[] {
                "Coins-White", "Necklace-White", "Ingot-White", "Lamp-White", "Chalice-White"
            }),
            new WeightedGroup(0.30f, new string[] {
                "Crown-White", "Potion-Large-Purple", "Key-Blue"
            }),
            new WeightedGroup(0.10f, new string[] {
                "Book-War-Purple",  "Book-Spiritual-Purple", "Bomb", "Book-Special-Pink"
            })
        };

        containerLootGroups["Pack-Blue"] = new WeightedGroup[]
        {
            new WeightedGroup(0.60f, new string[] {
                "Coins-White", "Necklace-White", "Ingot-White", "Lamp-White", "Chalice-White",  "Book-War-Purple", "Book-Spiritual-Purple"
            }),
            new WeightedGroup(0.30f, new string[] {
                "Crown-White", "Potion-Large-Purple", "Potion-Large-Pink", "Potion-Large-Purple"
            }),
            new WeightedGroup(0.10f, new string[] {
                "Book-Special-Blue", "Bomb", 
            })
        };

        //////////////////////
        /// CHEST
        //////////////////////
        containerLootGroups["Chest-Tan"] = new WeightedGroup[]
        {
            new WeightedGroup(0.50f, new string[] {
                "Coins-White", "Necklace-White", "Ingot-White", "Lamp-White", "Chalice-White",  "Key-Orange", "Key-Blue"
            }),
            new WeightedGroup(0.30f, new string[] {
                "Crown-White", "Potion-Large-Blue", "Book-War-Purple",  "Book-Spiritual-Purple"
            }),
            new WeightedGroup(0.20f, new string[] {
                "Book-Special-Blue", "Bomb"
            })
        };

        containerLootGroups["Chest-Orange"] = new WeightedGroup[]
        {
            new WeightedGroup(0.55f, new string[] {
                "Coins-White", "Necklace-White", "Ingot-White", "Lamp-White", "Key-Blue", "Potion-Large-Blue", "Book-Special-Blue"
            }),
            new WeightedGroup(0.30f, new string[] {
                "Crown-White", "Potion-Large-Pink", "Chalice-White", "Key-Blue",  "Book-War-Purple",  "Book-Spiritual-Pink", "Book-Special-Purple"
            }),
            new WeightedGroup(0.15f, new string[] {
                "Bomb"
            })
        };

        containerLootGroups["Chest-Blue"] = new WeightedGroup[]
        {
            new WeightedGroup(0.55f, new string[] {
                "Lamp-White", "Chalice-White", "Book-War-Purple",  "Book-Spiritual-Purple", "Potion-Large-Blue", "Book-Special-Blue", "Book-Special-Pink"
            }),
            new WeightedGroup(0.30f, new string[] {
                "Crown-White", "Potion-Large-Purple", "Potion-Large-Pink", "Book-Special-Purple", "Potion-Large-Purple"
            }),
            new WeightedGroup(0.15f, new string[] {
                "Bomb"
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
        // Clean up the containerType string if needed
        if (containerType.EndsWith(".vox(Clone)"))
        {
            containerType = containerType.Replace(".vox(Clone)", "").Trim();
        }
        else if (containerType.EndsWith(".vox"))
        {
            containerType = containerType.Replace(".vox", "").Trim();
        }

        if (!containerLootGroups.ContainsKey(containerType))
        {
            Debug.LogWarning($"No loot table for container type: {containerType}");
            return null;
        }

        // Retrieve the weighted groups for the container
        WeightedGroup[] groups = containerLootGroups[containerType];

        // Optionally creates a copy of groups with adjusted weights for usage iwth Pink small potion
        WeightedGroup[] adjustedGroups = new WeightedGroup[groups.Length];
        float boostFactor = 3.0f;  // Increase this factor as needed to fine tune the Pink Potion Small. This multples by 3 the current probability of the RAREST tier of items in each container

        for (int i = 0; i < groups.Length; i++)
        {
            // Assume the last group (i == groups.Length-1) holds the rare loot
            float adjustedWeight = groups[i].weight;
            if (gameManager.isSmallPinkPotionActive && i == groups.Length - 1)
            {
                adjustedWeight *= boostFactor;
            }
            adjustedGroups[i] = new WeightedGroup(adjustedWeight, groups[i].items);
        }

        // Sum up the adjusted weights
        float totalWeight = 0f;
        foreach (var group in adjustedGroups)
            totalWeight += group.weight;

        // Generate a random value
        float r = UnityEngine.Random.value * totalWeight;
        float cumulative = 0f;

        // Determine which group is selected
        foreach (var group in adjustedGroups)
        {
            cumulative += group.weight;
            if (r <= cumulative)
            {
                if (group.items == null || group.items.Length == 0)
                {
                    Debug.LogWarning($"WeightedGroup for {containerType} has no items!");
                    return null;
                }
                int index = UnityEngine.Random.Range(0, group.items.Length);
                Debug.Log("Contained Item:" + group.items[index]);
                inventoryManager.Spawn3DItem(group.items[index]);
                return group.items[index];
            }
        }
        return null; // Fallback (should not happen)
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