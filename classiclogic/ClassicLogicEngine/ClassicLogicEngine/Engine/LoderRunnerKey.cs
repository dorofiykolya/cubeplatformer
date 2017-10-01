using System;

namespace LoderRunnerGame
{
  public partial class LoderRunner
  {
    public Action keyAction = Action.ACT_STOP; //// keyLastLeftRight = ACT_RIGHT;
    public int shiftLevelNum = 0;
    public int runnerDebug = 0;

    public void pressShiftKey(code)
    {
    /*	cheat key code disable 5/16/2015
      switch(code) {
      case KEYCODE_PERIOD: //SHIFT-. = '>', change to next level
        shiftLevelNum = 1;	
        gameState = GAME_NEXT_LEVEL;
        break;	
      case KEYCODE_COMMA:	 //SHIFT-, = '<', change to previous level	
        shiftLevelNum = 1;	
        gameState = GAME_PREV_LEVEL;
        break;
      case KEYCODE_UP: //SHIFT-UP, inc runner 	
        if(playMode == PLAY_CLASSIC && runnerLife < 10) { //bug fixed: add check playMode, 12/15/2014
          runnerLife++;	
          drawLife();
        }
        break;	
      case KEYCODE_X: //SHIFT-X 
        toggleTrapTile();
        break;
      case KEYCODE_G: //SHIFT-G, toggle god mode
        toggleGodMode();
        break;	
      default:		
        if(runnerDebug) debugKeyPress(code);
        break;
      }
    */	
    }

    public void pressCtrlKey(code)
    {
      switch(code) {
      case KeyCode.KEYCODE_A: //CTRL-A : abort level
        gameState = GameState.GAME_RUNNER_DEAD;	
        break;	
      case KeyCode.KEYCODE_C: //CTRL-C : copy current level
        copyLevelMap = levelData[curLevel-1];
        copyLevelPassed = 1; //means copy from exists level	
        showTipsText("COPY MAP", 1500);	
        break;	
      case KeyCode.KEYCODE_J:	//CTRL-J : gamepad toggle
        toggleGamepadMode(1);
        //if(gamepadIconObj) gamepadIconObj.updateGamepadImage();	
        break;	
      case KeyCode.KEYCODE_K: //CTRL-K : repeat actions On/Off
        toggleRepeatAction();	
        repeatActionIconObj.updateRepeatActionImage();	
        break;	
      case KeyCode.KEYCODE_R: //CTRL-R : abort game
        runnerLife = 1;	
        gameState = GameState.GAME_RUNNER_DEAD;	
        break;	
      case KeyCode.KEYCODE_X: //CTRL-X 
        toggleTrapTile();
        break;
    //	case KEYCODE_Z: //CTRL-Z, toggle god mode
    //		toggleGodMode();
    //		break;	
      case KeyCode.KEYCODE_S: //CTRL-S, toggle sound 
        if( (soundOff ^= 1) == 1) {
          soundStop(soundDig);
          soundStop(soundFall);
          showTipsText("SOUND OFF", 1500);
        } else {
          showTipsText("SOUND ON", 1500);
        }
        soundIconObj.updateSoundImage(); //toggle sound On/Off icon	
        break;	
      case KeyCode.KEYCODE_LEFT: //SHIFT + <- : speed down
        setSpeed(-1);	
        break;	
      case KeyCode.KEYCODE_RIGHT: //SHIFT + -> : speed up
        setSpeed(1);	
        break;
      case KeyCode.KEYCODE_H:	//CTRL-H : redHat mode on/off
        toggleRedhatMode();
        break;
      case KeyCode.KEYCODE_1: //CTRL-1
      case KeyCode.KEYCODE_2: //CTRL-2
      case KeyCode.KEYCODE_3: //CTRL-3
      case KeyCode.KEYCODE_4: //CTRL-4
      case KeyCode.KEYCODE_5: //CTRL-5
        themeColorChange(code - KEYCODE_1);
        break;	
      }
    }

