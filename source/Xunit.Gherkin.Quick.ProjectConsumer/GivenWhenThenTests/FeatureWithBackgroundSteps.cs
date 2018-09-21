using System;

namespace Xunit.Gherkin.Quick.ProductConsumer
{
	[FeatureFile("./GivenWhenThenTests/FeatureWithBackground.feature")]
	public class FeatureWithBackgroundSteps : Feature
	{
		private string _orderValidator = String.Empty;

		[Given("a simple background")]
		public void SimpleBackground()
		{
			_orderValidator += "a";
		}

		[When("a sample step is run")]
		public void SampleStep()
		{
			_orderValidator += "b";
		}

		[Then("the steps are run in order")]
		public void StepsAreRunInOrder()
		{
			Assert.Equal("ab", _orderValidator);
		}
    }
}
