# ForecastHourly - Documentation

Ce modèle représente les prévisions horaires récupérées depuis l’API **Open-Meteo**.

## Propriétés et unités

| Propriété                 | Type       | Unité / Format | Description |
|----------------------------|------------|----------------|-------------|
| `Time`                     | `DateTime` | `iso8601`      | Horodatage de la mesure au format ISO 8601. |
| `Temperature`              | `float?`   | `°C`           | Température de l’air à 2 mètres du sol. |
| `Humidity`                 | `int?`     | `%`            | Humidité relative à 2 mètres du sol. |
| `WeatherCode`              | `enum`     | -              | Code météo représentant les conditions actuelles (voir table des valeurs ci-dessous). |
| `Pressure`                 | `float?`   | `hPa` (supposé) | Pression atmosphérique. *(non présent dans l’unité Open-Meteo mais généralement exprimé en hectopascals)* |
| `PrecipitationProbability` | `int?`     | `%`            | Probabilité de précipitations. |
| `Precipitation`            | `float?`   | `mm`           | Quantité totale de précipitations (pluie/neige) par heure. |

---

## WeatherCode (énumération)

L’énumération `WeatherCode` fournit des conditions météorologiques codées selon la convention d’Open-Meteo.

| Code | Nom                          | Description |
|------|------------------------------|-------------|
| `0`  | `ClearSky`                   | Ciel dégagé |
| `1`  | `MainlyClear`                | Peu nuageux |
| `2`  | `PartlyCloudy`               | Partiellement nuageux |
| `3`  | `Overcast`                   | Couvert |
| `45` | `Fog`                        | Brouillard |
| `48` | `DepositingRimeFog`          | Brouillard givrant |
| `51` | `LightDrizzle`               | Bruine légère |
| `53` | `ModerateDrizzle`            | Bruine modérée |
| `55` | `DenseDrizzle`               | Forte bruine |
| `56` | `LightFreezingDrizzle`       | Bruine verglaçante légère |
| `57` | `DenseFreezingDrizzle`       | Bruine verglaçante forte |
| `61` | `SlightRain`                 | Pluie faible |
| `63` | `ModerateRain`               | Pluie modérée |
| `65` | `HeavyRain`                  | Forte pluie |
| `66` | `LightFreezingRain`          | Pluie verglaçante légère |
| `67` | `HeavyFreezingRain`          | Pluie verglaçante forte |
| `71` | `SlightSnowFall`             | Faible chute de neige |
| `73` | `ModerateSnowFall`           | Chute de neige modérée |
| `75` | `HeavySnowFall`              | Forte chute de neige |
| `77` | `SnowGrains`                 | Neige en grains |
| `80` | `SlightRainShowers`          | Averses faibles |
| `81` | `ModerateRainShowers`        | Averses modérées |
| `82` | `ViolentRainShowers`         | Averses violentes |
| `85` | `SlightSnowShowers`          | Averses de neige faibles |
| `86` | `HeavySnowShowers`           | Averses de neige fortes |
| `95` | `ThunderstormSlight`         | Orage faible |
| `96` | `ThunderstormWithSlightHail` | Orage avec faible grêle |
| `99` | `ThunderstormWithHeavyHail`  | Orage avec forte grêle |

---