    public void debugKeyPress(code)
    {
      switch(code) {
      case KeyCode.KEYCODE_1: //SHIFT-1 , add 5 level
        shiftLevelNum = 5;	
        gameState = GameState.GAME_NEXT_LEVEL;
        break;	
      case KeyCode.KEYCODE_2: //SHIFT-2 , add 10 level
        shiftLevelNum = 10;	
        gameState = GameState.GAME_NEXT_LEVEL;
        break;	
      case KeyCode.KEYCODE_3: //SHIFT-3 , add 20 level
        shiftLevelNum = 20;	
        gameState = GameState.GAME_NEXT_LEVEL;
        break;	
      case KeyCode.KEYCODE_4: //SHIFT-4 , add 50 level
        shiftLevelNum = 50;	
        gameState = GameState.GAME_NEXT_LEVEL;
        break;	
      case KeyCode.KEYCODE_6: //SHIFT-6 , dec 5 level
        shiftLevelNum = 5;	
        gameState = GameState.GAME_PREV_LEVEL;
        break;	
      case KeyCode.KEYCODE_7: //SHIFT-7 , dec 10 level
        shiftLevelNum = 10;	
        gameState = GameState.GAME_PREV_LEVEL;
        break;	
      case KeyCode.KEYCODE_8: //SHIFT-8 , dec 20 level
        shiftLevelNum = 20;	
        gameState = GameState.GAME_PREV_LEVEL;
        break;	
      case KeyCode.KEYCODE_9: //SHIFT-9 , dec 50 level
        shiftLevelNum = 50;	
        gameState = GameState.GAME_PREV_LEVEL;
        break;
      }
    }

    public int repeatAction = 0;  //1: keyboard repeat on, 0: keyboard repeat Off
    public int repeatActionPressed = 0;
    public int gamepadMode = 1; //0: disable, 1: enable
    public int redhatMode = 1;
    public int godMode = 0, godModeKeyPressed = 0;

    public void initHotKeyVariable()
    {
      godMode = 0;
      godModeKeyPressed = 0;
      repeatActionPressed = 0;
    }

    public void toggleRepeatAction()
    {
      if( (repeatAction ^= 1) == 1) {
        showTipsText("REPEAT ACTIONS ON", 2500);
      } else {
        showTipsText("REPEAT ACTIONS OFF", 2500);
      }
      if(gameState != GAME_START) repeatActionPressed=1; //player change the "repeatAction" Mode at running
      
      setRepeatAction();
    }

    public void toggleGamepadMode(textMsg)
    {
      if(!gamepadSupport()) {
        if(textMsg) showTipsText("GAMEPAD NOT SUPPORTED", 2500);
        gamepadMode = 0;
      } else {
        if( (gamepadMode ^= 1) == 1) {
          gamepadEnable();
          if(textMsg) showTipsText("GAMEPAD ON", 2500);
        } else {
          gamepadDisable();
          if(textMsg) showTipsText("GAMEPAD OFF", 2500);
        }
      }
      setGamepadMode();
    }

    public void toggleRedhatMode()
    {
      if( (redhatMode ^= 1) == 1 ) { //enable
        for(var i = 0; i < guardCount; i++) {
          if(guard[i].hasGold > 0)
            guard[i].sprite.spriteSheet = redhatData;
          else	
            guard[i].sprite.spriteSheet = guardData;
        }
        showTipsText("REDHAT MODE ON", 1500);
      } else { //disable
        for(var i = 0; i < guardCount; i++) {
            guard[i].sprite.spriteSheet = guardData;
        }
        showTipsText("REDHAT MODE OFF", 1500);
      }
    }


    public void toggleGodMode()
    {
      godModeKeyPressed = 1; //means player press the god-mod hot-key
      sometimePlayInGodMode = 1; // 12/23/2014 
      
      godMode ^= 1;
      if(godMode) {
        showTipsText("GOD MODE ON", 1500);
      } else {	
        showTipsText("GOD MODE OFF", 1500);
      }
    }

    public void setSpeed(v)
    {
      speed += v;
      if(speed < 0) speed = 0;
      if(speed >= speedMode.length) speed = speedMode.length-1;
      createjs.Ticker.setFPS(speedMode[speed]);
      showTipsText(speedText[speed], 1500);
    }

