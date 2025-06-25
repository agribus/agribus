namespace Agribus.Core.Domain.AggregatesModels.SensorDataAggregates;

record TemperatureSensorData(string SensorId, string SourceAdress, DateTime Timestamp, double Value, string Unit);

record HumiditySensorData(string SensorId, string SourceAdress, DateTime Timestamp, double Value, string Unit);

record AirPressureSensorData(string SensorId, string SourceAdress, DateTime Timestamp, double Value, string Unit);

record MotionSensorData(string SensorId, string SourceAdress, DateTime Timestamp, bool Detected);
