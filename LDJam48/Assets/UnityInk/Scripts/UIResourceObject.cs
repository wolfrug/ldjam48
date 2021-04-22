using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UIResourceObject : MonoBehaviour
{
    public string inkVariableName;
    public string inkMaxVariableName = "";
    public TextMeshProUGUI amounttext;
    // This is not necessary, you can also manually enter a number in 'currentMaxValue'.
    public TextMeshProUGUI maxAmountText;
    public Image fillBar;
    [Tooltip("This bar is -set- rather than animated to fill. Use as alternative to fill bar, or for added effect.")]
    public Image secondaryFillBar;
    public float barFillSpeed = 0.1f;
    public bool UIManagerEffect = true;
    public Animator animator;
    public Image iconImage;
    //public int startingValue = 0;
    public int currentValue = 0;
    public float currentValueFloat = 0f;
    public int currentMaxValue = 0;
    public float currentMaxValueFloat = 0f;
    public float targetPercentage = 0f;

    [Tooltip("If the ink variable being tracked is a float it needs to be tracked as a float")]
    public bool castAsFloat = false;

    [Tooltip("The icon animates when the bar is full.")]
    public bool glowWhenFull = false;
    [Tooltip("The icon animates when the bar is empty.")]
    public bool glowWhenEmpty = false;
    [Tooltip("Determines the color of the glow at full or empty. E.g. if glow on empty and fullIsGood then Empty Is Bad = glow red.")]
    public bool fullIsGood = true;
    // Start is called before the first frame update
    void Start()
    {
        //Invoke ("LateStart", 0.5f);
    }
    public void Init()
    {
        SetValueToInkValue();
        if (inkVariableName != "")
        {
            if (castAsFloat)
            {
                InkWriter.main.story.ObserveVariable((inkVariableName), (string varName, object newValue) =>
            {
                EventListener(varName, (int)(float)newValue);
            });
            }
            else
            {
                InkWriter.main.story.ObserveVariable((inkVariableName), (string varName, object newValue) =>
                {
                    EventListener(varName, (int)newValue);
                });
            };
        }
        if (inkMaxVariableName != "")
        {
            InkWriter.main.story.ObserveVariable((inkMaxVariableName), (string varName, object newValue) =>
            {
                UpdateMaxValue();
            });
        }
    }
    public void SetValue(float newvalue)
    {
        SetValue((int)newvalue);
    }
    public void SetValue(int newvalue)
    {
        // Debug.Log ("Set value of " + gameObject.name + " to " + newvalue, gameObject);
        if (newvalue < currentValue)
        {
            if (animator != null) { animator.SetTrigger("down"); };
            if (UIManagerEffect)
            {
                //UIManager.instance.SpawnResourceEffect(inkVariableName, (newvalue - currentValue));
            };
        }
        else
        {
            if (animator != null) { animator.SetTrigger("up"); };
            if (UIManagerEffect)
            {
                //UIManager.instance.SpawnResourceEffect(inkVariableName, (newvalue - currentValue));
            };
        }
        if (animator != null) { animator.SetInteger("value", newvalue); };
        if (castAsFloat)
        {
            currentValueFloat = (float)InkWriter.main.story.variablesState[(inkVariableName)];
            currentValue = (int)currentValueFloat;
        }
        else
        {
            currentValue = (int)InkWriter.main.story.variablesState[(inkVariableName)];
            currentValueFloat = (float)currentValue;
        };
        // I think Ink sends the event twice, so...
        // Debug.Log ("Updated currentValue variable: " + currentValue + " for " + gameObject, gameObject);
        if (amounttext != null) { amounttext.text = newvalue.ToString(); };
        UpdateMaxValue();
        UpdateGlowEffect();
    }

    public void UpdateMaxValue()
    {
        if (inkMaxVariableName != "")
        {
            if (castAsFloat)
            {
                currentMaxValueFloat = (float)InkWriter.main.story.variablesState[(inkMaxVariableName)];
            }
            else
            {
                currentMaxValue = (int)InkWriter.main.story.variablesState[(inkMaxVariableName)];
            };
        }
        if (currentMaxValue > 0 || currentMaxValueFloat > 0f)
        {
            if (castAsFloat)
            {
                targetPercentage = currentValueFloat / currentMaxValueFloat;
            }
            else
            {
                targetPercentage = (float)currentValue / (float)currentMaxValue;
            };
        };
        if (maxAmountText != null)
        {
            maxAmountText.text = currentMaxValue.ToString();
        }
    }

    void UpdateGlowEffect()
    {
        if (inkMaxVariableName != "")
        { // can only do this ofc if we have a max value
            if (glowWhenEmpty && targetPercentage <= 0f)
            {
                if (fullIsGood)
                { // bad glow!
                    animator.SetBool("glow_bad", true);
                    animator.SetBool("glow_good", false);
                }
                else
                {
                    // good glow!
                    animator.SetBool("glow_bad", false);
                    animator.SetBool("glow_good", true);
                }
            }
            else if (glowWhenFull && targetPercentage >= 1f)
            {
                if (fullIsGood)
                { // good glow!
                    animator.SetBool("glow_bad", false);
                    animator.SetBool("glow_good", true);
                }
                else
                {
                    // bad glow!
                    animator.SetBool("glow_bad", true);
                    animator.SetBool("glow_good", false);
                }
            }
            else
            {
                // no glow!
                animator.SetBool("glow_bad", false);
                animator.SetBool("glow_good", false);
            }
        }
    }

    [NaughtyAttributes.Button]
    public void SetValueToInkValue()
    {
        // Debug.Log ("This is " + gameObject.name + " trying to set the value " + inkVariableName + " to its ink value in the story " + InkWriter.main.story + ". The variable is: " + InkWriter.main.story.variablesState[(inkVariableName)]);
        if (castAsFloat)
        {
            currentValueFloat = (float)InkWriter.main.story.variablesState[(inkVariableName)];
        }
        else
        {
            currentValue = (int)InkWriter.main.story.variablesState[(inkVariableName)];
        };
        if (inkMaxVariableName != "")
        {
            if (castAsFloat)
            {
                currentMaxValueFloat = (float)InkWriter.main.story.variablesState[(inkMaxVariableName)];
            }
            else
            {
                currentMaxValue = (int)InkWriter.main.story.variablesState[(inkMaxVariableName)];
            };
        }
        if (castAsFloat)
        {
            SetValue(currentValueFloat);
        }
        else
        {
            SetValue(currentValue);
        };
    }
    public void EventListener(string tag, int valuechange)
    { // from the tag listener
      // Debug.Log("New value from EventListener: tag=" + tag + " value=" + valuechange);
        SetValue(valuechange);
    }

    // Update is called once per frame
    void Update()
    {
        if (fillBar != null)
        {
            if (Mathf.Abs(targetPercentage - fillBar.fillAmount) < 0.01f)
            {
                fillBar.fillAmount = targetPercentage;
            }
            if (fillBar.fillAmount < targetPercentage)
            {
                fillBar.fillAmount += (Time.deltaTime * barFillSpeed);
            }
            else if (fillBar.fillAmount > targetPercentage)
            {
                fillBar.fillAmount -= (Time.deltaTime * barFillSpeed);
            }
            if (animator != null) { animator.SetFloat("percentage", fillBar.fillAmount); };
        }
        if (secondaryFillBar != null)
        {
            if (secondaryFillBar.fillAmount != targetPercentage)
            {
                secondaryFillBar.fillAmount = targetPercentage;
            }
            if (fillBar == null)
            {
                if (animator != null) { animator.SetFloat("percentage", secondaryFillBar.fillAmount); };
            }
        }
    }
}