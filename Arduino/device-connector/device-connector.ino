//set Pin for devices
#define PWM_PIN_EMS_1 4
#define PWM_PIN_EMS_2 7
#define PWM_PIN_EMS_3 8
#define PWM_PIN_EMS_4 12
#define PWM_PIN_VIB_1 3
#define PWM_PIN_VIB_2 5
#define PWM_PIN_VIB_3 9
#define PWM_PIN_VIB_4 10
#define SERIAL_BAUDRATE 9600

void setup() {
  //Bitrate per sec
  Serial.begin(SERIAL_BAUDRATE);
  Serial.setTimeout(2000);
  //Opens Pin which causes that the EMS starts
  pinMode(PWM_PIN_EMS_1, OUTPUT);
  pinMode(PWM_PIN_VIB_1, OUTPUT);
  pinMode(PWM_PIN_EMS_2, OUTPUT);
  pinMode(PWM_PIN_VIB_2, OUTPUT);
  pinMode(PWM_PIN_EMS_3, OUTPUT);
  pinMode(PWM_PIN_VIB_3, OUTPUT);
  pinMode(PWM_PIN_EMS_4, OUTPUT);
  pinMode(PWM_PIN_VIB_4, OUTPUT);
  pinMode(LED_BUILTIN, OUTPUT);
  
  //Test connection. Set pin to high to activate the vibration motor
  //digitalWrite(PWM_PIN_VIB_1, HIGH);
  //digitalWrite(PWM_PIN_VIB_2, HIGH);
  //digitalWrite(PWM_PIN_VIB_3, HIGH);
  //digitalWrite(PWM_PIN_VIB_4, HIGH);
  //delay(2000);
  // Set pin to high to close connection to EMS
  digitalWrite(PWM_PIN_EMS_1, HIGH);
  // Set pin low to make sure vibration motor is off
  digitalWrite(PWM_PIN_VIB_1, LOW);
  // Set pin to high to close connection to EMS
  digitalWrite(PWM_PIN_EMS_2, HIGH);
  // Set pin low to make sure vibration motor is off
  digitalWrite(PWM_PIN_VIB_2, LOW);
  // Set pin to high to close connection to EMS
  digitalWrite(PWM_PIN_EMS_3, HIGH);
  // Set pin low to make sure vibration motor is off
  digitalWrite(PWM_PIN_VIB_3, LOW);
  // Set pin to high to close connection to EMS
  digitalWrite(PWM_PIN_EMS_4, HIGH);
  // Set pin low to make sure vibration motor is off
  digitalWrite(PWM_PIN_VIB_4, LOW);
}

void loop() {
  //Check if programm is calling this skript
  if(!Serial.available()) {
    return;
  }
  // receive data from c# code
  int value = Serial.parseInt();
  value = max(min(value, 254), 0);
  // Check which device should be used
  if(value == 1) {
    //start EMS
     digitalWrite(PWM_PIN_EMS_1, LOW);
  }
  else if (value == 2){
    digitalWrite(PWM_PIN_EMS_2, LOW);
  }
  else if (value == 3){
    digitalWrite(PWM_PIN_EMS_3, LOW);
  }
  else if (value == 4){
    digitalWrite(PWM_PIN_EMS_4, LOW);
  }
  else if (value == 5){
    //start vibration motor
    digitalWrite(PWM_PIN_VIB_1, HIGH);
  }
  else if (value == 6){
    digitalWrite(PWM_PIN_VIB_2, HIGH);
  }
  else if (value == 7){
    digitalWrite(PWM_PIN_VIB_3, HIGH);
  }
  else if (value == 8){
    digitalWrite(PWM_PIN_VIB_4, HIGH);
  }
  else {
    //end all connections
    digitalWrite(PWM_PIN_EMS_1, HIGH); 
    digitalWrite(PWM_PIN_VIB_1, LOW); 
    digitalWrite(PWM_PIN_EMS_2, HIGH); 
    digitalWrite(PWM_PIN_VIB_2, LOW); 
    digitalWrite(PWM_PIN_EMS_3, HIGH); 
    digitalWrite(PWM_PIN_VIB_3, LOW); 
    digitalWrite(PWM_PIN_EMS_4, HIGH); 
    digitalWrite(PWM_PIN_VIB_4, LOW); 
  }
}
