#include <RobotCommands.h>

RobotCommands robot;

void setup() {
  
}

void loop() {
  robot.ROTATE(90);
  robot.MVEFORWARD(3);
} 
