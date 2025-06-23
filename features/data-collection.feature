Feature: Sensor Data Collection
  As a greenhouse owner
  I want automatic data collection from sensors
  So I can monitor conditions without manual checks

  Scenario: Collect RuuviTag sensor data
    Given RuuviTag sensors are deployed in the greenhouse
    When the Raspberry Pi gateway is online
    Then temperature, humidity, and presence data should be collected every X minutes
    And data should be transmitted to the central database
