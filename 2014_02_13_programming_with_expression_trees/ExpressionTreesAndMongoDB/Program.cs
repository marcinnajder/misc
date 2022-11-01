
using ExpressionTreesAndMongoDB;

MongoUtils.InitMongo(camelCaseTypes: new[] { typeof(ZipCode), typeof(Like) }, enableLogging: false);

SamplesZipCode.UseBsonApi();
SamplesZipCode.UseFluentApi();
SamplesZipCode.UseAggregateTyped();

// Samples.BasicOperators();
// Samples.AllZipCodeExamples();
// Samples.PreferenceExamples();