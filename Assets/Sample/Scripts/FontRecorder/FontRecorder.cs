using System;
using System.Linq;
using Cysharp.Threading.Tasks;
using SFB;
using UnityEngine;
using UnityEngine.EventSystems;
using Button = UnityEngine.UI.Button;

namespace ThreeDFont.Record.Test
{
    public class FontRecorder : MonoBehaviour
    {
        [SerializeField] FontData _fontData = new FontData();

        [Header("--------------------------------")] [SerializeField]
        private char _character;

        [SerializeField] private EventTrigger _recordTrigger;
        [SerializeField] private Button _finishButton;
        [SerializeField] private Button _playButton;
        [SerializeField] private Button _normalizePlayButton;
        [SerializeField] private Button _nomalizeButton;
        [SerializeField] private Button _saveButton;

        private CharacterData _characterData;
        private bool _isRecording = false;

        [SerializeField] private bool _isDebugLoad = true;
        [SerializeField] private GameObject _debugFontObject;
        private const float _targetFps = 30;
        private float _timer = 0;
        private void Start()
        {
            if (_isDebugLoad is true)
            {
                _fontData = FontData.Load(GetLoadFilePath());
            }

            
            EventTrigger.Entry entry = new EventTrigger.Entry { eventID = EventTriggerType.PointerDown };
            entry.callback.AddListener((data) => { Record(); });
            
            _recordTrigger.triggers.Add(entry);
            
            _finishButton.onClick.AddListener(Finish);
            _playButton.onClick.AddListener(() => Play().Forget());
            _normalizePlayButton.onClick.AddListener(() => NormalizePlay().Forget());
            _nomalizeButton.onClick.AddListener(() => GetCharacterData(_character).Normalize());
            _saveButton.onClick.AddListener(FontDataJsonSave);
        }

        [ContextMenu("LastWriteRemove")]
        private void LastWriteRemove()
        {
            GetCharacterData(_character).LastWriteDataRemove();
        }
        private void Update()
        {
            _timer += Time.deltaTime;
            if (_isRecording == false) return;
            if (_timer >= 1 / _targetFps)
            {
                _timer = 0;
                
                _characterData.FontPositions.Add(new FontPosition()
                {
                    WritePosition = Input.mousePosition,
                    IsWrite = Input.GetMouseButton(0)
                });
            }
        }

        private void Record()
        {
            if(_isRecording) return;
            _characterData = new CharacterData(_character);
            _isRecording = true;
        }

        private void Finish()
        {
            if(_isRecording is false) return;
            _isRecording = false;

            _fontData.CharacterData.Add(_characterData);
        }

        private async UniTask Play()
        {
            var data = GetCharacterData(_character);

            for (int i = 0; i < data.FontPositions.Count; i++)
            {
                _debugFontObject.SetActive(data.FontPositions[i].IsWrite);
                if (data.FontPositions[i].IsWrite)
                {
                    _debugFontObject.transform.position = data.FontPositions[i].WritePosition;
                }

                await UniTask.Delay(TimeSpan.FromSeconds(1f / 30f));
            }
        }

        private async UniTask NormalizePlay()
        {
            var data = GetCharacterData(_character);

            for (int i = 0; i < data.NormalizedFontPositions.Count; i++)
            {
                _debugFontObject.SetActive(data.NormalizedFontPositions[i].IsWrite);
                if (data.NormalizedFontPositions[i].IsWrite)
                {
                    _debugFontObject.transform.position = data.NormalizedFontPositions[i].WritePosition;
                }

                await UniTask.Delay(TimeSpan.FromSeconds(1f / 30f));
            }
        }

        private CharacterData GetCharacterData(char character)
        {
            return _fontData.CharacterData.FirstOrDefault(x => x.Character == character);
        }

        private string GetLoadFilePath()
        {
            var paths = StandaloneFileBrowser.OpenFilePanel("Open File", "", "json", false);
            return paths[0] == null ? "" : paths[0];
        }  
        private string GetSaveFilePath()
        {
            var path = StandaloneFileBrowser.SaveFilePanel("フォントの保存先", "", "font", "json");
            return path;
        }

        [UnityEngine.ContextMenu("FontDataJson")]
        private void FontDataJson()
        {
            var json = JsonUtility.ToJson(_fontData, true);
            Debug.Log(json);
        }

        [UnityEngine.ContextMenu("FontDataJsonSave")]
        private void FontDataJsonSave()
        {
            FontData.Save(_fontData, GetSaveFilePath());
        }

        [UnityEngine.ContextMenu("FontDataLoad")]
        private void FontDataLoad()
        {
            Debug.Log(GetLoadFilePath());
            _fontData = FontData.Load(GetLoadFilePath());
        }
    }
}