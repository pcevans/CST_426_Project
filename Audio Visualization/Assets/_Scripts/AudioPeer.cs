using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent (typeof(AudioSource))]
public class AudioPeer : MonoBehaviour {

	AudioSource _audioSource;
	public static float[] _samples = new float[512];
	float[] _freqBand = new float[8];
	float[] _freqBandBuffer = new float[8];
	float[] _freqBufferDecrease = new float[8];
	public float[] _freqBandHighest = new float[8];

	public static int _avgFreqBand;

	public static float[] _audioBand = new float[8];
	public static float[] _audioBandBuffer = new float[8];

	public static float _Amplitude, _AmplitudeBuffer;
	float _AmplitudeHighest;

	void Start () {
		_audioSource = GetComponent<AudioSource> ();
	}

	void Update () {
		GetSpectrumAudioSource ();
		MakeFrequencyBands ();
		BandBuffer ();
		CreateAudioBands ();
		GetAmplitude ();
		GetAverageFrequencyBand ();
	}

	void GetAverageFrequencyBand () {
		float sum = 0;
		float denominator = 0;
		for (int i = 0; i < 8; i++) {
			denominator += _audioBand [i];
			sum += i * _audioBand [i];
		}
		sum /= 8.0f;
		_avgFreqBand = ((int)sum + _avgFreqBand) / 2;

	}

	void GetAmplitude () {
		float _CurrentAmplitude = 0;
		float _CurrentAmplitudeBuffer = 0;

		for (int i = 0; i < 8; i++) {
			_CurrentAmplitude += _audioBand [i];
			_CurrentAmplitudeBuffer += _audioBandBuffer [i];
		}

		if (_CurrentAmplitude > _AmplitudeHighest)
			_AmplitudeHighest = _CurrentAmplitude;

		_Amplitude = _CurrentAmplitude / _AmplitudeHighest;
		_AmplitudeBuffer = _CurrentAmplitudeBuffer / _AmplitudeHighest;
	}

	void CreateAudioBands () {
		for (int i = 0; i < 8; i++) {
			if (_freqBand [i] > _freqBandHighest [i]) {
				_freqBandHighest [i] = _freqBand [i];
			}
			_audioBand [i] = (_freqBand [i] / _freqBandHighest [i]);
			_audioBandBuffer [i] = (_freqBandBuffer [i] / _freqBandHighest [i]);
		}
	}

	void GetSpectrumAudioSource () {
		_audioSource.GetSpectrumData (_samples, 0, FFTWindow.Blackman);
	}

	void BandBuffer () {
		for (int i = 0; i < 8; i++) {
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

	void MakeFrequencyBands () {
		int count = 0;
		for (int i = 0; i < 8; i++) {
			float average = 0;
			int sampleCount = (int)Mathf.Pow (2, i + 1);
			if (i == 7)
				sampleCount += 2;

			for (int j = 0; j < sampleCount; j++) {
				average += _samples [count] * (count + 1);
				count++;
			}
			average /= count;

			_freqBand [i] = average * 10;
		}
	}
}
