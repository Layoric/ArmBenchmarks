/*
 * Copyright 2011-2022 GatlingCorp (https://gatling.io)
 *
 * Licensed under the Apache License, Version 2.0 (the "License");
 * you may not use this file except in compliance with the License.
 * You may obtain a copy of the License at
 *
 *  http://www.apache.org/licenses/LICENSE-2.0
 *
 * Unless required by applicable law or agreed to in writing, software
 * distributed under the License is distributed on an "AS IS" BASIS,
 * WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
 * See the License for the specific language governing permissions and
 * limitations under the License.
 */

package computerdatabase;

import static io.gatling.javaapi.core.CoreDsl.*;
import static io.gatling.javaapi.http.HttpDsl.*;

import io.gatling.javaapi.core.*;
import io.gatling.javaapi.http.*;
import java.time.Duration;

public class BasicSimulation extends Simulation {

    HttpProtocolBuilder httpProtocol =
            http
                    // Here is the root for all relative URLs
                    .baseUrl("http://app")
                    // Here are the common headers
                    .acceptHeader("text/html,application/xhtml+xml,application/xml;q=0.9,*/*;q=0.8")
                    .doNotTrackHeader("1")
                    .acceptLanguageHeader("en-US,en;q=0.5")
                    .acceptEncodingHeader("gzip, deflate")
                    .userAgentHeader(
                            "Mozilla/5.0 (Macintosh; Intel Mac OS X 10.8; rv:16.0) Gecko/20100101 Firefox/16.0");

    // A scenario is a chain of requests and pauses
    ScenarioBuilder scn =
            scenario("Scenario Name")
                    .exec(http("request_1").get("/api/QueryBookings"))
                    // Note that Gatling has recorded real time pauses
                    .pause(2)
                    .exec(http("request_2").get("/bookings"))
.exec(http("request_3").get("/bookings?include=total&take=25"))
.pause(2)
.exec(http("request_4").get("/coupons?include=total&take=25"))
.pause(2)
.exec(http("request_5").get("/todos"))
.pause(2)
.exec(http("request_6").get("/hello?name=world"));
    {
        setUp(scn.injectOpen(
                nothingFor(1), // 1
                atOnceUsers(10), // 2
                rampUsersPerSec(20).to(100).during(300).randomized(),
                constantUsersPerSec(100).during(300)// 6
        )).protocols(httpProtocol);
    }
}
