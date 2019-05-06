using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

// Represents an activity that can be deleted
public class TankActivity
{
    public GameObject gameObject;
    public RectTransform transform;
    public Animator animator;
    public Text text;
    public Image image;

    public bool deleting = false;

    public void Delete ()
    {
        if (deleting) return;

        deleting = true;
        animator.Play("move out");
    }


    public bool NeedsDeletion()
    {
        // if deleting is true and the current animation has finished, we're ready to delete
        if (deleting && animator.GetCurrentAnimatorStateInfo(0).normalizedTime > 1) return true;
        return false;
    }
}

// maps the enums for the icons to a sprite
[Serializable]
public struct IconEnumToSprite
{
    public TankActivityIcon icon;
    public Sprite sprite;
}

// represents a tank activity icon
public enum TankActivityIcon
{
    MOVE_FORWARDS, MOVE_BACKWARDS, TURN_LEFT, TURN_RIGHT
}

public class TankActivityDisplay : MonoBehaviour
{
    public Canvas canvas;
    public GameObject activityPrefab;
    public IconEnumToSprite[] iconMappings;

    // height of each activity bar, used for seperating them
    public float activityHeight;

    // amount of activities that can be displayed at one time
    public int activityLimit;


    // mapping of icon enums to Sprites, is populated from the IconEnumToSprite array so it can be assigned in the editor
    Dictionary<TankActivityIcon, Sprite> icons = new Dictionary<TankActivityIcon, Sprite>();

    // list of activities being displayed right now
    List<TankActivity> activities = new List<TankActivity>();

    Tank tank;

    void Awake()
    {
        // add the inspector-friendly array to the dictionary
        foreach (IconEnumToSprite icon in iconMappings) icons.Add(icon.icon, icon.sprite);

        tank = GetComponent<Tank>();
        tank.OnTankMovementChanged += OnTankMove;
    }

    public void OnTankMove(MovementControl oldMovement, MovementControl newMovement)
    {
        // show the relevant activity when the movement has changed
        if (!oldMovement.forwards && newMovement.forwards) ShowActivity(TankActivityIcon.MOVE_FORWARDS, "Forwards", Color.white);
        if (!oldMovement.reverse && newMovement.reverse) ShowActivity(TankActivityIcon.MOVE_BACKWARDS, "Reverse", Color.white);
        if (!oldMovement.left && newMovement.left) ShowActivity(TankActivityIcon.TURN_LEFT, "Turn left", Color.white);
        if (!oldMovement.right && newMovement.right) ShowActivity(TankActivityIcon.TURN_RIGHT, "Turn right", Color.white);
    }

    // shows an activity
    public TankActivity ShowActivity(TankActivityIcon icon, string text, Color color, int secondsFor = 2)
    {
        TankActivity activity = new TankActivity();
        Sprite sprite = icons[icon];

        activity.gameObject = Instantiate(activityPrefab, canvas.transform);
        activity.animator = activity.gameObject.GetComponent<Animator>();
        activity.text = activity.gameObject.transform.GetComponentInChildren<Text>();
        activity.image = activity.gameObject.transform.Find("Symbol").GetComponent<Image>();
        activity.transform = activity.gameObject.GetComponent<RectTransform>();

        activity.image.sprite = sprite;
        activity.text.text = text;
        activity.text.color = color;

        // add the activity to the start of the list
        activities.Insert(0, activity);

        StartCoroutine(DeleteActivityAfter(activity, secondsFor));

        return activity;
    }

    public IEnumerator DeleteActivityAfter(TankActivity activity, int seconds)
    {
        yield return new WaitForSeconds(seconds);
        activity.Delete();
    }

    void FixedUpdate()
    {

        List<TankActivity> activitiesToDelete = new List<TankActivity>();

        // place all the activities in order on the canvas
        float y = 0;

        foreach (TankActivity activity in activities)
        {
            activity.transform.localPosition = new Vector2(activity.transform.localPosition.x, y);

            y += activityHeight;

            if (activity.NeedsDeletion()) activitiesToDelete.Add(activity);
        }

        // remove deleted activities from the list
        foreach (TankActivity delete in activitiesToDelete)
        {
            activities.Remove(delete);
            Destroy(delete.gameObject);
        }

        // delete the oldest activity when the limit has been reached
        if (activities.Count > activityLimit) activities[activities.Count - 1].Delete();

    }
}
