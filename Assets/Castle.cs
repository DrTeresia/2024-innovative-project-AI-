using UnityEngine;

public class Castle : MonoBehaviour
{
    public ArmyManager armyManager;

    private void Start()
    {
        if (armyManager != null)
        {
            armyManager.RegisterCastle(this);
        }
    }

    private void OnDestroy()
    {
        if (armyManager != null)
        {
            // �����ﴦ��ǳر��ݻٵ��߼�
        }
    }
}