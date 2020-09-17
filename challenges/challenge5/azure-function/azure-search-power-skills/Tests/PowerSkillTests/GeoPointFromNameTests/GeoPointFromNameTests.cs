// Copyright (c) Microsoft. All rights reserved.  
// Licensed under the MIT License. See LICENSE file in the project root for full license information.  

using AzureCognitiveSearch.PowerSkills.Common;
using AzureCognitiveSearch.PowerSkills.Geo.GeoPointFromName;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Net.Http;
using System.Threading.Tasks;

namespace AzureCognitiveSearch.PowerSkills.Tests.GeoPointFromNameTests
{

    [TestClass]
    public class GeoPointFromNameTests
    {
        [ClassInitialize]
        public static void Setup(TestContext context)
        {
            WebApiSkillHelpers.TestMode = true;
            WebApiSkillHelpers.TestWww = req =>
            {
                string query = req.RequestUri.ParseQueryString()["q"];
                JObject coordinates = JObject.Parse(query);
                object responseBody = new
                {
                    resourceSets = new object[] {
                        new
                        {
                            resources = new object[]
                            {
                                new 
                                {
                                    point = new Geography()
                                    {
                                        Coordinates = new double[] { (double)coordinates["Y"], (double)coordinates["X"] }
                                    }
                                }
                            }
                        }
                    }
                };
                return req.RespondRequestWith(responseBody);
            };
        }

        [TestMethod]
        public async Task GetCoordinatesFromPlaceName()
        {
            object mainGeoPoint = await Helpers.QuerySkill(
                Geo.GeoPointFromName.GeoPointFromName.RunGeoPointFromName,
                new { Address = JsonConvert.SerializeObject(new { X = 50, Y = 100 }) },
                "mainGeoPoint");
            var coordinates = mainGeoPoint.GetProperty<double[]>("Coordinates");
            Assert.IsNotNull(coordinates);
            Assert.AreEqual(100, coordinates[0]);
            Assert.AreEqual(50, coordinates[1]);
        }
    }
}
