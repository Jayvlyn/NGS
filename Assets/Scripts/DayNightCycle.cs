using UnityEngine;
using UnityEngine.Rendering.Universal;

public class DayNightCycle : MonoBehaviour
{
	[Header("References")]
	public Light2D globalLight;
	public SpriteRenderer sky;

	// game hour = minute
	[Header("Time Settings")]
	public int dayHours = 5;
	public int nightHours = 7;
	public int TotalHours => dayHours + nightHours;
	public float CurrentHour => (currentTime / 60) % TotalHours;
	public float NoonHour => dayHours * 0.5f;
	public float MidnightHour => dayHours + (nightHours * 0.5f);

	[Header("Color Settings")]
	public float middayIntensity = 1f;
	public float midnightIntensity = 0.13f;
	public float DawnIntensity => (middayIntensity + midnightIntensity) * 0.5f;
	public Color noonSkyColor = Color.white;
	public Color midnightSkyColor = Color.white;

	[HideInInspector] public float currentTime;
	public static bool isNight;

	[Header("Game Settings")]
    [SerializeField] public GameSettings settings;

	private void Start()
	{
		currentTime = NoonHour * 60; // start at noon
		if(settings == null) settings = GameUI.Instance.gameSettings;
		if(!GameUI.loadScreens) currentTime = settings.position.currentTime;
	}

	bool clockPaused = false;

	private void Update()
    {

        if (Input.GetKeyDown(KeyCode.Period)) // toggle pause
		{
			clockPaused = !clockPaused;
		}

		if (Input.GetKey(KeyCode.Comma)) // step back in time
		{
			float mult = 10;
			if (Input.GetKey(KeyCode.RightShift)) mult = 100;
			currentTime -= Time.deltaTime * mult;
		}
		else if (Input.GetKey(KeyCode.Slash)) // step forward in time
		{
			float mult = 10;
			if (Input.GetKey(KeyCode.RightShift)) mult = 100;
			currentTime += Time.deltaTime * mult;
		}
		else if (!clockPaused) // pass time
		{
			currentTime += Time.deltaTime;
		}

		if (Input.GetKeyDown(KeyCode.L)) // set to noon
		{
			currentTime = NoonHour * 60;
		}
		else if (Input.GetKeyDown(KeyCode.K)) // set to midnight
		{
			currentTime = MidnightHour * 60;
		}

		settings.position.currentTime = currentTime;

		UpdateLightIntensity();
	}

	private void UpdateLightIntensity()
	{
		if (globalLight == null) return;

		float t;
		float sunriseStart = 0; // Start of sunrise
		float sunriseEnd = sunriseStart + (nightHours * 0.2f);   // End of sunrise
		float sunsetStart = NoonHour + (dayHours * 0.8f);        // Start of sunset
		float sunsetEnd = sunsetStart + (dayHours * 0.2f);       // End of sunset

		if (CurrentHour >= sunriseEnd && CurrentHour < sunsetStart) // Full daylight period
		{
			globalLight.intensity = middayIntensity;
			sky.color = noonSkyColor;
			isNight = false;
		}
		else if (CurrentHour >= sunsetStart && CurrentHour < sunsetEnd) // Sunset transition
		{
			t = (CurrentHour - sunsetStart) / (sunsetEnd - sunsetStart);
			globalLight.intensity = Mathf.Lerp(middayIntensity, midnightIntensity, t);
			sky.color = Color.Lerp(noonSkyColor, midnightSkyColor, t);
			isNight = t >= 0.5f;
		}
		else if (CurrentHour >= sunsetEnd || CurrentHour < sunriseStart) // Full nighttime period
		{
			globalLight.intensity = midnightIntensity;
			sky.color = midnightSkyColor;
			isNight = true;
		}
		else if (CurrentHour >= sunriseStart && CurrentHour < sunriseEnd) // Sunrise transition
		{
			t = (CurrentHour - sunriseStart) / (sunriseEnd - sunriseStart);
			globalLight.intensity = Mathf.Lerp(midnightIntensity, middayIntensity, t);
			sky.color = Color.Lerp(midnightSkyColor, noonSkyColor, t);
			isNight = false;
		}
	}
}