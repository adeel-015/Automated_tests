# Automated Tests

## Resilience Strategy
To ensure tests remain robust against UI changes (refactors, ID changes, moved elements), I utilized **Semantic Locators**.

Instead of relying on brittle attributes like `id` or `class`, the test locates elements based on their visible labels, mimicking real user behavior.

**Example:**
`//label[contains(text(), 'First Name')]/following::input[1]`

This allows the test to pass even if the underlying HTML structure or attributes change, as long as the "First Name" label remains visible.

## How to Run
1. `git clone https://github.com/adeel-015/Automated_tests/`
2. `cd Automated_tests`
3. `dotnet test`
