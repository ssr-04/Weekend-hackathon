from flask import Flask, request, jsonify
from pydantic import ValidationError
from models import AIRequest, AIInsightResponse
from flask_cors import CORS

from gemini_api import (
    get_daily_summary_insight, 
    get_monthly_summary_insight, 
    get_monthly_comparison_insight
)

# Initialize the Flask app
app = Flask(__name__)

CORS(app) 


def generate_daily_summary(expenses: list) -> str:
    if not expenses:
        return "You had no expenses today. Great job staying on budget!"
    
    total_spent = sum(e.Amount for e in expenses)
    highest_expense = max(expenses, key=lambda e: e.Amount)
    
    return (f"Today, you spent a total of ${total_spent:.2f}. "
            f"Your largest expense was ${highest_expense.Amount:.2f} on '{highest_expense.CategoryName}'.")

def generate_monthly_summary(expenses: list, income: float or None) -> str:
    if not expenses:
        return "No expenses recorded this month. You're either incredibly frugal or just getting started!"
        
    total_spent = sum(e.Amount for e in expenses)
    insight = f"This month, you've spent a total of ${total_spent:.2f}. And today the date is {Date.Now()}"
    
    if income and income > 0:
        percentage_of_income = (total_spent / income) * 100
        insight += f"This represents {percentage_of_income:.1f}% of your monthly income. "
        if percentage_of_income > 80:
            insight += "Keep a close eye on your spending!"
        else:
            insight += "You are managing your income well."
            
    return insight

def generate_monthly_comparison(current_expenses: list, prev_expenses: list) -> str:
    current_total = sum(e.Amount for e in current_expenses)
    prev_total = sum(e.Amount for e in prev_expenses)
    
    if prev_total == 0:
        return f"You've spent ${current_total:.2f} this month. We have no data from the previous month to compare against."

    difference = current_total - prev_total
    percentage_change = (difference / prev_total) * 100
    
    comparison_text = "higher" if difference > 0 else "lower"
    
    return (f"Your spending this month is ${abs(difference):.2f} ({percentage_change:.1f}%) {comparison_text} "
            f"than last month (Current: ${current_total:.2f} vs. Previous: ${prev_total:.2f}).")


# --- The Main API Endpoint ---

@app.route('/insights/generate', methods=['POST'])
def generate_insights():
    """
    Receives expense data and generates an AI-driven textual insight
    by calling the appropriate helper function.
    """
    try:
        request_data = AIRequest.model_validate(request.json)
    except ValidationError as e:
        return jsonify({"error": "Invalid request payload", "details": e.errors()}), 400

    insight_text = "Invalid request type specified."

    # --- Simplified Logic: Route to the correct helper function ---
    if request_data.RequestType == "DailySummary":
        insight_text = get_daily_summary_insight(request_data.Expenses)
    
    elif request_data.RequestType == "MonthlySummary":
        insight_text = get_monthly_summary_insight(request_data.Expenses, request_data.MonthlyIncome)
        
    elif request_data.RequestType == "MonthlyComparison":
        insight_text = get_monthly_comparison_insight(request_data.Expenses, request_data.ComparisonExpenses or [])

    # Create the response object
    response_data = AIInsightResponse(InsightText=insight_text)
    
    # Check for errors returned from the gemini_api module
    if "error" in insight_text.lower():
         return jsonify({"error": insight_text}), 500

    return jsonify(response_data.model_dump()), 200


if __name__ == '__main__':
    app.run(debug=True, port=8000)