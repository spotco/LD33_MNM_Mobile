using UnityEngine;
using System.Collections.Generic;

public class MultiList<TKey, TValue> {
	public Dictionary<TKey,List<TValue>> _key_to_list = new Dictionary<TKey, List<TValue>>();
	public int count_of(TKey key) {
		if (!_key_to_list.ContainsKey(key)) _key_to_list[key] = new List<TValue>();
		return _key_to_list[key].Count;
	}
	public void add(TKey key, TValue val) {
		if (!_key_to_list.ContainsKey(key)) _key_to_list[key] = new List<TValue>();
		_key_to_list[key].Add(val);
	}
	public void clear(TKey key) {
		if (!_key_to_list.ContainsKey(key)) _key_to_list[key] = new List<TValue>();
		_key_to_list[key].Clear();
	}
	public List<TValue> list(TKey key) {
		if (!_key_to_list.ContainsKey(key)) _key_to_list[key] = new List<TValue>();
		return _key_to_list[key];
	}
	public List<TKey> keys() {
		return new List<TKey>(_key_to_list.Keys);
	}
}

public class Util {

	public static Vector3 vec_rotate_rad(Vector3 v, float rad) {
		float mag = v.magnitude;
		float ang = Mathf.Atan2(v.y, v.x);
		ang += rad;
		return new Vector3(mag*Mathf.Cos(ang), mag*Mathf.Sin(ang), v.z);
	}

	public static float drpt(float a, float b, float fric) {
		float deltaf = (b - a);
		deltaf *= Mathf.Pow(fric,1/Util.dt_scale);
		return a + deltaf;
	}

	public static float lerp(float a, float b, float t) {
		return a + (b - a) * t;
	}

	public static float dt_scale = 1;
	public static System.Random rand = new System.Random();
	
	public static float rand_range(float min, float max) {
		float r = (float)rand.NextDouble();
		return (max-min)*r + min;
	}
	
	public static GameObject proto_clone(GameObject proto) {
		GameObject rtv = ((GameObject)UnityEngine.Object.Instantiate(proto));
		rtv.transform.parent = proto.transform.parent;
		rtv.transform.localScale = proto.transform.localScale;
		rtv.transform.localPosition = proto.transform.localPosition;
		rtv.transform.localRotation = proto.transform.localRotation;
		rtv.SetActive(true);
		return rtv;
	}
	
	public static string vec_to_s(Vector3 v) {
		return string.Format("({0},{1},{2})",v.x,v.y,v.z);
	}
	
	public static Vector3 valv(float x) {
		return new Vector3(x,x,x);
	}
	
	
	public static Vector3 vec_scale(Vector3 v,float f) {
		v.x *= f;
		v.y *= f;
		v.z *= f;
		return v;
	}
	
	public static float rad2deg = 57.29f;
	public static float deg2rad = 0.017f;
	
	public static void transform_set_euler_world(Transform t,Vector3 tar) {
		Quaternion q = t.rotation;
		q.eulerAngles = tar;
		t.rotation = q;
	}
	
	public static Vector3 vec_add(Vector3 a, Vector3 b) {
		Vector3 v = new Vector3();
		v.x = a.x + b.x;
		v.y = a.y + b.y;
		v.z = a.z + b.z;
		return v;
	}
	
	public static Vector3 vec_sub(Vector3 a, Vector3 b) {
		return new Vector3(a.x-b.x,a.y-b.y,a.z-b.z);
	}

	public static bool vec_eq(Vector3 a, Vector3 b) {
		return a.x == b.x && a.y == b.y && a.z == b.z;
	}

	public static float bezier_val_for_t(float p0, float p1, float p2, float p3, float t) {
		float cp0 = (1 - t)*(1 - t)*(1 - t);
		float cp1 = 3 * t * (1-t)*(1-t);
		float cp2 = 3 * t * t * (1 - t);
		float cp3 = t * t * t;
		return cp0 * p0 + cp1 * p1 + cp2 * p2 + cp3 * p3;
	}

	public static Vector2 bezier_val_for_t(Vector2 p0, Vector2 p1, Vector2 p2, Vector2 p3, float t) {
		return new Vector2(
			bezier_val_for_t(p0.x,p1.x,p2.x,p3.x,t),
			bezier_val_for_t(p0.y,p1.y,p2.y,p3.y,t)
		);
	}

	public static Vector3 bezier_val_for_t(Vector3 p0, Vector3 p1, Vector3 p2, Vector3 p3, float t) {
		return new Vector3(
			bezier_val_for_t(p0.x,p1.x,p2.x,p3.x,t),
			bezier_val_for_t(p0.y,p1.y,p2.y,p3.y,t),
			bezier_val_for_t(p0.z,p1.z,p2.z,p3.z,t)
		);
	}
	
	public static float y_for_point_of_2pt_line(Vector2 pt1, Vector2 pt2, float x) {
		//(y - y1)/(x - x1) = m
		float m = (pt1.y - pt2.y) / (pt1.x - pt2.x);
		//y - mx = b
		float b = pt1.y - m * pt1.x;
		return m * x + b;
	}

