using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;


namespace Game2D
{
    public sealed class InventoryControl : MonoBehaviour
    {

        #region Fields

        [SerializeField] private List<Item> _items;
        [SerializeField] private bool _isIK;

        private EventHandler _eventHandler;
        private Animator _animator;
        private GameObject _inventory;
        private GameObject _currentItem;
        private GameObject _leftPlayerHand;
        private Transform _lhPoint;
        private Vector3 _inHandRotation = new Vector3(100.0f, 0.0f, 0.0f);

        private List<Item> _weaponList;
        private List<Item> _supplyList;

        private int _inventoryPointer = 0;
        private bool _isEquipProcessing;

        #endregion


        #region Properties

        public Item this[int index]
        {
            get => this[index];
            set => this[index] = value;
        }

        public List<Item> AllPlayerItems
        {
            get => _items;
        }

        public Item CurrentItem
        {
            get
            {
                try
                {
                    if (_inventoryPointer < 0)
                    {
                        return _weaponList[0];
                    }
                    else
                    {
                        return _weaponList[_inventoryPointer];
                    }
                }
                catch (Exception)
                {
                    return null;
                }
            }
        }

        public List<Item> SupplyItems
        {
            get => _supplyList;
        }

        #endregion



        #region UnityMethods

        private void Start()
        {
            _items = new List<Item>(10);
            _inventory = GameObject.FindGameObjectWithTag("Inventory");
            _inventoryPointer = -1;
            _animator = GetComponent<Animator>();
            _leftPlayerHand = GameObject.FindGameObjectWithTag("LeftHand");
            _lhPoint = GameObject.FindGameObjectWithTag("LHPoint").transform;
            _eventHandler = GameObject.FindGameObjectWithTag("GUI").GetComponent<EventHandler>();
        }

        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.Q))
            {
                if (!_isEquipProcessing)
                {
                    ChangeWeapon();
                }
            }

            if (Input.GetKeyDown(KeyCode.R))
            {
                if (_currentItem.activeSelf || _currentItem != null || _isEquipProcessing)
                {
                    _currentItem.SetActive(false);
                    _isEquipProcessing = false;
                }
            }
        }


        private void OnTriggerEnter(Collider collision)
        {
            bool canGetItems = CheckCanTakeItem(collision);

            if (canGetItems)
            {
                var item = collision.gameObject;
                var tag = collision.gameObject.tag;

                switch (tag)
                {
                    case nameof(ItemType.Weapon):
                        AddToInventory(item, ItemType.Weapon);
                        break;

                    case nameof(ItemType.Heal):
                        AddToInventory(item, ItemType.Heal);
                        break;

                    case nameof(ItemType.Ammo):
                        AddToInventory(item, ItemType.Ammo);
                        break;

                    default:
                        break;
                }
            }
        }

        private void OnAnimatorIK()
        {
            if (_animator)
            {
                MoveItemInHandWithinIK();
            }
        }

        #endregion


        #region Methods

        private bool CheckCanTakeItem(Collider collision)
        {
            bool canGetItems = false;

            var parent = collision.gameObject.transform.parent;
            if (parent != null && parent.CompareTag("AmmoBox"))
            {
                if (parent.GetComponent<BoxesHandler>().PressedECounter % 2 == 1)
                {
                    canGetItems = true;
                }
            }
            else
            {
                canGetItems = true;
            }
            return canGetItems;
        }


        private void MoveItemInHandWithinIK()
        {
            if (_isIK)
            {
                if (_leftPlayerHand != null)
                {
                    _animator.SetIKPositionWeight(AvatarIKGoal.LeftHand, 1);
                    _animator.SetIKRotationWeight(AvatarIKGoal.LeftHand, 1);
                    _animator.SetIKPosition(AvatarIKGoal.LeftHand, _lhPoint.position);
                    _animator.SetIKRotation(AvatarIKGoal.LeftHand, _lhPoint.rotation);
                }

            }
            else
            {
                _animator.SetIKPositionWeight(AvatarIKGoal.LeftHand, 0);
                _animator.SetIKRotationWeight(AvatarIKGoal.LeftHand, 0);
                _animator.SetLookAtWeight(0);
            }
        }

        private void AddToInventory(GameObject item, ItemType type)
        {
            _eventHandler.TriggerEvent(EventType.ITEM_RECEIVE, $"{item.name}");

            bool isTwoHandle;
            int Count;
            Item wrappedItem = PrepeareGameObjectAsItem(item, type, out isTwoHandle, out Count);

            CheckExistingItemAndAddOrUpdate(wrappedItem, out Count);

            if (item.GetComponent<BoxCollider>().enabled)
            {
                item.GetComponent<BoxCollider>().enabled = false;

            }

            item.GetComponent<BoxCollider>().isTrigger = true;

            item.SetActive(false);
            MoveObjectAsChildTo(item, _inventory);

            _weaponList = _items.Where(e => e.Type.Equals(ItemType.Weapon)).ToList();
            _supplyList = _items.Where(e => e.Type.Equals(ItemType.Ammo) || e.Type.Equals(ItemType.Heal)).ToList();
        }

        private void CheckExistingItemAndAddOrUpdate(Item wrappedItem, out int Count)
        {
            Count = 0;
            var item = wrappedItem.GameItem;
            var type = wrappedItem.Type;

            var itemInList = _items.Find(e => e.Compare(wrappedItem) == true);
            if (itemInList != null)
            {
                Count = itemInList.Count;
            }

            if (Count == 0)
            {
                AddNewItemOrMagazine(wrappedItem);
            }
            else if (Count >= 1)
            {
                UpdateCountItemOrMagazine(wrappedItem, itemInList);
            }
        }

        private void UpdateCountItemOrMagazine(Item wrappedItem, Item itemInList)
        {
            var item = wrappedItem.GameItem;
            var type = wrappedItem.Type;

            var name = item.name.Split()[0];

            if (type.Equals(ItemType.Ammo))
            {
                if (name.Equals("RevolverMagazineClassic"))
                {
                    wrappedItem.Count += 6; // Base count in revolver ammo pack                 
                } 
                else if (name.Equals("RevolverMagazine"))
                {
                    itemInList.Count += 8; // Base count in revolver ammo pack                        
                }

                itemInList.Name = name;
            }
            else
            {
                itemInList.Count++;
            }
        }

        private void AddNewItemOrMagazine(Item wrappedItem)
        {
            var type = wrappedItem.Type;
            var item = wrappedItem.GameItem;

            var name = item.name.Split()[0];

            if (type.Equals(ItemType.Ammo))
            {
                if (name.Equals("RevolverMagazineClassic"))
                {
                    wrappedItem.Count = 6; // Base count in revolver ammo pack
                } 
                else if (name.Equals("RevolverMagazine"))
                {
                    wrappedItem.Count = 8; // Base count in revolver ammo pack
                }
            }
            else
            {
                wrappedItem.Count = 1;
            }
            _items.Add(wrappedItem);
        }

        private Item PrepeareGameObjectAsItem(GameObject item, ItemType type, out bool isTwoHandle, out int Count)
        {
            isTwoHandle = false;
            Count = 0;
            if (item.name.Contains("Two") || item.name.Contains("Double"))
            {
                isTwoHandle = true;
            }           
            return new Item(item.name, type, item, isTwoHandle, Count);
        }

        private void MoveObjectAsChildTo(GameObject child, GameObject parent)
        {
            child.transform.SetPositionAndRotation(parent.transform.position, parent.transform.rotation);
            child.transform.SetParent(parent.transform);
            parent.transform.rotation = Quaternion.identity;
            child.transform.rotation = Quaternion.identity;
        }

        private void ChangeWeapon()
        {
            if (_weaponList!=null && _weaponList.Count != 0)
            {
                _inventoryPointer++;
                _inventoryPointer = _inventoryPointer % _weaponList.Count;

                if (_inventoryPointer != _weaponList.Count)
                {
                    _currentItem = _weaponList[_inventoryPointer].GameItem;
                    MoveObjectAsChildTo(_currentItem, _leftPlayerHand);
                    _leftPlayerHand.GetComponent<ItemHandlerBehaviour>().ItemInHand = _weaponList[_inventoryPointer];                   
                    _animator.SetTrigger("ChangeWeapon");
                    _leftPlayerHand.GetComponent<ItemHandlerBehaviour>().IsCurrentItemChanged = true;
                }
            }
        }
        private void ChangeItemActivityWithinState()
        {
            if (_currentItem.activeSelf == false)
            {
                _currentItem.SetActive(true);
                _isEquipProcessing = true;
                _isIK = true;
                SetDefaultRotateForItemHandlers();
            }
            else
            {
                _isIK = false;
                _currentItem.SetActive(false);
                _isEquipProcessing = false;
            }
            _eventHandler.TriggerAmmoBarRecalc();
        }

        private void SetDefaultRotateForItemHandlers()
        {
            _leftPlayerHand.transform.localRotation = Quaternion.identity;
            _currentItem.transform.localRotation = Quaternion.identity;
            _currentItem.transform.Rotate(_inHandRotation);
        }

        private void ResetIsProcessingBoolWithinEndState()
        {
            _isEquipProcessing = false;
        }

        public Item GetAmmoForCurrentWeapon()
        {
            if (_currentItem == null)
            {
                return null;
            } 

            Item ammo = null;
            var referencedParams = ItemUtil.GetReferencedTypesOfItemByTagName(_currentItem);

            if (referencedParams.Count != 0)
            {
                var wtype = referencedParams.Keys.First();
                var mtype = referencedParams.Values.First();

                var allRelevantTagNames = ItemUtil.GetAllRelevantTagsNamesByItemTypes(wtype, mtype);

                foreach (var e in allRelevantTagNames)
                {
                    ammo = _supplyList.Find(s => 
                        s.GameItem.CompareTag(e) | s.GameItem.name.Contains(e)
                    );
                    if (ammo != null)
                    {
                        break;
                    }
                }
            }
            
            return ammo;
        }


        #endregion

    }
}