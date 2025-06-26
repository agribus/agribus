# 🌱 Agribus – IoT Platform for Smart Greenhouse Monitoring

## 🧪 User Stories & Features

### 🎯 Key User Stories

| Role           | Goal                                                                    |
| -------------- | ----------------------------------------------------------------------- |
| Gardener       | Monitor the greenhouse status in real time                              |
| Home user      | Receive alerts when temperature or humidity crosses critical thresholds |
| IoT enthusiast | Create custom automation rules and manage alert triggers                |
| Technician     | Get a daily summary of the greenhouse's environmental conditions        |
| Power user     | Analyze long-term environmental trends (days/weeks)                     |
| Power user     | Monitor sensor metadata (e.g., battery level)                           |

---

### 📦 Main Goals

- [ ] Automatic data collection from **RuuviTag** sensors
- [ ] **Go-based gateway** running on **Raspberry Pi**
- [ ] Structured data transmission to either a **REST API** or directly to a **PostgreSQL database**
- [ ] Historical tracking of **temperature**, **humidity**, and **pressure**
- [ ] Real-time and historical **data visualization** via interactive dashboards (Web & Mobile)
- [ ] **User-defined alert rules** (e.g., thresholds, schedules)
- [ ] Multi-channel notification system (Email, in-app notifications, Telegram/WhatsApp, SMS)
- [ ] AI-powered suggestions (e.g., “Water the plants”, “Open the greenhouse window”)
- [ ] Summary report sent by email
- [ ] **Automated data archiving** (older than 3 months) to a cold database using a scheduled Go script

---

## 📊 Dashboard Widgets (TODO)

The Agribus dashboard is modular and composed of interactive widgets. Each widget is configurable and designed to support specific user needs.

### 📈 Sensor Data Charts

Displays line graphs for environmental parameters.

- **Parameters**:

  - Target range (min / max)
  - Sensor type (temperature, humidity, pressure)
  - Greenhouse ID or name
  - Time range (e.g. 24h, 7d, custom)
  - Update frequency (e.g. real-time, every 5 min)

- **Charting Library**: [Tao.js](https://tao.js.org) or alternatives like Chart.js / Recharts

### Sensor current value

Display the value for environmental parameters

- Parameters (filter by greenhouse):

  - Sensor type (temperature, humidity, pressure)

- **Displayed Info**:
  - Current value
  - Date of last retrieval

---

### 🚨 Alert History

Shows a list of the most recent alerts triggered by the rule engine.

- **Parameters**: None (filtered by current user / greenhouse context)
- **Displayed Info**:
  - Date and time of alert
  - Sensor type and measured value
  - Triggered rule (e.g., “humidity < 20%”)

---

### 🤖 AI Recommendations

Displays AI-generated actionable suggestions based on recent sensor data and patterns.

- **Examples**:

  - “Water your plants — soil humidity has been low for 3 days”
  - “Open the window — temperature exceeds 30°C for 2 hours”

- **Parameters**:
  - Prediction interval (e.g., every 6h, daily, weekly)

---

### ☁️ Weather Forecast

Displays external weather forecast data relevant to the greenhouse’s location.

- **Parameters**:
  - Location (can be auto-detected or manually set)
- **Displayed Info**:
  - Temperature, humidity, precipitation, wind
  - Forecast for current day and upcoming 3–5 days
  - **Data Source**: OpenWeatherMap, WeatherAPI, or other API (must be credited visibly)
