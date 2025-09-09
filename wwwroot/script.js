// Weather App JavaScript
class WeatherApp {
  constructor() {
    this.apiBaseUrl = "/api/weather";
    this.searchHistory = this.loadSearchHistory();
    this.initializeEventListeners();
    this.renderSearchHistory();
  }

  initializeEventListeners() {
    const searchBtn = document.getElementById("searchBtn");
    const cityInput = document.getElementById("cityInput");

    searchBtn.addEventListener("click", () => this.handleSearch());

    cityInput.addEventListener("keypress", (e) => {
      if (e.key === "Enter") {
        this.handleSearch();
      }
    });

    // Clear error message when user starts typing
    cityInput.addEventListener("input", () => {
      this.hideError();
    });
  }

  async handleSearch() {
    const cityInput = document.getElementById("cityInput");
    const city = cityInput.value.trim();

    if (!city) {
      this.showError("Please enter a city name");
      return;
    }

    this.showLoading();
    this.hideError();

    try {
      const weatherData = await this.fetchWeatherData(city);
      this.displayWeather(weatherData);
      this.addToSearchHistory(city);
    } catch (error) {
      console.error("Error fetching weather data:", error);
      this.showError(
        error.message || "Failed to fetch weather data. Please try again."
      );
    } finally {
      this.hideLoading();
    }
  }

  async fetchWeatherData(city) {
    try {
      const response = await fetch(
        `${this.apiBaseUrl}/${encodeURIComponent(city)}`
      );

      if (!response.ok) {
        const errorData = await response.json().catch(() => ({}));
        throw new Error(
          errorData.message || `HTTP error! status: ${response.status}`
        );
      }

      const data = await response.json();
      return data;
    } catch (error) {
      if (error.name === "TypeError" && error.message.includes("fetch")) {
        throw new Error(
          "Network error. Please check your connection and try again."
        );
      }
      throw error;
    }
  }

  displayWeather(weatherData) {
    const weatherDisplay = document.getElementById("weatherDisplay");

    const weatherCard = document.createElement("div");
    weatherCard.className = "weather-card";
    weatherCard.innerHTML = `
            <div class="weather-location">${weatherData.location}</div>
            <div class="weather-temp">${Math.round(
              weatherData.temperature
            )}°C</div>
            <div class="weather-description">${weatherData.description}</div>
            <div class="weather-details">
                <div class="weather-detail">
                    <div class="weather-detail-label">Humidity</div>
                    <div class="weather-detail-value">${
                      weatherData.humidity
                    }%</div>
                </div>
                <div class="weather-detail">
                    <div class="weather-detail-label">Wind Speed</div>
                    <div class="weather-detail-value">${
                      weatherData.windSpeed
                    } m/s</div>
                </div>
            </div>
            <div class="weather-timestamp">
                Last updated: ${new Date(
                  weatherData.timestamp
                ).toLocaleString()}
            </div>
        `;

    weatherDisplay.innerHTML = "";
    weatherDisplay.appendChild(weatherCard);
  }

  showLoading() {
    const weatherDisplay = document.getElementById("weatherDisplay");
    const searchBtn = document.getElementById("searchBtn");

    weatherDisplay.innerHTML = `
            <div class="loading">
                <div class="spinner"></div>
            </div>
        `;

    searchBtn.disabled = true;
    searchBtn.textContent = "Loading...";
  }

  hideLoading() {
    const searchBtn = document.getElementById("searchBtn");
    searchBtn.disabled = false;
    searchBtn.textContent = "Get Weather";
  }

  showError(message) {
    const errorMessage = document.getElementById("errorMessage");
    errorMessage.textContent = message;
    errorMessage.style.display = "block";
  }

  hideError() {
    const errorMessage = document.getElementById("errorMessage");
    errorMessage.style.display = "none";
  }

  addToSearchHistory(city) {
    this.searchHistory = this.searchHistory.filter(
      (item) => item.toLowerCase() !== city.toLowerCase()
    );

    // Add to beginning of array
    this.searchHistory.unshift(city);

    // Keep only last 10 searches
    this.searchHistory = this.searchHistory.slice(0, 10);

    this.saveSearchHistory();
    this.renderSearchHistory();
  }

  renderSearchHistory() {
    const searchHistory = document.getElementById("searchHistory");

    if (this.searchHistory.length === 0) {
      searchHistory.innerHTML =
        '<p style="color: #666; font-style: italic;">No recent searches</p>';
      return;
    }

    searchHistory.innerHTML = this.searchHistory
      .map(
        (city) => `
                <span class="search-history-item" onclick="weatherApp.searchFromHistory('${city}')">
                    ${city}
                </span>
            `
      )
      .join("");
  }

  searchFromHistory(city) {
    const cityInput = document.getElementById("cityInput");
    cityInput.value = city;
    this.handleSearch();
  }

  loadSearchHistory() {
    try {
      const saved = localStorage.getItem("weatherSearchHistory");
      return saved ? JSON.parse(saved) : [];
    } catch (error) {
      console.error("Error loading search history:", error);
      return [];
    }
  }

  saveSearchHistory() {
    try {
      localStorage.setItem(
        "weatherSearchHistory",
        JSON.stringify(this.searchHistory)
      );
    } catch (error) {
      console.error("Error saving search history:", error);
    }
  }

  clearSearchHistory() {
    this.searchHistory = [];
    this.saveSearchHistory();
    this.renderSearchHistory();
  }
}

document.addEventListener("DOMContentLoaded", () => {
  window.weatherApp = new WeatherApp();
});

// Add some utility functions for better user experience
document.addEventListener("DOMContentLoaded", () => {
  // Add keyboard shortcuts
  document.addEventListener("keydown", (e) => {
    // Ctrl/Cmd + K to focus search input
    if ((e.ctrlKey || e.metaKey) && e.key === "k") {
      e.preventDefault();
      document.getElementById("cityInput").focus();
    }
  });

  document.addEventListener("click", (e) => {
    if (!e.target.closest(".search-section")) {
      window.weatherApp?.hideError();
    }
  });
});
