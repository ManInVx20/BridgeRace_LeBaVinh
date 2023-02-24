using UnityEngine;

public class PoolableObject : MonoBehaviour, IPoolable
{
    public IObjectPool Origin { get; set; }

    public virtual void PrepareToUse()
    {
        
    }

    public virtual void ReturnToPool()
    {
        Origin.ReturnToPool(this);
    }
}
