using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Skeleton : MonoBehaviour
{
    //Variable para saber si está vigilando
    private bool isLooking = false;
    //Variable para saber si se está moviendo
    private bool isMoving = true;
    //Variable para saber si se le puede golpear
    [HideInInspector] public static bool hittable = false;
    //Variable para saber que ha sido golpeado
    [HideInInspector] public static bool hitted = false;
    //Variable de la velocidad del esqueleto
    private float speed = 2f;
    //Variable para acceder al animator
    private Animator animator;
    //Variable para acceder al spriteRenderer
    private SpriteRenderer spriteRenderer;
    //Variables para las llamas de la antorcha
    [SerializeField] GameObject[] flames;
    [SerializeField] GameObject[] flamesParent;
    //Variable para el transform del jugador
    private Transform player;
    //Variable para acceder al NavMesh
    private NavMeshAgent agente;
    //Tiempo para volver a normal después de ver al player
    private int afterSeenTime = 3;
    //Variable para saber si está vigilando
    private bool guarding = true;
    //Posiciónes de vigía 
    private Vector3 guardingPoint1;
    private Vector3 guardingPoint2;
    //Variable para saber si el player se ha escapado
    private bool gotAway = true;
    //Variable para saber si el esqueleto está en posición para volver a vigilar
    private bool onPosition = true;
    //Variable transform del raycast para ver al personaje
    private Transform detect;
    //Variable para el array de raycasts
    [SerializeField] GameObject[] detectors;
    private bool caught = false;
    [HideInInspector] public static bool knockedDown = false;
    private AudioSource audioSource;
    [SerializeField] AudioClip hittedSound;
    [SerializeField] AudioSource surprisedSource;
    [SerializeField] AudioSource hitSource;
    [SerializeField] AudioSource knockedSource;
    void Start()
    {
        //Accedemos a los componentes animator, sprite renderer y nav mesh
        animator = GetComponent<Animator>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        agente = GetComponent<NavMeshAgent>();
        audioSource = GetComponent<AudioSource>();
        //Accedemos al transform del player
        player = GameObject.Find("Player").GetComponent<Transform>();
        //Accedemos al transform del empty que tiene el raycast
        detect = GameObject.Find("SkelEyes").GetComponent<Transform>();
        //Asignamos los vectores donde irá el esqueleto cuando pierda al personaje de vista
        guardingPoint1 = new Vector3(12.5f, 0f, 0f);
        guardingPoint2 = new Vector3(21f, 0f, 0f);
        detectors[0].SetActive(false);
        detectors[2].SetActive(false);
        gotAway = true;
    }
    void Update()
    {
        if (knockedDown == true)
        {
            flamesParent[0].SetActive(false);
            flamesParent[1].SetActive(false);
        }
        if (!PlayerScript.isAlive)
        {
            audioSource.Stop();
        }
            if (!PlayerScript.isAlive || hitted == true)
            {
                speed = 0;
                agente.speed = 0;
                agente.destination = transform.position;
                if (!caught)
                {
                    animator.SetTrigger("Idle");
                    caught = true;
                }
            }
        if (!hitted)
        {
            if (transform.position.x > 16 && transform.position.x < 18)
            {
                hittable = true;
            }
            else
            {
                hittable = false;
            }
            if (animator.GetCurrentAnimatorStateInfo(0).IsName("Alert"))
            {
                agente.speed = 0;
            }
            else
            {
                agente.speed = 3;
            }
            //Asignamos el valor del animator para los sprites de andar
            animator.SetBool("Moviendo", isMoving);
            //Si el esqueleto está en estado neutral empieza a patrullar
            if (!isLooking && !Detector.spotted && !Detector.almostSpotted && guarding && onPosition)
            {
                //Metodo para patrullar
                Pathing();
            }
            //Comprobamos si detecta al personaje
            if (Detector.spotted)
            {
                animator.SetBool("Vigilando", false);
                if (gotAway == true)
                {
                    print("Gasp");
                    audioSource.Stop();
                    surprisedSource.Play();
                    animator.SetTrigger("Alertado");
                }
                //Cambiamos las variables para saber que el personaje ha sido descubierto y el esqueleto ya no está patrullando
                gotAway = false;
                //Paramos la corrutina para que no deje de buscar
                StopCoroutine(Follow());
            }
            //Si no detecta al personaje empezamos la corrutina para que pasados 3 segundos deje de buscar
            else if (!Detector.spotted && !gotAway && !Detector.almostSpotted)
            {
                StartCoroutine(Follow());
            }
            //Si está a media distancia el esqueleto se para y busca
            if (Detector.almostSpotted)
            {
                animator.SetBool("Vigilando", true);
                if (animator.GetCurrentAnimatorStateInfo(0).IsName("Guarding") == true)
                {
                    audioSource.Stop();
                }
                StartCoroutine(Suspicius());
            }
            else if (!Detector.almostSpotted)
            {
                if (audioSource.isPlaying == false)
                {
                    audioSource.Play();
                }
                animator.SetBool("Vigilando", false);
            }
            //Comprobamos si el esqueleto está viendo al personaje
            if (!gotAway)
            {
                if (audioSource.isPlaying == false)
                {
                    audioSource.Play();
                }
                Detector.spotted = true;
                //Le decimos que no está en posición de patrulla
                onPosition = false;
                //Le hacemos seguir al jugador
                agente.isStopped = false;
                if (PlayerScript.isAlive)
                {
                    agente.destination = player.position;
                }
                //Le decimos que está moviéndose para las animaciones
                isMoving = true;
                //Le decimos que no está vigilando para que no siga su rutina de patrulla
                guarding = false;
                StopCoroutine(Suspicius());
                //Le decimos que no está buscando al personaje y cambiamos el animator
                isLooking = false;
                animator.SetBool("Vigilando", isLooking);
                //Paramos la corrutina de vigilancia
                //StopCoroutine(Guarding());
                //Activamos el raycast que sigue al player
                detectors[0].SetActive(true);
                detectors[1].SetActive(false);
                detectors[2].SetActive(false);
                //Hacemos que el raycast siga al jugador
                detect.transform.LookAt(player);
                //Cambiamos el sprite para que se adecúe a donde esté mirando
                if (player.position.x < transform.position.x)
                {
                    if (transform.rotation.y > 90)
                    {
                        spriteRenderer.flipX = false;
                    }
                    else if (transform.rotation.y < 90)
                    {
                        spriteRenderer.flipX = true;
                    }
                }
                else if (player.position.x > transform.position.x)
                {
                    if (transform.rotation.y > 90)
                    {
                        spriteRenderer.flipX = true;
                    }
                    else if (transform.rotation.y < 90)
                    {
                        spriteRenderer.flipX = false;
                    }
                }
            }
            //Comprobamos si se ha escapado y en que zona está el esqueleto después de perseguir al jugador
            else if (gotAway && transform.position.x < 17 && !onPosition)
            {
                //El esqueleto va al punto de vigilancia de la izquierda
                agente.destination = guardingPoint1;
                spriteRenderer.flipX = true;
            }
            else if (gotAway && transform.position.x > 17 && !onPosition)
            {
                //El esqueleto va al punto de vigilancia de la derecha
                agente.destination = guardingPoint2;
                spriteRenderer.flipX = false;
            }
            //Empezamos el método de las llamas
            FlamesSprites();
        }
        if (hitted == true && !knockedDown)
        {
            knockedDown = true;
            audioSource.Stop();
            hitSource.Play();
            animator.SetTrigger("Golpeado");
            transform.position = new Vector3(17.5f, 0.0f, -1.1f);
            Invoke("KnockedSound", hittedSound.length);
        }
    }
    void Pathing()
    {
        //Hacemos que el esqueleto se mueva
        transform.Translate(Vector2.left * speed * Time.deltaTime);
        isMoving = true;
        if (speed == 2)
        {
            spriteRenderer.flipX = false;
        }
        else if (speed == -2)
        {
            spriteRenderer.flipX = true;
        }
    }
    private void OnTriggerEnter(Collider other)
    {
        if (guarding == true)
        {
            if (other.gameObject.tag == "Limit")
            {
                speed = speed * -1;
                if (spriteRenderer.flipX == true)
                {
                    spriteRenderer.flipX = false;
                    detectors[0].SetActive(false);
                    detectors[1].SetActive(true);
                    detectors[2].SetActive(false);
                }
                else if (spriteRenderer.flipX == false)
                {
                    spriteRenderer.flipX = true;
                    detectors[0].SetActive(false);
                    detectors[1].SetActive(false);
                    detectors[2].SetActive(true);
                }
            }
        }
        if (other.gameObject.tag == "onPoint" && !onPosition && gotAway)
        {
            onPosition = true;
            guarding = true;
            isLooking = false;
            Invoke("Pathing", 0.1f);
            agente.isStopped = true;
            spriteRenderer.flipX = false;
            if (other.gameObject.name == "OnPoint1")
            {
                detectors[0].SetActive(false);
                detectors[1].SetActive(true);
                detectors[2].SetActive(false);
            }
            else if (other.gameObject.name == "OnPoint2")
            {
                detectors[0].SetActive(false);
                detectors[1].SetActive(false);
                detectors[2].SetActive(true);
            }
        }
    }
    private void FlamesSprites()
    {
        if (!spriteRenderer.flipX)
        {
            flamesParent[0].SetActive(true);
            flamesParent[1].SetActive(false);
            if (!isMoving && !isLooking)
            {
                flames[0].SetActive(true);
                flames[1].SetActive(false);
                flames[2].SetActive(false);
                flames[3].SetActive(false);
            }
            else if (isMoving && !isLooking)
            {
                flames[0].SetActive(false);
                flames[1].SetActive(false);
                flames[2].SetActive(false);
                flames[3].SetActive(true);
            }
            else if (isMoving && isLooking)
            {
                flames[0].SetActive(false);
                flames[1].SetActive(true);
                flames[2].SetActive(false);
                flames[3].SetActive(false);
            }
        }
        else if (spriteRenderer.flipX)
        {
            flamesParent[0].SetActive(false);
            flamesParent[1].SetActive(true);
            if (!isMoving && !isLooking)
            {
                flames[4].SetActive(true);
                flames[5].SetActive(false);
                flames[6].SetActive(false);
                flames[7].SetActive(false);
            }
            else if (isMoving && !isLooking)
            {
                flames[4].SetActive(false);
                flames[5].SetActive(false);
                flames[6].SetActive(false);
                flames[7].SetActive(true);
            }
            else if (isMoving && isLooking)
            {
                flames[4].SetActive(false);
                flames[5].SetActive(true);
                flames[6].SetActive(false);
                flames[7].SetActive(false);
            }
        }
    }
    IEnumerator Follow()
    {
        yield return new WaitForSeconds(afterSeenTime);
        gotAway = true;
        StopCoroutine(Follow());
    }
    IEnumerator Suspicius()
    {
        yield return new WaitForSeconds(afterSeenTime);
        if (Detector.almostSpotted == true)
        {
            if (gotAway == true)
            {
                audioSource.Stop();
                surprisedSource.Play();
                animator.SetTrigger("Alertado");
            }
            yield return new WaitForSeconds(0.5f);
            gotAway = false;
            Detector.spotted = true;
        }
        else
        {
            StopCoroutine(Suspicius());
        }
    }
    private void KnockedSound()
    {
        audioSource.Stop();
        knockedSource.Play();
    }
}
