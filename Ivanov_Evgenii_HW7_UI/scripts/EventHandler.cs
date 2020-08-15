using UnityEngine;
using TMPro;
using System.Text;
using System.Text.RegularExpressions;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Game2D
{
    public sealed class EventHandler : MonoBehaviour
    {
        private static float AMMO_OFFSET_MIN = 5.0f;
        private static float AMMO_OFFSET_MAX = 150.0f;
        private static float AMMO_BAR_LENGTH = 145.0f;

        [SerializeField] private GameObject _player;
        [SerializeField] private GameObject _healthBar;
        [SerializeField] private GameObject _ammoBar;
        [SerializeField] private GameObject _eventPanel;
        [SerializeField] private float _msgTimeout;
        [SerializeField] private float _playerFullHealth;

        private InventoryControl _inventoryControl;
        private MainMenuHandler _menuHandler;
        private Dictionary<float, TextMeshProUGUI> _hpTitles = new Dictionary<float, TextMeshProUGUI>();

        private Item _currentWeapon;
        private Item _oldWeapon;
        private RectTransform _ammoBarTransform;
        private Vector2 _ammoBarOffset;
        private TextMeshProUGUI _playerHeadTitle;
        private GameObject _headTitleContainer;
        private EventType _triggeredEvent;

        private Regex _patternForNameAndDamage = new Regex(@"([a-zA-Z]{1,15}) ([0-9]{1,5})");
        private Regex _patternForKilledAndKiller = new Regex(@"([a-zA-Z]{1,15}) ([0-9]{1,5})");

        private string _describedEventMessage;
        private bool _isEventTriggered;
        private bool _needRecalcAmmoBar;
        private int _ammoMaxForCurrentWeapon;
        private int _magazine;

        private float _killedTimer;

        private void Start()
        {
            _headTitleContainer = GameObject.FindGameObjectWithTag("HeadTitle");
            _playerHeadTitle = _headTitleContainer.GetComponent<TextMeshProUGUI>();
            _playerHeadTitle.enabled = false;
            _inventoryControl = _player.GetComponent<InventoryControl>();
            _menuHandler = GameObject.FindGameObjectWithTag("EventSystem").GetComponent<MainMenuHandler>();
            ItemUtil.InitializeMatcher();
            _ammoBarTransform = _ammoBar.GetComponent<RectTransform>();
            UpdateAmmoBar();
        }

        private void Update()
        {
            if (_isEventTriggered)
            {
                ProcessEventHandling(_triggeredEvent, _describedEventMessage);
            }

            if (_needRecalcAmmoBar)
            {
                UpdateAmmoBar();
            }

            if (_hpTitles.Count > 0)
            {
                _hpTitles.Keys.ToList().ForEach(e => {
                    e += Time.deltaTime;

                    if (e >= _msgTimeout)
                    {
                        TextMeshProUGUI title;
                        _hpTitles.TryGetValue(e, out title);
                        DisableTextTitle(title);
                        _hpTitles.Remove(e);
                    }

                });
            }
        }

        public void TriggerEvent(EventType type, string describedEventMessage)
        {
            _triggeredEvent = type;
            _describedEventMessage = describedEventMessage;
            _isEventTriggered = true;
        }

        private void ProcessEventHandling(EventType type, string describedEventMessage)
        {
            _isEventTriggered = false;

            switch (type)
            {
                case EventType.DMG_RECEIVE:
                    float dmgReceive;
                    float.TryParse(describedEventMessage, out dmgReceive);

                    UpdatePlayerHeadTitle(dmgReceive);
                    UpdateHealthBar(dmgReceive);
                    UpdateEventPanel($"DAMAGE RECEIVE! {describedEventMessage} !");
                    break;

                case EventType.DMG_ENEMY:
                    var dmg = float.Parse(_patternForNameAndDamage.Match(describedEventMessage).Groups[2].Value);
                    var enemyTag = _patternForNameAndDamage.Match(describedEventMessage).Groups[1].Value;
                    var enemy = GameObject.FindGameObjectWithTag(enemyTag);
                    UpdateEnemyHeadTitle(enemy, dmg);
                    UpdateEventPanel($"ENEMY {enemy.name} RECEIVE! {dmg} !");
                    break;

                case EventType.ITEM_RECEIVE:
                    UpdateEventPanel($"Find! And Receive the {describedEventMessage}");
                    break;

                case EventType.AMMO_DISCHARGE:
                    UpdateAmmoBar();
                    break;

                case EventType.SMB_KILLED:
                    UpdateEventPanel(describedEventMessage, _killedTimer);
                    break;

                case EventType.INFO:
                    UpdateEventPanel(describedEventMessage);
                    break;

                default:
                    break;
            }
        }

        private void DisableTextTitle(TextMeshProUGUI title)
        {
            title.enabled = false;
        }

        private void UpdatePlayerHeadTitle(float inputDamage)
        {
            _hpTitles.Add(0.1f, _playerHeadTitle);

            _playerHeadTitle.enabled = true;
            _playerHeadTitle.SetText($"-{inputDamage}HP");
        }

        private void UpdateEnemyHeadTitle(GameObject enemy, float damage)
        {
            var enemyHeadTitle = enemy.GetComponentInChildren<TextMeshProUGUI>();
            _hpTitles.Add(0.0f, enemyHeadTitle);

            enemyHeadTitle.enabled = true;
            enemyHeadTitle.SetText($"-{damage}HP");
        }

        private void UpdateEventPanel(string describedEventMessage)
        {
            var msgBox = _eventPanel.GetComponentInChildren<TextMeshProUGUI>();
            StringBuilder sb = new StringBuilder();
            sb.AppendLine(describedEventMessage)
                .AppendLine()
                .AppendLine(msgBox.text);
            msgBox.text = sb.ToString();
        }

        private void UpdateEventPanel(string describedEventMessage, float timer)
        {
            if (timer <= 3.0f)
            {
                var killedTag = _patternForKilledAndKiller.Match(describedEventMessage).Groups[1].Value;
                var killed = GameObject.FindGameObjectWithTag(killedTag);
                var killer = _patternForKilledAndKiller.Match(describedEventMessage).Groups[2].Value;

                if (killedTag.Equals("Player"))
                {
                    UpdateEventPanel($"YOU LOOSE {killed.name.ToUpper()}. YOU was killed by {killer.ToUpper()}! \n You can start again, but I think you too week for this shit!");
                    timer += Time.deltaTime;
                }
                else
                {
                    UpdateEventPanel($"{killed.name.ToUpper()} WAS KILLED BY {killer.ToUpper()}! \n CONGRATS!");
                }
            }
            else
            {
                _menuHandler.PauseGame();
            }
        }

        private void UpdateHealthBar(float inputDamage)
        {
            UnityEngine.UI.Image img = _healthBar.GetComponent<UnityEngine.UI.Image>();
            img.fillAmount -= inputDamage / _playerFullHealth;
        }

        private void UpdateAmmoBar()
        {
            _needRecalcAmmoBar = false;
            _currentWeapon = _inventoryControl.CurrentItem;

            if (_currentWeapon != null)
            {
                if ((_currentWeapon.GameItem.activeSelf && _magazine == 0 || _magazine == _ammoMaxForCurrentWeapon))
                {
                    _magazine = _currentWeapon.GameItem.GetComponent<IDamagable>().Magazine;
                    var type = ItemUtil.DetermineMagazineTypeByWeapon(_currentWeapon);
                    _ammoMaxForCurrentWeapon = (int)type;
                    RenderFullAmmoBar();
                }
                else if (_currentWeapon.GameItem.activeSelf && _magazine > 0)
                {
                    RederAmmoBarWithinShots(_ammoMaxForCurrentWeapon - _magazine);
                }

                if (_triggeredEvent.Equals(EventType.AMMO_DISCHARGE))
                {
                    RederAmmoBarWithinShots(1);
                    _triggeredEvent = EventType.INFO;
                }

                if (!_currentWeapon.GameItem.activeSelf)
                {
                    RenderMinAmmoBar();
                }
            }
            else if (_currentWeapon == null)
            {
                RenderMinAmmoBar();
            }
        }

        private void RederAmmoBarWithinShots(int bulletsLeft)
        {
            if (bulletsLeft < 0)
            {
                throw new Exception("Expected bullets left > 0");
            }
            else if (bulletsLeft == 0)
            {
                RenderMinAmmoBar();
            }

            _ammoBarOffset.x = _ammoBarOffset.x - (AMMO_BAR_LENGTH * bulletsLeft) / _ammoMaxForCurrentWeapon;
            _ammoBarTransform.offsetMax = _ammoBarOffset;
        }

        private void RenderMinAmmoBar()
        {
            var offset = _ammoBarTransform.offsetMax;
            offset.x = -AMMO_OFFSET_MAX;
            _ammoBarTransform.offsetMax = offset;
        }

        private void RenderFullAmmoBar()
        {
            _ammoBarOffset = _ammoBarTransform.offsetMax;
            _ammoBarOffset.x = -AMMO_OFFSET_MIN;
            _ammoBarTransform.offsetMax = _ammoBarOffset;
        }

        public void TriggerAmmoBarRecalc()
        {
            _needRecalcAmmoBar = true;
        }
    }
}