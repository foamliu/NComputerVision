using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NComputerVision.Learning.SVM
{
    public interface Kernel
    {
        double Compute(SparseVector x, SparseVector z);
    }


    // It was said that for text classification, linear kernel is the best choice, 
    //  because of the already-high-enough feature dimension


    // Linear
    //
    // k(x,z) = <x, z>
    //
    [Serializable()]
    public class LinearKernel : Kernel
    {
        public double Compute(SparseVector x, SparseVector z)
        {
            return SparseVector.DotProduct(x, z);
        }
    }


    // In practice, a low degree polynomial kernel or RBF kernel with a reasonable width 
    //  is a good initial try for most applications.


    // Polynomial
    // 
    // k(x,z) = (scale <x, z> + offset)^{degree}
    //
    [Serializable()]
    public class PolynomialKernel : Kernel
    {
        private double scale;
        private double offset;
        private double degree;

        public double Compute(SparseVector x, SparseVector z)
        {
            return Math.Pow(scale * SparseVector.DotProduct(x, z) + offset, degree);
        }

        public PolynomialKernel(double scale, double offset, double degree)
        {
            this.scale = scale;
            this.offset = offset;
            this.degree = degree;
        }
    }


    // Gaussian RBF kernel
    // 
    // k(x,z) = exp(-σ |x - z|^2) 
    //
    [Serializable()]
    public class GaussianRBFKernel : Kernel
    {
        private double sigma;

        public double Compute(SparseVector x, SparseVector z)
        {
            double temp;

            temp = ((SparseVector)(x - z)).Magnitude;
            return Math.Exp(-sigma * (temp * temp));
        }

        public GaussianRBFKernel(double sigma)
        {
            this.sigma = sigma;
        }
    }

    // Hyperbolic tangent kernel
    // 
    // k(x, z) = tanh(scale <x, z> + offset)
    //
    // Notice: it does not satisfy the Mercer condition on all scale and offset
    //
    [Serializable()]
    public class HyperbolicTangentKernel : Kernel
    {
        private double scale;
        private double offset;

        public double Compute(SparseVector x, SparseVector z)
        {
            return Math.Tanh(scale * SparseVector.DotProduct(x, z) + offset);
        }

        public HyperbolicTangentKernel(double scale, double offset)
        {
            this.scale = scale;
            this.offset = offset;
        }
    }

    // Laplacian kernel 
    // 
    // k(x,x') = exp(-σ |x - x'|) 
    //
    [Serializable()]
    public class LaplacianKernel : Kernel
    {
        private double sigma;

        public double Compute(SparseVector x, SparseVector z)
        {
            double temp;

            temp = ((SparseVector)(x - z)).Magnitude;
            return Math.Exp(-sigma * temp);
        }

        public LaplacianKernel(double sigma)
        {
            this.sigma = sigma;
        }
    }
}
