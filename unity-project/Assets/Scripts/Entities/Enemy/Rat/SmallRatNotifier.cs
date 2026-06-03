using UnityEngine;

public class SmallRatNotifier : MonoBehaviour
{
    private RatPackComponent packLeader;

    public void RegisterLeader(RatPackComponent leader)
    {
        packLeader = leader;
    }

    private void OnDestroy()
    {
        if (packLeader != null)
            packLeader.OnPackMemberKilled(gameObject);
    }
}