    public void helpCallBack() //help complete call back
    {
      pressKey(KEYCODE_ESC);
    }

    public void pressKey(code)
    {
      switch(code) {
      case KeyCode.KEYCODE_LEFT:        
      case KeyCode.KEYCODE_J:	
      case KeyCode.KEYCODE_A:		
        keyAction = Action.ACT_LEFT;
        break;
      case KeyCode.KEYCODE_RIGHT: 
      case KeyCode.KEYCODE_L:		
      case KeyCode.KEYCODE_D:
        keyAction = Action.ACT_RIGHT;
        break;
      case KeyCode.KEYCODE_UP:
      case KeyCode.KEYCODE_I:
      case KeyCode.KEYCODE_W:
        keyAction = Action.ACT_UP;
        break;
      case KeyCode.KEYCODE_DOWN: 
      case KeyCode.KEYCODE_K:
      case KeyCode.KEYCODE_S:
        keyAction = Action.ACT_DOWN;
        break;
      case KeyCode.KEYCODE_Z:
      case KeyCode.KEYCODE_U:	
      case KeyCode.KEYCODE_Q:		
      case KeyCode.KEYCODE_COMMA: //,
        keyAction = Action.ACT_DIG_LEFT;
        break;	
      case KeyCode.KEYCODE_X:
      case KeyCode.KEYCODE_O:
      case KeyCode.KEYCODE_E:		
      case KeyCode.KEYCODE_PERIOD: //.
        keyAction = Action.ACT_DIG_RIGHT;
        break;	
      case KeyCode.KEYCODE_ESC: //help & pause
        if(gameState == GameState.GAME_PAUSE) {
          gameResume();
          showTipsText("", 1000); //clear text
        } else {
          gamePause();
          showTipsText("PAUSE", 0); //display "PAUSE"
          //helpObj.showHelp(helpCallBack);
        }
        break;
      case KeyCode.KEYCODE_ENTER: //display hi-score
        if(playMode == GameState.PLAY_CLASSIC) {
          menuIconDisable(1);
          gamePause();
          showScoreTable(playData, null, function() { menuIconEnable(); gameResume();});	
        } else {
          keyAction = Action.ACT_UNKNOWN;
        }
        break;	
      default:
        keyAction = Action.ACT_UNKNOWN;
        //debug("keycode = " + code);	
        break;	
      }
      if(recordMode && code != KeyCode.KEYCODE_ESC) saveKeyCode(code, keyAction);
    }

    public void gameResume()
    {
      gameState = lastGameState;
      soundResume(soundFall);
      soundResume(soundDig);
    }

    public void gamePause()
    {
      lastGameState = gameState;	
      gameState = GameState.GAME_PAUSE;
      soundPause(soundFall);
      soundPause(soundDig);
    }

    public bool handleKeyDown(Event @event) 
    {
      if(@event.shiftKey) {
        if(gameState == GameState.GAME_START || gameState == GameState.GAME_RUNNING) {
          pressShiftKey(@event.keyCode);
        }
      } else 
      if (@event.ctrlKey) {
        if(gameState == GameState.GAME_START || gameState == GameState.GAME_RUNNING) {
          pressCtrlKey(@event.keyCode);
        }
      } else {
        if((gameState == GameState.GAME_PAUSE && @event.keyCode == KeyCode.KEYCODE_ESC) ||
              gameState == GameState.GAME_START || gameState == GameState.GAME_RUNNING) 
        {
          if(recordMode != PlayMode.RECORD_PLAY && playMode != PlayMode.PLAY_AUTO) {
            pressKey(@event.keyCode);
          }
        }
        
      }
      
      if((int)@event.keyCode >= 112 && (int)@event.keyCode <= 123) return true; //F1 ~ F12
      return false;
    }	

    public bool handleKeyUp(Event @event)
    {
      if(repeatAction) return true;
      if(recordKeyCode == @event.keyCode && keyPressed != -1) keyPressed = 0;
      return true;
    }
  }
}