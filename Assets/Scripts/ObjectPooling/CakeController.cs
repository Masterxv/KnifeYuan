using System.Collections.Generic;
using System.Runtime.InteropServices.WindowsRuntime;
using UnityEngine;

namespace ObjectPooling
{
    public class CakeController : MonoBehaviour
    {
        [SerializeField] private List<GameObject> trippleTurrets;
        [SerializeField] private List<GameObject> scatterTurrets;
        
        [SerializeField] private List<GameObject> activeTurret;
        [SerializeField] private int currentTurretState;
        static readonly int material_Color = Shader.PropertyToID("_Color");
        static readonly int anim_Attack = Animator.StringToHash("attack");
        private Material _material;
        private Animator _animator;
        void Start()
        {
            _material.SetColor(material_Color, Color.white);
            _animator.SetTrigger(anim_Attack);
            UpgradeTurret();
        }

        void Update()
        {
            if (Input.GetMouseButtonDown(0))
            {
                Shoot();
            }
        }

        void Shoot()
        {
            foreach (var turret in activeTurret)
            {
                var bullet = ObjectPooler.SharedInstance.GetPooledObject("Knife");
                if (bullet != null)
                {
                    bullet.transform.position = turret.transform.position;
                    bullet.transform.rotation = turret.transform.rotation;
                    bullet.SetActive(true);
                }
            }
        }

        void UpgradeTurret()
        {
            if (currentTurretState == 0)
            {
                foreach (var turret in trippleTurrets)
                {
                    activeTurret.Add(turret);
                }
            }

            if (currentTurretState == 1)
            {
                foreach (var turret in scatterTurrets)
                {
                    activeTurret.Add(turret);
                }
            }
        }
    }
}