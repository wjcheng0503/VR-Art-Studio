using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class CanvasDrawToolScript : MonoBehaviour
{
    [SerializeField] private Transform _tip;
    [SerializeField] private int _penSize = 5;

    private Renderer _renderer;
    private Color[] _colors;
    private float _tipHeight;

    private RaycastHit _touch;
    private Canvas _canvas;
    private Vector2 _touchPos, _lastTouchPos;
    private Quaternion _lastTouchRot;
    private bool _touchedLastFrame;

    // Start is called before the first frame update
    void Start()
    {
        _renderer = _tip.GetComponent<Renderer>();
        _colors = Enumerable.Repeat(_renderer.material.color, _penSize * _penSize).ToArray();
        _tipHeight = _tip.localScale.y;
    }

    // Update is called once per frame
    void Update()
    {
        Draw();
    }

    private void Draw() {
        if (Physics.Raycast(_tip.position, transform.up, out _touch, _tipHeight)) {
            if (_touch.transform.CompareTag("Canvas")) {
                if (_canvas == null) {
                    _canvas = _touch.transform.GetComponent<Canvas>();
                }

                _touchPos = new Vector2(_touch.textureCoord.x, _touch.textureCoord.y);

                var x = (int)(_touchPos.x * _canvas.textureSize.x - (_penSize/2));
                var y = (int)(_touchPos.y * _canvas.textureSize.y - (_penSize/2));

                if (y < 0 || y > _canvas.textureSize.y || x < 0 || x > _canvas.textureSize.x) return;

                if (_touchedLastFrame) {
                    _canvas.texture.SetPixels(x, y, _penSize, _penSize, _colors);

                    for (float f = 0.01f; f < 1.00f; f+= 0.03f)
                    {
                        var lerpX = (int)Mathf.Lerp(_lastTouchPos.x, x, f);
                        var lerpY = (int)Mathf.Lerp(_lastTouchPos.y, y, f);
                        _canvas.texture.SetPixels(lerpX, lerpY, _penSize, _penSize, _colors);
                    }

                    transform.rotation = _lastTouchRot;

                    _canvas.texture.Apply();
                }

                _lastTouchPos = new Vector2(x,y);
                _lastTouchRot = transform.rotation;
                _touchedLastFrame = true;
                return;
            }
        }

        _canvas = null;
        _touchedLastFrame = false;
    }
}
