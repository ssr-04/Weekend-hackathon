/**
 * Defines the shape of the response from the /Auth/login and /Auth/register endpoints.
 */
export interface AuthResponse {
  accessToken: string;
  refreshToken: string;
  expiresIn?: number;
  userId: string;
}