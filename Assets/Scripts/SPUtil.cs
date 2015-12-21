using UnityEngine;
using System.Collections;

public class SPUtil {
  public static GameObject ProtoClone(GameObject proto) {
    GameObject rtv = ((GameObject)UnityEngine.Object.Instantiate(proto));
    rtv.transform.SetParent(proto.transform.parent);
    rtv.transform.localScale = proto.transform.localScale;
    rtv.transform.localPosition = proto.transform.localPosition;
    rtv.transform.localRotation = proto.transform.localRotation;
    rtv.SetActive(true);
    return rtv;
  }
  
  public static bool FloatCompareDelta(float a, float b, float delta) {
    return Mathf.Abs(a - b) <= delta;
  }
  
  public static Vector3 VectorOfValue(float val) {
    return new Vector3(val, val, val);
  }
  public static float RunningAverage(float avg, float val, float ct) {
    avg -= (avg / ct) * CurveAnimUtil.GetDeltaTimeScale();
    avg += (val / ct) * CurveAnimUtil.GetDeltaTimeScale();
    return avg;
  }
  
  // platform independent is any touch/click currently down
  public static bool IsTouch() {
    #if UNITY_EDITOR
    if (Input.GetMouseButton(0)) {
      return true;
    }
    #elif UNITY_IOS
    Touch[] touches = Input.touches;
    for (int i = 0; i < touches.Length; i++) {
      if (touches[i].fingerId == 0 && touches[i].phase != TouchPhase.Ended) {
        return true;
      }
    }
    #endif
    return false;
  }
  
  // platform independent is any touch/click currently down and out position
  public static bool IsTouchAndPosition(out Vector2 pos) {
    bool rtv = false;
    Vector2 pixel_screen_pos = new Vector2();
    if (SPUtil.IsTouch()) {
      #if UNITY_EDITOR
      rtv = true;
      pixel_screen_pos = 
        new Vector2(Input.mousePosition.x, Input.mousePosition.y);
      #elif UNITY_IOS
      rtv = true;
      Touch[] touches = Input.touches;
      for (int i = 0; i < touches.Length; i++) {
        if (touches[i].fingerId == 0) {
          pixel_screen_pos = touches[0].position;
        }
      }
      
      #endif
    }
    pos = pixel_screen_pos;
    return rtv;
  }
  
  public static bool RectTransformContainsScreenPoint(
    RectTransform rect, Vector2 s_pos) {
    Vector2 local_touch_pos = rect.InverseTransformPoint(s_pos);
    return (rect.rect.Contains(local_touch_pos));
  }
}
