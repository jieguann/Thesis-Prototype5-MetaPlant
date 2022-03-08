///\\\==========================================///\\\
///\\\ Title    -   LSystemsGenerator.cs        ///\\\
///\\\ Author   -   Peter Phillips              ///\\\
///\\\ Date     -   First entry:    08.10.18    ///\\\
///\\\              Lastentry:      04.12.18    ///\\\
///\\\==========================================///\\\


using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using System;

public class LSystemsGenerator : MonoBehaviour
{
    public static int NUM_OF_TREES = 8;
    public static int MAX_ITERATIONS = 10;
    public int MAX_GROUWITERACTIONS = 7;

    public float baseScale = 0.1f;
    public int title = 1;
    public int iterations = 4;
    public float angle = 30f;
    public float width = 1f;
    public float length = 2f;
    public float variance = 10f;
    public bool hasTreeChanged = false;
    public GameObject Tree = null;
    public float treeSize;
    
    [SerializeField] private GameObject treeParent;
    [SerializeField] private GameObject branch;
    [SerializeField] private GameObject leaf;
    [SerializeField] private GameObject fireParticles;
    [SerializeField] private GameObject rainEffectParticles;
    [SerializeField] private GameObject otherEffectParticles;
    
    public LightControl control;
    //[SerializeField] private GameObject dynamicObject;
    [SerializeField] private HUDScript HUD;

    private const string axiom = "X";

    private Dictionary<char, string> rules;
    private Stack<TransformInfo> transformStack;
    private int titleLastFrame;
    private int iterationsLastFrame;
    private float angleLastFrame;
    private float widthLastFrame;
    private float lengthLastFrame;
    private string currentString = string.Empty;
    private Vector3 initialPosition = Vector3.zero;
    private float[] randomRotationValues = new float[100];


    private string cloneString = "[";
    private string cloneStringFrame;
    private int currentPosition = 1;
    private void Start()
    {
        titleLastFrame = title;
        iterationsLastFrame = iterations;
        angleLastFrame = angle;
        widthLastFrame = width;
        lengthLastFrame = length;

        for (int i = 0; i < randomRotationValues.Length; i++)
        {
            randomRotationValues[i] = UnityEngine.Random.Range(-1f, 1f);
        }

        transformStack = new Stack<TransformInfo>();

        rules = new Dictionary<char, string>
        {
            { 'X', "[F-[[X]+X]+F[+FX]-X]" },
            { 'F', "FF" }
        };

        Generate();
    }

