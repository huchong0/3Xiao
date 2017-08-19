using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class PanelController : MonoBehaviour {
    static public PanelController instance;
    public List<GameObject> Grids;
    private List<GameObject> Items = new List<GameObject>();
    private Item preItem = null;
    private HashSet<GameObject> ItemsToRemove = new HashSet<GameObject>();
    private bool removing = false;

    private GameObject ItemA, ItemB, ItemC, ItemD, ItemE;


    private void Awake()
    {
        
        
        instance = this;
        ItemA = Resources.Load<GameObject>("Prefabs/ItemA");
        ItemB = Resources.Load<GameObject>("Prefabs/ItemB");
        ItemC = Resources.Load<GameObject>("Prefabs/ItemC");
        ItemD = Resources.Load<GameObject>("Prefabs/ItemD");
        ItemE = Resources.Load<GameObject>("Prefabs/ItemE");


        buildMap();
    }


    private void buildMap()
    {
        
        GameObject[] row = buildRow();
        int count = 0 ;



        addRowToMap(row,count);
        count += 9;

        
        int[] preNums = new int[9];
        int[] sameFlags = new int[9];
        for(int i=0;i<9;++i)
        {
            preNums[i] = itemToInt(row[i]);
            sameFlags[i] = -1;
        }



        for(int j=1; j<9; ++j)
        {
            bool successFlag = true; //如果有任意列出现三个重复则改变此flag，重新生成该排
            int[] preNums2 = (int[])preNums.Clone();
            int[] sameFlags2 = (int[])sameFlags.Clone();


            row = buildRow();
            int[] rowArray = rowToArray(row);
            for (int i = 0; i < 9; ++i)
            {
                int kind = rowArray[i];
                if(kind == sameFlags2[i])
                {
                    successFlag = false;
                    break;
                }
                else if(kind == preNums2[i])
                {
                    sameFlags2[i] = kind;
                }
                else
                {
                    preNums2[i] = kind;
                    sameFlags2[i] = -1;
                }
            }
            if(!successFlag)
            {
                --j;
                foreach (GameObject item in row) Destroy(item);
                continue;
            }
            preNums = (int[])preNums2.Clone();
            sameFlags = (int[])sameFlags2.Clone();
            addRowToMap(row, count);
            count += 9;
        }
        

    }

    private GameObject[] buildRow()
    {
        GameObject[] result = new GameObject[9];
        int preNum = -1;
        int sameFlag = -1;

        //以preNum记录前一个数，以sameFlag记录前两个数是否已经相同，若相同则sameFlag即为那个数，若不同则为-1

        for (int i=0;i<9;++i)
        {
            int kind;
            GameObject temp = buildGrid(out kind);
            result[i] = temp;
            if (kind == sameFlag)
            {
                --i;
                Destroy(temp);
                continue;
            }
            else if (kind == preNum)
            {
                sameFlag = kind;
            }
            else
            {
                preNum = kind;
                sameFlag = -1;
            }
        }
        return result;

    }

    private GameObject buildGrid(out int kind)
    {
        GameObject result;
        int i = Random.Range(0, 5);
        switch(i)
        {
            case (0):
                result = Instantiate(ItemA);
                kind = 0;
                break;
            case (1):
                result = Instantiate(ItemB);
                kind = 1;
                break;
            case (2):
                result = Instantiate(ItemC);
                kind = 2;
                break;
            case (3):
                result = Instantiate(ItemD);
                kind = 3;
                break;
            default:
                result = Instantiate(ItemE);
                kind = 4;
                break;
        }
        return result;
    }

    private void  addRowToMap(GameObject[] row,int i)
    {
        foreach (GameObject item in row)
        {
            
            item.transform.SetParent(Grids[i].transform,false);
            item.transform.localScale = Vector3.one;
            item.transform.localPosition = Vector3.zero;
            Items.Add(item);
            
            ++i;
        } 
    }

    private int itemToInt(GameObject item)
    {
        int kind;
            switch (item.tag)
            {
                case ("ItemA"):
                    kind = 0;
                    break;
                case ("ItemB"):
                    kind = 1;
                    break;
                case ("ItemC"):
                    kind = 2;
                    break;
                case ("ItemD"):
                    kind = 3;
                    break;
                default:
                    kind = 4;
                    break;
        }
        return kind;
    }

    private int[]rowToArray(GameObject[] row)
    {
        int[] result = new int[9];
        for(int i=0;i<9;++i)
        {
            result[i] = itemToInt(row[i]);
        }
        return result;
    }

    public void clickItem(Item Item)
    {
        if (removing) return;
        removing = true;
        if (preItem == null)
        {
            preItem = Item;
            preItem.appearBorder();
        }
        else
        {
            GameObject grid1 = preItem.transform.parent.gameObject;
            GameObject grid2 = Item.transform.parent.gameObject;

            var grids = instance.Grids;
            int pos1 = grids.FindIndex(temp => temp == grid1);
            int pos2 = grids.FindIndex(temp => temp == grid2);
            var distance = Mathf.Abs(pos1 - pos2);
            if (distance == 1 || distance == 9)//两格相邻
            {
                Item.disappearBorder();
                preItem.disappearBorder();
                preItem.GetComponent<Item>().ExchangeToItem(Item);
                Items[pos1] = Item.gameObject;
                Items[pos2] = preItem.gameObject;
                if(Check())
                {
                    Invoke("Remove", 0.3f);
                }
                else
                {
                    preItem.GetComponent<Item>().ExchangeToItem(Item);
                    Items[pos2] = Item.gameObject;
                    Items[pos1] = preItem.gameObject;
                }

                preItem = null;
            }
            else
            {
                Item.disappearBorder();
                preItem.disappearBorder();
                preItem = null;
            }
        }
        removing = false;
    }

    public bool Check()
    {
        bool result = false;
        for(int i=0;i<9;++i)
        {
            int preNum = -1;
            int sameFlag = -1;

            for (int j=i*9;j<(i+1)*9;++j)
            {
                int kind = itemToInt(Items[j]);
                if (kind == sameFlag)
                {
                    result = true;
                    ItemsToRemove.Add(Items[j]);
                    ItemsToRemove.Add(Items[j-1]);
                    ItemsToRemove.Add(Items[j-2]);
                }
                else if (kind == preNum)
                {
                    sameFlag = kind;
                }
                else
                {
                    preNum = kind;
                    sameFlag = -1;
                }
            }
        }

        for (int i = 0; i < 9; ++i)
        {
            int preNum = -1;
            int sameFlag = -1;

            for (int j = i; j < i+73; j+=9)
            {
                int kind = itemToInt(Items[j]);
                if (kind == sameFlag)
                {
                    result = true;
                    ItemsToRemove.Add(Items[j]);
                    ItemsToRemove.Add(Items[j - 9]);
                    ItemsToRemove.Add(Items[j - 18]);
                }
                else if (kind == preNum)
                {
                    sameFlag = kind;
                }
                else
                {
                    preNum = kind;
                    sameFlag = -1;
                }
            }
        }

        return result;
    }

    public void Remove()
    {
        removing = true;
        foreach(GameObject Item in ItemsToRemove)
        {
            Item.tag = "Boom";
            for (int i = 0; i < 9; ++i)
            {
                int count = 0;
                for (int j = i + 72; j >=0; j -= 9)
                {
                    if(Items[j].tag=="Boom")
                    {
                        Items[j].GetComponent<Item>().DestroySelf();
                        ++count;
                    }
                    else if(count>0)
                    {
                        Items[j].GetComponent<Item>().FallToGrid(Grids[j + count * 9]);
                        Items[j + count * 9] = Items[j];
                    }
                }

                for(int j = count; j>0; --j)
                {
                    int kind;
                    GameObject temp = buildGrid(out kind);
                    temp.transform.parent = Grids[i].transform;
                    temp.transform.localScale = Vector3.one;
                    temp.transform.localPosition = new Vector3(0, 65*count, 0);

                    temp.GetComponent<Item>().FallToGrid(Grids[i + 9 * (j - 1)]);
                    Items[i + 9 * (j - 1)] = temp;
                }

            }
        }
        ItemsToRemove.Clear();
        removing = false;
        if(Check())
        {
            Invoke("Remove", 1f);
        }

    }
}
