using System;
using System.Collections.Generic;
using UnityEngine;
using Aki.Common.Http;
using Aki.Reflection.Utils;
using Aki.SinglePlayer.Models.Healing;

namespace Aki.SinglePlayer.Utils.Healing
{
    public class HealthSynchronizer : MonoBehaviour
    {
        public bool IsEnabled = false;
        public bool IsSynchronized = false;
        float _sleepTime = 10f;
        float _timer = 0f;

        public void Update()
        {
            _timer += Time.deltaTime;

            if (_timer <= _sleepTime)
            {
                return;
            }

            _timer -= _sleepTime;

            if (IsEnabled && !IsSynchronized)
            {
                RequestHandler.PostJson("/player/health/sync", HealthListener.Instance.CurrentHealth.ToJson());
                IsSynchronized = true;
            }
        }
    }

    public class Disposable : IDisposable
    {
        private Action _onDispose;

        public Disposable(Action onDispose)
        {
            _onDispose = onDispose ?? throw new ArgumentNullException(nameof(onDispose));
        }

        public void Dispose()
        {
            _onDispose();
        }
    }

    public class HealthListener
    {
        private static HealthListener _instance;
        private IHealthController _healthController;
        private IDisposable _disposable = null;
        private HealthSynchronizer _simpleTimer;
        public PlayerHealth CurrentHealth { get; }

        public static HealthListener Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = new HealthListener();
                }

                return _instance;
            }
        }

        static HealthListener()
        {
            _ = nameof(IEffect.BodyPart);
            _ = nameof(IHealthController.HydrationChangedEvent);
            _ = nameof(DamageInfo.Weapon);
        }

        private HealthListener()
        {
            if (CurrentHealth == null)
            {
                CurrentHealth = new PlayerHealth();
            }
            
            _simpleTimer = HookObject.AddOrGetComponent<HealthSynchronizer>();
        }

        public void Init(IHealthController healthController, bool inRaid)
        {
            // cleanup
            if (_disposable != null)
            {
                _disposable.Dispose();
            }

            // init dependencies
            _healthController = healthController;
            _simpleTimer.IsEnabled = !inRaid;
            CurrentHealth.IsAlive = true;

            // init current health
            SetCurrentHealth(_healthController, CurrentHealth.Health, EBodyPart.Head);
            SetCurrentHealth(_healthController, CurrentHealth.Health, EBodyPart.Chest);
            SetCurrentHealth(_healthController, CurrentHealth.Health, EBodyPart.Stomach);
            SetCurrentHealth(_healthController, CurrentHealth.Health, EBodyPart.LeftArm);
            SetCurrentHealth(_healthController, CurrentHealth.Health, EBodyPart.RightArm);
            SetCurrentHealth(_healthController, CurrentHealth.Health, EBodyPart.LeftLeg);
            SetCurrentHealth(_healthController, CurrentHealth.Health, EBodyPart.RightLeg);

            CurrentHealth.Energy = _healthController.Energy.Current;
            CurrentHealth.Hydration = _healthController.Hydration.Current;
            CurrentHealth.Temperature = _healthController.Temperature.Current;

            // subscribe to events
            _healthController.DiedEvent += OnDiedEvent;
            _healthController.HealthChangedEvent += OnHealthChangedEvent;
            _healthController.EffectAddedEvent += OnEffectAddedEvent;
            _healthController.EffectRemovedEvent += OnEffectRemovedEvent;
            _healthController.HydrationChangedEvent += OnHydrationChangedEvent;
            _healthController.EnergyChangedEvent += OnEnergyChangedEvent;
            _healthController.TemperatureChangedEvent += OnTemperatureChangedEvent;

            // don't forget to unsubscribe
            _disposable = new Disposable(() =>
            {
                _healthController.DiedEvent -= OnDiedEvent;
                _healthController.HealthChangedEvent -= OnHealthChangedEvent;
                _healthController.EffectAddedEvent -= OnEffectAddedEvent;
                _healthController.EffectRemovedEvent -= OnEffectRemovedEvent;
                _healthController.HydrationChangedEvent -= OnHydrationChangedEvent;
                _healthController.EnergyChangedEvent -= OnEnergyChangedEvent;
            });
        }

        private void SetCurrentHealth(IHealthController healthController, IReadOnlyDictionary<EBodyPart, BodyPartHealth> bodyParts, EBodyPart bodyPart)
        {
            var bodyPartHealth = healthController.GetBodyPartHealth(bodyPart);
            bodyParts[bodyPart].Initialize(bodyPartHealth.Current, bodyPartHealth.Maximum);

            // set effects
            if (healthController.IsBodyPartBroken(bodyPart))
            {
                bodyParts[bodyPart].AddEffect(EBodyPartEffect.Fracture);
            }
            else
            {
                bodyParts[bodyPart].RemoveEffect(EBodyPartEffect.Fracture);
            }
        }

        private bool IsFracture(IEffect effect)
        {
            if (effect == null)
            {
                return false;
            }

            return effect.GetType().Name == "Fracture";
        }

        private void OnEffectAddedEvent(IEffect effect)
        {
            if (IsFracture(effect))
            {
                CurrentHealth.Health[effect.BodyPart].AddEffect(EBodyPartEffect.Fracture);
                _simpleTimer.IsSynchronized = false;
            }
        }

        private void OnEffectRemovedEvent(IEffect effect)
        {
            if (IsFracture(effect))
            {
                CurrentHealth.Health[effect.BodyPart].RemoveEffect(EBodyPartEffect.Fracture);
                _simpleTimer.IsSynchronized = false;
            }
        }

        private void OnHealthChangedEvent(EBodyPart bodyPart, float diff, DamageInfo effect)
        {
            CurrentHealth.Health[bodyPart].ChangeHealth(diff);
            _simpleTimer.IsSynchronized = false;
        }

        private void OnHydrationChangedEvent(float diff)
        {
            CurrentHealth.Hydration += diff;
            _simpleTimer.IsSynchronized = false;
        }

        private void OnEnergyChangedEvent(float diff)
        {
            CurrentHealth.Energy += diff;
            _simpleTimer.IsSynchronized = false;
        }

        private void OnTemperatureChangedEvent(float diff)
        {
            CurrentHealth.Temperature += diff;
            _simpleTimer.IsSynchronized = false;
        }

        private void OnDiedEvent(EFT.EDamageType obj)
        {
            CurrentHealth.IsAlive = false;
        }
    }
}
