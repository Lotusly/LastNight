
using TMPro;
using UnityEngine;


public class VoffsetLetter : MonoBehaviour
{

	[SerializeField] private TextMeshPro _tm;
	[Range(0.0f,1.0f)] [SerializeField] private float _possibility;
	[Range(-2.0f,2.0f)][SerializeField] private float _deviation;
	//[SerializeField] private bool _directionUp;
	[SerializeField] private bool _reset;

	private string _originalText;
	
	
	

	// Use this for initialization
	void Start ()
	{
		_originalText = _tm.text;
	}
	
	// Update is called once per frame
	void Update () {
		if (_reset)
		{
			_reset = false;
			Generate();
		}
	}

	private void Generate()
	{
		string _enterSwift="<voffset="+_deviation.ToString()+"em>";
		string _outSwift = "</voffset>";
		
		bool _inBrackets=false;
		int _len = _originalText.Length;
		string result = "";
		for (int i = 0; i < _len; i++)
		{
			if (_originalText[i] == '<')
			{
				result += '<';
				_inBrackets = true;
				continue;
			}

			if (!_inBrackets)
			{
				if (Random.Range(0f, 1f) < _possibility)
				{
					result += _enterSwift;
					result += _originalText[i];
					result += _outSwift;
				}
				else
				{
					result += _originalText[i];
				}
			}
			else
			{
				if (_originalText[i] == '>')
				{
					result += '>';
					_inBrackets = false;
				}
				else
				{
					result += _originalText[i];
				}
			}
		}

		_tm.text = result;

	}


}
