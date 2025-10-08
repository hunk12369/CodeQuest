#include <RobotCommands.h>
RobotCommands robot;

void setup() {

}

void loop() {
  robot.MOVEFORWARD(100); // Esto se validará
  robot.ROTATE(90);       // Esto también se validará
  // robot.GIRAR(10);     // Si este comando no existe, la validación (compilación) fallará.
}
