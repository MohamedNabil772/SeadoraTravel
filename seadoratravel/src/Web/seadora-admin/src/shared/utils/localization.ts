/**
 * Resolves a localized string from an object that might contain translated fields.
 * Expected structure of obj:
 * {
 *   en: "English text",
 *   ar: "Arabic text",
 *   fr: "French text"
 * }
 * 
 * If the object itself is a string, it returns it directly.
 * 
 * @param field The object containing localized strings or a direct string
 * @param currentLocale The currently active locale code (e.g. 'en', 'ar')
 * @param defaultLocale Fallback locale if the currentLocale is not present
 * @returns The resolved localized string
 */
export function getLocalizedText(
  field: any,
  currentLocale: string,
  defaultLocale: string = 'en'
): string {
  if (field == null) return '';
  if (typeof field === 'string') return field;
  
  if (field[currentLocale]) {
    return field[currentLocale];
  }
  
  if (field[defaultLocale]) {
    return field[defaultLocale];
  }
  
  // Fallback to first available key
  const keys = Object.keys(field);
  if (keys.length > 0) {
    return field[keys[0]];
  }
  
  return '';
}
