using System;
using ClassicLogic.Engine;

namespace LoderRunnerGame
{
    public partial class LoderRunner
    {
      public double RUNNER_SPEED = 0.65;  
      public double DIG_SPEED = 0.68;    //for support champLevel, change DIG_SPEED and FILL_SPEED, 2014/04/12
      public double FILL_SPEED = 0.24;
      public double GUARD_SPEED = 0.3;

      public double FLASH_SPEED = 0.25; //for flash cursor (hi-score input mode)

      public VersionSpeed[] spriteSpeed = Constants.SPEED_VERSION; 
      public double COVER_PROGRESS_BAR_H = 32;
      public double COVER_PROGRESS_UNDER_Y = 90;
      public double COVER_RUNNER_UNDER_Y = 136;
      public double COVER_SIDE_X = 56;		

      public double SIGNET_UNDER_X = 30;
      public double SIGNET_UNDER_Y = 30;


      public static string VERSION = "2.21c";
      public static int AI_VERSION = 4;

      public static int NO_OF_TILES_X = Constants.NO_OF_TILES_X;
      public static int NO_OF_TILES_Y = Constants.NO_OF_TILES_Y;

      public static int BASE_TILE_X = 40;
      public static int BASE_TILE_Y = 44;

      public static int GROUND_TILE_X = 40;
      public static int GROUND_TILE_Y = 20;
      public static int TEXT_TILE_X = 40;
      public static int TEXT_TILE_Y = 44;

      public static int BASE_SCREEN_X = (NO_OF_TILES_X * BASE_TILE_X);
      public static int BASE_SCREEN_Y = (NO_OF_TILES_Y * BASE_TILE_Y + GROUND_TILE_Y + TEXT_TILE_Y);

      public static float MIN_SCALE = 0.5f;
      public static float MAX_SCALE = 3f;

      public static int MENU_ICON_X = 40;
      public static int MENU_ICON_Y = 36;
      public static int ICON_BORDER = 4;
      public static int BASE_ICON_X = (MENU_ICON_X + ICON_BORDER * 2);

      

      public static int SCORE_COMPLETE_LEVEL = 1500, SCORE_COUNTER = 15;
        SCORE_GET_GOLD = 250,
        SCORE_IN_HOLE = 75,
        SCORE_GUARD_DEAD = 75;

      public static int SCORE_VALUE_PER_POINT = 100; //for modern & edit mode

        
      public static int  CLOSE_SCREEN_SPEED = 35; //20 ~ 80

    public static int  MAX_OLD_GUARD = Constants.MAX_OLD_GUARD;   //maximum number of guards for AI Version 1 and 2
    public static int  MAX_NEW_GUARD = Constants.MAX_NEW_GUARD;   //for AI Version >= 3

      public static int  RUNNER_LIFE = 5;   //number of runner life
      public static int  RUNNER_MAX_LIFE = 100;

      public static int  MAX_TIME_COUNT = 999; //for moden mode
      public static int  TICK_COUNT_PER_TIME = 16;
      public static int  MAX_DEMO_WAIT_COUNT = 200 * TICK_COUNT_PER_TIME;

      public static int  MAX_EDIT_LEVEL = 120;

      public static int  MAX_HISCORE_RECORD = 10, MAX_HISCORE_NAME_LENGTH = 12;

      //===========================
      // Define value for playData 
      //===========================
      public static int PLAY_DATA_USERDEF = 999;

      //================================================
      // Define themes string also as theme folder name 
      //================================================
      public static string  THEME_APPLE2 = "APPLE2"; 
      public static string  THEME_C64    = "C64";

      //================================
      // Below defined for LocalStorage
      //================================

      public static string STORAGE_LASTPLAY_MODE = "loderunner_lastplay";

      public static string  STORAGE_CLASSIC_INFO = "loderunner_classicInfo";

      public static string  STORAGE_MODERN_INFO = "loderunner_modernInfo";

      public static string  STORAGE_DEMO_INFO = "loderunner_demoInfo";

      public static string  STORAGE_FIRST_PLAY  = "loderunner_firstRun";

      public static string  STORAGE_MODERN_SCORE_INFO = "loderunner_modernScore";

      public static string  STORAGE_USER_INFO = "loderunner_userInfo"; //user created 
      public static string  STORAGE_USER_SCORE_INFO = "loderunner_userScore"; //user created

      public static string  STORAGE_EDIT_INFO = "loderunner_editInfo";

      public static string  STORAGE_USER_LEVEL = "loderunner_userLevel";
      public static string  STORAGE_TEST_LEVEL  = "loderunner_testlevel";

      public static string  STORAGE_HISCORE_INFO = "loderunner_hiScore";

      public static string  STORAGE_PLAYER_NAME = "loderunner_player";
      public static string  STORAGE_UID = "loderunner_uid";

      public static string  STORAGE_THEME_MODE = "loderunner_theme";
      public static string  STORAGE_THEME_COLOR = "loderunner_color_";

      public static string  STORAGE_REPEAT_ACTION = "loderunner_actRepeat";
      public static string  STORAGE_GAMEPAD_MODE = "loderunner_gamepadMode";

    }
}