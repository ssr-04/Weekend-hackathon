import os
import json
import logging
from google import genai
from dotenv import load_dotenv
from models import AIExpenseItem,AIInsightResponse

# --- Configuration & Initialization ---

# Basic logging
logging.basicConfig(level=logging.INFO, format='%(asctime)s - %(levelname)s - %(message)s')
logger = logging.getLogger(__name__)

load_dotenv()
my_api_key = os.getenv("GEMINI_API_KEY")


# --- Centralized Gemini API Call Function ---

def get_gemini_response(prompt: str) -> str:
    """
    Centralized function to get a JSON response from Gemini.
    Returns an empty string on error.
    """
    try:
        # Use a model that supports JSON output mode. gemini-1.5-flash is excellent for this.
        client = genai.Client(api_key = my_api_key)

        response = client.models.generate_content(
        model="gemini-2.5-flash", contents=prompt, config={
        "response_mime_type": "application/json",
        "response_schema": AIInsightResponse,
        }
        )
        # Safely navigate the response object as per the new pattern
        if response and getattr(response, "text", None):
            return response.text.strip()
        
        logger.warning(f"Gemini returned no valid text content. Full response: {response}")
        return ""
    except Exception as e:
        logger.error(f"Error calling Gemini API: {e}")
        return ""


def _generate_and_parse_insight(prompt: str, default_error_text: str) -> str:
    """
    Private helper that calls Gemini, parses the JSON, and extracts the insight text.
    """
    raw_response = get_gemini_response(prompt)
    
    if not raw_response:
        logger.error("Received an empty response from Gemini.")
        return default_error_text
        
    try:
        # The raw_response is a JSON string, so we need to parse it.
        parsed_json = json.loads(raw_response)
        return parsed_json.get("InsightText", default_error_text)
    except json.JSONDecodeError:
        logger.error(f"Failed to decode JSON from Gemini response: {raw_response}")
        return default_error_text

# --- Public Functions (Prompts remain the same) ---

def get_daily_summary_insight(expenses: list[AIExpenseItem]) -> str:
    """
    Generates a daily summary insight using the Gemini API.
    """
    if not expenses:
        return "You had no expenses today. Great job staying on budget!"

    expense_details = "\n".join([f"- Category: {e.CategoryName}, Amount: {e.Amount:.2f}" for e in expenses])
    total_spent = sum(e.Amount for e in expenses)

    prompt = f"""
    You are a friendly and insightful personal finance analyst. 
    Your task is to provide a brief, encouraging, and helpful summary of a user's daily spending.

    The user's total spending for today is ${total_spent:.2f}.
    Here is the list of their expenses:
    {expense_details}

    Based on this data, analyze the spending pattern for today.
    - Identify the category with the highest spending.
    - Provide one practical, positive, and non-judgmental tip or observation.

    Please provide your response in the following JSON format:
    {{
      "InsightText": "A one-to-three sentence summary of the day's spending, including the highest category and a helpful tip."
    }}
    """
    
    return _generate_and_parse_insight(prompt, "Sorry, I couldn't generate a daily insight right now.")


def get_monthly_summary_insight(expenses: list[AIExpenseItem], income: float | None) -> str:
    """
    Generates a monthly summary insight using the Gemini API.
    """
    if not expenses:
        return "No expenses recorded this month. A clean slate!"

    total_spent = sum(e.Amount for e in expenses)
    income_context = f"The user has a monthly income of ${income:.2f}." if income else "The user's monthly income is not specified."

    category_spending = {}
    for e in expenses:
        category_spending[e.CategoryName] = category_spending.get(e.CategoryName, 0) + e.Amount
    
    top_categories = sorted(category_spending.items(), key=lambda item: item[1], reverse=True)
    top_categories_str = "\n".join([f"- {name}: ${amount:.2f}" for name, amount in top_categories[:3]])

    prompt = f"""
    You are a sharp and professional personal finance analyst.
    Your task is to provide a summary of a user's spending for the current month.

    Context:
    - Total spent this month: ${total_spent:.2f}
    - {income_context}
    - Top spending categories this month are:
    {top_categories_str}

    Based on this data, provide a concise analysis of the monthly spending.
    - Comment on the total spending, possibly in relation to income if provided.
    - Point out the most significant spending area.
    - Offer a forward-looking piece of advice or question for the user to consider for the rest of the month.

    Please provide your response in the following JSON format:
    {{
      "InsightText": "A two-to-four sentence professional analysis of the monthly spending."
    }}
    """
    return _generate_and_parse_insight(prompt, "Sorry, I couldn't generate a monthly insight right now.")


def get_monthly_comparison_insight(current_expenses: list, prev_expenses: list) -> str:
    """
    Generates a month-over-month comparison insight using the Gemini API.
    """
    current_total = sum(e.Amount for e in current_expenses)
    prev_total = sum(e.Amount for e in prev_expenses)
    
    if prev_total == 0:
        return f"You've spent ${current_total:.2f} this month. We don't have data from the previous month to create a detailed comparison, but keep tracking to unlock this feature next month!"

    prompt = f"""
    You are a data-driven financial analyst.
    Your task is to compare a user's spending between the current month and the previous month and provide a clear, insightful summary.

    Financial Data:
    - Current Month Total Spending: ${current_total:.2f}
    - Previous Month Total Spending: ${prev_total:.2f}

    Based on this data, perform a comparison and generate an insight.
    - Calculate the absolute difference in spending.
    - Calculate the percentage change.
    - Clearly state whether spending went up or down.
    - Offer a brief, neutral commentary on the trend. For example, if spending went up, you might mention "This could be due to one-time purchases or a shift in habits."

    Please provide your response in the following JSON format:
    {{
      "InsightText": "A concise, data-driven comparison of the two months' spending."
    }}
    """
    return _generate_and_parse_insight(prompt, "Sorry, I couldn't generate a monthly comparison right now.")



#print(get_gemini_response("Hello"))
