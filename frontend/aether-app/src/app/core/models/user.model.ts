/**
 * Defines the shape of the user profile object from the /Users/profile endpoint.
 */
export interface UserProfile {
  id: string;
  name: string;
  email: string;
  monthlyIncome: number | null;
  numberOfDependents: number | null;
  financialGoals: string | null;
}