using Managers;
using UnityEngine;

//ABSOLVED, UNSUSED

namespace Systems
{
    public class MinimapSystem : MonoBehaviour
    {
        public Camera minimapCam;
        public Transform player;
        public float padding = 1f;

        void LateUpdate()
        {
            if (!minimapCam || !player) return;

            Vector3 center = new Vector3(
                GridManager.Instance.width / 2f,
                GridManager.Instance.height / 2f,
                minimapCam.transform.position.z
            );

            minimapCam.transform.position = center;
            minimapCam.orthographicSize = Mathf.Max(GridManager.Instance.width, GridManager.Instance.height) / 2f + padding;
        }
    }
}