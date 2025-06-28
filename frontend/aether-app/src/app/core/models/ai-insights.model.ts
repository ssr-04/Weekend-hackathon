/**
 * Defines the shape of the response from all /AIInsights endpoints.
 */
export interface AiInsight {
  period: string;
  insightText: string;
}