    private void Update()
    {   
        if (HUD.hasGenerateBeenPressed || Input.GetKeyDown(KeyCode.G))
        {
            ResetRandomValues();
            HUD.hasGenerateBeenPressed = false;
            Generate();
        }

        if (HUD.hasResetBeenPressed || Input.GetKeyDown(KeyCode.R))
        {
            ResetTreeValues();
            HUD.hasResetBeenPressed = false;
            HUD.Start();
            Generate();
        }

        if (titleLastFrame != title)
        {
            if (title >= 6)
            {
                HUD.rotation.gameObject.SetActive(true);
            }
            else
            {
                HUD.rotation.value = 0;
                HUD.rotation.gameObject.SetActive(false);
            }

            switch (title)
            {
                case 1:
                    SelectTreeOne();
                    break;

                case 2:
                    SelectTreeTwo();
                    break;

                case 3:
                    SelectTreeThree();
                    break;

                case 4:
                    SelectTreeFour();
                    break;

                case 5:
                    SelectTreeFive();
                    break;

                case 6:
                    SelectTreeSix();
                    break;

                case 7:
                    SelectTreeSeven();
                    break;

                case 8:
                    SelectTreeEight();
                    break;

                default:
                    SelectTreeOne();
                    break;
            }

            titleLastFrame = title;
        }

        if (iterationsLastFrame != iterations)
        {
            if (iterations >= 6)
            {
                HUD.warning.gameObject.SetActive(true);
                StopCoroutine("TextFade");
                StartCoroutine("TextFade");
            }
            else
            {
                HUD.warning.gameObject.SetActive(false);
            }
        }

        if (iterationsLastFrame != iterations ||
                angleLastFrame  != angle ||
                widthLastFrame  != width ||
                lengthLastFrame != length
                )
        {
            ResetFlags();
            Generate();
            //animateTree();
        }

        

    }
    public void animateTree()
    {
        //cloneString = axiom;
        //cloneString += currentString[currentPosition++];
        cloneString = currentString;
        Debug.Log(cloneString);

        for (int i = 0; i < cloneString.Length; i++)
        {
            switch (cloneString[i])
            {
                case 'F':
                    initialPosition = transform.position;
                    transform.Translate(Vector3.up * 2 * length);

                    GameObject fLine = cloneString[(i + 1) % cloneString.Length] == 'X' || cloneString[(i + 3) % cloneString.Length] == 'F' && cloneString[(i + 4) % cloneString.Length] == 'X' ? Instantiate(leaf) : Instantiate(branch);
                    fLine.transform.SetParent(Tree.transform);
                    fLine.GetComponent<LineRenderer>().SetPosition(0, initialPosition);
                    fLine.GetComponent<LineRenderer>().SetPosition(1, transform.position);
                    fLine.GetComponent<LineRenderer>().startWidth = width;
                    fLine.GetComponent<LineRenderer>().endWidth = width;

                    var fLineC = fLine.transform.Find("Sphere");
                    if (fLineC != null)
                    {
                        fLine.gameObject.SetActive(true);
                        fLineC.position = initialPosition;

                        //if (iterations > MAX_GROUWITERACTIONS)


                        fLineCScale(iterations * baseScale);
                        particleControl(iterations * baseScale * 5f);
                        /*
                        fLineCScale(2, 2* baseScale);
                        fLineCScale(3, 3* baseScale);
                        fLineCScale(4, 4* baseScale);
                        fLineCScale(5, 5* baseScale);
                        fLineCScale(6, 6* baseScale);
                        fLineCScale(7, 7* baseScale);
                        fLineCScale(8, 8* baseScale);
                        fLineCScale(9, 9* baseScale);
                        fLineCScale(10, 10* baseScale);
                        */
                        void fLineCScale(float scale)
                        {
                            fLineC.localScale = new Vector3(scale, scale, scale);

                        }

                        void particleControl(float scale)
                        {
                            if (iterations > 5 && iterations < 9)
                            {
                                fireParticles.SetActive(true);
                                fireParticles.transform.localScale = new Vector3(scale, scale, scale);
                                rainEffectParticles.SetActive(false);
                                otherEffectParticles.SetActive(false);

                                //control.updateLight(0.7109f, 0.4231f);
                                //control.HttpPutLight(0.7109f, 0.4231f);
                            }
                            else if (iterations >= 9)
                            {
                                rainEffectParticles.SetActive(true);
                                fireParticles.SetActive(false);
                                otherEffectParticles.SetActive(false);
                                //control.HttpPutLight(0f, 0f);
                            }
                            else
                            {
                                fireParticles.SetActive(false);
                                rainEffectParticles.SetActive(false);
                                otherEffectParticles.SetActive(true);

                            }
                        }
                    }




                    /*
                    if(fLine.transform == leaf.transform)
                    {
                        GameObject fObject = Instantiate(dynamicObject, Tree.transform);
                        fObject.transform.position = initialPosition;
                    }
                    */





                    break;

                case 'X':
                    break;

                case '+':
                    transform.Rotate(Vector3.back * angle * (1 + variance / 100 * randomRotationValues[i % randomRotationValues.Length]));
                    break;

                case '-':
                    transform.Rotate(Vector3.forward * angle * (1 + variance / 100 * randomRotationValues[i % randomRotationValues.Length]));
                    break;

                case '*':
                    transform.Rotate(Vector3.up * 120 * (1 + variance / 100 * randomRotationValues[i % randomRotationValues.Length]));
                    break;

                case '/':
                    transform.Rotate(Vector3.down * 120 * (1 + variance / 100 * randomRotationValues[i % randomRotationValues.Length]));
                    break;

                case '[':
                    transformStack.Push(new TransformInfo()
                    {
                        position = transform.position,
                        rotation = transform.rotation
                    });
                    break;

                case ']':
                    TransformInfo ti = transformStack.Pop();
                    transform.position = ti.position;
                    transform.rotation = ti.rotation;
                    break;

                default:
                    throw new InvalidOperationException("Invalid L-tree operation");
            }
        }
    }
    private void Generate()
    {
        Destroy(Tree);

        Tree = Instantiate(treeParent);
        Tree.transform.localScale = new Vector3(treeSize, treeSize, treeSize);

        currentString = axiom;
        
        //currentPosition = currentString.Length - 1;
        StringBuilder sb = new StringBuilder();

        if (iterations <= MAX_GROUWITERACTIONS)
        {
            for (int i = 0; i < iterations; i++)
            {
                foreach (char c in currentString)
                {
                    sb.Append(rules.ContainsKey(c) ? rules[c] : c.ToString());
                }

                currentString = sb.ToString();
                sb = new StringBuilder();
            }
        }
        else
        {
            for (int i = 0; i < MAX_GROUWITERACTIONS; i++)
            {
                foreach (char c in currentString)
                {
                    sb.Append(rules.ContainsKey(c) ? rules[c] : c.ToString());
                }

                currentString = sb.ToString();
                sb = new StringBuilder();
            }
        }


        //cloneString = currentString;


        Debug.Log(currentString);


        animateTree();
        Tree.transform.rotation = Quaternion.Euler(0, HUD.rotation.value, 0);
    }

