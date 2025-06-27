from pydantic import BaseModel
from typing import List, Optional
from datetime import datetime

# Corresponds to AIExpenseItemDto in .NET
class AIExpenseItem(BaseModel):
    Amount: float
    CategoryName: str
    Date: datetime

# Corresponds to AIRequestDto in .NET
class AIRequest(BaseModel):
    RequestType: str
    Expenses: List[AIExpenseItem]
    ComparisonExpenses: Optional[List[AIExpenseItem]] = None
    MonthlyIncome: Optional[float] = None
    NumberOfDependents: Optional[int] = None

# Corresponds to AIInsightResponseDto in .NET
class AIInsightResponse(BaseModel):
    # Period is set by the .NET service, so it's not needed here
    InsightText: str