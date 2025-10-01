export class NumberService {
    constructor(apiUrl = '/api/numbers') {
        this.apiUrl = apiUrl;
    }

    async getStoredNumbers() {
        try {
            const response = await fetch(this.apiUrl);
            if (!response.ok) throw new Error("Failed to get numbers");
            return await response.json();
        } catch (error) {
            console.error("Error fetching numbers:", error);
            return [];
        }
    }

    async saveNumbers(numbers) {
        try {
            const response = await fetch(this.apiUrl, {
                method: 'POST',
                headers: {
                    'Content-Type': 'application/json'
                },
                body: JSON.stringify(numbers)
            });

            if (!response.ok) throw new Error("Failed to save numbers");
        } catch (error) {
            console.error("Error saving numbers:", error);
        }
    }

    calculateSum(numbers) {
        return numbers.reduce((sum, num) => sum + num, 0);
    }
}