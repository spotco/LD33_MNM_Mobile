using UnityEngine;
using System.Collections.Generic;
using UnityEngine.UI;

public class ChatManager : MonoBehaviour {

	[SerializeField] public ScrollText _text_scroll;
	[SerializeField] Image _img;
	[SerializeField] List<Image> _additional_images;
	private float _img_tar_alpha = 0;
	private void set_img_alpha(float val) {
		Color c= _img.color;
		c.a=val;
		_img.color = c;
		_text_scroll._text.set_alpha(val);
		if (_additional_images != null) {
			for (int i = 0; i < _additional_images.Count; i++) {
				_additional_images[i].color = c;
			}
		}
	}

	public void Awake() {
		_text_scroll.clear();
	}

	public List<string> _messages = new List<string>();
	List<int> _ids = new List<int>();
	public int _current_id = 0;
	public void push_message(string msg, int id = 0) {
		_messages.Add(msg);
		_ids.Add(id);
	}

	public void clear_messages() {
		_text_scroll.finish();
		_messages.Clear();
		_ids.Clear();
		_text_scroll.clear();
		_ct = 0;
		this.set_img_alpha(0);
		this._img_tar_alpha = 0;
	}

	public float _ct = 0;
	public void Update() {
		if (_ct <= 0 && _messages.Count > 0 && _text_scroll.finished()) {
			_text_scroll.load(_messages[0]);
			_messages.RemoveAt(0);
			_current_id = _ids[0];
			_ids.RemoveAt(0);

		} else if (_text_scroll.finished()) {
			_ct-=Util.dt_scale*2.0f;
			_img_tar_alpha = Mathf.Clamp(_ct/100.0f,0,1);
			
		} else {
			_img_tar_alpha = 1;
			_ct = 150;
		}
		this.set_img_alpha(Util.drpt(_img.color.a,_img_tar_alpha,1/8.0f));
	}


}