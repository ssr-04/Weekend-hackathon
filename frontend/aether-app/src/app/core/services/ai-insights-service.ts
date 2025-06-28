import { HttpClient } from '@angular/common/http';
import { Injectable, inject } from '@angular/core';
import { Observable } from 'rxjs';
import { environment } from '../../../environments/environment';
import { AiInsight } from '../models/ai-insights.model';

@Injectable({ providedIn: 'root' })
export class AiInsightsService {
  private http = inject(HttpClient);
  private baseUrl = environment.apiBaseUrl;

  getDailyInsight(): Observable<AiInsight> {
    return this.http.get<AiInsight>(`${this.baseUrl}/AIInsights/daily`);
  }

  getMonthlyInsight(): Observable<AiInsight> {
    return this.http.get<AiInsight>(`${this.baseUrl}/AIInsights/monthly`);
  }

  getMonthlyComparison(): Observable<AiInsight> {
    return this.http.get<AiInsight>(`${this.baseUrl}/AIInsights/monthly-comparison`);
  }
}