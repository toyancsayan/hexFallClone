using UnityEngine;

public class MarkerManager : MonoBehaviour
{
    GameObject m_marker;
    Vector3 rot = new Vector3();
    int rotInt;
    Vector2 initialPos= new Vector3();

 public void Init(GameObject marker)
    {
        m_marker = marker;
        initialPos = m_marker.transform.position;

        HexManager.SwipeSuccesAction += Deselect;
    }


    private void OnDisable()
    {
        HexManager.SwipeSuccesAction -= Deselect;
    }
    public void Selected(MarkerObject markerObject)
    {
        rotInt = 0;

        if (!markerObject.IsUp)
            rotInt = 180;
       
        rot = new Vector3(0, 0 + rotInt, 0);
        m_marker.transform.rotation = Quaternion.Euler(rot);
        m_marker.transform.position = markerObject.transform.position;

    }

    void Deselect()
    {
        m_marker.transform.position = initialPos;
    }
}