	public static Vector2 line_seg_intersection_pts(Vector2 a1, Vector2 a2, Vector2 b1, Vector2 b2) {
		double Ax = a1.x; double Ay = a1.y;
		double Bx = a2.x; double By = a2.y;
		double Cx = b1.x; double Cy = b1.y;
		double Dx = b2.x; double Dy = b2.y;
		double X; double Y;
		double  distAB, theCos, theSin, newX, ABpos ;
		
		if ((Ax==Bx && Ay==By) || (Cx==Dx && Cy==Dy)) return new Vector2(float.NaN,float.NaN); //  Fail if either line segment is zero-length.
		
		Bx-=Ax; By-=Ay;//Translate the system so that point A is on the origin.
		Cx-=Ax; Cy-=Ay;
		Dx-=Ax; Dy-=Ay;
		
		distAB=System.Math.Sqrt(Bx*Bx+By*By);//Discover the length of segment A-B.
		
		theCos=Bx/distAB;//Rotate the system so that point B is on the positive X axis.
		theSin=By/distAB;
		
		newX=Cx*theCos+Cy*theSin;
		Cy  =Cy*theCos-Cx*theSin; Cx=newX;
		newX=Dx*theCos+Dy*theSin;
		Dy  =Dy*theCos-Dx*theSin; Dx=newX;
		
		if ((Cy<0.0 && Dy<0.0) || (Cy>=0.0 && Dy>=0.0)) return new Vector2(float.NaN,float.NaN); //C-D must be origin crossing line
		
		ABpos=Dx+(Cx-Dx)*Dy/(Dy-Cy);//Discover the position of the intersection point along line A-B.
		
		
		if (ABpos<0.0 || ABpos-distAB> 0.001) {
			return new Vector2(float.NaN,float.NaN);//  Fail if segment C-D crosses line A-B outside of segment A-B.
		}
		
		X=Ax+ABpos*theCos;//Apply the discovered position to line A-B in the original coordinate system.
		Y=Ay+ABpos*theSin;
		
		return new Vector2((float)X,(float)Y);
	}
}


public class PathRenderer : MonoBehaviour {
	
	[SerializeField] private GameObject _arrow_dot_proto;
	[SerializeField] private GameObject _arrow_head_proto;
	[SerializeField] public GameObject _path_renderer_root;
	
	private void Start () {
		_arrow_dot_proto.SetActive(false);
		_arrow_head_proto.SetActive(false);
	}
	
	//TODO -- pool me
	private MultiList<int,GameObject> _id_to_objs = new MultiList<int, GameObject>();
	public void id_draw_path(int id, Vector3 position, Vector3[] points) {
		if (!_id_to_theta.ContainsKey(id)) _id_to_theta[id] = 0.0f;
		float dist_per = 15.0f;
		Vector3 last = position;
		float last_remainder = 0;
		for (int i = 0; i < points.Length; i++) {
			Vector3 itr = points[i];
			float itr_dist_total = Vector3.Distance(last,itr); 
			float itr_dist = 0;
			if (i == 1) itr_dist = dist_per;
			while (itr_dist < itr_dist_total) {
				Vector3 neu_obj_pos = Vector3.Lerp(last,itr,itr_dist/itr_dist_total);
				
				GameObject neu_obj = Util.proto_clone(_arrow_dot_proto);
				neu_obj.transform.parent = _path_renderer_root.transform;
				neu_obj.transform.position = new Vector3(neu_obj_pos.x,neu_obj_pos.y,neu_obj_pos.z);
				_id_to_objs.add(id,neu_obj);
				
				itr_dist += dist_per;
			}
			
			if (i == points.Length-1) {
				GameObject neu_obj2 = Util.proto_clone(_arrow_head_proto);
				neu_obj2.transform.parent = _path_renderer_root.transform;
				neu_obj2.transform.position = new Vector3(itr.x,itr.y,itr.z);
				_id_to_objs.add(id,neu_obj2);
				
			}
			
			last_remainder = (itr_dist_total - itr_dist);
			last = itr;
		}
		this.update_anim(id,_id_to_theta[id]);
	}
	
	public void clear_path(int id) {
		foreach(GameObject itr in _id_to_objs.list(id)) {
			Destroy(itr);
		}
		_id_to_objs.clear(id);
	}
	
	public void clear_paths() {
		var keys = _id_to_objs.keys();
		foreach (var key in keys) {
			clear_path(key);
		}
	}
	
	private Dictionary<int, float> _id_to_theta = new Dictionary<int, float>();
	public void Update() {
		foreach(int id in _id_to_objs.keys()) {
			if (_id_to_objs.count_of(id) > 0) {
				if (!_id_to_theta.ContainsKey(id)) _id_to_theta[id] = 0.0f;
				float val = _id_to_theta[id];
				List<GameObject> list = _id_to_objs.list(id);
				
				val += 0.5f * list.Count * 0.02f;
				if (val > list.Count*1.35f) val = -list.Count*0.35f;
				
				this.update_anim(id,val);

				_id_to_theta[id] = val;
			}
		}
	}

	private void update_anim(int id, float val) {
		List<GameObject> list = _id_to_objs.list(id);
		for (int i_list = 0; i_list < list.Count; i_list++) {
			SpriteRenderer itr_list = list[i_list].GetComponent<SpriteRenderer>();
			if (itr_list.sprite.name != "move_arrow_cross") {
				Color itr_list_color = itr_list.color;
				float aval = Mathf.Pow(1-(Mathf.Abs(i_list-val))/list.Count,4.0f);
				
				// HACK: cap to prevent path points from exploding in size
				aval = Mathf.Min(aval, 1.0f);
				
				itr_list_color.a = Mathf.Max(aval,0.25f);
				itr_list.color = itr_list_color;
				itr_list.transform.localScale = Util.valv((0.75f + aval * 0.5f)*20.0f);
			}
		}
	}
}