using UnityEngine;
using System.Collections;

namespace Uzu {
    /// <summary>
    /// Uzu user interface panel.
    ///
    /// How to:
    /// Override this class to create your custom panel Logic
    /// </summary>
    public abstract class UiPanel : MonoBehaviour
    {
        #region Implementation
        /// <summary>
        /// The _owner manager of this panel.
        /// </summary>
        private UiPanelMgr _ownerManager;

        /// <summary>
        /// Gets or sets the owner manager.
        /// </summary>
        protected UiPanelMgr OwnerManager {
            get {
                return _ownerManager;
            }
            private set {
                _ownerManager = value;
            }
        }

        #region UiPanelInterface implementation.
        /// <summary>
        /// Initialize this panel.
        /// Create the link between the panel manager and the
        /// </summary>
        public void Initialize (UiPanelMgr ownerManager)
        {
            _ownerManager = ownerManager;
            
            OnInitialize ();

            // Deactivate the object just in case it was active during edit mode.
            // We don't want to call Deactivate, since this triggers the callback.
            this.gameObject.SetActive (false);
        }
        #endregion
        #endregion

        #region Events.
        /// <summary>
        /// Called when the panel is first initialized.
        /// </summary>
        public abstract void OnInitialize ();

        /// <summary>
        /// Called when the panel is activated.
        /// </summary>
        public abstract void OnEnter (PanelEnterContext context);
        
        /// <summary>
        /// Called when the panel is deactivated.
        /// </summary>
        public abstract void OnExit (PanelExitContext context);
        #endregion
    }
}