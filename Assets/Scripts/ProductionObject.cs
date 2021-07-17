using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using CustomMethodsName;
[RequireComponent(typeof(BoxCollider))]

    public class ProductionObject : MonoBehaviour
    {
        public Arm holdingArm;
        public GameObject parent = null;
        public bool root = true;
        public BoxCollider boundingBox;
        public List<ProductionObject> connectedObjects = new List<ProductionObject>();
        public string objectName;
        void OnValidate()
        {

        }
        void Awake()
        {
            connectedObjects.Add(this);
            boundingBox.isTrigger = true;
        }
        void Update()
        {
            if (parent == null)
            {
                parent = gameObject;
            }

        }
        public void Rebound()
        {
            Bounds combinedBounds = new Bounds(transform.position, Vector3.zero);
            foreach (Renderer renderer in GetComponentsInChildren<Renderer>())
            {
                combinedBounds.Encapsulate(renderer.bounds);
            }
            boundingBox.isTrigger = true;
            boundingBox.size = combinedBounds.size;
        }
        public void ToggleCollision(bool toggled)
        {
            foreach (ProductionObject obj in connectedObjects)
            {
                if(obj != null) {
                    
                foreach (Collider col in obj.GetComponentsInChildren<Collider>())
                {
                    if (!col.isTrigger)
                    {
                        col.enabled = toggled;
                    }
                }
            }
            }
        }
        public void ReJoin(ProductionObject parentPO)
        {
            if(parentPO == null) {parentPO = this;}
            CustomMethods.CenterParent(parentPO.gameObject);
            List<ProductionObject> immediateChildren = new List<ProductionObject>();
            foreach (ProductionObject childPO in parentPO.GetComponentsInChildren<ProductionObject>())
            {
                if (childPO.transform.parent == parentPO.transform || childPO.transform == parentPO.transform)
                {
                    childPO.connectedObjects.Clear();
                    immediateChildren.Add(childPO);
                }
            }
            List<Collider> colliders = new List<Collider>();
            foreach (ProductionObject jointOne in immediateChildren)
            {
                if(jointOne != parentPO) {
                    ReJoin(jointOne);
                }
                foreach (ProductionObject jointTwo in immediateChildren)
                {
                    if (jointOne != jointTwo)
                    {
                        jointOne.connectedObjects.Add(jointTwo);
                        jointOne.connectedObjects = jointOne.connectedObjects.Distinct().ToList();
                        FixedJoint joint = jointOne.gameObject.AddComponent<FixedJoint>();
                        joint.connectedBody = jointTwo.GetComponent<Rigidbody>();
                    }
                }

            }
 
        }



    }
