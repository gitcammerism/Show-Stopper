// using System.Collections;
// using UnityEngine;
//
// public class Tile : MonoBehaviour
// {
//     public int type;    // index in tilePrefabs, used for match comparison
//     public Cell cell;   // cell this tile occupies
//
//     [SerializeField] private float moveSpeed = 8f;
//
//     public void Init(int type, Cell cell)
//     {
//         this.type = type;
//         this.cell = cell;
//     }
//     
//     // moves tile to a world position and yields until it arrives
//     public IEnumerator MoveTo(Vector3 target)
//     {
//         while ((transform.position - target).sqrMagnitude > 0.0001f)
//         {
//             transform.position = Vector3.MoveTowards(
//                 transform.position, target, moveSpeed * Time.deltaTime);
//             yield return null;
//         }
//         transform.position = target;
//     }
//
//     public IEnumerator Clear()
//     {
//         // pop animation/particles/vfx go here
//         Destroy(gameObject);
//         yield break;
//     }
// }
