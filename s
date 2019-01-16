[33mcommit d50c24cce6876ca6d259ffa6d9eb5a9466369498[m
Author: MassimoC <massimo.crippa@gmail.com>
Date:   Thu Jan 10 09:57:19 2019 +0100

    Players integration tests

[1mdiff --git a/maturity-level-one/tests/Codit.IntegrationTest/PlayersTest.cs b/maturity-level-one/tests/Codit.IntegrationTest/PlayersTest.cs[m
[1mindex 11a287f..124a9c0 100644[m
[1m--- a/maturity-level-one/tests/Codit.IntegrationTest/PlayersTest.cs[m
[1m+++ b/maturity-level-one/tests/Codit.IntegrationTest/PlayersTest.cs[m
[36m@@ -9,6 +9,7 @@[m [musing Codit.LevelOne.Models;[m
 using Newtonsoft.Json;[m
 using System.Text;[m
 using System.Net.Http.Headers;[m
[32m+[m[32musing Microsoft.AspNetCore.JsonPatch;[m
 [m
 namespace Codit.IntegrationTest[m
 {[m
[36m@@ -39,7 +40,8 @@[m [mnamespace Codit.IntegrationTest[m
         [InlineData("GET")][m
         public async Task GetSinglePlayer_Ok_TestAsync(string httpMethod)[m
         {[m
[31m-            var request = new HttpRequestMessage(new HttpMethod(httpMethod), "/world-cup/v1/players/1");[m
[32m+[m[32m            int playerId = 1;[m
[32m+[m[32m            var request = new HttpRequestMessage(new HttpMethod(httpMethod), $"/world-cup/v1/players/{playerId}");[m
             var response = await _httpClient.SendAsync(request);[m
             response.EnsureSuccessStatusCode();[m
             Assert.Equal(HttpStatusCode.OK, response.StatusCode);[m
[36m@@ -50,7 +52,8 @@[m [mnamespace Codit.IntegrationTest[m
         [InlineData("GET")][m
         public async Task GetSinglePlayer_NotFound_TestAsync(string httpMethod)[m
         {[m
[31m-            var request = new HttpRequestMessage(new HttpMethod(httpMethod), "/world-cup/v1/players/-1");[m
[32m+[m[32m            int playerId = -1;[m
[32m+[m[32m            var request = new HttpRequestMessage(new HttpMethod(httpMethod), $"/world-cup/v1/players/{playerId}");[m
             var response = await _httpClient.SendAsync(request);[m
             Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);[m
         }[m
[36m@@ -67,13 +70,7 @@[m [mnamespace Codit.IntegrationTest[m
                 IsTopPlayer = false,[m
                 TeamId = 1[m
             };[m
[31m-            var data = JsonConvert.SerializeObject(player);[m
[31m-            var content = new ByteArrayContent(Encoding.UTF8.GetBytes(data));[m
[31m-            content.Headers.ContentType = new MediaTypeHeaderValue("application/json");[m
[31m-[m
[31m-            var request = new HttpRequestMessage(new HttpMethod(httpMethod), "/world-cup/v1/players");[m
[31m-            request.Content = content;[m
[31m-[m
[32m+[m[32m            var request = TestUtils.GetJsonRequest(player, httpMethod, $"/world-cup/v1/players");[m
             var response = await _httpClient.SendAsync(request);[m
             response.EnsureSuccessStatusCode();[m
             Assert.Equal(HttpStatusCode.Created, response.StatusCode);[m
[36m@@ -89,15 +86,142 @@[m [mnamespace Codit.IntegrationTest[m
                 Description = "He plays for Codit.",[m
                 IsTopPlayer = false[m
             };[m
[31m-            var data = JsonConvert.SerializeObject(player);[m
[31m-            var content = new ByteArrayContent(Encoding.UTF8.GetBytes(data));[m
[31m-            content.Headers.ContentType = new MediaTypeHeaderValue("application/json");[m
[32m+[m[32m            var request = TestUtils.GetJsonRequest(player, httpMethod, "/world-cup/v1/players");[m
[32m+[m[32m            var response = await _httpClient.SendAsync(request);[m
[32m+[m[32m            Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);[m
[32m+[m[32m        }[m
 [m
[31m-            var request = new HttpRequestMessage(new HttpMethod(httpMethod), "/world-cup/v1/players");[m
[31m-            request.Content = content;[m
[32m+[m[32m        [Theory][m
[32m+[m[32m        [InlineData("POST")][m
[32m+[m[32m        public async Task VoteAsBestPlayer_Accepted_TestAsync(string httpMethod)[m
[32m+[m[32m        {[m
[32m+[m[32m            int playerId = 1;[m
[32m+[m[32m            var request = new HttpRequestMessage(new HttpMethod(httpMethod), $"/world-cup/v1/players/{playerId}/vote");[m
[32m+[m[32m            var response = await _httpClient.SendAsync(request);[m
[32m+[m[32m            response.EnsureSuccessStatusCode();[m
[32m+[m[32m            Assert.Equal(HttpStatusCode.Accepted, response.StatusCode);[m
[32m+[m[32m        }[m
[32m+[m
[32m+[m[32m        [Theory][m
[32m+[m[32m        [InlineData("POST")][m
[32m+[m[32m        public async Task VoteAsBestPlayer_NotFound_TestAsync(string httpMethod)[m
[32m+[m[32m        {[m
[32m+[m[32m            int playerId = -1;[m
[32m+[m[32m            var request = new HttpRequestMessage(new HttpMethod(httpMethod), $"/world-cup/v1/players/{playerId}/vote");[m
[32m+[m[32m            var response = await _httpClient.SendAsync(request);[m
[32m+[m[32m            Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);[m
[32m+[m[32m        }[m
[32m+[m
[32m+[m[32m        [Theory][m
[32m+[m[32m        [InlineData("PUT")][m
[32m+[m[32m        public async Task UpdatePlayer_NoContent_TestAsync(string httpMethod)[m
[32m+[m[32m        {[m
[32m+[m[32m            int playerId = 1;[m
[32m+[m[32m            var player = new PlayerDto[m
[32m+[m[32m            {[m
[32m+[m[32m                FirstName = "Hazard",[m
[32m+[m[32m                Description = "He plays in Chelsea.",[m
[32m+[m[32m                IsTopPlayer = true,[m
[32m+[m[32m                TeamId = 2[m
[32m+[m[32m            };[m
[32m+[m
[32m+[m[32m            var request = new HttpRequestMessage(new HttpMethod("GET"), $"/world-cup/v1/players/{playerId}");[m
[32m+[m[32m            var response = await _httpClient.SendAsync(request);[m
[32m+[m[32m            var actualDto = JsonConvert.DeserializeObject<PlayerDto>(response.Content.ReadAsStringAsync().Result);[m
[32m+[m
[32m+[m[32m            request = TestUtils.GetJsonRequest(player, httpMethod, $"/world-cup/v1/players/{playerId}");[m
[32m+[m[32m            response = await _httpClient.SendAsync(request);[m
[32m+[m[32m            Assert.Equal(HttpStatusCode.NoContent, response.StatusCode);[m
[32m+[m
[32m+[m[32m            request = new HttpRequestMessage(new HttpMethod("GET"), $"/world-cup/v1/players/{playerId}");[m
[32m+[m[32m            response = await _httpClient.SendAsync(request);[m
[32m+[m[32m            var updatedDto = JsonConvert.DeserializeObject<PlayerDto>(response.Content.ReadAsStringAsync().Result);[m
[32m+[m
[32m+[m[32m            Assert.Equal(actualDto.FirstName, updatedDto.FirstName);[m
[32m+[m[32m            Assert.Equal(actualDto.Description, updatedDto.Description);[m
[32m+[m[32m            Assert.Equal(actualDto.IsTopPlayer, updatedDto.IsTopPlayer);[m
[32m+[m[32m            Assert.Equal(2, updatedDto.TeamId);[m
[32m+[m
[32m+[m[32m        }[m
[32m+[m
[32m+[m[32m        [Theory][m
[32m+[m[32m        [InlineData("PATCH")][m
[32m+[m[32m        public async Task UpdatePlayerIncremental_NoContent_TestAsync(string httpMethod)[m
[32m+[m[32m        {[m
[32m+[m[32m            int playerId = 1;[m
[32m+[m[32m            var player = new PlayerDto[m
[32m+[m[32m            {[m
[32m+[m[32m                Description = "He's still playing for Chelsea."[m
[32m+[m[32m            };[m
 [m
[32m+[m[32m            var request = new HttpRequestMessage(new HttpMethod("GET"), $"/world-cup/v1/players/{playerId}");[m
[32m+[m[32m            var response = await _httpClient.SendAsync(request);[m
[32m+[m[32m            var actualDto = JsonConvert.DeserializeObject<PlayerDto>(response.Content.ReadAsStringAsync().Result);[m
[32m+[m
[32m+[m[32m            request = TestUtils.GetJsonRequest(player, httpMethod, $"/world-cup/v1/players/{playerId}");[m
[32m+[m[32m            response = await _httpClient.SendAsync(request);[m
[32m+[m[32m            Assert.Equal(HttpStatusCode.NoContent, response.StatusCode);[m
[32m+[m
[32m+[m[32m            request = new HttpRequestMessage(new HttpMethod("GET"), $"/world-cup/v1/players/{playerId}");[m
[32m+[m[32m            response = await _httpClient.SendAsync(request);[m
[32m+[m[32m            var updatedDto = JsonConvert.DeserializeObject<PlayerDto>(response.Content.ReadAsStringAsync().Result);[m
[32m+[m
[32m+[m[32m            Assert.Equal(actualDto.FirstName, updatedDto.FirstName);[m
[32m+[m[32m            Assert.NotEqual(actualDto.Description, updatedDto.Description);[m
[32m+[m[32m            Assert.Equal(actualDto.IsTopPlayer, updatedDto.IsTopPlayer);[m
[32m+[m[32m            Assert.Equal(actualDto.TeamId, updatedDto.TeamId);[m
[32m+[m
[32m+[m[32m        }[m
[32m+[m
[32m+[m[32m        [Theory][m
[32m+[m[32m        [InlineData("PATCH")][m
[32m+[m[32m        public async Task UpdatePlayerIncrementalJsonPatch_Ok_TestAsync(string httpMethod)[m
[32m+[m[32m        {[m
[32m+[m[32m            int playerId = 1;[m
[32m+[m[32m            JsonPatchDocument<PlayerDto> player = new JsonPatchDocument<PlayerDto>();[m
[32m+[m[32m            player.Replace(p => p.Description, "He's still playing for Chelsea.");[m
[32m+[m[32m            player.Replace(p => p.IsTopPlayer, false);[m
[32m+[m
[32m+[m[32m            var request = new HttpRequestMessage(new HttpMethod("GET"), $"/world-cup/v1/players/{playerId}");[m
[32m+[m[32m            var response = await _httpClient.SendAsync(request);[m
[32m+[m[32m            var actualDto = JsonConvert.DeserializeObject<PlayerDto>(response.Content.ReadAsStringAsync().Result);[m
[32m+[m
[32m+[m[32m            request = TestUtils.GetJsonRequest(player, httpMethod, $"/world-cup/v1/players/{playerId}/update", "application/json-patch+json");[m
[32m+[m[32m            response = await _httpClient.SendAsync(request);[m
[32m+[m[32m            Assert.Equal(HttpStatusCode.OK, response.StatusCode);[m
[32m+[m
[32m+[m[32m            var updatedDto = JsonConvert.DeserializeObject<PlayerDto>(response.Content.ReadAsStringAsync().Result);[m
[32m+[m
[32m+[m[32m            Assert.Equal(actualDto.FirstName, updatedDto.FirstName);[m
[32m+[m[32m            Assert.NotEqual(actualDto.Description, updatedDto.Description);[m
[32m+[m[32m            Assert.NotEqual(actualDto.IsTopPlayer, updatedDto.IsTopPlayer);[m
[32m+[m[32m            Assert.Equal(actualDto.TeamId, updatedDto.TeamId);[m
[32m+[m
[32m+[m[32m        }[m
[32m+[m
[32m+[m[32m        [Theory][m
[32m+[m[32m        [InlineData("PATCH")][m
[32m+[m[32m        public async Task UpdatePlayerIncrementalJsonPatch_BadRequest_TestAsync(string httpMethod)[m
[32m+[m[32m        {[m
[32m+[m[32m            int playerId = 1;[m
[32m+[m[32m            var player = new PlayerDto[m
[32m+[m[32m            {[m
[32m+[m[32m                Description = "He's still playing for Chelsea."[m
[32m+[m[32m            };[m
[32m+[m
[32m+[m[32m            var request = TestUtils.GetJsonRequest(player, httpMethod, $"/world-cup/v1/players/{playerId}/update");[m
             var response = await _httpClient.SendAsync(request);[m
             Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);[m
         }[m
[32m+[m
[32m+[m[32m        [Theory][m
[32m+[m[32m        [InlineData("DELETE")][m
[32m+[m[32m        public async Task DeletePlayer_NotFound_TestAsync(string httpMethod)[m
[32m+[m[32m        {[m
[32m+[m[32m            int playerId = 1;[m
[32m+[m[32m            var request = new HttpRequestMessage(new HttpMethod(httpMethod), $"/world-cup/v1/players/{playerId}");[m
[32m+[m[32m            var response = await _httpClient.SendAsync(request);[m
[32m+[m[32m            Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);[m
[32m+[m[32m        }[m
     }[m
 }[m
[1mdiff --git a/maturity-level-one/tests/Codit.IntegrationTest/TestUtils.cs b/maturity-level-one/tests/Codit.IntegrationTest/TestUtils.cs[m
[1mnew file mode 100644[m
[1mindex 0000000..ca1022b[m
[1m--- /dev/null[m
[1m+++ b/maturity-level-one/tests/Codit.IntegrationTest/TestUtils.cs[m
[36m@@ -0,0 +1,24 @@[m
[32m+[m[32m﻿using Newtonsoft.Json;[m
[32m+[m[32musing System;[m
[32m+[m[32musing System.Collections.Generic;[m
[32m+[m[32musing System.Net.Http;[m
[32m+[m[32musing System.Net.Http.Headers;[m
[32m+[m[32musing System.Text;[m
[32m+[m
[32m+[m[32mnamespace Codit.IntegrationTest[m
[32m+[m[32m{[m
[32m+[m[32m    public static class TestUtils[m
[32m+[m[32m    {[m
[32m+[m[32m        public static HttpRequestMessage GetJsonRequest(object data, string method, string requestUri, string contentType = "application/json")[m
[32m+[m[32m        {[m
[32m+[m[32m            var serializedData = JsonConvert.SerializeObject(data);[m
[32m+[m[32m            var content = new ByteArrayContent(Encoding.UTF8.GetBytes(serializedData));[m
[32m+[m[32m            content.Headers.ContentType = new MediaTypeHeaderValue(contentType);[m
[32m+[m
[32m+[m[32m            var request = new HttpRequestMessage(new HttpMethod(method), requestUri);[m
[32m+[m[32m            request.Content = content;[m
[32m+[m
[32m+[m[32m            return request;[m
[32m+[m[32m        }[m
[32m+[m[32m    }[m
[32m+[m[32m}[m
