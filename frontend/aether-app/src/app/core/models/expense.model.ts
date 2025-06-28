/**
 * Defines a single expense record.
 */
export interface Expense {
  id: string;
  title: string;
  description: string | null;
  amount: number;
  date: string; // ISO 8601 date string
  categoryId: string;
  categoryName: string;
  paymentMethod: string | null;
  createdAt: string;
}

/**
 * Defines the shape of the paginated response from the GET /Expenses endpoint.
 */
export interface PaginatedExpenses {
  items: Expense[];
  pageNumber: number;
  pageSize: number;
  totalPages: number;
  totalCount: number;
}