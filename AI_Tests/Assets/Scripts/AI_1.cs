using UnityEngine;
using System.Collections;

public class AI_1 : MonoBehaviour {

    public enum Move_States { NONE, MOVE, TURN_RIGHT, TURN_LEFT, MOVE_DIR, MOVE_TEMP_POS, COMP_POS };

    public GameObject target;
    public float speed;
    public float distance;
    public float rayDistance;
    public float rayDistance_Sides;
    public float turn_Rate;
    public Move_States move_States = Move_States.MOVE;

    private Vector3 temp_Pos; // Needed to store a temporary position
                              // for the gameobject to move towards
    private Vector3 temp_Pos_L; // Needed to store a temporary position
                                // for the gameobject to move towards
    private Vector3 temp_Pos_R; // Needed to store a temporary position
                                // for the gameobject to move towards

    private float distance_Target = float.MaxValue;
    private Vector3 ori_Rot;
    
    // Update is called once per frame
    void Update () {
        MOVE();
	}

    bool MOVE()
    {
        if (Vector3.Distance(transform.position, target.transform.position) < distance)
        {
            //TODO: Reached target.
        }
        else
        {
            if(move_States == Move_States.MOVE)
            {
                if (!drawRay_ToTarget(0.5f) && !drawRay_ToTarget(-0.5f))
                {
                    Look_At_Target(target.transform.position);
                    transform.Translate(0, 0, speed);
                }
                else
                {
                    distance_Target = float.MaxValue;
                    move_States = Move_States.TURN_RIGHT;
                    ori_Rot = transform.rotation.eulerAngles;
                }
            }
            else if (move_States == Move_States.TURN_RIGHT)
            {
                Vector3 angle = transform.rotation.eulerAngles; //<--- Getting the euler angle from the AI transform
                angle = new Vector3(angle.x, angle.y + 1, angle.z); //<--- Adding one degree to the angle
                TURN(angle);

                if (!drawRay_ToTarget(0.5f) && !drawRay_ToTarget(-0.5f))
                {
                    move_States = Move_States.TURN_LEFT;
                    temp_Pos_R = temp_Pos;
                    transform.rotation = Quaternion.Euler(ori_Rot);
                }
            }
            else if (move_States == Move_States.TURN_LEFT)
            {
                Vector3 angle = transform.rotation.eulerAngles; //<--- Getting the euler angle from the AI transform
                angle = new Vector3(angle.x, angle.y - 1, angle.z); //<--- Subtracting one degree to the angle
                TURN(angle);

                if (!drawRay_ToTarget(0.5f) && !drawRay_ToTarget(-0.5f))
                {
                    move_States = Move_States.MOVE_TEMP_POS;
                    temp_Pos_L = temp_Pos;

                    float dist_R = Vector3.Distance(target.transform.position, temp_Pos_R);
                    float dist_L = Vector3.Distance(target.transform.position, temp_Pos_L);

                    if(dist_R < dist_L)
                    {
                        temp_Pos = temp_Pos_R;
                    }
                }
            }
            else if(move_States == Move_States.MOVE_TEMP_POS)
            {
                if (Vector3.Distance(transform.position, temp_Pos) < distance)
                {
                    move_States = Move_States.MOVE;
                }
                else
                {
                    Look_At_Target(temp_Pos);
                    transform.Translate(0, 0, speed);
                }
            }
        }

        return false;
    }

    void TURN(Vector3 angle)
    {
        Quaternion target = Quaternion.identity;
        target = Quaternion.Euler(angle);
        transform.rotation = Quaternion.RotateTowards(transform.rotation, target, turn_Rate * Time.deltaTime);
    }

    void Look_At_Target(Vector3 pos)
    {
        transform.LookAt(pos);
    }

    /// <summary>
    /// Draws a ray towards the target. Also checks if any obstacles are in the way.
    /// </summary>
    /// <param name="x">Changing the x axis origin of the ray.</param>
    /// <returns>true = obstacles are in the way.</returns>
    bool drawRay_ToTarget(float x)
    {
        float distance = Vector3.Distance(transform.position, target.transform.position);

        RaycastHit hit;
        // Vector3 newPos = transform.TransformPoint(-Vector3.forward * 0.0f); // for moving the origin point of the ray.
        Vector3 newPos = transform.TransformPoint(Vector3.right * x); // for moving the origin point of the ray sideways.
        newPos = new Vector3(newPos.x, newPos.y, newPos.z); // If any changes needed to be done.
        Vector3 dir = Quaternion.AngleAxis(0, transform.up) * transform.forward * distance;
        Ray ray = new Ray(newPos, dir);
        Debug.DrawRay(newPos, dir);

        if (Physics.Raycast(ray, out hit, distance))
        {
            if (detect_Obstacles_Layer(hit.transform.gameObject.layer))
            {
                Debug.Log("Left Diagonal hit " + hit.transform.gameObject.name);
                return true;
            }
        }

        temp_Pos = ray.origin + (ray.direction * distance);

        return false;
    }

    bool drawRay_ToCollision(float x)
    {
        return false;
    }

    /// <summary>
    /// Draws a ray.
    /// </summary>
    /// <param name="angle">The angle of the ray.</param>
    /// <param name="isRayDistance">true = use rayDistance, false = use rayDistance_Sides</param>
    /// <returns>true = found obstacles</returns>
    bool drawRay(float angle, bool isRayDistance)
    {
        float ray_Dist = 0;

        if (isRayDistance)
        {
            ray_Dist = rayDistance;
        }
        else
        {
            ray_Dist = rayDistance_Sides;
        }

        RaycastHit hit;
        Vector3 newPos = transform.TransformPoint(-Vector3.forward * 0.0f);
        newPos = new Vector3(newPos.x, newPos.y, newPos.z);
        Vector3 dir = Quaternion.AngleAxis(angle, transform.up) * transform.forward * ray_Dist;
        Ray ray = new Ray(newPos, dir);
        Debug.DrawRay(newPos, dir);

        

        if (Physics.Raycast(ray, out hit, ray_Dist))
        {
            if (detect_Obstacles_Layer(hit.transform.gameObject.layer))
            {
                Debug.Log("Left Diagonal hit " + hit.transform.gameObject.name);
                return true;
            }
        }

        return false;
    }

    bool detect_Obstacles_Layer(LayerMask layer)
    {
        if (layer == LayerMask.NameToLayer("Obstacles"))
        {
            return true;
        }

        return false;
    }
}
