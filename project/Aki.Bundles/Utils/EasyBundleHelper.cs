using Aki.Common.Utils;
using Aki.Reflection.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using UnityEngine;
using BindableState = BindableState<Diz.DependencyManager.ELoadState>;

namespace Aki.Bundles.Utils
{
    public class EasyBundleHelper
    {
        private const BindingFlags _flags = BindingFlags.Instance | BindingFlags.NonPublic;
        private static readonly FieldInfo _pathField;
        private static readonly FieldInfo _keyWithoutExtensionField;
        private static readonly FieldInfo _bundleLockField;
        private static readonly FieldInfo _loadingJobField;
        private static readonly PropertyInfo _dependencyKeysProperty;
        private static readonly PropertyInfo _keyProperty;
        private static readonly PropertyInfo _loadStateProperty;
        private static readonly PropertyInfo _progressProperty;
        private static readonly FieldInfo _bundleField;
        private static readonly FieldInfo _loadingAssetOperationField;
        private static readonly PropertyInfo _assetsProperty;
        private static readonly PropertyInfo _sameNameAssetProperty;
        private static readonly MethodInfo _loadingCoroutineMethod;
        private readonly object _instance;
        public static readonly Type Type;

        static EasyBundleHelper()
        {
            _ = nameof(IBundleLock.IsLocked);
            _ = nameof(BindableState.Bind);

            Type = PatchConstants.EftTypes.Single(x => x.GetMethod("set_SameNameAsset", _flags) != null);
            _pathField = Type.GetField("string_1", _flags);
            _keyWithoutExtensionField = Type.GetField("string_0", _flags);
            _bundleLockField = Type.GetFields(_flags).FirstOrDefault(x => x.FieldType == typeof(IBundleLock));
            _loadingJobField = Type.GetField("task_0", _flags);
            _dependencyKeysProperty = Type.GetProperty("DependencyKeys");
            _keyProperty = Type.GetProperty("Key");
            _loadStateProperty = Type.GetProperty("LoadState");
            _progressProperty = Type.GetProperty("Progress");
            _bundleField = Type.GetField("assetBundle_0", _flags);
            _loadingAssetOperationField = Type.GetField("assetBundleRequest_0");
            _assetsProperty = Type.GetProperty("Assets");
            _sameNameAssetProperty = Type.GetProperty("SameNameAsset");
            _loadingCoroutineMethod = Type.GetMethods(_flags).Single(x => x.GetParameters().Length == 0 && x.ReturnType == typeof(Task));
        }

        public EasyBundleHelper(object easyBundle)
        {
            _instance = easyBundle;
        }

        public IEnumerable<string> DependencyKeys
        {
            get
            {
                return (IEnumerable<string>)_dependencyKeysProperty.GetValue(_instance);
            }
            set
            {
                _dependencyKeysProperty.SetValue(_instance, value);
            }
        }

        public IBundleLock BundleLock
        {
            get
            {
                return (IBundleLock)_bundleLockField.GetValue(_instance);
            }
            set
            {
                _bundleLockField.SetValue(_instance, value);
            }
        }

        public Task LoadingJob
        {
            get
            {
                return (Task)_loadingJobField.GetValue(_instance);
            }
            set
            {
                _loadingJobField.SetValue(_instance, value);
            }
        }

        public string Path
        {
            get
            {
                return (string)_pathField.GetValue(_instance);
            }
            set
            {
                _pathField.SetValue(_instance, value);
            }
        }

        public string Key
        {
            get
            {
                return (string)_keyProperty.GetValue(_instance);
            }
            set
            {
                _keyProperty.SetValue(_instance, value);
            }
        }

        public BindableState LoadState
        {
            get
            {
                return (BindableState)_loadStateProperty.GetValue(_instance);
            }
            set
            {
                _loadStateProperty.SetValue(_instance, value);
            }
        }

        public float Progress
        {
            get
            {
                return (float)_progressProperty.GetValue(_instance);
            }
            set
            {
                _progressProperty.SetValue(_instance, value);
            }
        }


        public AssetBundle Bundle
        {
            get
            {
                return (AssetBundle)_bundleField.GetValue(_instance);
            }
            set
            {
                _bundleField.SetValue(_instance, value);
            }
        }

        public AssetBundleRequest loadingAssetOperation
        {
            get
            {
                return (AssetBundleRequest)_loadingAssetOperationField.GetValue(_instance);
            }
            set
            {
                _loadingAssetOperationField.SetValue(_instance, value);
            }
        }


        public UnityEngine.Object[] Assets
        {
            get
            {
                return (UnityEngine.Object[])_assetsProperty.GetValue(_instance);
            }
            set
            {
                _assetsProperty.SetValue(_instance, value);
            }
        }

        public UnityEngine.Object SameNameAsset
        {
            get
            {
                return (UnityEngine.Object)_sameNameAssetProperty.GetValue(_instance);
            }
            set
            {
                _sameNameAssetProperty.SetValue(_instance, value);
            }
        }

        public string KeyWithoutExtension
        {
            get
            {
                return (string)_keyWithoutExtensionField.GetValue(_instance);
            }
            set
            {
                _keyWithoutExtensionField.SetValue(_instance, value);
            }
        }

        public Task LoadingCoroutine()
        {
            return (Task)_loadingCoroutineMethod.Invoke(_instance, new object[] { });
        }
    }
}
