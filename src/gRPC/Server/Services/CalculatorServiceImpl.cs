using System.Threading.Tasks;
using Grpc.Core;

namespace Server
{
    public class CalculatorServiceImpl : Calculator.CalculatorBase
    {
        public override Task<AdditionResponse> Addition(AdditionRequest request, ServerCallContext context)
        {
            AdditionResponse response = new AdditionResponse
            {
                Result = request.FirstNumber + request.SecondNumber
            };

            return Task.FromResult(response);
        }
    }
}