+-------------------------------------------------------------+
|                                                             |
|                        SVM.NET 1.6.3                        |
|             Copyright © 2009 by Matthew Johnson             |
|                   Adapted from libsvm 2.89                  |
|       http://www.matthewajohnson.org/software/svm.html      |
|                                                             |
+-------------------------------------------------------------+

Thank you for downloading this .NET version of the libsvm
library.  Full documentation of the library is found in
the included help document.  To get started quickly, however,
here is a sample of how to perform learning on the sample
data provided for download with this library in C# 
(for more data in the libsvm format, please go to 
http://www.csie.ntu.edu.tw/~cjlin/libsvmtools/datasets/.

// First, read in the training and test data
Problem train = Problem.Read("a1a.train");
Problem test = Problem.Read("a1a.test");
// For this example (and indeed, many scenarios), the default 
// parameters will suffice.
Parameter parameters = new Parameter();

double C;
double Gamma;
// This will do a grid optimization to find the best parameters
// and store them in C and Gamma, outputting the entire
// search to params.txt.
ParameterSelection.Grid(
        train, 
        parameters, 
        "params.txt", 
        out C, 
        out Gamma 
);
parameters.C = C; parameters.Gamma = Gamma;

// Train the model using the optimal parameters.
Model model = Training.Train(train, parameters);
// Perform classification on the test data, putting the
// results in results.txt.
Prediction.Predict(test, "results.txt", model, false);


==================== Revision Information ====================

Version 1.6.3 (9/22/2009)
-------------------------
* Fixed culture invariance so it doesn't permanently override
  the existing culture.
* Fixed an off-by-one bug in GaussianTransform.Transform()

Version 1.6.2 (9/9/2009)
-------------------------
* Added culture invariance to the Write() and Read() methods

Version 1.6.1 (8/9/2009)
-------------------------
* Fixed minor bug where passing a null argument for outputFile into
  ParameterSelection.Grid() resulted in a error.

Version 1.6 (7/12/2009)
-----------------------
* Fixed major bug in Kernel.cs with computeSquaredDistance()
* Fixed major bug in svm_predict_probability where a two 
  dimensional array was being intialized incorrectly.
* Changed the way in which Scaling works.  RangeTransform and
  GaussianTransform both have a Compute() method, which will
  compute the transform from a problem.  They also have a 
  Scale() method, which will scale a problem using that
  IRangeTransform object.

Version 1.5 (6/7/2009)
----------------------
* Updated to libsvm 2.89
* Optimized Cache class and Kernel methods
* Fixed bug with PerformanceEvaluator for multiple category problems

Version 1.4 (9/3/2008)
----------------------
* Added PerformanceEvaluator and RankPair classes which enable easy 
  evaluation using Precision/Recall and Receiver Operating 
  Characteristic Curves
* Added PrecomputedKernel class, which makes it easier to train SVMs 
  using custom kernels.
* Small changes to improve performance. See documentation for details.

Version 1.3 (9/17/2007)
-----------------------
* Updated to libsvm 2.84

Version 1.2 (3/27/2007)
-----------------------
* Added the ability to predict class membership and probabilities 
  for a single vector to the Prediction class.
* Added the GaussianTransform class.
* Fixed minor bugs and completed documentation.

Version 1.1 (2/19/2007)
-----------------------
* Fixed a bug when writing to file, where the existing file 
  wouldn't be completely overwritten.
* Updated documentation.

Version 1.0 (2/16/2007)
-----------------------
* First public release.
