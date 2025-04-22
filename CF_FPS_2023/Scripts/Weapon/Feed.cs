using UnityEngine;

public class Feed:MonoBehaviour
{
    private ItemDataConfig m_itemDataConfig;
    public bool isDynamic;//是否是被抛出的 
    public bool isInGround { get; protected set; }
    public void RegisterFeed(ItemDataConfig itemDataConfig)
    {
        m_itemDataConfig = itemDataConfig;
        isInGround = false;

    }

    public ItemDataConfig GetFeed()
    {
        gameObject.SetActive(false);
        Destroy(gameObject);
        return m_itemDataConfig;
    }
    public ItemDataConfig VisitFeed()
    {
         return m_itemDataConfig;
    }
    public void OnCollisionEnter(Collision collision)
    {
        isInGround = true;
    }
}

