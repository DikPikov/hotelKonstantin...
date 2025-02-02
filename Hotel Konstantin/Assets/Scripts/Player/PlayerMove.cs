using UnityEngine;
using System.Collections;

public class PlayerMove : MonoBehaviour
{
    [SerializeField] private LayerMask LayerMask;

    [SerializeField] private AudioSource[] WalkSounds;

    [SerializeField] private PlayerRotation Rotation;
    [SerializeField] private Transform Camera;
    [SerializeField] private Player Player;
    [SerializeField] private float Speed;
    private float CameraShake = 0;
    private float WalkNoise = 0;

    private float Fall = 0;

    private Coroutine RecoverCoroutine = null;

    private void Update()
    {
        if (Pause._Paused || Game._GameOver)
        {
            return;
        }
#if !UNITY_ANDROID
        Vector3 direction = transform.forward * InputManager.GetAxis(InputManager.AxisEnum.Vertical) + transform.right * InputManager.GetAxis(InputManager.AxisEnum.Horizontal);
#else
        Vector3 direction = transform.forward * SimpleInput.GetAxis("Vertical") + transform.right * SimpleInput.GetAxis("Horizontal");
#endif
        if(direction != Vector3.zero)
        {
            direction.y = 0;
            direction = direction.normalized;

            if(Physics.OverlapBox(transform.position + new Vector3(direction.x * 0.25f, 0.85f, 0), new Vector3(0.25f, 0.75f, 0.25f), transform.rotation, LayerMask).Length > 0)
            {
                direction.x = 0;
            }
            if (Physics.OverlapBox(transform.position + new Vector3(0, 0.85f, direction.z * 0.25f), new Vector3(0.25f, 0.75f, 0.25f), transform.rotation, LayerMask).Length > 0)
            {
                direction.z = 0;
            }



#if !UNITY_ANDROID
            float speed = (Speed + Game._HotelMadness) * Time.deltaTime;

            if (InputManager.GetButton(InputManager.ButtonEnum.Run) && direction != Vector3.zero && Player._Stamina > 0)
            {
                if (RecoverCoroutine != null)
                {
                    StopCoroutine (RecoverCoroutine);
                    RecoverCoroutine = null;
                }

                speed += (4f + 2 * Game._HotelMadness) * Time.deltaTime;
                Player._Stamina -= Time.deltaTime;
            }
            else if (Player._Stamina < 5)
            {
                if(RecoverCoroutine == null)
                {
                    RecoverCoroutine = StartCoroutine(RecoverStamina());
                }
            }
#else
            float speed = (Speed + Game._HotelMadness + 2) * Time.deltaTime;
#endif
            CameraShake = (CameraShake + speed * 120) % 360;

            WalkNoise += speed;
            if (WalkNoise > 2)
            {
                WalkNoise -= 2;


                RaycastHit hit;
                if(Physics.Raycast(transform.position + Vector3.up * 0.1f, Vector3.down, out hit, 1, LayerMask))
                {
                    Ground ground = hit.transform.GetComponent<Ground>();
                    if (ground != null)
                    {
                        int sound = 0;

                        switch (ground._Material)
                        {
                            case Ground.GroundMaterial.Wood:
                                sound = Random.Range(0, 2);
                                break;
                            case Ground.GroundMaterial.Metal:
                                sound = Random.Range(2, 4);
                                break;
                            case Ground.GroundMaterial.Beton:
                                sound = Random.Range(4, 6);
                                break;
                        }

                        WalkSounds[sound].pitch = Random.Range(0.9f, 1.1f);
                        WalkSounds[sound].Play();
                    }
                }
            }

           // Camera.localPosition = new Vector3(0, 1.6f + Mathf.Sin(CameraShake * Mathf.Deg2Rad) * 0.05f, 0);

            Vector3 rotation = Rotation._Rotation;
            rotation.z = Mathf.Sin(CameraShake * Mathf.Deg2Rad) * 1f;
            Rotation._Rotation = rotation;
            
            transform.position += direction * speed;
        }

        if(Physics.CheckSphere(transform.position, 0.04f, LayerMask))
        {
            Fall = 0;
        }
        else
        {
            if(Fall < 15)
            {
                Fall += Time.deltaTime * 10;
            }

            if(Physics.OverlapBox(transform.position + new Vector3(0, 0.49f, 0), new Vector3(0.25f, 0.5f, 0.25f), transform.rotation, LayerMask).Length < 1)
            {
                transform.position -= Vector3.up * Fall * Time.deltaTime;
            }
        }
    }

    private IEnumerator RecoverStamina()
    {
        yield return new WaitForSeconds(0.5f);

        WaitForEndOfFrame waitForEndOfFrame = new WaitForEndOfFrame();

        while (Player._Stamina < 5)
        {
            Player._Stamina += Time.deltaTime * 0.75f;

            yield return waitForEndOfFrame;
        }

        RecoverCoroutine = null;
    }
}