    private void SelectTreeOne()
    {
        rules = new Dictionary<char, string>
        {
            { 'X', "[F-[X+X]+F[+FX]-X]" },
            { 'F', "FF" }
        };

        Generate();
    }

    private void SelectTreeTwo()
    {
        rules = new Dictionary<char, string>
        {
            { 'X', "[-FX][+FX][FX]" },
            { 'F', "FF" }
        };

        Generate();
    }

    private void SelectTreeThree()
    {
        rules = new Dictionary<char, string>
        {
            { 'X', "[-FX]X[+FX][+F-FX]" },
            { 'F', "FF" }
        };

        Generate();
    }

    private void SelectTreeFour()
    {
        rules = new Dictionary<char, string>
        {
            { 'X', "[FF[+XF-F+FX]--F+F-FX]" },
            { 'F', "FF" }
        };

        Generate();
    }

    private void SelectTreeFive()
    {
        rules = new Dictionary<char, string>
        {
            { 'X', "[FX[+F[-FX]FX][-F-FXFX]]" },
            { 'F', "FF" }
        };

        Generate();
    }

    private void SelectTreeSix()
    {
        rules = new Dictionary<char, string>
        {
            { 'X', "[F[+FX][*+FX][/+FX]]" },
            { 'F', "FF" }
        };

        Generate();
    }

    private void SelectTreeSeven()
    {
        rules = new Dictionary<char, string>
        {
            { 'X', "[*+FX]X[+FX][/+F-FX]" },
            { 'F', "FF" }
        };

        Generate();
    }

    private void SelectTreeEight()
    {
        rules = new Dictionary<char, string>
        {
            { 'X', "[F[-X+F[+FX]][*-X+F[+FX]][/-X+F[+FX]-X]]" },
            { 'F', "FF" }
        };

        Generate();
    }

    private void ResetRandomValues()
    {
        for (int i = 0; i < randomRotationValues.Length; i++)
        {
            randomRotationValues[i] = UnityEngine.Random.Range(-1f, 1f);
        }
    }

    private void ResetFlags()
    {
        iterationsLastFrame = iterations;
        angleLastFrame = angle;
        widthLastFrame = width;
        lengthLastFrame = length;
        cloneStringFrame = cloneString;
    }

    private void ResetTreeValues()
    {
        iterations = 4;
        angle = 30f;
        width = 1f;
        length = 2f;
        variance = 10f;
    }

    IEnumerator TextFade()
    {
        Color c = HUD.warning.color;

        float TOTAL_TIME = 4f;
        float FADE_DURATION = .25f;

        for (float timer = 0f; timer <= TOTAL_TIME; timer += Time.deltaTime)
        {
            if (timer > TOTAL_TIME - FADE_DURATION)
            {
                c.a = (TOTAL_TIME - timer) / FADE_DURATION;
            }
            else if (timer > FADE_DURATION)
            {
                c.a = 1f;
            }
            else
            {
                c.a = timer / FADE_DURATION;
            }

            HUD.warning.color = c;

            yield return null;
        }
    }
}