using UnityEngine;

namespace Uzu
{
    /// <summary>
    /// Miscellaneous services.
    /// </summary>
    public static class Util
    {
        /// <summary>
        /// Finds the specified component on the game object or one of its parents.
        /// </summary>
        public static T FindInParents<T> (GameObject go) where T : Component
        {
            if (go == null) {
                return null;
            }

            object comp = go.GetComponent<T>();

            if (comp == null) {
                Transform t = go.transform.parent;

                while (t != null && comp == null) {
                    comp = t.gameObject.GetComponent<T>();
                    t = t.parent;
                }
            }

            return (T)comp;
        }

        /// <summary>
        /// Search through all children of the game object (and children's children)
        /// until an object with the given name is found.
        ///
        /// Performance can be quite slow, so it's not advised to call this
        /// in performance critical regions.
        /// </summary>
        public static GameObject FindInChildren (GameObject go, string name, bool searchInactiveChildren = false)
        {
            Transform xform = go.transform;
            Transform resultXform = FindInChildren (xform, name, searchInactiveChildren);
            if (resultXform != null) {
                return resultXform.gameObject;
            }
            else {
                return null;
            }
        }

        /// <summary>
        /// Search through all children of the transform (and children's children)
        /// until an object with the given name is found.
        ///
        /// Performance can be quite slow, so it's not advised to call this
        /// in performance critical regions.
        /// </summary>
        public static Transform FindInChildren (Transform xform, string name, bool searchInactiveChildren = false)
        {
            // Found it.
            if (xform.name == name) {
                return xform;
            }

            // Search children.
            for (int i = 0; i < xform.childCount; i++) {
                Transform childXform = xform.GetChild (i);

                // Don't traverse inactive objects.
                if (!searchInactiveChildren &&
                    !childXform.gameObject.activeSelf) {
                    continue;
                }

                Transform resultXform = FindInChildren (childXform, name);
                if (resultXform != null) {
                    return resultXform;
                }
            }

            // Not found.
            return null;
        }
    }
}