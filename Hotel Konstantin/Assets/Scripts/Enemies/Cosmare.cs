using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class Cosmare : MonoBehaviour
{
    [SerializeField] private Player Player;
    [SerializeField] private SpriteRenderer Renderer;
    [SerializeField] private Image CosmareNoise;
    [SerializeField] private AudioSource CosmareNoiseSource;
    [SerializeField] private LayerMask WallLayer;
    private bool Visioned = false;

     private Lighter[] Lighters = new Lighter[0];
    private float[] Intense = new float[0];

    private float WatchTime = 0;
    private float WatchTimer = 0;

    private bool Killing = false;

    private void OnTriggerEnter(Collider other)
    {
        Lighter lighter = other.GetComponent<Lighter>();

        if (lighter != null)
        {
            Lighters = StaticTools.ExpandMassive(Lighters, lighter);

            Intense = StaticTools.ExpandMassive(Intense, lighter._Intensity);
            lighter._Intensity = 0;
        }
    }

    public void SetInfo(Player player, Image noise)
    {
        Player = player;
        CosmareNoise = noise;

        WatchTime = 4 - 3 * Game._HotelMadness;
        WatchTimer = WatchTime;
    }

    private void OnDestroy()
    {
        Visioned = true;
        CosmareNoise.color = new Color(1, 0, 0, 0);

        for(int i = 0; i < Lighters.Length; i++)
        {
            Lighters[i]._Intensity = Intense[i];
        }
    }

    private void Update()
    {
        if (Pause._Paused)
        {
            return;
        }

        transform.localEulerAngles = new Vector3(0, Quaternion.LookRotation(Player.transform.position - transform.position).eulerAngles.y, 0);

        if (Renderer.isVisible)
        {
            Transform camera = Camera.main.transform;
            if (Physics.Raycast(camera.position, transform.position + new Vector3(0, 2, 0) - camera.position, Vector3.Distance(camera.position, transform.position + new Vector3(0, 2, 0)), WallLayer))
            {
                if (Visioned)
                {
                    Destroy(gameObject);
                }
                return;
            }

            if (!Visioned)
            {
                CosmareNoise.color = new Color(1, 0, 0, 1 - WatchTimer / WatchTime);
                Visioned = true;

                CosmareNoiseSource.Play();
                CosmareNoiseSource.time = Random.Range(0, CosmareNoiseSource.clip.length - 3);
            }

            WatchTimer -= Time.deltaTime;
            CosmareNoise.color = new Color(1, 0, 0, 1 - WatchTimer / WatchTime);

            if(Vector3.Distance(Player.transform.position, transform.position) < 0.25f && Game._HotelMadness > 0.5f)
            {
                Killing = true;
                StartCoroutine(Kill());
            }

            if (WatchTimer <= 0)
            {
                Killing = true;
                StartCoroutine(Kill());
            }
        }
        else if (Visioned && Game._HotelMadness < 0.5f && !Killing)
        {
            Destroy(gameObject);
        }
    }

    private IEnumerator Kill()
    {
        CosmareNoise.color = new Color(1, 0, 0, 1);

        yield return new WaitForSecondsRealtime(1);

#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
                Application.Quit();
#endif
    }
}
