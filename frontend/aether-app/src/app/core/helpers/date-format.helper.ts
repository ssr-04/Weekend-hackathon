import { formatDate } from '@angular/common';

/**
 * A helper class for centralized date formatting logic.
 */
export class DateFormatHelper {
  /**
   * Formats a JavaScript Date object or a date string into the specific 
   * 'dd-MM-yyyy HH:mm' format required by the backend API.
   * @param date The date to format.
   * @param endOfDay If true, sets the time to 23:59. Otherwise, it's 00:00.
   * @returns The formatted date string.
   */
  public static formatDateForApi(date: Date | string, endOfDay: boolean = false): string {
    const time = endOfDay ? '23:59' : '00:00';
    // The API seems to accept dd-MM-yyyy, let's stick to that for consistency.
    // The HH:mm part seems to be for filtering with time, which we can add later if needed.
    // For now, let's use a simpler format that covers the whole day.
    const datePart = formatDate(date, 'dd-MM-yyyy', 'en-US');
    return `${datePart} ${time}`;
  }
}