using UnityEngine;
using System.Collections;

namespace PrefsGUI
{
    public class GUIUtilSample : PrefsGUISampleBase
    {
        protected override float MinWidth { get { return 1000f; } }


        GUIUtil.Folds _fieldFold = new GUIUtil.Folds();
        GUIUtil.Folds _sliderFold = new GUIUtil.Folds();
        GUIUtil.Folds _miscFold = new GUIUtil.Folds();
        GUIUtil.Folds _dynamicFold = new GUIUtil.Folds();


        public EnumSample _enum;
        public string _string;
        public bool _bool;
        public int _int;
        public float _float;
        public Vector2 _vector2;
        public Vector3 _vector3;
        public Vector4 _vector4;
        public Rect _rect;


        public string _intStr;
        public string _floatStr;
        public string _vector2Str;
        public string _vector3Str;
        public string _vector4Str;
        public string _rectStr;


        bool _dynamicFoldEnable = true;

        public void Start()
        {
            _miscFold.Add("Fold0", () => { GUILayout.Label("Fold0"); });
            _miscFold.Add("Fold1", () => { GUILayout.Label("Fold1 FirstAdd"); });
            _miscFold.Add("Fold1", () => { GUILayout.Label("Fold1 SecondAdd"); });
            _miscFold.Add(-1, "FoldCustomOrder", () => { GUILayout.Label("FoldCustomOrder"); });
            _miscFold.Add("IDebugMenu", typeof(IDebugMenuSample));
            _dynamicFold.Add("DynamicFold", () => _dynamicFoldEnable, () => { GUILayout.Label("DynamicFold"); });

            _fieldFold.Add("Field", () =>
            {
                _enum = GUIUtil.Field(_enum, "enum");
                _string = GUIUtil.Field(_string, "string");
                _bool = GUIUtil.Field(_bool, "bool");
                _int = GUIUtil.Field(_int, "int");
                _float = GUIUtil.Field(_float, "float");
                _vector2 = GUIUtil.Field(_vector2, "vector2");
                _vector3 = GUIUtil.Field(_vector3, "vector3");
                _vector4 = GUIUtil.Field(_vector4, "vector4");
                _rect = GUIUtil.Field(_rect, "rect");
            });

            _fieldFold.Add("FieldWithUnparsedStr", () =>
            {
                _int = GUIUtil.Field(_int, ref _intStr, "int");
                _float = GUIUtil.Field(_float, ref _floatStr, "float");
                _vector2 = GUIUtil.Field(_vector2, ref _vector2Str, "vector2");
                _vector3 = GUIUtil.Field(_vector3, ref _vector3Str, "vector3");
                _vector4 = GUIUtil.Field(_vector4, ref _vector4Str, "vector4");
                _rect = GUIUtil.Field(_rect, ref _rectStr, "rect");
            },
            true);

            _sliderFold.Add("Slider", () =>
            {
                _int = GUIUtil.Slider(_int, 0, 100, "Slider(int)");
                _float = GUIUtil.Slider(_float, "Slider(float)");
                _vector2 = GUIUtil.Slider(_vector2, Vector2.zero, Vector2.one, "Slider(Vector2)");
                _vector3 = GUIUtil.Slider(_vector3, Vector3.zero, Vector3.one, "Slider(Vector3)");
                _vector4 = GUIUtil.Slider(_vector4, Vector4.zero, Vector4.one, "Slider(Vector4)");
                _rect = GUIUtil.Slider(_rect, Rect.zero, new Rect(1f,1f,1f,1f), "Slider(Rect)");
            });

            _sliderFold.Add("SliderWithUnparsedStr", () =>
            {
                _int = GUIUtil.Slider(_int, 0, 100, ref _intStr, "Slider(int)");
                _float = GUIUtil.Slider(_float, ref _floatStr, "Slider(float)");
                _vector2 = GUIUtil.Slider(_vector2, Vector2.zero, Vector2.one, ref _vector2Str, "Slider(Vector2)");
                _vector3 = GUIUtil.Slider(_vector3, Vector3.zero, Vector3.one, ref _vector3Str, "Slider(Vector3)");
                _vector4 = GUIUtil.Slider(_vector4, Vector4.zero, Vector4.one, ref _vector4Str, "Slider(Vector4)");
                _rect = GUIUtil.Slider(_rect, Rect.zero, new Rect(1f, 1f, 1f, 1f), ref _rectStr, "Slider(Rect)");
            },
            true);
        }


        protected override void OnGUIInternal()
        {
            using (var h = new GUILayout.HorizontalScope())
            {
                using (var v = new GUILayout.VerticalScope())
                {
                    _miscFold.OnGUI();
                    _dynamicFoldEnable = GUILayout.Toggle(_dynamicFoldEnable, "DynamicFold");
                    _dynamicFold.OnGUI();
                    _int = GUIUtil.IntButton(_int, "IntButton");

                    GUIUtil.Indent(() =>
                    {
                        GUILayout.Label("Indent");
                    });

                    using (var cs = new GUIUtil.ColorScope(Color.green))
                    {
                        GUILayout.Label("ColorScope");
                    }
                }

                _fieldFold.OnGUI();
                _sliderFold.OnGUI();
            }
        }
    }
}