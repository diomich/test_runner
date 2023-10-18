namespace Gameplay
{
    /// <summary>
    /// World object constants
    /// </summary>
    public class WorldObjectConstants
    {
        public const string OBJ_TYPE_CHARACTER = "obj_character";
        public const string OBJ_TYPE_COIN = "obj_coin";
        public const string OBJ_TYPE_OBSTACLE = "obj_obstacle";

        public const string OBJ_TYPE_BOOST_SPEED_UP = "obj_boost_speed_up";
        public const string OBJ_TYPE_BOOST_SLOW_DOWN = "obj_boost_slow_down";
        public const string OBJ_TYPE_BOOST_FLY = "obj_boost_fly";
        
        public const float OBJ_VERTICAL_POS_GROUND = 0f;
        public const float OBJ_VERTICAL_POS_AIR = 0.5f;
        public const float BOOST_FLY_DURATION = 4f;
        
        public const float BOOST_SPEED_UP_CHANGE = 3f;
        public const float BOOST_SLOW_DOWN_CHANGE = -3f;
        public const float BOOST_SPEED_UP_DURATION = 4f;
        public const float BOOST_SLOW_DOWN_DURATION = 4f;
    }
}