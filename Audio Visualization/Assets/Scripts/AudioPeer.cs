using UnityEngine;

[RequireComponent (typeof(AudioSource))]
public class AudioPeer : MonoBehaviour {

	AudioSource _audioSource;
	public static float[] _samplesLeft = new float[512];
	public static float[] _samplesRight = new float[512];

	public static int _numBands = 8;
	float[] _freqBand;
	float[] _freqBandBuffer;
	float[] _freqBufferDecrease;
	float[] _freqBandHighest;
	public static int _avgFreqBand;

	public static float[] _audioBand;
	public static float[] _audioBandBuffer;

	public static float _Amplitude, _AmplitudeBuffer;
	float _AmplitudeHighest;
	public float _audioProfile;


	public enum _channel {
		Stereo,
		Left,
		Right
	}

	public _channel channel = new _channel ();

	void Awake () {
		_freqBand = new float[_numBands];
		_freqBandBuffer = new float[_numBands];
		_freqBufferDecrease = new float[_numBands];
		_freqBandHighest = new float[_numBands];

		_audioBand = new float[_numBands];
		_audioBandBuffer = new float[_numBands];
	}

	void Start () {
		_audioSource = GetComponent<AudioSource> ();
		AudioProfile (_audioProfile);
	}

	void Update () {
		GetSpectrumAudioSource ();
		MakeFrequencyBands ();
		BandBuffer ();
		CreateAudioBands ();
		GetAverageAudioBand ();
		GetAmplitude ();
	}

	void AudioProfile (float audioProfile) {
		for (int i = 0; i < _numBands; i++) {
			_freqBandHighest [i] = audioProfile;
		}
	}

	void GetSpectrumAudioSource () {
		_audioSource.GetSpectrumData (_samplesLeft, 0, FFTWindow.Blackman);
		_audioSource.GetSpectrumData (_samplesRight, 1, FFTWindow.Blackman);
	}

	void MakeFrequencyBands () {
		int count = 0;
		for (int i = 0; i < _numBands; i++) {
			float average = 0;
			int sampleCount = (int)Mathf.Pow (2, i + 1);
			if (i == 7)
				sampleCount += 2;

			for (int j = 0; j < sampleCount; j++) {
				average += _samplesLeft [count] + _samplesRight [count] * (count + 1);
				count++;
			}
			average /= count;

			_freqBand [i] = average * 10;
		}
	}

	void BandBuffer () {
		for (int i = 0; i < _numBands; i++) {
			if (_freqBand [i] > _freqBandBuffer [i]) {
				_freqBandBuffer [i] = _freqBand [i];
				_freqBufferDecrease [i] = 0.005f;
			}

			if (_freqBand [i] < _freqBandBuffer [i]) {
				_freqBandBuffer [i] -= _freqBufferDecrease [i];
				if (_freqBandBuffer [i] < 0)
					_freqBandBuffer [i] = 0;
				_freqBufferDecrease [i] *= 1.2f;
			}
		}
	}

	void CreateAudioBands () {
		for (int i = 0; i < _numBands; i++) {
			if (_freqBand [i] > _freqBandHighest [i]) {
				_freqBandHighest [i] = _freqBand [i];
			}
			_audioBand [i] = (_freqBand [i] / _freqBandHighest [i]);
			_audioBandBuffer [i] = (_freqBandBuffer [i] / _freqBandHighest [i]);
		}
	}

	void GetAverageAudioBand () {
		float sum = 0;
		float denominator = 0;
		for (int i = 0; i < _numBands; i++) {
			denominator += _audioBand [i];
			sum += i * _audioBand [i];
		}
		sum /= denominator;
		_avgFreqBand = (int)sum;
	}

	void GetAmplitude () {
		float _CurrentAmplitude = 0;
		float _CurrentAmplitudeBuffer = 0;

		for (int i = 0; i < _numBands; i++) {
			_CurrentAmplitude += _audioBand [i];
			_CurrentAmplitudeBuffer += _audioBandBuffer [i];
		}

		if (_CurrentAmplitude > _AmplitudeHighest)
			_AmplitudeHighest = _CurrentAmplitude;

		_Amplitude = _CurrentAmplitude / _AmplitudeHighest;
		_AmplitudeBuffer = _CurrentAmplitudeBuffer / _AmplitudeHighest;
	}
}
