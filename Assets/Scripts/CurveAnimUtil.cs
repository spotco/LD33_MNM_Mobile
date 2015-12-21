using System.Collections.Generic;
using UnityEngine;

public class CurveAnimUtil {
  public static float Lerp(float a, float b, float t) {
    return a + (b - a) * t;
  }
  
  // Quadratic bezier curve with control points p0, p1, p2, p3
  public static float BezierValForT(
    float p0, float p1, float p2, float p3, float t) {
    float cp0 = (1 - t) * (1 - t) * (1 - t);
    float cp1 = 3 * t * (1 - t) * (1 - t);
    float cp2 = 3 * t * t * (1 - t);
    float cp3 = t * t * t;
    return cp0 * p0 + cp1 * p1 + cp2 * p2 + cp3 * p3;
  }
  public static Vector2 BezierValForT(
    Vector2 p0, Vector2 p1, Vector2 p2, Vector2 p3, float t) {
    return new Vector2(
      BezierValForT(p0.x, p1.x, p2.x, p3.x, t),
      BezierValForT(p0.y, p1.y, p2.y, p3.y, t)
    );
  }
  public static Vector3 BezierValForT(
    Vector3 p0, Vector3 p1, Vector3 p2, Vector3 p3, float t) {
    return new Vector3(
      BezierValForT(p0.x, p1.x, p2.x, p3.x, t),
      BezierValForT(p0.y, p1.y, p2.y, p3.y, t),
      BezierValForT(p0.z, p1.z, p2.z, p3.z, t)
     );
  }
  
  // Smooth friction curve, a will approach b by "friction" percent
  // Delta time scaled
  public static float ApplyFrictionTick(float start, float to, float fric) {
    // y = e ^ (-a * timescale)
    fric = 1 - fric;
    float a = Mathf.Log(fric);
    float y = 1 - Mathf.Exp(a * CurveAnimUtil.GetDeltaTimeScale());
    
    // rtv = start + (to - start) * timescaled_friction
    float delta = (to - start) * y;
    return start + delta;
  }
  public static float GetDeltaTimeScale() {
    return (Time.deltaTime) / (1 / 60.0f);
  }
  public static float SecondsToDeltaTimeTicks(float sec) {
    return (1 / 60.0f) / sec;
  }
  public static Vector2 VectorMult(Vector2 a, Vector2 b) {
    return new Vector2(a.x * b.x, a.y * b.y);
  }
  
  public static float LinearMoveTo(float a, float b, float vmax) {
    float dir = CurveAnimUtil.Sig(b - a);
    float mag = Mathf.Abs(b - a) > vmax * CurveAnimUtil.GetDeltaTimeScale() ? 
      vmax * CurveAnimUtil.GetDeltaTimeScale() : Mathf.Abs(b - a);
    return a + dir * mag;
  }
  
  public static float EasedLinearMoveTo(float a, float b, float vmax, 
    float accel, float vcur, out float vnext) {
    float dir = CurveAnimUtil.Sig(b - a);
    float dt_vel = vcur * CurveAnimUtil.GetDeltaTimeScale();
    float mag = Mathf.Abs(b - a) > dt_vel ? dt_vel : Mathf.Abs(b - a);
    if (dir != 0) {
      vnext = vcur + accel * CurveAnimUtil.GetDeltaTimeScale();
    } else {
      vnext = 0;
    }
    return a + dir * mag;
  }
  
  public static int Sig(float a) {
    if (a > 0) {
      return 1;
    } else if (a < 0) {
      return -1;
    } else {
      return 0;
    }
  }
}

