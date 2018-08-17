using System.Collections.Generic;
using UnityEngine;

namespace KnifeGame
{
    public class PoolSystem : MonoBehaviour
    {
//        public GameObject knifePrefab; // square = knife
//        public List<DotManager> knives = new List<DotManager>(); // Squares = Knives

        public GameObject hitTargetParticlePref;
        public List<GameObject> hitTargetParticles = new List<GameObject>();

        public GameObject hitAppleParticlePref;
        public List<GameObject> hitApplePaticles = new List<GameObject>();

        private void Awake()
        {
//            PreparePools();
        }

        private void PreparePools()
        {
//            while (knives.Count < 50)
//            {
//                knives.Add(DOInstantiate(knifePrefab).GetComponent<DotManager>());
//            }

            while (hitTargetParticles.Count < 10)
            {
                hitTargetParticles.Add(DOInstantiate(hitTargetParticlePref));
            }

            while (hitApplePaticles.Count < 10)
            {
                hitApplePaticles.Add(DOInstantiate(hitAppleParticlePref));
            }
        }

        GameObject DOInstantiate(GameObject obj)
        {
            var o = Instantiate(obj);
            o.transform.parent = transform;
            o.SetActive(false);
            return o;
        }

//        public DotManager SpawnKnife(Vector3 pos, Quaternion angle, Transform parent)
//        {
//            if (knives.Count > 0)
//            {
//                var l = knives.FindAll(o => o.isEnable == false);
//
//                if (l == null || l.Count == 0)
//                {
//                    var obj = DOInstantiate(knifePrefab);
//                    knives.Add(obj.GetComponent<DotManager>());
//                    return SpawnKnife(pos, angle, parent);
//                }
//
//                l[0].transform.parent = parent;
//                l[0].transform.position = pos;
//                l[0].transform.rotation = angle;
//                l[0].gameObject.SetActive(true);
//                l[0].isEnable = true;
//                return l[0];
//            }
//
//            var ob = DOInstantiate(knifePrefab);
//            knives.Add(ob.GetComponent<DotManager>());
//            return SpawnKnife(pos, angle, parent);
//        }

        public GameObject SpawnHitTargetParticle(Vector3 pos, Quaternion angle)
        {
            if (hitTargetParticles.Count > 0)
            {
                var l = hitTargetParticles.FindAll(o => o.activeInHierarchy == false);

                if (l == null || l.Count == 0)
                {
                    var obj = DOInstantiate(hitTargetParticlePref);
                    hitTargetParticles.Add(obj);
                    return SpawnHitTargetParticle(pos, angle);
                }

                l[0].transform.position = pos;
                l[0].transform.rotation = angle;
                l[0].SetActive(true);
                return l[0];
            }

            var ob = DOInstantiate(hitTargetParticlePref);
            hitTargetParticles.Add(ob);
            return SpawnHitTargetParticle(pos, angle);
        }

        public GameObject SpawnHitAppleParticle(Vector3 pos, Quaternion angle)
        {
            if (hitApplePaticles.Count > 0)
            {
                var l = hitApplePaticles.FindAll(o => o.activeInHierarchy == false);

                if (l == null || l.Count == 0)
                {
                    var obj = DOInstantiate(hitAppleParticlePref);
                    hitApplePaticles.Add(obj);
                    return SpawnHitAppleParticle(pos, angle);
                }

                l[0].transform.position = pos;
                l[0].transform.rotation = angle;
                l[0].SetActive(true);
                return l[0];
            }

            var ob = DOInstantiate(hitTargetParticlePref);
            hitApplePaticles.Add(ob);
            return SpawnHitAppleParticle(pos, angle);
        }
    }
}